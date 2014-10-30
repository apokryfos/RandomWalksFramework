using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStructures;
using GraphFramework.Interfaces;
using System.IO;

namespace GraphFramework.Algorithms {

	public static partial class GraphTrianglesExtensions {

		
		public static IEnumerable<TriangleGeneric<TVertex>> ListTriangles<TVertex>(this IAdjacencyGraph<TVertex> graph, TVertex vertex) {
			List<TriangleGeneric<TVertex>> triangles = new List<TriangleGeneric<TVertex>>();
			foreach (var v in graph.AdjacentEdges(vertex)) {
				if (v.Equals(vertex)) { continue; }
				triangles.AddRange(graph.ListTriangles(vertex,v));
			}
			for (int i = triangles.Count - 1; i >= 0; i++) {
				yield return triangles[i];
				var ind = triangles.FindIndex(0,i,t => t.Equals(triangles[i]));
				if (ind >= 0) { triangles.RemoveAt(ind); }
			}
		}
		public static IEnumerable<TriangleGeneric<TVertex>> ListTriangles<TVertex>(this IAdjacencyGraph<TVertex> graph, TVertex source, TVertex target) {
			List<TVertex> N1 = new List<TVertex>(graph.AdjacentEdges(source).Except(new TVertex[] { source }));
			N1.Remove(target);
			List<TVertex> N2 = new List<TVertex>(graph.AdjacentEdges(target).Except(new TVertex[] { target }));
			N2.Remove(source);
			List<TVertex> N3 = new List<TVertex>();
			N3.AddRange(N1.Where(v => N2.Contains(v)));
			N1.RemoveAll(v => N3.Contains(v));
			N3.AddRange(N2.Where(v => N1.Contains(v)));
			int tc = 0;
			foreach (var v in N3) {
				TriangleGeneric<TVertex> t = new TriangleGeneric<TVertex>(source, target, v);
				for (int i = 0; i < graph.NumberOfMultiEdges(source, v) * graph.NumberOfMultiEdges(target, v); i++) {
					yield return t;
				}				
			}
			
		}


		
		
	}



}
