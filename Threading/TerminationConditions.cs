using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomWalkFramework.RandomWalkInterface;
using RandomWalkFramework;
using RandomWalkFramework.Analysis;
using GraphFramework.Interfaces;

namespace RandomWalkFramework.Threading {
	public interface ITerminationConditions<TVertex> {
		bool CheckCondition();
	}

	public static class CombinedTerminationCondition<TVertex> {
		public static ITerminationConditions<TVertex> GetCombinationAny(bool any, params ITerminationConditions<TVertex>[] conditions) {
			return new CombinationTerminationConditions<TVertex>(any, conditions);
		}

		private class CombinationTerminationConditions<TVertex> : ITerminationConditions<TVertex> {
			private bool any;
			private ITerminationConditions<TVertex>[] conditions;
			public CombinationTerminationConditions(bool any, ITerminationConditions<TVertex>[] condition) {
				this.any = any;
				this.conditions = condition;
			}




			public bool CheckCondition() {
				bool res = (any ? false : true);
				foreach (var tc in conditions) {
					if (any) { res |= tc.CheckCondition(); } else { res &= tc.CheckCondition(); }
				}
				return res;
			}
		}

	}


	public class DegreeCoverageTerminationCondition<TVertex> : ITerminationConditions<TVertex> {
		protected IRandomWalk<TVertex> Walk { get; set; }
		private int Min_Covered_Degree { get; set; }
		RandomWalkDegreeCoverageObserver<TVertex> DegreeCoverageObserver { get; set; }
		int Current_Min_Covered_Degree = int.MaxValue;
		HashSet<int> degrees;

		public DegreeCoverageTerminationCondition(IRandomWalk<TVertex> rw, IUndirectedGraph<TVertex> graph, int Min_Covered_Degree) {
			degrees = new HashSet<int>(graph.Vertices.Select(v => graph.AdjacentDegree(v)));
			Walk = rw;
			DegreeCoverageObserver = new RandomWalkDegreeCoverageObserver<TVertex>(rw, graph);
			this.Min_Covered_Degree = Min_Covered_Degree;

			DegreeCoverageObserver.ObservationEvent += new ObserverEvent<TVertex>(DegreeCoverageObserver_ObservationEvent);
		}

		void DegreeCoverageObserver_ObservationEvent(RandomWalkObserver<TVertex> sampler, TVertex previous, TVertex current, object ObservationParameters) {
			if (degrees.Contains((int)ObservationParameters)) {
				degrees.Remove((int)ObservationParameters);
				Current_Min_Covered_Degree = (degrees.Count>0?degrees.Max()+1:1);
			}
		}



		public virtual bool CheckCondition() {
			return Current_Min_Covered_Degree <= Min_Covered_Degree;
		}

		public static CoverageTerminationCondition<TVertex> CoveragePCCondition(IRandomWalk<TVertex> rw, int vertexCount, int loopCount, decimal coverage, int i) {
			var r = new CoverageTerminationCondition<TVertex>(rw, vertexCount, coverage);
			return r;
		}

	}



	public class CoverageTerminationCondition<TVertex> : ITerminationConditions<TVertex> {
		protected IRandomWalk<TVertex> Walk { get; set; }
		private decimal CoverTarget { get; set; }
		RandomWalkCoverageObserver<TVertex> CoverageObserver { get; set; }

		public CoverageTerminationCondition(IRandomWalk<TVertex> rw, int totalVertices, decimal coverPC) {
			Walk = rw;
			CoverageObserver = new RandomWalkCoverageObserver<TVertex>(rw, totalVertices);
			CoverTarget = coverPC;


		}



		public virtual bool CheckCondition() {
			return CoverageObserver.Coverage >= CoverTarget;
		}

		public static CoverageTerminationCondition<TVertex> CoveragePCCondition(IRandomWalk<TVertex> rw, int vertexCount, int loopCount, decimal coverage, int i) {
			var r = new CoverageTerminationCondition<TVertex>(rw, vertexCount, coverage);
			return r;
		}

	}




	public class StepsTerminationCondition<TVertex> : ITerminationConditions<TVertex> {

		private StepsTerminationConditionRW<TVertex> wrapped;

		public StepsTerminationCondition(IRandomWalk<TVertex> rw, decimal steps) {
			MetropolisRandomWalk<TVertex> mhrw = rw as MetropolisRandomWalk<TVertex>;
			mhrw = null;
			if (mhrw != null) {
				wrapped = new StepsTerminationConditionMHRW<TVertex>(mhrw, steps);
			} else {
				wrapped = new StepsTerminationConditionRW<TVertex>(rw, steps);
			}
		}


		public static StepsTerminationCondition<TVertex> IncrementalStepsConditions(IRandomWalk<TVertex> rw, int loopCount, decimal maxTarget, int i) {
			var r = new StepsTerminationCondition<TVertex>(rw, (decimal)(((decimal)(i + 1) / (decimal)loopCount) * (decimal)maxTarget));
			return r;
		}

		public static StepsTerminationCondition<TVertex> SublinearStepsCondition(IRandomWalk<TVertex> rw, int vertexCount, int loopCount, decimal minExponent, decimal maxTargetExponent, int i) {
			decimal step = (maxTargetExponent - minExponent) / (decimal)loopCount;


			var r = new StepsTerminationCondition<TVertex>(rw, (decimal)Math.Pow(vertexCount, (double)(minExponent + (i + 1) * step)));
			return r;
		}

		#region ITerminationConditions<TVertex,TEdge> Members

		public bool CheckCondition() {
			return wrapped.CheckCondition();
		}

		#endregion
	}

	public class StepsTerminationConditionMHRW<TVertex> : StepsTerminationConditionRW<TVertex>, ITerminationConditions<TVertex> {
		private new MetropolisRandomWalk<TVertex> Walk;

		public StepsTerminationConditionMHRW(MetropolisRandomWalk<TVertex> rw, decimal steps)
			: base(rw, steps) {
			this.Walk = rw;
		}
		public override bool CheckCondition() {
			return (this.Walk.DiscreetSteps - this.Walk.Loops) >= StepTarget;
		}
	}


	public class StepsTerminationConditionRW<TVertex> : ITerminationConditions<TVertex> {
		protected IRandomWalk<TVertex> Walk { get; set; }
		protected decimal StepTarget { get; set; }

		public StepsTerminationConditionRW(IRandomWalk<TVertex> rw, decimal steps) {

			Walk = rw;
			StepTarget = steps;
		}

		public virtual bool CheckCondition() {
			return Walk.DiscreetSteps >= StepTarget;
		}



	}

	public class TimeTerminationCondition<TVertex> : StepsTerminationConditionRW<TVertex> {
		public TimeTerminationCondition(IRandomWalk<TVertex> rw, decimal time)
			: base(rw, time) {
		}

		public override bool CheckCondition() {
			return Walk.TotalSteps >= StepTarget;
		}

	}


	public class RehitsTerminationCondition<TVertex> : StepsTerminationConditionRW<TVertex> {
		private int Rehits { get; set; }
		private RandomWalkRevisitObserver<TVertex> Observer;
		private int maxRehits = 0;

		public RehitsTerminationCondition(IRandomWalk<TVertex> rw, int rehits)
			: base(rw, 0) {
			Walk = rw;
			Rehits = rehits;
			Observer = new RandomWalkRevisitObserver<TVertex>(rw);
			Observer.ObservationEvent += new ObserverEvent<TVertex>(Observer_ObservationEvent);
		}

		void Observer_ObservationEvent(RandomWalkObserver<TVertex> sampler, TVertex previous, TVertex current, object ObservationParameters) {
			RandomWalkRevisitObserver<TVertex> obs = (RandomWalkRevisitObserver<TVertex>)sampler;
			if (obs.VisitSteps[current].Count > maxRehits)
				maxRehits = obs.VisitSteps[current].Count;
		}

		public override bool CheckCondition() {
			return maxRehits >= Rehits;
		}

		public static RehitsTerminationCondition<TVertex> IncrementalRehitsConditions(IRandomWalk<TVertex> rw, int initialTarget, int maxTarget, int loopCount, int i) {
			var r = new RehitsTerminationCondition<TVertex>(rw, initialTarget + (int)(((decimal)(i + 1) / (decimal)loopCount) * (decimal)(maxTarget - initialTarget)));
			return r;
		}
	}
}