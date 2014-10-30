using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStructures;
using GraphFramework.Interfaces;
using System.IO;

namespace GraphFramework.Algorithms {

	public static partial class GraphTrianglesExtensions {

		public static int GetContainingTrianglesCount<TVertex>(this IAdjacencyGraph<TVertex> graph) {
			var tc = 0;
			foreach (var v in graph.Vertices) { tc += graph.GetContainingTrianglesCount(v); }
			return tc/3;
		}
		public static int GetContainingTrianglesCount<TVertex>(this IAdjacencyGraph<TVertex> graph, TVertex vertex) {
			var tc = 0;
			foreach (var v in graph.AdjacentEdges(vertex)) { 
				if (v.Equals(vertex)) { continue; } 
				tc += graph.GetContainingTrianglesCount(vertex, v); 
			}
			return tc/2;
		}
		public static int GetContainingTrianglesCount<TVertex>(this IAdjacencyGraph<TVertex> graph, TVertex source, TVertex target) {
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
				tc += graph.NumberOfMultiEdges(source, v) * graph.NumberOfMultiEdges(target, v);
			}
			return tc;
		}


		
		
	}



}
