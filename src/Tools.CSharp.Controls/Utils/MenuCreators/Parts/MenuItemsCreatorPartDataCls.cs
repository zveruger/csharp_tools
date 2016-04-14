using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tools.CSharp.MenuCreators
{
    public partial class MenuItemsCreator
    {
        #region private
        private static void _UpdateDataClsItem<TData>(ToolStripItem item, TData data, string text, string toolTipe, Image image, bool updateToolTip, bool updateImage, bool updateText)
        {
            if (updateToolTip)
            { item.ToolTipText = toolTipe; }

            if (updateImage)
            { item.Image = image; }

            if (updateText)
            { item.Text = text; }

            item.Tag = data;
        }
        #endregion
        #region protected
        protected static TData FindDataClsItem<TData>(object item, out ToolStripMenuItem menuItem)
            where TData : class
        {
            menuItem = item as ToolStripMenuItem;
            return menuItem?.Tag as TData;
        }
        #endregion
        //---------------------------------------------------------------------
        public static EventHandler CreateMenuDataClsItemHandler<TData>(MenuItemMethod<TData> method)
            where TData : class 
        {
            return (sender, e) =>
            {
                var menuItem = sender as ToolStripItem;
                var data = menuItem?.Tag as TData;

                if (data != null)
                { method?.Invoke(data); }
            };
        }
        //---------------------------------------------------------------------
        public ToolStripItem CreateDataClsItem<TData>(
            string text,
            TData data,
            EventHandler handler,
            string toolTip = "",
            Keys shortcutKeys = Keys.None,
            Image image = null,
            ToolStripItemImageScaling imageScaling = ToolStripItemImageScaling.None
        ) where TData : class
        {
            if (data == null)
            { throw new ArgumentNullException(nameof(data)); }

            var menuItem = new ToolStripMenuItem(text, image, handler)
            {
                ShortcutKeys = shortcutKeys,
                ImageScaling = imageScaling,
                Font = new Font(_fontFamilyName, _fontEmSize),
            };

            _UpdateDataClsItem(menuItem, data, text, toolTip, image, true, false, false);

            return menuItem;
        }
        //---------------------------------------------------------------------
        public void UpdateDataClsItem<TData>(ToolStripItem item, TData data, string text, string toolTipe = "", Image image = null, bool updateToolTip = true, bool updateImage = true, bool updateText = true)
            where TData : class
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }
            if (data == null)
            { throw new ArgumentNullException(nameof(data)); }

            _UpdateDataClsItem(item, data, text, toolTipe, image, updateToolTip, updateImage, updateText);
        }
        public void UpdateDataClsItem<TData>(ToolStripItem item, string text, string toolTipe = "", Image image = null, bool updateToolTip = true, bool updateImage = true, bool updateText = true)
            where TData : class
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            var data = item.Tag as TData;
            if (data != null)
            { _UpdateDataClsItem(item, data, text, toolTipe, image, updateToolTip, updateImage, updateText); }
        }
        //---------------------------------------------------------------------
        public void AddDataClsItemsInCollection<TData>(ToolStripItemCollection itemCollection, IEnumerable<TData> datas, Func<TData, ToolStripItem> createItem, bool addSeparator = true)
            where TData : class
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (datas == null)
            { throw new ArgumentNullException(nameof(datas)); }
            if (createItem == null)
            { throw new ArgumentNullException(nameof(createItem)); }

            var isFirstItemAdding = true;

            foreach (var item in datas.Where(data => data != null).Select(createItem).Where(item => item != null))
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
        public void AddDataClsItemsInCollection<TData>(ToolStripItemCollection itemCollection, IEnumerable<TData> datas, Func<TData, ToolStripItem> createItem, bool itemsEnabled, bool addSeparator)
            where TData : class
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (datas == null)
            { throw new ArgumentNullException(nameof(datas)); }
            if (createItem == null)
            { throw new ArgumentNullException(nameof(createItem)); }

            var isFirstItemAdding = true;

            foreach (var item in datas.Where(data => data != null).Select(createItem).Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuDataClsItemEnabled<TData>(item, itemsEnabled);
            }
        }
        public void AddDataClsItemsInCollection<TData>(ToolStripItemCollection itemCollection, IEnumerable<TData> datas, Func<TData, ToolStripItem> createItem, bool itemsEnabled, TData data, bool addSeparator = true)
            where TData : class
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (datas == null)
            { throw new ArgumentNullException(nameof(datas)); }
            if (createItem == null)
            { throw new ArgumentNullException(nameof(createItem)); }

            var isFirstItemAdding = true;

            foreach (var item in datas.Where(tmpData => tmpData != null).Select(createItem).Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuDataClsItemEnabledAndCheck(item, data, itemsEnabled);
            }
        }
        public void AddDataClsItemsInCollection<TData>(ToolStripItemCollection itemCollection, IEnumerable<TData> datas, Func<TData, ToolStripItem> createItem, TData data, bool addSeparator = true)
            where TData : class 
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (datas == null)
            { throw new ArgumentNullException(nameof(datas)); }
            if (createItem == null)
            { throw new ArgumentNullException(nameof(createItem)); }

            var isFirstItemAdding = true;

            foreach (var item in datas.Where(tmpData => tmpData != null).Select(createItem).Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuDataClsItemEnabledAndCheck(item, data);
            }
        }

        public void AddDataClsItemsInCollection<TData>(ICollection<ToolStripItem> itemCollection, IEnumerable<TData> datas, Func<TData, ToolStripItem> createItem, bool addSeparator = true)
            where TData : class
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (datas == null)
            { throw new ArgumentNullException(nameof(datas)); }
            if (createItem == null)
            { throw new ArgumentNullException(nameof(createItem)); }

            var isFirstItemAdding = true;

            foreach (var item in datas.Where(data => data != null).Select(createItem).Where(item => item != null))
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
        public void AddDataClsItemsInCollection<TData>(ICollection<ToolStripItem> itemCollection, IEnumerable<TData> datas, Func<TData, ToolStripItem> createItem, bool itemsEnabled, bool addSeparator)
            where TData : class
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (datas == null)
            { throw new ArgumentNullException(nameof(datas)); }
            if (createItem == null)
            { throw new ArgumentNullException(nameof(createItem)); }

            var isFirstItemAdding = true;

            foreach (var item in datas.Where(tmpData => tmpData != null).Select(createItem).Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuDataClsItemEnabled<TData>(item, itemsEnabled);
            }
        }
        public void AddDataClsItemsInCollection<TData>(ICollection<ToolStripItem> itemCollection, IEnumerable<TData> datas, Func<TData, ToolStripItem> createItem, bool itemsEnabled, TData data, bool addSeparator = true)
            where TData : class
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (datas == null)
            { throw new ArgumentNullException(nameof(datas)); }
            if (createItem == null)
            { throw new ArgumentNullException(nameof(createItem)); }

            var isFirstItemAdding = true;

            foreach (var item in datas.Where(tmpData => tmpData != null).Select(createItem).Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuDataClsItemEnabledAndCheck(item, data, itemsEnabled);
            }
        }
        public void AddDataClsItemsInCollection<TData>(ICollection<ToolStripItem> itemCollection, IEnumerable<TData> datas, Func<TData, ToolStripItem> createItem, TData data, bool addSeparator = true)
            where TData : class
        {
            if (itemCollection == null)
            { throw new ArgumentNullException(nameof(itemCollection)); }
            if (datas == null)
            { throw new ArgumentNullException(nameof(datas)); }
            if (createItem == null)
            { throw new ArgumentNullException(nameof(createItem)); }

            var isFirstItemAdding = true;

            foreach (var item in datas.Where(tmpData => tmpData != null).Select(createItem).Where(item => item != null))
            {
                if (isFirstItemAdding)
                {
                    isFirstItemAdding = false;

                    if (addSeparator && itemCollection.Count != 0)
                    { itemCollection.Add(CreateSeparatorItem()); }
                }

                itemCollection.Add(item);

                MenuDataClsItemEnabledAndCheck(item, data);
            }
        }
        //---------------------------------------------------------------------
        public void MenuDataClsItemsEnabledAndCheck<TData>(ToolStripItemCollection items, TData data)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuDataClsItemEnabledAndCheck(item, data); }
        }
        public void MenuDataClsItemsEnabledAndCheck<TData>(IEnumerable<ToolStripItem> items, TData data)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuDataClsItemEnabledAndCheck(item, data); }
        }
        public void MenuDataClsItemEnabledAndCheck<TData>(ToolStripItem item, TData data)
            where TData : class
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataClsItem<TData>(item, out menuItem);

            if (dataItem != null)
            {
                var check = ReferenceEquals(dataItem, data);
                var enabled = !check;

                InternalMenuItemEnabled(menuItem, enabled);
                InternalMenuItemChecked(menuItem, check);
            }
        }

        public void MenuDataClsItemsEnabledAndCheck<TData>(ToolStripItemCollection items, TData data, bool enabled)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuDataClsItemEnabledAndCheck(item, data, enabled); }
        }
        public void MenuDataClsItemsEnabledAndCheck<TData>(IEnumerable<ToolStripItem> items, TData data, bool enabled)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuDataClsItemEnabledAndCheck(item, data, enabled); }
        }
        public void MenuDataClsItemEnabledAndCheck<TData>(ToolStripItem item, TData data, bool enabled)
            where TData : class
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataClsItem<TData>(item, out menuItem);

            if (dataItem != null)
            {
                InternalMenuItemEnabled(menuItem, enabled);
                InternalMenuItemChecked(menuItem, ReferenceEquals(dataItem, data));
            }
        }
        //---------------------------------------------------------------------
        public void MenuDataClsItemsChecked<TData>(ToolStripItemCollection items, TData data)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuDataClsItemChecked(item, data); }
        }
        public void MenuDataClsItemsChecked<TData>(IEnumerable<ToolStripItem> items, TData data)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuDataClsItemChecked(item, data); }
        }
        public void MenuDataClsItemChecked<TData>(ToolStripItem item, TData data)
            where TData : class
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataClsItem<TData>(item, out menuItem);

            if (dataItem != null)
            { InternalMenuItemChecked(menuItem, ReferenceEquals(dataItem, data)); }
        }

        public void MenuDataClsItemsCheckedReset<TData>(ToolStripItemCollection items)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuDataClsItemCheckedReset<TData>(item); }
        }
        public void MenuDataClsItemsCheckedReset<TData>(IEnumerable<ToolStripItem> items)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuDataClsItemCheckedReset<TData>(item); }
        }
        public void MenuDataClsItemCheckedReset<TData>(ToolStripItem item)
            where TData : class
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataClsItem<TData>(item, out menuItem);

            if (dataItem != null)
            { InternalMenuItemChecked(menuItem, false); }
        }
        //---------------------------------------------------------------------
        public void MenuDataClsItemsEnabled<TData>(ToolStripItemCollection items, bool enabled)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (ToolStripItem item in items)
            { MenuDataClsItemEnabled<TData>(item, enabled); }
        }
        public void MenuDataClsItemsEnabled<TData>(IEnumerable<ToolStripItem> items, bool enabled)
            where TData : class
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach (var item in items)
            { MenuDataClsItemEnabled<TData>(item, enabled); }
        }
        public void MenuDataClsItemEnabled<TData>(ToolStripItem item, bool enabled)
            where TData : class
        {
            ToolStripMenuItem menuItem;
            var dataItem = FindDataClsItem<TData>(item, out menuItem);

            if (dataItem != null)
            { InternalMenuItemEnabled(menuItem, enabled); }
        }
        //---------------------------------------------------------------------
    }
}