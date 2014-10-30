using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;
using DataStructures;

namespace GraphFramework.Containers {
	public class AdjacencyList<TVertex> : GenericFragmentedDictionary<TVertex, IEdgeList<TVertex>, Dictionary<TVertex,IEdgeList<TVertex>>>, IAdjacencyList<TVertex> {

		public AdjacencyList() : base() { }
		public AdjacencyList(int fragments) : base(fragments) { }
		
		public new System.Collections.IEnumerator GetEnumerator() {
			throw new NotImplementedException();
		}
	}
}
