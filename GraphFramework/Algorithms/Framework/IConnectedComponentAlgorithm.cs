using System;
using System.Collections.Generic;
using System.Text;
using GraphFramework.Interfaces;

namespace GraphFramework.Algorithms.Framework
{
    public interface IConnectedComponentAlgorithm<TVertex> : IAlgorithm<TVertex> {
        int ComponentCount { get;}
        IDictionary<TVertex, int> Components { get;}
    }
}
