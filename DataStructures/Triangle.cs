using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{

	public class TriangleEqualityComparer<TVertex> : IEqualityComparer<TriangleGeneric<TVertex>> {
		#region IEqualityComparer<Triangle> Members

		public bool Equals(TriangleGeneric<TVertex> x, TriangleGeneric<TVertex> y) {
			return x.Equals(y);
		}

		public int GetHashCode(TriangleGeneric<TVertex> obj) {
			return (((long)obj.A.GetHashCode() + (long)obj.B.GetHashCode() + (long)obj.C.GetHashCode()) / 3).GetHashCode();
		}

		#endregion
	}

	
	public class TriangleGeneric<TVertex> : IEquatable<TriangleGeneric<TVertex>> {
		private TVertex a, b, c;

		private TriangleGeneric() { }
		public TriangleGeneric(TVertex a, TVertex b, TVertex c) {
			this.a = a;
			this.b = b;
			this.c = c;
		}


		public TVertex A { get { return a; } }
		public TVertex B { get { return b; } }
		public TVertex C { get { return c; } }

		public TVertex Vertex(int index) {
			index = index % 3;
			if (index == 0)
				return A;
			else if (index == 1)
				return B;
			else
				return C;
		}

		public TVertex GetOtherVertex(TVertex a, TVertex b) {
			if ((A.Equals(a) && B.Equals(b)) || (B.Equals(a) && A.Equals(b))) { 
				return C;
			} else if ((A.Equals(a) && C.Equals(b)) || (C.Equals(a) && A.Equals(b))) {
				return B;
			} else if ((B.Equals(a) && C.Equals(b)) || (C.Equals(a) && B.Equals(b))) {
				return A;
			} else {
				throw new InvalidOperationException("Vertices " + a + " and " + b + "are not both adjacent to this triangle");
			}

		}

		public bool HasVertex(TVertex vertex) {
			return (A.Equals(vertex) || B.Equals(vertex) || C.Equals(vertex));
		}


		#region IEquatable<TriangleGeneric<TVertex>> Members

		public bool Equals(TriangleGeneric<TVertex> other) {
			return (other.HasVertex(A) && other.HasVertex(B) && other.HasVertex(C));
		}

		public bool IsValid {
			get { return (!A.Equals(B) && !B.Equals(C) && !C.Equals(A)); }
		}

		public override string ToString() {
			return A + "<->" + B + "<->" + C;
		}

		public override int GetHashCode() {
			return (int)(((long)A.GetHashCode() + (long)B.GetHashCode() + (long)C.GetHashCode()) / 3L);
		}



		#endregion

	}
}
