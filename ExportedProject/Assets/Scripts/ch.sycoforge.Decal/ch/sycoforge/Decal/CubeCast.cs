using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	public static class CubeCast
	{
		private static Vector3[] unitCube;

		public static Vector3[] UnitCube
		{
			get
			{
				if (unitCube == null)
				{
					unitCube = CubicCastPoints(Vector3.zero, 0.5f);
				}
				return unitCube;
			}
		}

		public static IList<RaycastHit> Cast(Vector3 center, float size)
		{
			IList<RaycastHit> list = new List<RaycastHit>();
			Vector3[] array = UnitCube;
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector = TransformPoint(array[i], center, size);
				Vector3 direction = center - vector;
				Ray ray = new Ray(vector, direction);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 5f))
				{
					list.Add(hitInfo);
				}
			}
			return list;
		}

		private static Vector3 TransformPoint(Vector3 p, Vector3 center, float size)
		{
			return p * size + center;
		}

		private static Vector3[] CubicCastPoints(Vector3 center, float size)
		{
			return new Vector3[8]
			{
				center + (Vector3.forward + Vector3.up + Vector3.right) * size,
				center + (Vector3.forward + Vector3.up + Vector3.left) * size,
				center + (Vector3.back + Vector3.up + Vector3.right) * size,
				center + (Vector3.back + Vector3.up + Vector3.left) * size,
				center + (Vector3.forward + Vector3.down + Vector3.right) * size,
				center + (Vector3.forward + Vector3.down + Vector3.left) * size,
				center + (Vector3.back + Vector3.down + Vector3.right) * size,
				center + (Vector3.back + Vector3.down + Vector3.left) * size
			};
		}
	}
}
