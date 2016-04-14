using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Tools.CSharp.Collections
{
    public class ImprovedObservableCollection<T> : Collection<T>, INotifyPropertyChanged
        where T : class
    {
        #region private
        private const string _CountString = "Count";
        private const string _IndexerName = "Item[]";
        //---------------------------------------------------------------------
        private readonly SimpleMonitor _monitor = new SimpleMonitor();
        private readonly bool _improve;
        //---------------------------------------------------------------------
        private sealed class SimpleMonitor : IDisposable
        {
            #region private
            private int _busyCount;
            #endregion
            //-----------------------------------------------------------------
            public bool Busy
            {
                get { return _busyCount > 0; }
            }
            //-----------------------------------------------------------------
            public void Enter()
            {
                ++_busyCount;
            }
            //-----------------------------------------------------------------
            public void Dispose()
            {
                --_busyCount;
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private void _copyFrom(IEnumerable<T> collection)
        {
            var items = Items;
            if (collection != null && items != null)
            {
                foreach (var item in collection)
                { items.Add(item); }
            }
        }
        //---------------------------------------------------------------------
        private void _OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        private void _OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            if (_improve)
            { OnImprovedCollectionChanged(new ImprovedNotifyCollectionChangedEventArgs<T>(action, item, index)); }
            else
            { OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index)); }
        }
        private void _OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index, int oldIndex)
        {
            if (_improve)
            { OnImprovedCollectionChanged(new ImprovedNotifyCollectionChangedEventArgs<T>(action, item, index, oldIndex)); }
            else
            { OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex)); }
        }
        private void _OnCollectionChanged(NotifyCollectionChangedAction action, T oldItem, T newItem, int index)
        {
            if (_improve)
            { OnImprovedCollectionChanged(new ImprovedNotifyCollectionChangedEventArgs<T>(action, newItem, oldItem, index)); }
            else
            { OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index)); }
        }
        private void _OnCollectionReset()
        {
            const NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;

            if (_improve)
            { OnImprovedCollectionChanged(new ImprovedNotifyCollectionChangedEventArgs<T>(action)); }
            else
            { OnCollectionChanged(new NotifyCollectionChangedEventArgs(action)); }
        }
        #endregion
        #region protected
        protected IDisposable BlockReentrancy()
        {
            _monitor.Enter();
            return _monitor;
        }
        protected void CheckReentrancy()
        {
            if (_monitor.Busy)
            {
                Delegate[] delegates = null;
                if (_improve)
                {
                    if (ImprovedCollectionChanged != null)
                    { delegates = ImprovedCollectionChanged.GetInvocationList(); }
                }
                else if (CollectionChanged != null)
                { delegates = CollectionChanged.GetInvocationList(); }

                if (delegates != null && delegates.Length > 1)
                { throw new InvalidOperationException(); }
            }
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            CheckReentrancy();

            var removeItem = this[oldIndex];

            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, removeItem);

            _OnPropertyChanged(_IndexerName);
            _OnCollectionChanged(NotifyCollectionChangedAction.Move, removeItem, newIndex, oldIndex);
        }

        protected virtual void OnImprovedCollectionChanged(ImprovedNotifyCollectionChangedEventArgs<T> e)
        {
            if (ImprovedCollectionChanged != null)
            {
                using (BlockReentrancy())
                { ImprovedCollectionChanged(this, e); }
            }
        }
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                using (BlockReentrancy())
                { CollectionChanged(this, e); }
            }
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected override void ClearItems()
        {
            CheckReentrancy();

            base.ClearItems();

            _OnPropertyChanged(_CountString);
            _OnPropertyChanged(_IndexerName);
            _OnCollectionReset();
        }
        protected override void RemoveItem(int index)
        {
            CheckReentrancy();

            var removedItem = this[index];
            base.RemoveItem(index);

            _OnPropertyChanged(_CountString);
            _OnPropertyChanged(_IndexerName);
            _OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }
        protected override void InsertItem(int index, T item)
        {
            CheckReentrancy();

            base.InsertItem(index, item);

            _OnPropertyChanged(_CountString);
            _OnPropertyChanged(_IndexerName);
            _OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }
        protected override void SetItem(int index, T item)
        {
            CheckReentrancy();

            var originalItem = this[index];
            base.SetItem(index, item);

            _OnPropertyChanged(_IndexerName);
            _OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }
        #endregion
        public ImprovedObservableCollection(bool improve = true)
        {
            _improve = improve;
        }
        public ImprovedObservableCollection(List<T> list, bool improve = true)
            : base(list != null ? new List<T>(list.Count) : list)
        {
            _improve = improve;

            _copyFrom(list);
        }
        public ImprovedObservableCollection(IEnumerable<T> collection, bool improve = true)
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            _improve = improve;

            _copyFrom(collection);
        }

        //---------------------------------------------------------------------
        public bool IsImproved
        {
            get { return _improve; }
        }
        //---------------------------------------------------------------------
        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }
        //---------------------------------------------------------------------
        public event EventHandler<ImprovedNotifyCollectionChangedEventArgs<T>> ImprovedCollectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        //---------------------------------------------------------------------
    }
}
