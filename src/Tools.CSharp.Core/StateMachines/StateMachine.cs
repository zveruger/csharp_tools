using System;
using System.ComponentModel;
using System.Linq;

namespace Tools.CSharp.StateMachines
{
    public sealed class StateMachine<T> : IStateMachine<T>, INotifyPropertyChanged
         where T : struct
    {
        #region private
        private T _state;
        //---------------------------------------------------------------------
        private void _onStateChanging(T oldState, T newState)
        {
            StateChanging?.Invoke(this, new StateEventArgs<T>(oldState, newState));
        }
        private void _onStateChanged(T oldState, T newState)
        {
            StateChanged?.Invoke(this, new StateEventArgs<T>(oldState, newState));
        }
        //---------------------------------------------------------------------
        private void _onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public StateMachine(T initState)
        {
            _state = initState;
        }

        //---------------------------------------------------------------------
        public T State
        {
            get { return _state; }
            set
            {
                if (!_state.Equals(value))
                {
                    var oldState = _state;
                    var newState = value;

                    _onStateChanging(oldState, newState);
                    _state = value;
                    _onStateChanged(oldState, newState);

                    _onPropertyChanged(nameof(State));
                }
            }
        }
        //---------------------------------------------------------------------
        public bool CheckState(params T[] states)
        {
            if (states == null)
            { throw new ArgumentNullException(nameof(states)); }

            return states.Contains(_state);
        }
        //---------------------------------------------------------------------
        public event EventHandler<StateEventArgs<T>> StateChanging;
        public event EventHandler<StateEventArgs<T>> StateChanged;
        //---------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        //---------------------------------------------------------------------

    }
}