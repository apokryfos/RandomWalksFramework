using System;
using System.Collections.Generic;
using GraphFramework.Algorithms.Framework;
using GraphFramework.Interfaces;
using DataStructures;

namespace GraphFramework.Algorithms.Search {
	/// <summary>
	/// A depth first search algorithm for directed graph
	/// </summary>
	/// <typeparam name="TVertex">type of a vertex</typeparam>
	/// <typeparam name="TEdge">type of an edge</typeparam>
	/// <reference-ref
	///     idref="gross98graphtheory"
	///     chapter="4.2"
	///     />
	///     

	public enum VisitedState { NOTVISITED = 0, PARTIALLYVISITED = 1, VISITED = 2 }

	public sealed class DepthFirstSearchAlgorithm<TVertex> : RootedAlgorithmBase<TVertex> {

		private static IDictionary<TVertex, VisitedState> DictionaryProvider() {
			return new FragmentedDictionary<TVertex, VisitedState>();
		}
		private readonly IDictionary<TVertex, VisitedState> vertexState;
		
		private readonly Func<IEnumerable<TVertex>, IEnumerable<TVertex>> edgeEnumerator;

		private int NotvisitedCount,Partiallyvisitedcount,Visitedcount;

		/// <summary>
		/// Initializes a new instance of the algorithm.
		/// </summary>
		/// <param name="visitedGraph">visited graph</param>
		public DepthFirstSearchAlgorithm(IAdjacencyGraph<TVertex> visitedGraph)
			: this(visitedGraph, DictionaryProvider()) { }

		/// <summary>
		/// Initializes a new instance of the algorithm.
		/// </summary>
		/// <param name="visitedGraph">visited graph</param>
		/// <param name="colors">vertex color map</param>
		public DepthFirstSearchAlgorithm(IAdjacencyGraph<TVertex> visitedGraph, IDictionary<TVertex, VisitedState> vertexState) : this(visitedGraph, DictionaryProvider(), e => e) { }

		/// <summary>
		/// Initializes a new instance of the algorithm.
		/// </summary>
		/// <param name="host">algorithm host</param>
		/// <param name="visitedGraph">visited graph</param>
		/// <param name="colors">vertex color map</param>
		/// <param name="outEdgeEnumerator">
		/// Delegate that takes the enumeration of out-edges and reorders
		/// them. All vertices passed to the method should be enumerated once and only once.
		/// May be null.
		/// </param>
		public DepthFirstSearchAlgorithm(IAdjacencyGraph<TVertex> visitedGraph, IDictionary<TVertex, VisitedState> vertexState, Func<IEnumerable<TVertex>, IEnumerable<TVertex>> outEdgeEnumerator
			)
			: base(visitedGraph) {
			this.vertexState = vertexState;
			this.edgeEnumerator = outEdgeEnumerator;
			MaxDepth = int.MaxValue;
		}

		public IDictionary<TVertex, VisitedState> VertexState { get { return this.vertexState; } }

		public Func<IEnumerable<TVertex>, IEnumerable<TVertex>> OutEdgeEnumerator {
			get { return this.edgeEnumerator; }
		}

		public VisitedState GetVertexState(TVertex vertex) {
			return this.vertexState[vertex];
		}


		public int MaxDepth { get; set; }
		


		public event VertexAction<TVertex> StartVertex;
		private void OnStartVertex(TVertex v) {
			var eh = this.StartVertex;
			if (eh != null)
				eh(VisitedGraph, v);
		}

		public event VertexAction<TVertex> DiscoverVertex;
		private void OnDiscoverVertex(TVertex v) {
			var eh = this.DiscoverVertex;
			if (eh != null)
				eh(VisitedGraph, v);
		}

		public event EdgeAction<TVertex> ExamineEdge;
		private void OnExamineEdge(TVertex source, TVertex target) {
			var eh = this.ExamineEdge;
			if (eh != null)
				eh(VisitedGraph, source, target);
		}

		public event EdgeAction<TVertex> TreeEdge;
		private void OnTreeEdge(TVertex source, TVertex target) {
			var eh = this.TreeEdge;
			if (eh != null)
				eh(VisitedGraph, source, target);
		}

		public event EdgeAction<TVertex> BackEdge;
		private void OnBackEdge(TVertex source, TVertex target) {
			var eh = this.BackEdge;
			if (eh != null)
				eh(VisitedGraph, source, target);
		}

		public event EdgeAction<TVertex> ForwardOrCrossEdge;
		private void OnForwardOrCrossEdge(TVertex source, TVertex target) {
			var eh = this.ForwardOrCrossEdge;
			if (eh != null)
				eh(VisitedGraph, source, target);
		}

		public event VertexAction<TVertex> FinishVertex;
		private void OnFinishVertex(TVertex v) {
			var eh = this.FinishVertex;
			if (eh != null)
				eh(VisitedGraph, v);
		}

		protected override void InternalCompute() {
			// if there is a starting vertex, start whith him:
			TVertex rootVertex;
			if (this.TryGetRootVertex(out rootVertex)) {
				this.OnStartVertex(rootVertex);
				this.Visit(rootVertex);
			} else {
				// process each vertex 
				foreach (var u in this.VisitedGraph.Vertices) {
					if (this.VertexState[u] == VisitedState.NOTVISITED) {
						this.OnStartVertex(u);
						this.Visit(u);
					}
				}
			}
		}

		public override void Initialize() {
			base.Initialize();
			Partiallyvisitedcount = 0;
			NotvisitedCount = 0;
			Visitedcount = 0;
			this.VertexState.Clear();
			foreach (var u in this.VisitedGraph.Vertices) {
				this.VertexState[u] = VisitedState.NOTVISITED;
				NotvisitedCount++;
			}
		}

		struct SearchFrame {
			public readonly TVertex Vertex;
			public readonly IEnumerator<TVertex> Edges;
			public readonly int Depth;
			public SearchFrame(TVertex vertex, IEnumerator<TVertex> edges, int depth) {
				this.Vertex = vertex;
				this.Edges = edges;
				this.Depth = depth;
			}
		}

		public void Visit(TVertex root) {
			var todo = new Stack<SearchFrame>();
			var oee = this.OutEdgeEnumerator;
			NotvisitedCount--;
			this.VertexState[root] = VisitedState.PARTIALLYVISITED;
			Partiallyvisitedcount++;
			this.OnDiscoverVertex(root);

			var enumerable = oee(this.VisitedGraph.AdjacentEdges(root));
			todo.Push(new SearchFrame(root, enumerable.GetEnumerator(), 0));
			while (todo.Count > 0) {
				var frame = todo.Pop();
				var u = frame.Vertex;
				var depth = frame.Depth;
				var edges = frame.Edges;

				
				if (depth > this.MaxDepth) {
					if (edges != null)
						edges.Dispose();
					if (this.VertexState[u] == VisitedState.NOTVISITED) { NotvisitedCount--; } else { Partiallyvisitedcount--; } 
					this.VertexState[u] = VisitedState.VISITED;
					Visitedcount++;
					this.OnFinishVertex(u);
					continue;
				}

				while (edges.MoveNext()) {
					TVertex e = edges.Current;
					this.OnExamineEdge(u, e);
					TVertex v = e;
					VisitedState c = this.VertexState[v];
					if (c == VisitedState.NOTVISITED) {
						this.OnTreeEdge(u, e);
						todo.Push(new SearchFrame(u, edges, depth));
						u = v;
						edges = oee(this.VisitedGraph.AdjacentEdges(u)).GetEnumerator();
						depth++;
						NotvisitedCount--;
						this.VertexState[u] = VisitedState.PARTIALLYVISITED;
						Partiallyvisitedcount++;
						this.OnDiscoverVertex(u);
					} else if (c == VisitedState.PARTIALLYVISITED) {
						this.OnBackEdge(u, e);
					} else {
						this.OnForwardOrCrossEdge(u, e);
					}
				}
				if (edges != null)
					edges.Dispose();

				if (this.VertexState[u] == VisitedState.NOTVISITED) { NotvisitedCount--; } else { Partiallyvisitedcount--; } 
				this.VertexState[u] = VisitedState.VISITED;
				Visitedcount++;
				this.OnFinishVertex(u);				
			}
		}
	}
}
