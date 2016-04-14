using System;
using System.Windows.Forms;
using Tools.CSharp.Collections;

namespace Tools.CSharp.MenuCreators
{
    public abstract class BaseDataImageHelpItemMenuItemsCreator<TData, TSelectableCollection> :
        BaseDataItemMenuItemsCreator<DataImageHelpItem<TData>, TData, TSelectableCollection>
        where TSelectableCollection : SelectableDataItemCollection<DataImageHelpItem<TData>, TData>, new()
    {
        #region protected
        protected override ToolStripMenuItem CreateMenuItem(DataImageHelpItem<TData> item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            return new ToolStripMenuItem(item.Name, item.Image) { ImageScaling = ToolStripItemImageScaling.None, ToolTipText = item.Help };
        }
        #endregion
    }
}