using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Serializers;
using System.IO;
using DataStructures;
using GraphFramework.Interfaces;

namespace GraphFramework.Serializers {

	class TextGraphWriter<TVertex> : GenericGraphWriterBase<TVertex>, IGraphWriter<TVertex>
		where TVertex : IComparable<TVertex> {
		private StreamWriter stream;


		public TextGraphWriter(string file)
			: this(file, (int)Math.Pow(2, 11)) {
		}
		public TextGraphWriter(string file, int bufferSize) {
			stream = new StreamWriter(file, false, Encoding.ASCII, bufferSize);

		}


		~TextGraphWriter() {
			Dispose(false);
		}


		protected override void Dispose(bool disposing) {
			if (!base.disposed) {
				if (disposing) {
					stream.Dispose();
				}
				base.disposed = true;
			}
		}


		#region IGraphWriter Members

		protected override void WriteGraph(IAdjacencyGraph<TVertex> graph, bool asDirected) {
			int i = 0;
			OnProgressChanged(0, "Started");
			if (!graph.GetType().Equals(typeof(EdgeListUndirectedGraph<TVertex>))) {
				var enumerable = graph.EdgesTargets;
				var enumerator = enumerable.GetEnumerator();
				while (enumerator.MoveNext()) {
					if (enumerable.CurrentSource.CompareTo(enumerator.Current) >= 0) {
						stream.WriteLine(enumerable.CurrentSource + "\t" + enumerator.Current);
					}
					i++;
					if (i % 100 == 0) {
						OnProgressChanged((int)(((double)i / (double)graph.EdgeCount) * 100.0), "Working");
					}
				}
			
			} else {
				foreach (var e in graph.Edges) {					
					stream.WriteLine(e.Source + "\t" + e.Target);					
					i++;
					if (i % 100 == 0) {
						OnProgressChanged((int)(((double)i / (double)graph.EdgeCount) * 100.0), "Working");
					}
				}			
			}
			OnProgressChanged(100, "Finished");
		}

		
		public override void WriteNextPart(IDictionary<TVertex, IEdgeList<TVertex>> graph, bool directed) {
			int i = 0;
			OnProgressChanged(0, "Started");
			foreach (var kv in graph) {
				foreach (var v in kv.Value) {
					if (!directed && kv.Key.CompareTo(v) >= 0) {
						stream.WriteLine(kv.Key.ToString() + "\t" + v.ToString());
					} else if (directed) {
						stream.WriteLine(kv.Key.ToString() + "\t" + v.ToString());
					}
				}
				i++;
				if (i % 100 == 0) {
					OnProgressChanged((int)(((double)i / (double)graph.Count) * 100.0), "Working");
				}
			}
			OnProgressChanged(100, "Finished");
		}

		#endregion


	}

	class CSVGraphWriter<TVertex> : GenericGraphWriterBase<TVertex>, IGraphWriter<TVertex>
		where TVertex : IComparable<TVertex> {
		private StreamWriter stream;

		public CSVGraphWriter(string file)
			: this(file, (int)Math.Pow(2, 11)) {

		}
		public CSVGraphWriter(string file, int bufferSize) {
			stream = new StreamWriter(file, false, Encoding.ASCII, bufferSize);
		}

		~CSVGraphWriter() {
			Dispose(false);
		}


		protected override void Dispose(bool disposing) {
			if (!base.disposed) {
				if (disposing) {
					stream.Dispose();
				}
				base.disposed = true;
			}
		}

		#region IGraphWriter Members

	
		protected override void WriteGraph(IAdjacencyGraph<TVertex> ggraph, bool asDirected) {
			IVertexSet<TVertex> graph = ggraph as IVertexSet<TVertex>;
			if (graph != null) {
				int i = 0;
				OnProgressChanged(0, "Started");
				foreach (var v in graph.Vertices) {
					stream.Write(v.ToString());
					if (!asDirected) {
						foreach (var e in ggraph.AdjacentEdges(v)) {
							if (v.CompareTo(e) >= 0) {
								stream.Write("," + e.ToString());
							}
						}
					} else {
						foreach (var e in ggraph.AdjacentEdges(v)) {
							stream.Write("," + e.ToString());
						}
					}
					stream.WriteLine();
					i++;
					if (i % 100 == 0) {
						OnProgressChanged((int)(((double)i / (double)graph.VertexCount) * 100.0), "Working");
					}
				}
				OnProgressChanged(100, "Finished");
			}
		}

		
		public override void WriteNextPart(IDictionary<TVertex, IEdgeList<TVertex>> graph, bool directed) {
			int i = 0;
			OnProgressChanged(0, "Started");
			
			foreach (var kv in graph) {
				stream.Write(kv.Key.ToString());
				foreach (var v in kv.Value) {
					if (!directed && kv.Key.CompareTo(v) >= 0) {
						stream.Write("," + v.ToString());
					} else if (directed) {
						stream.Write("," + v.ToString());
					}
				}
				stream.WriteLine();				
				i++;
				if (i % 100 == 0) {
					OnProgressChanged((int)(((double)i / (double)graph.Count) * 100.0), "Working");
				}
			}
			OnProgressChanged(100, "Finished");
		}

		#endregion

		
	}

}
