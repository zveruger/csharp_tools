using System;
using System.Collections.Generic;
using System.ComponentModel;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Collections
{
    public abstract class SortedItemCollection<TViewModel, TViewModelCollection> : BaseDisposable, IEnumerable<TViewModel>
        where TViewModelCollection : IList<TViewModel>
    {
        #region private
        private readonly SelectedSortedItemCollection _selectedItems;
        private readonly List<SortedItem> _items = new List<SortedItem>();
        private readonly Dictionary<TViewModel, SortedItem> _cacheViewModelAndItems = new Dictionary<TViewModel, SortedItem>();
        private readonly TViewModel _emptyViewModel = default(TViewModel);
        //---------------------------------------------------------------------
        private SortDirection _sortDirection = SortDirection.Asc;
        private TViewModelCollection _originalCollection;
        private int _itemsCursorCount;
        //---------------------------------------------------------------------
        private sealed class SortedItem
        {
            //-----------------------------------------------------------------
            public int Index { get; set; }
            //-----------------------------------------------------------------
            public TViewModel ViewModel { get; set; }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private sealed class SortedItemComparer : IComparer<SortedItem>
        {
            #region private
            private readonly IComparer<TViewModel> _comparer;
            private readonly SortDirection _sortDirection;
            //-----------------------------------------------------------------
            private int _applyDirection(int arg)
            {
                return _sortDirection == SortDirection.Asc ? arg : -arg;
            }
            //-----------------------------------------------------------------
            private void _excangeIndexes(SortedItem x, SortedItem y, int compareResult)
            {
                switch (_sortDirection)
                {
                    case SortDirection.Asc:
                    case SortDirection.Desc:
                        {
                            if (compareResult > 0)
                            { _ExcangeIndexes(x, y); }
                        }
                        break;
                }
            }
            private static void _ExcangeIndexes(SortedItem x, SortedItem y)
            {
                var tmpIndex = x.Index;
                x.Index = y.Index;
                y.Index = tmpIndex;
            }
            #endregion
            public SortedItemComparer(IComparer<TViewModel> comparer, SortDirection sortDirection)
            {
                _comparer = comparer;
                _sortDirection = sortDirection;
            }

            //-----------------------------------------------------------------
            public int Compare(SortedItem x, SortedItem y)
            {
                var compareResult = _applyDirection(_comparer.Compare(x.ViewModel, y.ViewModel));
                _excangeIndexes(x, y, compareResult);
                return compareResult;
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private sealed class ViewModelEnumerator : IEnumerator<TViewModel>
        {
            #region private
            private readonly IList<SortedItem> _collection;
            private readonly int _count;
            //-----------------------------------------------------------------
            private int _index;
            #endregion
            public ViewModelEnumerator(IList<SortedItem> collection)
            {
                _collection = collection;
                _count = _collection.Count;
                _index = -1;
            }

            //-----------------------------------------------------------------
            public TViewModel Current
            {
                get { return _collection[_index].ViewModel; }
            }
            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }
            //-----------------------------------------------------------------
            public bool MoveNext()
            {
                if (_index < _count)
                {
                    _index++;
                    return (_index < _count);
                }
                return false;
            }
            public void Reset()
            {
                _index = -1;
            }
            //-----------------------------------------------------------------
            public void Dispose()
            { }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private void _incrementItemsCursorCount(bool isRaiseCountChanged)
        {
            _updateItemsCursorCount(_itemsCursorCount + 1, isRaiseCountChanged);
        }
        private void _decrementItemsCursorCount(bool isRaiseCountChanged)
        {
            _updateItemsCursorCount(_itemsCursorCount - 1, isRaiseCountChanged);
        }
        private void _updateItemsCursorCount(int itemsCoursorCount, bool isRaiseCountChanged)
        {
            var tmpItemsCoursorCount = itemsCoursorCount.GetValue(0, int.MaxValue);
            if (tmpItemsCoursorCount != _itemsCursorCount)
            {
                _itemsCursorCount = tmpItemsCoursorCount;
                if (isRaiseCountChanged)
                { OnCountChanged(_itemsCursorCount, true); }
            }
        }

        private void _add(int viewModelIndex, bool isRaiseCountChanged)
        {
            var viewModel = _originalCollection[viewModelIndex];
            SortedItem item;

            if (viewModelIndex < _items.Count)
            { item = _items[viewModelIndex]; }
            else
            {
                item = new SortedItem();
                _items.Add(item);
                item.Index = _items.Count - 1;
            }

            item.ViewModel = viewModel;
            _cacheViewModelAndItems.Add(viewModel, item);

            _incrementItemsCursorCount(isRaiseCountChanged);
        }
        private void _remove(TViewModel viewModel, bool isRaiseCountChanged)
        {
            SortedItem item;
            if (_cacheViewModelAndItems.TryGetValue(viewModel, out item))
            {
                _cacheViewModelAndItems.Remove(viewModel);
                _items.Remove(item);

                _selectedItems.RemoveSelectedIndex(viewModel);
                _decrementItemsCursorCount(isRaiseCountChanged);
            }
        }
        private void _prepareFill(int oldItemsCursolCount)
        {
            var newItemsCursorCount = (ReferenceEquals(_originalCollection, null)) || (_originalCollection.Count == 0) ? 0 : _originalCollection.Count;
            for (var i = oldItemsCursolCount - 1; i >= newItemsCursorCount; i--)
            {
                _selectedItems.RemoveSelectedIndex(i);
                _items[i].ViewModel = _emptyViewModel;
            }
            _clear(false);
        }
        private void _fill(int oldItemsCursolCount)
        {
            _prepareFill(oldItemsCursolCount);

            if (!ReferenceEquals(_originalCollection, null))
            {
                for (var i = 0; i < _originalCollection.Count; i++)
                { _add(i, false); }
            }
        }
        private void _clear(bool isRaiseCountChanged)
        {
            _cacheViewModelAndItems.Clear();
            _updateItemsCursorCount(0, isRaiseCountChanged);
        }
        //---------------------------------------------------------------------
        private void _reset(bool isRaiseCountChanged, int oldItemsCursolCount)
        {
            if (isRaiseCountChanged)
            {
                _fill(oldItemsCursolCount);
                OnCountChanged(_itemsCursorCount, true);
            }
            else
            { _clear(false); }
        }
        //---------------------------------------------------------------------
        private void _onOriginalCollectionChanged()
        {
            OriginalCollectionChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #region protected
        protected void Add(int viewModelIndex, bool isRaiseCountChanged = false)
        {
            if (viewModelIndex < 0 || viewModelIndex >= _originalCollection.Count)
            { throw new ArgumentOutOfRangeException(nameof(viewModelIndex)); }

            _add(viewModelIndex, isRaiseCountChanged);
        }
        protected void Remove(TViewModel viewModel, bool isRaiseCountChanged = false)
        {
            _remove(viewModel, isRaiseCountChanged);
        }
        protected void Reset(bool isRaiseCountChanged = false)
        {
            _reset(isRaiseCountChanged, Count);
        }
        protected void ResetSelectedItems()
        {
            _selectedItems.Clear();
        }

        protected virtual bool OriginalCollectionAvailable
        {
            get { return !ReferenceEquals(_originalCollection, null); }
        }
        protected virtual void UpdatingOriginalCollection(TViewModelCollection oldCollection)
        { }
        protected virtual void UpdateOriginalCollection(TViewModelCollection collection)
        { }
        protected virtual void UpdatedOriginalCollection(TViewModelCollection newCollection)
        { }

        protected void OnCountChanged()
        {
            OnCountChanged(_itemsCursorCount, OriginalCollectionAvailable);
        }
        protected virtual void OnCountChanged(int count, bool isIntall)
        {
            CountChanged?.Invoke(this, new SortedItemCollectionCountEventArgs(count, isIntall));
        }
        #endregion
        protected SortedItemCollection()
        {
            _selectedItems = new SelectedSortedItemCollection(this);
        }

        //---------------------------------------------------------------------
        public sealed class SelectedSortedItemCollection
        {
            #region private
            private const int _DefautLastMoveSelectedIndexToEnd = -1;
            //-----------------------------------------------------------------
            private readonly SortedItemCollection<TViewModel, TViewModelCollection> _owner;
            private readonly List<SortedItem> _items = new List<SortedItem>();
            private readonly InternalIndexEnumerable _indexEnumerable;
            private readonly InternalViewModelEnumerable _viewModelEnumerable;
            private readonly TViewModel _emptyViewModel = default(TViewModel);
            //-----------------------------------------------------------------
            private bool _multiSelectViewModels;
            private int _lastMoveSelectedIndexToEnd = _DefautLastMoveSelectedIndexToEnd;
            //-----------------------------------------------------------------
            private sealed class InternalIndexEnumerable : IEnumerable<int>
            {
                #region private
                private readonly IEnumerable<SortedItem> _otherEnumerable;
                #endregion
                public InternalIndexEnumerable(IEnumerable<SortedItem> otherEnumerable)
                {
                    _otherEnumerable = otherEnumerable;
                }

                //-------------------------------------------------------------
                public IEnumerator<int> GetEnumerator()
                {
                    return new InternalIndexEnumerator(_otherEnumerable.GetEnumerator());
                }
                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
                //-------------------------------------------------------------
            }
            private sealed class InternalViewModelEnumerable : IEnumerable<TViewModel>
            {
                #region private
                private readonly IEnumerable<SortedItem> _otherEnumerable;
                #endregion
                public InternalViewModelEnumerable(IEnumerable<SortedItem> otherEnumerable)
                {
                    _otherEnumerable = otherEnumerable;
                }

                //-------------------------------------------------------------
                public IEnumerator<TViewModel> GetEnumerator()
                {
                    return new InternalViewModelEnumerator(_otherEnumerable.GetEnumerator());
                }
                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
                //-------------------------------------------------------------
            }

            private sealed class InternalIndexEnumerator : IEnumerator<int>
            {
                #region private
                private readonly IEnumerator<SortedItem> _otherEnumerator;
                #endregion
                public InternalIndexEnumerator(IEnumerator<SortedItem> otherEnumerator)
                {
                    _otherEnumerator = otherEnumerator;
                }

                //-------------------------------------------------------------
                public int Current
                {
                    get { return _otherEnumerator.Current.Index; }
                }
                object System.Collections.IEnumerator.Current
                {
                    get { return Current; }
                }
                //-------------------------------------------------------------
                public bool MoveNext()
                {
                    return _otherEnumerator.MoveNext();
                }
                public void Reset()
                {
                    _otherEnumerator.Reset();
                }
                //-------------------------------------------------------------
                public void Dispose()
                {
                    _otherEnumerator.Dispose();
                }
                //-------------------------------------------------------------
            }
            private sealed class InternalViewModelEnumerator : IEnumerator<TViewModel>
            {
                #region private
                private readonly IEnumerator<SortedItem> _otherEnumerator;
                #endregion
                public InternalViewModelEnumerator(IEnumerator<SortedItem> otherEnumerator)
                {
                    _otherEnumerator = otherEnumerator;
                }

                //-------------------------------------------------------------
                public TViewModel Current
                {
                    get { return _otherEnumerator.Current.ViewModel; }
                }
                object System.Collections.IEnumerator.Current
                {
                    get { return Current; }
                }
                //-------------------------------------------------------------
                public bool MoveNext()
                {
                    return _otherEnumerator.MoveNext();
                }
                public void Reset()
                {
                    _otherEnumerator.Reset();
                }
                //-------------------------------------------------------------
                public void Dispose()
                {
                    _otherEnumerator.Dispose();
                }
                //-------------------------------------------------------------
            }
            //-----------------------------------------------------------------
            private int _findIndex(int sortedItemIndex)
            {
                var index = -1;
                for (var i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Index == sortedItemIndex)
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
            private int _findIndex(SortedItem item)
            {
                var index = -1;
                for (var i = 0; i < _items.Count; i++)
                {
                    if (ReferenceEquals(_items[i], item))
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
            private int _findIndex(TViewModel viewModel)
            {
                var index = -1;
                for (var i = 0; i < _items.Count; i++)
                {
                    if (ReferenceEquals(_items[i].ViewModel, viewModel))
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
            //-----------------------------------------------------------------
            private void _remove(int index)
            {
                if (index != -1)
                { _items.RemoveAt(index); }
            }
            //-----------------------------------------------------------------
            #endregion
            internal SelectedSortedItemCollection(SortedItemCollection<TViewModel, TViewModelCollection> owner)
            {
                _owner = owner;

                _indexEnumerable = new InternalIndexEnumerable(_items);
                _viewModelEnumerable = new InternalViewModelEnumerable(_items);
            }

            public TViewModel FirstViewModel
            {
                get { return Count == 0 ? _emptyViewModel : _items[0].ViewModel; }
            }
            public TViewModel LastViewModel
            {
                get
                {
                    var count = Count;
                    return count == 0 ? _emptyViewModel : _items[count - 1].ViewModel;
                }
            }
            public int Count
            {
                get { return _items.Count; }
            }
            internal bool MultiSelectViewModels
            {
                get { return _multiSelectViewModels; }
                set
                {
                    if (_multiSelectViewModels != value)
                    {
                        _multiSelectViewModels = value;

                        if (!_multiSelectViewModels)
                        {
                            var count = Count;
                            if (count > 1)
                            { AddSelectedIndex(1, _items[count - 1].Index); }
                        }
                    }
                }
            }

            public IEnumerable<int> GetIndexesEnumerable
            {
                get { return _indexEnumerable; }
            }
            public IEnumerable<TViewModel> GetViewModelsEnumerable
            {
                get { return _viewModelEnumerable; }
            }

            public void AddSelectedIndex(int selectedItemCount, int index)
            {
                if (index < 0 || index >= _owner._itemsCursorCount)
                { return; }

                var lastSelectedItemIndex = _multiSelectViewModels ? (selectedItemCount - 1) : 0;
                int lastIndex;

                while ((lastIndex = _items.Count - 1) >= lastSelectedItemIndex)
                { _items.RemoveAt(lastIndex); }

                if (lastIndex < selectedItemCount)
                {
                    _lastMoveSelectedIndexToEnd = index;

                    var item = _owner._items[index];
                    _items.Add(item);
                }
            }
            public bool MoveSelectedIndexToEnd(int index)
            {
                if (index < 0 || index >= _owner._itemsCursorCount)
                {
                    _lastMoveSelectedIndexToEnd = _DefautLastMoveSelectedIndexToEnd;
                    return false;
                }

                var isMoved = false;
                if (_lastMoveSelectedIndexToEnd != index)
                {
                    var itemIndex = _findIndex(index);
                    if (itemIndex != -1)
                    {
                        _lastMoveSelectedIndexToEnd = index;

                        var lastIndex = _items.Count - 1;
                        if (itemIndex != lastIndex)
                        {
                            _items[itemIndex] = _items[lastIndex];
                            _items[lastIndex] = _owner._items[index];

                            isMoved = true;
                        }
                    }
                    else
                    { _lastMoveSelectedIndexToEnd = _DefautLastMoveSelectedIndexToEnd; }
                }
                else
                { isMoved = true; }

                return isMoved;
            }

            public void Clear()
            {
                _lastMoveSelectedIndexToEnd = _DefautLastMoveSelectedIndexToEnd;

                _items.Clear();
            }
            internal void RemoveSelectedIndex(int sortedItemIndex)
            {
                var index = _findIndex(_owner._items[sortedItemIndex]);
                _remove(index);
            }
            internal void RemoveSelectedIndex(TViewModel viewModel)
            {
                if (ReferenceEquals(viewModel, null))
                { return; }

                var index = _findIndex(viewModel);
                _remove(index);
            }
        }
        //---------------------------------------------------------------------
        public TViewModel this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                { throw new ArgumentOutOfRangeException(nameof(index)); }

                return _items[index].ViewModel;
            }
        }
        public int Count
        {
            get { return OriginalCollectionAvailable ? _itemsCursorCount : 0; }
        }
        //---------------------------------------------------------------------
        public bool IsAvailable
        {
            get { return OriginalCollectionAvailable; }
        }
        //---------------------------------------------------------------------
        public SelectedSortedItemCollection SelectedItems
        {
            get { return _selectedItems; }
        }
        public TViewModelCollection OriginalCollection
        {
            get { return _originalCollection; }
            set
            {
                if (!ReferenceEquals(_originalCollection, value))
                {
                    UpdatingOriginalCollection(_originalCollection);

                    var oldItemsCursorCount = Count;
                    _originalCollection = value;

                    UpdateOriginalCollection(_originalCollection);
                    _reset(OriginalCollectionAvailable, oldItemsCursorCount);
                    UpdatedOriginalCollection(_originalCollection);

                    _onOriginalCollectionChanged();
                }
            }
        }
        public SortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                if (value < SortDirection.Asc || value > SortDirection.Desc)
                { throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(SortDirection)); }

                _sortDirection = value;
            }
        }
        public bool MultiSelectItems
        {
            get { return _selectedItems.MultiSelectViewModels; }
            set { _selectedItems.MultiSelectViewModels = value; }
        }
        //---------------------------------------------------------------------
        public void Sort(IComparer<TViewModel> comparer)
        {
            if (comparer == null)
            { throw new ArgumentNullException(nameof(comparer)); }
            if (!OriginalCollectionAvailable)
            { throw new InvalidOperationException(); }

            var sortedItemComparer = new SortedItemComparer(comparer, _sortDirection);
            _items.Sort(0, _itemsCursorCount, sortedItemComparer);
        }
        public void Clear()
        {
            _items.Clear();
            _selectedItems.Clear();
            _clear(true);
        }
        //---------------------------------------------------------------------
        public IEnumerator<TViewModel> GetEnumerator()
        {
            return new ViewModelEnumerator(_items);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //---------------------------------------------------------------------
        public event EventHandler<SortedItemCollectionCountEventArgs> CountChanged;
        public event EventHandler OriginalCollectionChanged;
        //---------------------------------------------------------------------
    }
}