namespace Tools.CSharp.DataTable.CompositeObjects
{
    //-------------------------------------------------------------------------
    public interface IContainedObject
    {
        //---------------------------------------------------------------------
        int UniqueId { get; }
        //---------------------------------------------------------------------
        bool IsNew { get; }
        bool IsDestroyed { get; }
        //---------------------------------------------------------------------
        void Destroy();
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public interface IContainedObject<out TContainedObjectCollection> : IContainedObject
        where TContainedObjectCollection : IContainedObjectCollection
    {
        //---------------------------------------------------------------------
        TContainedObjectCollection Owner { get; }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}