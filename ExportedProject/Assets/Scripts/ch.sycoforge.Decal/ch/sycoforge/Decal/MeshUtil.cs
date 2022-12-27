using System;
using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal
{
	public static class MeshUtil
	{
		private static Mesh cubeMesh24 = new Mesh();

		private static Vector3[] cubeVertices24 = new Vector3[24];

		private static Vector3[] cubeNormals24 = new Vector3[24]
		{
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.down,
			Vector3.down,
			Vector3.down,
			Vector3.down,
			Vector3.right,
			Vector3.right,
			Vector3.right,
			Vector3.right,
			Vector3.left,
			Vector3.left,
			Vector3.left,
			Vector3.left,
			Vector3.back,
			Vector3.back,
			Vector3.back,
			Vector3.back,
			Vector3.forward,
			Vector3.forward,
			Vector3.forward,
			Vector3.forward
		};

		private static int[] cubeTriangles24 = new int[36]
		{
			0, 1, 2, 0, 2, 3, 6, 5, 4, 7,
			6, 4, 8, 9, 10, 8, 10, 11, 12, 15,
			14, 12, 14, 13, 16, 17, 18, 16, 18, 19,
			22, 21, 20, 23, 22, 20
		};

		public static List<int> AdjacentIndices(IList<Vector3> vertices, IList<int> triangles, int index)
		{
			List<int> list = new List<int>();
			int num = 0;
			int num2 = 0;
			bool flag = false;
			for (int i = 0; i < triangles.Count; i += 3)
			{
				num = 0;
				num2 = 0;
				flag = false;
				if (index == triangles[i])
				{
					num = triangles[i + 1];
					num2 = triangles[i + 2];
					flag = true;
				}
				else if (index == triangles[i + 1])
				{
					num = triangles[i];
					num2 = triangles[i + 2];
					flag = true;
				}
				else if (index == triangles[i + 2])
				{
					num = triangles[i];
					num2 = triangles[i + 1];
					flag = true;
				}
				if (flag)
				{
					list.Add(num);
					list.Add(num2);
					flag = false;
				}
			}
			return list;
		}

		internal static IList<Vector4> CalculateTangents(IList<Vector3> vertices, IList<Vector3> normals, IList<Vector2> uv, IList<int> triangles)
		{
			int count = triangles.Count;
			int count2 = vertices.Count;
			Vector3[] array = new Vector3[count2];
			Vector3[] array2 = new Vector3[count2];
			IList<Vector4> list = new List<Vector4>(count2);
			int num = 0;
			while (num < count)
			{
				int num2 = triangles[num++];
				int num3 = triangles[num++];
				int num4 = triangles[num++];
				Vector3 vector = vertices[num2];
				Vector3 vector2 = vertices[num3];
				Vector3 vector3 = vertices[num4];
				Vector2 vector4 = uv[num2];
				Vector2 vector5 = uv[num3];
				Vector2 vector6 = uv[num4];
				float num5 = vector2.x - vector.x;
				float num6 = vector3.x - vector.x;
				float num7 = vector2.y - vector.y;
				float num8 = vector3.y - vector.y;
				float num9 = vector2.z - vector.z;
				float num10 = vector3.z - vector.z;
				float num11 = vector5.x - vector4.x;
				float num12 = vector6.x - vector4.x;
				float num13 = vector5.y - vector4.y;
				float num14 = vector6.y - vector4.y;
				float num15 = 1f / (num11 * num14 - num12 * num13);
				Vector3 vector7 = new Vector3((num14 * num5 - num13 * num6) * num15, (num14 * num7 - num13 * num8) * num15, (num14 * num9 - num13 * num10) * num15);
				Vector3 vector8 = new Vector3((num11 * num6 - num12 * num5) * num15, (num11 * num8 - num12 * num7) * num15, (num11 * num10 - num12 * num9) * num15);
				array[num2] += vector7;
				array[num3] += vector7;
				array[num4] += vector7;
				array2[num2] += vector8;
				array2[num3] += vector8;
				array2[num4] += vector8;
			}
			for (int i = 0; i < count2; i++)
			{
				Vector3 vector9 = normals[i];
				Vector3 vector10 = array[i];
				Vector4 item = (vector10 - vector9 * Vector3.Dot(vector9, vector10)).normalized;
				item.w = ((Vector3.Dot(Vector3.Cross(vector9, vector10), array2[i]) < 0f) ? (-1f) : 1f);
				list.Add(item);
			}
			return list;
		}

		internal static void CalculateNormals(IList<Vector3> normals, IList<Vector3> vertices, IList<int> triangles, float factor = 1f)
		{
			if (normals.Count != vertices.Count)
			{
				throw new ArgumentException("Easy Decal: Normals count must match vertices count");
			}
			for (int i = 0; i < triangles.Count; i += 3)
			{
				int index = triangles[i];
				int index2 = triangles[i + 1];
				int index3 = triangles[i + 2];
				Vector3 vector = vertices[index];
				Vector3 vector2 = vertices[index2];
				Vector3 vector3 = vertices[index3];
				Vector3 lhs = vector - vector3;
				Vector3 rhs = vector2 - vector3;
				Vector3 value = (normals[index2] = (normals[index] = (Vector3.Cross(lhs, rhs) * factor).normalized));
				normals[index3] = value;
			}
		}

		internal static void SmoothVertexNormals(IList<Vector3> normals, Vector3 inverseProjectionDir, float factor, float threshold)
		{
			for (int i = 0; i < normals.Count; i++)
			{
				Vector3 vector = normals[i];
				float num = FaceFactor(vector, inverseProjectionDir);
				if (num > threshold)
				{
					num = (num - threshold) / (1f - threshold);
					factor *= num;
				}
				normals[i] = Vector3.Lerp(vector, inverseProjectionDir, factor).normalized;
			}
		}

		internal static float FaceFactor(Vector3 normal, Vector3 inverseProjectionDir)
		{
			return (Vector3.Dot(normal, inverseProjectionDir) + 1f) * 0.5f;
		}

		internal static void AutoWeld(DynamicMesh mesh, float threshold)
		{
			IList<Vector3> vertices = mesh.Vertices;
			List<Vector3> list = new List<Vector3>();
			List<Vector2> list2 = new List<Vector2>();
			int i = 0;
			using (IEnumerator<Vector3> enumerator = vertices.GetEnumerator())
			{
				for (; enumerator.MoveNext(); i++)
				{
					Vector3 current = enumerator.Current;
					foreach (Vector3 item in list)
					{
						if (!(Vector3.Distance(item, current) <= threshold))
						{
							continue;
						}
						goto IL_0091;
					}
					list.Add(current);
					list2.Add(mesh.UV[i]);
					IL_0091:;
				}
			}
			IList<int> triangleIndices = mesh.TriangleIndices;
			for (int j = 0; j < triangleIndices.Count; j++)
			{
				for (int k = 0; k < list.Count; k++)
				{
					if (Vector3.Distance(list[k], vertices[triangleIndices[j]]) <= threshold)
					{
						triangleIndices[j] = k;
						break;
					}
				}
			}
		}

		internal static void AutoWeld(DynamicMesh mesh, float threshold, float bucketStep)
		{
			IList<Vector3> vertices = mesh.Vertices;
			IList<Vector3> list = new List<Vector3>();
			int[] array = new int[vertices.Count];
			int num = 0;
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			for (int i = 0; i < vertices.Count; i++)
			{
				if (vertices[i].x < vector.x)
				{
					vector.x = vertices[i].x;
				}
				if (vertices[i].y < vector.y)
				{
					vector.y = vertices[i].y;
				}
				if (vertices[i].z < vector.z)
				{
					vector.z = vertices[i].z;
				}
				if (vertices[i].x > vector2.x)
				{
					vector2.x = vertices[i].x;
				}
				if (vertices[i].y > vector2.y)
				{
					vector2.y = vertices[i].y;
				}
				if (vertices[i].z > vector2.z)
				{
					vector2.z = vertices[i].z;
				}
			}
			int num2 = Mathf.FloorToInt((vector2.x - vector.x) / bucketStep) + 1;
			int num3 = Mathf.FloorToInt((vector2.y - vector.y) / bucketStep) + 1;
			int num4 = Mathf.FloorToInt((vector2.z - vector.z) / bucketStep) + 1;
			List<int>[,,] array2 = new List<int>[num2, num3, num4];
			for (int i = 0; i < vertices.Count; i++)
			{
				int num5 = Mathf.FloorToInt((vertices[i].x - vector.x) / bucketStep);
				int num6 = Mathf.FloorToInt((vertices[i].y - vector.y) / bucketStep);
				int num7 = Mathf.FloorToInt((vertices[i].z - vector.z) / bucketStep);
				if (array2[num5, num6, num7] == null)
				{
					array2[num5, num6, num7] = new List<int>();
				}
				int num8 = 0;
				while (true)
				{
					if (num8 < array2[num5, num6, num7].Count)
					{
						Vector3 vector3 = list[array2[num5, num6, num7][num8]] - vertices[i];
						if (Vector3.SqrMagnitude(vector3) < threshold)
						{
							array[i] = array2[num5, num6, num7][num8];
							break;
						}
						num8++;
						continue;
					}
					list.Add(vertices[i]);
					array2[num5, num6, num7].Add(num);
					array[i] = num;
					num++;
					break;
				}
			}
			IList<int> triangleIndices = mesh.TriangleIndices;
			IList<int> list2 = new int[triangleIndices.Count];
			for (int i = 0; i < triangleIndices.Count; i++)
			{
				mesh.TriangleIndices[i] = array[triangleIndices[i]];
			}
			for (int i = 0; i < num; i++)
			{
				mesh.Vertices[i] = list[i];
			}
		}

		internal static void TangentsMultithreaded(Mesh mesh)
		{
			Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			int[] triangles = mesh.triangles;
			Vector2[] uv = mesh.uv;
			int num = triangles.Length;
			int num2 = vertices.Length;
			Vector3[] array = new Vector3[num2];
			Vector3[] array2 = new Vector3[num2];
			Vector4[] array3 = new Vector4[num2];
			int num3 = 0;
			while (num3 < num)
			{
				long num4 = triangles[num3++];
				long num5 = triangles[num3++];
				long num6 = triangles[num3++];
				Vector3 vector = vertices[num4];
				Vector3 vector2 = vertices[num5];
				Vector3 vector3 = vertices[num6];
				Vector2 vector4 = uv[num4];
				Vector2 vector5 = uv[num5];
				Vector2 vector6 = uv[num6];
				float num7 = vector2.x - vector.x;
				float num8 = vector3.x - vector.x;
				float num9 = vector2.y - vector.y;
				float num10 = vector3.y - vector.y;
				float num11 = vector2.z - vector.z;
				float num12 = vector3.z - vector.z;
				float num13 = vector5.x - vector4.x;
				float num14 = vector6.x - vector4.x;
				float num15 = vector5.y - vector4.y;
				float num16 = vector6.y - vector4.y;
				float num17 = 1f / (num13 * num16 - num14 * num15);
				Vector3 vector7 = new Vector3((num16 * num7 - num15 * num8) * num17, (num16 * num9 - num15 * num10) * num17, (num16 * num11 - num15 * num12) * num17);
				Vector3 vector8 = new Vector3((num13 * num8 - num14 * num7) * num17, (num13 * num10 - num14 * num9) * num17, (num13 * num12 - num14 * num11) * num17);
				array[num4] += vector7;
				array[num5] += vector7;
				array[num6] += vector7;
				array2[num4] += vector8;
				array2[num5] += vector8;
				array2[num6] += vector8;
			}
			for (int i = 0; i < num2; i++)
			{
				Vector3 vector9 = normals[i];
				Vector3 vector10 = array[i];
				array3[i] = (vector10 - vector9 * Vector3.Dot(vector9, vector10)).normalized;
				array3[i].w = ((Vector3.Dot(Vector3.Cross(vector9, vector10), array2[i]) < 0f) ? (-1f) : 1f);
			}
			mesh.tangents = array3;
		}

		internal static Mesh MeshFromBoxCollider24(BoxCollider collider)
		{
			Vector3 vector = collider.center + collider.size * 0.5f;
			Vector3 vector2 = vector;
			Vector3 vector3 = new Vector3(vector.x, vector.y, 0f - vector.z);
			Vector3 vector4 = new Vector3(0f - vector.x, vector.y, 0f - vector.z);
			Vector3 vector5 = new Vector3(0f - vector.x, vector.y, vector.z);
			Vector3 vector6 = new Vector3(vector.x, 0f - vector.y, vector.z);
			Vector3 vector7 = new Vector3(vector.x, 0f - vector.y, 0f - vector.z);
			Vector3 vector8 = new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z);
			Vector3 vector9 = new Vector3(0f - vector.x, 0f - vector.y, vector.z);
			cubeVertices24[0] = vector2;
			cubeVertices24[1] = vector3;
			cubeVertices24[2] = vector4;
			cubeVertices24[3] = vector5;
			cubeVertices24[4] = vector6;
			cubeVertices24[5] = vector7;
			cubeVertices24[6] = vector8;
			cubeVertices24[7] = vector9;
			cubeVertices24[8] = vector2;
			cubeVertices24[9] = vector6;
			cubeVertices24[10] = vector7;
			cubeVertices24[11] = vector3;
			cubeVertices24[12] = vector5;
			cubeVertices24[13] = vector9;
			cubeVertices24[14] = vector8;
			cubeVertices24[15] = vector4;
			cubeVertices24[16] = vector3;
			cubeVertices24[17] = vector7;
			cubeVertices24[18] = vector8;
			cubeVertices24[19] = vector4;
			cubeVertices24[20] = vector2;
			cubeVertices24[21] = vector6;
			cubeVertices24[22] = vector9;
			cubeVertices24[23] = vector5;
			Mesh mesh = new Mesh();
			mesh.vertices = cubeVertices24;
			mesh.triangles = cubeTriangles24;
			return mesh;
		}

		internal static Mesh MeshFromBoxCollider(BoxCollider collider)
		{
			Vector3[] array = new Vector3[8];
			Matrix4x4 localToWorldMatrix = collider.transform.localToWorldMatrix;
			Quaternion rotation = collider.transform.rotation;
			Vector3 localScale = collider.transform.localScale;
			Vector3 extents = collider.bounds.extents;
			array[0] = localToWorldMatrix.MultiplyPoint3x4(extents);
			array[1] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(0f - extents.x, extents.y, extents.z));
			array[2] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, 0f - extents.z));
			array[3] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(0f - extents.x, extents.y, 0f - extents.z));
			array[4] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, 0f - extents.y, extents.z));
			array[5] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(0f - extents.x, 0f - extents.y, extents.z));
			array[6] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, 0f - extents.y, 0f - extents.z));
			array[7] = localToWorldMatrix.MultiplyPoint3x4(-extents);
			collider.transform.localScale = localScale;
			collider.transform.rotation = rotation;
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.triangles = new int[36]
			{
				0, 3, 1, 0, 2, 3, 6, 5, 7, 6,
				4, 5, 4, 1, 5, 4, 0, 1, 2, 7,
				3, 2, 6, 7, 1, 7, 5, 1, 3, 7,
				4, 2, 0, 4, 6, 2
			};
			return mesh;
		}

		public static Mesh Clone(Mesh sourceMesh)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = sourceMesh.vertices;
			mesh.normals = sourceMesh.normals;
			mesh.uv = sourceMesh.uv;
			mesh.uv2 = sourceMesh.uv2;
			mesh.tangents = sourceMesh.tangents;
			mesh.colors = sourceMesh.colors;
			mesh.triangles = sourceMesh.triangles;
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}
