using System;

namespace Tools.CSharp.Controls
{
    internal sealed class DataGridViewCheckBoxColumnHeaderCellEventArgs : EventArgs
    {
        #region private
        private readonly int _columnIndex;
        private readonly bool _checkedState;
        #endregion
        internal DataGridViewCheckBoxColumnHeaderCellEventArgs(int columnIndex, bool checkedState)
        {
            _columnIndex = columnIndex;
            _checkedState = checkedState;
        }

        //---------------------------------------------------------------------
        public int ColumnIndex
        {
            get { return _columnIndex; }
        }
        //---------------------------------------------------------------------
        public bool CheckedState
        {
            get { return _checkedState; }
        }
        //---------------------------------------------------------------------
    }
}
