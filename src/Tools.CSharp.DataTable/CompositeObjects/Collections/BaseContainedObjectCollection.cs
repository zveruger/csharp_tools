using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    public abstract class BaseContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection> :
        CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection>
        where TDataTableController : IDataTableController
        where TContainedObject : BaseContainedObject<TDataTableController, TContainedObjectCollection, TRow, TContainedObject>
        where TRow : DataRow
        where TContainedObjectCollection : CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection>
    {
        #region private
        private readonly TDataTableController _controller;
        private readonly DataView _dataView;
        private readonly BaseCacheContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection> _cacheContainedObjectCollection;
        //---------------------------------------------------------------------
        private readonly object _lock = new object();
        //---------------------------------------------------------------------
        private Dictionary<int, TContainedObject> _cacheObjects;
        //---------------------------------------------------------------------
        private void _subscribeDataViewAllEvents(bool addEvents)
        {
            if (addEvents)
            { _dataView.ListChanged += _dataViewOnListChanged; }
            else
            { _dataView.ListChanged -= _dataViewOnListChanged; }
        }
        private void _dataViewOnListChanged(object sender, ListChangedEventArgs e)
        {
            CollectionChange(e);
        }
        //---------------------------------------------------------------------
        private void _resetCacheObjects()
        {
            lock (_lock)
            {
                var oldCacheObjects = _cacheObjects;
                _cacheObjects = new Dictionary<int, TContainedObject>();

                for (var i = 0; i < _dataView.Count; i++)
                {
                    var row = GetRowWithoutVerification(i);
                    var rowUniqueId = GetRowUniqueIdWithoutVerification(row);

                    TContainedObject containedObject;
                    if (oldCacheObjects.TryGetValue(rowUniqueId, out containedObject))
                    {
                        //oldCacheObjects.Remove(rowUniqueId);
                        _cacheObjects.Add(rowUniqueId, containedObject);
                    }
                }

                oldCacheObjects.Clear();
            }
        }
        //---------------------------------------------------------------------
        private TContainedObject _createObject(int rowUniqueId)
        {
            var index = FindIndexImp(rowUniqueId);

            if (index == -1)
            { return null; }

            var row = GetRowWithoutVerification(index);

            return _createObject(row);
        }
        private TContainedObject _createObject(TRow row)
        {
            if (_cacheContainedObjectCollection == null)
            { return CreateObject(row); }

            var cacheRowUniqueId = _cacheContainedObjectCollection.GetRowUniqueId(row);
            var cacheContainedObject = _cacheContainedObjectCollection.GetObjectWithoutVerification(cacheRowUniqueId);

            return cacheContainedObject;
        }
        #endregion
        #region internal
        internal sealed override TRow GetRowWithoutVerification(int index)
        {
            return (TRow)_dataView[index].Row;
        }
        internal sealed override TContainedObject GetObjectByIndex(int index)
        {
            var row = GetRowWithoutVerification(index);

            if (_cacheObjects == null)
            { return _createObject(row); }

            var rowUniqueId = GetRowUniqueIdWithoutVerification(row);
            TContainedObject containedObject;

            lock (_lock)
            {
                if (!_cacheObjects.TryGetValue(rowUniqueId, out containedObject))
                {
                    containedObject = CreateObject(row);
                    _cacheObjects.Add(rowUniqueId, containedObject);
                }
            }

            return containedObject;
        }
        //---------------------------------------------------------------------
        internal sealed override int GetRowUniqueIdWithoutVerification(TRow row)
        {
            return GetRowUniqueId(row);
        }
        //---------------------------------------------------------------------
        internal sealed override void DeleteRowFromCollectionImp(TRow row)
        {
            DeleteRow(row);
        }
        //---------------------------------------------------------------------
        internal sealed override int FindIndexImp(int rowUniqueId)
        {
            return _dataView.Find(rowUniqueId);
        }
        //---------------------------------------------------------------------
        internal sealed override TContainedObject GetObjectWithoutVerification(int rowUniqueId)
        {
            if (_cacheObjects == null)
            { return _createObject(rowUniqueId); }

            TContainedObject containedObject;

            lock (_lock)
            {
                if (!_cacheObjects.TryGetValue(rowUniqueId, out containedObject))
                {
                    var index = FindIndexImp(rowUniqueId);
                    if (index != -1)
                    {
                        var row = GetRowWithoutVerification(index);
                        containedObject = CreateObject(row);

                        _cacheObjects.Add(rowUniqueId, containedObject);
                    }
                }
            }

            return containedObject;
        }
        internal sealed override TContainedObject GetObjectWithoutVerification(TRow row)
        {
            if (_cacheObjects == null)
            { return _createObject(row); }

            var rowUniqueId = GetRowUniqueIdWithoutVerification(row);
            TContainedObject containedObject;

            lock (_lock)
            {
                if (!_cacheObjects.TryGetValue(rowUniqueId, out containedObject))
                {
                    var index = FindIndexImp(rowUniqueId);
                    if (index != -1)
                    {
                        containedObject = CreateObject(row);

                        _cacheObjects.Add(rowUniqueId, containedObject);
                    }
                }
            }

            return containedObject;
        }
        #endregion
        #region protected
        protected abstract TContainedObject CreateObject(TRow row);
        protected abstract int GetRowUniqueId(TRow row);
        //---------------------------------------------------------------------
        protected DataView DataView
        {
            get { return _dataView; }
        }
        //---------------------------------------------------------------------
        protected bool TryObject(int rowUniqueId, out TContainedObject obj)
        {
            obj = GetObjectWithoutVerification(rowUniqueId);
            return obj != null;
        }
        //---------------------------------------------------------------------
        protected virtual void DeleteRow(TRow row)
        {
            var rowUniqueId = GetRowUniqueIdFromCollection(row);

            lock (_lock)
            { _cacheObjects.Remove(rowUniqueId); }

            base.DeleteRowFromCollectionImp(row);
        }
        //---------------------------------------------------------------------
        protected override void ObjectRemove(TContainedObject obj)
        {
            if (_cacheObjects != null)
            {
                var rowUniqueId = GetRowUniqueId(obj.InternalRow);

                lock (_lock)
                { _cacheObjects.Remove(rowUniqueId); }
            }
           
            base.ObjectRemove(obj);
        }
        protected override void CollectionChange(ListChangedEventArgs e)
        {
            if (_cacheObjects != null)
            {
                var changedType = e.ListChangedType;
                if (changedType == ListChangedType.Reset)
                { _resetCacheObjects(); }
            }

            base.CollectionChange(e);
        }
        //---------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    if (_dataView != null)
                    {
                        _subscribeDataViewAllEvents(false);
                        _dataView.Dispose();
                    }
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected BaseContainedObjectCollection(
            TDataTableController controller,
            string rowFilter,
            string sort,
            BaseCacheContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection> cacheContainedObjectCollection
        ) : this(controller, rowFilter, sort, false)
        {
            _cacheContainedObjectCollection = cacheContainedObjectCollection;
        }
        protected BaseContainedObjectCollection(TDataTableController controller, string rowFilter, string sort, bool createCacheObjects = false)
        {
            if (controller == null)
            { throw new ArgumentNullException(nameof(controller)); }

            //-----------------------------------------------------------------
            _controller = controller;
            //-----------------------------------------------------------------
            _dataView = new DataView(_controller.DataTable, rowFilter, sort, DataViewRowState.CurrentRows);
            _subscribeDataViewAllEvents(true);
            //-----------------------------------------------------------------
            if (createCacheObjects)
            { _cacheObjects = new Dictionary<int, TContainedObject>(); }
            //-----------------------------------------------------------------
        }

        //---------------------------------------------------------------------
        public sealed override TDataTableController Controller
        {
            get { return _controller; }
        }
        //---------------------------------------------------------------------
        public sealed override int Count
        {
            get { return _dataView.Count; }
        }
        //---------------------------------------------------------------------
    }
}