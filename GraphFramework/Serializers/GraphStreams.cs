using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using GraphFramework.Serializers;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using GraphFramework.Interfaces;



namespace GraphFramework.Serializers {



	public class GraphWriter<TVertex> : IGraphWriter<TVertex>
		where TVertex : IComparable<TVertex> {
		private IGraphWriter<TVertex> baseWriter;

		public GraphWriter(string file)
			: this(file, 1024) {			

		}
		public GraphWriter(string file, int bufferSize) {
			baseWriter = GenericGraphWriterFactory<TVertex>.GetGraphWriterForExtension(file, bufferSize);
		}

		~GraphWriter() {
			Dispose();
		}

		public void Dispose() {
			baseWriter.Dispose();
		}
		

		#region IGraphWriter<int,Edge<int> Members

		public void WriteNextPart(IDictionary<TVertex, IEdgeList<TVertex>> graph,bool directed) {
			baseWriter.WriteNextPart(graph,directed);
		}

		public event ProgressChangedEventHandler ProgressChanged {
			add { baseWriter.ProgressChanged += value; }
			remove { baseWriter.ProgressChanged -= value; }
		}

		public void WriteGraph(IAdjacencyGraph<TVertex> graph) {
			baseWriter.WriteGraph(graph);
		}

		#endregion


		public static void WriteGraph(IAdjacencyGraph<TVertex> graph, string filename) {			
			using (GraphWriter<TVertex> gw = new GraphWriter<TVertex>(filename)) {
				gw.WriteGraph(graph);
			}
		}




		public void WriteGraphAsUndirected(IBidirectionalGraph<TVertex> graph) {
			baseWriter.WriteGraphAsUndirected(graph);
		}

		public void WriteGraphAsUndirected(IUndirectedGraph<TVertex> graph) {
			baseWriter.WriteGraphAsUndirected(graph);
		}
	}


	public class GraphReader<TVertex> : IGraphReader<TVertex>
		where TVertex : IComparable<TVertex> {
		private IGraphReader<TVertex> baseReader;

		public GraphReader(string file, int bufferSize) {
			
			baseReader = GenericGraphReaderFactory<TVertex>.GetGraphReaderForExtension(file, bufferSize);
		}

		public GraphReader(string file)
			: this(file, 1024) {

		}

		~GraphReader() { Dispose(); }

		public void Dispose() {
			baseReader.Dispose();
		}


		public static IAdjacencyGraph<TVertex> ReadGraph(string filename) {
			using (GraphReader<TVertex> gw = new GraphReader<TVertex>(filename)) {
				return gw.ReadEntireGraph();
			}
		}

		public static IBidirectionalGraph<TVertex> ReadGraphAsBidirectional(string filename) {
			using (GraphReader<TVertex> gw = new GraphReader<TVertex>(filename)) {
				return gw.ReadEntireGraphAsBidirectional();
			}
		}

		public static IUndirectedGraph<TVertex> ReadGraphAsUndirected(string filename) {
			using (GraphReader<TVertex> gw = new GraphReader<TVertex>(filename)) {
				return gw.ReadEntireGraphAsUndirected();
			}
		}


		public event ProgressChangedEventHandler ProgressChanged {
			add { baseReader.ProgressChanged += value; }
			remove { baseReader.ProgressChanged -= value; }

		}

		public IAdjacencyGraph<TVertex> ReadEntireGraph() {
			return baseReader.ReadEntireGraph();
		}

		public IBidirectionalGraph<TVertex> ReadEntireGraphAsBidirectional() {
			return baseReader.ReadEntireGraphAsBidirectional();
		}

		
		public void ResetStream() {
			baseReader.ResetStream();
		}

		public long Position {
			get {
				return baseReader.Position;
			}
			set {
				baseReader.Position = value;
			}
		}

		public long Length {
			get { return baseReader.Length; }
		}

		public void Calibrate() {
			baseReader.Calibrate();
		}


		public IUndirectedGraph<TVertex> ReadEntireGraphAsUndirected() {
			return baseReader.ReadEntireGraphAsUndirected();
		}


		public IUndirectedGraph<TVertex> ReadUndirectedGraphFileToUndirectedGraph() {
			return baseReader.ReadUndirectedGraphFileToUndirectedGraph();
		}
	}

}
