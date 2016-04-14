using System.Collections.Specialized;
using Tools.CSharp.ViewModels;

namespace Tools.CSharp.Collections
{
    public class SortedViewModelItemCollection<TViewModel> : SortedItemCollection<TViewModel, BaseViewModelCollection<TViewModel>>
        where TViewModel : class
    {
        #region private
        private void _subscribeOriginalCollectionAllEvents(BaseViewModelCollection<TViewModel> collection, bool addEvents)
        {
            if (collection != null)
            {
                if (addEvents)
                { collection.CollectionChanged += _originalCollectionOnCollectionChanged; }
                else
                { collection.CollectionChanged -= _originalCollectionOnCollectionChanged; }
            }
        }
        private void _originalCollectionOnCollectionChanged(object sender, ViewModelCollectionChangedEventArgs<TViewModel> e)
        {
            var isRaiseCountChange = OriginalCollectionAvailable;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    { Add(e.NewStartingIndex, isRaiseCountChange); }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    { Remove(e.OldItems[0], isRaiseCountChange); }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    { Reset(); }
                    break;
            }
        }
        #endregion
        #region protected
        protected override void UpdatingOriginalCollection(BaseViewModelCollection<TViewModel> collection)
        {
            base.UpdatingOriginalCollection(collection);

            _subscribeOriginalCollectionAllEvents(collection, false);
        }
        protected override void UpdatedOriginalCollection(BaseViewModelCollection<TViewModel> collection)
        {
            base.UpdatedOriginalCollection(collection);

            _subscribeOriginalCollectionAllEvents(collection, true);
        }
        //---------------------------------------------------------------------
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