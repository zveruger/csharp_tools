using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public class SorterListView : ListView
    {
        #region private
        private int _cacheWidth;
        private int _cacheHeigth;
        //---------------------------------------------------------------------
        private ListViewColumnSorter _defaultSorter;
        //---------------------------------------------------------------------
        private void _sort(int columnIndex, SortOrder sortOrder, bool forceSortOrder)
        {
            ListViewColumnSorter sorter = null;
            var itemExtended = Columns[columnIndex] as SorterListViewColumnHeader;

            if (itemExtended != null)
            { sorter = itemExtended.Sorter; }

            if ((sorter == null) && (columnIndex == _defaultSorter.SortColumn))
            { sorter = _defaultSorter; }

            if (sorter == null)
            {
                _defaultSorter.SortColumn = columnIndex;
                sorter = _defaultSorter;
            }
            else
            {
                if (forceSortOrder)
                { sorter.Sorting = sortOrder; }
                else
                { sorter.Sorting = sorter.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending; }
            }

            _sort(sorter);
        }
        private void _sort(ListViewColumnSorter sorter)
        {
            ListViewItemSorter = null;
            ListViewItemSorter = sorter;

            if (sorter != null)
            { this.SetSortIcon(sorter.SortColumn, sorter.Sorting); }

            Sort();
        }
        #endregion
        #region protected
        protected override void OnResize(EventArgs e)
        {
            if (_cacheWidth != Width || _cacheHeigth != Height)
            {
                _cacheWidth = Width;
                _cacheHeigth = Height;

                var lastColumnIndex = Columns.Count - 1;
                if (lastColumnIndex != -1)
                { Columns[lastColumnIndex].Width = -2; }
            }

            base.OnResize(e);
        }
        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            base.OnColumnClick(e);

            _sort(e.Column, SortOrder.Ascending, false);
        }
        #endregion
        public SorterListView()
        {
            Sorting = SortOrder.Ascending;
            DefaultSorter = new ListViewColumnTextSorter();
        }

        //---------------------------------------------------------------------
        [DefaultValue(typeof(ListViewColumnSorter), "ListViewColumnTextSorter")]
        public ListViewColumnSorter DefaultSorter
        {
            get { return _defaultSorter; }
            set
            {
                if (value == null)
                { throw new ArgumentNullException(nameof(value)); }

                if (_defaultSorter != value)
                {
                    value.Sorting = Sorting;
                    _defaultSorter = value;

                    _sort(_defaultSorter);
                }
            }
        }
        //---------------------------------------------------------------------
        [DefaultValue(typeof(SortOrder), "Ascending")]
        public new SortOrder Sorting
        {
            get { return _defaultSorter?.Sorting ?? base.Sorting; }
            set
            {
                var sorting = Sorting;
                if (sorting != value)
                {
                    base.Sorting = value;

                    if (_defaultSorter != null)
                    { _defaultSorter.Sorting = value; }
                }
            }
        }
        //---------------------------------------------------------------------
        public void ColumnSort(ColumnHeader columnHeader)
        {
            if (columnHeader == null)
            { throw new ArgumentNullException(nameof(columnHeader)); }

            ColumnSort(columnHeader.DisplayIndex);
        }
        public void ColumnSort(int columnIndex, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (columnIndex < 0 || columnIndex >= Columns.Count)
            { throw new IndexOutOfRangeException(nameof(columnIndex)); }

            _sort(columnIndex, sortOrder, true);
        }
        //---------------------------------------------------------------------
    }
}
