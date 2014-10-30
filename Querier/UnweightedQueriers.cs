using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomWalkFramework.RandomWalkInterface;
using System.Reflection;
using GraphFramework.Interfaces;
using RandomWalkFramework.RNG;

namespace RandomWalkFramework.Querier
{
	/// <summary>
	/// Generalization of a graph querier with specific applicability to unweighted random walks.
	/// Implements <see cref="IGraphQuerier"/>
	/// </summary>
	/// <typeparam name="TVertex">The type of the vertices (states)</typeparam>
	/// <typeparam name="TEdge">The type of the edges</typeparam>
    public class UnweightedGraphQuerier<TVertex> : IGraphQuerier<TVertex> {

		protected Random r = new MersenneTwister();
		protected IAdjacencyGraph<TVertex> targetGraph;
		

			/// <summary>
		/// Initializes the querier on the specified graph and a default name.
		/// </summary>
		/// <param name="targetGraph">The QuickGraph.IGraph object to use.</param>
		public UnweightedGraphQuerier(IAdjacencyGraph<TVertex> targetGraph) 
			: this(targetGraph, new KeyValuePair<string,string>("SRW","Simple Random Walk")) {
		}

		public UnweightedGraphQuerier(IAdjacencyGraph<TVertex> targetGraph, KeyValuePair<string, string> policyName)
        {
			TotalQueries = 0;
            this.targetGraph = targetGraph;
			this.PolicyName = policyName;
				

		}

		#region IGraphQuerier Members

		public virtual IEnumerable<TVertex> AdjecentEdges(TVertex vertex) {
			TotalQueries++;
			return targetGraph.AdjacentEdges(vertex);
		}


		public virtual int AdjecentDegree(TVertex vertex) {
			TotalQueries++;
			return targetGraph.AdjacentDegree(vertex);
		}

		public virtual TVertex AdjecentEdge(TVertex vertex, int index) {
			TotalQueries++;
			return targetGraph.AdjacentEdge(vertex, index);
		}

		public int TotalQueries { get; private set; }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            
        }

		public KeyValuePair<string, string> PolicyName {
			get;
			protected set;
		}
			

		public virtual TVertex WeightedAdjacentEdge(TVertex vertex, decimal weightedIndex) {
			return AdjecentEdge(vertex, (int)((decimal)AdjecentDegree(vertex)*weightedIndex));
		}

		public virtual decimal EdgeWeight(TVertex source, TVertex target) {
			return 1.0M;
		}

		public virtual decimal EdgeWeightAt(TVertex source, int adjacentIndex) {
			return 1.0M;
		}

		public virtual decimal VertexWeight(TVertex vertex) {
			return AdjecentDegree(vertex);
		}

		#endregion


		public bool IsDirected {
			get { return targetGraph.IsDirected; }
		}


	
	}


   
}
