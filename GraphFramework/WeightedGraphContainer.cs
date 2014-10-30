using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;
using GraphFramework.Extensions;

namespace GraphFramework {


	public class WeightsProvider<TGraph, TVertex, TWeight> : IWeighted<TVertex, TWeight> 
		where TGraph : IAdjacencyGraph<TVertex> {
		
		EdgeWeight<TVertex, TWeight> weightFunction;
		IWeightedAdjacencyList<TVertex, TWeight> weights;
		TGraph graph;

		public WeightsProvider(TGraph graph) {
			this.graph = graph;
			this.weightFunction = null;
			weights = GraphExtensions.GetWeightedAdjacencyListInstace<TVertex, TWeight>();
		}


		public WeightsProvider(TGraph graph, EdgeWeight<TVertex, TWeight> weightFunction) {
			this.graph = graph;
			this.weightFunction = weightFunction;
			weights = GraphExtensions.GetWeightedAdjacencyListInstace<TVertex, TWeight>();
		}


		public virtual TWeight GetWeight(TVertex source, TVertex target) {
			var ind = graph.AdjacentEdgeIndex(source, target);
			if (ind < 0) { return default(TWeight); }
			if (!weights.ContainsKey(source)) {				
				IEdgeList<TWeight> weightsList = GraphExtensions.GetEdgeListInstance<TWeight>();
				weights.Add(source, weightsList);
				for (int i = 0 ;i < graph.AdjacentDegree(source);i++) {
					if (object.Equals(weightFunction, null)) { weightsList.Add(default(TWeight)); } else { weightsList.Add(weightFunction(source, graph.AdjacentEdge(source, i))); }
				}
			}
			return weights[source][ind];
		}

		public virtual void SetWeight(TVertex source, TVertex target, TWeight newWeight) {
			var ind = graph.AdjacentEdgeIndex(source, target);
			if (ind >= 0) {
				GetWeight(source, target);
				weights[source][ind] = newWeight;
			}
		}
	}

	
	public class WeightedAdjacencyGraph<TVertex,TWeight> : AdjacencyGraph<TVertex>, IWeightedAdjacencyGraph<TVertex, TWeight> {
		
		
		WeightsProvider<AdjacencyGraph<TVertex>, TVertex, TWeight> weights;

		public WeightedAdjacencyGraph() : this(null) {	}
		public WeightedAdjacencyGraph(EdgeWeight<TVertex, TWeight> weightFunction) : base() {
			weights = new WeightsProvider<AdjacencyGraph<TVertex>, TVertex, TWeight>(this, weightFunction);
		}
		public virtual TWeight GetWeight(TVertex source, TVertex target) {
			return weights.GetWeight(source, target);
		}

		public virtual void SetWeight(TVertex source, TVertex target, TWeight newWeight) {
			weights.SetWeight(source, target, newWeight);
		}
	}
	public class WeightedBidirectionalGraph<TVertex, TWeight> : BidirectionalGraph<TVertex>, IWeightedBidirectionalGraph<TVertex,TWeight>, IWeighted<TVertex, TWeight> {

		WeightsProvider<BidirectionalGraph<TVertex>, TVertex, TWeight> weights;
		

		public WeightedBidirectionalGraph() : this(null) {	}
		public WeightedBidirectionalGraph(EdgeWeight<TVertex, TWeight> weightFunction) : base() {
			weights = new WeightsProvider<BidirectionalGraph<TVertex>, TVertex, TWeight>(this, weightFunction);
		}

		public virtual TWeight GetWeight(TVertex source, TVertex target) {			
			return weights.GetWeight(source, target);
		}

		public virtual void SetWeight(TVertex source, TVertex target, TWeight newWeight) {
			weights.SetWeight(source, target, newWeight);
		}
	}

	public class WeightedUndirectedGraph<TVertex, TWeight> : UndirectedGraph<TVertex>, IWeightedUndirectedGraph<TVertex,TWeight>, IWeighted<TVertex, TWeight> {

		WeightsProvider<UndirectedGraph<TVertex>, TVertex, TWeight> weights;


		public WeightedUndirectedGraph() : this(null) {	}
		public WeightedUndirectedGraph(EdgeWeight<TVertex, TWeight> weightFunction)
			: base() {
				weights = new WeightsProvider<UndirectedGraph<TVertex>, TVertex, TWeight>(this, weightFunction);
		}

		public virtual TWeight GetWeight(TVertex source, TVertex target) {
			return weights.GetWeight(source, target);
		}

		public virtual void SetWeight(TVertex source, TVertex target, TWeight newWeight) {
			weights.SetWeight(source, target, newWeight);
		}
	}
}
