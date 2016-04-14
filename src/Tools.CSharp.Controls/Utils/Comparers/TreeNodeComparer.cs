using System.Collections.Generic;
using System.Windows.Forms;
using Tools.CSharp.Collections;

namespace Tools.CSharp.Comparers
{
    public abstract class TreeNodeComparer : IComparer<TreeNode>
    {
        #region private
        private SortDirection _direction = SortDirection.Asc;
        #endregion
        #region protected
        protected int ApplyDirection(int arg)
        {
            return _direction == SortDirection.Asc ? arg : -arg;
        }
        //---------------------------------------------------------------------
        protected virtual int CompareCore(TreeNode x, TreeNode y)
        {
            return string.CompareOrdinal(x.Text, y.Text);
        }
        #endregion
        //---------------------------------------------------------------------
        public SortDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        //-----------------------------------------------------------------
        public int Compare(TreeNode x, TreeNode y)
        {
            return ApplyDirection(CompareCore(x, y));
        }
        //-----------------------------------------------------------------
    }
}