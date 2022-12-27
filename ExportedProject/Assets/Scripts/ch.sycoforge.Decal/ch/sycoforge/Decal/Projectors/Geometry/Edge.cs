namespace ch.sycoforge.Decal.Projectors.Geometry
{
	internal struct Edge
	{
		public ushort IndexPointA;

		public ushort IndexPointB;

		public Edge(ushort indexPointA, ushort indexPointB)
		{
			IndexPointA = indexPointA;
			IndexPointB = indexPointB;
		}

		public override int GetHashCode()
		{
			return HashCode(this);
		}

		public static int HashCode(Edge edge)
		{
			return HashCode(edge.IndexPointA, edge.IndexPointB);
		}

		public static int HashCode(int indexPointA, int indexPointB)
		{
			return (indexPointA << 16) | indexPointB;
		}
	}
}
