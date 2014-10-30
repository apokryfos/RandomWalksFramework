using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using GraphFramework.Serializers;
using System.ComponentModel;
using GraphFramework.Interfaces;
using GraphFramework.Extensions;

namespace GraphFramework {

	


	public class TextGraphReader<TVertex> : GenericGraphReaderBase<TVertex>, IGraphReader<TVertex> {

		public List<string> CommentLines { get; set; }
		
		private StreamReader stream;
		private int bufferSize;
		private bool srcIsCSV = false;
	
		

		public TextGraphReader(string file)
			: this(file, (int)Math.Pow(2, 11)) {

		}
		public TextGraphReader(string file, int bufferSize) {
			stream = new StreamReader(file, Encoding.ASCII, false, bufferSize);
			this.bufferSize = bufferSize;
			srcIsCSV = (Path.GetExtension(file) == ".csv");
			CommentLines = new List<string>();
			CommentLines.Add("#");
		}
		public TextGraphReader(string file, int bufferSize, Func<string,TVertex> vertexParser) 
			: this(file, (int)Math.Pow(2, 11)) {
				
		}

		~TextGraphReader() {
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

		#region IGraphReader Members


		protected override void ReadEntireGraph(IAdjacencyGraph<TVertex> graph) {
			if (stream.EndOfStream) { return; }
			int count = 0;
			IEdgeList<TVertex> current;
			do {
				string s = stream.ReadLine();				
				if (CommentLines.Any(pre => s.StartsWith(pre))) { continue; }
				var parts = s.Split(' ', ',', '\t');
				TVertex source, target;
				try {
					source = (TVertex)Convert.ChangeType(parts[0], typeof(TVertex));
					current = GraphExtensions.GetEdgeListInstance<TVertex>();
				} catch (InvalidCastException) {
					continue;
				}

				for (int i = 1; i < parts.Length; i++) {
					try {
						target = (TVertex)Convert.ChangeType(parts[i], typeof(TVertex));
						current.Add(target);
						count++;
					} catch (NotSupportedException) {
						continue;
					}											
				}
				graph.AddVertexAndOutEdges(source, current);

			} while (!stream.EndOfStream);
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
	

		#endregion
	}
}
