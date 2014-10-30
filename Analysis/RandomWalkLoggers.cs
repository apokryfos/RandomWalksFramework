using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RandomWalkFramework.Analysis {

	public class HitProperties<TVertex> {

		public TVertex VertexID { get; private set; }
		public decimal FirstHittingTime { get; private set; }
		public decimal LastHittingTime { get; private set; }
		public decimal Hits { get; private set; }
		public decimal Weight { get; private set; }
		public int Degree { get; private set; }


		public HitProperties(TVertex vertex, decimal currentStep)
			: this(vertex, currentStep, default(decimal), default(int)) {
		}

		public void Hit(decimal currentStep) {
			Hits++;
			LastHittingTime = currentStep;
		}

		public HitProperties(TVertex vertex, decimal currentStep, decimal weight, int degree) {
			VertexID = vertex;
			FirstHittingTime = currentStep;
			LastHittingTime = currentStep;
			Hits = 1;
			Weight = weight;
			Degree = degree;
		}



		public override string ToString() {
			return VertexID + "," + Weight + "," + Degree + "," + FirstHittingTime + "," + LastHittingTime + "," + Hits;
		}
	}

	[Flags]
	public enum LoggerType {
		NONE = 0,
		STEP = 1 << 0,
		REVISITS = 1 << 1,
		HITS = 1 << 2,
		DEGREECOVER = 1 << 3,
		RANDOMVARIABLE = 1 << 4,
		HIDDENPARTITION = 1 << 5,
		CYCLICFORMULA = 1 << 6,
		STEP2 = 1 << 7,
		MSTEP = 1 << 8,
		CYCLICFORMULA2 = 1 << 9
	}



	public class OldRandomWalkCyclicFormulaStepsLogger<TVertex> : RandomWalkProgressiveLogger<TVertex> {

		Func<TVertex, double>[] values;

		public OldRandomWalkCyclicFormulaStepsLogger(RandomWalkFrameworktepObserver<TVertex> obs, string logPath, params Func<TVertex, double>[] functions)
			: base(obs, logPath) {
			this.values = functions;
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler, TVertex previous, TVertex current, object ObservationParameters) {
			double[] results = new double[values.Length];
			for (int i = 0; i < values.Length; i++) {
				results[i] = values[i](current);
			}

			object[] objs = new object[] 
			{ 
				sampler.Observed.TotalSteps, 
				current, sampler.Observed.GetStateWeight(current), 
				sampler.Observed.GetTransitionWeight(previous, current), 
				(ObservationParameters!=null?ObservationParameters.ToString():null) 
			};
			logger.LogLine(objs.Concat(results.Select(d => (object)d)));
		}
	}

	public class RandomWalkCyclicFormulaStepsLogger<TVertex> : RandomWalkProgressiveLogger<TVertex> {

		Func<TVertex, double>[] values;

		public RandomWalkCyclicFormulaStepsLogger(RandomWalkFrameworktepObserver<TVertex> obs, string logPath, params Func<TVertex, double>[] functions)
			: base(obs, logPath) {
			this.values = functions;
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler, TVertex previous, TVertex current, object ObservationParameters) {
			double[] results = new double[values.Length];
			for (int i = 0; i < values.Length; i++) {
				results[i] = values[i](current);
			}

			List<object> objs = new List<object>();
			objs.Add(sampler.Observed.TotalSteps);
			objs.Add(current);
			objs.Add(sampler.Observed.GetStateWeight(current));  
			logger.LogLine(objs.Concat(results.Select(d => (object)d)));
		}
	}


	public class RandomWalkHiddenPartitionLogger<TVertex> : RandomWalkProgressiveLogger<TVertex> {

		Func<TVertex, bool> inPartition;

		public RandomWalkHiddenPartitionLogger(RandomWalkFrameworktepObserver<TVertex> obs, string logPath, Func<TVertex, bool> inPartition)
			: base(obs, logPath) {
			this.inPartition = inPartition;
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler,TVertex previous, TVertex current, object ObservationParameters) {
			int dS = 0;
			if (typeof(TVertex) == typeof(int)) {
				for (int i = 0; i < sampler.Observed.GetAdjacentTransitionCount(current); i++) {
					if (inPartition(sampler.Observed.GetAdjacentTransition(current, i)))
						dS++;
				}
			}

			object[] objs = new object[] { sampler.Observed.TotalSteps, current, sampler.Observed.GetStateWeight(current), sampler.Observed.GetTransitionWeight(previous,current), (ObservationParameters != null ? ObservationParameters.ToString() : null), dS };
			logger.LogLine(objs);
		}
	}

	public class MetropolisRandomWalkFrameworktepLogger<TVertex> : RandomWalkFrameworktepLogger<TVertex> {

		int T;

		public MetropolisRandomWalkFrameworktepLogger(RandomWalkFrameworktepObserver<TVertex> obs, int T, string logPath)
			: base(obs, logPath) {
			this.T = T;
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler,TVertex previous, TVertex current, object ObservationParameters) {
			if (sampler.Observed.TotalSteps % T == 0) {
				object[] objs = new object[] { sampler.Observed.TotalSteps, current, Math.Round(sampler.Observed.GetStateWeight(current), 3), Math.Round(sampler.Observed.GetTransitionWeight(previous,current), 3), (ObservationParameters != null ? ObservationParameters.ToString() : null) };
				logger.LogLine(objs);
			}
		}
	}

	public class RandomWalkNonLoopingStepLogger<TVertex> : RandomWalkFrameworktepLogger<TVertex> {
		public RandomWalkNonLoopingStepLogger(RandomWalkFrameworktepObserver<TVertex> obs, string logPath)
			: base(obs, logPath) {
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler,TVertex previous, TVertex current, object ObservationParameters) {
			if (!object.Equals(previous, current)) {
				object[] objs = new object[] { sampler.Observed.TotalSteps, current, sampler.Observed.GetStateWeight(current), sampler.Observed.GetTransitionWeight(previous,current), (ObservationParameters != null ? ObservationParameters.ToString() : null) };
				logger.LogLine(objs);
			}
		}
	}


	public class RandomWalkFrameworktepLogger<TVertex> : RandomWalkProgressiveLogger<TVertex> {
		public RandomWalkFrameworktepLogger(RandomWalkFrameworktepObserver<TVertex> obs, string logPath)
			: base(obs, logPath) {
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler,TVertex previous, TVertex current, object ObservationParameters) {
			object[] objs = new object[] { sampler.Observed.TotalSteps, current, sampler.Observed.GetStateWeight(current), sampler.Observed.GetTransitionWeight(previous,current), (ObservationParameters != null ? ObservationParameters.ToString() : null) };
			logger.LogLine(objs);
		}
	}


	public class RandomWalkRandomVariableIncrementalLogger<TVertex> : RandomWalkProgressiveLogger<TVertex> {
		Func<RandomWalkObserver<TVertex>, TVertex, TVertex, double> Function;


		public RandomWalkRandomVariableIncrementalLogger(RandomWalkFrameworktepObserver<TVertex> obs, string logPath, Func<RandomWalkObserver<TVertex>, TVertex, TVertex, double> function)
			: base(obs, logPath) {
			Function = function;
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler,TVertex previous, TVertex current, object ObservationParameters) {

			object[] objs = new object[] 
            { 
                sampler.Observed.TotalSteps, 
                current, 
                sampler.Observed.GetStateWeight(current),
                sampler.Observed.GetTransitionWeight(previous,current), 
                Function(sampler, previous, current) 
            };
			logger.LogLine(objs);

			//log.WriteLine(sampler.Observed.TotalSteps + "," + current + "," + sampler.Observed.GetStateWeight(current) + "," + sampler.Observed.GetTransitionWeight(previous,current) + "," + Function(sampler,current,transition));
		}

	}

	public class RandomWalkRevisitsLogger<TVertex> : RandomWalkCumulativeLogger<TVertex> {


		public RandomWalkRevisitsLogger(RandomWalkRevisitObserver<TVertex> obs, string logPath)
			: base(obs, logPath) {
		}

		protected override void LogCumilativeData() {
			RandomWalkRevisitObserver<TVertex> observer = (RandomWalkRevisitObserver<TVertex>)obs;
			foreach (var kv in observer.VisitSteps) {
				if (kv.Value.Count == 0)
					continue;


				List<object> objs = new List<object>();
				objs.Add(kv.Key);
				objs.Add(observer.StateWeights[kv.Key]);
				foreach (var v in kv.Value)
					objs.Add(v);

				logger.LogLine(objs);

				//log.Write(kv.Key+","+observer.StateWeights[kv.Key]);
				//foreach (var v in kv.Value)
				//    log.Write("," + v);
				//log.WriteLine();



			}
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler,TVertex previous, TVertex current, object ObservationParameters) {

		}
	}


	public class RandomWalkHitsLogger<TVertex> : RandomWalkCumulativeLogger<TVertex> {

		Dictionary<TVertex, HitProperties<TVertex>> hits;


		public RandomWalkHitsLogger(RandomWalkFrameworktepObserver<TVertex> obs, string logPath)
			: base(obs, logPath) {
			hits = new Dictionary<TVertex, HitProperties<TVertex>>();
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler,TVertex previous, TVertex current, object ObservationParameters) {
			HitProperties<TVertex> hp;
			if (!hits.TryGetValue(current, out hp)) {
				hp = new HitProperties<TVertex>(current, sampler.Observed.TotalSteps, sampler.Observed.GetStateWeight(current), sampler.Observed.GetAdjacentTransitionCount(current));
				hits.Add(current, hp);
			} else
				hp.Hit(sampler.Observed.TotalSteps);
		}


		protected override void LogCumilativeData() {


			foreach (var kv in hits) {
				object[] objs = new object[] 
                {
                    kv.Value.VertexID,
                    kv.Value.Weight,
                    kv.Value.Degree,
                    kv.Value.FirstHittingTime,
                    kv.Value.LastHittingTime,
                    kv.Value.Hits
                };
				logger.LogLine(objs);
			}


			//foreach (var kv in hits)
			//    log.WriteLine(kv.Value.ToString());
		}

	}


	public class RandomWalkUndirectedDegreeCoverageLogger<TVertex> : RandomWalkProgressiveLogger<TVertex> {

		public RandomWalkUndirectedDegreeCoverageLogger(RandomWalkDegreeCoverageObserver<TVertex> obs, string logPath)
			: base(obs, logPath) {
		}

		protected override void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler,TVertex previous, TVertex current, object ObservationParameters) {


			object[] objs = new object[] 
                {
                    sampler.Observed.TotalSteps,
                    current,ObservationParameters
                };
			logger.LogLine(objs);

			// log.WriteLine(sampler.Observed.TotalSteps + ","+current+","+ObservationParameters);           
		}

	}

}
