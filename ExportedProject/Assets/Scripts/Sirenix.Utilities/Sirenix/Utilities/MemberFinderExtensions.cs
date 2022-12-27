using System;

namespace Sirenix.Utilities
{
	public static class MemberFinderExtensions
	{
		public static MemberFinder FindMember(this Type type)
		{
			return MemberFinder.Start(type);
		}
	}
}
