using System;
using System.Windows.Forms;
using Tools.CSharp.Collections;

namespace Tools.CSharp.MenuCreators
{
    public abstract class BaseDataImageItemMenuItemsCreator<TData, TSelectableCollection> :
        BaseDataItemMenuItemsCreator<DataImageItem<TData>, TData, TSelectableCollection>
        where TSelectableCollection : SelectableDataItemCollection<DataImageItem<TData>, TData>, new()
    {
        #region protected
        protected override ToolStripMenuItem CreateMenuItem(DataImageItem<TData> item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            return new ToolStripMenuItem(item.Name, item.Image) { ImageScaling = ToolStripItemImageScaling.None };
        }
        #endregion
    }
}