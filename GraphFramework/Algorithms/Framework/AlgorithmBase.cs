using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using GraphFramework.Interfaces;

namespace GraphFramework.Algorithms.Framework
{
	public abstract class AlgorithmBase<TVertex> : IAlgorithm<TVertex> {
		private IAdjacencyGraph<TVertex> visitedGraph;
		private volatile object syncRoot = new object();

		protected AlgorithmBase(IAdjacencyGraph<TVertex> visitedGraph) {
			this.visitedGraph = visitedGraph;
		}
		public IAdjacencyGraph<TVertex> VisitedGraph { get { return this.visitedGraph; } set { this.visitedGraph = value; } }

		public Object SyncRoot { get { return this.syncRoot; } }

		public void Compute() {			
			this.Initialize();
			try {
				this.InternalCompute();
			} finally {
				this.Clean();
			}			
		}

		public virtual void Initialize() { }

		protected virtual void Clean() { }

		protected abstract void InternalCompute();

		public virtual void Abort() { }

	}
}
