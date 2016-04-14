using System.ComponentModel;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public class SorterListViewColumnHeader : ColumnHeader
    {
        #region private
        private ListViewColumnSorter _sorter;
        #endregion
        //---------------------------------------------------------------------
        [DefaultValue(typeof(ListViewColumnSorter), null)]
        public ListViewColumnSorter Sorter
        {
            get { return _sorter; }
            set
            {
                _sorter = value;

                if (_sorter != null)
                { _sorter.SortColumn = DisplayIndex; }
            }
        }
        //---------------------------------------------------------------------
    }
}
