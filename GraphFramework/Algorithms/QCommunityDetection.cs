using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;


namespace GraphFramework.Algorithms {
	class QCommunityDetection : CommunityDetection {



		public QCommunityDetection(IUndirectedGraph<int> VisitedGraph)
			: base(VisitedGraph) {

		}


		public override void Initialize() {
			Dictionary<int, Community> addedVertices = new Dictionary<int, Community>();
			foreach (var e in VisitedGraph.Edges) {
				Community source, target;
				if (!addedVertices.TryGetValue(e.Source, out source)) {
					source = new Community();
					source.AddVertex(e.Source, VisitedGraph.AdjacentDegree(e.Source), 0);
					addedVertices.Add(e.Source, source);
				}
				if (!addedVertices.TryGetValue(e.Target, out target)) {
					target = new Community();
					target.AddVertex(e.Target, VisitedGraph.AdjacentDegree(e.Target), 0);
					addedVertices.Add(e.Target, target);
				}
				communityGraph.AddVerticesAndEdge(source, target);
				communityGraph.SetWeight(source, target, 1);
			}

		}

		public double GetModularity() {
			double cs = 0.0;
			foreach (var v in communityGraph.Vertices)
				cs += ((double)v.InternalEdges / (double)VisitedGraph.EdgeCount) - Math.Pow(GetAi(v), 2);

			return cs;
		}


		private double GetEij(Community i, Community j) {
			return (double)communityGraph.GetWeight(i,j) / (double)VisitedGraph.EdgeCount;
			
		}

		private double GetAi(Community i) {
			return (double)(i.TotalEdges - i.InternalEdges) / (double)VisitedGraph.EdgeCount;
		}

		private double GetModularityDifference(Community source, Community target) {
			return GetEij(source, target) - (GetAi(source) * GetAi(target));
		}


		private bool CommunityMergePhase = true;



	

		protected override void InternalCompute() {
			do {
				if (CommunityMergePhase) {
					if (CommunityMerge())
						CommunityMergePhase = false;
				} else {
					if (Optimization())
						Finished = true;
				}
				Finished = false;
			} while (!Finished);

		}

		protected bool Optimization() {
			
			int currentMax = -1;
			Community maxS=null, maxT=null;

			var enumerable = communityGraph.EdgesTargets;


			foreach (var e in enumerable) {
				var w = communityGraph.GetWeight(enumerable.CurrentSource, e);
				if (w < currentMax) {
					currentMax = w;
					maxS = enumerable.CurrentSource;
					maxT = e;
				}
			}

			int maxCross1 = 0;
			int inner1 = -1;
			int maxV1 = -1;
			foreach (var v in maxS) {
				int n = VisitedGraph.AdjacentEdges(v).Where(e => maxT.CommunityContains(e)).Count();
				int n2 = VisitedGraph.AdjacentEdges(v).Where(e => maxS.CommunityContains(e)).Count();
				double r = (double)(n - n2) / (double)VisitedGraph.AdjacentDegree(v);

				if (maxV1 == -1 || r > (double)(maxCross1 - inner1) / (double)VisitedGraph.AdjacentDegree(maxV1)) {
					maxV1 = v;
					inner1 = n2;
					maxCross1 = n;
				}
			}

			int maxCross2 = 0;
			int inner2 = 0;
			int maxV2 = -1;
			foreach (var v in maxT) {

				int n = VisitedGraph.AdjacentEdges(v).Where(e => maxS.CommunityContains(e)).Count();
				int n2 = VisitedGraph.AdjacentEdges(v).Where(e => maxT.CommunityContains(e)).Count();
				double r = (double)(n - n2) / (double)VisitedGraph.AdjacentDegree(v);

				if (maxV2 == -1 || r > (double)(maxCross2 - inner2) / (double)VisitedGraph.AdjacentDegree(maxV2)) {
					maxV2 = v;
					inner2 = n2;
					maxCross2 = n;
				}
			}

			int gain = (maxCross1 + maxCross2) - (inner1 + inner2);
			gain -= (VisitedGraph.ContainsEdge(maxV1, maxV2) ? 1 : 0);

			if (gain > 0) {
				maxT.AddVertex(maxV1, VisitedGraph.AdjacentDegree(maxV1), maxCross1 - (VisitedGraph.ContainsEdge(maxV1, maxV2) ? 1 : 0));
				maxS.AddVertex(maxV2, VisitedGraph.AdjacentDegree(maxV2), maxCross2 - (VisitedGraph.ContainsEdge(maxV1, maxV2) ? 1 : 0));
				communityGraph.SetWeight(maxS,maxT, currentMax - gain);

				maxS.RemoveVertex(maxV1, VisitedGraph.AdjacentDegree(maxV1), inner1);
				maxT.RemoveVertex(maxV2, VisitedGraph.AdjacentDegree(maxV2), inner2);
				return false;
			}


			return true;

		}

		protected bool CommunityMerge() {

			bool optimal = true;
			double mdiff = 0.0;
			Community c1 = null, c2 = null;

			foreach (var e in communityGraph.Edges) {
				double pending_mdiff = GetModularityDifference(e.Source, e.Target);
				if (pending_mdiff > mdiff) {
					c1 = e.Source;
					c2 = e.Target;
					mdiff = pending_mdiff;
					optimal = false;
				}
			}


			if (mdiff > 0.0)
				MergeCommunities(c1, c2);

			return optimal;

		}



		private void MergeCommunities(Community move, Community moveTarget) {
			var crossEdges = communityGraph.GetWeight(move, moveTarget);			
			moveTarget.MergeCommunities(move, crossEdges);

			foreach (var e in communityGraph.AdjacentEdges(move)) {
				var w = communityGraph.GetWeight(e, moveTarget);
				communityGraph.SetWeight(e,moveTarget, w + communityGraph.GetWeight(move,e));
			}
			communityGraph.RemoveVertex(move);

		}


	}
}
