
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;
using GraphFramework.Extensions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GraphFramework {
	public class BidirectionalGraph<TVertex> : AdjacencyGraph<TVertex>, IBidirectionalGraph<TVertex> {


		protected readonly IAdjacencyList<TVertex> inEdges;		
	
		
		public BidirectionalGraph()
			: this(true) { }

		public BidirectionalGraph(bool allowParallelEdges)
			: this(allowParallelEdges, EdgeExtensions.GetDirectedVertexEquality<TVertex>()) {
			
		}

		public BidirectionalGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer)
			: this(allowParallelEdges, edgeEqualityComparer, EqualityComparer<TVertex>.Default) { }

		public BidirectionalGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer, IEqualityComparer<TVertex> vertexComparer)
			: this(allowParallelEdges, edgeEqualityComparer, vertexComparer, true) { }


		public BidirectionalGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer, IEqualityComparer<TVertex> vertexComparer, bool isMutable) 
			: base(allowParallelEdges, edgeEqualityComparer, vertexComparer, isMutable) {
				inEdges = GraphExtensions.GetAdjacencyListInstance<TVertex>();
			
		}

		public override bool AddVertex(TVertex v) {
			if (base.AddVertex(v)) {
				inEdges.Add(v, GraphExtensions.GetEdgeListInstance<TVertex>());
				return true;
			}
			return false;
		}

		

		protected override IEdgeList<TVertex> AddAndReturnEdges(TVertex v) {
			var outE = base.AddAndReturnEdges(v);
			IEdgeList<TVertex> edges;
			if (!inEdges.TryGetValue(v, out edges)) {
				edges = GraphExtensions.GetEdgeListInstance<TVertex>();
				inEdges.Add(v, edges);
			}
			return outE;
		}

		public override bool AddEdge(TVertex source, TVertex target) {
			if (base.AddEdge(source, target)) {				
				inEdges[target].Add(source);
				return true;
			}
			return false;
		}


		public override bool RemoveEdge(TVertex source, TVertex target) {
			if (base.RemoveEdge(source, target)) {
				inEdges[target].Remove(source);
				return true;
			}
			return false;
		}

		public void ClearInEdges(TVertex v) {
			foreach (var e in OutEdges(v)) {
				RemoveEdge(e, v);
			}			
			inEdges[v].Clear();
		}

		public void ClearEdges(TVertex v) {
			ClearInEdges(v);
			ClearOutEdges(v);
		}

		public bool IsInEdgesEmpty(TVertex v) {
			return InDegree(v) == 0;
		}

		public int InDegree(TVertex v) {
			return inEdges[v].Count;
		}

		public IEnumerable<TVertex> InEdges(TVertex v) {
			return inEdges[v];
		}

		public bool TryGetInEdges(TVertex v, out IEnumerable<TVertex> edges) {
			try { edges = InEdges(v); return true; } catch { edges = null; return false; }			
		}

		public TVertex InEdge(TVertex v, int index) {
			return inEdges[v][index];
		}

		public int Degree(TVertex v) {
			return OutDegree(v) + InDegree(v);
		}


		public int RemoveInEdgeIf(TVertex v, EdgePredicate<TVertex> edgePredicate) {
			int c = 0;
			foreach (var vertex in InEdges(v)) {
				if (edgePredicate(v, vertex)) {
					RemoveEdge(vertex, v);
					c++;
				}
			}
			return c;
		}

		public override int AdjacentDegree(TVertex v) {
			return Degree(v);
		}
		public override TVertex AdjacentEdge(TVertex v, int index) {
			if (index < OutDegree(v)) {
				return base.AdjacentEdge(v, index);
			} else {
				return inEdges[v][index - OutDegree(v)];
			}
		}

		public override IEnumerable<TVertex> AdjacentEdges(TVertex v) {
			foreach (var e in OutEdges(v)) {
				yield return e;
			}
			foreach (var e in InEdges(v)) {
				yield return e;
			}
			yield break;
		}
				

		public override bool AddVerticesAndEdge(TVertex source, TVertex target) {
			if (base.AddVerticesAndEdge(source, target)) {
				AddVertex(source);
				AddVertex(target);
				inEdges[target].Add(source);				
				return true;
			}
			return false;
		}

		public override void ClearOutEdges(TVertex v) {
			foreach (var e in OutEdges(v)) {
				inEdges[e].Remove(v);
			}
			base.ClearOutEdges(v);
		}
		public override bool IsAdjacentEdgesEmpty(TVertex v) {
			return IsOutEdgesEmpty(v) && IsInEdgesEmpty(v);
		}


		public int InEdgeIndex(TVertex source, TVertex target) {
			if (ContainsVertex(source)) {
				return inEdges[source].IndexOf(target);
			} else {
				return -2;
			}
		}
	}
}
