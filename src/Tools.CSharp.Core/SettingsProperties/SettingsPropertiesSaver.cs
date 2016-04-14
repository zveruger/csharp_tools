using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Tools.CSharp.SettingsProperties
{
    public sealed class SettingsPropertiesSaver : BaseDisposable
    {
        #region private
        private readonly Timer _timer;
        private readonly IComplexSettingsProperties _complexSettingsProperties;
        //---------------------------------------------------------------------
        private int _syncPoint;
        private bool _nextStart;
        //---------------------------------------------------------------------
        private void _subscribeComplexSettingsProperties(bool addEvents)
        {
            if (addEvents)
            { _complexSettingsProperties.PropertyChanged += _complexSettingsPropertiesOnPropertyChanged; }
            else
            { _complexSettingsProperties.PropertyChanged -= _complexSettingsPropertiesOnPropertyChanged; }
        }

        private void _complexSettingsPropertiesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_timer.Enabled)
            {
                if (_syncPoint == 0)
                { _timer.Start(); }
                else
                { _nextStart = true; }
            }
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
            var sync = Interlocked.CompareExchange(ref _syncPoint, 1, 0);
            if (sync == 0)
            {
                _complexSettingsProperties.Save();

                if (_nextStart)
                {
                    _syncPoint = 0;
                    _timer.Start();
                }
                else
                { _syncPoint = 0; }

                _nextStart = false;
            }
        }
        #endregion
        #region protected
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_complexSettingsProperties != null)
                { _subscribeComplexSettingsProperties(false); }

                if (_timer != null)
                {
                    _subscribeTimerAllEvents(false);
                    _timer.Close();
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        public SettingsPropertiesSaver(IComplexSettingsProperties complexSettingsProperties)
            : this(complexSettingsProperties, DefaultInterval)
        { }
        public SettingsPropertiesSaver(IComplexSettingsProperties complexSettingsProperties, int interval)
        {
            if (complexSettingsProperties == null)
            { throw new ArgumentNullException(nameof(complexSettingsProperties)); }

            _timer = new Timer(interval) { AutoReset = false };
            _subscribeTimerAllEvents(true);

            _complexSettingsProperties = complexSettingsProperties;
            _subscribeComplexSettingsProperties(true);
        }

        //---------------------------------------------------------------------
        public const int DefaultInterval = 1000;
        //---------------------------------------------------------------------
        public int Interval
        {
            get { return (int)_timer.Interval; }
            set { _timer.Interval = value; }
        }
        //---------------------------------------------------------------------
    }
}