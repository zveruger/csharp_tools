using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Tools.CSharp.Extensions;
using Tools.CSharp.StateMachines;
using Timer = System.Timers.Timer;

namespace Tools.CSharp.ProgressProcesses
{
    public abstract class ProgressProcess : BaseDisposable, IProgressProcess
    {
        #region privare
        private const int _DefaultMaxValue = -1;
        private const int _WaitPollTime = 1000;
        //---------------------------------------------------------------------
        private readonly Action<Action> _validThreadAction;
        private readonly Action<Exception> _exceptionHandler;
        private readonly ProgressProcessPriority _priority;
        private readonly StateMachine<ProgressProcessState> _stateMachine = new StateMachine<ProgressProcessState>(ProgressProcessState.None);
        private readonly Timer _waitPollTimer = new Timer(_WaitPollTime);
        private readonly EventWaitHandle _eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly ProgressProcessSheduler _sheduler;
        //---------------------------------------------------------------------
        private string _title;
        private string _description;
        private int _maxValue = _DefaultMaxValue;
        private int _stepValue;
        private int _processPercentage = -1;
        private CancellationToken _cancellationToken;
        //---------------------------------------------------------------------
        private void _stateUpdate(ProgressProcessState state)
        {
            _stateMachine.State = state;
        }
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
        private void _stateMachineOnStateChanging(object sender, StateEventArgs<ProgressProcessState> e)
        {
            ValidThreadAction(() => e.Raise(this, ref StateChanging));
        }
        private void _stateMachineOnStateChanged(object sender, StateEventArgs<ProgressProcessState> e)
        {
            ValidThreadAction(() => e.Raise(this, ref StateChanged));
        }
        //---------------------------------------------------------------------
        private void _initializeVariables()
        {
            _title = string.Empty;
            _description = string.Empty;

            _resetProgress();
        }
        private void _resetProgress()
        {
            _maxValue = _DefaultMaxValue;
            _processPercentage = -1;
            _stepValue = 0;
        }
        //---------------------------------------------------------------------
        private void _subscribeWaitPoolTimerAllEvents(bool addEvents)
        {
            if (_waitPollTimer != null)
            {
                if (addEvents)
                { _waitPollTimer.Elapsed += _waitPollTimerOnElapsed; }
                else
                { _waitPollTimer.Elapsed -= _waitPollTimerOnElapsed; }
            }
        }
        private void _waitPollTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_cancellationToken.IsCancellationRequested)
            { _removeWaitProcess(); }
        }
        //---------------------------------------------------------------------
        private void _waitProcess(CancellationToken cancellationToken)
        {
            if (_isWaitProcess())
            {
                _cancellationToken = cancellationToken;
                _stateUpdate(ProgressProcessState.Waiting);

                WaitingProcess();

                _waitPollTimer.Start();

                _eventWaitHandle.Reset();
                _eventWaitHandle.WaitOne();
            }
        }
        private bool _isWaitProcess()
        {
            if (_sheduler == null)
            { return false; }

            _sheduler.AddProcess(this);
            return !_sheduler.IncrementRunProcessCount();
        }
        private void _resetWaitProcess()
        {
            _subscribeWaitPoolTimerAllEvents(false);
            _waitPollTimer.Stop();

            _initializeVariables();
            _eventWaitHandle.Set();
        }
        private void _removeWaitProcess()
        {
            _waitPollTimer.Stop();

            _sheduler.RemoveProcess(this);
            _resetWaitProcess();
        }
        private void _nextLoaderProcess()
        {
            var nextProcess = _sheduler?.GetNextProcessAndDecrementRunProcessCount(this);
            nextProcess?._resetWaitProcess();
        }
        #endregion
        #region protected
        protected void ValidThreadAction(Action action)
        {
            _validThreadAction(action);
        }
        protected void ExceptionHandler(Exception exception)
        {
            _exceptionHandler(exception);
        }

        protected void TitleUpdate(string title)
        {
            _title = title;
            ValidThreadAction(OnTitleChanged);
        }
        protected void DescriptionUpdate(string description)
        {
            _description = description;
            ValidThreadAction(OnDescriptionChanged);
        }

        protected void SetMaxProgressValue(int maxValue)
        {
            if (maxValue < 0)
            { throw new ArgumentOutOfRangeException(nameof(maxValue)); }

            _maxValue = maxValue;
            _stepValue = 0;

            OnMaxProgressChanged();
        }
        protected void UpdateProgress()
        {
            if (_maxValue < 0)
            { return; }

            if (_stepValue < _maxValue)
            { _stepValue++; }

            UpdateProgress(_stepValue);
        }
        protected void UpdateProgress(int value)
        {
            if (_maxValue < 0)
            { return; }

            if (_maxValue == 0)
            { _processPercentage = -1; }
            else
            { _processPercentage = value > _maxValue ? 100 : value * 100 / _maxValue; }

            ValidThreadAction(() => OnProgressChanged(_processPercentage));
        }
        protected void ResetProgress()
        {
            _resetProgress();
            ValidThreadAction(() => OnProgressChanged(_processPercentage));
        }

        protected void InternalLoadProcess(CancellationToken cancellationToken)
        {
            _initializeVariables();

            try
            {
                _waitProcess(cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    _stateUpdate(ProgressProcessState.Starting);

                    LoadingProcess();
                    LoadProcess(cancellationToken);
                }
                LoadedProcess(cancellationToken);
            }
            finally
            {
                var oldState = _stateMachine.State;

                _stateUpdate(ProgressProcessState.Stoped);

                if (oldState == ProgressProcessState.Starting)
                { _nextLoaderProcess(); }
            }
        }
        protected void LoadProcessExceptionHandler(Task task)
        {
            if (task?.Exception != null)
            {
                task.Exception.Flatten();

                foreach (var exception in task.Exception.InnerExceptions)
                {
                    var tmpException = exception;
                    ValidThreadAction(() => ExceptionHandler(tmpException));
                }
            }
        }

        protected virtual Task CreateStartLoadAsync(CancellationToken cancellationToken)
        {
            var task = new Task(obj => InternalLoadProcess((CancellationToken)obj), cancellationToken);
            task.ContinueWith(LoadProcessExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
            return task;
        }
        protected virtual void WaitingProcess()
        { }
        protected virtual void LoadingProcess()
        {

        }
        protected abstract void LoadProcess(CancellationToken cancellationToken);
        protected virtual void LoadedProcess(CancellationToken cancellationToken)
        {

        }

        protected virtual void OnTitleChanged()
        {
            TitleChanged?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnDescriptionChanged()
        {
            DescriptionChanged?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnMaxProgressChanged()
        {
            MaxProgressValueChanged?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnProgressChanged(int progressPercentage)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progressPercentage, null));
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    _subscribeStateMachineAllEvents(false);

                    _eventWaitHandle?.Dispose();

                    if (_waitPollTimer != null)
                    {
                        _subscribeWaitPoolTimerAllEvents(false);
                        _waitPollTimer.Dispose();
                    }
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected ProgressProcess(Action<Action> validThreadAction, Action<Exception> exceptionHandler, ProgressProcessPriority priority, ProgressProcessSheduler sheduler = null)
        {
            if (validThreadAction == null)
            { throw new ArgumentNullException(nameof(validThreadAction)); }
            if (exceptionHandler == null)
            { throw new ArgumentNullException(nameof(exceptionHandler)); }

            _validThreadAction = validThreadAction;
            _exceptionHandler = exceptionHandler;
            _priority = priority;
            _sheduler = sheduler;

            _subscribeStateMachineAllEvents(true);
            _subscribeWaitPoolTimerAllEvents(true);

            _initializeVariables();
        }

        //---------------------------------------------------------------------
        public string Title
        {
            get { return _title; }
        }
        public string Description
        {
            get { return _description; }
        }
        //---------------------------------------------------------------------
        public int MaxProgressValue
        {
            get { return _maxValue; }
        }
        public int ProgressPercentage
        {
            get { return _processPercentage; }
        }
        //---------------------------------------------------------------------
        public ProgressProcessState State
        {
            get { return _stateMachine.State; }
        }
        public ProgressProcessPriority Priority
        {
            get { return _priority; }
        }
        public bool IsStartLoadAvailable
        {
            get { return _stateMachine.State == ProgressProcessState.None || _stateMachine.State == ProgressProcessState.Stoped; }
        }
        //---------------------------------------------------------------------
        public Task StartLoadAsync(CancellationToken cancellationToken)
        {
            if (!IsStartLoadAvailable)
            { throw new InvalidOperationException(); }

            return CreateStartLoadAsync(cancellationToken);
        }
        public bool CheckState(params ProgressProcessState[] states)
        {
            return _stateMachine.CheckState(states);
        }
        //---------------------------------------------------------------------
        public event EventHandler MaxProgressValueChanged;
        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler TitleChanged;
        public event EventHandler DescriptionChanged;
        public event EventHandler<StateEventArgs<ProgressProcessState>> StateChanging;
        public event EventHandler<StateEventArgs<ProgressProcessState>> StateChanged;
        //---------------------------------------------------------------------
    }
}