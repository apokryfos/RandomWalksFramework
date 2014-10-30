using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStructures;
using GraphFramework.Interfaces;
using System.IO;

namespace GraphFramework.Algorithms {

	public static partial class GraphTrianglesExtensions {

		public static double GetClusteringCoefficientCached<TVertex>(this IAdjacencyGraph<TVertex> graph, TVertex vertex) {
			var tc = TriangleCache<TVertex>.GetTriangleCount(graph, vertex);
			return ClusteringCoefficient(graph, vertex, tc);
		}

		private static double ClusteringCoefficient<TVertex>(IAdjacencyGraph<TVertex> graph, TVertex vertex, int triangleCount) {			
			var effective_degree = graph.AdjacentDegree(vertex) - graph.NumberOfMultiEdges(vertex, vertex);
			if (effective_degree <= 1) { return 0.0; }
			if ((graph as IUndirectedGraph<TVertex>) != null || (graph as IBidirectionalGraph<TVertex>) != null) {				
				return (2.0 * triangleCount) / (effective_degree * (effective_degree - 1));
			} else {
				return (triangleCount) / (effective_degree * (effective_degree - 1));
			}
		}

		public static double GetClusteringCoefficient<TVertex>(this IAdjacencyGraph<TVertex> graph, bool cached) {
			double cumulate = 0.0;
			double cnt = 0;
			foreach (var v in graph.Vertices) {
				double ci;
				if (cached) { 
					ci = GetClusteringCoefficientCached(graph, v); 
				} else { 
					ci = GetClusteringCoefficient(graph, v); 
				}
				if (double.IsNaN(ci)) { continue; }
				cumulate += ci;
				cnt++;
			}
			return cumulate / cnt;
		}
		public static double GetClusteringCoefficient<TVertex>(this IAdjacencyGraph<TVertex> graph, TVertex vertex) {
			int tc = graph.GetContainingTrianglesCount<TVertex>(vertex);
			return ClusteringCoefficient(graph, vertex, tc);

		}

		
		
	}



}
