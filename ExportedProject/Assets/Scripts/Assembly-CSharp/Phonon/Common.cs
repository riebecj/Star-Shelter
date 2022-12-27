using System.Text;
using UnityEngine;

namespace Phonon
{
	public static class Common
	{
		public static Vector3 ConvertVector(UnityEngine.Vector3 point)
		{
			Vector3 result = default(Vector3);
			result.x = point.x;
			result.y = point.y;
			result.z = 0f - point.z;
			return result;
		}

		public static UnityEngine.Vector3 ConvertVector(Vector3 point)
		{
			UnityEngine.Vector3 result = default(UnityEngine.Vector3);
			result.x = point.x;
			result.y = point.y;
			result.z = 0f - point.z;
			return result;
		}

		public static byte[] ConvertString(string s)
		{
			return Encoding.UTF8.GetBytes(s + '\0');
		}
	}
}
