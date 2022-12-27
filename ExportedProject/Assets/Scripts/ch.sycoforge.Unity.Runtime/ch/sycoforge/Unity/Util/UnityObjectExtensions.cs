using System;
using UnityEngine;

namespace ch.sycoforge.Unity.Util
{
	public static class UnityObjectExtensions
	{
		public static bool IsNull(this UnityEngine.Object obj)
		{
			bool result = true;
			if (obj != null)
			{
				IntPtr field = Caller.GetField<IntPtr>(obj, "m_CachedPtr");
				result = field == IntPtr.Zero;
			}
			return result;
		}
	}
}
