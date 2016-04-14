using System;
using System.Windows.Input;

namespace Tools.CSharp.ViewModels
{
    public interface IBaseViewModelForwardAndBack : IBaseViewModelDataError
    {
        //---------------------------------------------------------------------
        int CurrentViewModelIndex { get; }
        //---------------------------------------------------------------------
        ViewModelForwardAndBackCollection Owner { get; }
        //---------------------------------------------------------------------
        bool IsForwardCommand { get; }
        bool IsBackCommand { get; }
        //---------------------------------------------------------------------
        IBaseViewModelForwardAndBack ForwardViewModel { get; }
        IBaseViewModelForwardAndBack BackViewModel { get; }
        //---------------------------------------------------------------------
        ICommand ForwardCommand { get; }
        ICommand BackCommand { get; }
        //---------------------------------------------------------------------
        event EventHandler<ViewModelForwardAndBackEventArgs> CallForwardViewModel;
        event EventHandler<ViewModelForwardAndBackEventArgs> CallBackViewModel;
        //---------------------------------------------------------------------
    }
}