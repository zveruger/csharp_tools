using System;
using System.ComponentModel;

namespace Tools.CSharp
{
    public abstract class BaseDisposable : IDisposable
    {
        #region private
        private bool _isDisposed;
        #endregion
        #region protected
        protected void CheckThrowObjectDisposedException()
        {
            if (IsDisposed)
            { throw new ObjectDisposedException(GetType().FullName); }
        }
        //---------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!_isDisposed && disposing)
                {
                }
            }
            finally
            { _isDisposed = true; }
        }
        #endregion
        ~BaseDisposable()
        {
            Dispose(false);
        }

        //---------------------------------------------------------------------
        [Browsable(false)]
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        //---------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //---------------------------------------------------------------------
    }
}