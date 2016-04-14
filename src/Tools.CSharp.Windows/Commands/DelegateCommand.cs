using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Tools.CSharp.Windows.Commands
{
    public sealed class DelegateCommand : ICommand
    {
        #region private
        private readonly Action _executeMethod;
        private readonly Func<bool> _canExecuteMethod;
        private bool _isAutomaticRequeryDisabled;
        private List<WeakReference> _canExecuteChangedHandlers;
        #endregion
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            _isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null)
        {
        }

        //---------------------------------------------------------------------
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return _isAutomaticRequeryDisabled;
            }
            set
            {
                if (_isAutomaticRequeryDisabled != value)
                {
                    if (value)
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
                    else
                        CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);

                    _isAutomaticRequeryDisabled = value;
                }
            }
        }
        //---------------------------------------------------------------------
        public void RaiseCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }
        //---------------------------------------------------------------------
        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod == null || _canExecuteMethod();
        }
        public void Execute(object parameter)
        {
            _executeMethod?.Invoke();
        }
        //---------------------------------------------------------------------
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!_isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested += value;
                CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value);
            }
            remove
            {
                if (!_isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested -= value;
                CommandManagerHelper.RemoveWeakRefenceHandler(_canExecuteChangedHandlers, value);
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public sealed class DelegateCommand<T> : ICommand
    {
        #region private
        private readonly Action<T> _executeMethod;
        private readonly Func<T, bool> _canExecuteMethod;
        private bool _isAutomaticRequeryDisabled;
        private List<WeakReference> _canExecuteChangedHandlers;
        #endregion
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            _isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        //---------------------------------------------------------------------
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return _isAutomaticRequeryDisabled;
            }
            set
            {
                if (_isAutomaticRequeryDisabled != value)
                {
                    if (value)
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
                    else
                        CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);

                    _isAutomaticRequeryDisabled = value;
                }
            }
        }
        //---------------------------------------------------------------------
        public void RaiseCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }
        //---------------------------------------------------------------------
        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
            { return (_canExecuteMethod == null); }

            return _canExecuteMethod == null || _canExecuteMethod((T)parameter);
        }
        public void Execute(object parameter)
        {
            _executeMethod?.Invoke((T)parameter);
        }
        //---------------------------------------------------------------------
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!_isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested += value;
                CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value);
            }
            remove
            {
                if (!_isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested -= value;
                CommandManagerHelper.RemoveWeakRefenceHandler(_canExecuteChangedHandlers, value);
            }
        }
        //---------------------------------------------------------------------
    }
}