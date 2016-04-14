using System;

namespace Tools.CSharp.Collections
{
    public sealed class SortedItemCollectionCountEventArgs : EventArgs
    {
        #region private
        private readonly int _count;
        private readonly bool _isInstall;
        #endregion
        public SortedItemCollectionCountEventArgs(int count, bool isInstall)
        {
            _count = count;
            _isInstall = isInstall;
        }

        //---------------------------------------------------------------------
        public int Count
        {
            get { return _count; }
        }
        //---------------------------------------------------------------------
        public bool IsInstall
        {
            get { return _isInstall; }
        }
        //---------------------------------------------------------------------
    }
}