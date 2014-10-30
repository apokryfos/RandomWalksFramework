using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomWalkFramework.RandomWalkInterface;
using GraphFramework.Interfaces;

namespace RandomWalkFramework.Analysis {

	[Flags]
	public enum ObserverType {
		NONE = 0x0,
		STEP = 0x1,
		REVISITS = 0x2,
		COVERAGE = 0x4,
		DEGREECOVER = 0x10
	}



	public class RandomWalkFrameworktepObserver<TVertex> : RandomWalkObserver<TVertex> {

		public RandomWalkFrameworktepObserver(IRandomWalk<TVertex> sampler)
			: base(sampler) {
		}
		public RandomWalkFrameworktepObserver(IRandomWalk<TVertex> sampler, ulong truncate)
			: base(sampler, truncate) {
		}


		protected override void Observed_Transition(IRandomWalk<TVertex> sampler, TVertex previous, TVertex current, decimal weight) {
			OnObservation(previous, current, weight);
		}

		public override void Dispose() {

		}
	}

	public class RandomWalkRevisitObserver<TVertex> : RandomWalkObserver<TVertex> {
		public Dictionary<TVertex, List<decimal>> VisitSteps { private set; get; }
		public Dictionary<TVertex, decimal> StateWeights { private set; get; }


		public RandomWalkRevisitObserver(IRandomWalk<TVertex> sampler)
			: base(sampler) {
			VisitSteps = new Dictionary<TVertex, List<decimal>>();
			StateWeights = new Dictionary<TVertex, decimal>();
		}



		protected override void Observed_Transition(IRandomWalk<TVertex> sampler, TVertex previous, TVertex current, decimal weight) {

			List<decimal> vsv;
			if (!VisitSteps.TryGetValue(current, out vsv)) {
				vsv = new List<decimal>();
				VisitSteps.Add(current, vsv);
				StateWeights.Add(current, sampler.GetStateWeight(current));
			}
			vsv.Add(sampler.TotalSteps);
			OnObservation(previous, current, StateWeights[current]);

		}

		public override void Dispose() {
			VisitSteps.Clear();
			StateWeights.Clear();
		}
	}


	public class RandomWalkCoverageObserver<TVertex> : RandomWalkObserver<TVertex> {
		HashSet<TVertex> visited;

		public int VisitedStates { get { return visited.Count; } }
		public decimal Coverage { get { return ((decimal)VisitedStates / (decimal)TotalStates); } }
		public int TotalStates { get; private set; }

		public RandomWalkCoverageObserver(IRandomWalk<TVertex> sampler, int totalStates)
			: base(sampler) {
			TotalStates = totalStates;
			visited = new HashSet<TVertex>();
		}


		protected override void Observed_Transition(IRandomWalk<TVertex> sampler, TVertex previous, TVertex current, decimal weight) {
			if (visited.Add(current)) {
				OnObservation(previous, current, weight);
			}
		}


		public override void Dispose() {

		}
	}

	public class RandomWalkDegreeCoverageObserver<TVertex> : RandomWalkObserver<TVertex> {
		private Dictionary<int, int> degreeCounts;
		private RandomWalkCoverageObserver<TVertex> coverage;
		private IUndirectedGraph<TVertex> targetGraph;

		public RandomWalkDegreeCoverageObserver(IRandomWalk<TVertex> obs, IUndirectedGraph<TVertex> targetGraph)
			: base(obs) {
			this.targetGraph = targetGraph;
			degreeCounts = new Dictionary<int, int>();
			foreach (var v in targetGraph.Vertices) {
				if (degreeCounts.ContainsKey(targetGraph.AdjacentDegree(v)))
					degreeCounts[targetGraph.AdjacentDegree(v)]++;
				else
					degreeCounts.Add(targetGraph.AdjacentDegree(v), 1);
			}
			coverage = new RandomWalkCoverageObserver<TVertex>(obs, targetGraph.VertexCount);
			coverage.ObservationEvent += new ObserverEvent<TVertex>(coverage_Hit);

		}

		void coverage_Hit(RandomWalkObserver<TVertex> sampler, TVertex previous, TVertex current, object parameters) {
			degreeCounts[targetGraph.AdjacentDegree(current)]--;
			if (degreeCounts[targetGraph.AdjacentDegree(current)] == 0)
				OnObservation(previous, current, targetGraph.AdjacentDegree(current));
		}


		protected override void Observed_Transition(IRandomWalk<TVertex> sampler, TVertex previous, TVertex current, decimal weight) {

		}

		public override void Dispose() {
			degreeCounts.Clear();
		}
	}
}
