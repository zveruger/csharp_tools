using System;
using System.Collections;
using System.Collections.Generic;

namespace Tools.CSharp.Collections
{
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region private
        private readonly IDictionary<TKey, TValue> _dictionary;
        #endregion
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            { throw new ArgumentNullException(nameof(dictionary)); }

            _dictionary = dictionary;
        }

        //---------------------------------------------------------------------
        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
            set { throw new NotSupportedException(); }
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
        //---------------------------------------------------------------------
        public bool IsReadOnly
        {
            get { return true; }
        }
        //---------------------------------------------------------------------
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
        //---------------------------------------------------------------------
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }
        //---------------------------------------------------------------------
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }
        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }
        public bool Remove(TKey key)
        {
            throw new NotSupportedException();
        }
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Remove(item);
        }
        public void Clear()
        {
            throw new NotSupportedException();
        }
        //---------------------------------------------------------------------
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }
        //---------------------------------------------------------------------
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
}
