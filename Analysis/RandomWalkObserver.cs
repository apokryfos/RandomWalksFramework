using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomWalkFramework.RandomWalkInterface;

namespace RandomWalkFramework.Analysis {
	public delegate void ObserverEvent<TState>(RandomWalkObserver<TState> sampler, TState previous, TState current, object ObservationParameters);

	public abstract class RandomWalkObserver<TVertex> : IDisposable {
		public IRandomWalk<TVertex> Observed { get; set; }
		protected ulong truncate;

		public RandomWalkObserver(IRandomWalk<TVertex> observedSampler) {
			Observed = observedSampler;
			this.truncate = ulong.MaxValue;
			Observed.Step += new TransitionEvent<TVertex>(Observed_Transition);
		}
		public RandomWalkObserver(IRandomWalk<TVertex> observedSampler, ulong truncate) {
			Observed = observedSampler;
			this.truncate = truncate;
			Observed.Step += new TransitionEvent<TVertex>(Observed_Transition);

		}
		~RandomWalkObserver() {
			Dispose();
		}
		protected abstract void Observed_Transition(IRandomWalk<TVertex> sampler, TVertex previous, TVertex current, decimal weight);


		public event ObserverEvent<TVertex> ObservationEvent;

		protected void OnObservation(TVertex previous, TVertex current, object parameters) {
			var oe = ObservationEvent;
			if (oe != null && Observed.DiscreetSteps < truncate) {				
				oe(this, previous, current, parameters);				
			}
		}
		#region IDisposable Members
		public abstract void Dispose();
		#endregion
	}
}
