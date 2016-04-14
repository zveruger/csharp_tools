using System;
using System.Data;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    //-------------------------------------------------------------------------
    public abstract class BaseContainedObject<TDataController, TContainedObjectCollection, TRow, TContainedObject> : IContainedObject<TContainedObjectCollection>
           where TDataController : IDataTableController
           where TContainedObjectCollection : CoreContainedObjectCollection<TDataController, TContainedObject, TRow, TContainedObjectCollection>
           where TRow : DataRow
           where TContainedObject : BaseContainedObject<TDataController, TContainedObjectCollection, TRow, TContainedObject>
    {
        #region private
        private readonly TContainedObjectCollection _owner;
        private readonly TRow _row;
        //---------------------------------------------------------------------
        private bool _isDestroyed;
        #endregion
        #region protected
        protected TRow Row
        {
            get
            {
                ThrowIfDestroyed();
                return _row;
            }
        }
        //---------------------------------------------------------------------
        protected void ThrowIfDestroyed()
        {
            if (_isDestroyed)
            { throw new ContainedObjectDestroyedException(this); }
        }
        #endregion
        protected BaseContainedObject(TRow row, TContainedObjectCollection owner)
        {
            if (row == null)
            { throw new ArgumentNullException(nameof(row)); }
            if (owner == null)
            { throw new ArgumentNullException(nameof(owner)); }

            _row = row;
            _owner = owner;
        }

        //---------------------------------------------------------------------
        internal TRow InternalRow
        {
            get { return Row; }
        }
        //---------------------------------------------------------------------
        public TContainedObjectCollection Owner
        {
            get { return _owner; }
        }
        //---------------------------------------------------------------------
        public int UniqueId
        {
            get { return Owner.GetRowUniqueIdWithoutVerification(Row); }
        }
        //---------------------------------------------------------------------
        public bool IsNew
        {
            get { return Row.RowState == DataRowState.Detached; }
        }
        //---------------------------------------------------------------------
        public bool IsDestroyed
        {
            get { return _isDestroyed; }
        }
        //---------------------------------------------------------------------
        public void Destroy()
        {
            if (!_isDestroyed)
            {
                _isDestroyed = true;
                _owner.DeleteObjectFromCollection((TContainedObject)this);
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}