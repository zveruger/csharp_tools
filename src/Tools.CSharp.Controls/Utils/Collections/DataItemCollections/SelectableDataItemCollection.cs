using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Tools.CSharp.Collections
{
    //-------------------------------------------------------------------------
    public class SelectableDataItemCollection<TDataItem, TData> : DataItemCollection<TDataItem, TData>, IDisposable
        where TDataItem : DataItem<TData>
    {
        #region private
        private const int _MinIndex = -1;
        //---------------------------------------------------------------------
        private bool _disposed;
        private int _selectedIndex;
        private ComboBox _comboBox;
        private bool _isFreezeAddItemInComboBox;
        //---------------------------------------------------------------------
        private TDataItem _SelectedItem
        {
            get { return _selectedIndex == _MinIndex ? default(TDataItem) : this[_selectedIndex]; }
        }
        private TData _SelectedDate
        {
            get
            {
                var selectedItem = _SelectedItem;
                return selectedItem == default(TDataItem) ? default(TData) : selectedItem.Data;
            }
            set
            {
                for (var itemCounter = 0; itemCounter < Count; itemCounter++)
                {
                    if (Items[itemCounter].Data.Equals(value))
                    {
                        _selectedIndexUpdateAndComboBox(itemCounter);
                        break;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private void _checkIndexThrowArgumentOutOfRangeException(int index)
        {
            if (index < _MinIndex || index >= Count)
            { throw new ArgumentOutOfRangeException(nameof(index)); }
        }
        private void _checkThrowObjectDisposedException()
        {
            if (_disposed)
            { throw new ObjectDisposedException(GetType().FullName); }
        }
        //---------------------------------------------------------------------
        private void _selectedIndexUpdateAndComboBox(int index)
        {
            _checkThrowObjectDisposedException();
            _checkIndexThrowArgumentOutOfRangeException(index);

            _selectedIndexUpdate(index);

            if (_comboBox != null && _comboBox.Items.Count != 0)
            { _comboBox.SelectedIndex = _selectedIndex; }
        }
        private void _selectedIndexUpdate(int index)
        {
            if (_selectedIndex != index)
            {
                _selectedIndex = index;
                OnSelectedIndexChanged();
            }
        }
        //---------------------------------------------------------------------
        private void _subscribeComboBoxAllEvents(bool addEvents)
        {
            if (_comboBox != null)
            {
                if (addEvents)
                { _comboBox.SelectedIndexChanged += _comboBoxOnSelectedIndexChanged; }
                else
                { _comboBox.SelectedIndexChanged -= _comboBoxOnSelectedIndexChanged; }
            }
        }
        private void _comboBoxOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (_comboBox != null && _comboBox.SelectedIndex != _selectedIndex)
            { _selectedIndexUpdate(_comboBox.SelectedIndex); }
        }
        //---------------------------------------------------------------------
        private void _addItemInComboBox(TDataItem item)
        {
            _comboBox.Items.Add(item.Name);
        }
        private void _addItemsInComboBox(IEnumerable<TDataItem> items)
        {
            foreach (var item in items)
            { _addItemInComboBox(item); }
        }
        #endregion
        #region protected
        protected override void AddInternal(TDataItem item)
        {
            base.AddInternal(item);

            if (_comboBox != null && !_isFreezeAddItemInComboBox)
            { _addItemInComboBox(item); }
        }
        protected override void AddRangeInternal(IEnumerable<TDataItem> items)
        {
            base.AddRangeInternal(items);

            if (_comboBox != null && !_isFreezeAddItemInComboBox)
            { _addItemsInComboBox(items); }
        }
        protected override void ClearInternal()
        {
            base.ClearInternal();

            _selectedIndexUpdateAndComboBox(_MinIndex);

            ClearComboBox();
        }
        protected override void SortInternal(IComparer<TDataItem> comparer)
        {
            _isFreezeAddItemInComboBox = false;
            var bufSelectedItem = _SelectedItem;

            base.SortInternal(comparer);

            if (bufSelectedItem != default(TDataItem))
            { _SelectedDate = bufSelectedItem.Data; }
        }

        protected virtual void OnSelectedIndexChanged()
        {
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void ClearComboBox()
        {
            if (_comboBox != null)
            {
                _comboBox.Items.Clear();
                _comboBox.Text = string.Empty;
            }
        }
        protected virtual void ComboBoxItemsUpdate()
        {
            if (_comboBox != null && Count != 0)
            {
                _comboBox.BeginUpdate();

                try
                {
                    _addItemsInComboBox(Items);
                }
                finally
                { _comboBox.EndUpdate(); }
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!_disposed && disposing)
                {
                    if (_comboBox != null)
                    { _subscribeComboBoxAllEvents(false); }
                }
            }
            finally
            { _disposed = true; }
        }
        #endregion
        public SelectableDataItemCollection()
        {
            _selectedIndex = _MinIndex;
        }
        ~SelectableDataItemCollection()
        {
            Dispose(false);
        }

        //---------------------------------------------------------------------
        public bool IsFreezeAddItemInComboBox
        {
            get { return _isFreezeAddItemInComboBox; }
            set { _isFreezeAddItemInComboBox = value; }
        }
        //---------------------------------------------------------------------
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndexUpdateAndComboBox(value); }
        }
        //---------------------------------------------------------------------
        public TDataItem SelectedItem
        {
            get { return _SelectedItem; }
        }
        public TData SelectedDate
        {
            get { return _SelectedDate; }
            set { _SelectedDate = value; }
        }
        //---------------------------------------------------------------------
        public ComboBox ComboBox
        {
            get { return _comboBox; }
            set
            {
                _checkThrowObjectDisposedException();

                if (_comboBox != null)
                {
                    _subscribeComboBoxAllEvents(false);
                    ClearComboBox();
                }

                _comboBox = value;

                if (_comboBox != null)
                {
                    ClearComboBox();

                    _comboBox.Sorted = false;
                    _comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                    ComboBoxItemsUpdate();

                    _subscribeComboBoxAllEvents(true);
                    _selectedIndexUpdateAndComboBox(_selectedIndex);
                }
            }
        }
        //---------------------------------------------------------------------
        public bool IsDisposed
        {
            get { return _disposed; }
        }
        //---------------------------------------------------------------------
        public bool SetSelectedIndex(Func<TData, bool> functor)
        {
            if (functor == null)
            { throw new ArgumentNullException(nameof(functor)); }

            _checkThrowObjectDisposedException();

            var findedIndex = Items.FindIndex(x => functor(x.Data));
            _selectedIndexUpdateAndComboBox(findedIndex);

            return findedIndex != _MinIndex;
        }
        public bool UpdateItemName(Func<TDataItem, bool> functor, string name)
        {
            if (functor == null)
            { throw new ArgumentNullException(nameof(functor)); }

            _checkThrowObjectDisposedException();

            for (var itemCounter = 0; itemCounter < Count; itemCounter++)
            {
                var item = Items[itemCounter];
                if (functor(item))
                {
                    item.Name = name;

                    if (_comboBox != null)
                    {
                        var bufSelectedIndex = _selectedIndex;

                        _comboBox.Items[itemCounter] = item.Name;
                        _comboBox.Refresh();

                        _selectedIndexUpdateAndComboBox(bufSelectedIndex);
                    }

                    return true;
                }
            }

            return false;
        }
        //---------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //---------------------------------------------------------------------
        public event EventHandler SelectedIndexChanged;
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public class SelectableDataItemCollection<TDataItem, TData, TSelectableDataItemCollection> : SelectableDataItemCollection<TDataItem, TData>
        where TDataItem : DataItem<TData>
        where TSelectableDataItemCollection : SelectableDataItemCollection<TDataItem, TData>, new()
    {
        #region private
        private static TSelectableDataItemCollection _Instance;
        #endregion
        //---------------------------------------------------------------------
        public static TSelectableDataItemCollection Instance
        {
            get { return LazyInitializer.EnsureInitialized(ref _Instance, () => new TSelectableDataItemCollection()); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}