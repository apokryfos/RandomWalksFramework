using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphFramework.Interfaces {

	public delegate bool EdgePredicate<TVertex>(TVertex source, TVertex target);
	public delegate bool VertexPredicate<TVertex>(TVertex vertex);
	public delegate void EdgeAction<TVertex>(IGraph<TVertex> graph, TVertex source, TVertex target);
	public delegate void WeightedEdgeAction<TVertex, TWeight>(IGraph<TVertex> graph, TVertex source, TVertex target, TWeight weight);
	public delegate void VertexAction<TVertex>(IGraph<TVertex> graph, TVertex vertex);
	public delegate bool EdgeEqualityComparer<TVertex>(TVertex source, TVertex target, TVertex source2, TVertex target2);

	public interface IGraph<TVertex> : ICloneable {

		#region Basics
		bool IsDirected { get; }
		bool AllowParallelEdges { get; }
		bool IsMutable { get; }
		#endregion

	}
}
