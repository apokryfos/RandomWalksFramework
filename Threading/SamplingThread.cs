using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RandomWalkFramework.RandomWalkInterface;

namespace RandomWalkFramework.Threading {
	public class SamplingThread<TVertex> : Sampler<TVertex> {
		private ManualResetEvent mre;
		private object syncRoot = new object();

		public override IRandomWalk<TVertex> RandomWalk {
			get {
				return base.RandomWalk;
			}
			protected set {
				lock (syncRoot) {
					base.RandomWalk = value;
				}
			}
		}

		public SamplingThread(IRandomWalk<TVertex> rw, ITerminationConditions<TVertex> c)
			: base(rw, c) {
			this.mre = new ManualResetEvent(false);

		}

		protected override void RandomWalk_Terminated(object sender, EventArgs e) {
			lock (syncRoot) {
				base.RandomWalk_Terminated(sender, e);
			}
			mre.Set();
		}

		public ManualResetEvent ResetFlag {
			get { return mre; }
		}

		protected override TVertex SampleOne() {
			lock (syncRoot) {
				return base.SampleOne();
			}
		}

	}
}
