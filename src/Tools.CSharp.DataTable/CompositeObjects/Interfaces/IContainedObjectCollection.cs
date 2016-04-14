using System;
using System.ComponentModel;

namespace Tools.CSharp.DataTable.CompositeObjects
{
    //-------------------------------------------------------------------------
    public interface IContainedObjectCollection : IDisposable
    {
        //---------------------------------------------------------------------
        int Count { get; }
        //---------------------------------------------------------------------
        event ListChangedEventHandler Changed;
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public interface IContainedObjectCollection<out TContainedObject> : IContainedObjectCollection
    {
        //---------------------------------------------------------------------
        TContainedObject this[int index] { get; }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}