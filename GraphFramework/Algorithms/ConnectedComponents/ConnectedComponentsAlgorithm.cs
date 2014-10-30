using System;
using System.Collections.Generic;
using GraphFramework.Algorithms.Search;
using System.Diagnostics.Contracts;
using GraphFramework.Interfaces;
using GraphFramework.Algorithms.Framework;
using DataStructures;



namespace GraphFramework.Algorithms.ConnectedComponents {

	/// <summary>
	/// Based on the QuickGraph connected component algorithm
	/// </summary>
	/// <typeparam name="TVertex">Vertex type</typeparam>
	public sealed class ConnectedComponentsAlgorithm<TVertex> : AlgorithmBase<TVertex>, IConnectedComponentAlgorithm<TVertex> {
		private IDictionary<TVertex, int> components;
		private int componentCount = 0;

		private static IDictionary<TVertex, int> DictionaryProvider() {
			return new FragmentedDictionary<TVertex, int>();
		}

		public ConnectedComponentsAlgorithm(IAdjacencyGraph<TVertex> g)
			: this(g, DictionaryProvider()) { }

		public ConnectedComponentsAlgorithm(
			IAdjacencyGraph<TVertex> visitedGraph,
			IDictionary<TVertex, int> components) : base(visitedGraph) {
			this.components = components;
		}

		public IDictionary<TVertex, int> Components {
			get {
				return this.components;
			}
		}

		public int ComponentCount {
			get { return this.componentCount; }
		}

		private void StartVertex(IGraph<TVertex> graph, TVertex v) {
			++this.componentCount;
		}

		private void DiscoverVertex(IGraph<TVertex> graph, TVertex v) {
			this.Components[v] = this.componentCount;
		}

		protected override void InternalCompute() {
			this.components.Clear();
			if (this.VisitedGraph.VertexCount == 0) {
				this.componentCount = 0;
				return;
			}

			this.componentCount = -1;
			DepthFirstSearchAlgorithm<TVertex> dfs = null;
			try {
				dfs = new DepthFirstSearchAlgorithm<TVertex>(this.VisitedGraph, new Dictionary<TVertex, VisitedState>(this.VisitedGraph.VertexCount));
				dfs.StartVertex += new VertexAction<TVertex>(this.StartVertex);
				dfs.DiscoverVertex += new VertexAction<TVertex>(this.DiscoverVertex);
				dfs.Compute();
				++this.componentCount;
			} finally {
				if (dfs != null) {
					dfs.StartVertex -= new VertexAction<TVertex>(this.StartVertex);
					dfs.DiscoverVertex -= new VertexAction<TVertex>(this.DiscoverVertex);
				}
			}
		}
	}
}
