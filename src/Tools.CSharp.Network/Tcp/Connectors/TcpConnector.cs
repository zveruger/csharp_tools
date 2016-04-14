using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Tools.CSharp.Connectors;
using Tools.CSharp.Extensions;
using Tools.CSharp.Loggers;
using Tools.CSharp.Network.Tcp.DataTypes;

namespace Tools.CSharp.Network.Tcp.Connectors
{
    //-------------------------------------------------------------------------
    public class TcpConnector : TcpConnector<IServerConnectorConnectParams>
    {
        #region protected
        protected override void Connect(IServerConnectorConnectParams connectParams, out Socket socket)
        {
            if (connectParams == null)
            { throw new ArgumentNullException(nameof(connectParams)); }

            var client = new TcpClient { Client = { NoDelay = false } };
            var asyncResult = client.BeginConnect(connectParams.Address, connectParams.Port, null, null);

            if (asyncResult.AsyncWaitHandle.WaitOne(connectParams.MillisecondsTimeout, false))
            {
                client.EndConnect(asyncResult);

                socket = client.Client;
            }
            else
            { throw new IOException(string.Empty, new SocketException((int)SocketError.TimedOut)); }
        }
        #endregion
        public TcpConnector(ILogger logger = null)
           : base(logger)
        { }
    }
    //-------------------------------------------------------------------------
    public abstract class TcpConnector<TConnectorConnectParams> : BaseConnector<TConnectorConnectParams>, ITcpConnectorSendAndReceive
        where TConnectorConnectParams : IConnectorConnectParams
    {
        #region private
        //---------------------------------------------------------------------
        private NetworkStream _connectorStream;
        private Socket _socket;
        private byte[] _buffer;
        private int _positionInBuffer;
        //---------------------------------------------------------------------
        private static void _ExceptionHandling(Exception innerException, TcpConnectorAction action)
        {
            if (innerException is TcpConnectorException)
            { throw innerException; }

            var socketException = innerException?.Get<SocketException>();
            if (socketException != null)
            {
                /*странное исключение do nothing*/
                if (socketException.ErrorCode == 10035)
                { return; }

                switch (socketException.SocketErrorCode)
                {
                    case SocketError.AddressFamilyNotSupported:
                    case SocketError.AddressNotAvailable:
                    case SocketError.NetworkDown:
                    case SocketError.NetworkUnreachable:
                    case SocketError.TimedOut:
                    case SocketError.ConnectionRefused:
                    case SocketError.HostUnreachable:
                    case SocketError.SystemNotReady:
                    case SocketError.NoData:
                        { throw new TcpConnectorException(action, TcpConnectorErrorType.DeviceNotFound, string.Empty, innerException); }
                    default:
                        { throw new TcpConnectorException(action, TcpConnectorErrorType.ConnectionLost, string.Empty, innerException); }
                }
            }

            throw new TcpConnectorException(action, TcpConnectorErrorType.Unknown, string.Empty, innerException);
        }
        #endregion
        #region protected
        protected abstract void Connect(TConnectorConnectParams connectParams, out Socket socket);
        //---------------------------------------------------------------------
        protected override void Connecting(TConnectorConnectParams connectParams)
        {
            try
            {
                Connect(connectParams, out _socket);
                _connectorStream = new NetworkStream(_socket, true);
            }
            catch (Exception exception)
            {
                Disconnect();
                _ExceptionHandling(exception, TcpConnectorAction.Connecting);
            }
        }
        protected override void Disconnecting()
        {
            _positionInBuffer = 0;

            _connectorStream?.Close();
            _socket?.Close();
        }
        #endregion
        protected TcpConnector(ILogger logger = null)
            : base(logger)
        {
            _buffer = new byte[0];
        }

        //---------------------------------------------------------------------
        public int ReceiveBufferSize
        {
            get { return _buffer.Length; }
            set
            {
                if (value < 0)
                { throw new ArgumentOutOfRangeException(nameof(value), value.ToString(), string.Empty); }

                if (_buffer.Length != value)
                { _buffer = new byte[value]; }

                _positionInBuffer = 0;
            }
        }
        //---------------------------------------------------------------------
        public void Send(ITcpTypeData data)
        {
            if (data == null)
            { throw new ArgumentNullException(nameof(data)); }

            if (!IsConnected)
            { throw new TcpConnectorException(TcpConnectorAction.Send, TcpConnectorErrorType.DeviceNotFound); }

            if (_connectorStream != null && _connectorStream.CanWrite)
            {
                try
                {
                    _connectorStream.Write(data.ToArray(), 0, data.Length);
                }
                catch (Exception exception)
                { _ExceptionHandling(exception, TcpConnectorAction.Send); }
            }
        }
        public bool Receive(out ITcpTypeData receivedData)
        {
            return Receive(Timeout.Infinite, out receivedData);
        }
        public bool Receive(int millisecondsTimeout, out ITcpTypeData receivedData)
        {
            if (millisecondsTimeout < Timeout.Infinite)
            { throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), millisecondsTimeout.ToString(), string.Empty); }

            receivedData = null;
            if (IsConnected && _connectorStream != null)
            {
                if (_connectorStream.CanRead && _connectorStream.DataAvailable)
                {
                    try
                    {
                        _connectorStream.ReadTimeout = millisecondsTimeout;
                        
                        var dataReadCount = _connectorStream.Read(_buffer, _positionInBuffer, _buffer.Length - _positionInBuffer);

                        if (dataReadCount == 0)
                        {
                            if (_socket.Poll(0, SelectMode.SelectRead) && _socket.Available == 0)
                            { throw new TcpConnectorException(TcpConnectorAction.Receive, TcpConnectorErrorType.ConnectionLost, "Poll", null); }
                        }

                        _positionInBuffer += dataReadCount;

                        if (_positionInBuffer == _buffer.Length)
                        { receivedData = new TcpTypeData(_buffer); }
                    }
                    catch (Exception exception)
                    { _ExceptionHandling(exception, TcpConnectorAction.Receive); }
                }
            }
            return receivedData != null;
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}