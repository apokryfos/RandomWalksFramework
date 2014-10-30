using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace DataStructures
{

    public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey,TValue>>
    {
        
        void Insert(int index, TKey key, TValue value);
        void RemoveAt(int index);
        KeyValuePair<TKey,TValue> ElementAt(int index);
      
    }

    public class GenericOrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
    {
        private OrderedDictionary innerDictionary;

        public GenericOrderedDictionary()
        {
            innerDictionary = new OrderedDictionary();
        }
        public GenericOrderedDictionary(int capacity) { innerDictionary = new OrderedDictionary(capacity); }
        public GenericOrderedDictionary(IEqualityComparer comparer) { innerDictionary = new OrderedDictionary(comparer); }
        public GenericOrderedDictionary(int capacity, IEqualityComparer comparer) { innerDictionary = new OrderedDictionary(capacity, comparer); }




        #region IOrderedDictionary<TKey,TValue> Members


        public void Insert(int index, TKey key, TValue value)
        {
            innerDictionary.Insert(index, key, value);
        }

        public KeyValuePair<TKey, TValue> ElementAt(int index)
        {
            return new KeyValuePair<TKey,TValue>((TKey)(((DictionaryEntry)innerDictionary[index]).Key),(TValue)(((DictionaryEntry)innerDictionary[index]).Value));
        }

        public void Insert(int index, KeyValuePair<TKey, TValue> element)
        {
            Insert(index, element.Key, element.Value);
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            innerDictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return innerDictionary.Contains(key);
        }

        private class KeysValuesCollection<T> : ICollection<T>
        {
            private OrderedDictionary parent;
            private ICollection collection;

            public KeysValuesCollection(OrderedDictionary parent, bool keys)
            {
                this.parent = parent;
                if (keys)
                    collection = parent.Keys;
                else
                    collection = parent.Values;
            }


            #region ICollection<T> Members

            public void Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T item)
            {
                return parent.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                collection.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return collection.Count; }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return parent.Keys.Cast<T>().GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return parent.Keys.GetEnumerator();
            }

            #endregion
        }

        public ICollection<TKey> Keys
        {
            get { return new KeysValuesCollection<TKey>(innerDictionary, true); }
        }

        public bool Remove(TKey key)
        {
            try
            {
                int cnt = innerDictionary.Count;
                innerDictionary.Remove(key);
                return innerDictionary.Count < cnt;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = (TValue)(innerDictionary[(object)key]);
                return (value.Equals(default(TValue)));
            }
            catch (Exception)
            {
                value = default(TValue);
                return false;
            }
        }

        public ICollection<TValue> Values
        {
            get { return new KeysValuesCollection<TValue>(innerDictionary, false); }
        }

        public TValue this[TKey key]
        {
            get
            {
                return (TValue)(innerDictionary[(object)key]);
            }
            set
            {
                innerDictionary[(object)key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return (innerDictionary.Contains(item.Key) && ((TValue)(innerDictionary[(object)item.Key])).Equals(item.Value)); 
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < innerDictionary.Count; i++)
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>((TKey)(((DictionaryEntry)innerDictionary[i]).Key), (TValue)(((DictionaryEntry)innerDictionary[i]).Value));            
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
                return Remove(item);
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new GenericOrderedDictionaryEnumerator(innerDictionary);
        }
        private class GenericOrderedDictionaryEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {

            IDictionaryEnumerator enumerator;
            public GenericOrderedDictionaryEnumerator(OrderedDictionary parent)
            {
                enumerator = parent.GetEnumerator();
            }

            #region IEnumerator<KeyValuePair<TKey,TValue>> Members

            public KeyValuePair<TKey, TValue> Current
            {
                get { return new KeyValuePair<TKey,TValue>((TKey)enumerator.Key,(TValue)enumerator.Value); }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }

            #endregion
        }

        #endregion

        #region IOrderedDictionary<TKey,TValue> Members

        public void RemoveAt(int index)
        {
            innerDictionary.RemoveAt(index);
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members


        public void Clear()
        {
            innerDictionary.Clear();
        }

        public int Count
        {
            get { return innerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerDictionary.GetEnumerator();
        }

        #endregion
    }
    


}
