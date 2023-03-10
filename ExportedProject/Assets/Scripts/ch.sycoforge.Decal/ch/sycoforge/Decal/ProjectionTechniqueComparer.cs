using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ch.sycoforge.Decal
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ProjectionTechniqueComparer : IEqualityComparer<ProjectionTechnique>
	{
		public bool Equals(ProjectionTechnique x, ProjectionTechnique y)
		{
			return x == y;
		}

		public int GetHashCode(ProjectionTechnique obj)
		{
			return (int)obj;
		}
	}
}
