using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using RandomWalkFramework.RandomWalkInterface;
using RandomWalkFramework.Analysis;
using RandomWalkFramework.Querier;
using RandomWalkFramework.Weights;
using RandomWalkFramework.RNG;
using GraphFramework.Algorithms;
using DataStructures;
using System.Threading.Tasks;
using GraphFramework.Serializers;
using GraphFramework.Interfaces;
using GraphFramework;
using GraphFramework.Algorithms.ConnectedComponents;
using RandomWalkFramework;
using RandomWalkFramework.Threading;

namespace RandomWalksDeployment {


	public class Program {
		public static Random r = new MersenneTwister();

		
		public static IGraphQuerier<int> PreproccessWeightsCache(IUndirectedGraph<int> undg, WeightFunction<int> wf, KeyValuePair<string, string> policyName) {
			GeneralWeightedQuerier<int> gq = new WeightedGraphQuerier<int>(undg, wf, policyName);
			foreach (var v in undg.Vertices) {
				gq.VertexWeight(v);
			}
			return gq;
		}


		public static decimal WeightFun(IAdjacencyGraph<int> g, int s, int t) {
			return g.AdjacentDegree(s) + g.AdjacentDegree(t);
		}


		public static void Main(string[] args) {
			//Make a random graph (ER Graph)
			int n = 1000;
			double p = 0.01;
			var g = new UndirectedGraph<int>();
			for (int i = 0; i < n; i++) {
				g.AddVertex(i);
				for (int j = 0; j < n; j++) {
					if (r.NextDouble() <= p) {
						g.AddVerticesAndEdge(i, j);
					}
				}
			}

			//Get Connected components
			ConnectedComponentsAlgorithm<int> algo = new ConnectedComponentsAlgorithm<int>(g);
			algo.Compute();
			UndirectedGraph<int> largestwcc = new UndirectedGraph<int>();
			Dictionary<int, int> componentSizes = new Dictionary<int, int>();
			int maxCSize = 0;
			int maxC=0;
			for (int i = 0; i < algo.ComponentCount; i++) {
				componentSizes[i] = algo.Components.Values.Count(v => v == i);
				if (componentSizes[i] > maxCSize) {
					maxCSize = componentSizes[i];
					maxC = i;
				}
			}
			foreach (var v in g.Vertices) {
				if (algo.Components[v] == maxC) {
					largestwcc.AddVertexAndOutEdges(v, g.AdjacentEdges(v));
				}
			}


			SampleMain(largestwcc, ".", 10);
			
			
		}

	


		static List<KeyValuePair<Type, IGraphQuerier<int>>> SamplingMethods(IUndirectedGraph<int> undg) {
		
			

			List<KeyValuePair<Type, IGraphQuerier<int>>> dic = new List<KeyValuePair<Type, IGraphQuerier<int>>>();
			dic.Add(new KeyValuePair<Type, IGraphQuerier<int>>(
			    typeof(RandomWalk<int>), new WeightedGraphQuerier<int>(
			        undg,
			        VertexRandomWalkWeight.EdgeWeight<int>,
			        new KeyValuePair<string, string>("VRW", "Vertex Random Walk"))));

			dic.Add(new KeyValuePair<Type, IGraphQuerier<int>>(typeof(RandomWalk<int>), new UnweightedGraphQuerier<int>(undg)));
			return dic;
		}


		static void SampleMain(IUndirectedGraph<int> undg, string filePath, int loopCount) {
			//int dmax = undg.Vertices.Max(v => undg.AdjacentDegree(v));
			
			var dic = SamplingMethods(undg);
			ManualResetEvent[] mre = new ManualResetEvent[dic.Count];

			ThreadPool.SetMaxThreads(12, 12);

			int i = 0;
			foreach (var kv in dic) {
				Console.Write("Sampling using {0}.", kv.Value.PolicyName.Value);
				if (kv.Value.PolicyName.Key.Contains("HPRW")) {
					CFRPFunctions<int>.GetFunctions(undg, kv.Value.PolicyName.Key, null);
				} else {
					CFRPFunctions<int>.GetFunctions(undg, kv.Value.PolicyName.Key);
				}
				mre[i++] = new ManualResetEvent(false);
				var vertices = GetStartingVerticesForQuerier(undg, kv.Value);
				object[] parameters = new object[] { undg, kv, filePath, loopCount, mre[i-1], vertices };
				
				ThreadPool.QueueUserWorkItem(SampleMain, parameters);				
			}
			ManualResetEvent.WaitAll(mre);
		}

		private static void SampleMain(object o) {
			object[] parameters = (object[])o;
			var undg = parameters[0] as IUndirectedGraph<int>;
			var samplerType = (KeyValuePair<Type, IGraphQuerier<int>>)parameters[1];			
			SampleMain(undg, samplerType, parameters[2] as string, (int)parameters[3], parameters[5] as int[]);
			ManualResetEvent mre = parameters[4] as ManualResetEvent;
			mre.Set();
		}

		private static void SampleMain(IUndirectedGraph<int> undg, KeyValuePair<Type, IGraphQuerier<int>> samplerType, string filePath, int loopCount, int[] vertices) {
			List<int> startingVertices = new List<int>();
			for (int i = 0; i < 1; i++) {
				var v = vertices[r.Next(vertices.Length)];
				startingVertices.Add(v);
			}

			
			LoggerType l = LoggerType.NONE;
			int[] Ms = { };
			if (samplerType.Value.PolicyName.Key.Equals("SRW") || samplerType.Value.PolicyName.Key.StartsWith("BRW") || samplerType.Value.PolicyName.Key.Equals("VRW")) {
				l = l | LoggerType.DEGREECOVER;
			}
			if (!samplerType.Value.PolicyName.Key.StartsWith("BRW")) {
				l = (l|LoggerType.CYCLICFORMULA);
			} 

			if (samplerType.Value.GetType().Equals(typeof(ReducedGraphQuerier<int>))) {				
				l = (l|LoggerType.MSTEP);				
				Ms = new int[] { 100, 200, 500, 1000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000,20000,30000 };
			} 


			for (int i = 0; i < loopCount; i++) {
				if (!Directory.Exists(filePath + Path.DirectorySeparatorChar + "Loop" + i + Path.DirectorySeparatorChar))
					Directory.CreateDirectory(filePath + Path.DirectorySeparatorChar + "Loop" + i + Path.DirectorySeparatorChar);


				IRandomWalk<int> w;
				w = (IRandomWalk<int>)Activator.CreateInstance(samplerType.Key, vertices[r.Next(vertices.Length)], samplerType.Value);

				w.Initialize();
				//(samplerType.Key.Equals(typeof(RandomWalk<int>))?:new StepsTerminationCondition<int>(w, 100*vertexcount))
				
				
				ITerminationConditions<int> TC=null;
				if (samplerType.Value.PolicyName.Key.Equals("SRW") || samplerType.Value.PolicyName.Key.StartsWith("BRW")) {
					TC = CombinedTerminationCondition<int>.GetCombinationAny(false, new DegreeCoverageTerminationCondition<int>(w, undg, (int)Math.Pow(undg.VertexCount, 0.27)), new CoverageTerminationCondition<int>(w, undg.VertexCount, 0.4M), new StepsTerminationCondition<int>(w,undg.VertexCount*5));
				} else {
					TC = new StepsTerminationCondition<int>(w, undg.VertexCount * 5);
				}

				var Sampler = new Sampler<int>(
					w,
					TC
				);


				Sampler.AttachLoggers(l, Ms, undg, filePath + Path.DirectorySeparatorChar + "Loop" + i + Path.DirectorySeparatorChar);
				Sampler.Sample(null);
				lock (CSyncRoot) {
					Console.Write(i + ".");
				}
			}
		}

		private static int[] GetStartingVerticesForQuerier(IUndirectedGraph<int> undg, IGraphQuerier<int> gq) {
			int[] vertices;
			if (gq.GetType().Equals(typeof(ReducedGraphQuerier<int>))) {
				var rg = new UndirectedGraph<int>();
				int M = ((ReducedGraphQuerier<int>)gq).MaxDegree;
				Console.WriteLine("Walking on the reduced graph with max degree {0}", M);

				var enumerable = undg.EdgesTargets;
				foreach (var e in enumerable) {
					if (undg.AdjacentDegree(enumerable.CurrentSource) <= M && undg.AdjacentDegree(e) <= M) {
						rg.AddVerticesAndEdge(enumerable.CurrentSource, e);
					}
				}
				ConnectedComponentsAlgorithm<int> algo =
				new ConnectedComponentsAlgorithm<int>(rg);

				algo.Compute();


				Dictionary<int, int> componentSizes = new Dictionary<int, int>();
				if (algo.ComponentCount > 1) {
					foreach (var kv in algo.Components) {
						if (componentSizes.ContainsKey(kv.Value)) {
							componentSizes[kv.Value]++;
						} else {
							componentSizes.Add(kv.Value, 1);
						}
					}
				} else {
					componentSizes[0] = rg.VertexCount;
				}

				int maxC = -1;
				int maxCS = -1;
				foreach (var kv in componentSizes) {
					if (kv.Value > maxCS) {
						maxCS = kv.Value;
						maxC = kv.Key;
					}
				}
				vertices = algo.Components.Where(kv => kv.Value.Equals(maxC)).Select(kv => kv.Key).ToArray();
				Console.WriteLine("There are {0} in the largest component", vertices.Length);


			} else {				
				vertices = undg.Vertices.ToArray();
				Console.WriteLine("Walking on the entire graph");
				Console.WriteLine("There are {0} in the largest component", vertices.Length);
			}
			
			return vertices;
			
		}

		private static void SampleMain(IUndirectedGraph<int> undg, KeyValuePair<Type, IGraphQuerier<int>> samplerType, string filePath, int loopCount) {

			var vertices = GetStartingVerticesForQuerier(undg, samplerType.Value);
			int vertexcount = vertices.Length;
			SampleMain(undg, samplerType, filePath, loopCount, vertices);
			
		}
		static object CSyncRoot = new object();

		static void bgr_ProgressTick(string text, long currentProgress, long maxProgress) {
			if (r.NextDouble() < 0.05) {
				Console.SetCursorPosition(0, 0);
				Console.WriteLine("Reading...{0}%", Math.Round(100.0M * (decimal)currentProgress / (decimal)maxProgress, 2));
			}
		}
	}
}
