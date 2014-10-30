using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphFramework.Interfaces {
	public interface IVertexSet<TVertex> : IGraph<TVertex> {
		#region Vertex Operations
		event VertexAction<TVertex> VertexAdded;
		bool AddVertex(TVertex v);
		int AddVertexRange(IEnumerable<TVertex> vertices);
		event VertexAction<TVertex> VertexRemoved;
		bool RemoveVertex(TVertex v);
		int RemoveVertexIf(VertexPredicate<TVertex> pred);
		bool ContainsVertex(TVertex vertex);
		bool IsVerticesEmpty { get; }
		int VertexCount { get; }
		IEnumerable<TVertex> Vertices { get; }
		#endregion
	}
}
