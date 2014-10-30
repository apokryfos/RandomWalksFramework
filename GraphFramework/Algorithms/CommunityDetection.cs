using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework;
using GraphFramework.Interfaces;
using GraphFramework.Algorithms.Framework;


namespace GraphFramework.Algorithms {
	public class Community : IEnumerable<int> {
		private HashSet<int> vertices;
		
		public int TotalEdges { get; private set; }
		public int InternalEdges { get; private set; }



		public Community() {
			this.vertices = new HashSet<int>();
		}

		public void AddVertex(int v, IEnumerable<int> adjacentVertices) {
			this.vertices.Add(v);
			foreach (var e in adjacentVertices) {
				if (CommunityContains(e))
					InternalEdges++;
				TotalEdges++;
			}
		}
		public void AddVertex(int v, int degree, int internalEdges) {
			vertices.Add(v);
			InternalEdges += internalEdges;
			TotalEdges += degree;

		}


		public void RemoveVertex(int v, IEnumerable<int> adjacentVertices) {
			if (CommunityContains(v)) {
				vertices.Remove(v);
				foreach (var e in adjacentVertices) {
					if (CommunityContains(e))
						InternalEdges--;
					TotalEdges--;
				}
			}
		}
		public void RemoveVertex(int v, int degree, int internalEdges) {
			if (CommunityContains(v)) {
				vertices.Remove(v);
				InternalEdges -= internalEdges;
				TotalEdges -= degree;
			}

		}

		public void MergeCommunities(Community c2, int crossEdges) {
			foreach (var v in c2)
				vertices.Add(v);
			TotalEdges += c2.TotalEdges;
			InternalEdges += c2.InternalEdges + crossEdges;

		}

		public bool CommunityContains(int vertex) {
			return vertices.Contains(vertex);
		}



		#region IEnumerable<int> Members

		public IEnumerator<int> GetEnumerator() {
			return vertices.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion
	}

	public abstract class CommunityDetection : AlgorithmBase<int> {

		protected IWeightedUndirectedGraph<Community, int> communityGraph = new WeightedUndirectedGraph<Community,int>();
		protected bool Finished { get; set; }

		public CommunityDetection(IUndirectedGraph<int> graph) : base(graph) { }

		
		public IEnumerable<Community> GetDetectedCommunities {
			get { return communityGraph.Vertices; }
		}

		
		public IUndirectedGraph<Community> CommunityCut {
			get { return communityGraph; }
		}
	
		
	}
}
