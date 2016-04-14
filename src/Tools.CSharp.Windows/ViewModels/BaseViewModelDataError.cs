using System.Windows.Input;

namespace Tools.CSharp.ViewModels
{
    public abstract class BaseViewModelDataError : BaseViewModel, IBaseViewModelDataError
    {
        #region protected
        protected abstract bool IsValidViewModel();
        protected abstract string GetValidationPropertyMessage(string propertyName);
        #endregion
        //---------------------------------------------------------------------
        public string this[string columnName]
        {
            get
            {
                CommandManager.InvalidateRequerySuggested();
                return GetValidationPropertyMessage(columnName);
            }
        }
        //---------------------------------------------------------------------
        public bool IsValid
        {
            get { return IsValidViewModel(); }
        }
        public string Error
        {
            get { return string.Empty; }
        }
        //---------------------------------------------------------------------
    }
}