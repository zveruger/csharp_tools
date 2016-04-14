using System.ComponentModel;

namespace Tools.CSharp.ViewModels
{
    public abstract class BaseViewModel: BaseDisposable, IBaseViewModel
    {
        #region protected
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        //---------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        //---------------------------------------------------------------------
    }
}