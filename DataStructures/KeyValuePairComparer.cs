using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{
	public enum KeyValuePairComparissonType { KEYS, VALUES, BOTH }

	public class KeyValuePairComparer<TKey, TValue> : Comparer<KeyValuePair<TKey, TValue>>, IEqualityComparer<KeyValuePair<TKey, TValue>> {

		public static KeyValuePairComparer<TKey, TValue> CreateKeysComparer() {
			return new KeyValuePairComparer<TKey, TValue>(KeyValuePairComparissonType.KEYS);
		}

		public static KeyValuePairComparer<TKey, TValue> CreateValuesComparer() {
			return new KeyValuePairComparer<TKey, TValue>(KeyValuePairComparissonType.VALUES);
		}


		private KeyValuePairComparissonType type;

		private IComparer<TKey> KeysComparer;
		private IComparer<TValue> ValuesComparer;
		

		public static KeyValuePairComparer<TKey, TValue> CreateDefaultComparer() {
			return new KeyValuePairComparer<TKey, TValue>();
		}

		public KeyValuePairComparer() : this(KeyValuePairComparissonType.BOTH) {			
		}


		public KeyValuePairComparer(KeyValuePairComparissonType type) {
			this.type = type;
			KeysComparer = Comparer<TKey>.Default;
			ValuesComparer = Comparer<TValue>.Default;

		}

		#region IComparer<KeyValuePair<TKey,TValue>> Members

		public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) {
			

			if (type == KeyValuePairComparissonType.KEYS)
				return KeysComparer.Compare(x.Key,y.Key);
			else if (type == KeyValuePairComparissonType.VALUES)
				return ValuesComparer.Compare(x.Value,y.Value);
			else
				return (KeysComparer.Compare(x.Key, y.Key) == 0 ? ValuesComparer.Compare(x.Value, y.Value) : KeysComparer.Compare(x.Key, y.Key));
		}

		#endregion

		#region IEqualityComparer<KeyValuePair<TKey,TValue>> Members

		public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) {
			return this.Compare(x, y) == 0;
		}

		public int GetHashCode(KeyValuePair<TKey, TValue> obj) {
			return (type == KeyValuePairComparissonType.KEYS ? obj.Key.GetHashCode() : (type == KeyValuePairComparissonType.VALUES ? obj.Value.GetHashCode() : (int)Math.Abs(obj.Key.GetHashCode() - obj.Value.GetHashCode())));
		}

		#endregion
	}


    
    
}
