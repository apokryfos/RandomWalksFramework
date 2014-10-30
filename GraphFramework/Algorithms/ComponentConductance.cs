using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;


namespace GraphFramework.Algorithms {
	public static class ComponentConductance {

		public static double GetSetConductance<TVertex>(this IUndirectedGraph<TVertex> graph, HashSet<TVertex> vertices) {			
			int outgoing=0;
			int total = 0;
			foreach (var v in vertices) {
				foreach (var e in graph.AdjacentEdges(v)) {
					if (!vertices.Contains(e)) {
						outgoing++;
					}
					total++;
				}

			}
			return (double)outgoing / (double)total;
		}

		public static double GetVertexNeigbourhoodConductance<TVertex>(this IUndirectedGraph<TVertex> graph, TVertex vertex) {
				HashSet<TVertex> neighborhood = new HashSet<TVertex>(graph.AdjacentEdges(vertex));
				neighborhood.Add(vertex);
				return GetSetConductance(graph, neighborhood);
		}

	}
}
