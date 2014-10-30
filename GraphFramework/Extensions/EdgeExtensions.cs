using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;

namespace GraphFramework.Extensions {
	public static class EdgeExtensions {


		

		public static TVertex GetOtherVertex<TVertex>(this Edge<TVertex> edge, TVertex thisVertex) {
			return (edge.Source.Equals(thisVertex)? edge.Target : edge.Source);
		}

		public static bool IsAdjacent<TVertex>(this Edge<TVertex> edge, TVertex vertex) {
			return (edge.Source.Equals(vertex) || edge.Target.Equals(vertex));
		}


		public static bool UndirectedVertexEquality<TVertex>(TVertex source, TVertex target, TVertex source2, TVertex target2) {
			return ((source.Equals(source2) && target.Equals(target2)) || (source.Equals(target2) && target.Equals(source2)));
		}

		public static bool DirectedVertexEquality<TVertex>(TVertex source, TVertex target, TVertex source2, TVertex target2) {
			return ((source.Equals(source2) && target.Equals(target2)));
		}


		public static EdgeEqualityComparer<TVertex> GetUndirectedVertexEquality<TVertex>() {
			return UndirectedVertexEquality;
		}
		public static EdgeEqualityComparer<TVertex> GetDirectedVertexEquality<TVertex>() {
			return DirectedVertexEquality;
		}

		public class EdgeComparer<TVertex> : IEqualityComparer<Edge<TVertex>> {

			public bool Equals(Edge<TVertex> x, Edge<TVertex> y) {
				if (object.Equals(x, y)) { return true; }				
				if (x.Directed != y.Directed) { return false; }
				
				if (x.Directed) {
					return DirectedVertexEquality<TVertex>(x.Source,x.Target,y.Source,y.Target);
				} else {
					return UndirectedVertexEquality<TVertex>(x.Source,x.Target,y.Source,y.Target);
				}
			}

			public int GetHashCode(Edge<TVertex> x) {
				return x.Source.GetHashCode() + x.Target.GetHashCode() + (x.Directed?0:1);
				
			}
		}
	}

}

