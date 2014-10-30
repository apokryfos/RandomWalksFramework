using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;

namespace GraphFramework.Containers {
	public class ListEdgeList<TVertex> : List<TVertex>, IEdgeList<TVertex> {
		public ListEdgeList()
			: base() {
		}

		public ListEdgeList(int capacity)
			: base(capacity) {
		}

		public ListEdgeList(IEnumerable<TVertex> edges)
			: base(edges) {
		}

	}
}
