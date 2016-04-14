using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Tools.CSharp.Collections;

namespace Tools.CSharp.MenuCreators
{
    //-------------------------------------------------------------------------
    public abstract class BaseDataItemMenuItemsCreator<TDataItem, TData, TSelectableCollection> : BaseDisposable
        where TDataItem : DataItem<TData>
        where TSelectableCollection : SelectableDataItemCollection<TDataItem, TData>, new()
    {
        #region private
        private TSelectableCollection _dataItemCollection;
        //---------------------------------------------------------------------
        private TSelectableCollection _DataItemCollection
        {
            get { return LazyInitializer.EnsureInitialized(ref _dataItemCollection, () => new TSelectableCollection()); }
        }
        #endregion
        #region protected
        protected abstract ToolStripMenuItem CreateMenuItem(TDataItem item);
        //---------------------------------------------------------------------
        protected bool GetDataItem<TMenuItem>(TMenuItem menuItem, out TDataItem dataItem) 
            where TMenuItem : ToolStripItem
        {
            if (menuItem == null)
            { throw new ArgumentNullException(nameof(menuItem)); }

            dataItem = menuItem.Tag as TDataItem;
            return dataItem != null;
        }
        //---------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    _dataItemCollection?.Dispose();
                }
            }
            finally
            { base.Dispose(disposing); }
            
        }
        #endregion
        //---------------------------------------------------------------------
        public TDataItem SelectedItem
        {
            get { return _DataItemCollection.SelectedItem; }
        }
        public int SelectedIndex
        {
            get { return _DataItemCollection.SelectedIndex; }
            set { _DataItemCollection.SelectedIndex = value; }
        }
        public TData SelectedValue
        {
            get { return _DataItemCollection.SelectedDate; }
            set { _DataItemCollection.SelectedDate = value; }
        }
        //---------------------------------------------------------------------
        public bool SetSelectedValue<TMenuItem>(TMenuItem menuItem) 
            where TMenuItem : ToolStripItem
        {
            var dataItem = menuItem?.Tag as TDataItem;
            if (dataItem != null)
            {
                var newValue = dataItem.Data;
                var oldValue = SelectedValue;

                SelectedValue = newValue;
                return newValue.Equals(oldValue);
            }
            return false;
        }
        //---------------------------------------------------------------------
        public ToolStripMenuItem[] CreateMenuItems()
        {
            var menuItems = new List<ToolStripMenuItem>();
            var dataItemCollection = _DataItemCollection;

            foreach (var dataItem in dataItemCollection)
            {
                if (dataItem != null)
                {
                    var menuItem = CreateMenuItem(dataItem);
                    if (menuItem != null)
                    {
                        menuItem.Tag = dataItem;
                        menuItems.Add(menuItem);
                    }
                }
            }

            return menuItems.ToArray();
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public abstract class BaseDataItemMenuItemsCreator<TData, TSelectableCollection> :
       BaseDataItemMenuItemsCreator<DataItem<TData>, TData, TSelectableCollection>
       where TSelectableCollection : SelectableDataItemCollection<DataItem<TData>, TData>, new()
    {
        #region protected
        protected override ToolStripMenuItem CreateMenuItem(DataItem<TData> item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            return new ToolStripMenuItem(item.Name);
        }
        #endregion
    }
    //-------------------------------------------------------------------------
}