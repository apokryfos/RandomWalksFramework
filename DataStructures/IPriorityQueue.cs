using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{
	public interface IPriorityQueue<T>
		: IQueue<T> {
		void Update(T value);
	}
}
