using System.ComponentModel;

namespace Tools.CSharp.ViewModels
{
    public interface IBaseViewModelDataError : IBaseViewModel, IDataErrorInfo
    {
        //---------------------------------------------------------------------
        bool IsValid { get; }
        //---------------------------------------------------------------------
    }
}