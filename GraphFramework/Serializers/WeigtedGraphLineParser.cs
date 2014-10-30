using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Serializers;
using System.IO;
using GraphFramework.Interfaces;
using GraphFramework.Extensions;

namespace GraphFramework.TextGraphStreams {

	
	public class WeigtedGraphLineParser<TVertex, TTag> : GenericGraphReaderBase<TVertex>, IGraphReader<TVertex> {

		private StreamReader stream;
		private int bufferSize;
		private LineToEdgesConverter edgeFactory;


		public delegate Edge<TVertex>[] LineToEdgesConverter(string line);


		private WeigtedGraphLineParser() { }

		public WeigtedGraphLineParser(string file, LineToEdgesConverter edgeFactoryFromLine)
			: this(file, edgeFactoryFromLine, (int)Math.Pow(2, 14)) {
		}

		public WeigtedGraphLineParser(string file, LineToEdgesConverter edgeFactoryFromLine, int bufferSize)
			: base() {
			edgeFactory = edgeFactoryFromLine;
			this.bufferSize = bufferSize;
			stream = new StreamReader(file,Encoding.ASCII, false, bufferSize);
				 
		}

		protected override void Dispose(bool disposing) {
			if (!base.disposed) {
				if (disposing) {
					stream.Dispose();
				}
				base.disposed = true;
			}
		}

		protected override void ReadEntireGraph(IAdjacencyGraph<TVertex> graph) {
			if (stream.EndOfStream) { return; }

			
			IEdgeList<TVertex> current;
			int count = 0;
			do {
				string s = stream.ReadLine();
				var edges = edgeFactory(s);
				graph.AddVerticesAndEdgeRange(edges);
			} while (!stream.EndOfStream && count < bufferSize);
			OnProgressChanged((int)(((double)Position / (double)Length) * 100.0), "Working");
		}



		public override void ResetStream() {
			stream.BaseStream.Seek(0, SeekOrigin.Begin);
			stream.DiscardBufferedData();
		}

		public override long Position {
			get { return stream.BaseStream.Position; }
			set {
				stream.BaseStream.Position = Math.Min(value, Length);
				Calibrate();
			}
		}

		public override long Length {
			get { return stream.BaseStream.Length; }
		}


		public override void Calibrate() {
			if (Position < Length) {
				Position -= Environment.NewLine.Length;
				stream.ReadLine();
			}
		}
	}
}
