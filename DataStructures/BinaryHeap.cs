﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace DataStructures {
	/// <summary>
	/// Binary heap
	/// </summary>
	/// <remarks>
	/// Indexing rules:
	/// 
	/// parent index: index ¡ 1)/2
	/// left child: 2 * index + 1
	/// right child: 2 * index + 2
	/// 
	/// Reference:
	/// http://dotnetslackers.com/Community/files/folders/data-structures-and-algorithms/entry28722.aspx
	/// </remarks>
	/// <typeparam name="TValue">type of the value</typeparam>
	/// <typeparam name="TPriority">type of the priority metric</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public class BinaryHeap<TPriority, TValue>
		: IEnumerable<KeyValuePair<TPriority, TValue>> {
		readonly Func<TPriority, TPriority, int> priorityComparsion;
		KeyValuePair<TPriority, TValue>[] items;
		int count;
		int version;

		const int DefaultCapacity = 16;

		public BinaryHeap()
			: this(DefaultCapacity, Comparer<TPriority>.Default.Compare) { }

		public BinaryHeap(Func<TPriority, TPriority, int> priorityComparison)
			: this(DefaultCapacity, priorityComparison) { }

		public BinaryHeap(int capacity, Func<TPriority, TPriority, int> priorityComparison) {
			this.items = new KeyValuePair<TPriority, TValue>[capacity];
			this.priorityComparsion = priorityComparison;
		}

		public Func<TPriority, TPriority, int> PriorityComparison {
			get { return this.priorityComparsion; }
		}

		public int Capacity {
			get { return this.items.Length; }
		}

		public int Count {
			get { return this.count; }
		}

		public void Add(TPriority priority, TValue value) {
			this.version++;
			this.ResizeArray();
			this.items[this.count++] = new KeyValuePair<TPriority, TValue>(priority, value);
			this.MinHeapifyDown(this.count - 1);
		}

		private void MinHeapifyDown(int start) {
			int i = start;
			int j = (i - 1) / 2;
			while (i > 0 &&
				this.Less(i, j)) {
				this.Swap(i, j);
				i = j;
				j = (i - 1) / 2;
			}
		}

		public TValue[] ToValueArray() {
			var values = new TValue[this.items.Length];
			for (int i = 0; i < values.Length; ++i)
				values[i] = this.items[i].Value;
			return values;
		}

		private void ResizeArray() {
			if (this.count == this.items.Length) {
				var newItems = new KeyValuePair<TPriority, TValue>[this.count * 2 + 1];
				Array.Copy(this.items, newItems, this.count);
				this.items = newItems;
			}
		}

		public KeyValuePair<TPriority, TValue> Minimum() {
			if (this.count == 0)
				throw new InvalidOperationException();
			return this.items[0];
		}

		public KeyValuePair<TPriority, TValue> RemoveMinimum() {
			// shortcut for heap with 1 element.
			if (this.count == 1) {
				this.version++;
				return this.items[--this.count];
			}
			return this.RemoveAt(0);
		}

		public KeyValuePair<TPriority, TValue> RemoveAt(int index) {
			if (this.count == 0)
				throw new InvalidOperationException("heap is empty");
			if (index < 0 | index >= this.count | index + this.count < this.count)
				throw new ArgumentOutOfRangeException("index");

			this.version++;
			// shortcut for heap with 1 element.
			if (this.count == 1)
				return this.items[--this.count];

			if (index < this.count - 1) {
				this.Swap(index, this.count - 1);
				this.MinHeapifyUp(index);
			}

			return this.items[--this.count];
		}

		private void MinHeapifyUp(int index) {
			var left = 2 * index + 1;
			var right = 2 * index + 2;
			while (
					(left < this.count - 1 && !this.Less(index, left)) ||
					(right < this.count - 1 && !this.Less(index, right))
				   ) {
				if (right >= this.count - 1 ||
					this.Less(left, right)) {
					this.Swap(left, index);
					index = left;
				} else {
					this.Swap(right, index);
					index = right;
				}
				left = 2 * index + 1;
				right = 2 * index + 2;
			}
		}

		public int IndexOf(TValue value) {
			for (int i = 0; i < this.count; i++) {
				if (object.Equals(value, this.items[i].Value))
					return i;
			}
			return -1;
		}

		public bool MinimumUpdate(TPriority priority, TValue value) {
			// find index
			for (int i = 0; i < this.count; i++) {
				if (object.Equals(value, this.items[i].Value)) {
					if (this.priorityComparsion(priority, this.items[i].Key) <= 0) {
						this.RemoveAt(i);
						this.Add(priority, value);
						return true;
					}
					return false;
				}
			}

			// not in collection
			this.Add(priority, value);
			return true;
		}

		public void Update(TPriority priority, TValue value) {
			// find index
			var index = this.IndexOf(value);
			// remove if needed
			if (index > -1)
				this.RemoveAt(index);
			this.Add(priority, value);
		}

		[Pure]
		private bool Less(int i, int j) {
			return this.priorityComparsion(this.items[i].Key, this.items[j].Key) <= 0;
		}

		private void Swap(int i, int j) {

			var kv = this.items[i];
			this.items[i] = this.items[j];
			this.items[j] = kv;
		}



		#region IEnumerable<KeyValuePair<TKey,TValue>> Members
		public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator() {
			foreach (var kv in items) {
				yield return kv;
			}
			yield break;
		}
		#endregion


		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
	}
}
