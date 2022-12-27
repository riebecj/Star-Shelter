using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	public static class MeshCast
	{
		public static IList<RaycastHit> Cast(Vector3 center, float size, Mesh mesh)
		{
			IList<RaycastHit> list = new List<RaycastHit>();
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vector = TransformPoint(vertices[i], center, size);
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
	}
}
