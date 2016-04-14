using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Tools.CSharp.Collections;

namespace Tools.CSharp.ViewModels
{
    public sealed class ViewModelCollectionChangedEventArgs<TItem> : EventArgs
        where TItem : class
    {
        #region private
        private readonly ImprovedNotifyCollectionChangedEventArgs<TItem> _originalEventArgs;
        #endregion
        internal ViewModelCollectionChangedEventArgs(ImprovedNotifyCollectionChangedEventArgs<TItem> eventArgs)
        {
            if (eventArgs == null)
            { throw new ArgumentNullException(nameof(eventArgs)); }

            _originalEventArgs = eventArgs;
        }

        //---------------------------------------------------------------------
        public NotifyCollectionChangedAction Action
        {
            get { return _originalEventArgs.Action; }
        }
        //---------------------------------------------------------------------
        public IList<TItem> NewItems
        {
            get { return _originalEventArgs.NewItems; }
        }
        public IList<TItem> OldItems
        {
            get { return _originalEventArgs.OldItems; }
        }
        //---------------------------------------------------------------------
        public int NewStartingIndex
        {
            get { return _originalEventArgs.NewStartingIndex; }
        }
        public int OldStartingIndex
        {
            get { return _originalEventArgs.OldStartingIndex; }
        }
        //---------------------------------------------------------------------
    }
}