using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphFramework.Interfaces {

	public delegate TWeight EdgeWeight<TVertex, TWeight>(TVertex source, TVertex target);

	public interface IWeighted<TVertex, TWeight>  {
		TWeight GetWeight(TVertex source, TVertex target);
		void SetWeight(TVertex source, TVertex target, TWeight newWeight);
	}


	public interface IWeightedGraph<TVertex,TWeight> : IWeighted<TVertex,TWeight>, IGraph<TVertex> {
	}


	public interface IWeightedAdjacencyGraph<TVertex, TWeight> : IWeightedGraph<TVertex,TWeight> ,IAdjacencyGraph<TVertex> {
	}

	public interface IWeightedBidirectionalGraph<TVertex, TWeight> : IWeightedGraph<TVertex, TWeight>, IBidirectionalGraph<TVertex> {
	}

	public interface IWeightedUndirectedGraph<TVertex, TWeight> : IWeightedGraph<TVertex, TWeight>, IUndirectedGraph<TVertex> {
	}







}
