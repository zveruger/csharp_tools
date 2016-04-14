using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tools.CSharp.Collections
{
    public sealed class SortedObservableItemCollection<TViewModel> : SortedItemCollection<TViewModel, ObservableCollection<TViewModel>>
    {
        #region private
        private void _subscribeOriginalCollectionAllEvents(ObservableCollection<TViewModel> collection, bool addEvents)
        {
            if (collection != null)
            {
                if (addEvents)
                { collection.CollectionChanged += _originalCollectionOnCollectionChanged; }
                else
                { collection.CollectionChanged -= _originalCollectionOnCollectionChanged; }
            }
        }
        private void _originalCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var isRaiseCountChange = OriginalCollectionAvailable;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    { Add(e.NewStartingIndex, isRaiseCountChange); }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    { Remove((TViewModel)e.OldItems[0], isRaiseCountChange); }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    { Reset(); }
                    break;
            }
        }
        #endregion
        #region protected
        protected override void UpdatingOriginalCollection(ObservableCollection<TViewModel> collection)
        {
            base.UpdatingOriginalCollection(collection);

            _subscribeOriginalCollectionAllEvents(collection, false);
        }
        protected override void UpdatedOriginalCollection(ObservableCollection<TViewModel> collection)
        {
            base.UpdatedOriginalCollection(collection);

            _subscribeOriginalCollectionAllEvents(collection, true);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                { _subscribeOriginalCollectionAllEvents(OriginalCollection, false); }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
    }
}