using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;


namespace RandomWalkFramework.Querier {
	public class ReducedGraphQuerier<TVertex> : UnweightedGraphQuerier<TVertex> {

		public int MaxDegree { get; private set; }
		private IAdjacencyList<TVertex> ReducedAdjacencyList;
		private object SyncRoot = new object();

		public ReducedGraphQuerier(IAdjacencyGraph<TVertex> targetGraph, int MaxDegree, KeyValuePair<string, string> name)
			: base(targetGraph, name) {
				this.MaxDegree = MaxDegree;
				ReducedAdjacencyList = GraphFramework.Extensions.GraphExtensions.GetAdjacencyListInstance<TVertex>();
			}

		public IEdgeList<TVertex> GetReducedAdjecencyListFor(TVertex vertex) {

			IEdgeList<TVertex> AdjecencyList;
			lock (SyncRoot) {
				if (!ReducedAdjacencyList.TryGetValue(vertex, out AdjecencyList)) {
					if (base.AdjecentDegree(vertex) <= MaxDegree) {
						AdjecencyList = GraphFramework.Extensions.GraphExtensions.GetEdgeListInstance<TVertex>(base.AdjecentEdges(vertex).Where(e => base.AdjecentDegree(e) <= MaxDegree));
					} else {
						AdjecencyList = GraphFramework.Extensions.GraphExtensions.GetEdgeListInstance<TVertex>();
					}
					ReducedAdjacencyList.Add(vertex, AdjecencyList);
				}
			}
			return AdjecencyList;
		}

		public override int AdjecentDegree(TVertex vertex) {
			return GetReducedAdjecencyListFor(vertex).Count;
		}
		public override TVertex AdjecentEdge(TVertex vertex, int index) {
			return GetReducedAdjecencyListFor(vertex)[index];
		}
		public override IEnumerable<TVertex> AdjecentEdges(TVertex vertex) {
			return GetReducedAdjecencyListFor(vertex);
		}
		public override decimal EdgeWeight(TVertex source, TVertex target) {
			if (!object.Equals(source, target)) {
				return 1M / (decimal)MaxDegree;
			} else {
				return 1M - (decimal)AdjecentDegree(source) / (decimal)MaxDegree;
			}
		}
		public override decimal VertexWeight(TVertex vertex) {
			return 1M;
		}
	}
		
}
