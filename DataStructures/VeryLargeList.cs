using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DataStructures {
	public class VeryLargeList<T> : IList<T>, ICollection<T>, IEnumerable<T> {
		private List<T[]> InternalLists;
		private List<int> Counts = new List<int>();
		private static int MaxListSize = 10000000;

		


		public VeryLargeList() : this(MaxListSize/2) { }

		public VeryLargeList(int capacity) {
			int int_list_count = (int)Math.Ceiling((double)capacity / (double)MaxListSize);
			InternalLists = new List<T[]>(int_list_count);
			for (int i = 0; i < int_list_count; i++) {
				InternalLists.Add(new T[MaxListSize]);
				Counts.Add(0);
			}
		}

		public VeryLargeList(int capacity, int maxListSize) {
			MaxListSize = maxListSize;
			int int_list_count = (int)Math.Ceiling((double)capacity / (double)MaxListSize);
			InternalLists = new List<T[]>(int_list_count);
			for (int i = 0; i < int_list_count; i++) {
				InternalLists.Add(new T[MaxListSize]);
				Counts.Add(0);
			}
		}

		public VeryLargeList(IEnumerable<T> other) {
			foreach (var v in other) {
				Add(v);
			}
		}




		public int IndexOf(T item) {
			for (int i = 0; i < InternalLists.Count; i++) {
				int ind = Array.IndexOf(InternalLists[i],item);
				if (ind >= 0) {
					return i * MaxListSize + ind;
				}
			}
			return -1;
		}

		public void Insert(int index, T item) {
			int lindex = index / MaxListSize;
			int cindex = index % MaxListSize;
			while (InternalLists.Count <= lindex) {
				InternalLists.Add(new T[MaxListSize]);	
			}
			T[] narray = new T[MaxListSize];
			Array.ConstrainedCopy(InternalLists[lindex], 0, narray, 0, cindex);
			narray[index] = item;
			Array.ConstrainedCopy(InternalLists[lindex], cindex+1, narray, cindex+1, cindex + 1 - MaxListSize);
			InternalLists[lindex] = narray;
			Counts[lindex]++;
			
			if (!InternalLists[lindex][MaxListSize - 1].Equals(default(T))) {
				Insert((lindex + 1) * MaxListSize, InternalLists[lindex][MaxListSize - 1]);
			}
		}

		public void RemoveAt(int index) {
			int lindex = index / MaxListSize;
			int cindex = index % MaxListSize;
			T[] narray = new T[MaxListSize];
			Array.ConstrainedCopy(InternalLists[lindex], 0, narray, 0, cindex);			
			Array.ConstrainedCopy(InternalLists[lindex], cindex, narray, cindex,MaxListSize - cindex);
			Counts[lindex]--;			
			InternalLists[lindex] = narray;
		}

		public T this[int index] {
			get {
				int lindex = index / MaxListSize;
				int cindex = index % MaxListSize;
				return InternalLists[lindex][cindex];				
			}
			set {
				int lindex = index / MaxListSize;
				int cindex = index % MaxListSize;
				InternalLists[lindex][cindex] = value;				
			}
		}

		public void Add(T item) {
			int i ;
			if (InternalLists.Count == 0) { InternalLists.Add(new T[MaxListSize]); Counts.Add(0); }

			for (i = 0; i < InternalLists.Count; i++) {
				if (Counts[i] < MaxListSize) { break; }
			}

			if (i == InternalLists.Count) {
				InternalLists.Add(new T[MaxListSize]);
				Counts.Add(0);
			}
			InternalLists[i][Counts[i]++] = item; 
		}

		public void AddRange(T[] items) {
			int i;
			if (InternalLists.Count == 0) { InternalLists.Add(new T[MaxListSize]); Counts.Add(0); }

			for (i = 0; i < InternalLists.Count; i++) {
				if (Counts[i] < MaxListSize) { break; }
			}

			if (i == InternalLists.Count) {
				InternalLists.Add(new T[MaxListSize]);
				Counts.Add(0);
			}
			if (items.Length + Counts[i] <= MaxListSize) {
				Array.ConstrainedCopy(items, 0, InternalLists[i], Counts[i], items.Length);
				Counts[i] += items.Length;
			} else {
				Array.ConstrainedCopy(items, 0, InternalLists[i], Counts[i], MaxListSize - Counts[i]);
				var nl = new T[MaxListSize - (items.Length + Counts[i])];
				Array.ConstrainedCopy(items, Counts[i] = MaxListSize, nl, 0, MaxListSize - (items.Length + Counts[i]));
				Counts[i] = MaxListSize;
				AddRange(nl);
			}
			
		}


		public void Clear() {
			InternalLists.Clear();
			Counts.Clear();
		}

		public bool Contains(T item) {
			return IndexOf(item) >= 0;
		}

		public void CopyTo(T[] array, int arrayIndex) {
			for (int i =0 ;i < InternalLists.Count;i++) {
				InternalLists[i].CopyTo(array, arrayIndex);
				arrayIndex += Counts[i];
			}
		}

		public int Count {
			get { return Counts.Sum(); }
		}

		public long LongCount {
			get { return Counts.Sum(c => (long)c); }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(T item) {
			int ind = IndexOf(item);
			if (ind >= 0) { RemoveAt(ind); return true; }
			return false;
		}

		public IEnumerator<T> GetEnumerator() {
			for (int j = 0;j < InternalLists.Count;j++) {			
				for (int i = 0;i < Counts[j];i++) {
					yield return InternalLists[j][i];
				}
			}
			yield break;

		}

		public void TrimExcess() {
			for (int i = Counts.Count-1; i >= 0; i--) {
				if (Counts[i] == 0) { InternalLists.RemoveAt(i); Counts.RemoveAt(i); }
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
