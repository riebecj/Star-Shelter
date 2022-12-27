using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal
{
	public class MeshCutter<M> : IMeshCutter where M : DynamicMesh
	{
		internal const sbyte OUTSIDE = -1;

		internal const sbyte ONPLANE = 0;

		internal const sbyte INSIDE = 1;

		internal M dynamicMesh;

		internal EasyDecal Parent;

		internal sbyte[] vertexPositions;

		internal List<int> outsideIndices = new List<int>();

		internal List<int> trashedTriangleIndices = new List<int>();

		internal IndexTrashBin indexTrash = new IndexTrashBin();

		internal static Vector3 normalTop = Vector3.up;

		internal static Vector3 normalBottom = Vector3.down;

		internal static Vector3 normalRight = Vector3.right;

		internal static Vector3 normalLeft = Vector3.left;

		internal static Vector3 normalFront = Vector3.forward;

		internal static Vector3 normalBack = Vector3.back;

		internal static Vector3 centerTop = new Vector3(0f, -0.5f, 0f);

		internal static Vector3 centerBottom = new Vector3(0f, 0.5f, 0f);

		internal static Vector3 centerRight = new Vector3(-0.5f, 0f, 0f);

		internal static Vector3 centerLeft = new Vector3(0.5f, 0f, 0f);

		internal static Vector3 centerFront = new Vector3(0f, 0f, -0.5f);

		internal static Vector3 centerBack = new Vector3(0f, 0f, 0.5f);

		internal static MathPlane[] cutPlanes = new MathPlane[6];

		public MeshCutter(EasyDecal parent)
		{
			Parent = parent;
			cutPlanes[0] = new MathPlane(normalRight, centerRight);
			cutPlanes[1] = new MathPlane(normalLeft, centerLeft);
			cutPlanes[2] = new MathPlane(normalTop, centerTop);
			cutPlanes[3] = new MathPlane(normalBottom, centerBottom);
			cutPlanes[4] = new MathPlane(normalFront, centerFront);
			cutPlanes[5] = new MathPlane(normalBack, centerBack);
		}

		void IMeshCutter.CutMesh(DynamicMesh dynamicMesh)
		{
			this.dynamicMesh = dynamicMesh as M;
			vertexPositions = new sbyte[dynamicMesh.VerticesAmount];
			for (int num = 5; num >= 0; num--)
			{
				CutAlongPlane(dynamicMesh, cutPlanes[num]);
			}
			Vector2 vector = new Vector2(0.5f, 0.5f);
			Color fadeoutColor = Parent.GetFadeoutColor();
			dynamicMesh.VertexColors.AddRange(new Color[dynamicMesh.VerticesAmount]);
			for (int num = 0; num < dynamicMesh.VerticesAmount; num++)
			{
				Vector3 vector2 = dynamicMesh.Vertices[num];
				Vector2 vector3 = new Vector2(vector2.x, vector2.z);
				Vector2 value = vector3 + vector;
				dynamicMesh.UV[num] = value;
				dynamicMesh.VertexColors[num] = fadeoutColor;
			}
		}

		private void Cleanup()
		{
			int count = dynamicMesh.triangleIndices.Count;
			for (int i = 0; i < count; i++)
			{
				int index = dynamicMesh.triangleIndices[i];
				int value = indexTrash.ClampIndex(index);
				dynamicMesh.triangleIndices[i] = value;
			}
			RemoveIndices(indexTrash);
		}

		private void RemoveIndices(IndexTrashBin trash)
		{
			int num = -1;
			int num2 = 0;
			for (int num3 = dynamicMesh.Vertices.Count - 1; num3 >= 0; num3--)
			{
				bool flag = trash.Contains(num3);
				if (flag)
				{
					if (num != -1)
					{
						num = num3;
						num2++;
					}
					else
					{
						num = num3;
						num2 = 1;
					}
				}
				if ((!flag || num3 == 0) && num != -1)
				{
					dynamicMesh.RemoveRange(num, num2);
					num = -1;
					num2 = 0;
				}
			}
		}

		private void DebugPlane(Vector3 normal, Vector3 point)
		{
			Debug.DrawRay(point, normal, Color.yellow);
		}

		private bool HasIntersection(sbyte vertexPositionA, sbyte vertexPositionB)
		{
			return vertexPositionA != 0 && vertexPositionB != 0 && vertexPositionA != vertexPositionB;
		}

		private void InitializeVertexPositions(DynamicMesh dynamicMesh, MathPlane plane)
		{
			if (vertexPositions.Length < dynamicMesh.VerticesAmount)
			{
				vertexPositions = new sbyte[dynamicMesh.VerticesAmount];
			}
			for (int i = 0; i < dynamicMesh.VerticesAmount; i++)
			{
				Vector3 point = dynamicMesh.Vertices[i];
				sbyte vertexPosition = GetVertexPosition(plane, point);
				if (vertexPosition == -1)
				{
					outsideIndices.Add(i);
				}
				vertexPositions[i] = vertexPosition;
			}
		}

		private void CutAlongPlane(DynamicMesh dynamicMesh, MathPlane plane)
		{
			InitializeVertexPositions(dynamicMesh, plane);
			int indicesAmount = dynamicMesh.IndicesAmount;
			for (int i = 0; i < indicesAmount; i += 3)
			{
				int num = dynamicMesh.TriangleIndices[i];
				int num2 = dynamicMesh.TriangleIndices[i + 1];
				int num3 = dynamicMesh.TriangleIndices[i + 2];
				sbyte b = 0;
				sbyte b2 = 0;
				sbyte b3 = 0;
				try
				{
					b = vertexPositions[num];
					b2 = vertexPositions[num2];
					b3 = vertexPositions[num3];
				}
				catch
				{
					Debug.LogError(string.Format("Easy Decal: Failed to access vertex positions. indexA: {0}, indexB: {1}, indexC: {2} -> Vertices: ", num, num2, num3, vertexPositions.Length));
					break;
				}
				if (b == 1 || b2 == 1 || b3 == 1)
				{
					Vector3 vector = dynamicMesh.Vertices[num];
					Vector3 vector2 = dynamicMesh.Vertices[num2];
					Vector3 vector3 = dynamicMesh.Vertices[num3];
					bool flag = HasIntersection(b, b2);
					bool flag2 = HasIntersection(b, b3);
					bool flag3 = HasIntersection(b2, b3);
					int num4 = 0;
					int num5 = 0;
					int num6 = 0;
					if (flag)
					{
						num4 = dynamicMesh.VerticesAmount;
						Cut(num, num2, b, plane);
					}
					if (flag2)
					{
						num5 = dynamicMesh.VerticesAmount;
						Cut(num, num3, b, plane);
					}
					if (flag3)
					{
						num6 = dynamicMesh.VerticesAmount;
						Cut(num2, num3, b2, plane);
					}
					if (flag && flag2)
					{
						if (b != 1)
						{
							dynamicMesh.TriangleIndices[i] = num4;
							dynamicMesh.TriangleIndices.Add(num3);
							dynamicMesh.TriangleIndices.Add(num5);
							dynamicMesh.TriangleIndices.Add(num4);
						}
						else
						{
							dynamicMesh.TriangleIndices[i + 1] = num4;
							dynamicMesh.TriangleIndices[i + 2] = num5;
						}
					}
					else if (flag && flag3)
					{
						if (b2 != 1)
						{
							dynamicMesh.TriangleIndices[i + 1] = num4;
							dynamicMesh.TriangleIndices.Add(num3);
							dynamicMesh.TriangleIndices.Add(num4);
							dynamicMesh.TriangleIndices.Add(num6);
						}
						else
						{
							dynamicMesh.TriangleIndices[i] = num4;
							dynamicMesh.TriangleIndices[i + 2] = num6;
						}
					}
					else if (flag2 && flag3)
					{
						if (b3 != 1)
						{
							dynamicMesh.TriangleIndices[i + 2] = num5;
							dynamicMesh.TriangleIndices.Add(num2);
							dynamicMesh.TriangleIndices.Add(num6);
							dynamicMesh.TriangleIndices.Add(num5);
						}
						else
						{
							dynamicMesh.TriangleIndices[i] = num5;
							dynamicMesh.TriangleIndices[i + 1] = num6;
						}
					}
					else if (flag3 && b == 0)
					{
						if (b2 != 1)
						{
							dynamicMesh.TriangleIndices[i + 1] = num6;
						}
						else
						{
							dynamicMesh.TriangleIndices[i + 2] = num6;
						}
					}
					else if (flag2 && b2 == 0)
					{
						if (b != 1)
						{
							dynamicMesh.TriangleIndices[i] = num5;
						}
						else
						{
							dynamicMesh.TriangleIndices[i + 2] = num5;
						}
					}
					else if (flag && b3 == 0)
					{
						if (b != 1)
						{
							dynamicMesh.TriangleIndices[i] = num4;
						}
						else
						{
							dynamicMesh.TriangleIndices[i + 1] = num4;
						}
					}
				}
				else
				{
					trashedTriangleIndices.Add(i);
				}
			}
			int num7 = trashedTriangleIndices.Count - 1;
			for (int j = 0; j < trashedTriangleIndices.Count; j++)
			{
				int index = trashedTriangleIndices[num7 - j];
				dynamicMesh.RemoveTriangleAt(index);
			}
			for (int j = 0; j < outsideIndices.Count; j++)
			{
				int index = outsideIndices[j];
				indexTrash.TrashIndex(index);
			}
			Cleanup();
			trashedTriangleIndices.Clear();
			outsideIndices.Clear();
			indexTrash.ClearBin();
		}

		private void DrawRay(Ray ray, float factor)
		{
			Vector3 vector = Parent.CachedTransform.TransformPoint(ray.origin);
			Vector3 vector2 = Parent.CachedTransform.TransformDirection(ray.direction);
			Debug.DrawLine(vector, vector + factor * vector2, Color.green);
		}

		protected sbyte GetVertexPosition(MathPlane plane, Vector3 point)
		{
			Vector3 origin = plane.Origin;
			Vector3 lhs = point - origin;
			float num = Vector3.Dot(lhs, plane.Normal);
			if (num < float.Epsilon && num > -1E-45f)
			{
				return 0;
			}
			return (sbyte)((!(num > 0f)) ? 1 : (-1));
		}

		protected virtual void Cut(int indexA, int indexB, sbyte vertexPosA, MathPlane plane)
		{
			Vector3 vector = dynamicMesh.Vertices[indexA];
			Vector3 vector2 = dynamicMesh.Vertices[indexB];
			Vector3 a = dynamicMesh.Normals[indexA];
			Vector3 b = dynamicMesh.Normals[indexB];
			Ray ray = new Ray(vector, vector2 - vector);
			float num = plane.StretchFactor(ray);
			Vector3 item = TransformPoint(ray, num);
			dynamicMesh.Vertices.Add(item);
			dynamicMesh.Normals.Add(Vector3.Lerp(a, b, num));
			dynamicMesh.UV.Add(Vector2.Lerp(dynamicMesh.UV[indexA], dynamicMesh.UV[indexB], num));
			dynamicMesh.Tangents.Add(Vector4.zero);
		}

		protected Vector3 TransformPoint(Vector3 pointA, Vector3 pointB, float factor)
		{
			return pointA + (pointB - pointA).normalized * factor;
		}

		protected Vector3 TransformPoint(Ray ray, float factor)
		{
			return ray.GetPoint(factor);
		}
	}
}
