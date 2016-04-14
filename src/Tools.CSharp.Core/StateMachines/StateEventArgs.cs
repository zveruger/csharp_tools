using System;

namespace Tools.CSharp.StateMachines
{
    public sealed class StateEventArgs<T> : EventArgs
        where T : struct
    {
        #region private
        private readonly T _newState;
        private readonly T _oldState;
        #endregion
        public StateEventArgs(T oldState, T newState)
        {
            _oldState = oldState;
            _newState = newState;
        }

        //---------------------------------------------------------------------
        public T NewState
        {
            get { return _newState; }
        }
        public T OldState
        {
            get { return _oldState; }
        }
        //---------------------------------------------------------------------
    }
}