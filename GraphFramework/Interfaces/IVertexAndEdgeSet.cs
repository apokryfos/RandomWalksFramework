using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Extensions;

namespace GraphFramework.Interfaces {
	public interface IVertexAndEdgeSet<TVertex> : IGraph<TVertex>, IVertexSet<TVertex> {

		#region Global Edge Operations		
		bool ContainsEdge(TVertex source, TVertex target);
		int NumberOfMultiEdges(
			TVertex source,
			TVertex target);
		bool AddEdge(TVertex source, TVertex target);
		event EdgeAction<TVertex> EdgeAdded;
		int AddEdgeRange(IEnumerable<TVertex> sources, IEnumerable<TVertex> targets);
		int AddEdgeRange(IEnumerable<Edge<TVertex>> edges);
		bool AddVerticesAndEdge(TVertex source, TVertex target);
		int AddVerticesAndEdgeRange(IEnumerable<TVertex> sources, IEnumerable<TVertex> targets);
		int AddVerticesAndEdgeRange(IEnumerable<Edge<TVertex>> edges);
		bool RemoveEdge(TVertex source, TVertex target);
		int RemoveEdgeRange(IEnumerable<Edge<TVertex>> edges);
		event EdgeAction<TVertex> EdgeRemoved;
		int RemoveEdgeIf(EdgePredicate<TVertex> predicate);
		void Clear();
		event EventHandler Cleared;
		bool IsEdgesEmpty { get; }
		int EdgeCount { get; }
		IEnumerable<Edge<TVertex>> Edges { get; }
		EdgeEnumeration<TVertex> EdgesTargets { get; }
		#endregion
	}
}
