using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using GraphFramework.Interfaces;

namespace GraphFramework.Algorithms.Framework {

	public abstract class RootedAlgorithmBase<TVertex> : AlgorithmBase<TVertex> {
		private TVertex rootVertex;
		private bool hasRootVertex;

		protected RootedAlgorithmBase(IAdjacencyGraph<TVertex> visitedGraph)
			: base(visitedGraph) { }

		public bool TryGetRootVertex(out TVertex rootVertex) {
			if (this.hasRootVertex) {
				rootVertex = this.rootVertex;
				return true;
			} else {
				rootVertex = default(TVertex);
				return false;
			}
		}

		public void SetRootVertex(TVertex rootVertex) {
			bool changed = Comparer<TVertex>.Default.Compare(this.rootVertex, rootVertex) != 0;
			this.rootVertex = rootVertex;
			if (changed)
				this.OnRootVertexChanged(EventArgs.Empty);
			this.hasRootVertex = true;
		}

		public void ClearRootVertex() {
			this.rootVertex = default(TVertex);
			this.hasRootVertex = false;
		}

		public event EventHandler RootVertexChanged;
		protected virtual void OnRootVertexChanged(EventArgs e) {
			var eh = this.RootVertexChanged;
			if (eh != null)
				eh(this, e);
		}

		public void Compute(TVertex rootVertex) {
			this.SetRootVertex(rootVertex);
			this.Compute();
		}

	}
}
