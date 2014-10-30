using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RandomWalkFramework.RandomWalkInterface;
using RandomWalkFramework.Querier;

namespace RandomWalkFramework {

	public class MetropolisRandomWalk<TVertex> : RandomWalk<TVertex>, IRandomWalk<TVertex> {
		decimal M;
		public ulong Loops { get; private set; }

		public MetropolisRandomWalk(TVertex entryPoint, ReducedGraphQuerier<TVertex> targetGraph)
			: this(entryPoint, targetGraph, targetGraph.MaxDegree) {


		}
		public MetropolisRandomWalk(TVertex entryPoint, UnweightedGraphQuerier<TVertex> targetGraph, decimal M)
			: base(entryPoint, targetGraph, new KeyValuePair<string, string>("MHRW" + M, "Metropolis Hastings Random Walk")) {
			Loops = 0;
			this.M = M;
		}

		protected override TVertex ChooseNext(TVertex current) {
			/*RNG.ThreadSafeRandom gr = new RNG.ThreadSafeRandom(new RNG.GeometricallyDistributedRandom((double)GetAdjacentTransitionCount(current) / (double)M));
			var shuffle = RNG.RNGProvider.r.Next(1, 15);
			for (int i = 0; i < shuffle; i++) { gr.Next(); }
			ulong loops = (ulong)gr.Next();
		 	base.DiscreetSteps += loops;
			base.TotalSteps += loops;
			Loops += loops;
			 * */
			while (r.NextDouble() <= (1.0 - ((double)base.GetAdjacentTransitionCount(current) / (double)M))) {
				base.DiscreetSteps++;
				base.TotalSteps++;
				Loops++;
				OnStep(CurrentState, CurrentState, Math.Round(GetTransitionWeight(CurrentState, CurrentState), 3));
			}

			var n = base.ChooseNext(current);
			return n;
		}

		public override decimal GetStateWeight(TVertex state) {
			return 1.0M;
		}


		public override decimal GetTransitionWeight(TVertex source, TVertex target) {
			return (!source.Equals(target) ? 1.0M / M : 1.0M - ((decimal)GetAdjacentTransitionCount(source) / M));
		}

	}
}
