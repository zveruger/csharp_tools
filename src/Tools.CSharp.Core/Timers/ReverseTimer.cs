using System;
using System.Threading;

namespace Tools.CSharp.Timers
{
    public sealed class ReverseTimer : BaseDisposable
    {
        #region private
        private const long _TicksPerMillisecond = 10000;
        private const long _TicksPerSecond = _TicksPerMillisecond * 1000;
        //---------------------------------------------------------------------
        private readonly int _initializeSeconds;
        private readonly DateTime _initializedTime;
        //---------------------------------------------------------------------
        private readonly System.Timers.Timer _timer; 
        //---------------------------------------------------------------------
        private int _minuendTimeSeconds;
        private bool _autoStopTimer;
        private int _syncPoint;
        //---------------------------------------------------------------------
        private bool _trySetMinuendTimeSeconds(int newValue)
        {
            if ((newValue >= 0) && (_minuendTimeSeconds != newValue))
            {
                _minuendTimeSeconds = newValue;
                _onMinuendTimeChanged();

                return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        private void _subscribeTimerAllEvents(bool addEvents)
        {
            if (addEvents)
            { _timer.Elapsed += _timerOnElapsed; }
            else
            { _timer.Elapsed -= _timerOnElapsed; }
        }

        private void _timerOnElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var sync = Interlocked.CompareExchange(ref _syncPoint, 1, 0);
            if (sync == 0)
            {
                if (_trySetMinuendTimeSeconds(_minuendTimeSeconds - 1))
                {
                    if (_minuendTimeSeconds == 0)
                    { _stopTime(true); }
                }

                _syncPoint = 0;
            } 
        }
        //---------------------------------------------------------------------
        private void _startTime()
        {
            if (!_timer.Enabled)
            {
                _autoStopTimer = false;

                _resetMinuendTimeSeconds();
                _onMinuendTimeChanged();

                _timer.Start();
            }
        }
        private void _stopTime(bool autoStopTimer)
        {
            if (_timer.Enabled)
            {
                _autoStopTimer = autoStopTimer;

                _timer.Stop();

                _resetMinuendTimeSeconds();
            }
        }
        //---------------------------------------------------------------------
        private void _resetMinuendTimeSeconds()
        {
            _trySetMinuendTimeSeconds(_initializeSeconds);
        }
        //---------------------------------------------------------------------
        private void _onMinuendTimeChanged()
        {
            MinuendTimeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #region protected
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_timer != null)
                {
                    _subscribeTimerAllEvents(false);
                    _timer.Stop();
                    _timer.Dispose();
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        public ReverseTimer(int initializeSeconds)
        {
            if (initializeSeconds < 0)
            { throw new ArgumentOutOfRangeException(nameof(initializeSeconds)); }

            _initializeSeconds = initializeSeconds;
            _minuendTimeSeconds = _initializeSeconds;
            //-----------------------------------------------------------------
            _initializedTime = new DateTime(_initializeSeconds * _TicksPerSecond);
            //-----------------------------------------------------------------
            _timer = new System.Timers.Timer(1000);
            _subscribeTimerAllEvents(true);
        }

        //---------------------------------------------------------------------
        public DateTime InitializedTime
        {
            get { return _initializedTime; }
        }
        public DateTime MinuendTime
        {
            get { return new DateTime(_minuendTimeSeconds * _TicksPerSecond); }
        }
        //---------------------------------------------------------------------
        public bool Enabled
        {
            get { return _timer.Enabled; }
            set
            {
                if (value)
                { _startTime(); }
                else
                { _stopTime(false); }
            }
        }
        public bool AutoStop
        {
            get { return _autoStopTimer; }
        }
        //---------------------------------------------------------------------
        public void Start()
        {
            _startTime();
        }
        public void Stop()
        {
            _stopTime(false);
        }
        public void Reload()
        {
            _resetMinuendTimeSeconds();
        }
        //---------------------------------------------------------------------
        public event EventHandler MinuendTimeChanged;
        //---------------------------------------------------------------------
    }
}