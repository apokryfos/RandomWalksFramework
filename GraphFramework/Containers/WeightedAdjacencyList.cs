using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;
using DataStructures;

namespace GraphFramework.Containers {
	public class WeightedAdjacencyList<TVertex, TWeight> : FragmentedDictionary<TVertex, IEdgeList<TWeight>>, IWeightedAdjacencyList<TVertex,TWeight> {
		public new System.Collections.IEnumerator GetEnumerator() {
			throw new NotImplementedException();
		}
	}
}
