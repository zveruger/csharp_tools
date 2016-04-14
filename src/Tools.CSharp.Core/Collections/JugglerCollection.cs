using System.Collections;
using System.Collections.Generic;

namespace Tools.CSharp.Collections
{
    public class JugglerCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region private
        private readonly Dictionary<TKey, TValue> _keyCollection;
        private readonly Dictionary<TValue, TKey> _valueCollection; 
        #endregion
        public JugglerCollection()
            :this(0)
        {
            
        }
        public JugglerCollection(int capacity)
        {
            _keyCollection = new Dictionary<TKey, TValue>(capacity);
            _valueCollection = new Dictionary<TValue, TKey>(capacity);
        }
        public JugglerCollection(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _keyCollection = new Dictionary<TKey, TValue>(keyComparer);
            _valueCollection = new Dictionary<TValue, TKey>(valueComparer);
        }
        public JugglerCollection(int capacity, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _keyCollection = new Dictionary<TKey, TValue>(capacity, keyComparer);
            _valueCollection = new Dictionary<TValue, TKey>(capacity, valueComparer);
        }

        //---------------------------------------------------------------------
        public TValue this[TKey key]
        {
            get { return _keyCollection[key]; }
        }
        public TKey this[TValue key]
        {
            get { return _valueCollection[key]; }
        }
        public int Count
        {
            get { return _keyCollection.Count; }
        }
        //---------------------------------------------------------------------
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _keyCollection.TryGetValue(key, out value);
        }
        public bool TryGetValue(TValue key, out TKey value)
        {
            return _valueCollection.TryGetValue(key, out value);
        }
        //---------------------------------------------------------------------
        public bool ContainsKey(TKey key)
        {
            return _keyCollection.ContainsKey(key);
        }
        public bool ContainsKey(TValue key)
        {
            return _valueCollection.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _keyCollection.ContainsValue(value);
        }
        public bool ContainsValue(TKey value)
        {
            return _valueCollection.ContainsValue(value);
        }
        //---------------------------------------------------------------------
        public void Add(TKey key, TValue value)
        {
            _keyCollection.Add(key, value);
            _valueCollection.Add(value, key);
        }
        public void Remove(TKey key)
        {
            var value = _keyCollection[key];

            _keyCollection.Remove(key);
            _valueCollection.Remove(value);
        }
        public void Clear()
        {
            _keyCollection.Clear();
            _valueCollection.Clear();
        }
        //---------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_keyCollection).GetEnumerator(); 
        }
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _keyCollection.GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
}