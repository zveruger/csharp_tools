using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    //-------------------------------------------------------------------------
    public abstract class BaseContainedDifferenceCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection> :
        BaseContainedDifferenceCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection, TContainedObjectCollection>
        where TDataTableController : IDataTableController
        where TContainedObject : BaseContainedObject<TDataTableController, TContainedObjectCollection, TRow, TContainedObject>
        where TRow : DataRow
        where TContainedObjectCollection : CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection>
    {
        protected BaseContainedDifferenceCollection(TContainedObjectCollection minuendCollection, IContainedObjectCollection<IContainedObject> subtrahendCollection, bool differenceResultEmptyIfSubtrahendNotEmpty = false)
            : base(minuendCollection, subtrahendCollection, differenceResultEmptyIfSubtrahendNotEmpty)
        { }
    }
    //-------------------------------------------------------------------------
    public abstract class BaseContainedDifferenceCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection, TCacheContainedObjectCollection> :
        CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TCacheContainedObjectCollection>
        where TDataTableController : IDataTableController
        where TContainedObject : BaseContainedObject<TDataTableController, TCacheContainedObjectCollection, TRow, TContainedObject>
        where TRow : DataRow
        where TContainedObjectCollection : CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TCacheContainedObjectCollection>
        where TCacheContainedObjectCollection : CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TCacheContainedObjectCollection>
    {
        #region private
        private readonly TContainedObjectCollection _minuendCollection;
        private readonly IContainedObjectCollection<IContainedObject> _subtrahendCollection;
        private readonly bool _differenceResultEmptyIfSubtrahendNotEmpty;
        //---------------------------------------------------------------------
        private readonly List<int> _differenceRowUniqueIds = new List<int>();
        //---------------------------------------------------------------------
        private void _subscribeMinuendCollectionAllEvents(bool addEvents)
        {
            if (addEvents)
            { _minuendCollection.Changed += _minuendCollectionOnChanged; }
            else
            { _minuendCollection.Changed -= _minuendCollectionOnChanged; }
        }
        private void _minuendCollectionOnChanged(object sender, ListChangedEventArgs e)
        {
            if (_differenceRowUniqueIdsUpdateAvailable())
            {
                var changeType = e.ListChangedType;
                switch (changeType)
                {
                    case ListChangedType.ItemAdded:
                        { _minuendAddRowIndexInCache(e.NewIndex); }
                        break;
                    case ListChangedType.ItemDeleted:
                        { _minuendDeleteRowIndexFromCache(); }
                        break;
                    case ListChangedType.Reset:
                        { _minuendResetRowIndexesInCache(); }
                        break;
                }
            }
        }

        private void _minuendAddRowIndexInCache(int rowIndex)
        {
            var mainRowUniqueId = GetMinuendRowUniqueId(_minuendCollection.GetRowWithoutVerification(rowIndex));
            if (mainRowUniqueId != -1)
            {
                _differenceRowUniqueIds.Add(mainRowUniqueId);
                CollectionChange(new ListChangedEventArgs(ListChangedType.ItemAdded, Count - 1));
            }
        }
        private void _minuendDeleteRowIndexFromCache()
        {
            var rowIndex = _minuendResetRowIndexesInCache(false);
            CollectionChange(new ListChangedEventArgs(ListChangedType.ItemDeleted, rowIndex));
        }
        private void _minuendResetRowIndexesInCache()
        {
            _minuendResetRowIndexesInCache(true);
            CollectionChange(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private int _minuendResetRowIndexesInCache(bool fullReset)
        {
            var resultMainRowIndex = -1;

            for (var i = 0; i < _differenceRowUniqueIds.Count; i++)
            {
                var mainRowUniqueId = _differenceRowUniqueIds[i];
                var mainRowIndex = FindMinuendRowIndex(_minuendCollection, mainRowUniqueId);

                if (mainRowIndex == -1)
                {
                    if (_differenceRowUniqueIds.Remove(mainRowUniqueId))
                    {
                        if (fullReset)
                        { --i; }
                        else
                        {
                            resultMainRowIndex = i;
                            break;
                        }
                    }
                }
            }

            return resultMainRowIndex;
        }
        //---------------------------------------------------------------------
        private void _subscribeSubtrahendCollectionAllEvents(bool addEvents)
        {
            if (addEvents)
            { _subtrahendCollection.Changed += _subtrahendCollectionOnChanged; }
            else
            { _subtrahendCollection.Changed -= _subtrahendCollectionOnChanged; }
        }
        private void _subtrahendCollectionOnChanged(object sender, ListChangedEventArgs e)
        {
            var changeType = e.ListChangedType;
            switch (changeType)
            {
                case ListChangedType.ItemAdded:
                    { _subtrahendDeleteRowIndexFromCache(e.NewIndex); }
                    break;
                case ListChangedType.ItemDeleted:
                case ListChangedType.Reset:
                    { _subtrahendResetRowIndexesInCache(); }
                    break;
            }
        }

        private void _subtrahendDeleteRowIndexFromCache(int rowIndex)
        {
            if (_differenceRowUniqueIdsUpdateAvailable())
            {
                var exludeRow = GetSubtrahendRow(_subtrahendCollection, rowIndex);
                if (exludeRow != null)
                {
                    var excludeRowUniqueId = GetSubtrahendRowUniqueId(exludeRow);
                    if (excludeRowUniqueId != -1)
                    {
                        var cacheIndex = _differenceRowUniqueIds.FindIndex(x => x == excludeRowUniqueId);
                        if (cacheIndex != -1)
                        {
                            _differenceRowUniqueIds.RemoveAt(cacheIndex);
                            CollectionChange(new ListChangedEventArgs(ListChangedType.ItemDeleted, cacheIndex));
                        }
                    }
                }
            }
            else
            {
                if (_differenceRowUniqueIds.Count != 0)
                { _resetDifferenceRowUniqueIds(); }
            }
        }
        private void _subtrahendResetRowIndexesInCache()
        {
            if (_differenceRowUniqueIdsUpdateAvailable())
            {
                _initialize();
                CollectionChange(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
            else
            {
                if (_differenceRowUniqueIds.Count != 0)
                { _resetDifferenceRowUniqueIds(); }
            }
        }
        //---------------------------------------------------------------------
        private void _initialize()
        {
            _differenceRowUniqueIds.Clear();

            for (var i = 0; i < _minuendCollection.Count; i++)
            {
                var mainRowUniqueId = GetMinuendRowUniqueId(_minuendCollection.GetRowWithoutVerification(i));

                if (mainRowUniqueId != -1)
                { _differenceRowUniqueIds.Add(mainRowUniqueId); }
            }

            for (var i = 0; i < _subtrahendCollection.Count; i++)
            {
                var exludeRow = GetSubtrahendRow(_subtrahendCollection, i);
                if (exludeRow != null)
                {
                    var excludeRowUniqueId = GetSubtrahendRowUniqueId(exludeRow);

                    if (excludeRowUniqueId != -1)
                    { _differenceRowUniqueIds.Remove(excludeRowUniqueId); }
                }
            }
        }
        //---------------------------------------------------------------------
        private bool _differenceRowUniqueIdsUpdateAvailable()
        {
            if (_subtrahendCollection == null)
            { return false; }

            if (_differenceResultEmptyIfSubtrahendNotEmpty && _subtrahendCollection.Count != 0)
            { return false; }

            return true;
        }
        private void _resetDifferenceRowUniqueIds()
        {
            _differenceRowUniqueIds.Clear();
            CollectionChange(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        #endregion
        #region internal
        internal sealed override TRow GetRowWithoutVerification(int index)
        {
            return _minuendCollection.GetRowWithoutVerification(index);
        }
        internal sealed override TContainedObject GetObjectByIndex(int index)
        {
            var rowUniqueId = _differenceRowUniqueIds[index];
            return _minuendCollection.GetObjectWithoutVerification(rowUniqueId);
        }
        //---------------------------------------------------------------------
        internal override int GetRowUniqueIdWithoutVerification(TRow row)
        {
            return _minuendCollection.GetRowUniqueIdWithoutVerification(row);
        }
        //---------------------------------------------------------------------
        internal override int FindIndexImp(int rowUniqueId)
        {
            for (var i = 0; i < _differenceRowUniqueIds.Count; i++)
            {
                if (_differenceRowUniqueIds[i] == rowUniqueId)
                { return i; }
            }
            return -1;
        }
        //---------------------------------------------------------------------
        internal sealed override TContainedObject GetObjectWithoutVerification(int rowUniqueId)
        {
            return _minuendCollection.GetObjectWithoutVerification(rowUniqueId);
        }
        internal override TContainedObject GetObjectWithoutVerification(TRow row)
        {
            return _minuendCollection.GetObjectWithoutVerification(row);
        }
        #endregion
        #region protected
        protected static TExcludeRow GetSubtrahendRow<TExcludeDataTableController, TExcludeContainedObject, TExcludeRow, TExcludeContainedObjectCollection>(
            CoreContainedObjectCollection<TExcludeDataTableController, TExcludeContainedObject, TExcludeRow, TExcludeContainedObjectCollection> excludeCollection, int rowIndex)
            where TExcludeDataTableController : IDataTableController
            where TExcludeContainedObject : BaseContainedObject<TExcludeDataTableController, TExcludeContainedObjectCollection, TExcludeRow, TExcludeContainedObject>
            where TExcludeRow : DataRow
            where TExcludeContainedObjectCollection : CoreContainedObjectCollection<TExcludeDataTableController, TExcludeContainedObject, TExcludeRow, TExcludeContainedObjectCollection>
        {
            if (excludeCollection == null)
            { throw new ArgumentNullException(nameof(excludeCollection)); }

            return excludeCollection.GetRow(rowIndex);
        }
        //---------------------------------------------------------------------
        protected abstract DataRow GetSubtrahendRow(IContainedObjectCollection<IContainedObject> excludeCollection, int rowIndex);
        protected abstract int GetSubtrahendRowUniqueId(DataRow row);
        //---------------------------------------------------------------------
        protected virtual int GetMinuendRowUniqueId(TRow row)
        {
            return _minuendCollection.GetRowUniqueIdFromCollection(row);
        }
        protected virtual int FindMinuendRowIndex(TContainedObjectCollection minuendCollection, int minuendRowUniqueId)
        {
            if (minuendCollection == null)
            { throw new ArgumentNullException(nameof(minuendCollection)); }
            if (!ReferenceEquals(minuendCollection, _minuendCollection))
            { throw new ArgumentException(string.Empty, nameof(minuendCollection)); }

            return minuendCollection.FindIndex(minuendRowUniqueId);
        }
        //---------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    if (_minuendCollection != null)
                    { _subscribeMinuendCollectionAllEvents(false); }

                    if (_subtrahendCollection != null)
                    { _subscribeSubtrahendCollectionAllEvents(false); }
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected BaseContainedDifferenceCollection(TContainedObjectCollection minuendCollection, IContainedObjectCollection<IContainedObject> subtrahendCollection, bool differenceResultEmptyIfSubtrahendNotEmpty = false)
        {
            if (minuendCollection == null)
            { throw new ArgumentNullException(nameof(minuendCollection)); }

            _minuendCollection = minuendCollection;
            _subtrahendCollection = subtrahendCollection;
            _differenceResultEmptyIfSubtrahendNotEmpty = differenceResultEmptyIfSubtrahendNotEmpty;

            _subscribeMinuendCollectionAllEvents(true);

            if (_subtrahendCollection != null)
            { _subscribeSubtrahendCollectionAllEvents(true); }

            if (_differenceRowUniqueIdsUpdateAvailable())
            { _initialize(); }
        }

        //---------------------------------------------------------------------
        public sealed override TDataTableController Controller
        {
            get { return _minuendCollection.Controller; }
        }
        //---------------------------------------------------------------------
        public sealed override int Count
        {
            get { return _differenceRowUniqueIds.Count; }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}