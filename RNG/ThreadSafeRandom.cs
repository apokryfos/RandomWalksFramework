using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomWalkFramework.RNG {
	
	public sealed class ThreadSafeRandom : Random {
		private object SyncRoot = new object();
		private Random r = null;

		public ThreadSafeRandom() : base() { r = null; }
		public ThreadSafeRandom(int seed) : base(seed) { r = null; }
		public ThreadSafeRandom(Random enclosed) { r = enclosed; }

		
		public override int Next() {
			lock (SyncRoot) {
				if (r == null) { return base.Next(); } else { return r.Next(); }				
			}
		}
		public override int Next(int maxValue) {
			lock (SyncRoot) {
				if (r == null) { return base.Next(maxValue); } else { return r.Next(maxValue); }				
			}
		}
		public override int Next(int minValue, int maxValue) {
			lock (SyncRoot) {
				if (r == null) { return base.Next(minValue, maxValue); } else { return r.Next(minValue, maxValue); }								
			}
		}
		public override void NextBytes(byte[] buffer) {
			lock (SyncRoot) {
				if (r == null) { base.NextBytes(buffer); } else { r.NextBytes(buffer); }												
			}
		}
		public override double NextDouble() {
			lock (SyncRoot) {
				if (r == null) { return base.NextDouble(); } else { return r.NextDouble(); }		
			}
		}
	}
}
