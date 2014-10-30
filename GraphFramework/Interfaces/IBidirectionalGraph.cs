using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphFramework.Interfaces {
	public interface IBidirectionalGraph<TVertex> : IAdjacencyGraph<TVertex> {

		#region Adjacency Operations In
		int InEdgeIndex(TVertex source, TVertex target);
		void ClearInEdges(TVertex v);
		bool IsInEdgesEmpty(TVertex v);
		IEnumerable<TVertex> InEdges(TVertex v);
		int InDegree(TVertex v);
		bool TryGetInEdges(TVertex v, out IEnumerable<TVertex> edges);
		TVertex InEdge(TVertex v, int index);
		int RemoveInEdgeIf(TVertex v, EdgePredicate<TVertex> edgePredicate);
		#endregion

		#region Adjacency Operations All		
		void ClearEdges(TVertex v);
		void TrimEdgeExcess();
		int Degree(TVertex v);
		#endregion

	}
}
