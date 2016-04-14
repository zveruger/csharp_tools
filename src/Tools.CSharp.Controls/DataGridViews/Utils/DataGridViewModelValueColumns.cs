using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tools.CSharp.Controls.Utils
{
    //-------------------------------------------------------------------------
    public class DataGridViewModelValueColumns<TModel, TValue>
    {
        #region private
        private readonly TValue _defaultValue = default(TValue);
        private readonly DataGridViewColumnCollection _columns;
        private readonly Dictionary<int, Func<TModel, TValue>> _cacheColumnIndexes = new Dictionary<int, Func<TModel, TValue>>();
        #endregion
        public DataGridViewModelValueColumns(DataGridView dataGridView)
        {
            if(dataGridView == null)
            { throw new ArgumentNullException(nameof(dataGridView)); }

            _columns = dataGridView.Columns;
        }
        public DataGridViewModelValueColumns(DataGridViewColumnCollection columns)
        {
            if(columns == null)
            { throw new ArgumentNullException(nameof(columns)); }

            _columns = columns;
        }

        //---------------------------------------------------------------------
        public DataGridViewColumnCollection GetColumns
        {
            get { return _columns; }
        }
        //---------------------------------------------------------------------
        public void AddColumn(DataGridViewColumn column, Func<TModel, TValue> predicate = null)
        {
            if(column == null)
            { throw new ArgumentNullException(nameof(column)); }
            if(_columns.Contains(column))
            { throw new ArgumentException(string.Empty, nameof(column)); }

            _columns.Add(column);

            if (predicate != null)
            { _cacheColumnIndexes.Add(column.Index, predicate); }           
        }
        public TValue GetValue(DataGridViewColumn column, TModel model)
        {
            bool predicateAvailable;
            return GetValue(column, model, out predicateAvailable);
        }
        public TValue GetValue(DataGridViewColumn column, TModel model, out bool predicateAvailable)
        {
            if(column == null)
            { throw new ArgumentNullException(nameof(column)); }

            predicateAvailable = false;

            if(EqualityComparer<TModel>.Default.Equals(model, default(TModel)))
            { return _defaultValue; }

            Func<TModel, TValue> predicate;
            predicateAvailable = _cacheColumnIndexes.TryGetValue(column.Index, out predicate);
            return predicateAvailable ? predicate(model) : _defaultValue;
        }
        //---------------------------------------------------------------------
        public void Clear()
        {
            _columns.Clear();
            _cacheColumnIndexes.Clear();
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public class DataGridViewModelValueColumns<TModel> : DataGridViewModelValueColumns<TModel, object>
    {
        public DataGridViewModelValueColumns(DataGridViewColumnCollection columns) 
            : base(columns)
        { }
    }
    //-------------------------------------------------------------------------
}
