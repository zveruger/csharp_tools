using System.ComponentModel;

namespace Tools.CSharp
{
    public sealed class ListChangedEventArgs<T> : ListChangedEventArgs
    {
        #region private
        private readonly T _newItem;
        private readonly T _oldItem;
        #endregion
        public ListChangedEventArgs(ListChangedType listChangedType, T newItem, int newIndex) 
            : base(listChangedType, newIndex)
        {
            _newItem = newItem;
        }
        public ListChangedEventArgs(ListChangedType listChangedType, T item, int newIndex, PropertyDescriptor propDesc) 
            : base(listChangedType, newIndex, propDesc)
        {
            _newItem = item;
            _oldItem = item;
        }
        public ListChangedEventArgs(ListChangedType listChangedType, PropertyDescriptor propDesc)
            : base(listChangedType, propDesc)
        { }
        public ListChangedEventArgs(ListChangedType listChangedType, T newItem, int newIndex, T oldItem, int oldIndex) 
            : base(listChangedType, newIndex, oldIndex)
        {
            _newItem = newItem;
            _oldItem = oldItem;
        }

        //---------------------------------------------------------------------
        public T NewItem
        {
            get { return _newItem; }
        }
        public T OldItem
        {
            get { return _oldItem; }
        }
        //---------------------------------------------------------------------
    }
}