using System;

namespace Tools.CSharp.StateMachines
{
    public interface IStateMachine<T>
        where T : struct
    {
        //---------------------------------------------------------------------
        T State { get; set; }
        //---------------------------------------------------------------------
        bool CheckState(params T[] states);
        //---------------------------------------------------------------------
        event EventHandler<StateEventArgs<T>> StateChanging;
        event EventHandler<StateEventArgs<T>> StateChanged;
        //---------------------------------------------------------------------
    }
}