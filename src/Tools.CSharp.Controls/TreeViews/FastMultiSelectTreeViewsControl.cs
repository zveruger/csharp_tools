using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools.CSharp.Controls.Extensions;

namespace Tools.CSharp.Controls
{
    //-------------------------------------------------------------------------
    public sealed class FastMultiSelectTreeViewsControl : UserControl
    {
        #region private
        private const int _MinAvailableSubstitute = 1000;
        private const int _DeltaAvailableSubstitute = _MinAvailableSubstitute / 100;

        private readonly static object _Lock = new object();
        //-----------------------------------------------------------------------
        private static TreeNode _tmpSelectNode;
        //-----------------------------------------------------------------------
        private struct IntComparer : IComparer<int>
        {
            //-----------------------------------------------------------------
            public int Compare(int x, int y)
            {
                return y.CompareTo(x);
            }
            //-----------------------------------------------------------------
        }
        //-----------------------------------------------------------------------
        private sealed class MultiSelectTreeViewDoEvents : MultiSelectTreeView
        {
            //-----------------------------------------------------------------
            public void RemoveAt(int index)
            {
                Application.DoEvents();
                Nodes.RemoveAt(index);
                Application.DoEvents();
            }
            //-----------------------------------------------------------------
        }

        private MultiSelectTreeViewDoEvents _createTreeViewClone(MultiSelectTreeView treeView)
        {
            var treeViewClone = new MultiSelectTreeViewDoEvents
            {
                Name = treeView.Name,
                Location = treeView.Location,
                Size = treeView.Size,
                TabIndex = treeView.TabIndex,
                BackColor = treeView.BackColor,
                Visible = treeView.Visible,
                Enabled = treeView.Enabled,
                ForeColor = treeView.ForeColor,
                ContextMenuStrip = treeView.ContextMenuStrip,
                Dock = treeView.Dock,
                ImageIndex = treeView.ImageIndex,
                ImageList = treeView.ImageList,
                SelectedImageIndex = treeView.SelectedImageIndex,
                Font = treeView.Font,
                IsMultiSelectSameParent = treeView.IsMultiSelectSameParent,
                StateImageList = treeView.StateImageList,
                ShowRootLines = treeView.ShowRootLines,
                ShowPlusMinus = treeView.ShowPlusMinus,
                ShowNodeToolTips = treeView.ShowNodeToolTips,
                ShowLines = treeView.ShowLines,
                Scrollable = treeView.Scrollable,
                AutoScrollOffset = treeView.AutoScrollOffset,
                PathSeparator = treeView.PathSeparator,
                Margin = treeView.Margin,
                LineColor = treeView.LineColor,
                LabelEdit = treeView.LabelEdit,
                ItemHeight = treeView.ItemHeight,
                HotTracking = treeView.HotTracking,
                HideSelection = treeView.HideSelection,
                FullRowSelect = treeView.FullRowSelect,
                Cursor = treeView.Cursor,
                CheckBoxes = treeView.CheckBoxes,
                BorderStyle = treeView.BorderStyle,
                AllowDrop = treeView.AllowDrop,
                AccessibleDescription = treeView.AccessibleDescription,
                AccessibleName = treeView.AccessibleName
            };

            _onCreateTreeView(treeViewClone, treeView);
            return treeViewClone;
        }
        private void _onCreateTreeView(MultiSelectTreeView newTreeView, MultiSelectTreeView oldTreeView)
        {
            CreateTreeView?.Invoke(this, new FastTreeViewsControlEventArgs(newTreeView, oldTreeView));
        }
        //-----------------------------------------------------------------------
        private static void _AsyncRemoveTreeView(MultiSelectTreeViewDoEvents treeView)
        {
            var thread = new Thread(() => _RemoveTreeView(treeView));
            thread.Start();
        }
        private static void _RemoveTreeView(MultiSelectTreeViewDoEvents treeView)
        {
            if (!treeView.IsDisposed)
            {
                var action = new Action(() =>
                {
                    treeView.Parent = null;
                    treeView.Size = new Size(1, 1);

                    treeView.BeginUpdate();
                    while (treeView.Nodes.Count != 0)
                    { treeView.RemoveAt(treeView.Nodes.Count - 1); }
                    treeView.EndUpdate();

                    treeView.Dispose();

                    GC.Collect();
                });

                try
                {
                    if (treeView.InvokeRequired)
                    { treeView.Invoke(() => action()); }
                    else
                    { action(); }
                }
                catch (Exception)
                { }
            }
        }
        //-----------------------------------------------------------------------
        private bool _availableSubstitute(int actionCount)
        {
            var nodesCount = _getTreeView.Nodes.Count;
            var moreMinAvailableSubstitute = nodesCount / _MinAvailableSubstitute;
            var numberLentgth = (int)Math.Log10(moreMinAvailableSubstitute);
            var delta = moreMinAvailableSubstitute == 1 ? 1 : (int)Math.Pow(10, numberLentgth);
            var result = nodesCount != 0 && (actionCount * delta * _DeltaAvailableSubstitute) >= nodesCount;

            return result;
        }
        private MultiSelectTreeViewDoEvents _substituteTreeView(MultiSelectTreeViewDoEvents oldTreeView)
        {
            var newTreeView = _createTreeViewClone(oldTreeView);

            Controls.Add(newTreeView);

            return newTreeView;
        }
        //-----------------------------------------------------------------------

        private static TreeNode[] _createTreeNodeArray(MultiSelectTreeView treeView, ICollection<int> excludingNodesIndexes, int selecteNodeIndex, out TreeNode selectNode)
        {
            _tmpSelectNode = null;

            var nodesCount = treeView.Nodes.Count - excludingNodesIndexes.Count;
            var nodes = new List<TreeNode>(nodesCount);

            Parallel.ForEach(treeView, node =>
            {
                if (!excludingNodesIndexes.Contains(node.Index))
                {
                    var newNode = (TreeNode)node.Clone();
                    if (node.Index == selecteNodeIndex)
                    { _tmpSelectNode = newNode; }

                    lock (_Lock)
                    { nodes.Add(newNode); }
                }
            });

            selectNode = _tmpSelectNode;

            return nodes.ToArray();
        }

        private static IList<int> _createSortNodeIndexs(IEnumerable<TreeNode> collection)
        {
            var comparer = new IntComparer();
            var nodeIndexs = new List<int>(collection.Select(t => t.Index).OrderBy(x => x, comparer));
            return nodeIndexs;
        }
        //-----------------------------------------------------------------------
        private MultiSelectTreeViewDoEvents _getTreeView
        {
            get
            {
                var treeView = (MultiSelectTreeViewDoEvents)Controls[0];
                return treeView;
            }
        }
        private static TreeNode _getNextTreeNode(TreeNode node)
        {
            if (node == null || node.Index == -1)
            { return null; }

            var nextNode = node.NextNode;
            return Interlocked.CompareExchange(ref nextNode, node.PrevNode, null);
        }
        //-----------------------------------------------------------------------
        private MultiSelectTreeView _fastClearNodes(MultiSelectTreeViewDoEvents treeView)
        {
            var tmpTreeView = treeView;
            if (tmpTreeView != null)
            {
                if (_availableSubstitute(tmpTreeView.Nodes.Count))
                {
                    var newTreeView = _substituteTreeView(tmpTreeView);

                    _switchTreeViews(tmpTreeView, newTreeView);

                    return newTreeView;
                }

                tmpTreeView.ClearSelectedNodes();
                tmpTreeView.Nodes.Clear();
            }

            return tmpTreeView;
        }
        private TreeNode _fastRemoveNodes(MultiSelectTreeViewDoEvents treeView, int removeNodesCount, IEnumerable<TreeNode> removeNodes)
        {
            TreeNode nextNode = null;

            if (treeView.Nodes.Count == removeNodesCount)
            { _fastClearNodes(treeView); }
            else
            {
                var removeNodesIndexes = _createSortNodeIndexs(removeNodes);
                var lastRemoveNodeIndex = removeNodesIndexes.Count == 0 ? -1 : removeNodesIndexes[0];
                nextNode = _getNextTreeNode((lastRemoveNodeIndex > -1 && lastRemoveNodeIndex < treeView.Nodes.Count) ? treeView.Nodes[lastRemoveNodeIndex] : null);

                if (_availableSubstitute(removeNodesCount))
                {
                    var nodes = _createTreeNodeArray(treeView, removeNodesIndexes, nextNode?.Index ?? -1, out nextNode);

                    var newTreeView = _substituteTreeView(treeView);
                    newTreeView.Nodes.AddRange(nodes);

                    _switchTreeViews(treeView, newTreeView);
                }
                else
                {
                    treeView.BeginUpdate();

                    foreach (var removeNodesIndex in removeNodesIndexes.Where(removeNodesIndex => (removeNodesIndex > -1) && (removeNodesIndex < treeView.Nodes.Count)))
                    { treeView.Nodes.RemoveAt(removeNodesIndex); }

                    treeView.EndUpdate();
                }
            }

            return nextNode;
        }
        private void _switchTreeViews(MultiSelectTreeViewDoEvents oldTreeView, MultiSelectTreeViewDoEvents newTreeView)
        {
            if (oldTreeView != null && newTreeView != null)
            {
                Controls.SetChildIndex(oldTreeView, Controls.Count - 1);
                oldTreeView.Visible = false;

                _AsyncRemoveTreeView(oldTreeView);
            }
        }
        //-----------------------------------------------------------------------
        private void _selectedNode(TreeNode node)
        {
            var treeView = _getTreeView;
            _selectedNode(node, treeView);
        }
        private static void _selectedNode(TreeNode node, MultiSelectTreeView treeView)
        {
            if (treeView != null)
            {
                var selectedNode = node;

                if (treeView.Nodes.IndexOf(node) == -1 && treeView.Nodes.Count != 0)
                { selectedNode = treeView.Nodes[0]; }

                treeView.SelectedNode = selectedNode;
            }
        }
        #endregion
        #region protected
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                try
                {
                    if (disposing)
                    { }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
        #endregion
        public FastMultiSelectTreeViewsControl()
        {
            var treeView = new MultiSelectTreeViewDoEvents { Dock = DockStyle.Fill };
            Controls.Add(treeView);

            Font = new Font("Arial", 9F);
        }

        //---------------------------------------------------------------------
        [Browsable(true)]
        public MultiSelectTreeView TreeView
        {
            get { return _getTreeView; }
        }
        //---------------------------------------------------------------------
        [Browsable(true)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                var treeView = _getTreeView;
                if (treeView != null)
                { treeView.BackColor = value; }

                base.BackColor = value;
            }
        }
        [Browsable(true)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                var treeView = _getTreeView;
                if (treeView != null)
                { treeView.ForeColor = value; }

                base.ForeColor = value;
            }
        }
        //---------------------------------------------------------------------
        [Browsable(true), DefaultValue(typeof(Font), "Arial, 9pt")]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                var treeView = _getTreeView;
                if (treeView != null)
                { treeView.Font = value; }

                base.Font = value;
            }
        }
        //---------------------------------------------------------------------
        [Browsable(true)]
        public new bool Visible
        {
            get { return base.Visible; }
            set
            {
                var treeView = _getTreeView;
                if (treeView != null)
                { treeView.Visible = value; }

                base.Visible = value;
            }
        }

        [Browsable(true)]
        public new bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                var treeView = _getTreeView;
                if (treeView != null)
                { treeView.Enabled = value; }

                base.Enabled = value;
            }
        }
        //---------------------------------------------------------------------
        public void FastRemoveNodes(TreeNode[] treeNodes)
        {
            if (treeNodes == null)
            { throw new ArgumentNullException(nameof(treeNodes)); }

            if (treeNodes.Length != 0)
            {
                var treeView = _getTreeView;
                if (treeView != null)
                {
                    if (treeView.Nodes.Count < treeNodes.Length)
                    { throw new ArgumentOutOfRangeException(nameof(treeNodes), treeNodes.Length, ""); }

                    var nextNode = _fastRemoveNodes(treeView, treeNodes.Length, treeNodes);
                    _selectedNode(nextNode);
                }
            }
        }
        public void FastRemoveSelectedNodes()
        {
            var treeView = _getTreeView;
            if (treeView != null && treeView.SelectedNodesCount != 0)
            {
                var nextNode = _fastRemoveNodes(treeView, treeView.SelectedNodesCount, new List<TreeNode>(treeView.SelectedNodes));
                _selectedNode(nextNode);
            }
        }
        public MultiSelectTreeView FastClearNodes()
        {
            return _fastClearNodes(_getTreeView);
        }
        //---------------------------------------------------------------------
        public event EventHandler<FastTreeViewsControlEventArgs> CreateTreeView;
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public sealed class FastTreeViewsControlEventArgs : EventArgs
    {
        #region private
        private readonly MultiSelectTreeView _newTreeView;
        private readonly MultiSelectTreeView _oldTreeView;
        #endregion
        internal FastTreeViewsControlEventArgs(MultiSelectTreeView newTreeView, MultiSelectTreeView oldTreeView)
        {
            _newTreeView = newTreeView;
            _oldTreeView = oldTreeView;
        }

        //---------------------------------------------------------------------
        public MultiSelectTreeView NewTreeView
        {
            get { return _newTreeView; }
        }
        public MultiSelectTreeView OldTreeView
        {
            get { return _oldTreeView; }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}
