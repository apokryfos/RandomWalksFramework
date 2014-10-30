using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures {
	public class EqualityComparerFromDelegate<T> : EqualityComparer<T> {
		
		Func<T,T,bool> equalityComparer;
		Func<T,int> hashCodeProvider;
		
		protected EqualityComparerFromDelegate(Func<T,T,bool> equalityComparer, Func<T,int> hashCodeProvider) {
			this.equalityComparer = equalityComparer;
			this.hashCodeProvider=hashCodeProvider;
		}

		public static EqualityComparerFromDelegate<T> Create(Func<T,T,bool> equalityComparer, Func<T,int> hashCodeProvider) {
			return new EqualityComparerFromDelegate<T>(equalityComparer, hashCodeProvider);
		}

		public override bool Equals(T x, T y) {
			if (Object.Equals(x, null) && Object.Equals(y, null)) { return true; } else if (Object.Equals(x, null) || Object.Equals(y, null)) { return false; } else {
				return equalityComparer(x, y);
			}

		}


		public override int GetHashCode(T obj) {
			return hashCodeProvider(obj);
		}
	}
}
