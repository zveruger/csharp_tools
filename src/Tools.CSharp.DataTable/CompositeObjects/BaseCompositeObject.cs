using System;
using System.Collections.Generic;
using System.Data;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    public abstract class BaseCompositeObject<TDataSet> : ICompositeObject
        where TDataSet : DataSet
    {
        #region private
        private readonly Lazy<TDataSet> _lazyDataSet; 
        private readonly DataTableControllerCollection _controllers =  new DataTableControllerCollection();
        #endregion
        #region protected
        protected TDataSet DataSet
        {
            get { return _lazyDataSet.Value; }
        }
        protected DataTableControllerCollection Controllers
        {
            get { return _controllers; }
        }
        //---------------------------------------------------------------------
        protected sealed class DataTableControllerCollection
        {
            #region private
            private readonly Dictionary<Type, IDataTableController> _controllers = new Dictionary<Type, IDataTableController>(); 
            #endregion
            internal DataTableControllerCollection()
            { }

            //-----------------------------------------------------------------
            public void Add(IDataTableController dataTableController)
            {
                if (dataTableController == null)
                { throw new ArgumentNullException(nameof(dataTableController)); }

                var typeDataTableController = dataTableController.GetType();
                if (!_controllers.ContainsKey(typeDataTableController))
                { _controllers.Add(typeDataTableController, dataTableController); }
            }
            //-----------------------------------------------------------------
            public TDataTableController Get<TDataTableController>() 
                where TDataTableController : IDataTableController
            {
                var typeDataTableController = typeof(TDataTableController);
                IDataTableController dataTableController;

                if (_controllers.TryGetValue(typeDataTableController, out dataTableController))
                { return (TDataTableController)dataTableController; }

                return default(TDataTableController);
            }
            //-----------------------------------------------------------------
            public int Count
            {
                get { return _controllers.Count; }
            }
            //-----------------------------------------------------------------
            internal void ClearControllers()
            {
                foreach (var dataTableController in _controllers)
                { dataTableController.Value.Clear(); }
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        protected abstract TDataSet CreateDataSet();
        //---------------------------------------------------------------------
        protected virtual void Cleared()
        { }
        #endregion
        protected BaseCompositeObject()
        {
            _lazyDataSet = new Lazy<TDataSet>(CreateDataSet);
        }

        //---------------------------------------------------------------------
        public void Clear()
        {
            _controllers.ClearControllers();

            Cleared();
        }
        //---------------------------------------------------------------------
    }
}