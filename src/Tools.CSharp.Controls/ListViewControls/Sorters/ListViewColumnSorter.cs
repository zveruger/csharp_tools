using System.Collections;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public abstract class ListViewColumnSorter : IComparer
    {
        #region private
        private int _columnToSort;
        private SortOrder _sorting;
        #endregion
        #region protected
        protected int CreateCompareResult(int compareResult)
        {
            if (Sorting == SortOrder.Ascending)
            { return compareResult; }

            if (Sorting == SortOrder.Descending)
            { return (-compareResult); }

            return 0;
        }
        protected abstract int Compar(ListViewItem x, ListViewItem y);
        #endregion
        protected ListViewColumnSorter()
        {
            _columnToSort = 0;
            _sorting = SortOrder.None;
        }

        //---------------------------------------------------------------------
        public int SortColumn
        {
            set { _columnToSort = value; }
            get { return _columnToSort; }
        }
        public SortOrder Sorting
        {
            set { _sorting = value; }
            get { return _sorting; }
        }
        //---------------------------------------------------------------------
        public int Compare(object x, object y)
        {
            var listViewX = (ListViewItem)x;
            var listViewY = (ListViewItem)y;

            return Compar(listViewX, listViewY);
        }
        //---------------------------------------------------------------------
    }
}
