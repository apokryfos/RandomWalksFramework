using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Containers;
using DataStructures;
using GraphFramework.Interfaces;

namespace GraphFramework.Extensions {


	
	public static class GraphExtensions {
		public static IEdgeList<TVertex> GetEdgeListInstance<TVertex>() { return new ListEdgeList<TVertex>(); }
		public static IEdgeList<TVertex> GetEdgeListInstance<TVertex>(int edgeCapacity) { return new ListEdgeList<TVertex>(edgeCapacity); }
		public static IEdgeList<TVertex> GetEdgeListInstance<TVertex>(IEnumerable<TVertex> initial) { return new ListEdgeList<TVertex>(initial); }
		public static IEdgeList<TVertex> GetEdgeListInstance<TVertex>(bool allowDuplicateEdges) { return (allowDuplicateEdges ? (IEdgeList<TVertex>)new ListEdgeList<TVertex>() : (IEdgeList<TVertex>)new HashSetEdgeList<TVertex>()); }
		public static IEdgeList<TVertex> GetEdgeListInstance<TVertex>(bool allowDuplicateEdges, int edgeCapacity) { return (allowDuplicateEdges ? (IEdgeList<TVertex>)new ListEdgeList<TVertex>(edgeCapacity) : (IEdgeList<TVertex>)new HashSetEdgeList<TVertex>()); }
		public static IEdgeList<TVertex> GetEdgeListInstance<TVertex>(bool allowDuplicateEdges, IEnumerable<TVertex> initial) { return (allowDuplicateEdges ? (IEdgeList<TVertex>)new ListEdgeList<TVertex>(initial) : (IEdgeList<TVertex>)new HashSetEdgeList<TVertex>(initial)); }		
		public static IAdjacencyList<TVertex> GetAdjacencyListInstance<TVertex>() { return new AdjacencyList<TVertex>(); }

		public static IWeightedAdjacencyList<TVertex, TWeight> GetWeightedAdjacencyListInstace<TVertex, TWeight>() { return new WeightedAdjacencyList<TVertex, TWeight>(); }

	}
}
