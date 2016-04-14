using System.Data;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    public abstract class BaseCacheContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection> :
        BaseContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection>
        where TDataTableController : IDataTableController
        where TContainedObject : BaseContainedObject<TDataTableController, TContainedObjectCollection, TRow, TContainedObject>
        where TRow : DataRow
        where TContainedObjectCollection : CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection>
    {
        protected BaseCacheContainedObjectCollection(TDataTableController controller, string sort) 
            : base(controller, "", sort, true)
        { }
    }
}