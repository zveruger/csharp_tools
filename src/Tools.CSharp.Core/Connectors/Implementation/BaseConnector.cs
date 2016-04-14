using System;
using Tools.CSharp.Loggers;
using Tools.CSharp.StateMachines;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Connectors
{
    public abstract class BaseConnector<TConnectorConnectParams> : BaseDisposable, IConnector<TConnectorConnectParams>
        where TConnectorConnectParams : IConnectorConnectParams
    {
        #region private
        private readonly StateMachine<ConnectorState> _stateMachine = new StateMachine<ConnectorState>(ConnectorState.Disconnected);
        private readonly ILogger _logger;
        //---------------------------------------------------------------------
        private TConnectorConnectParams _connectParamsWhereConnect;
        //---------------------------------------------------------------------
        private void _subscribeStateMachineAllEvents(bool addEvents)
        {
            if (addEvents)
            {
                _stateMachine.StateChanging += _stateMachineOnStateChanging;
                _stateMachine.StateChanged += _stateMachineOnStateChanged;
            }
            else
            {
                _stateMachine.StateChanging -= _stateMachineOnStateChanging;
                _stateMachine.StateChanged -= _stateMachineOnStateChanged;
            }
        }

        private void _stateMachineOnStateChanging(object sender, StateEventArgs<ConnectorState> e)
        {
            StateChanging?.Invoke(this, e);
        }
        private void _stateMachineOnStateChanged(object sender, StateEventArgs<ConnectorState> e)
        {
            StateChanged?.Invoke(this, e);
        }
        //---------------------------------------------------------------------
        #endregion
        #region protected
        protected ILogger Logger
        {
            get { return _logger; }
        }

        protected abstract void Connecting(TConnectorConnectParams connectParams);
        protected abstract void Disconnecting();

        protected virtual void Connected(TConnectorConnectParams connectParams)
        {
            if (connectParams != null)
            {  _logger.Log(LoggerLevel.Info, () => $"Connected to: {connectParams}."); }
        }
        protected virtual void Disconnected(TConnectorConnectParams connectParams)
        {
           if (connectParams != null)
            {  _logger.Log(LoggerLevel.Info, () => $"Disconnected from: {connectParams}."); }
        }
        //---------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    _subscribeStateMachineAllEvents(false);
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected BaseConnector(ILogger logger)
        {
            _logger = logger;

            _subscribeStateMachineAllEvents(true);
        }

        //---------------------------------------------------------------------
        public bool IsConnected
        {
            get { return _stateMachine.State == ConnectorState.Connected; }
        }
        public TConnectorConnectParams ConnectParamsWhereConnect
        {
            get { return _connectParamsWhereConnect; }
        }
        //---------------------------------------------------------------------
        public ConnectorState State
        {
            get { return _stateMachine.State; }
            private set { _stateMachine.State = value; }
        }
        //---------------------------------------------------------------------
        public void Connect(TConnectorConnectParams connectParams)
        {
            if (IsConnected)
            { throw new InvalidOperationException(); }
            if (connectParams == null)
            { throw new ArgumentNullException(nameof(connectParams)); }

            _connectParamsWhereConnect = connectParams;
            State = ConnectorState.Connecting;

            Connecting(connectParams);
            Connected(connectParams);

            State = ConnectorState.Connected;
        }
        public void Disconnect()
        {
            if (State != ConnectorState.Disconnected)
            {
                State = ConnectorState.Disconnecting;
                try
                {
                    Disconnecting();
                    Disconnected(_connectParamsWhereConnect);
                }
                catch { }
                finally
                {
                    State = ConnectorState.Disconnected;
                }
            }
        }
        //---------------------------------------------------------------------
        public bool CheckState(params ConnectorState[] states)
        {
            return _stateMachine.CheckState(states);
        }
        //---------------------------------------------------------------------
        public event EventHandler<StateEventArgs<ConnectorState>> StateChanging;
        public event EventHandler<StateEventArgs<ConnectorState>> StateChanged;
        //---------------------------------------------------------------------
    }
}
