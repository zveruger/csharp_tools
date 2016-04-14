using System;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    public abstract class BaseDataTableController<TOwner, TDataTable> : IDataTableController
        where TOwner : ICompositeObject
        where TDataTable : System.Data.DataTable
    {
        #region private
        private readonly TOwner _owner;
        private readonly TDataTable _table;
        #endregion
        #region protected
        protected TDataTable Table
        {
            get { return _table; }
        }
        //---------------------------------------------------------------------
        protected virtual void Cleared()
        { }
        #endregion
        protected BaseDataTableController(TOwner owner, TDataTable table)
        {
            if (owner == null)
            { throw new ArgumentNullException(nameof(owner)); }
            if (table == null)
            { throw new ArgumentNullException(nameof(table)); }

            _owner = owner;
            _table = table;
        }

        //---------------------------------------------------------------------
        public TOwner Owner
        {
            get { return _owner; }
        }
        //---------------------------------------------------------------------
        System.Data.DataTable IDataTableController.DataTable
        {
            get { return _table; }
        }
        //---------------------------------------------------------------------
        public void Clear()
        {
            _table.Clear();

            Cleared();
        }
        //---------------------------------------------------------------------
    }
}
