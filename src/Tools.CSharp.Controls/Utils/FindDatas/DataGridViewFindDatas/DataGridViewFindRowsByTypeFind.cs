using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Tools.CSharp.FindDatas
{
    public abstract class DataGridViewFindRowsByTypeFind<TFind> : DataGridViewFindRows
        where TFind : struct, IEquatable<TFind>
    {
        #region private
        private readonly List<int> _cacheExcludedColumnIndexCollectionByTypeFind = new List<int>();
        //---------------------------------------------------------------------
        private TFind _oldFindValue;
        #endregion
        #region protected
        protected abstract void AddAllExcludedColumn(TFind findValue);
        //---------------------------------------------------------------------
        protected ReadOnlyCollection<int> CreateExcludedColumnIndexCollection(TFind findValue)
        {
            if (_oldFindValue.Equals(default(TFind)) || !_oldFindValue.Equals(findValue))
            {
                _oldFindValue = findValue;

                _cacheExcludedColumnIndexCollectionByTypeFind.Clear();
                
                AddAllExcludedColumn(_oldFindValue);
            }
            return _cacheExcludedColumnIndexCollectionByTypeFind.AsReadOnly();
        }
        //---------------------------------------------------------------------
        protected bool AddExcludedColumn(TFind findValue, TFind columnFindValue, DataGridViewBand column)
        {
            if (column == null || findValue.Equals(columnFindValue))
            { return false; }

            _cacheExcludedColumnIndexCollectionByTypeFind.Add(column.Index);
            return true;
        }
        #endregion
        protected DataGridViewFindRowsByTypeFind(DataGridView dataGridView, bool multiFindText, bool isClearSelection)
            : base(dataGridView, multiFindText, isClearSelection)
        { }
    }
}