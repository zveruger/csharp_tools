using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Security;

namespace Tools.CSharp.Collections
{
    public class MutationDictionaryAndListCollection<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, ISerializable, IDeserializationCallback
    {
        #region private
        private readonly List<TValue> _collection;
        private readonly ReadOnlyCollection<TValue> _readOnlyCollection;
        private readonly Dictionary<TKey, TValue> _dictionary;
        #endregion
        public MutationDictionaryAndListCollection()
            : this(0)
        {
        }
        public MutationDictionaryAndListCollection(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
            _collection = new List<TValue>(capacity);
            _readOnlyCollection = new ReadOnlyCollection<TValue>(_collection);
        }
        public MutationDictionaryAndListCollection(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }
        public MutationDictionaryAndListCollection(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
            _collection = new List<TValue>(capacity);
            _readOnlyCollection = new ReadOnlyCollection<TValue>(_collection);
        }

        //---------------------------------------------------------------------
        public TValue this[int index]
        {
            get { return _collection[index]; }
        }
        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
            set
            {
                var oldValue = _dictionary[key];
                _dictionary[key] = value;

                _collection.Remove(oldValue);
                _collection.Add(value);
            }
        }
        public int Count
        {
            get { return _dictionary.Count; }
        }
        //---------------------------------------------------------------------
        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }
        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }
        public ReadOnlyCollection<TValue> Collection
        {
            get { return _readOnlyCollection; }
        }
        public IEqualityComparer<TKey> Comparer
        {
            get { return _dictionary.Comparer; }
        }
        //---------------------------------------------------------------------
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _collection.Add(value);
        }
        public bool Remove(TKey key)
        {
            TValue value;

            if (_dictionary.TryGetValue(key, out value))
            { _collection.Remove(value); }

            return _dictionary.Remove(key);
        }
        public void Clear()
        {
            _dictionary.Clear();
            _collection.Clear();
        }
        //---------------------------------------------------------------------
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }
        public bool ContainsValue(TValue value)
        {
            return _dictionary.ContainsValue(value);
        }
        //---------------------------------------------------------------------
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _dictionary.GetObjectData(info, context);
        }
        public virtual void OnDeserialization(Object sender)
        {
            _dictionary.OnDeserialization(sender);
        }
        //---------------------------------------------------------------------
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly; }
        }
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            var result = ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(keyValuePair);
            _collection.Remove(keyValuePair.Value);

            return result;
        }
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(keyValuePair);
        }
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_dictionary).IsSynchronized; }
        }
        object ICollection.SyncRoot
        {
            get { return ((ICollection)_dictionary).SyncRoot; }
        }
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_dictionary).CopyTo(array, index);
        }
        //---------------------------------------------------------------------
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return ((IDictionary<TKey, TValue>)_dictionary).Values; }
        }
        object IDictionary.this[object key]
        {
            get { return ((IDictionary)_dictionary)[key]; }
            set
            {
                var dictionary = (IDictionary)_dictionary;

                var oldValue = dictionary[key];
                dictionary[key] = value;

                _collection.Remove((TValue)oldValue);
            }
        }
        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)_dictionary).IsFixedSize; }
        }
        bool IDictionary.IsReadOnly
        {
            get { return ((IDictionary)_dictionary).IsReadOnly; }
        }
        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)_dictionary).Keys; }
        }
        ICollection IDictionary.Values
        {
            get { return ((IDictionary)_dictionary).Values; }
        }
        void IDictionary.Add(object key, object value)
        {
            ((IDictionary)_dictionary).Add(key, value);
            _collection.Add((TValue)value);
        }
        void IDictionary.Remove(object key)
        {
            var dictionary = (IDictionary)_dictionary;

            try
            { _collection.Remove((TValue)dictionary[key]); }
            catch { }

            dictionary.Remove(key);
        }
        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_dictionary).Contains(key);
        }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_dictionary).GetEnumerator();
        }
        //---------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary).GetEnumerator();
        }
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
}
