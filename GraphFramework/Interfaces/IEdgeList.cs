using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphFramework.Interfaces {
	public interface IEdgeList<TVertex> : IList<TVertex> {
		void TrimExcess();
	}
}
