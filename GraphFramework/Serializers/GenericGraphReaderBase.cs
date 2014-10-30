using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Serializers;
using System.ComponentModel;
using GraphFramework.Interfaces;

namespace GraphFramework.Serializers {
	public abstract class GenericGraphReaderBase<TVertex> : IGraphReader<TVertex> {


		protected bool disposed = false;

		protected void Merge(IAdjacencyGraph<TVertex> graph, IDictionary<TVertex, IEdgeList<TVertex>> part) {
			foreach (var kv in part) {
				graph.AddVertexAndOutEdges(kv.Key, kv.Value);
			}
		}

		#region IDisposable Members
		public void Dispose() {
			Dispose(true);			
			GC.SuppressFinalize(this);
		}
		protected abstract void Dispose(bool disposing);

		#endregion

		#region IGraphReader<TVertex,TEdge> Members


		protected abstract void ReadEntireGraph(IAdjacencyGraph<TVertex> g);

		public IUndirectedGraph<TVertex> ReadUndirectedGraphFileToUndirectedGraph() {
			var g = ReadEntireGraph();
			return new SymmetricToUndirectedAdapter<TVertex>(g);
		}

		public IAdjacencyGraph<TVertex> ReadEntireGraph() {
			var g = new AdjacencyGraph<TVertex>();
			ReadEntireGraph(g);
			return g;
		}

		public IBidirectionalGraph<TVertex> ReadEntireGraphAsBidirectional() {
			var g = new BidirectionalGraph<TVertex>();
			ReadEntireGraph(g);
			return g;
		}

		public IUndirectedGraph<TVertex> ReadEntireGraphAsUndirected() {
			var g = new UndirectedGraph<TVertex>();
			ReadEntireGraph(g);
			return g;
		}

		public abstract void ResetStream();

		public abstract long Position {
			get;
			set;
		}

		public abstract long Length {
			get;
		}

		public abstract void Calibrate();

		public event ProgressChangedEventHandler ProgressChanged;

		

		#endregion



		protected void OnProgressChanged(int percentage, object state) {
			if (ProgressChanged != null) {
				ProgressChanged(this, new ProgressChangedEventArgs(percentage, state));
			}
		}
		
	}
}
