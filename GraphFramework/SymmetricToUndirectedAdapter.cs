using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;
using GraphFramework.Extensions;

namespace GraphFramework {
	public class SymmetricToUndirectedAdapter<TVertex> : IUndirectedGraph<TVertex> {
		private IAdjacencyGraph<TVertex> SymmetricGraph;
		private bool MutationCascade = false;
		
		public SymmetricToUndirectedAdapter(IAdjacencyGraph<TVertex> SymmetricGraph) { 
			this.SymmetricGraph = SymmetricGraph;
			SymmetricGraph.EdgeAdded += new EdgeAction<TVertex>(SymmetricGraph_EdgeAdded);
			SymmetricGraph.EdgeRemoved += new EdgeAction<TVertex>(SymmetricGraph_EdgeRemoved);
		}

		void SymmetricGraph_EdgeRemoved(IGraph<TVertex> graph, TVertex source, TVertex target) {
			if (MutationCascade == true) {
				SymmetricGraph.RemoveEdge(target, source);
				MutationCascade = false;
			}
		}

		void SymmetricGraph_EdgeAdded(IGraph<TVertex> graph, TVertex source, TVertex target) {
			if (MutationCascade == true) {
				SymmetricGraph.AddEdge(target, source);
				MutationCascade = false;
			}

		}

		public int OutEdgeIndex(TVertex source, TVertex target) { return SymmetricGraph.OutEdgeIndex(source, target); }
		public int RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex> predicate) { return SymmetricGraph.RemoveOutEdgeIf(v, predicate); }
		public void ClearOutEdges(TVertex v) { SymmetricGraph.ClearOutEdges(v);	}
		public bool IsOutEdgesEmpty(TVertex v) { return SymmetricGraph.IsOutEdgesEmpty(v); }
		public int OutDegree(TVertex v) { return SymmetricGraph.OutDegree(v); }
		public IEnumerable<TVertex> OutEdges(TVertex v) { return SymmetricGraph.OutEdges(v); }
		public bool TryGetOutEdges(TVertex v, out IEnumerable<TVertex> edges) { return SymmetricGraph.TryGetOutEdges(v, out edges); }
		public TVertex OutEdge(TVertex v, int index) { return SymmetricGraph.OutEdge(v, index); }
		public int AddOutEdges(TVertex v, IEnumerable<TVertex> targets) { return SymmetricGraph.AddOutEdges(v, targets); }
		public int AddVertexAndOutEdges(TVertex v, IEnumerable<TVertex> targets) { return SymmetricGraph.AddVertexAndOutEdges(v, targets); }
		public int AdjacentEdgeIndex(TVertex source, TVertex target) { return OutEdgeIndex(source, target); }
		public int AdjacentDegree(TVertex v) { return OutDegree(v);	}
		public TVertex AdjacentEdge(TVertex v, int index) {	return AdjacentEdge(v, index); }
		public IEnumerable<TVertex> AdjacentEdges(TVertex v) { return OutEdges(v); }
		public bool TryGetAdjacentEdges(TVertex v, out IEnumerable<TVertex> edges) { return TryGetOutEdges(v, out edges); }
		public bool IsAdjacentEdgesEmpty(TVertex v) { return IsOutEdgesEmpty(v); }
		public void ClearAdjacentEdges(TVertex v) { ClearAdjacentEdges(v); }
		public bool ContainsEdge(TVertex source, TVertex target) { return SymmetricGraph.ContainsEdge(source, target); }
		public int NumberOfMultiEdges(TVertex source, TVertex target) { return SymmetricGraph.NumberOfMultiEdges(source, target); }
		public bool AddEdge(TVertex source, TVertex target) { 
			MutationCascade = true; 
			var result = SymmetricGraph.RemoveEdge(source, target);
			MutationCascade = false;
			return result;
		}
		public event EdgeAction<TVertex> EdgeAdded {
			add { SymmetricGraph.EdgeAdded += value; }
			remove { SymmetricGraph.EdgeAdded -= value; }
		}
		public int AddEdgeRange(IEnumerable<TVertex> sources, IEnumerable<TVertex> targets) { return SymmetricGraph.AddEdgeRange(sources, targets);	}
		public int AddEdgeRange(IEnumerable<Edge<TVertex>> edges) {	return SymmetricGraph.AddEdgeRange(edges);	}
		public bool AddVerticesAndEdge(TVertex source, TVertex target) { return SymmetricGraph.AddVerticesAndEdge(source, target); }
		public int AddVerticesAndEdgeRange(IEnumerable<TVertex> sources, IEnumerable<TVertex> targets) { return SymmetricGraph.AddVerticesAndEdgeRange(sources, targets); }
		public int AddVerticesAndEdgeRange(IEnumerable<Edge<TVertex>> edges) {	return SymmetricGraph.AddVerticesAndEdgeRange(edges); }
		public bool RemoveEdge(TVertex source, TVertex target) {
			MutationCascade = true;
			var result = SymmetricGraph.RemoveEdge(source, target);
			MutationCascade = false;
			return result;
		}
		public int RemoveEdgeRange(IEnumerable<Edge<TVertex>> edges) { return SymmetricGraph.RemoveEdgeRange(edges); }
		public event EdgeAction<TVertex> EdgeRemoved {
			add { SymmetricGraph.EdgeRemoved += value; }
			remove { SymmetricGraph.EdgeRemoved -= value; }
		}

		public int RemoveEdgeIf(EdgePredicate<TVertex> predicate) {
			return SymmetricGraph.RemoveEdgeIf(predicate);
		}

		public void Clear() {
			SymmetricGraph.Clear();
		}

		public event EventHandler Cleared {
			add { SymmetricGraph.Cleared += value; }
			remove { SymmetricGraph.Cleared -= value; }
		}

		public bool IsEdgesEmpty {
			get { return SymmetricGraph.IsEdgesEmpty; }
		}

		public int EdgeCount {
			get { return (SymmetricGraph.EdgeCount / 2); }
		}

		public IEnumerable<Edge<TVertex>> Edges {
			get { return SymmetricGraph.Edges; }
		}

		public EdgeEnumeration<TVertex> EdgesTargets {
			get { return SymmetricGraph.EdgesTargets; }
		}

		public bool IsDirected {
			get { return false; }
		}

		public bool AllowParallelEdges {
			get { return SymmetricGraph.AllowParallelEdges; }
		}

		public bool IsMutable {
			get { return SymmetricGraph.IsMutable; }
		}

		public object Clone() {
			var g2 = (IAdjacencyGraph<TVertex>)SymmetricGraph.Clone();
			return new SymmetricToUndirectedAdapter<TVertex>(g2);
		}
		public object Clone(bool AllowParallelEdges) {
			var g2 = (IAdjacencyGraph<TVertex>)SymmetricGraph.Clone(AllowParallelEdges);
			return new SymmetricToUndirectedAdapter<TVertex>(g2);
		}

		public event VertexAction<TVertex> VertexAdded {
			add { SymmetricGraph.VertexAdded += value; }
			remove { SymmetricGraph.VertexAdded -= value; }
		}

		public bool AddVertex(TVertex v) {
			return SymmetricGraph.AddVertex(v);
		}

		public int AddVertexRange(IEnumerable<TVertex> vertices) {
			return SymmetricGraph.AddVertexRange(vertices);
		}

		public event VertexAction<TVertex> VertexRemoved {
			add { SymmetricGraph.VertexRemoved += value; }
			remove { SymmetricGraph.VertexRemoved -= value; }
		}
		public bool RemoveVertex(TVertex v) {
			return SymmetricGraph.RemoveVertex(v);
		}

		public int RemoveVertexIf(VertexPredicate<TVertex> pred) {
			return SymmetricGraph.RemoveVertexIf(pred);
		}

		public bool ContainsVertex(TVertex vertex) {
			return SymmetricGraph.ContainsVertex(vertex);
		}

		public bool IsVerticesEmpty {
			get { return SymmetricGraph.IsVerticesEmpty; }
		}

		public int VertexCount {
			get { return SymmetricGraph.VertexCount; }
		}

		public IEnumerable<TVertex> Vertices {
			get { return SymmetricGraph.Vertices; }
		}
	}
}
