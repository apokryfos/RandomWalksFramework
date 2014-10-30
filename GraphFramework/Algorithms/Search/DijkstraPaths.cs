using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Algorithms.Framework;
using GraphFramework.Interfaces;

namespace GraphFramework.Algorithms.Search {
	

	public class RootedDijkstraPaths<TVertex> : RootedSearchAlgorithmBase<TVertex>, IAlgorithm<TVertex> {

		
		private Dictionary<TVertex, double> Distances;
		private Dictionary<TVertex, TVertex> Previous;
		private HashSet<TVertex> Covered;

		private Func<TVertex,TVertex, double> weightFunc;

		public RootedDijkstraPaths(IAdjacencyGraph<TVertex> graph, Func<TVertex, TVertex, double> weightFunc)
			: base(graph) {
				Distances = new Dictionary<TVertex, double>();
				Previous = new Dictionary<TVertex, TVertex>();
			this.weightFunc = weightFunc;
			Covered = new HashSet<TVertex>();
			DiscoveredPath += new WeightedEdgeAction<TVertex, double>(DijkstraPaths_DiscoveredPath);
		}

		

		public RootedDijkstraPaths(IAdjacencyGraph<TVertex> graph) : this(graph, (i,j) =>1) {						
		}
		public RootedDijkstraPaths(IAdjacencyGraph<TVertex> graph, IWeighted<TVertex, double> w)
			: this(graph, w.GetWeight) {
		}

		public override void Initialize() {
			base.Initialize();
			TVertex root;
			if (!TryGetRootVertex(out root)) {
				SetRootVertex(VisitedGraph.Vertices.First());
			}
		}

		public event WeightedEdgeAction<TVertex,double> DiscoveredPath;
		public void OnDiscoverPath(TVertex s, TVertex t, double length) {
			var eh = DiscoveredPath;
			if (eh != null) {
				eh(VisitedGraph, s, t, length);
			}
		}

		void DijkstraPaths_DiscoveredPath(IGraph<TVertex> graph, TVertex source, TVertex target, double length) {
			var pending_distance = GetDistance(target);
			if (pending_distance > length) {
				SetDistance(target, length);
			} 
		}

		protected override void InternalCompute() {
			TVertex start;
			TryGetRootVertex(out start);
			Previous[start] = start;
			Queue<TVertex> items = new Queue<TVertex>();			
			items.Enqueue(start);
			while (items.Count > 0) {
				TVertex c = items.Dequeue();				
				foreach (var e in VisitedGraph.AdjacentEdges(c)) {
					OnDiscoverPath(c, e, weightFunc(c, e));										
					if (!Covered.Contains(e) && !items.Contains(e)) {
						items.Enqueue(e);
					}
				}				
				Covered.Add(c);
			}
		}

		
		public TVertex GetPrevious(TVertex v) {
			return (Previous.ContainsKey(v) ? Previous[v] : default(TVertex));
		}

		public double GetDistance(TVertex t) {			
			return (Distances.ContainsKey(t) ? Distances[t] : double.PositiveInfinity);
		}

		protected void SetPrevious(TVertex v, TVertex p) {
			Previous[v] = p;
		}
		protected void SetDistance(TVertex t, double distance) {			
			Distances[t] = distance;							
		}

	}
}
