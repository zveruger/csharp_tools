using System;

namespace Tools.CSharp.StateMachines
{
    public interface IStateble<T>
        where T : struct
    {
        //---------------------------------------------------------------------
        T State { get; }
        //---------------------------------------------------------------------
        bool CheckState(params T[] states);
        //---------------------------------------------------------------------
        event EventHandler<StateEventArgs<T>> StateChanging;
        event EventHandler<StateEventArgs<T>> StateChanged;
        //---------------------------------------------------------------------
    }
}