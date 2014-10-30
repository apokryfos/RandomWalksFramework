using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;

namespace GraphFramework.Containers {
	public class HashSetEdgeList<TVertex> : IEdgeList<TVertex> {

		List<TVertex> InternalList = new List<TVertex>();
		HashSet<TVertex> InternalSet = new HashSet<TVertex>();

		public HashSetEdgeList()
			: base() {
		}
		public HashSetEdgeList(IEnumerable<TVertex> initial)  { }
		

		public int IndexOf(TVertex item) {
			return InternalList.IndexOf(item);
		}

		public void Insert(int index, TVertex item) {
			if (InternalSet.Add(item)) {
				InternalList.Insert(index, item);
			}
		}

		public void RemoveAt(int index) {
			InternalSet.Remove(InternalList[index]);
			InternalList.RemoveAt(index);
		}

		public TVertex this[int index] {
			get {
				return InternalList[index];
			}
			set {
				if (InternalSet.Add(value)) {
					InternalSet.Remove(InternalList[index]);
					InternalList[index] = value;
				}
			}
		}

		public void TrimExcess() {
			InternalList.TrimExcess();
			InternalSet.TrimExcess();
		}

		public void Add(TVertex item) {
			if (InternalSet.Add(item)) {
				InternalList.Add(item);
			}
		}

		public void Clear() {
			InternalList.Clear();
			InternalSet.Clear();
		}

		public bool Contains(TVertex item) {
			return InternalSet.Contains(item);
		}

		public void CopyTo(TVertex[] array, int arrayIndex) {
			InternalList.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return InternalList.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(TVertex item) {
			if (InternalSet.Remove(item)) {
				return InternalList.Remove(item);
			}
			return false;
		}

		public IEnumerator<TVertex> GetEnumerator() {
			return InternalList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return InternalList.GetEnumerator();
		}
	}
}
