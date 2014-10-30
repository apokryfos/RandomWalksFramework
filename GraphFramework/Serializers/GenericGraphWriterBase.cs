using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Serializers;
using GraphFramework.Extensions;
using System.ComponentModel;
using GraphFramework.Interfaces;

namespace GraphFramework.Serializers {
	public abstract class GenericGraphWriterBase<TVertex> : IGraphWriter<TVertex>
		where TVertex : IComparable<TVertex> {


		protected bool disposed = false;
			
		
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected abstract void Dispose(bool disposing);

		public event ProgressChangedEventHandler ProgressChanged;

		#endregion

		protected void OnProgressChanged(int percentage, object state) {
			if (ProgressChanged != null) {
				ProgressChanged(this, new ProgressChangedEventArgs(percentage, state));
			}
		}
		
		#region IGraphWriter<TVertex,TEdge> Members
		protected abstract void WriteGraph(IAdjacencyGraph<TVertex> graph, bool asDirected);

		public void WriteGraph(IAdjacencyGraph<TVertex> graph) {
			WriteGraph(graph, graph.IsDirected);
		}
		public abstract void WriteNextPart(IDictionary<TVertex, IEdgeList<TVertex>> graph, bool directed);
		
		public void WriteGraphAsUndirected(IBidirectionalGraph<TVertex> graph) {
			WriteGraph(graph, false);
		}
		public void WriteGraphAsUndirected(IUndirectedGraph<TVertex> graph) {
			WriteGraph(graph, false);
		}
		
		#endregion
	}
}
