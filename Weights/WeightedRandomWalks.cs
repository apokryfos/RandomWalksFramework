using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RandomWalkFramework.Querier;
using DataStructures;
using GraphFramework.Algorithms;
using System.Diagnostics;
using GraphFramework.Interfaces;



namespace RandomWalkFramework.Weights {
	/// <summary>
	/// Examples on how to use the <see cref="WeightFunction"/>
	/// These methods generally provide an implementation for the weight function delegate
	/// <para>
	///	These weights were used in various experiments
	/// </para>
	/// </summary>
	public static class BetaRandomWalk {
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target, double beta)
		   {
			   return (decimal)Math.Pow(((long)targetGraph.AdjacentDegree(source) * (long)targetGraph.AdjacentDegree(target)), beta);
		}
	}

	public static class MaxReciprocalDegreeWeightedRandomWalk {
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target) {
			return (decimal)Math.Max(1.0/targetGraph.AdjacentDegree(source), 1.0/targetGraph.AdjacentDegree(target));
		}
	}

	public static class AgeBias {
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target, double c) 
			where TVertex : IComparable<TVertex> {
			return (decimal)((source.CompareTo(target)<0?1:2) * c);
		}
	}

	public static class ClusteringCoefficient {
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target, decimal epsilon, decimal gamma) {
			double w = (double)epsilon;			
			w += ((double)gamma*targetGraph.GetClusteringCoefficientCached(source)) / (double)targetGraph.AdjacentDegree(source);
			if (double.IsNaN(w)) { return 0.0M; }
			w += ((double)gamma * targetGraph.GetClusteringCoefficientCached(target)) / (double)targetGraph.AdjacentDegree(target);
			if (double.IsNaN(w)) { return 0.0M; }
			return (decimal)w;
		}
	}

	public class HiddenPartitionRandomWalkWeight<TVertex>
		 {
		private int vcount;
		private decimal c;
		public HiddenPartitionRandomWalkWeight(decimal c, int vcount) {
			this.c = c;
			this.vcount = vcount;
		}
		public decimal EdgeWeight(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target) {
			return (1.0M + (Convert.ToInt32(source) < vcount ? c : 0) + (Convert.ToInt32(target) < vcount ? c : 0));
		}
	}

	/// <summary>
	/// General weight to use for weighted random walks.
	/// Tag must be convertible to decimal. 
	/// Tags are commonly (signed) real or natural numbers
	/// If you deal with different weights consider creating another delegate implementation
	/// </summary>
	/// <typeparam name="TVertex">Vertex Type</typeparam>
	/// <typeparam name="TEdge">Edge Type</typeparam>
	/// <typeparam name="TTag">Edge tag type</typeparam>
	public static class GeneralWeight<TVertex, TWeight> {				
		public static decimal WeightedGraphEdgeWeight(IWeightedAdjacencyGraph<TVertex,TWeight> targetGraph, TVertex source, TVertex target) {
			return Convert.ToDecimal(targetGraph.GetWeight(source,target));
		}
		public static decimal UnweightedGraphEdgeWeight(IAdjacencyGraph<TVertex> targetGraph, WeightFunction<TVertex> wf , TVertex source, TVertex target) {
			return (decimal)wf(targetGraph, source, target);
		}

	}

	public static class VertexRandomWalkWeight {
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target, decimal gamma) {
			return ((gamma / (decimal)targetGraph.AdjacentDegree(source)) + (gamma / (decimal)targetGraph.AdjacentDegree(target)));
		}
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target) {
			return EdgeWeight<TVertex>(targetGraph, source, target, 1.0M);
		}

	}

	public static class TriangleRandomWalkWeight {
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target) {
			return TriangleRandomWalkWeight.EdgeWeight<TVertex>(targetGraph, source, target, 1.0);
		}

		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target, double c) {
			if (source.Equals(target))
				return 1.0M;

			var t = targetGraph.GetContainingTrianglesCount(source, target);			
			return 1.0M + (decimal)c * (decimal)t;
		}
	}

	public static class TriangleAvoidingRandomWalkWeight {
			
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target)
			 {
				if (source.Equals(target))
					return (decimal)(2*targetGraph.AdjacentDegree(source));

			var cnt = targetGraph.GetContainingTrianglesCount(source,target);			
			return targetGraph.AdjacentDegree(source) + targetGraph.AdjacentDegree(target) - cnt;
			
		}
	}
	public static class VertexTriangleRandomWalkWeight {
		
		public static decimal EdgeWeight<TVertex>(IAdjacencyGraph<TVertex> targetGraph, TVertex source, TVertex target)
			 {
			if (source.Equals(target))
				return (decimal)(2 * targetGraph.AdjacentDegree(source));

			var cnt = targetGraph.GetContainingTrianglesCount(source,target);			
			return 1.0M / (decimal)targetGraph.AdjacentDegree(source) + 1.0M / (decimal)targetGraph.AdjacentDegree(target) + cnt;

		}
	}

	

}
