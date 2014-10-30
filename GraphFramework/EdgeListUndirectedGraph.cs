using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using GraphFramework.Extensions;
using System.Threading.Tasks;
using GraphFramework.Interfaces;
using DataStructures;

namespace GraphFramework {




	[DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
	public class EdgeListUndirectedGraph<TVertex> : IUndirectedGraph<TVertex> {

		//protected readonly IAdjacencyList<TVertex> adjacentEdges;
		IList<Edge<TVertex>> edgeList;
		HashSet<TVertex> vertices;

		public EdgeListUndirectedGraph()
			: this(true) { }

		public EdgeListUndirectedGraph(bool allowParallelEdges)
			: this(allowParallelEdges, EdgeExtensions.GetUndirectedVertexEquality<TVertex>()) {
		}

		public EdgeListUndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer)
			: this(allowParallelEdges, edgeEqualityComparer, EqualityComparer<TVertex>.Default) { }


		public EdgeListUndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer, IEqualityComparer<TVertex> vertexComparer)
			: this(allowParallelEdges, edgeEqualityComparer, vertexComparer,true) { }


		public EdgeListUndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex> edgeEqualityComparer, IEqualityComparer<TVertex> vertexComparer, bool isMutable) {
			IsDirected = false;
			AllowParallelEdges = allowParallelEdges;
			this.EdgeEqualityComparer = edgeEqualityComparer;
			this.edgeList = new VeryLargeList<Edge<TVertex>>();
			vertices = new HashSet<TVertex>();			
			this.VertexComparer = vertexComparer;
			IsMutable = isMutable;
		}

		public virtual bool IsMutable { get; protected set; }


		public IEqualityComparer<TVertex> VertexComparer { get; private set; }


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
			return vertices.Add(v);
		}

		public event VertexAction<TVertex> VertexRemoved;
		protected virtual void OnVertexRemoved(TVertex args) {
			var eh = this.VertexRemoved; if (eh != null) { eh(this, args); }
		}

		public bool RemoveVertex(TVertex v) {
			var elc = edgeList.Count;
			this.ClearAdjacentEdges(v);
			
			var result = vertices.Remove(v);
			if (result) {
				this.OnVertexRemoved(v);
				return true;
			}
			return false;

		}

		public int RemoveVertexIf(VertexPredicate<TVertex> pred) {
			var vertices = GraphExtensions.GetEdgeListInstance<TVertex>();
			foreach (var v in this.Vertices) { if (pred(v)) { vertices.Add(v); } }
			foreach (var v in vertices) { RemoveVertex(v); }
			return vertices.Count;
		}


		public bool ContainsVertex(TVertex vertex) {
			return vertices.Contains(vertex);
		}

		public EdgeEqualityComparer<TVertex> EdgeEqualityComparer {
			get;
			protected set;
		}

		protected virtual IEdgeList<TVertex> AddAndReturnEdges(TVertex v) {
			AddVertex(v);
			return null;
		}

		public int RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex> predicate) {
			int c = 0;
			for (int i = edgeList.Count-1; i >= 0; i--) {
				if (edgeList[i].IsAdjacent(v) && predicate(edgeList[i].Source, edgeList[i].Target)) {
					edgeList.RemoveAt(i);
					c++;
				}
			}
			return c;
		}

		public virtual void ClearOutEdges(TVertex v) {			
			for (int i = edgeList.Count-1; i >= 0; i--) {
				if (edgeList[i].IsAdjacent(v)) {
					edgeList.RemoveAt(i);					
				}
			}
		}

		public void TrimEdgeExcess() {
			
		}

		public bool IsOutEdgesEmpty(TVertex v) {
			return (OutDegree(v) == 0);
		}

		public int OutDegree(TVertex v) {
			return edgeList.Where(e => e.IsAdjacent(v)).Count();
		}

		public IEnumerable<TVertex> OutEdges(TVertex v) {
			return edgeList.Where(e => e.IsAdjacent(v)).Select(e => e.GetOtherVertex(v));
		}

		public bool TryGetOutEdges(TVertex v, out IEnumerable<TVertex> edges) {
			try { edges = OutEdges(v); return true; } catch { edges = null; return false; }
		}

		public TVertex OutEdge(TVertex v, int index) {
			return OutEdges(v).Skip(index).First();
		}

		public bool ContainsEdge(TVertex source, TVertex target) {
			return edgeList.FirstOrDefault(e => EdgeExtensions.UndirectedVertexEquality<TVertex>(source, target, e.Source, e.Target)).Equals(default(Edge<TVertex>));
		}

		public int NumberOfMultiEdges(TVertex source, TVertex target) {
			return edgeList.Count(e => EdgeExtensions.UndirectedVertexEquality<TVertex>(source, target, e.Source, e.Target));
		}
		public virtual bool AddEdge(TVertex source, TVertex target) {
			if (!AllowParallelEdges) {
				if (ContainsEdge(source, target)) {
					return false;
				}
			}
			edgeList.Add(new Edge<TVertex>(source, target, false));						
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
				edgeList.Add(e);
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
			AddVertex(source); AddVertex(target);
			AddEdge(source, target);			
			OnEdgeAdded(source, target);
			return true;
		}

		public int AddVerticesAndEdgeRange(IEnumerable<Edge<TVertex>> edges) {
			var ec = EdgeCount;
			foreach (var e in edges) {
				if (!AllowParallelEdges) {
					if (ContainsEdge(e.Source, e.Target)) {
						continue;
					}
					AddVertex(e.Source); AddVertex(e.Target);
					edgeList.Add(e);
				}
			}
			return EdgeCount - ec;
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
			
			for (int i = edgeList.Count-1;i >= 0;i--) {
				if (EdgeEqualityComparer(source, target, edgeList[i].Source, edgeList[i].Target)) {
					edgeList.RemoveAt(i);
					OnEdgeRemoved(source, target);
					return true;
				}
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
				ec += (RemoveEdge(e.Source, e.Target) ? 1 : 0);
			}
			return ec;
		}

		public int RemoveEdgeIf(EdgePredicate<TVertex> predicate) {
			int cnt = 0;
			for (int i = edgeList.Count - 1; i >= 0; i--) {
				if (predicate(edgeList[i].Source, edgeList[i].Target)) {
					edgeList.RemoveAt(i);
					cnt++;
				}
			}
			return cnt;
		}

		public void Clear() {
			edgeList.Clear();
			vertices.Clear();
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
			get { return edgeList.Count; }
			
		}

		public IList<Edge<TVertex>> EdgeList {
			get {
				return edgeList;
			}
		}
		public IEnumerable<Edge<TVertex>> Edges {
			get {
				foreach (var e in edgeList) {
					yield return e;
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
			get { return vertices.Count > 0; }
		}

		public int VertexCount {
			get { return vertices.Count; }
		}

		public IEnumerable<TVertex> Vertices {
			get { return vertices; }
		}

		public ISet<TVertex> VertexSet {
			get { return vertices; }
		}


		public int AddVertexAndOutEdges(TVertex v, IEnumerable<TVertex> targets) {
			AddVertex(v);
			foreach (var t in targets) { AddVertex(t); }
			return AddOutEdges(v, targets);

		}

		public int AddOutEdges(TVertex v, IEnumerable<TVertex> targets) {
			var ec = 0;
			foreach (var e in targets) {
				ec += (AddEdge(v, e) ? 1 : 0);
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
			return edgeList.Where(e => e.IsAdjacent(source)).TakeWhile(e => !e.IsAdjacent(target)).Count();
		}

		public object Clone() {
			var G = new EdgeListUndirectedGraph<TVertex>(AllowParallelEdges, EdgeEqualityComparer, VertexComparer);
			foreach (var e in edgeList) {
				G.edgeList.Add((Edge<TVertex>)e.Clone());				
			}
			foreach (var v in vertices) {
				G.vertices.Add(v);
			}
			return G;
		}
		public object Clone(bool allowParallelEdges) {
			var G = new EdgeListUndirectedGraph<TVertex>(allowParallelEdges, EdgeEqualityComparer, VertexComparer);
			foreach (var e in edgeList) {
				G.edgeList.Add((Edge<TVertex>)e.Clone());
			}
			foreach (var v in vertices) {
				G.vertices.Add(v);
			}
			return G;
		}

		
	}
}
