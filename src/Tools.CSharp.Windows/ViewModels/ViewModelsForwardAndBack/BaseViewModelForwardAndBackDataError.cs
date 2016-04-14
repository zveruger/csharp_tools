using System;
using System.Windows.Input;
using Tools.CSharp.Extensions;
using Tools.CSharp.Windows.Commands;

namespace Tools.CSharp.ViewModels
{
    public abstract class BaseViewModelForwardAndBackDataError : BaseViewModelDataError, IBaseViewModelForwardAndBack, IInternalViewModelForwardAndBack
    {
        #region private
        private readonly ViewModelForwardAndBackCollection _owner;
        private int _currentViewModelIndex = -1;
        private DelegateCommand _forwardCommand;
        private DelegateCommand _backCommand;

        private IBaseViewModelForwardAndBack _forwardViewModel;
        private IBaseViewModelForwardAndBack _backViewModel;

        private void _onCallForwardViewModel(ViewModelForwardAndBackEventArgs e)
        {
            e.Raise(this, ref CallForwardViewModel);
        }
        private void _onCallBackViewModel(ViewModelForwardAndBackEventArgs e)
        {
            e.Raise(this, ref CallBackViewModel);
        }
        #endregion
        #region protected
        protected virtual bool AvailableCallForwardViewModel()
        {
            return true;
        }
        protected virtual bool AvailableCallBackViewModel()
        {
            return true;
        }

        protected virtual void OnCallForwardViewModel(IBaseViewModelForwardAndBack viewModel)
        {
            _onCallForwardViewModel(new ViewModelForwardAndBackEventArgs(viewModel));
        }
        protected virtual void OnCallBackViewModel(IBaseViewModelForwardAndBack viewModel)
        {
            _onCallBackViewModel(new ViewModelForwardAndBackEventArgs(viewModel));
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                { }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected BaseViewModelForwardAndBackDataError(ViewModelForwardAndBackCollection owner)
        {
            if (owner == null)
            { throw new ArgumentNullException(nameof(owner)); }

            _owner = owner;
        }

        //---------------------------------------------------------------------
        public int CurrentViewModelIndex
        {
            get { return _currentViewModelIndex; }
        }
        //---------------------------------------------------------------------
        public ViewModelForwardAndBackCollection Owner
        {
            get { return _owner; }
        }
        //---------------------------------------------------------------------
        public bool IsForwardCommand
        {
            get { return _forwardViewModel != null && AvailableCallForwardViewModel(); }
        }
        public bool IsBackCommand
        {
            get { return _backViewModel != null && AvailableCallBackViewModel(); }
        }
        //---------------------------------------------------------------------
        public IBaseViewModelForwardAndBack ForwardViewModel
        {
            get { return _forwardViewModel; }
        }
        public IBaseViewModelForwardAndBack BackViewModel
        {
            get { return _backViewModel; }
        }
        //---------------------------------------------------------------------
        public ICommand ForwardCommand
        {
            get { return _forwardCommand ?? (_forwardCommand = new DelegateCommand(() => OnCallForwardViewModel(_forwardViewModel), () => IsForwardCommand)); }
        }
        public ICommand BackCommand
        {
            get { return _backCommand ?? (_backCommand = new DelegateCommand(() => OnCallBackViewModel(_backViewModel), () => IsBackCommand)); }
        }
        //---------------------------------------------------------------------
        public void SetCurrentViewModeIndex(int currentViewModelIndex)
        {
            _currentViewModelIndex = currentViewModelIndex;
        }
        public void SetForwardViewModel(IBaseViewModelForwardAndBack viewModel)
        {
            _forwardViewModel = viewModel;
        }
        public void SetBackViewModel(IBaseViewModelForwardAndBack viewModel)
        {
            _backViewModel = viewModel;
        }
        //---------------------------------------------------------------------
        public event EventHandler<ViewModelForwardAndBackEventArgs> CallForwardViewModel;
        public event EventHandler<ViewModelForwardAndBackEventArgs> CallBackViewModel;
        //---------------------------------------------------------------------
    }
}