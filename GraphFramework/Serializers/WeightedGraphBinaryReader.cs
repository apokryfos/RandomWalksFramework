using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Serializers;
using System.IO;
using GraphFramework.Interfaces;
using GraphFramework.Extensions;

namespace GraphFramework.Serializers {
	
	
	
	public class WeightedGraphBinaryReader<TVertex, TTag> : GenericGraphReaderBase<TVertex>, IGraphReader<TVertex> {
		protected FileStream baseStream;
		protected BinaryReader stream;
		private string file;
		BlockToEdgeConverter bytesToEdges;
		int blockSize;
		int bufferSize;

		
		public delegate int BlockToEdgeConverter(byte[] block, int start, out Edge<TVertex> edge);
		
	

		public WeightedGraphBinaryReader(string file, BlockToEdgeConverter bytesToEdges, int blockSize)
			: this(file, bytesToEdges, blockSize, (int)Math.Pow(2, 16)) {
		}
		public WeightedGraphBinaryReader(string file, BlockToEdgeConverter bytesToEdges, int blockSize, int bufferSize) {
			baseStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
			stream = new BinaryReader(baseStream);
			this.file = file;
			this.bytesToEdges = bytesToEdges;
			this.blockSize = blockSize;
			this.bufferSize = bufferSize;
		}

		~WeightedGraphBinaryReader() {
			Dispose(false);
		}


		#region IGraphReader Members

		protected override void ReadEntireGraph(IAdjacencyGraph<TVertex> graph) {			
			bufferSize -= bufferSize % blockSize;
			byte[] buffer;						
			do {
				int i = 0;
				buffer = stream.ReadBytes(bufferSize);
				while (i < buffer.Length) {
					Edge<TVertex> e;
					var used = bytesToEdges(buffer, i, out e);
					if (used > 0) {
						graph.AddVerticesAndEdge(e.Source, e.Target);
						i += used;
					} else {
						i += blockSize;
					}
				}
			} while (baseStream.Position < baseStream.Length);
			OnProgressChanged((int)(((double)Position / (double)Length) * 100.0), "Working");
			
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
			baseStream.Position -= baseStream.Position % blockSize;
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
