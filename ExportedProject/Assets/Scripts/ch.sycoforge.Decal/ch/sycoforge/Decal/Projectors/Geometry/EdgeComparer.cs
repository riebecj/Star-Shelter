using System.Collections.Generic;

namespace ch.sycoforge.Decal.Projectors.Geometry
{
	internal class EdgeComparer : IEqualityComparer<Edge>
	{
		public bool Equals(Edge x, Edge y)
		{
			int num = x.IndexPointA.CompareTo(y.IndexPointA);
			int num2 = x.IndexPointB.CompareTo(y.IndexPointB);
			return num == 0 && num2 == 0;
		}

		public int GetHashCode(Edge obj)
		{
			return obj.GetHashCode();
		}
	}
}
