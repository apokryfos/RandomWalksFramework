using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace DataStructures
{
    public class HashCollection<T> : ICollection<T>, IList<T>
    {

        private int hashTableLowerBound;
        private int hashTableUpperBound;

        List<List<T>> hashTable;
        int _count;
  

        public HashCollection() : this(1000) { }
        public HashCollection(int capacity) 
        {
            int buckets = capacity / 10000;
            if (buckets <= 1)
                buckets = 10;
            if (buckets > 10000)
                buckets = 10000;

            hashTableLowerBound = Math.Max(10,buckets / 2);
            hashTableUpperBound = Math.Min(50000,buckets * 10);

            
            if (capacity < 100)            
                buckets = 1;

            _count = 0;
            hashTable = new List<List<T>>(capacity/buckets);
            while (hashTable.Count < buckets)
                hashTable.Add(new List<T>());

        }
        public HashCollection(IEnumerable<T> collection) : this(collection.Count(), collection) { }

        public HashCollection(int capacity, IEnumerable<T> collection) : this(capacity)
        {
            foreach (var v in collection)
                Add(v);            
        }




        #region ICollection<T> Members

        public void GrowHash()
        {
            int newSize = hashTable.Count * 2;
            while (hashTable.Count < newSize)
                hashTable.Add(new List<T>());

            for (int i = 0; i < hashTable.Count / 2; i++)
            {
                for (int j = hashTable[i].Count-1; j >= 0; j--)
                {
                    var item = hashTable[i][j];
                    if (GetBucket(item) != i)
                    {
                        hashTable[GetBucket(item)].Add(item);
                        hashTable[i].RemoveAt(j);
                    }
                }
            }
        }

        public void ShrinkHash()
        {
            int newSize = hashTable.Count / 2;
            if (newSize >= hashTableLowerBound)
            {
                for (int i = hashTable.Count - 1; i >= newSize; i--)
                {
                    hashTable[i % newSize].AddRange(hashTable[i]);
                    hashTable.RemoveAt(i);
                }
            }
        }

        private int GetBucket(T item)
        {
            return Math.Abs(item.GetHashCode()) % hashTable.Count;
        }

        public void Add(T item)
        {
            hashTable[GetBucket(item)].Add(item);
            _count++;

            if (_count >= hashTable.Count*500)
                GrowHash();
        }

        public void Clear()
        {
            foreach (var l in hashTable) 
                l.Clear();
            _count = 0;
        }

        public bool Contains(T item)
        {
            return hashTable[GetBucket(item)].Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int cindex = arrayIndex;
            for (int i = 0; i < hashTable.Count; i++)
            {
                foreach (var item in hashTable[i])
                    array[cindex++] = item;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (hashTable[GetBucket(item)].Remove(item))
            {
                _count--;
                if (_count <= 100*hashTable.Count)
                    ShrinkHash();
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        public class HashCollectionEnumerator : IEnumerator<T>
        { 
            private IEnumerator<List<T>> tableEnumerator;
            private IEnumerator<T> internalEnumerator;

            private HashCollection<T> activeCollection;

            private HashCollectionEnumerator() { }

            public HashCollectionEnumerator(HashCollection<T> c)
            {
                activeCollection = c;
                Reset();
            }

            #region IEnumerator<T> Members

            public T Current
            {
                get { return internalEnumerator.Current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {   
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return (object)Current; }
            }

            public bool MoveNext()
            {
                if (internalEnumerator == null)
                {
                    if (!tableEnumerator.MoveNext())
                        return false;
                    internalEnumerator = tableEnumerator.Current.GetEnumerator();
                }

                if (!internalEnumerator.MoveNext())
                {
                    internalEnumerator = null;
                    return MoveNext();
                }

                return true;
            }

            public void Reset()
            {
                tableEnumerator = (IEnumerator<List<T>>)activeCollection.hashTable.GetEnumerator();
                internalEnumerator = null;
            }

            #endregion
        }


        public IEnumerator<T> GetEnumerator()
        {
            return new HashCollectionEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IList<T> Members

        private int GetCountOfPreviousBuckets(int currentBucket)
        {
            int c=0;
            for (int i = 0; i < currentBucket; i++)
                c += hashTable[i].Count;
            return c;
        }


        public int IndexOf(T item)
        {   
            int b = GetBucket(item);
            int j = hashTable[b].IndexOf(item);
            if (j == -1)
                return -1;
            return GetCountOfPreviousBuckets(b) + j;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            int pending_index;
            int b = GetBucketAndIndex(index, out pending_index);
            hashTable[b].RemoveAt(pending_index);
            _count--;
          
        }

        private int GetBucketAndIndex(int index, out int bucketIndex)
        {
            if (index > _count)
                throw new IndexOutOfRangeException();
            int pending_index = index;
            for (int i = 0; i < hashTable.Count; i++)
            {
                if (pending_index >= hashTable[i].Count)
                    pending_index -= hashTable[i].Count;
                else
                {
                    bucketIndex = pending_index;
                    return i;
                }
            }
            throw new IndexOutOfRangeException();
        }

        public T this[int index]
        {
            get
            {
                int pending_index;
                int b = GetBucketAndIndex(index, out pending_index);
                return hashTable[b][pending_index];
            }
            set
            {
                Insert(index, value);
            }
        }

        #endregion
    }
}
