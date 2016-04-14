using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Tools.CSharp.Connectors;
using Tools.CSharp.Extensions;
using Tools.CSharp.Loggers;
using Tools.CSharp.Network.Tcp.DataTypes;
using Tools.CSharp.StateMachines;

namespace Tools.CSharp.Network.Tcp.Connectors
{
    //-------------------------------------------------------------------------
    public abstract class TcpQueueDataConnector<TTcpTypeValue, TValue> :
        TcpQueueDataConnector<TTcpTypeValue, TValue, IServerConnectorConnectParams>
        where TTcpTypeValue : ITcpTypeValue<TValue>, ITcpTypeConvertible, new()
    {
        #region protected
        protected sealed override TcpConnector<IServerConnectorConnectParams> CreateConnector()
        {
            return new TcpConnector(Logger);
        }
        #endregion
        protected TcpQueueDataConnector(ILogger logger = null)
           : base(DefaultReceiveMillisecondsTimeout, false, logger)
        { }
        protected TcpQueueDataConnector(bool continueWorkWhenConnectionLost, ILogger logger = null)
            : base(DefaultReceiveMillisecondsTimeout, continueWorkWhenConnectionLost, logger)
        { }
        protected TcpQueueDataConnector(int receiveMillisecondsTimeout, bool continueWorkWhenConnectionLost = false)
            : base(receiveMillisecondsTimeout, continueWorkWhenConnectionLost, null)
        { }
        protected TcpQueueDataConnector(int receiveMillisecondsTimeout = DefaultReceiveMillisecondsTimeout, bool continueWorkWhenConnectionLost = false, ILogger logger = null)
            : base(receiveMillisecondsTimeout, continueWorkWhenConnectionLost, logger)
        {
        }
    }
    //-------------------------------------------------------------------------
    public abstract class TcpQueueDataConnector<TTcpTypeValue, TValue, TConnectorConnectParams> : ITcpConnectorSendAndReceive, IConnector<TConnectorConnectParams>
        where TTcpTypeValue : ITcpTypeValue<TValue>, ITcpTypeConvertible, new()
        where TConnectorConnectParams : IConnectorConnectParams
    {
        #region private
        private readonly int _receiveMillisecondsTimeout;
        //---------------------------------------------------------------------
        private readonly TcpConnector<TConnectorConnectParams> _tcpConnector;
        private readonly ILogger _logger;
        private readonly int _headerDataLength;
        private readonly bool _continueWorkWhenConnectionLost;
        //---------------------------------------------------------------------
        private readonly BackgroundWorker _worker;
        //---------------------------------------------------------------------
        private readonly ThreadSafeQueue<ITcpTypeData> _sendDataQueue = new ThreadSafeQueue<ITcpTypeData>();
        private readonly SyncQueue<ReceivedResult> _receivedResultsQueue = new SyncQueue<ReceivedResult>();
        //---------------------------------------------------------------------
        private ReceiveDataState _receiveDataState = ReceiveDataState.LenWaiting;
        private bool _isDisposed;
        private bool _connectionLost;
        //---------------------------------------------------------------------
        private enum ReceiveDataState
        {
            /// <summary>
            /// Длина сообщения.
            /// </summary>
            LenWaiting,

            /// <summary>
            /// Сообщение.
            /// </summary>
            DataWaiting
        }
        //---------------------------------------------------------------------
        private sealed class ThreadSafeQueue<TItemType>
        {
            #region private
            private readonly Queue<TItemType> _queue = new Queue<TItemType>();
            private readonly object _locker = new object();
            #endregion
            //-----------------------------------------------------------------
            public int Length
            {
                get
                {
                    lock (_locker)
                    { return _queue.Count; }
                }
            }
            //-----------------------------------------------------------------
            public void Enqueue(TItemType item)
            {
                lock (_locker)
                {
                    _queue.Enqueue(item);
                }
            }
            public TItemType Dequeue()
            {
                lock (_locker)
                {
                    return _queue.Dequeue();
                }
            }
            //-----------------------------------------------------------------
            public void Clear()
            {
                lock (_locker)
                {
                    _queue.Clear();
                }
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private sealed class SyncQueue<TItemType> : BaseDisposable
            where TItemType : class
        {
            #region private
            private readonly Queue<TItemType> _queue = new Queue<TItemType>();
            private readonly Semaphore _semaphore = new Semaphore(0, 1);
            private readonly object _locker = new object();
            //-----------------------------------------------------------------
            private bool _dequeueBlocked
            {
                get
                {
                    lock (_locker)
                    { return _queue.Count == 0; }
                }
            }
            #endregion
            #region protected
            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (!IsDisposed && disposing)
                    {
                        _semaphore.Dispose();
                    }
                }
                finally
                { base.Dispose(disposing); }
               
            }
            #endregion
            //-----------------------------------------------------------------
            public void Enqueue(TItemType item)
            {
                lock (_locker)
                {
                    _queue.Enqueue(item);

                    if (_dequeueBlocked)
                    { _semaphore.Release(); }
                }
            }
            public TItemType Dequeue(int millisecondsTimeout)
            {
                if (_dequeueBlocked)
                {
                    if (!_semaphore.WaitOne(millisecondsTimeout, false))
                    { return null; }
                }
                return _queue.Dequeue();
            }
            //-----------------------------------------------------------------
            public void Clear()
            {
                lock (_locker)
                {
                    _queue.Clear();
                }
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private sealed class ReceivedResult
        {
            #region private
            private readonly ReceivedDataType _type;
            private readonly ITcpTypeData _data;
            private readonly Exception _exception;
            #endregion
            private ReceivedResult(ReceivedDataType type, ITcpTypeData data, Exception exception)
            {
                _type = type;
                _data = data;
                _exception = exception;
            }

            //-----------------------------------------------------------------
            public enum ReceivedDataType
            {
                Data,
                Exception,
                CloseConnection
            }
            //-----------------------------------------------------------------
            public static ReceivedResult CreateDataResult(ITcpTypeData data)
            {
                if (data == null)
                { throw new ArgumentNullException(nameof(data)); }

                return new ReceivedResult(ReceivedDataType.Data, data, null);
            }
            public static ReceivedResult CreateExceptionResult(Exception exception)
            {
                if (exception == null)
                { throw new ArgumentNullException(nameof(exception)); }

                return new ReceivedResult(ReceivedDataType.Exception, null, exception);
            }
            public static ReceivedResult CreateCloseConnectionResult()
            {
                return new ReceivedResult(ReceivedDataType.CloseConnection, null, null);
            }
            //-----------------------------------------------------------------
            public ReceivedDataType Type
            {
                get { return _type; }
            }
            public ITcpTypeData Data
            {
                get
                {
                    if (_type == ReceivedDataType.Data)
                    { return _data; }

                    throw new InvalidOperationException();
                }
            }
            public Exception Exception
            {
                get
                {
                    if (_type == ReceivedDataType.Exception)
                    { return _exception; }

                    throw new InvalidOperationException();
                }
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private void _subscribeWorkerAllEvents(bool addEvents)
        {
            if (addEvents)
            { _worker.DoWork += _WorkerOnDoWork; }
            else
            { _worker.DoWork -= _WorkerOnDoWork; }
            
        }

        private static void _WorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var owner = (TcpQueueDataConnector<TTcpTypeValue, TValue, TConnectorConnectParams>)e.Argument;
            var worker = (BackgroundWorker)sender;
            
            _ReceivedAndSendData(owner, worker);
        }
        //---------------------------------------------------------------------
        private static void _ReceivedAndSendData(TcpQueueDataConnector<TTcpTypeValue, TValue, TConnectorConnectParams> owner, BackgroundWorker worker)
        {
            //-----------------------------------------------------------------
            owner._receiveDataState = ReceiveDataState.LenWaiting;
            owner._tcpConnector.ReceiveBufferSize = owner._headerDataLength;
            //-----------------------------------------------------------------
            while (!worker.CancellationPending)
            {
                try
                {
                    var receivedData = _ReceivedData(owner);
                    var sendedData = _SendedData(owner);

                    if (receivedData || sendedData)
                    { continue; }

                    Thread.Sleep(50);
                }
                catch (TcpConnectorException exception)
                {
                    owner._connectionLost = exception.ErrorType == TcpConnectorErrorType.ConnectionLost;

                    owner._receivedResultsQueue.Enqueue(ReceivedResult.CreateExceptionResult(exception));
                    owner._logger.Log(LoggerLevel.Verbose, () => "Created exception data in queue.");
                }
            }
            //-----------------------------------------------------------------
            owner._receivedResultsQueue.Enqueue(ReceivedResult.CreateCloseConnectionResult());
            owner._logger.Log(LoggerLevel.Verbose, () => "Created close connection data in queue.");
            //-----------------------------------------------------------------
        }
        private static bool _ReceivedData(TcpQueueDataConnector<TTcpTypeValue, TValue, TConnectorConnectParams> owner)
        {
            var tcpConnector = owner._tcpConnector;
            ITcpTypeData receivedData;

            var receiveDataAvailable = tcpConnector.Receive(owner._receiveMillisecondsTimeout, out receivedData);
            if (receiveDataAvailable)
            {
                if (owner._receiveDataState == ReceiveDataState.LenWaiting)
                {
                    owner._logger.Log(LoggerLevel.Info, () => $"Receive data length: {receivedData.Dump()}");
                    var dataLength = owner.CreateDataLength();

                    try
                    {
                        var offset = 0;
                        dataLength.Write(receivedData, ref offset);
                        //-----------------------------------------------------
                        tcpConnector.ReceiveBufferSize = TcpConvert.ToInt32(dataLength);
                        owner._receiveDataState = ReceiveDataState.DataWaiting;
                    }
                    catch (Exception exception)
                    {
                        tcpConnector.ReceiveBufferSize = owner._headerDataLength;
                        owner._receiveDataState = ReceiveDataState.LenWaiting;

                        throw new TcpConnectorException(TcpConnectorAction.Receive, TcpConnectorErrorType.WrongPacket, string.Empty, exception);
                    }                    
                }
                else
                {
                    owner._logger.Log(LoggerLevel.Info, () => $"Receive data: {receivedData.Dump()}");
                    //---------------------------------------------------------
                    owner._receivedResultsQueue.Enqueue(ReceivedResult.CreateDataResult(receivedData));
                    owner._logger.Log(LoggerLevel.Verbose, () => "Created received data in queue.");
                    //---------------------------------------------------------
                    tcpConnector.ReceiveBufferSize = owner._headerDataLength;
                    owner._receiveDataState = ReceiveDataState.LenWaiting;
                }

                return true;
            }

            return false;
        }
        private static bool _SendedData(TcpQueueDataConnector<TTcpTypeValue, TValue, TConnectorConnectParams> owner)
        {
            var result = owner._sendDataQueue.Length != 0;
            if (result)
            {
                try
                {
                    var dataType = owner._sendDataQueue.Dequeue();
                    //---------------------------------------------------------
                    owner._logger.Log(LoggerLevel.Verbose, () => "Removed sent data from queue.");
                    //---------------------------------------------------------
                    var dataLength = owner.CreateDataLength();
                    TcpConvert.ToValueTcpType(dataLength, dataType.Length);
                    //---------------------------------------------------------
                    var dataTypeAndLength = new TcpTypeData(dataLength.DataLength + TcpConvert.ToInt32(dataLength));
                    //---------------------------------------------------------
                    var offset = 0;
                    dataLength.Read(dataTypeAndLength, ref offset);
                    //---------------------------------------------------------
                    dataType.CopyTo(dataTypeAndLength, offset);
                    //---------------------------------------------------------
                    owner._tcpConnector.Send(dataTypeAndLength);
                    //---------------------------------------------------------
                    owner._logger.Log(LoggerLevel.Info, () => $"Send data: {dataTypeAndLength.Dump()}");
                }
                catch(TcpConnectorException)
                { throw; }
                catch (Exception exception)
                { throw new TcpConnectorException(TcpConnectorAction.Send, TcpConnectorErrorType.Unknown, string.Empty, exception); }
            }
            return result;
        }
        //---------------------------------------------------------------------
        private void _subscribeTcpConnectorAllEvents(bool addEvents)
        {
            if (addEvents)
            {
                _tcpConnector.StateChanging += _tcpConnectorOnStateChanging;
                _tcpConnector.StateChanged += _tcpConnectorOnStateChanged;
            }
            else
            {
                _tcpConnector.StateChanging -= _tcpConnectorOnStateChanging;
                _tcpConnector.StateChanged -= _tcpConnectorOnStateChanged;
            }
        }

        private void _tcpConnectorOnStateChanging(object sender, StateEventArgs<ConnectorState> e)
        {
            StateChanging?.Invoke(this, e);
        }
        private void _tcpConnectorOnStateChanged(object sender, StateEventArgs<ConnectorState> e)
        {
            var state = e.NewState;
            //-----------------------------------------------------------------
            switch (state)
            {
                case ConnectorState.Connected:
                    {
                        _connectionLost = false;
                        _startReceivedAndSendData();
                    }
                    break;
                case ConnectorState.Disconnecting:
                    {
                        if (!_continueWorkWhenConnectionLost)
                        { _stopReceivedAndSendData(); }
                    }
                    break;
            }
            //-----------------------------------------------------------------
            StateChanged?.Invoke(this, e);
        }
        //---------------------------------------------------------------------
        private void _startReceivedAndSendData()
        {
            _clearQueues();

            if (!_worker.IsBusy)
            { _worker.RunWorkerAsync(this); }
        }
        private void _stopReceivedAndSendData()
        {
            _clearQueues();

            _worker.CancelAsync();
        }
        //---------------------------------------------------------------------
        private void _clearQueues(bool clearReceivedQueue = true, bool clearSendQueue = true)
        {
            if (clearReceivedQueue)
            {  _receivedResultsQueue.Clear(); }

            if (clearSendQueue)
            {  _sendDataQueue.Clear(); }
        }
        //---------------------------------------------------------------------
        private void _dispose(bool disposing)
        {
            try
            {
                if (!_isDisposed && disposing)
                {
                    if (_worker != null)
                    {
                        _subscribeWorkerAllEvents(false);

                        _worker.CancelAsync();

                        /*while (_worker.IsBusy)
                        { }*/

                        _worker.Dispose();
                    }

                    if (_tcpConnector != null)
                    {
                        _subscribeTcpConnectorAllEvents(false);
                        _tcpConnector.Dispose();
                    }

                    _receivedResultsQueue?.Dispose();
                }
            }
            finally
            { _isDisposed = true; }
        }
        #endregion
        #region protected
        protected abstract TTcpTypeValue CreateDataLength();
        protected abstract TcpConnector<TConnectorConnectParams> CreateConnector();
        //---------------------------------------------------------------------
        protected ILogger Logger
        {
            get { return _logger; }
        }
        #endregion
        protected TcpQueueDataConnector(ILogger logger = null)
            : this(DefaultReceiveMillisecondsTimeout, false, logger)
        { }
        protected TcpQueueDataConnector(bool continueWorkWhenConnectionLost, ILogger logger = null)
            : this(DefaultReceiveMillisecondsTimeout, continueWorkWhenConnectionLost, logger)
        { }
        protected TcpQueueDataConnector(int receiveMillisecondsTimeout, bool continueWorkWhenConnectionLost = false)
            : this(receiveMillisecondsTimeout, continueWorkWhenConnectionLost, null)
        { }
        protected TcpQueueDataConnector(int receiveMillisecondsTimeout = DefaultReceiveMillisecondsTimeout, bool continueWorkWhenConnectionLost = false, ILogger logger = null)
        {
            if (receiveMillisecondsTimeout < Timeout.Infinite)
            { throw new ArgumentOutOfRangeException(nameof(receiveMillisecondsTimeout), receiveMillisecondsTimeout.ToString(), string.Empty); }

            _receiveMillisecondsTimeout = receiveMillisecondsTimeout;
            _continueWorkWhenConnectionLost = continueWorkWhenConnectionLost;
            _logger = logger;

            _headerDataLength = new TTcpTypeValue().DataLength;

            _worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _subscribeWorkerAllEvents(true);

            _tcpConnector = CreateConnector();
            _subscribeTcpConnectorAllEvents(true);
        }
        ~TcpQueueDataConnector()
        {
            _dispose(false);
        }

        //---------------------------------------------------------------------
        public const int DefaultReceiveMillisecondsTimeout = 500;
        //---------------------------------------------------------------------
        public bool IsConnected
        {
            get { return _tcpConnector.IsConnected; }
        }
       
        public ConnectorState State
        {
            get { return _tcpConnector.State; }
        }
        public TConnectorConnectParams ConnectParamsWhereConnect
        {
            get { return _tcpConnector.ConnectParamsWhereConnect; }
        }
        public bool ConnectionLost
        {
            get { return _connectionLost; }
        }
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        //---------------------------------------------------------------------
        public void Connect(TConnectorConnectParams connectParams)
        {
            _tcpConnector.Connect(connectParams);
        }
        public void Disconnect()
        {
            _stopReceivedAndSendData();

            _tcpConnector.Disconnect();
        }
        //---------------------------------------------------------------------
        public void Send(ITcpTypeData data)
        {
            if (data == null)
            { throw new ArgumentNullException(nameof(data)); }

            if (_worker.IsBusy)
            {
                _sendDataQueue.Enqueue(data);
                _logger.Log(LoggerLevel.Verbose, () => "Add sent data in queue.");
            }
            else
            { throw new TcpConnectorException(TcpConnectorAction.Send, TcpConnectorErrorType.DeviceNotFound); }
        }
        public bool Receive(out ITcpTypeData receivedData)
        {
            return Receive(_receiveMillisecondsTimeout, out receivedData);
        }
        public bool Receive(int millisecondsTimeout, out ITcpTypeData receivedData)
        {
            receivedData = null;

            var receivedResult = _receivedResultsQueue.Dequeue(_receiveMillisecondsTimeout);
            if (receivedResult != null)
            {
                switch (receivedResult.Type)
                {
                    case ReceivedResult.ReceivedDataType.Data:
                        {
                            receivedData = receivedResult.Data;
                            _logger.Log(LoggerLevel.Verbose, () => "Remove received data from queue.");
                        }
                        break;
                    case ReceivedResult.ReceivedDataType.CloseConnection:
                        {
                            _logger.Log(LoggerLevel.Verbose, () => "Remove close connection data from queue.");
                        }
                        break;
                    case ReceivedResult.ReceivedDataType.Exception:
                        {
                            _logger.Log(LoggerLevel.Verbose, () => "Remove exception data from queue.");
                            throw receivedResult.Exception;
                        }
                }
            }

            return receivedData != null;
        }
        //---------------------------------------------------------------------
        public bool CheckState(params ConnectorState[] states)
        {
            return _tcpConnector.CheckState(states);
        }
        //---------------------------------------------------------------------
        public void Dispose()
        {
            _dispose(true);
            GC.SuppressFinalize(this);
        }
        //---------------------------------------------------------------------
        public event EventHandler<StateEventArgs<ConnectorState>> StateChanging;
        public event EventHandler<StateEventArgs<ConnectorState>> StateChanged;
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}