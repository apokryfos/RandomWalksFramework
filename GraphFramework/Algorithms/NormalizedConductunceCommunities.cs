using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;


namespace GraphFramework.Algorithms {

	public class NormalizedConductunceCommunities : CommunityDetection {

		

		public NormalizedConductunceCommunities(IWeightedUndirectedGraph<int,int> VisitedGraph) : base(VisitedGraph) {			
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

		private double GetNormalizedConductunce(Community i) {
			return 0.0;

		}


		private double GetEij(Community i, Community j) {
			var w = communityGraph.GetWeight(i, j);			
			return (double)w / (double)VisitedGraph.EdgeCount;
			
		}

		private double GetAi(Community i) {
			return (double)(i.TotalEdges - i.InternalEdges) / (double)VisitedGraph.EdgeCount;
		}

		private double GetModularityDifference(Community source, Community target) {
			return GetEij(source, target) - (GetAi(source) * GetAi(target));
		}


		protected override void InternalCompute() {
			Finished = false;
			do {
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
				Finished = optimal;
			} while (!Finished);

		}



		private void MergeCommunities(Community move, Community moveTarget) {
			int crossEdges = communityGraph.GetWeight(move, moveTarget);
			moveTarget.MergeCommunities(move, crossEdges);


			foreach (var e in communityGraph.AdjacentEdges(move)) {
				var w = communityGraph.GetWeight(e, moveTarget);
				crossEdges += w;
			}
			communityGraph.SetWeight(move, moveTarget, crossEdges);
			communityGraph.RemoveVertex(move);
		}
	}
}
