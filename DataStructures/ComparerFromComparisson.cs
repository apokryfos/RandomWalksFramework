using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DataStructures {

	public class ComparerFromComparison<T> : Comparer<T> {
		private Comparison<T> f;
		
		
		public static Comparer<T> Create(Comparison<T> cmp) {
			return new ComparerFromComparison<T>(cmp);
		}

		private ComparerFromComparison(Comparison<T> f) {			
			this.f = f;
		}

		#region IComparer<string> Members

		public override int Compare(T x, T y) {
			return f(x, y);
		}

		#endregion
	}
}
