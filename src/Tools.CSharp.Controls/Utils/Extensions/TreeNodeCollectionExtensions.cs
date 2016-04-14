using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tools.CSharp.Controls.Extensions
{
    public static class TreeNodeCollectionExtensions
    {
        //---------------------------------------------------------------------
        public static TreeNode FindObject<TObject>(this TreeNodeCollection nodesCollection, TObject obj)
            where TObject : class
        {
            if (nodesCollection == null)
            { throw new ArgumentNullException(nameof(nodesCollection)); }

            TreeNode findedNode = null;
            for (var i = 0; i < nodesCollection.Count; i++)
            {
                var node = nodesCollection[i];
                if (ReferenceEquals(node.Tag as TObject, obj))
                {
                    findedNode = node;
                    break;
                }
            }

            return findedNode;
        }
        //---------------------------------------------------------------------
        public static int BinarySearchInsertPosition(this TreeNodeCollection nodeCollection, TreeNode node, IComparer<TreeNode> comparer)
        {
            if (nodeCollection == null)
            { throw new ArgumentNullException(nameof(nodeCollection)); }
            if (node == null)
            { throw new ArgumentNullException(nameof(node)); }
            if (comparer == null)
            { throw new ArgumentNullException(nameof(comparer)); }

            var searchIndexResult = -1;

            var lastNodeIndex = nodeCollection.Count - 1;
            if (lastNodeIndex != -1)
            {
                var leftNodeIndex = 0;
                var rightNodeIndex = lastNodeIndex;

                do
                {
                    searchIndexResult = leftNodeIndex + (rightNodeIndex - leftNodeIndex) / 2;
                    var searchNode = nodeCollection[searchIndexResult];
                    if (searchNode != null)
                    {
                        var compareResult = comparer.Compare(searchNode, node);
                        if (compareResult < 0)
                        { leftNodeIndex = searchIndexResult + 1; }
                        else if (compareResult > 0)
                        { rightNodeIndex = searchIndexResult - 1; }
                        else
                        { break; }

                        if (leftNodeIndex > rightNodeIndex)
                        {
                            searchIndexResult = leftNodeIndex;
                            break;
                        }
                    }

                } while (true);
            }

            return searchIndexResult;
        }
        //---------------------------------------------------------------------
    }
}
