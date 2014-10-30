using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using GraphFramework.Interfaces;

namespace GraphFramework.Serializers {

	public interface IGraphReader<TVertex> : IDisposable {
		
		event ProgressChangedEventHandler ProgressChanged;
		
		IAdjacencyGraph<TVertex> ReadEntireGraph();		
		IBidirectionalGraph<TVertex> ReadEntireGraphAsBidirectional();
		IUndirectedGraph<TVertex> ReadEntireGraphAsUndirected();		
		IUndirectedGraph<TVertex> ReadUndirectedGraphFileToUndirectedGraph();
		

		//IDictionary<TVertex, IEdgeList<TVertex>> ReadAdjecencyList();
		void ResetStream();
		long Position { get; set; }
		long Length { get; }
		void Calibrate();
	}

	public interface IGraphWriter<TVertex> : IDisposable {
		
		event ProgressChangedEventHandler ProgressChanged;		
		void WriteGraph(IAdjacencyGraph<TVertex> graph);
		void WriteGraphAsUndirected(IBidirectionalGraph<TVertex> graph);
		void WriteGraphAsUndirected(IUndirectedGraph<TVertex> graph);
		void WriteNextPart(IDictionary<TVertex, IEdgeList<TVertex>> graph, bool directed);
	}
}
