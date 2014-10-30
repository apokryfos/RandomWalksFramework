using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomWalkFramework.Analysis{

	public enum LoggingMode { BINARY, TEXT } ;

	public abstract class RandomWalkProgressiveLogger<TVertex> : RandomWalkCumulativeLogger<TVertex> {
		protected override void LogCumilativeData() { }

		public RandomWalkProgressiveLogger(RandomWalkObserver<TVertex> obs, string logPath)
			: base(obs, logPath) {
			logger.OpenLogger();
		}

		public RandomWalkProgressiveLogger(RandomWalkObserver<TVertex> obs, string logPath, LoggingMode mode)
			: base(obs, logPath, mode) {
			logger.OpenLogger();
		}

		public override void Dispose() {
			logger.Dispose();
		}
	}

	public abstract class RandomWalkCumulativeLogger<TVertex> : IDisposable {

		protected IRandomWalkLogger<TVertex> logger;
		protected RandomWalkObserver<TVertex> obs;

		public RandomWalkCumulativeLogger(RandomWalkObserver<TVertex> obs, string logPath)
			: this(obs, logPath, LoggingMode.TEXT) {
		}


		public RandomWalkCumulativeLogger(RandomWalkObserver<TVertex> obs, string logPath, LoggingMode mode) {
			if (mode == LoggingMode.BINARY)
				logger = new RandomWalkBinaryLogger<TVertex>(logPath);
			else
				logger = new RandomWalkLogger<TVertex>(logPath);

			this.obs = obs;
			obs.ObservationEvent += new ObserverEvent<TVertex>(obs_ObservationEvent);
		}

		protected abstract void obs_ObservationEvent(RandomWalkObserver<TVertex> sampler, TVertex previous, TVertex current, object ObservationParameters);

		protected abstract void LogCumilativeData();

		public virtual void Dispose() {
			try {
				LogCumilativeData();
			} catch {

			} finally {
				logger.Dispose();
			}
		}
	}
}
