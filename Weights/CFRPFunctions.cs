using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework;
using GraphFramework.Algorithms;
using GraphFramework.Interfaces;
using System.Threading;
using DataStructures;

namespace RandomWalkFramework.Weights {
	public class CFRPFunctions<TVertex> {

		private ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
		private IUndirectedGraph<TVertex> targetGraph;
		public CFRPFunctions(IUndirectedGraph<TVertex> targetGraph) {
			this.targetGraph = targetGraph;		
		}
		
		public double TriangleCount(TVertex v) {
			return TriangleCache<TVertex>.GetTriangleCount(targetGraph, v);						
		}

		public double Degree(TVertex v) {
			return targetGraph.AdjacentDegree(v);
		}

		public double NumVertices(TVertex v) {
			return 1.0;
		}

		public double NumVerticesIn(TVertex v, Func<TVertex, bool> InSet) {
			return (InSet(v) ? 1 : 0);
		}

		public double NumEdgesIn(TVertex v, Func<TVertex, bool> InSet) {
			return (InSet(v) ? targetGraph.AdjacentEdges(v).Where(e => InSet(e)).Count() : 0);
		}
		
		public double NumEdgesCross(TVertex v, Func<TVertex, bool> InSet) {
			return (InSet(v) ? targetGraph.AdjacentDegree(v) : 0);
		}

		public double ClusteringCoefficient(TVertex v) {
			return (2.0*(double)TriangleCount(v)) / (double)(targetGraph.AdjacentDegree(v)*(targetGraph.AdjacentDegree(v)-1));
		}


		private static object SyncRoot = new object();
		private static Dictionary<IUndirectedGraph<TVertex>, CFRPFunctions<TVertex>> cache = new Dictionary<IUndirectedGraph<TVertex>, CFRPFunctions<TVertex>>();


		public static Func<TVertex, double>[] GetFunctions(IUndirectedGraph<TVertex> targetGraph, string walk) {
			CFRPFunctions<TVertex> t;
			lock (SyncRoot) {
				if (!cache.TryGetValue(targetGraph, out t)) {
					t = new CFRPFunctions<TVertex>(targetGraph);
					cache.Add(targetGraph, t);
				}
			}
			if (walk.Contains("SRW")) {
				return new Func<TVertex, double>[] { t.TriangleCount, t.NumVertices };
			} else {
				return new Func<TVertex, double>[] { t.Degree, t.TriangleCount, t.NumVertices };
			}
		}

		public static Func<TVertex, double>[] GetFunctions(IUndirectedGraph<TVertex> targetGraph, string walk, Func<TVertex, bool> InSet) {
			CFRPFunctions<TVertex> t;
			lock (SyncRoot) {
				if (!cache.TryGetValue(targetGraph, out t)) {
					t = new CFRPFunctions<TVertex>(targetGraph);
					cache.Add(targetGraph, t);
				}
			}
			if (InSet == null) {
				try {
					t.rwl.EnterWriteLock();
					t.GenerateInSetFunctionBasedOnGraph(1000);
				} finally {
					t.rwl.ExitWriteLock();
				}
				InSet = t.InSet;
			}
			if (walk.Contains("SRW")) {
				return new Func<TVertex, double>[] { t.TriangleCount, t.NumVertices, (v => t.NumVerticesIn(v, InSet)), (v => t.NumEdgesIn(v, InSet)), (v => t.NumEdgesCross(v, InSet)) };
			} else {
				return new Func<TVertex, double>[] { t.Degree, t.TriangleCount, t.NumVertices, (v => t.NumVerticesIn(v, InSet)), (v => t.NumEdgesIn(v, InSet)), (v => t.NumEdgesCross(v, InSet)) };
			}
		}

		public bool InSet(TVertex vertex) {
			if (top10 == null) { GenerateInSetFunctionBasedOnGraph(1000); }
			return top10.Contains(vertex);
		}

		HashSet<TVertex> top10 = null;

		private void GenerateInSetFunctionBasedOnGraph(int sizeMax) {
			Random r = new Random();
			top10 = new HashSet<TVertex>();
			SortedDictionary<int, List<TVertex>> degrees = new SortedDictionary<int, List<TVertex>>(ComparerFromComparison<int>.Create((d1,d2) => d2.CompareTo(d1)));
			foreach (var v in targetGraph.Vertices) {
				List<TVertex> cl;
				if (!degrees.TryGetValue(targetGraph.AdjacentDegree(v), out cl)) {
					cl = new List<TVertex>();
					degrees.Add(targetGraph.AdjacentDegree(v), cl);
				}
				cl.Add(v);
			}


			foreach (var d in degrees.Take(10)) {
				foreach (var v in d.Value) { top10.Add(v); }
			}

			while (top10.Count < sizeMax) {
				var v = top10.ElementAt(r.Next(top10.Count));
				top10.Add(targetGraph.AdjacentEdge(v, r.Next(targetGraph.AdjacentDegree(v)))); 
			}
			

		}

	}
}
