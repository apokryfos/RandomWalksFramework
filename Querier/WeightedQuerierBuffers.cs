using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomWalkFramework.RandomWalkInterface;
using System.Threading;
using GraphFramework.Interfaces;

namespace RandomWalkFramework.Querier {

	public delegate decimal WeightFunction<TVertex>(IAdjacencyGraph<TVertex> graph, TVertex source, TVertex target);
		


	public class UnbufferedWeightedQuerier<TVertex> : GeneralWeightedQuerier<TVertex> {

		public UnbufferedWeightedQuerier(IAdjacencyGraph<TVertex> targetGraph, WeightFunction<TVertex> wf, KeyValuePair<string, string> policyName)
			: base(targetGraph, wf, policyName, 0) {
		}
	}
	public class WeightedGraphQuerier<TVertex> : GeneralWeightedQuerier<TVertex> {
		public WeightedGraphQuerier(IAdjacencyGraph<TVertex> targetGraph, WeightFunction<TVertex> wf, KeyValuePair<string, string> policyName)

			: base(targetGraph, wf, policyName,int.MaxValue) {
		}
	}
}
