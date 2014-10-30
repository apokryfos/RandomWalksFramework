using System;
using System.Collections.Generic;
using RandomWalkFramework.RandomWalkInterface;
using RandomWalkFramework.Weights;
using RandomWalkFramework.Analysis;
using GraphFramework.Interfaces;

namespace RandomWalkFramework.Threading {
	public class Sampler<TVertex> {

		public const int vcount = 500;

		public virtual IRandomWalk<TVertex> RandomWalk { get; protected set; }
		protected List<RandomWalkCumulativeLogger<TVertex>> loggers = new List<RandomWalkCumulativeLogger<TVertex>>();
		protected List<RandomWalkObserver<TVertex>> observers = new List<RandomWalkObserver<TVertex>>();
		protected ITerminationConditions<TVertex> terminationCondition;



		public virtual void AttachLoggers(LoggerType l, int[] MStepParameters, IUndirectedGraph<TVertex> graph, string logPath) {
			string NameBase = logPath + "\\" + RandomWalk.Name.Key + "-";

			if ((l & (LoggerType.STEP | LoggerType.STEP | LoggerType.MSTEP | LoggerType.HITS | LoggerType.RANDOMVARIABLE | LoggerType.HIDDENPARTITION | LoggerType.CYCLICFORMULA)) != 0) {
				var rwSO = new RandomWalkFrameworktepObserver<TVertex>(RandomWalk, (ulong)graph.VertexCount * 5);
				observers.Add(rwSO);

				if (l.HasFlag(LoggerType.STEP2)) {
					var rwsl = new RandomWalkNonLoopingStepLogger<TVertex>(rwSO, NameBase + "STEP.csv");
					loggers.Add(rwsl);
				}
				if (l.HasFlag(LoggerType.STEP)) {
					var rwsl = new RandomWalkFrameworktepLogger<TVertex>(rwSO, NameBase + "STEP.csv");
					loggers.Add(rwsl);
				}
				if (l.HasFlag(LoggerType.MSTEP) && MStepParameters != null && MStepParameters.Length > 0) {
					foreach (var M in MStepParameters) {
						var rwsl = new MetropolisRandomWalkFrameworktepLogger<TVertex>(rwSO, M, NameBase + "M" + M + "STEP.csv");
						loggers.Add(rwsl);
					}
				}
				if (l.HasFlag(LoggerType.HITS))
					loggers.Add(new RandomWalkHitsLogger<TVertex>(rwSO, NameBase + "HITS.csv"));

				if (l.HasFlag(LoggerType.RANDOMVARIABLE)) {
					Func<RandomWalkObserver<TVertex>, TVertex ,TVertex, double> function = ((rw, s,t) => 1.0 / (double)rw.Observed.GetStateWeight(s));
					loggers.Add(new RandomWalkRandomVariableIncrementalLogger<TVertex>(rwSO, NameBase + "CTRW.csv", function));
				}
				if (l.HasFlag(LoggerType.HIDDENPARTITION) && typeof(TVertex) == typeof(int))
					loggers.Add(new RandomWalkHiddenPartitionLogger<TVertex>(rwSO, NameBase + "HP.csv", (v => Convert.ToInt32(v) <= vcount)));

				if (l.HasFlag(LoggerType.CYCLICFORMULA) && typeof(TVertex) == typeof(int)) {
					RandomWalkCyclicFormulaStepsLogger<TVertex> cfrplog;
					if (RandomWalk.Name.Key.Contains("HPRW")) {
						cfrplog = new RandomWalkCyclicFormulaStepsLogger<TVertex>(rwSO, NameBase + "CFRP.csv", CFRPFunctions<TVertex>.GetFunctions(graph, RandomWalk.Name.Key, null));
					} else {
						cfrplog = new RandomWalkCyclicFormulaStepsLogger<TVertex>(rwSO, NameBase + "CFRP.csv", CFRPFunctions<TVertex>.GetFunctions(graph, RandomWalk.Name.Key));
					}
					loggers.Add(cfrplog);
				}

			}

			if (l.HasFlag(LoggerType.REVISITS)) {
				var rwRO = new RandomWalkRevisitObserver<TVertex>(RandomWalk);
				observers.Add(rwRO);
				loggers.Add(new RandomWalkRevisitsLogger<TVertex>(rwRO, NameBase + "REVISITS.csv"));
				//Estimator = new GraphWeightEstimation<TVertex>(rwRO, NameBase);
				// FEstimator = new MostVerticesRehitsDistribution<TVertex>(rwRO, NameBase);
			}
			if (l.HasFlag(LoggerType.DEGREECOVER)) {
				var rwDO = new RandomWalkDegreeCoverageObserver<TVertex>(RandomWalk, graph);
				observers.Add(rwDO);
				loggers.Add(new RandomWalkUndirectedDegreeCoverageLogger<TVertex>(rwDO, NameBase + "DCOVER.csv"));
			}

		}

		public Sampler(IRandomWalk<TVertex> rw, ITerminationConditions<TVertex> c) {
			this.RandomWalk = rw;
			terminationCondition = c;
			RandomWalk.Terminated += new EventHandler(RandomWalk_Terminated);
		}

		protected virtual void RandomWalk_Terminated(object sender, EventArgs e) {
			RandomWalk = null;
			foreach (var l in loggers) {
				l.Dispose();
			}
		}

		protected bool CheckCondition() {
			return terminationCondition.CheckCondition();
		}

		protected virtual TVertex SampleOne() {
			if (RandomWalk != null) {
				return RandomWalk.NextSample();
			} else {
				return default(TVertex);
			}
		}


		public virtual void Sample(object nothing) {
			bool conditionReached = false;
			while (!conditionReached) {
				var s = SampleOne();
				conditionReached = CheckCondition() || (s.Equals(default(TVertex)) && RandomWalk == null);
			}
			if (RandomWalk != null) {
				RandomWalk.Terminate();
			}

			foreach (var l in this.loggers) {
				l.Dispose();
			}

			foreach (var o in this.observers) {
				o.Dispose();
			}
		}
	}
}
