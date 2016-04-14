using System;

namespace Tools.CSharp.ViewModels
{
    public sealed class ViewModelForwardAndBackEventArgs : EventArgs
    {
        #region
        private readonly IBaseViewModelForwardAndBack _viewModel;
        #endregion
        public ViewModelForwardAndBackEventArgs(IBaseViewModelForwardAndBack viewModel)
        {
            if (viewModel == null)
            { throw new ArgumentNullException(nameof(viewModel)); }

            _viewModel = viewModel;
        }

        //---------------------------------------------------------------------
        public IBaseViewModelForwardAndBack VieModel
        {
            get { return _viewModel; }
        }
        //---------------------------------------------------------------------
    }
}