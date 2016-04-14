using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tools.CSharp.Collections;

namespace Tools.CSharp.MenuCreators
{
    public partial class MenuItemsCreator
    {
        #region private
        private readonly string _fontFamilyName;
        private readonly float _fontEmSize;
        //---------------------------------------------------------------------
        private static void _UpdateItem<TData>(ToolStripItem item, DataItem<TData> dataItem, bool showToolTip, bool showImage, bool updateText)
        {
            if (updateText && dataItem != null)
            { item.Text = dataItem.Name; }

            item.ToolTipText = showToolTip ? GetDataItemToolTip(dataItem) : string.Empty;
            item.Image = showImage ? GetDataItemImage(dataItem) : null;
            item.Tag = dataItem;
        }
        #endregion
        #region protected
        protected string FontFamilyName
        {
            get { return _fontFamilyName; }
        }
        protected float FontEmSize
        {
            get { return _fontEmSize; }
        }
        //---------------------------------------------------------------------
        protected static DataItem<TData> FindDataItem<TData>(object item, out ToolStripMenuItem menuItem)
        {
            menuItem = item as ToolStripMenuItem;
            return menuItem?.Tag as DataItem<TData>;
        }
        //---------------------------------------------------------------------
        protected static void InternalMenuItemEnabled(ToolStripMenuItem menuItem, bool enabled)
        {
            menuItem.Enabled = enabled;
        }
        protected static void InternalMenuItemChecked(ToolStripMenuItem menuItem, bool check)
        {
            menuItem.Checked = check;
        }
        //---------------------------------------------------------------------
        protected static string GetDataItemToolTip<TData>(DataItem<TData> dataItem)
        {
            if (dataItem == null)
            { return string.Empty; }

            var toolTip = (dataItem as DataHelpItem<TData>)?.Help ?? (dataItem as DataImageHelpItem<TData>)?.Help;
            return string.IsNullOrWhiteSpace(toolTip) ? dataItem.Name : toolTip;
        }
        protected static Image GetDataItemImage<TData>(DataItem<TData> dataItem)
        {
            return (dataItem as DataImageItem<TData>)?.Image;
        }
        #endregion
        public MenuItemsCreator()
            : this(DefaultMenuItemFontFamilyName, DefaultMenuItemFontEmSize)
        { }
        public MenuItemsCreator(string fontFamilyName, float fontEmSize)
        {
            if (string.IsNullOrWhiteSpace(fontFamilyName))
            { throw new ArgumentNullException(nameof(fontFamilyName)); }
            if (float.IsNaN(fontEmSize) || float.IsInfinity(fontEmSize) || fontEmSize <= 0)
            { throw new ArgumentOutOfRangeException(nameof(fontEmSize)); }

            _fontFamilyName = fontFamilyName;
            _fontEmSize = fontEmSize;
        }

        //---------------------------------------------------------------------
        public const string DefaultMenuItemFontFamilyName = "Arial";
        public const float DefaultMenuItemFontEmSize = 9.0f;
        //---------------------------------------------------------------------
        public delegate void MenuItemMethod<in TData>(TData data);
        public static EventHandler CreateMenuItemHandler<TData>(MenuItemMethod<TData> method)
        {
            return (sender, e) =>
            {
                var menuItem = sender as ToolStripItem;
                var dataItem = menuItem?.Tag as DataItem<TData>;

                if (dataItem != null)
                { method?.Invoke(dataItem.Data); }
            };
        }
        //---------------------------------------------------------------------
        public ToolStripItem CreateItem<TDataItem, TData>(
            TData data,
            DataItemCollection<TDataItem, TData> collection, 
            EventHandler handler,
            bool showToolTip = true,
            Keys shortcutKeys = Keys.None,
            bool showImage = false,
            ToolStripItemImageScaling imageScaling = ToolStripItemImageScaling.None
        ) where TDataItem : DataItem<TData>
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            var dataItem = collection[data];
            if (dataItem == null)
            { throw new ArgumentException(string.Empty, nameof(data)); }

            var menuItem = new ToolStripMenuItem(dataItem.Name, null, handler)
            {
                ShortcutKeys = shortcutKeys,
                ImageScaling = imageScaling,
                Font = new Font(_fontFamilyName, _fontEmSize),
            };

            _UpdateItem(menuItem, dataItem, showToolTip, showImage, false);

            return menuItem;
        }
        public ToolStripItem CreateItem<TData>(
            DataItem<TData> dataItem,
            EventHandler handler,
            bool showToolTip = true,
            Keys shortcutKeys = Keys.None,
            bool showImage = false,
            ToolStripItemImageScaling imageScaling = ToolStripItemImageScaling.None
        )
        {
            if (dataItem == null)
            { throw new ArgumentNullException(nameof(dataItem)); }

            var menuItem = new ToolStripMenuItem(dataItem.Name, null, handler)
            {
                ShortcutKeys = shortcutKeys,
                ImageScaling = imageScaling,
                Font = new Font(_fontFamilyName, _fontEmSize),
            };

            _UpdateItem(menuItem, dataItem, showToolTip, showImage, false);

            return menuItem;
        }
        //---------------------------------------------------------------------
        public void UpdateItem<TData>(ToolStripItem item, bool showToolTip = true, bool showImage = false, bool updateText = false)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            var dataItem = item.Tag as DataItem<TData>;
            if (dataItem != null)
            { _UpdateItem(item, dataItem, showToolTip, showImage, updateText); }
        }
        public void UpdateItem<TDataItem, TData>(ToolStripItem item, TData data, DataItemCollection<TDataItem, TData> collection, bool showToolTip = true, bool showImage = false, bool updateText = false) 
            where TDataItem : DataItem<TData>
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            var dataItem = collection[data];
            if (dataItem == null)
            { throw new ArgumentException(string.Empty, nameof(data)); }

            _UpdateItem(item, dataItem, showToolTip, showImage, updateText);
        }
        public void UpdateItem<TData>(ToolStripItem item, DataItem<TData> dataItem, bool showToolTip = true, bool showImage = false, bool updateText = false)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }
            if (dataItem == null)
            { throw new ArgumentNullException(nameof(dataItem)); }

            _UpdateItem(item, dataItem, showToolTip, showImage, updateText);
        }
        //---------------------------------------------------------------------
        public ToolStripItem CreateBannerItem(string bannerMessage)
        {
            return CreateBannerItem(bannerMessage, _fontFamilyName, _fontEmSize);
        }
        public ToolStripItem CreateBannerItem(string bannerMessage, string fontFamilyName, float fontEmSize)
        {
            if (string.IsNullOrWhiteSpace(bannerMessage))
            { throw new ArgumentNullException(nameof(bannerMessage)); }

            var item = new ToolStripMenuItem(bannerMessage)
            {
                Enabled = false,
                Font = new Font(_fontFamilyName, _fontEmSize, FontStyle.Bold)
            };
            return item;
        }
        //---------------------------------------------------------------------
        public ToolStripItem CreateSeparatorItem()
        {
            return new ToolStripSeparator();
        }
        //---------------------------------------------------------------------
        public ToolStripDropDownItem CreateDropDownItem(string text)
        {
            return new ToolStripMenuItem(text);
        }
        public ToolStripDropDownItem CreateDropDownItem(string text, Image image, ToolStripItemImageScaling imageScaling, EventHandler handler)
        {
            return new ToolStripMenuItem(text, image, handler)
            {
                ImageScaling = imageScaling,
                Font = new Font(_fontFamilyName, _fontEmSize)
            };
        }
        //---------------------------------------------------------------------
        public void DestroyItemsInCollection(ToolStripItemCollection itemCollection)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }

            itemCollection.Clear();
        }
        public void DestroyItemsInCollection(ICollection<ToolStripItem> itemCollection)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }

            foreach (var item in itemCollection)
            { DestroyMenuItem(item); }

            itemCollection.Clear();
        }

        public void DestroyMenuItem(ToolStripItem item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            item.Dispose();
        }
        //---------------------------------------------------------------------
        public void AddItemsInCollection(ToolStripItemCollection itemCollection, IEnumerable<ToolStripItem> items, bool addSeparator = true)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }
            
            var isFirstItemAdding = true;

            foreach (var item in items.Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    {  itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);
            }
        }
        public void AddItemsInCollection<TData>(ToolStripItemCollection itemCollection, IEnumerable<ToolStripItem> items, bool itemsEnabled, bool addSeparator = true)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            var isFirstItemAdding = true;

            foreach (var item in items.Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    {  itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuItemEnabled<TData>(item, itemsEnabled);
            }
        }
        public void AddItemsInCollection<TData>(ToolStripItemCollection itemCollection, IEnumerable<ToolStripItem> items, bool itemsEnabled, TData data, bool addSeparator = true)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            var isFirstItemAdding = true;

            foreach (var item in items.Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuItemEnabledAndCheck(item, data, itemsEnabled);
            }
        }
        public void AddItemsInCollection<TData>(ToolStripItemCollection itemCollection, IEnumerable<ToolStripItem> items, TData data, bool addSeparator = true)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            var isFirstItemAdding = true;

            foreach (var item in items.Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuItemEnabledAndCheck(item, data);
            }
        }

        public void AddItemsInCollection(ICollection<ToolStripItem> itemCollection, IEnumerable<ToolStripItem> items, bool addSeparator = true)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            var isFirstItemAdding = true;

            foreach (var item in items.Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);
            }
        }
        public void AddItemsInCollection<TData>(ICollection<ToolStripItem> itemCollection, IEnumerable<ToolStripItem> items, bool itemsEnabled, bool addSeparator = true)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            var isFirstItemAdding = true;

            foreach (var item in items.Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuItemEnabled<TData>(item, itemsEnabled);
            }
        }
        public void AddItemsInCollection<TData>(ICollection<ToolStripItem> itemCollection, IEnumerable<ToolStripItem> items, bool itemsEnabled, TData data, bool addSeparator = true)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            var isFirstItemAdding = true;

            foreach (var item in items.Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuItemEnabledAndCheck(item, data, itemsEnabled);
            }
        }
        public void AddItemsInCollection<TData>(ICollection<ToolStripItem> itemCollection, IEnumerable<ToolStripItem> items, TData data, bool addSeparator = true)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            var isFirstItemAdding = true;

            foreach (var item in items.Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuItemEnabledAndCheck(item, data);
            }
        }

        public void AddItemInCollection(ToolStripItemCollection itemCollection, ToolStripItem item)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }
            
            itemCollection.Add(item);
        }
        public void AddItemInCollection(ICollection<ToolStripItem> itemCollection, ToolStripItem item)
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }
            
            itemCollection.Add(item);
        }
        //---------------------------------------------------------------------
        public void MenuItemsEnabledAndCheck<TData>(ToolStripItemCollection items, TData data)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuItemEnabledAndCheck(item, data); }
        }
        public void MenuItemsEnabledAndCheck<TData>(IEnumerable<ToolStripItem> items, TData data)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuItemEnabledAndCheck(item, data); }
        }
        public void MenuItemEnabledAndCheck<TData>(ToolStripItem item, TData data)
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataItem<TData>(item, out menuItem);

            if (dataItem != null)
            {
                var check = dataItem.Data.Equals(data);
                var enabled = !check;

                InternalMenuItemEnabled(menuItem, enabled);
                InternalMenuItemChecked(menuItem, check);
            }
        }

        public void MenuItemsEnabledAndCheck<TData>(ToolStripItemCollection items, TData data, bool enabled)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuItemEnabledAndCheck(item, data, enabled); }
        }
        public void MenuItemsEnabledAndCheck<TData>(IEnumerable<ToolStripItem> items, TData data, bool enabled)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuItemEnabledAndCheck(item, data, enabled); }
        }
        public void MenuItemEnabledAndCheck<TData>(ToolStripItem item, TData data, bool enabled)
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataItem<TData>(item, out menuItem);

            if (dataItem != null)
            {
                InternalMenuItemEnabled(menuItem, enabled);
                InternalMenuItemChecked(menuItem, dataItem.Data.Equals(data));
            }
        }
        //---------------------------------------------------------------------
        public void MenuItemsChecked<TData>(ToolStripItemCollection items, TData data)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuItemChecked(item, data); }
        }
        public void MenuItemsChecked<TData>(IEnumerable<ToolStripItem> items, TData data)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuItemChecked(item, data); }
        }
        public void MenuItemChecked<TData>(ToolStripItem item, TData data)
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataItem<TData>(item, out menuItem);

            if (dataItem != null)
            { InternalMenuItemChecked(menuItem, dataItem.Data.Equals(data)); }
        }
        public void MenuItemChecked(ToolStripMenuItem menuItem, bool check)
        {
            if (menuItem == null)
            { throw new ArgumentNullException(nameof(menuItem)); }

            InternalMenuItemChecked(menuItem, check);
        }

        public void MenuItemsCheckedReset<TData>(ToolStripItemCollection items)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuItemCheckedReset<TData>(item); }
        }
        public void MenuItemsCheckedReset<TData>(IEnumerable<ToolStripItem> items)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuItemCheckedReset<TData>(item); }
        }
        public void MenuItemCheckedReset<TData>(ToolStripItem item)
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataItem<TData>(item, out menuItem);

            if (dataItem != null)
            { InternalMenuItemChecked(menuItem, false); }
        }
        public void MenuItemCheckedReset(ToolStripMenuItem menuItem)
        {
            if (menuItem == null)
            { throw new ArgumentNullException(nameof(menuItem)); }

            InternalMenuItemChecked(menuItem, false);
        }
        //---------------------------------------------------------------------
        public void MenuItemsEnabled<TData>(ToolStripItemCollection items, bool enabled)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuItemEnabled<TData>(item, enabled); }
        }
        public void MenuItemsEnabled<TData>(IEnumerable<ToolStripItem> items, bool enabled)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuItemEnabled<TData>(item, enabled); }
        }
        public void MenuItemEnabled<TData>(ToolStripItem item, bool enabled)
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataItem<TData>(item, out menuItem);

            if (dataItem != null)
            { InternalMenuItemEnabled(menuItem, enabled); }
        }
        public void MenuItemEnabled(ToolStripMenuItem menuItem, bool enabled)
        {
            if (menuItem == null)
            { throw new ArgumentNullException(nameof(menuItem)); }

            InternalMenuItemEnabled(menuItem, enabled);
        }
        //---------------------------------------------------------------------
    }
}