using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using Tools.CSharp.StateMachines;
using Timer = System.Timers.Timer;

namespace Tools.CSharp.Pings
{
    public sealed class PingServer : BaseDisposable
    {
        #region private
        private const string _AddressPattern = @"^(\d|\d{2}|[01]\d{2}|2[0-4]\d|25[0-5])(\.(\d|\d{2}|[01]\d{2}|2[0-4]\d|25[0-5])){3}$";
        //---------------------------------------------------------------------
        private readonly StateMachine<PingServerStateResult> _stateMachine = new StateMachine<PingServerStateResult>(PingServerStateResult.Init);
        private readonly int _timeout;
        private readonly Timer _timer;
        //---------------------------------------------------------------------
        private bool _isStart;
        private Ping _ping;
        private PingServerStateResult _lastServerStateResult = PingServerStateResult.Init;
        //---------------------------------------------------------------------
        private sealed class InternalPingServerActionEventArgs : PingServerActionEventArgs
        {
            #region private
            private readonly PingServer _owner;
            #endregion
            public InternalPingServerActionEventArgs(PingServer owner)
            {
                _owner = owner;
            }

            //-----------------------------------------------------------------
            public override void SetServerAddress(string address)
            {
                var stateResult = PingServerStateResult.Init;

                try
                {
                    if (!string.IsNullOrWhiteSpace(address))
                    {
                        stateResult = PingServerStateResult.ServerFailed;

                        if (Regex.IsMatch(address, _AddressPattern, RegexOptions.IgnoreCase))
                        {
                            var ping = LazyInitializer.EnsureInitialized(ref _owner._ping, () => new Ping());
                            var pingResult = ping.Send(address, _owner._timeout);

                            if (pingResult != null && pingResult.Status == IPStatus.Success)
                            { stateResult = PingServerStateResult.ServerSuccess; }
                        }
                    }
                }
                catch
                { }
                finally
                { _owner._stateResultUpdate(stateResult); }

                if (_owner._isStart)
                { _owner._timer.Start(); }
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private void _stateResultUpdate(PingServerStateResult state)
        {
            _stateMachine.State = state;
        }
        //---------------------------------------------------------------------
        private void _subscribeTimerAllEvents(bool addEvents)
        {
            if (addEvents)
            { _timer.Elapsed += _timerOnElapsed; }
            else
            { _timer.Elapsed -= _timerOnElapsed; }
        }
        private void _timerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var timer = (Timer)sender;
            timer.Stop();

            _onPingAction();
        }
        //---------------------------------------------------------------------
        private void _subscribeStateMachineAllEvents(bool addEvents)
        {
            if (addEvents)
            { _stateMachine.StateChanged += _stateMachineOnStateChanged; }
            else
            { _stateMachine.StateChanged -= _stateMachineOnStateChanged; }
        }
        private void _stateMachineOnStateChanged(object sender, StateEventArgs<PingServerStateResult> e)
        {
            _lastServerStateResult = e.NewState;
            _onResultChanged(_lastServerStateResult);
        }
        //---------------------------------------------------------------------
        private void _onPingAction()
        {
            PingAction?.Invoke(this, new InternalPingServerActionEventArgs(this));
        }
        private void _onResultChanged(PingServerStateResult value)
        {
            ResultChanged?.Invoke(this, new PingServerResultEventArgs(value));
        }
        #endregion
        #region protected
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    if (_stateMachine != null)
                    { _subscribeStateMachineAllEvents(false); }

                    if (_timer != null)
                    {
                        _subscribeTimerAllEvents(false);
                        _isStart = false;
                        _timer.Stop();
                        _timer.Dispose();
                    }

                    _ping?.Dispose();
                }
            }
            finally
            { base.Dispose(disposing); }
            
        }
        #endregion
        public PingServer(int timeout = DefaultTimeout)
        {
            if (timeout < 0)
            { throw new ArgumentOutOfRangeException(nameof(timeout)); }

            _timeout = timeout;
            _timer = new Timer(_timeout);

            _subscribeTimerAllEvents(true);
            _subscribeStateMachineAllEvents(true);
        }

        //---------------------------------------------------------------------
        public const int DefaultTimeout = 1000;
        //---------------------------------------------------------------------
        public bool IsStarted
        {
            get { return _isStart; }
        }
        public PingServerStateResult LastServerStateResult
        {
            get { return _lastServerStateResult; }
        }
        //---------------------------------------------------------------------
        public void Start()
        {
            if (!_timer.Enabled)
            {
                _isStart = true;
                _timer.Start();
            }
        }
        public void Stop()
        {
            if (_timer.Enabled)
            {
                _isStart = false;
                _timer.Stop();
            }
        }
        //---------------------------------------------------------------------
        public delegate IAsyncResult GetServerAddress(out IPAddress address);
        //---------------------------------------------------------------------
        public event EventHandler<PingServerActionEventArgs> PingAction; 
        public event EventHandler<PingServerResultEventArgs> ResultChanged;
        //---------------------------------------------------------------------
    }
}