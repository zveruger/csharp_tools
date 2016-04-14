using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Tools.CSharp.Collections
{
    public sealed class ImprovedNotifyCollectionChangedEventArgs<T> : EventArgs
        where T : class
    {
        #region private
        private NotifyCollectionChangedAction _action;
        private IList<T> _newItems;
        private IList<T> _oldItems;
        private int _newStartingIndex = -1;
        private int _oldStartingIndex = -1;
        //---------------------------------------------------------------------
        private void _initializeAddOrRemove(NotifyCollectionChangedAction action, IList<T> changedItems, int startingIndex)
        {
            if (action == NotifyCollectionChangedAction.Add)
            { _initializeAdd(action, changedItems, startingIndex); }
            else if (action == NotifyCollectionChangedAction.Remove)
            { _initializeRemove(action, changedItems, startingIndex); }
            else
            {  Contract.Assert(false, $"Unsupported action: { action.ToString() }"); }
        }
        private void _initializeAdd(NotifyCollectionChangedAction action, IList<T> newItems, int newStartingIndex)
        {
            _action = action;
            _newItems = (newItems == null) ? null : new ReadOnlyCollection<T>(newItems);
            _newStartingIndex = newStartingIndex;
        }
        private void _initializeRemove(NotifyCollectionChangedAction action, IList<T> oldItems, int oldStartingIndex)
        {
            _action = action;
            _oldItems = (oldItems == null) ? null : new ReadOnlyCollection<T>(oldItems);
            _oldStartingIndex = oldStartingIndex;
        }
        private void _initializeMoveOrReplace(NotifyCollectionChangedAction action, IList<T> newItems, IList<T> oldItems, int startingIndex, int oldStartingIndex)
        {
            _initializeAdd(action, newItems, startingIndex);
            _initializeRemove(action, oldItems, oldStartingIndex);
        }
        #endregion
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            if (action != NotifyCollectionChangedAction.Reset)
            { throw new ArgumentException(string.Empty, nameof(action)); }

            _initializeAdd(action, null, -1);
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem)
        {
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove) && (action != NotifyCollectionChangedAction.Reset))
            { throw new ArgumentException(string.Empty, nameof(action)); }

            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                { throw new ArgumentException(string.Empty, nameof(changedItem)); }

                _initializeAdd(action, null, -1);
            }
            else
            {
                _initializeAddOrRemove(action, new List<T> { changedItem }, -1);
            }
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem, int index)
        {
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove) && (action != NotifyCollectionChangedAction.Reset))
            { throw new ArgumentException(string.Empty, nameof(action)); }

            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                { throw new ArgumentException(string.Empty, nameof(changedItem)); }
                if (index != -1)
                { throw new ArgumentException(string.Empty, nameof(index)); }

                _initializeAdd(action, null, -1);
            }
            else
            {
                _initializeAddOrRemove(action, new List<T> { changedItem }, index);
            }
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems)
        {
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove) && (action != NotifyCollectionChangedAction.Reset))
            { throw new ArgumentException(string.Empty, nameof(action)); }

            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                { throw new ArgumentException(string.Empty, nameof(changedItems)); }

                _initializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                { throw new ArgumentNullException(nameof(changedItems)); }

                _initializeAddOrRemove(action, changedItems, -1);
            }
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems, int startingIndex)
        {
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove) && (action != NotifyCollectionChangedAction.Reset))
            { throw new ArgumentException(string.Empty, nameof(action)); }

            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                { throw new ArgumentException(string.Empty, nameof(changedItems)); }
                if (startingIndex != -1)
                { throw new ArgumentException(string.Empty, nameof(startingIndex)); }

                _initializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                { throw new ArgumentNullException(nameof(changedItems)); }
                if (startingIndex < -1)
                { throw new ArgumentException(string.Empty, nameof(startingIndex)); }

                _initializeAddOrRemove(action, changedItems, startingIndex);
            }
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T newItem, T oldItem)
        {
            if (action != NotifyCollectionChangedAction.Replace)
            { throw new ArgumentException(string.Empty, nameof(action)); }

            _initializeMoveOrReplace(action, new List<T> { newItem }, new List<T> { oldItem }, -1, -1);
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T newItem, T oldItem, int index)
        {
            if (action != NotifyCollectionChangedAction.Replace)
            { throw new ArgumentException(string.Empty, nameof(action)); }

            _initializeMoveOrReplace(action, new List<T> { newItem }, new List<T> { oldItem }, index, index);
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> newItems, IList<T> oldItems)
        {
            if (action != NotifyCollectionChangedAction.Replace)
            { throw new ArgumentException(string.Empty, nameof(action)); }
            if (newItems == null)
            { throw new ArgumentNullException(nameof(newItems)); }
            if (oldItems == null)
            { throw new ArgumentNullException(nameof(oldItems)); }

            _initializeMoveOrReplace(action, newItems, oldItems, -1, -1);
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> newItems, IList<T> oldItems, int startingIndex)
        {
            if (action != NotifyCollectionChangedAction.Replace)
            { throw new ArgumentException(string.Empty, nameof(action)); }
            if (newItems == null)
            { throw new ArgumentNullException(nameof(newItems)); }
            if (oldItems == null)
            { throw new ArgumentNullException(nameof(oldItems)); }

            _initializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem, int index, int oldIndex)
        {
            if (action != NotifyCollectionChangedAction.Move)
            { throw new ArgumentException(string.Empty, nameof(action)); }
            if (index < 0)
            { throw new ArgumentException(string.Empty, nameof(index)); }

            var changedItems = new List<T> { changedItem };
            _initializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        }
        public ImprovedNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems, int index, int oldIndex)
        {
            if (action != NotifyCollectionChangedAction.Move)
            { throw new ArgumentException(string.Empty, nameof(action)); }
            if (index < 0)
            { throw new ArgumentException(string.Empty, nameof(index)); }

            _initializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        }

        //---------------------------------------------------------------------
        public NotifyCollectionChangedAction Action
        {
            get { return _action; }
        }
        //---------------------------------------------------------------------
        public IList<T> NewItems
        {
            get { return _newItems; }
        }
        public IList<T> OldItems
        {
            get { return _oldItems; }
        }
        //---------------------------------------------------------------------
        public int NewStartingIndex
        {
            get { return _newStartingIndex; }
        }
        public int OldStartingIndex
        {
            get { return _oldStartingIndex; }
        }
        //---------------------------------------------------------------------
    }
}