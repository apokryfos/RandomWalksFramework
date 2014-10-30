using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GraphFramework.Interfaces;
using GraphFramework.Extensions;

namespace GraphFramework.Serializers {

	public struct BinaryGraphFileConstants {
		public const int EndOfLine = -1;
		public const int EndOfLine2 = -2;
		public const int Empty = -3;
	}

	public class BinaryGraphReader : GenericGraphReaderBase<int>, IGraphReader<int> {
		protected FileStream baseStream;
		protected BinaryReader stream;
		private string file;
		private int bufferSize;



		public BinaryGraphReader(string file)
			: this(file, (int)Math.Pow(2, 25)) {
		}
		public BinaryGraphReader(string file, int bufferSize) {
			baseStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
			stream = new BinaryReader(baseStream);
			this.file = file;
			this.bufferSize = bufferSize;
		}

		~BinaryGraphReader() {
			Dispose(false);
		}


		#region IGraphReader Members

		protected override void ReadEntireGraph(IAdjacencyGraph<int> graph)  {
			int? src = null;
			IEdgeList<int> current = null;				
				
			do {
				byte[] buffer;
				buffer = stream.ReadBytes(bufferSize * sizeof(int));
				for (int i = 0; i < buffer.Length; i += sizeof(int)) {
					int cv = BitConverter.ToInt32(buffer, i);
					if (cv == BinaryGraphFileConstants.Empty) { continue; }
					if (cv == BinaryGraphFileConstants.EndOfLine || cv == BinaryGraphFileConstants.EndOfLine2) {
						src = null;
						continue;
					}

					if (!src.HasValue) {
						src = cv;
						if (current != null) {
							graph.AddVertexAndOutEdges(src.Value, current);
						}
						current = GraphExtensions.GetEdgeListInstance<int>();
						continue;
					}
					current.Add(cv);
				}
				OnProgressChanged((int)(((double)Position / (double)Length) * 100.0), "Working");	
			} while (baseStream.Position < baseStream.Length);
			if (current != null && src.HasValue) {
				graph.AddVertexAndOutEdges(src.Value, current);
			}
		}
		
		public override void ResetStream() {
			baseStream.Seek(0, SeekOrigin.Begin);
		}

		public override long Position {
			get {
				return baseStream.Position;
			}
			set {
				baseStream.Position = Math.Min(value, Length);
				Calibrate();
			}
		}

		public override long Length {
			get { return baseStream.Length; }
		}

		public override void Calibrate() {
			baseStream.Position -= baseStream.Position % sizeof(int);
			if (Position < Length && Position >= sizeof(int)) {				
				baseStream.Position -= sizeof(int);
				int next = stream.ReadInt32();
				while (next != BinaryGraphFileConstants.EndOfLine && next != BinaryGraphFileConstants.EndOfLine2 && Position < Length)
					next = stream.ReadInt32();
			}
		}

		#endregion

		#region IDisposable Members

		protected override void Dispose(bool disposing) {
			if (!base.disposed) {
				if (disposing) {
					stream.Dispose();
				}
				base.disposed = true;
			}
		}
		#endregion

	}

	public class BinaryGraphWriter : GenericGraphWriterBase<int>, IGraphWriter<int> {
		private FileStream baseStream;
		private BinaryWriter stream;


		public BinaryGraphWriter(string file)
			: this(file, 1024) {
		}
		public BinaryGraphWriter(string file, int bufferSize) {
			baseStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);
			stream = new BinaryWriter(baseStream);
		}

		~BinaryGraphWriter() {

			Dispose(false);
		}

		#region IGraphWriter Members

		public override void WriteNextPart(IDictionary<int, IEdgeList<int>>  graph, bool directed) {
			int i = 0;
			OnProgressChanged(0, "Started");			
			foreach (var kv in graph) {
				IEdgeList<int> l;
				if (!directed) {
					l = GraphExtensions.GetEdgeListInstance<int>(kv.Value.Where(v => v >= kv.Key));
				} else {
					l = GraphExtensions.GetEdgeListInstance<int>(kv.Value);
				}
				stream.Write(kv.Key);
				foreach (var v in l) { stream.Write(v); }
				stream.Write(BinaryGraphFileConstants.EndOfLine);
				i++;
				if (i % 100 == 0) {
					OnProgressChanged((int)(((double)i / (double)graph.Count) * 100.0), "Working");
				}
			}
			OnProgressChanged(100, "Finished");

		}

		protected override void WriteGraph(IAdjacencyGraph<int> graph, bool asDirected) {

			var vgraph = graph as IVertexSet<int>;
			
			int i = 0;
			OnProgressChanged(0, "Started");
			foreach (var v in vgraph.Vertices) {
				IEdgeList<int> l;
				if (asDirected) {
					l = GraphFramework.Extensions.GraphExtensions.GetEdgeListInstance<int>(graph.OutEdges(v));
				} else {
					l = GraphFramework.Extensions.GraphExtensions.GetEdgeListInstance<int>(graph.AdjacentEdges(v).Where(e => e >= v));
				}
				stream.Write(v);
				foreach (var e in l) { stream.Write(e); }
				stream.Write(BinaryGraphFileConstants.EndOfLine);
				i++;
				if (i % 100 == 0) {
					OnProgressChanged((int)(((double)i / (double)vgraph.VertexCount) * 100.0), "Working");
				}
			}
			OnProgressChanged(100, "Finished");
		}

		#endregion

		#region IDisposable Members

		protected override void Dispose(bool disposing) {
			if (!base.disposed) {
				if (disposing) {
					stream.Dispose();
				}
				base.disposed = true;
			}
		}
		#endregion




		
	}
}
