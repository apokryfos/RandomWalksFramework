using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;
using GraphFramework.Algorithms;
using System.Threading;

namespace GraphFramework.Algorithms {
	public class TriangleCache<TVertex> {
		private static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
		public static IDictionary<IAdjacencyGraph<TVertex>, IDictionary<TVertex, int>> Cache = new Dictionary<IAdjacencyGraph<TVertex>, IDictionary<TVertex, int>>();

		private static IDictionary<TVertex,int> GetCache(IAdjacencyGraph<TVertex> Graph) {
			IDictionary<TVertex, int> cache;
			try {
				rwl.EnterUpgradeableReadLock();
				if (!Cache.TryGetValue(Graph, out cache)) {
					cache = new Dictionary<TVertex, int>();
					rwl.EnterWriteLock();
					Cache.Add(Graph, cache);
					rwl.ExitWriteLock();
				}
				return cache;
			} finally {
				rwl.ExitUpgradeableReadLock();
			}
			
		}

		public static int GetTriangleCount(IAdjacencyGraph<TVertex> Graph, TVertex vertex) {
			var cache = GetCache(Graph);
			bool res;
			int tc;
			lock (cache) {
				res = cache.TryGetValue(vertex, out tc);
			}
			if (!res) {
				tc = Graph.GetContainingTrianglesCount(vertex);
			}
			lock (cache) {
				res = cache.TryGetValue(vertex, out tc);
				if (!res) {
					cache.Add(vertex,tc);
				}
			}
			return tc;
			
			
		}
		

		public static void ClearCache(IAdjacencyGraph<TVertex> Graph) {
			if (Cache.ContainsKey(Graph)) {
				Cache[Graph].Clear();
				Cache.Remove(Graph);
			}
			
		}
		

	}
}
