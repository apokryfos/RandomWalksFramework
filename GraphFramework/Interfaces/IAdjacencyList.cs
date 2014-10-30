using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphFramework.Interfaces {
	public interface IAdjacencyList<TVertex> : IDictionary<TVertex, IEdgeList<TVertex>> { }

	public interface IWeightedAdjacencyList<TVertex, TWeight> : IDictionary<TVertex, IEdgeList<TWeight>> { }
}
