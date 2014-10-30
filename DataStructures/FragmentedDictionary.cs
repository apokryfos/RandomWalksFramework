using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DataStructures {

	public class FragmentedDictionary<TKey, TValue> : GenericFragmentedDictionary<TKey, TValue, Dictionary<TKey, TValue>> {
		#region Constructors
		public FragmentedDictionary() : base() { }

		public FragmentedDictionary(int fragments, IDictionary<TKey, TValue> dictionary) : base(fragments, dictionary) { }

		public FragmentedDictionary(int fragments, int capacity) : base(fragments, capacity) { }

		public FragmentedDictionary(IDictionary<TKey, TValue> dictionary)
			: this(5, dictionary) {
		}

		public FragmentedDictionary(int fragments) : base(fragments) { }


		#endregion
	}


	public class GenericFragmentedDictionary<TKey, TValue, TDic> : IDictionary<TKey, TValue> {


	
		private class DictionaryKeysCollectionProxy : ICollection<TKey> {
			private GenericFragmentedDictionary<TKey, TValue, TDic> proxy;

			public DictionaryKeysCollectionProxy(GenericFragmentedDictionary<TKey, TValue, TDic> proxied) {
				this.proxy = proxied;
			}

			#region ICollection<TKey> Members

			public void Add(TKey item) {
				throw new NotSupportedException();
			}

			public void Clear() {
				throw new NotSupportedException();
			}

			public bool Contains(TKey item) {
				return proxy.ContainsKey(item);
			}

			public void CopyTo(TKey[] array, int arrayIndex) {
				int cindex = arrayIndex;
				foreach (var kv in proxy)
					array[cindex++] = kv.Key;
			}

			public int Count {
				get { return proxy.Count; }
			}

			public bool IsReadOnly {
				get { return true; }
			}

			public bool Remove(TKey item) {
				throw new NotSupportedException();
			}

			#endregion

			#region IEnumerable<TKey> Members

			public IEnumerator<TKey> GetEnumerator() {
				foreach (var kv in proxy) {
					yield return kv.Key;
				}
				yield break;
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator() {
				throw new NotImplementedException();
			}

			#endregion
		}

		private class DictionaryValuesCollectionProxy : ICollection<TValue> {
			private GenericFragmentedDictionary<TKey, TValue, TDic> proxy;

			public DictionaryValuesCollectionProxy(GenericFragmentedDictionary<TKey, TValue, TDic> proxied) {
				this.proxy = proxied;
			}

			#region ICollection<TValue> Members

			public void Add(TValue item) {
				throw new NotSupportedException();
			}

			public void Clear() {
				throw new NotSupportedException();
			}

			public bool Contains(TValue item) {
				return !(proxy.FirstOrDefault<KeyValuePair<TKey, TValue>>(kv => kv.Value.Equals(item)).Equals(default(KeyValuePair<TKey, TValue>)));
			}

			public void CopyTo(TValue[] array, int arrayIndex) {
				int cindex = arrayIndex;
				foreach (var kv in proxy)
					array[cindex++] = kv.Value;
			}

			public int Count {
				get { return proxy.Count; }
			}

			public bool IsReadOnly {
				get { return true; }
			}

			public bool Remove(TValue item) {
				throw new NotSupportedException();
			}

			#endregion

			#region IEnumerable<TValue> Members

			public IEnumerator<TValue> GetEnumerator() {
				foreach (var kv in proxy) {
					yield return kv.Value;
				}
				yield break;
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator() {
				throw new NotImplementedException();
			}

			#endregion
		}

		protected List<IDictionary<TKey, TValue>> _fragments;


		#region Constructors
		public GenericFragmentedDictionary()
			: this(15) {
		}

		public GenericFragmentedDictionary(int fragments, IDictionary<TKey, TValue> dictionary)
			: this(fragments) {
			foreach (var v in dictionary)
				Add(v.Key, v.Value);
		}

		public GenericFragmentedDictionary(int fragments, int capacity) {
			if (fragments <= 0 || capacity <= 0) { throw new ArgumentException(); }
			int c = capacity / fragments;
			_fragments = new List<IDictionary<TKey, TValue>>(fragments);
			for (int i = 0; i < fragments; i++) {
				_fragments.Add(DictionaryProvider(c));				
			}
		}

		public GenericFragmentedDictionary(IDictionary<TKey, TValue> dictionary)
			: this(5, dictionary) {
		}

		public GenericFragmentedDictionary(int fragments) {
			if (fragments <= 0) { throw new ArgumentException(); }
			IsConcurent = false;
			_fragments = new List<IDictionary<TKey, TValue>>(fragments);			
			for (int i = 0; i < fragments; i++)
				_fragments.Add(DictionaryProvider());
		}

		#endregion

		private IDictionary<TKey, TValue> DictionaryProvider(int Capacity) {
			if (typeof(IDictionary<TKey, TValue>).IsAssignableFrom(typeof(TDic))) {
				return (IDictionary<TKey, TValue>)Activator.CreateInstance(typeof(TDic),Capacity);
			}
			throw new InvalidCastException();
		}

		private IDictionary<TKey, TValue> DictionaryProvider() {
			if (typeof(IDictionary<TKey,TValue>).IsAssignableFrom(typeof(TDic))) {
				return (IDictionary<TKey,TValue>)Activator.CreateInstance(typeof(TDic));
			}
			throw new InvalidCastException();

		}

		public bool IsConcurent { get; private set; }
		//private object SyncRoot = new object();

		#region IDictionary<TKey,TValue> Members

		protected virtual IDictionary<TKey, TValue> Source(TKey key) {
			int a;
			if (object.ReferenceEquals(key, null)) {
				a = 0;
			} else {
				a = key.GetHashCode();
			}
			var b = _fragments.Count;
			return _fragments[((a % b) + b) % b];			
		}

		public virtual void Add(TKey key, TValue value) {
			Source(key).Add(key, value);
		}

		public virtual bool ContainsKey(TKey key) {
			return Source(key).ContainsKey(key);
		}

		public ICollection<TKey> Keys {
			get {
				return new GenericFragmentedDictionary<TKey, TValue, TDic>.DictionaryKeysCollectionProxy(this);
			}
		}

		public bool Remove(TKey key) {
			return Source(key).Remove(key);
		}

		public virtual bool TryGetValue(TKey key, out TValue value) {
			try {
				return Source(key).TryGetValue(key, out value);
			} catch (Exception) {
				value = default(TValue);
				return false;
			}
		}

		public ICollection<TValue> Values {
			get {
				return new GenericFragmentedDictionary<TKey, TValue, TDic>.DictionaryValuesCollectionProxy(this);
			}
		}

		public virtual TValue this[TKey key] {
			get {
				return Source(key)[key];
			}
			set {
				Source(key)[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}

		public void Clear() {
			foreach (var d in _fragments)
				d.Clear();
			
		}

		public bool Contains(KeyValuePair<TKey, TValue> item) {			
			return (ContainsKey(item.Key) && this[item.Key].Equals(item.Value));			
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			int index = arrayIndex;

			foreach (var v in this) {
				array[index++] = new KeyValuePair<TKey, TValue>(v.Key, v.Value);
			}

			

		}

		public int Count {
			get {
				int cnt = 0;
				foreach (var d in _fragments)
					cnt += d.Count;				
				return cnt;
			}
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item) {
			if (Contains(item))
				return Remove(item.Key);
			return false;
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			var enumerators = _fragments.Select(f => f.GetEnumerator()).ToArray();
			bool finished = false;
			while (!finished) {
				finished = true;
				for (int i = 0; i < _fragments.Count; i++) {					
					if (enumerators[i].MoveNext()) {
						finished = false;
						yield return enumerators[i].Current;
					}
				}
			}
			yield break;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return (IEnumerator)GetEnumerator();
		}

		#endregion

	}	
}
