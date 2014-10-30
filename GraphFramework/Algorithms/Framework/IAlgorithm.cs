using GraphFramework.Interfaces;
namespace GraphFramework.Algorithms.Framework {
	
	public interface IAlgorithm<TVertex> {
		IAdjacencyGraph<TVertex> VisitedGraph { get; set;  }
		void Initialize();
		void Compute();		
	}

	public interface IIncrementalAlgorithm<TGraph> : IAlgorithm<TGraph> {
		void Step();
	}
}
