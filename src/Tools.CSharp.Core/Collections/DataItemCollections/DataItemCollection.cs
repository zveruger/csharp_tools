using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tools.CSharp.Collections
{
    //-------------------------------------------------------------------------
    public class DataItemCollection<TDataItem, TData> : IList<TDataItem>
       where TDataItem : DataItem<TData>
    {
        #region private
        private readonly List<TDataItem> _items = new List<TDataItem>();
        private readonly DataItemComparer _comparer;
        //---------------------------------------------------------------------
        private TDataItem _bufferItem;
        #endregion
        #region protected
        protected List<TDataItem> Items
        {
            get { return _items; }
        }

        protected abstract class DataItemComparer : IComparer<TDataItem>
        {
            #region protected
            protected abstract int Comparer(TDataItem x, TDataItem y);
            #endregion
            public int Compare(TDataItem x, TDataItem y)
            {
                return Comparer(x, y);
            }
        }
        protected class DataItemNameComparer : DataItemComparer
        {
            #region protected
            protected override int Comparer(TDataItem x, TDataItem y)
            {
                return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            }
            #endregion
        }

        protected virtual void AddInternal(TDataItem item)
        {
            _items.Add(item);
        }
        protected virtual void AddRangeInternal(IEnumerable<TDataItem> items)
        {
            _items.AddRange(items);
        }
        protected virtual void ClearInternal()
        {
            _bufferItem = default(TDataItem);
            _items.Clear();
        }
        protected virtual void SortInternal(IComparer<TDataItem> comparer)
        {
            _items.Sort(comparer);
        }
        #endregion
        protected DataItemCollection()
            : this(new DataItemNameComparer())
        { }
        protected DataItemCollection(DataItemComparer comparer)
        {
            if (comparer == null)
            { throw new ArgumentNullException(nameof(comparer)); }

            _comparer = comparer;
        }

        //---------------------------------------------------------------------
        public TDataItem this[int index]
        {
            get { return _items[index]; }
            set
            {
                if (value == null)
                { throw new ArgumentNullException(nameof(value)); }

                _items[index] = value;
            }
        }
        public TDataItem this[TData data]
        {
            get
            {
                if (_bufferItem == default(TDataItem) || !_bufferItem.Data.Equals(data))
                {
                    _bufferItem = default(TDataItem);

                    foreach (var item in _items.Where(item => item.Data.Equals(data)))
                    {
                        _bufferItem = item;
                        break;
                    }
                }

                return _bufferItem;
            }
        }
        public int Count
        {
            get { return _items.Count; }
        }
        //---------------------------------------------------------------------
        public bool IsReadOnly
        {
            get { return false; }   
        }
        //---------------------------------------------------------------------
        public void Add(TDataItem item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            AddInternal(item);
        }
        public void AddRange(IEnumerable<TDataItem> items)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            AddRangeInternal(items);
        }
        public void Clear()
        {
            ClearInternal();
        }
        public int IndexOf(TDataItem item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            return _items.IndexOf(item);
        }
        public void Insert(int index, TDataItem item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            _items.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }
        public bool Contains(TDataItem item)
        {
            return item != null && _items.Contains(item);
        }
        public bool Remove(TDataItem item)
        {
            return item != null && _items.Remove(item);
        }
        public bool Remove(TData data)
        {
            if (_bufferItem != default(TDataItem) && _bufferItem.Data.Equals(data))
            { _bufferItem = default(TDataItem); }

            var removed = false;
            for (var itemCounter = 0; itemCounter < _items.Count; ++itemCounter)
            {
                var item = _items[itemCounter];
                if (item.Data.Equals(data))
                {
                    RemoveAt(itemCounter);
                    removed = true;
                    break;
                }
            }
            return removed;
        }
        public bool Remove(Func<TDataItem, bool> functor)
        {
            if (functor == null)
            { throw new ArgumentNullException(nameof(functor)); }

            for (var itemCounter = 0; itemCounter < Count; itemCounter++)
            {
                var item = _items[itemCounter];
                if (functor(item))
                {
                    RemoveAt(itemCounter);
                    return true;
                }
            }

            return false;
        }
        public string GetName(TData data)
        {
            var item = this[data];
            return item == default(TDataItem) ? string.Empty : item.Name;
        }
        public void CopyTo(TDataItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }
        public void Sort()
        {
            Sort(_comparer);
        }
        public void Sort(IComparer<TDataItem> comparer)
        {
            if (comparer == null)
            { throw new ArgumentNullException(nameof(comparer)); }

            SortInternal(comparer);
        }
        public int BinarySearch(TDataItem item)
        {
            return item == null ? -1 : _items.BinarySearch(item);
        }
        public int BinarySearch(TDataItem item, IComparer<TDataItem> comparer)
        {
            return item == null ? -1 : _items.BinarySearch(item, comparer);
        }
        public int BinarySearch(int startIndex, int count, TDataItem item, IComparer<TDataItem> comparer)
        {
            return item == null ? -1 : _items.BinarySearch(startIndex, count, item, comparer);
        }
        //---------------------------------------------------------------------
        public TData GetData(TData data, TData defaultData)
        {
            var dataItem = this[data];
            return dataItem == null ? defaultData : data;
        }
        public TData GetData(TData data, TData defaultData, out bool returnDefaultData)
        {
            var dataItem = this[data];
            returnDefaultData = dataItem == null;
            return returnDefaultData ? defaultData : data;
        }
        //---------------------------------------------------------------------
        public IEnumerator<TDataItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public class DataItemCollection<TDataItem, TData, TDataItemCollection> : DataItemCollection<TDataItem, TData>
        where TDataItem : DataItem<TData>
        where TDataItemCollection : DataItemCollection<TDataItem, TData>, new()
    {
        #region private
        private static TDataItemCollection _Instance;
        #endregion

        //---------------------------------------------------------------------
        public static TDataItemCollection Instance
        {
            get { return LazyInitializer.EnsureInitialized(ref _Instance, () => new TDataItemCollection()); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}