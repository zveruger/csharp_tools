namespace Tools.CSharp.ViewModels
{
    internal interface IInternalViewModelForwardAndBack
    {
        //---------------------------------------------------------------------
        void SetCurrentViewModeIndex(int currentViewModelIndex);
        //---------------------------------------------------------------------
        void SetForwardViewModel(IBaseViewModelForwardAndBack viewModel);
        void SetBackViewModel(IBaseViewModelForwardAndBack viewModel);
        //---------------------------------------------------------------------
    }
}