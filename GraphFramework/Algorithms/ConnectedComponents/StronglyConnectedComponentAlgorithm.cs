using System;
using System.Collections.Generic;
using GraphFramework.Algorithms.Search;
using System.Linq;
using GraphFramework.Algorithms.Framework;
using GraphFramework.Interfaces;

namespace GraphFramework.Algorithms.ConnectedComponents {

	public sealed class StronglyConnectedComponentsAlgorithm<TVertex> : AlgorithmBase<TVertex>, IConnectedComponentAlgorithm<TVertex> {
		private readonly IDictionary<TVertex, int> components;
		private readonly Dictionary<TVertex, int> discoverTimes;
		private readonly Dictionary<TVertex, TVertex> roots;
		private Stack<TVertex> stack;
		int componentCount;
		int dfsTime;

		public StronglyConnectedComponentsAlgorithm(
			IAdjacencyGraph<TVertex> g)
			: this(g, new Dictionary<TVertex, int>()) { }


		public StronglyConnectedComponentsAlgorithm(
			IAdjacencyGraph<TVertex> g,
			IDictionary<TVertex, int> components)
			: base(g) {

			this.components = components;
			this.roots = new Dictionary<TVertex, TVertex>();
			this.discoverTimes = new Dictionary<TVertex, int>();
			this.stack = new Stack<TVertex>();
			this.componentCount = 0;
			this.dfsTime = 0;
		}

		public IDictionary<TVertex, int> Components {
			get {
				return this.components;
			}
		}

		public IDictionary<TVertex, TVertex> Roots {
			get {
				return this.roots;
			}
		}

		public IDictionary<TVertex, int> DiscoverTimes {
			get {
				return this.discoverTimes;
			}
		}

		public int ComponentCount {
			get {
				return this.componentCount;
			}
		}

		private void DiscoverVertex(IGraph<TVertex> visitedGraph, TVertex v) {
			this.Roots[v] = v;
			this.Components[v] = int.MaxValue;
			this.DiscoverTimes[v] = dfsTime++;
			this.stack.Push(v);
		}

		/// <summary>
		/// Used internally
		/// </summary>
		private void FinishVertex(IGraph<TVertex> visitedGraph, TVertex v) {
			var roots = this.Roots;

			foreach (var w in this.VisitedGraph.OutEdges(v)) {				
				if (this.Components[w] == int.MaxValue)
					roots[v] = this.MinDiscoverTime(roots[v], roots[w]);
			}
			if (this.roots[v].Equals(v)) {
				var w = default(TVertex);
				do {
					w = this.stack.Pop();
					this.Components[w] = componentCount;
				}
				while (!w.Equals(v));
				++componentCount;
			}
		}

		private TVertex MinDiscoverTime(TVertex u, TVertex v) {
			if (this.discoverTimes[u] < this.discoverTimes[v])
				return u;
			else
				return v;
		}

		protected override void InternalCompute() {
			this.Components.Clear();
			this.Roots.Clear();
			this.DiscoverTimes.Clear();
			this.stack.Clear();
			this.componentCount = 0;
			this.dfsTime = 0;

			DepthFirstSearchAlgorithm<TVertex> dfs = null;
			try {
				dfs = new DepthFirstSearchAlgorithm<TVertex>(					
					VisitedGraph,
					new Dictionary<TVertex, VisitedState>(this.VisitedGraph.VertexCount)
					);
				dfs.DiscoverVertex += new VertexAction<TVertex>(this.DiscoverVertex);
				dfs.FinishVertex += new VertexAction<TVertex>(this.FinishVertex);

				dfs.Compute();
			} finally {
				if (dfs != null) {
					dfs.DiscoverVertex -= new VertexAction<TVertex>(this.DiscoverVertex);
					dfs.FinishVertex -= new VertexAction<TVertex>(this.FinishVertex);
				}
			}
		}
	}
}
