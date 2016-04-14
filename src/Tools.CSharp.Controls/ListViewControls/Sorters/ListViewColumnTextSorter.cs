using System.Collections;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public class ListViewColumnTextSorter : ListViewColumnSorter
    {
        #region private
        private readonly CaseInsensitiveComparer _objectComparer;
        #endregion
        #region protected
        protected override int Compar(ListViewItem x, ListViewItem y)
        {
            var listViewXText = x.SubItems[SortColumn].Text;
            var listViewYText = y.SubItems[SortColumn].Text;

            return Compar(listViewXText, listViewYText);
        }
        protected virtual int Compar(string x, string y)
        {
            var compareResult = _objectComparer.Compare(x, y);
            return CreateCompareResult(compareResult);
        }
        #endregion
        public ListViewColumnTextSorter()
        {
            _objectComparer = new CaseInsensitiveComparer();
        }
    }
}
