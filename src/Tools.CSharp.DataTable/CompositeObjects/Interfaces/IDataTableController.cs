namespace Tools.CSharp.DataTable.CompositeObjects
{
    public interface IDataTableController
    {
        //---------------------------------------------------------------------
        System.Data.DataTable DataTable { get; }
        //---------------------------------------------------------------------
        void Clear();
        //---------------------------------------------------------------------
    }
}