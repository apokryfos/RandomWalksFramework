using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures {
	class FibonacciHeap<TPriority,TValue> : BinaryHeap<TPriority, TValue>, IEnumerable<KeyValuePair<TPriority, TValue>> {

	}
}
