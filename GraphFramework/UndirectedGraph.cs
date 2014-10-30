using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using GraphFramework.Extensions;
using System.Threading.Tasks;
using GraphFramework.Interfaces;

namespace GraphFramework {
	
	
	 

	[DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
	public class UndirectedGraph<TVertex> : IUndirectedGraph<TVertex> {

		protected IAdjacencyList<TVertex> adjacentEdges;

		public UndirectedGraph()
			: this(true) { }

		public UndirectedGraph(bool allowParallelEdges)
			: this(allowParallelEdges, EdgeExtensions.GetUndirectedVertexEquality<TVertex>()) {		
		}

		public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer)
			: this(allowParallelEdges, edgeEqualityComparer, EqualityComparer<TVertex>.Default, true) { }


		public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer, IEqualityComparer<TVertex> vertexComparer)
			: this(allowParallelEdges, edgeEqualityComparer, vertexComparer, true) { }


		public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer, IEqualityComparer<TVertex> vertexComparer, bool isMutable) {
			IsDirected = false;
			AllowParallelEdges = allowParallelEdges;
			this.EdgeEqualityComparer = edgeEqualityComparer;
			this.adjacentEdges = GraphExtensions.GetAdjacencyListInstance<TVertex>();
			EdgeCount = 0;
			this.VertexComparer = vertexComparer;
			IsMutable = isMutable;
		}

		public UndirectedGraph(IAdjacencyList<TVertex> adjacencyList) : this(adjacencyList, true)  { }
		public UndirectedGraph(IAdjacencyList<TVertex> adjacencyList, bool allowParallelEdges) : this(adjacencyList, allowParallelEdges, EdgeExtensions.GetUndirectedVertexEquality<TVertex>()) { }
		public UndirectedGraph(IAdjacencyList<TVertex> adjacencyList, bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer) : this(adjacencyList, allowParallelEdges, edgeEqualityComparer, EqualityComparer<TVertex>.Default) { }
		public UndirectedGraph(IAdjacencyList<TVertex> adjacencyList, bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer, IEqualityComparer<TVertex> vertexComparer) : this(adjacencyList, allowParallelEdges, edgeEqualityComparer, EqualityComparer<TVertex>.Default,true) { }
		public UndirectedGraph(IAdjacencyList<TVertex> adjacencyList, bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer, IEqualityComparer<TVertex> vertexComparer, bool isMutable) 
		: this(allowParallelEdges, edgeEqualityComparer, vertexComparer, isMutable) {
			this.adjacentEdges = adjacencyList;
		}

		public IEqualityComparer<TVertex> VertexComparer { get; private set; }

		public virtual bool IsMutable {
			get;
			protected set; 
		}

		public virtual bool IsDirected {
			get;
			protected set;
		}

		public bool AllowParallelEdges {
			get;
			protected set;
		}



		public event VertexAction<TVertex> VertexAdded;
		protected virtual void OnVertexAdded(TVertex args) {
			var eh = this.VertexAdded; if (eh != null) { eh(this, args); }
		}

		public int AddVertexRange(IEnumerable<TVertex> vertices) {			
			var vc = VertexCount;
			foreach (var v in vertices) {
				AddVertex(v);
			}
			return VertexCount - vc;
			
		}

		public virtual bool AddVertex(TVertex v) {
			if (!IsMutable) { throw new NotSupportedException(); }
			if (adjacentEdges.ContainsKey(v)) {
				return false;
			}
			adjacentEdges.Add(v, GraphExtensions.GetEdgeListInstance<TVertex>(AllowParallelEdges));
			OnVertexAdded(v);
			return true;

		}

		public event VertexAction<TVertex> VertexRemoved;
		protected virtual void OnVertexRemoved(TVertex args) {
			var eh = this.VertexRemoved; if (eh != null) { eh(this, args); }
		}

		public bool RemoveVertex(TVertex v) {
			if (!IsMutable) { throw new NotSupportedException(); }
			this.ClearOutEdges(v);
			bool result = this.adjacentEdges.Remove(v);
			if (result) { this.OnVertexRemoved(v); }
			return result;
		}

		public int RemoveVertexIf(VertexPredicate<TVertex> pred) {
			if (!IsMutable) { throw new NotSupportedException(); }
			var vertices = GraphExtensions.GetEdgeListInstance<TVertex>();
			foreach (var v in this.Vertices) { if (pred(v)) { vertices.Add(v); } }
			foreach (var v in vertices) { RemoveVertex(v); }
			return vertices.Count;
		}


		public bool ContainsVertex(TVertex vertex) {
			return adjacentEdges.ContainsKey(vertex);
		}

		public EdgeEqualityComparer<TVertex> EdgeEqualityComparer {
			get;
			protected set;
		}

		protected virtual IEdgeList<TVertex> AddAndReturnEdges(TVertex v) {
			if (!IsMutable) { throw new NotSupportedException(); }
			IEdgeList<TVertex> edges;
			if (!this.adjacentEdges.TryGetValue(v, out edges)) {
				edges = GraphExtensions.GetEdgeListInstance<TVertex>();
				adjacentEdges.Add(v, edges);
				OnVertexAdded(v);
	
			}
			return edges;
		}

		public int RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex> predicate) {			
			int c = 0;
			foreach (var vertex in OutEdges(v)) {
				if (predicate(v, vertex)) {
					RemoveEdge(vertex, v);
					c++;
				}
			}
			return c;
		}

		public virtual void ClearOutEdges(TVertex v) {			
			var edges = adjacentEdges[v];
			foreach (var e in edges) {
				RemoveEdge(v, e);
			}
		}

		public void TrimEdgeExcess() {
			foreach (var el in adjacentEdges.Values) { el.TrimExcess(); }
		}

		public bool IsOutEdgesEmpty(TVertex v) {
			return (OutDegree(v) == 0);
		}

		public int OutDegree(TVertex v) {
			return adjacentEdges[v].Count;
		}

		public IEnumerable<TVertex> OutEdges(TVertex v) {
			foreach (var vertex in adjacentEdges[v]) { yield return vertex; } yield break;
		}

		public bool TryGetOutEdges(TVertex v, out IEnumerable<TVertex> edges) {
			try { edges = OutEdges(v); return true; } catch { edges = null; return false; }
		}

		public TVertex OutEdge(TVertex v, int index) {			

			return adjacentEdges[v][index];
		}

		public bool ContainsEdge(TVertex source, TVertex target) {
			return (ContainsVertex(source) && ContainsVertex(target) && adjacentEdges[source].Contains(target));
		}

		public int NumberOfMultiEdges(TVertex source, TVertex target) {
			return adjacentEdges[source].Count(v => v.Equals(target));
		}
		public virtual bool AddEdge(TVertex source, TVertex target) {
			if (!IsMutable) { throw new NotSupportedException(); }
			if (!AllowParallelEdges) {
				if (ContainsEdge(source, target)) {
					return false;
				}
			}
			adjacentEdges[source].Add(target);
			if (!target.Equals(source)) { adjacentEdges[target].Add(source); }
			EdgeCount++;
			OnEdgeAdded(source, target);
			return true;
		}

		protected virtual void OnEdgeAdded(TVertex source, TVertex target) {
			var eh = this.EdgeAdded; if (eh != null) { eh(this, source, target); }
		}


		public event EdgeAction<TVertex> EdgeAdded;

		public int AddEdgeRange(IEnumerable<Edge<TVertex>> edges) {
			var ec = EdgeCount;
			foreach (var e in edges) {
				AddEdge(e.Source, e.Target);
			}
			return EdgeCount - ec;
		}
		
		public int AddEdgeRange(IEnumerable<TVertex> sources, IEnumerable<TVertex> targets) {
			var senum = sources.GetEnumerator();
			var tenum = targets.GetEnumerator();
			int cnt = 0;
			while (senum.MoveNext() && tenum.MoveNext()) {
				cnt += (AddEdge(senum.Current, tenum.Current) ? 1 : 0);
			}
			return cnt;
		}

		public virtual bool AddVerticesAndEdge(TVertex source, TVertex target) {
			if (!AllowParallelEdges) {
				if (ContainsEdge(source, target)) {
					return false;
				}
			}
			var sedges = AddAndReturnEdges(source);
			sedges.Add(target);
			if (!target.Equals(source)) {
				var tedges = AddAndReturnEdges(target);
				tedges.Add(source);
			}
			EdgeCount++;
			OnEdgeAdded(source, target);
			return true;
		}

		public int AddVerticesAndEdgeRange(IEnumerable<Edge<TVertex>> edges) {
			var ec = 0;
			foreach (var e in edges) {
				ec += (AddVerticesAndEdge(e.Source,e.Target)?1:0);
			}			
			return ec;
		}

		public int AddVerticesAndEdgeRange(IEnumerable<TVertex> sources, IEnumerable<TVertex> targets) {
			var senum = sources.GetEnumerator();
			var tenum = targets.GetEnumerator();
			int cnt = 0;
			while (senum.MoveNext() && tenum.MoveNext()) {
				cnt += (AddVerticesAndEdge(senum.Current, tenum.Current) ? 1 : 0);
			}
			return cnt;
		}

		public virtual bool RemoveEdge(TVertex source, TVertex target) {
			if (!IsMutable) { throw new NotSupportedException(); }
			var sedges = adjacentEdges[source];						
			if (sedges.Remove(target)) {
				if (!target.Equals(source)) {
					var tedges = adjacentEdges[target];			
					tedges.Remove(source);
				}
				EdgeCount--;
				OnEdgeRemoved(source, target);
				return true;
			}
			return false;
		}
		protected virtual void OnEdgeRemoved(TVertex source, TVertex target) {
			var eh = this.EdgeRemoved; if (eh != null) { eh(this, source, target); }
		}
		public event EdgeAction<TVertex> EdgeRemoved;


		public int RemoveEdgeRange(IEnumerable<Edge<TVertex>> edges) {
			var ec = 0;
			foreach (var e in edges) {
				ec += (RemoveEdge(e.Source, e.Target)?1:0);
			}			
			return ec;
		}

		public int RemoveEdgeIf(EdgePredicate<TVertex> predicate) {
			int cnt = 0;			
			foreach (var kv in adjacentEdges) {
				for (int j = kv.Value.Count - 1;j >= 0;j--) {
					if (predicate(kv.Key, kv.Value[j])) {
						RemoveEdge(kv.Key, kv.Value[j]);
						EdgeCount--;
						cnt++;
					}
				}
			}
			return cnt;
		}

		public void Clear() {
			if (!IsMutable) { throw new NotSupportedException(); }
			adjacentEdges.Clear();
			OnCleared();
		}
		protected virtual void OnCleared() {
			var eh = this.Cleared; if (eh != null) { eh(this, EventArgs.Empty); }
		}

		public event EventHandler Cleared;

		public bool IsEdgesEmpty {
			get { return EdgeCount == 0; }
		}

		public int EdgeCount {
			get;
			private set;
		}

		public IEnumerable<Edge<TVertex>> Edges {
			get {
				var tempedge = new Edge<TVertex>();
				foreach (var v in Vertices) {
					foreach (var e in OutEdges(v)) {
						tempedge.Set(v, e, IsDirected);
						yield return tempedge;
					}
				}
				yield break;
			}
		}

		public EdgeEnumeration<TVertex> EdgesTargets {
			get {
				return new EdgeEnumeration<TVertex>(this);
			}
		}


		public bool IsVerticesEmpty {
			get { return adjacentEdges.Count > 0; }
		}

		public int VertexCount {
			get { return adjacentEdges.Count; }
		}

		public IEnumerable<TVertex> Vertices {
			get { return adjacentEdges.Keys; }
		}

		public int AddVertexAndOutEdges(TVertex v, IEnumerable<TVertex> targets) {						
			AddVertex(v);
			foreach (var t in targets) { AddVertex(t); }
			return AddOutEdges(v, targets);
			
		}

		public int AddOutEdges(TVertex v, IEnumerable<TVertex> targets) {
			var ec = 0;
			foreach (var e in targets) {
				ec += (AddEdge(v, e)?1:0);
			}
			return ec;
		}

		public virtual int AdjacentDegree(TVertex v) {
			return OutDegree(v);
		}

		public virtual TVertex AdjacentEdge(TVertex v, int index) {
			return OutEdge(v, index);
		}

		public virtual IEnumerable<TVertex> AdjacentEdges(TVertex v) {
			return OutEdges(v);
		}

		public virtual bool TryGetAdjacentEdges(TVertex v, out IEnumerable<TVertex> edges) {
			return TryGetOutEdges(v, out edges);
		}

		public virtual bool IsAdjacentEdgesEmpty(TVertex v) {
			return IsOutEdgesEmpty(v);
		}

		public void ClearAdjacentEdges(TVertex v) {
			ClearOutEdges(v);
		}

		public int OutEdgeIndex(TVertex source, TVertex target) {
			return AdjacentEdgeIndex(source, target);
		}

		public int AdjacentEdgeIndex(TVertex source, TVertex target) {
			if (ContainsVertex(source)) {
				return adjacentEdges[source].IndexOf(target);
			} else {
				return -2;
			}
		}

		public IUndirectedGraph<TVertex> ReadOnlyClone() {
			var G = new UndirectedGraph<TVertex>(AllowParallelEdges, EdgeEqualityComparer, VertexComparer);
			foreach (var kv in adjacentEdges) {
				var lc = GraphExtensions.GetEdgeListInstance<TVertex>(kv.Value);
				G.adjacentEdges.Add(kv.Key, lc);
			}
			G.IsMutable = false;
			return G;

		}

		public object Clone() {
			var G = new UndirectedGraph<TVertex>(AllowParallelEdges, EdgeEqualityComparer, VertexComparer);
			foreach (var kv in adjacentEdges) {
				var lc = GraphExtensions.GetEdgeListInstance<TVertex>(kv.Value);
				G.adjacentEdges.Add(kv.Key, lc);
			}
			return G;
		}
		public object Clone(bool allowParallelEdges) {
			var G = new UndirectedGraph<TVertex>(allowParallelEdges, EdgeEqualityComparer, VertexComparer);
			foreach (var kv in adjacentEdges) {
				var lc = GraphExtensions.GetEdgeListInstance<TVertex>(allowParallelEdges, kv.Value);
				G.adjacentEdges.Add(kv.Key, lc);
			}
			return G;
		}
	}
}
