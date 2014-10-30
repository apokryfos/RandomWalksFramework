using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomWalkFramework.RandomWalkInterface;
using RandomWalkFramework.Querier;

namespace RandomWalkFramework {
	public abstract class LazySimpleRandomWalk<TVertex> : RandomWalk<TVertex>, IRandomWalk<TVertex>
	   {
		public LazySimpleRandomWalk(TVertex entryPoint, UnweightedGraphQuerier<TVertex> targetGraph, KeyValuePair<string, string> name)
			: base(entryPoint, targetGraph, name) {
			Name = new KeyValuePair<string, string>("LSRW", "Lazy Simple Random Walk");
		}

		protected override TVertex ChooseNext(TVertex current) {
			if (r.NextDouble() <= 0.5)
				return base.ChooseNext(current);
			else
				return current;
		}

		public override TVertex GetAdjacentTransition(TVertex state, int index) {
			if (index < 0 || index >= base.GetAdjacentTransitionCount(state))
				return state;
			return base.GetAdjacentTransition(state, index);
		}

		public override int GetAdjacentTransitionCount(TVertex state) {
			return base.GetAdjacentTransitionCount(state) + 1;
		}

		public override decimal GetTransitionWeight(TVertex source, TVertex target) {
			if (source.Equals(target))
				return 0.5M;
			return base.GetTransitionWeight(source, target) * 0.5M;
		}
	}
	public abstract class LazyWeightedRandomWalk<TVertex> : LazySimpleRandomWalk<TVertex>, IRandomWalk<TVertex>
	   {
		public LazyWeightedRandomWalk(TVertex entryPoint, WeightedGraphQuerier<TVertex> targetGraph, KeyValuePair<string, string> name)
			: base(entryPoint, targetGraph, new KeyValuePair<string, string>("L" + targetGraph.PolicyName.Key, "Lazy " + targetGraph.PolicyName.Value)) {			

		}
	}

}
