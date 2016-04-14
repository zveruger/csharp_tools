using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace Tools.CSharp.Controls
{
    public class MultiSelectTreeView : TreeView, IEnumerable<TreeNode>
    {
        #region private
        private const int _TvFirst = 0x1100;
        private const int _TvmSetBkColor = _TvFirst + 29;
        private const int _TvmSetExtendedStyle = _TvFirst + 44;
        private const int _TvsExDoubleBuffer = 0x0004;
        //----------------------------------------------------------------------
        private readonly HashSet<TreeNode> _selectedNodes = new HashSet<TreeNode>();
        private readonly object _locked = new object();
        //----------------------------------------------------------------------
        private TreeNode _selectedNode;
        private bool _isMultiSelectSameParent = true;
        private TreeNode _oldSelectedNode;
        //---------------------------------------------------------------------
        private struct TreeNodeCollectionEnumerator : IEnumerable<TreeNode>
        {
            #region private
            private readonly TreeNodeCollection _collection;
            #endregion
            public TreeNodeCollectionEnumerator(TreeNodeCollection collection)
            {
                _collection = collection;
            }

            //-----------------------------------------------------------------
            public IEnumerator<TreeNode> GetEnumerator()
            {
                for (var i = 0; i < _collection.Count; i++)
                {
                    yield return _collection[i];
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            //-----------------------------------------------------------------
        }
        internal sealed class NativeInterop
        {
            public const int WmSetfont = 0x0030;
            public const int WmPrintclient = 0x0318;
            public const int PrfClient = 0x00000004;

            public const int WmReflect = 0x2000;   // WM_USER + 0x1C00;
            public const int WmNotify = 0x004E;

            // CustomDraw paint stages
            public const int CddsPrepaint = 0x1;
            public const int CddsItem = 0x10000;
            public const int CddsItemprepaint = CddsItem | CddsPrepaint;

            // Common notification codes (WM_NOTIFY)
            public const int NmFirst = 0;
            public const int NmCustomdraw = (NmFirst - 12);

            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            };

            [StructLayout(LayoutKind.Sequential)]
            public struct Nmhdr
            {
                public IntPtr hwndFrom;
                public IntPtr idFrom;
                public int code;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Nmcustomdraw
            {
                public Nmhdr hdr;
                public int dwDrawStage;
                public IntPtr hdc;
                public Rect rc;
                public IntPtr dwItemSpec;
                public int uItemState;
                public int lItemlParam;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            public static bool IsWinXp
            {
                get
                {
                    var osVersion = Environment.OSVersion;
                    return (osVersion.Platform == PlatformID.Win32NT) && ((osVersion.Version.Major > 5) || ((osVersion.Version.Major == 5) && (osVersion.Version.Minor == 1)));
                }
            }
            public static bool IsWinVista
            {
                get
                {
                    var osVersion = Environment.OSVersion;
                    return (osVersion.Platform == PlatformID.Win32NT) && (osVersion.Version.Major >= 6);
                }
            }
        }
        //--------------------------------------------------------------------
        private void _simpleClearNode(TreeNode node)
        {
            node.BackColor = BackColor;
            node.ForeColor = ForeColor;
        }
        private void _simpleSelectNode(TreeNode node)
        {
            node.BackColor = SystemColors.Highlight;
            node.ForeColor = SystemColors.HighlightText;
        }
        private TreeNode _simpleGetSelectedNode()
        {
            using (var selectedNodesEnumerator = _selectedNodes.GetEnumerator())
            { return selectedNodesEnumerator.MoveNext() ? selectedNodesEnumerator.Current : null; }
        }
        //---------------------------------------------------------------------
        private void _fastClearSelectedNodes(IEnumerable<TreeNode> nodes, bool parallel = true)
        {
            var action = new Func<TreeNode, TreeNode>(node =>
            {
                if (node != null)
                {
                    _simpleClearNode(node);
                    _fastClearSelectedNodes(new TreeNodeCollectionEnumerator(node.Nodes), false);
                }

                return node;
            });

            if (parallel)
            { Parallel.ForEach(nodes, node => action(node)); }
            else
            {
                foreach (var node in nodes)
                { action(node); }
            }
        }
        private void _fastClearSelectedNodes(bool removeSelectedNodeOnlyParentNotNull)
        {
            try
            {
                _fastClearSelectedNodes(_selectedNodes, removeSelectedNodeOnlyParentNotNull);
            }
            finally
            {
                _oldSelectedNode = null;
                _selectedNodes.Clear();
                _selectedNode = null;
            }
        }
        //---------------------------------------------------------------------
        private void _selectSingleNode(TreeNode node)
        {
            if (ReferenceEquals(_selectedNode, node))
            { return; }

            if (node != null)
            {
                _fastClearSelectedNodes(false);

                _toggleNode(node, true);
                node.EnsureVisible();
            }
            _onRaiseAfterSelect(node);
        }
        private void _selectNode(TreeNode node)
        {
            if (node != null)
            {
                if (_selectedNode == null || ModifierKeys == Keys.Control)
                {
                    if (_isMultiSelectSameParent && ModifierKeys == Keys.Control)
                    {
                        if (node.Parent != null)
                        { _fastClearSelectedNodes(false); }
                    }

                    //Ctrl+Click selects an unselected node, or unselects a selected node.
                    var isSelectedNode = _selectedNodes.Contains(node);
                    _toggleNode(node, !isSelectedNode);
                }
                else if (!_isMultiSelectSameParent && ModifierKeys == Keys.Shift)
                {
                    //Shift+Click selects nodes between the selected node and here.
                    var nodeStart = _selectedNode;
                    var nodeEnd = node;

                    if (nodeStart.Parent == nodeEnd.Parent)
                    {
                        //Selected node and clicked node have same parent, easy case.
                        if (nodeStart.Index < nodeEnd.Index)
                        { _nextVisibleNodeByToggleNode(nodeStart, nodeEnd); }
                        else if (nodeStart.Index > nodeEnd.Index)
                        { _prevVisibleNodeByToggleNode(nodeStart, nodeEnd); }
                    }
                    else
                    {
                        //Selected node and clicked node have same parent, hard case.
                        //We need to find a common parent to determine if we need
                        //to walk down selecting, or walk up selecting.
                        var nodeStartPreview = nodeStart;
                        var nodeEndPreview = nodeEnd;
                        var startDepth = Math.Min(nodeStartPreview.Level, nodeEndPreview.Level);

                        //Bring lower node up to common depth
                        while (nodeStartPreview.Level > startDepth)
                        { nodeStartPreview = nodeStartPreview.Parent; }

                        //Bring lower node up to common depth
                        while (nodeEndPreview.Level > startDepth)
                        { nodeEndPreview = nodeEndPreview.Parent; }

                        //Walk up the tree until we find the common parent
                        while (nodeStartPreview.Parent != nodeEndPreview.Parent)
                        {
                            nodeStartPreview = nodeStartPreview.Parent;
                            nodeEndPreview = nodeEndPreview.Parent;
                        }

                        if (nodeStartPreview.Index < nodeEndPreview.Index)
                        { _nextVisibleNodeByToggleNode(nodeStart, nodeEnd); }
                        else if (nodeStartPreview.Index == nodeEndPreview.Index)
                        {
                            if (nodeStart.Level < nodeEnd.Level)
                            { _nextVisibleNodeByToggleNode(nodeStart, nodeEnd); }
                            else
                            { _prevVisibleNodeByToggleNode(nodeStart, nodeEnd); }
                        }
                        else
                        { _prevVisibleNodeByToggleNode(nodeStart, nodeEnd); }
                    }
                }
                else
                { _selectSingleNode(node); }
            }
            _onRaiseAfterSelect(_selectedNode);
        }
        //----------------------------------------------------------------------
        private void _toggleNode(TreeNode node, bool selectNode, bool setSelectedNode = true)
        {
            lock (_locked)
            {
                if (node != null)
                {
                    if (selectNode)
                    {
                        _simpleSelectNode(node);

                        _selectedNodes.Add(node);

                        if (setSelectedNode)
                        { _selectedNode = node; }
                    }
                    else
                    {
                        _simpleClearNode(node);
                        _selectedNodes.Remove(node);
                    }
                }
            }
        }

        private void _nextVisibleNodeByToggleNode(TreeNode nodeStart, TreeNode nodeEnd)
        {
            var startNode = nodeStart;
            var endNode = nodeEnd;

            while (startNode != endNode)
            {
                startNode = startNode.NextVisibleNode;
                if (startNode == null)
                { break; }
                _toggleNode(startNode, true);
            }
        }
        private void _prevVisibleNodeByToggleNode(TreeNode nodeStart, TreeNode nodeEnd)
        {
            var startNode = nodeStart;
            var endNode = nodeEnd;

            while (startNode != endNode)
            {
                startNode = startNode.PrevVisibleNode;
                if (startNode == null)
                { break; }
                _toggleNode(startNode, true);
            }
        }
        //---------------------------------------------------------------------
        private void _onRaiseAfterSelect(TreeNode node)
        {
            if (!ReferenceEquals(_oldSelectedNode, node) || node == null)
            {
                _oldSelectedNode = node;
                OnAfterSelect(new TreeViewEventArgs(node));
            }
        }
        #endregion
        #region protected
        protected override void OnGotFocus(EventArgs e)
        {
            //Make sure at least one node has a selection
            //this way we can tab to the ctrl and use the 
            //keyboard to select nodes
            if (_selectedNode == null && TopNode != null)
            { _toggleNode(TopNode, true); }

            base.OnGotFocus(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // If the clicked on a node that WAS previously
            // selected then, reselect it now. This will clear
            // any other selected nodes. e.g. A B C D are selected
            // the user clicks on B, now A C & D are no longer selected.
            var node = GetNodeAt(e.Location);
            if (node != null)
            {
                if (ModifierKeys == Keys.None && _selectedNodes.Contains(node))
                {
                    var leftBound = node.Bounds.X; //Allow user to click on image
                    var rightBound = node.Bounds.Right + 10; //Give a little extra room
                    if (e.Location.X > leftBound && e.Location.X < rightBound)
                    { _selectNode(node); }
                }
            }

            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.SelectedNode = null;

            var node = GetNodeAt(e.Location);
            if (node != null)
            {
                var leftBound = node.Bounds.X; //Allow user to click on image
                var rightBound = node.Bounds.Right + 10; // Give a little extra room
                if (e.Location.X > leftBound && e.Location.X < rightBound)
                {
                    if (ModifierKeys == Keys.None && _selectedNodes.Contains(node))
                    {
                        //Potential Drag Operation
                        //Let Mouse Up do select
                    }
                    else
                    { _selectNode(node); }
                }
            }

            base.OnMouseDown(e);
        }
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            var node = e.Item as TreeNode;
            if (node != null && !_selectedNodes.Contains(node))
            { _selectSingleNode(node); }

            base.OnItemDrag(e);
        }
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.SelectedNode = null;
            e.Cancel = true;

            base.OnBeforeSelect(e);
        }
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.SelectedNode = null;

            base.OnAfterSelect(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.ShiftKey)
            { return; }

            var buttonShift = ModifierKeys == Keys.Shift;

            //Nothing is selected in the tree, this isn't a good state
            //select the top node
            if (_selectedNode == null && TopNode != null)
            { _toggleNode(TopNode, true); }

            //Nothing is still selected in the tree, this isn't a good state, leave.
            if (_selectedNode == null)
            { return; }

            switch (e.KeyCode)
            {
                case Keys.Left:
                    {
                        if (_selectedNode.IsExpanded && _selectedNode.Nodes.Count > 0)
                        { _selectedNode.Collapse(); }
                        else if (_selectedNode.Parent != null)
                        { _selectSingleNode(_selectedNode.Parent); }
                    }
                    break;
                case Keys.Right:
                    {
                        if (_selectedNode.IsExpanded)
                        { _selectSingleNode(_selectedNode.FirstNode); }
                        else
                        { _selectedNode.Expand(); }
                    }
                    break;
                case Keys.Up:
                    {
                        if (_selectedNode.PrevVisibleNode != null)
                        { _selectNode(_selectedNode.PrevVisibleNode); }
                    }
                    break;
                case Keys.Down:
                    {
                        if (_selectedNode.NextVisibleNode != null)
                        { _selectNode(_selectedNode.NextVisibleNode); }
                    }
                    break;
                case Keys.Home:
                    {
                        if (buttonShift)
                        {
                            if (_selectedNode.Parent == null)
                            {
                                if (Nodes.Count > 0)
                                { _selectNode(Nodes[0]); }
                            }
                            else
                            { _selectNode(_selectedNode.Parent.FirstNode); }
                        }
                        //Select this first node in the tree
                        else if (Nodes.Count > 0)
                        { _selectSingleNode(Nodes[0]); }
                    }
                    break;
                case Keys.End:
                    {
                        if (buttonShift)
                        {
                            if (_selectedNode.Parent == null)
                            {
                                //Select the last ROOT node in the tree
                                if (Nodes.Count > 0)
                                { _selectNode(Nodes[Nodes.Count - 1]); }
                            }
                            else
                            { _selectNode(_selectedNode.Parent.LastNode); }
                        }
                        else if (Nodes.Count > 0)
                        {
                            //Select the last node visible node in the tree.
                            //Don't expand branches incase the tree is virtual
                            var lastNode = Nodes[0].LastNode;
                            while (lastNode.IsExpanded && lastNode.LastNode != null)
                            { lastNode = lastNode.LastNode; }

                            _selectSingleNode(lastNode);
                        }
                    }
                    break;
                case Keys.PageUp:
                    {
                        var nodeCount = VisibleCount;
                        var currentNode = _selectedNode;
                        while (nodeCount > 0 && currentNode.PrevVisibleNode != null)
                        {
                            currentNode = currentNode.PrevVisibleNode;
                            nodeCount--;
                        }
                        _selectSingleNode(currentNode);
                    }
                    break;
                case Keys.PageDown:
                    {
                        var nodeCount = VisibleCount;
                        var currentNode = _selectedNode;
                        while (nodeCount > 0 && currentNode.NextVisibleNode != null)
                        {
                            currentNode = currentNode.NextVisibleNode;
                            nodeCount--;
                        }
                        _selectSingleNode(currentNode);
                    }
                    break;
                default:
                    {
                        //if (ModifierKeys == Keys.None)
                        //{
                        //    //Assume this is a search character a-z, A-Z, 0-9, etc.
                        //    //Select the first node after the current node that 
                        //    //starts with this character
                        //    var keyValue = ((char)e.KeyValue).ToString();
                        //    var currentNode = _selectedNode;
                        //    while (currentNode.NextVisibleNode != null)
                        //    {
                        //        currentNode = currentNode.NextVisibleNode;
                        //        if (currentNode.Text.StartsWith(keyValue))
                        //        {
                        //            _selectSingleNode(currentNode);
                        //            break;
                        //        }
                        //    }
                        //}
                    }
                    break;
            }
        }
        //---------------------------------------------------------------------
        //http://www.codeproject.com/Articles/37253/Double-buffered-Tree-and-Listviews
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (DoubleBuffered)
            { NativeInterop.SendMessage(Handle, _TvmSetExtendedStyle, (IntPtr)_TvsExDoubleBuffer, (IntPtr)_TvsExDoubleBuffer); }

            if (!NativeInterop.IsWinXp)
            { NativeInterop.SendMessage(Handle, _TvmSetBkColor, IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(BackColor)); }
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (NativeInterop.WmNotify | NativeInterop.WmReflect))
            {
                var nmhdr = (NativeInterop.Nmhdr)m.GetLParam(typeof(NativeInterop.Nmhdr));
                if (nmhdr.hwndFrom == Handle && nmhdr.code == NativeInterop.NmCustomdraw)
                {
                    var nmCustomDraw = (NativeInterop.Nmcustomdraw)m.GetLParam(typeof(NativeInterop.Nmcustomdraw));
                    if (nmCustomDraw.dwDrawStage == NativeInterop.CddsPrepaint /*CDDS_ITEMPREPAINT - individual NodeFont*/)
                    {
                        var rcUpdateRgn = Rectangle.FromLTRB(nmCustomDraw.rc.Left, nmCustomDraw.rc.Top, nmCustomDraw.rc.Right, nmCustomDraw.rc.Bottom);
                        if (rcUpdateRgn.IsEmpty)
                        {
                            //individual NodeFont
                            //var tn = TreeNode.FromHandle(this, nmCustomDraw.dwItemSpec);
                            //if (tn == null || tn.NodeFont == null)
                            return;
                        }
                    }
                }
            }
            base.WndProc(ref m);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                var m = new Message
                {
                    HWnd = Handle,
                    Msg = NativeInterop.WmPrintclient,
                    WParam = e.Graphics.GetHdc(),
                    LParam = (IntPtr)NativeInterop.PrfClient
                };
                DefWndProc(ref m);
                e.Graphics.ReleaseHdc(m.WParam);
            }
            base.OnPaint(e);
        }
        #endregion
        public MultiSelectTreeView()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            if (!NativeInterop.IsWinVista) { SetStyle(ControlStyles.UserPaint, true); }
            UpdateStyles();

            base.SelectedNode = null;
            Font = new Font("Arial", 9F);
        }

        //---------------------------------------------------------------------
        [Browsable(true), DefaultValue(typeof(bool), "True")]
        public bool IsMultiSelectSameParent
        {
            get { return _isMultiSelectSameParent; }
            set
            {
                if (_isMultiSelectSameParent != value)
                {
                    _isMultiSelectSameParent = value;
                    _fastClearSelectedNodes(_isMultiSelectSameParent);
                }
            }
        }
        //---------------------------------------------------------------------
        [DefaultValue(typeof(Font), "Arial, 9pt")]
        public new Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;

                if (GetStyle(ControlStyles.UserPaint))
                { NativeInterop.SendMessage(Handle, NativeInterop.WmSetfont, Font.ToHfont(), IntPtr.Zero); }
            }
        }
        //---------------------------------------------------------------------
        public new TreeNode SelectedNode
        {
            get { return _selectedNode; }
            set { _selectSingleNode(value); }
        }
        public IEnumerable<TreeNode> SelectedNodes
        {
            get { return _selectedNodes; }
            set
            {
                if (!ReferenceEquals(_selectedNodes, value))
                {
                    SuspendLayout();

                    _fastClearSelectedNodes(false);

                    if (value != null)
                    {
                        Parallel.ForEach(value, node => _toggleNode(node, true, false));
                        _selectedNode = _simpleGetSelectedNode();
                    }
                    _onRaiseAfterSelect(_selectedNode);

                    ResumeLayout(false);
                }

            }
        }
        public TreeNode GetFirstSelectedNode()
        {
            var treeNode = _simpleGetSelectedNode();
            return treeNode;
        }
        public int SelectedNodesCount
        {
            get { return _selectedNodes.Count; }
        }
        //---------------------------------------------------------------------
        public bool RemoveSelectedNode(TreeNode node)
        {
            return _selectedNodes.Remove(node);
        }
        public void ClearSelectedNodes()
        {
            _fastClearSelectedNodes(false);
            _onRaiseAfterSelect(null);
        }
        //---------------------------------------------------------------------
        public void RemoveNode(TreeNode node)
        {
            if (node == null)
            { throw new ArgumentNullException(nameof(node)); }

            var nodeIndex = node.Index;
            Nodes.RemoveAt(nodeIndex);

            TreeNode nextSelectedNode = null;
            var nodeCount = Nodes.Count;
            if (RemoveSelectedNode(node) && SelectedNodesCount != 0)
            {
                if (nodeCount != 0)
                { nextSelectedNode = nodeIndex < nodeCount ? Nodes[nodeIndex] : Nodes[nodeCount - 1]; }
            }

            if (nodeCount == 0)
            { ClearSelectedNodes(); }
            else
            { SelectedNode = nextSelectedNode; }
        }
        //---------------------------------------------------------------------
        public IEnumerator<TreeNode> GetEnumerator()
        {
            return new TreeNodeCollectionEnumerator(Nodes).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
}
