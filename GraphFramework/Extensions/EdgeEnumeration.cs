using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;

namespace GraphFramework.Extensions {
	
	public class EdgeEnumeration<TVertex> : IEnumerable<TVertex> {

		private IAdjacencyGraph<TVertex> graph;

		public EdgeEnumeration(IAdjacencyGraph<TVertex> graph) { this.graph = graph; }

		public TVertex CurrentSource { get; private set; }

		public IEnumerator<TVertex> GetEnumerator() {
			foreach (var v in graph.Vertices) {
				CurrentSource = v;
				foreach (var e in graph.OutEdges(v)) {
					yield return e;
				}
			}
			yield break;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
	}

	
}
