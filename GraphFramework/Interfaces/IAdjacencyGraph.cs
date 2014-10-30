using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphFramework.Interfaces {
	public interface IAdjacencyGraph<TVertex> : IVertexAndEdgeSet<TVertex> {

		#region Adjacency Operations Out

		int OutEdgeIndex(TVertex source, TVertex target);
		int RemoveOutEdgeIf(
			TVertex v,
			EdgePredicate<TVertex> predicate);
		void ClearOutEdges(TVertex v);
		bool IsOutEdgesEmpty(TVertex v);
		int OutDegree(TVertex v);
		IEnumerable<TVertex> OutEdges(TVertex v);
		bool TryGetOutEdges(TVertex v, out IEnumerable<TVertex> edges);
		TVertex OutEdge(TVertex v, int index);
		int AddOutEdges(TVertex v, IEnumerable<TVertex> targets);
		int AddVertexAndOutEdges(TVertex v, IEnumerable<TVertex> targets);

		int AdjacentEdgeIndex(TVertex source, TVertex target);
		int AdjacentDegree(TVertex v);
		TVertex AdjacentEdge(TVertex v, int index);
		IEnumerable<TVertex> AdjacentEdges(TVertex v);
		bool TryGetAdjacentEdges(TVertex v, out IEnumerable<TVertex> edges);
		bool IsAdjacentEdgesEmpty(TVertex v);
		void ClearAdjacentEdges(TVertex v);
		object Clone(bool AllowParallelEdges);
		#endregion
	}
}
