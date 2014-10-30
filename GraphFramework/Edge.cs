using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Extensions;
using System.Diagnostics;

namespace GraphFramework {
	[DebuggerDisplay("{Source}->{Target}")]
	public struct Edge<TVertex> : IEquatable<Edge<TVertex>>, ICloneable  {

		private TVertex[] vertices;

		public bool Directed { get; private set; }

		public TVertex Source { get { return vertices[0]; } }
		public TVertex Target { get { return vertices[1]; } }

		public Edge(TVertex source, TVertex target)
			: this(source, target, false) {
		}
		
		public Edge(TVertex source, TVertex target, bool directed) : this() {			
			vertices = new TVertex[2]; 			
			vertices[0] = source;
			vertices[1] = target;
			Directed = directed;
		}

		public void Set(TVertex source, TVertex target, bool directed) {
			if (vertices == null) {
				vertices = new TVertex[2];
			} 
			vertices[0] = source; 
			vertices[1] = target; 
			Directed = directed; 
		}
		
		public bool Equals(Edge<TVertex> other) {
			if (Directed) { return EdgeExtensions.DirectedVertexEquality(vertices[0], vertices[1], other.vertices[0], other.vertices[1]); } 
			else { return EdgeExtensions.UndirectedVertexEquality(vertices[0], vertices[1], other.vertices[0], other.vertices[1]); }
		}

		public object Clone() {
			return new Edge<TVertex>(vertices[0], vertices[1], Directed);
		}
	}
}
