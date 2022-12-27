using System;
using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal.Projectors.Geometry
{
	public class DynamicMesh : IMesh
	{
		private const int capacity = 1000;

		protected List<Vector2> uv = new List<Vector2>(1000);

		protected List<Vector2> uv2 = new List<Vector2>(1000);

		protected List<Vector3> vertices = new List<Vector3>(1000);

		protected List<Vector3> normals = new List<Vector3>(1000);

		protected List<Vector4> tangents = new List<Vector4>(1000);

		protected List<Color> vertexColors = new List<Color>(1000);

		internal Color[] vertexColorsFreezed = new Color[0];

		internal RecreationMode mode = RecreationMode.Always;

		private int lastVertexCount;

		internal List<int> triangleIndices = new List<int>();

		internal IndexTrashBin trashBin = new IndexTrashBin();

		protected DecalBase Parent;

		protected bool freezed;

		private bool copiedOnce;

		protected static Vector3 projectionDirection = new Vector3(0f, 1f, 0f);

		public List<Vector3> Vertices
		{
			get
			{
				return vertices;
			}
			internal set
			{
				vertices = value;
			}
		}

		public List<Vector3> Normals
		{
			get
			{
				return normals;
			}
			internal set
			{
				normals = value;
			}
		}

		public List<Vector4> Tangents
		{
			get
			{
				return tangents;
			}
			internal set
			{
				tangents = value;
			}
		}

		public List<Color> VertexColors
		{
			get
			{
				return vertexColors;
			}
			internal set
			{
				vertexColors = value;
			}
		}

		public List<int> TriangleIndices
		{
			get
			{
				return triangleIndices;
			}
			internal set
			{
				triangleIndices = value;
			}
		}

		public List<Vector2> UV
		{
			get
			{
				return uv;
			}
			internal set
			{
				uv = value;
			}
		}

		public List<Vector2> UV2
		{
			get
			{
				return uv2;
			}
			internal set
			{
				uv2 = value;
			}
		}

		internal RecreationMode Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
			}
		}

		internal int VerticesAmount
		{
			get
			{
				return vertices.Count;
			}
		}

		internal int IndicesAmount
		{
			get
			{
				return triangleIndices.Count;
			}
		}

		internal bool Freezed
		{
			get
			{
				return freezed;
			}
		}

		public DynamicMesh()
		{
		}

		public DynamicMesh(DecalBase parent, RecreationMode mode)
		{
			Parent = parent;
			this.mode = mode;
		}

		internal void Freeze()
		{
			if (!freezed)
			{
				vertexColorsFreezed = vertexColors.ToArray();
			}
			freezed = true;
		}

		internal void Unfreeze()
		{
			if (freezed)
			{
			}
			freezed = false;
		}

		internal void SetParent(EasyDecal parent)
		{
		}

		public virtual void Clear()
		{
			vertices.Clear();
			normals.Clear();
			tangents.Clear();
			vertexColors.Clear();
			uv.Clear();
			uv2.Clear();
			triangleIndices.Clear();
		}

		public void Add(Mesh mesh, Matrix4x4 localToWorld)
		{
			int[] triangles = mesh.triangles;
			Vector3[] array = mesh.vertices;
			Vector3[] array2 = mesh.normals;
			Vector2[] uvs = new Vector2[array.Length];
			Vector4[] array3 = mesh.tangents;
			if (array != null)
			{
				if (array2 == null || array2.Length == 0)
				{
					mesh.RecalculateNormals();
					array2 = mesh.normals;
				}
				Add(triangles, array, array2, array3, uvs, localToWorld);
				return;
			}
			throw new NullReferenceException("The vertices of the specified mesh are null.");
		}

		public void Add(Terrain terrain, Matrix4x4 localToWorld)
		{
			Bounds bounds = Parent.Bounds();
			bounds.center -= terrain.transform.position;
			Vector3 max = bounds.max;
			Vector3 min = bounds.min;
			TerrainData terrainData = terrain.terrainData;
			List<int> list = new List<int>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector3> list3 = new List<Vector3>();
			List<Vector4> list4 = new List<Vector4>();
			List<Vector2> list5 = new List<Vector2>();
			if (terrainData != null)
			{
				if (min.z > max.z)
				{
					float z = min.z;
					min.z = max.z;
					max.z = z;
				}
				if (min.x > max.x)
				{
					float x = min.x;
					min.x = max.x;
					max.x = x;
				}
				min.x = Mathf.Min(Mathf.Max(min.x, 0f), terrainData.size.x);
				max.x = Mathf.Min(Mathf.Max(max.x, 0f), terrainData.size.x);
				min.z = Mathf.Min(Mathf.Max(min.z, 0f), terrainData.size.z);
				max.z = Mathf.Min(Mathf.Max(max.z, 0f), terrainData.size.z);
				Vector3 heightmapScale = terrainData.heightmapScale;
				int num = Mathf.FloorToInt(min.x / heightmapScale.x);
				int num2 = Mathf.FloorToInt(min.z / heightmapScale.z);
				int num3 = Mathf.CeilToInt(max.x / heightmapScale.x);
				int num4 = Mathf.CeilToInt(max.z / heightmapScale.z);
				int verticesAmount = VerticesAmount;
				int indicesAmount = IndicesAmount;
				if (num < num3 && num2 < num4)
				{
					float num5 = 1f / (float)(terrainData.heightmapWidth - 1);
					float num6 = 1f / (float)(terrainData.heightmapHeight - 1);
					for (int i = num; i <= num3; i++)
					{
						float x2 = (float)i * heightmapScale.x;
						float x3 = (float)i * num5;
						for (int j = num2; j <= num4; j++)
						{
							float z2 = (float)j * heightmapScale.z;
							float y = (float)j * num6;
							Vector3 item = new Vector3(x2, terrainData.GetInterpolatedHeight(x3, y), z2);
							Vector3 interpolatedNormal = terrainData.GetInterpolatedNormal(x3, y);
							list2.Add(item);
							list3.Add(interpolatedNormal);
							list5.Add(Vector2.zero);
							list4.Add(Vector4.zero);
						}
					}
					int num7 = num4 - num2 + 1;
					int num8 = num3 - num;
					int num9 = num4 - num2;
					for (int k = 0; k < num8; k++)
					{
						for (int l = 0; l < num9; l++)
						{
							int num10 = verticesAmount + l + k * num7;
							int item2 = num10 + num7 + 1;
							int item3 = num10 + num7;
							int item4 = num10 + 1;
							list.Add(num10);
							list.Add(item2);
							list.Add(item3);
							list.Add(num10);
							list.Add(item4);
							list.Add(item2);
						}
					}
				}
				Add(list, list2, list3, list4, list5, localToWorld);
				return;
			}
			throw new ArgumentNullException("The specified terrain is null.");
		}

		private void Add(IList<int> triangles, IList<Vector3> vertices, IList<Vector3> normals, IList<Vector4> tangents, IList<Vector2> uvs, Matrix4x4 localToWorldMatrix)
		{
			int verticesAmount = VerticesAmount;
			AddMeshData(vertices, normals, tangents, uvs, localToWorldMatrix);
			int count = triangles.Count;
			for (int i = 0; i < count; i += 3)
			{
				int index = triangles[i];
				int index2 = triangles[i + 1];
				int index3 = triangles[i + 2];
				if (!trashBin.Contains(index) && !trashBin.Contains(index2) && !trashBin.Contains(index3))
				{
					index = verticesAmount + trashBin.ClampIndex(index);
					index2 = verticesAmount + trashBin.ClampIndex(index2);
					index3 = verticesAmount + trashBin.ClampIndex(index3);
					AddIndices(index, index2, index3);
				}
			}
			trashBin.ClearBin();
		}

		protected void AddIndices(bool det, int indexA, int indexB, int indexC)
		{
			if (det)
			{
				triangleIndices.Add(indexB);
				triangleIndices.Add(indexA);
				triangleIndices.Add(indexC);
			}
			else
			{
				triangleIndices.Add(indexA);
				triangleIndices.Add(indexB);
				triangleIndices.Add(indexC);
			}
		}

		protected void AddIndices(int indexA, int indexB, int indexC)
		{
			triangleIndices.Add(indexA);
			triangleIndices.Add(indexB);
			triangleIndices.Add(indexC);
		}

		protected virtual void AddMeshData(IList<Vector3> vertices, IList<Vector3> normals, IList<Vector4> tangents, IList<Vector2> uvs, Matrix4x4 localToWorld)
		{
			Matrix4x4 worldToLocalMatrix = Parent.WorldToLocalMatrix;
			float num = Mathf.Cos(Parent.AngleConstraint * ((float)Math.PI / 180f));
			Matrix4x4 matrix4x = worldToLocalMatrix * localToWorld;
			int count = vertices.Count;
			float num2 = (Parent.FlipNormals ? (-1f) : 1f);
			float distance = Parent.Distance;
			Vector4 item = Vector4.zero;
			for (int i = 0; i < count; i++)
			{
				Vector3 vector = num2 * matrix4x.MultiplyVector(normals[i]);
				Vector3 vector2 = vertices[i];
				float num3 = Vector3.Dot(projectionDirection, vector);
				bool flag = false;
				if (num <= num3)
				{
					vector2 = matrix4x.MultiplyPoint3x4(vector2);
					if (Parent.BackfaceCulling)
					{
						Vector3 lhs = vector2;
						float num4 = Vector3.Dot(lhs, vector);
						if (num4 <= 0f)
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (tangents != null)
					{
						if (tangents.Count > 0)
						{
							item = tangents[i];
						}
						this.tangents.Add(item);
					}
					if (distance > 0f)
					{
						vector2 += vector * distance;
					}
					this.vertices.Add(vector2);
					this.normals.Add(vector);
					if (uvs != null)
					{
						uv.Add(uvs[i]);
					}
				}
				else
				{
					trashBin.TrashIndex(i);
				}
				OnProcessVertexIndex(i, flag);
			}
		}

		protected virtual void OnProcessVertexIndex(int index, bool valid)
		{
		}

		protected bool AddVertexNormal(Vector3 point, Vector3 normal)
		{
			Vector3 item = point;
			if (Parent.Distance > 0f)
			{
				item = point + normal * Parent.Distance;
			}
			vertices.Add(item);
			normals.Add(normal.normalized);
			return true;
		}

		public void PostOffsetVertices()
		{
			float distance = Parent.Distance;
			if (!Mathf.Approximately(distance, 0f))
			{
				for (int i = 0; i < vertices.Count; i++)
				{
					vertices[i] += normals[i] * distance;
				}
			}
		}

		internal int[] TrianglesToArray()
		{
			int count = triangleIndices.Count;
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = triangleIndices[i];
			}
			return array;
		}

		public void Add(DynamicMesh dynamicMesh, Matrix4x4 sourceLocalToWorld, Matrix4x4 targetWorldToLocal)
		{
			int count = TriangleIndices.Count;
			int count2 = Vertices.Count;
			dynamicMesh.TransformGeometry(sourceLocalToWorld, targetWorldToLocal);
			Vertices.AddRange(dynamicMesh.Vertices);
			Normals.AddRange(dynamicMesh.Normals);
			UV.AddRange(dynamicMesh.UV);
			UV2.AddRange(dynamicMesh.UV2);
			VertexColors.AddRange(dynamicMesh.VertexColors);
			int[] array = dynamicMesh.TriangleIndices.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] += count2;
			}
			TriangleIndices.AddRange(array);
		}

		public void TransformGeometry(Matrix4x4 sourceLocalToWorld, Matrix4x4 targetWorldToLocal)
		{
			Matrix4x4 matrix = sourceLocalToWorld * targetWorldToLocal;
			TransformGeometry(matrix);
		}

		public void TransformGeometry(Matrix4x4 matrix)
		{
			for (int i = 0; i < vertices.Count; i++)
			{
				normals[i] = matrix.MultiplyVector(normals[i]);
				vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
			}
		}

		public virtual Mesh ConvertToMesh(Mesh target, bool forceRecreation = false)
		{
			bool flag = forceRecreation;
			Mesh mesh = null;
			if (target == null)
			{
				mesh = new Mesh();
				mesh.name = "Decal Mesh";
				mesh.MarkDynamic();
				flag = true;
			}
			else
			{
				mesh = target;
			}
			int count = Vertices.Count;
			int count2 = TriangleIndices.Count;
			if (mode == RecreationMode.Always)
			{
				flag = true;
			}
			else if (mode == RecreationMode.Never && !copiedOnce)
			{
				flag = true;
				copiedOnce = true;
			}
			else if (mode == RecreationMode.OnSizeChange && lastVertexCount != count)
			{
				flag = true;
			}
			if (flag)
			{
				CopyGeometryData(mesh);
			}
			lastVertexCount = count;
			return mesh;
		}

		private void CopyGeometryData(Mesh mesh)
		{
			mesh.Clear();
			mesh.vertices = Vertices.ToArray();
			mesh.normals = Normals.ToArray();
			if (Tangents != null && Tangents.Count == Vertices.Count)
			{
				mesh.tangents = Tangents.ToArray();
			}
			if (UV != null && UV.Count == Vertices.Count)
			{
				mesh.uv = UV.ToArray();
			}
			if (UV2 != null && UV2.Count == Vertices.Count)
			{
				mesh.uv2 = UV2.ToArray();
			}
			mesh.triangles = TriangleIndices.ToArray();
			mesh.colors = VertexColors.ToArray();
		}

		internal virtual void RemoveRange(int startIndex, int count)
		{
			vertices.RemoveRange(startIndex, count);
			uv.RemoveRange(startIndex, count);
			normals.RemoveRange(startIndex, count);
			tangents.RemoveRange(startIndex, count);
		}

		internal void RemoveTriangleAt(int index)
		{
			triangleIndices.RemoveRange(index, 3);
		}

		internal void RemoveTrianglesAt(int startIndex, int count)
		{
			triangleIndices.RemoveRange(startIndex, 3 * count);
		}

		internal static void Assign(DynamicMesh dynMesh, Mesh mesh)
		{
			dynMesh.Vertices.AddRange(mesh.vertices);
			dynMesh.TriangleIndices.AddRange(mesh.triangles);
			dynMesh.Normals.AddRange(mesh.normals);
			dynMesh.VertexColors.AddRange(mesh.colors);
			dynMesh.Tangents.AddRange(mesh.tangents);
			dynMesh.UV.AddRange(mesh.uv);
			dynMesh.UV2.AddRange(mesh.uv2);
		}

		protected static float Determinant(Matrix4x4 m)
		{
			float num = m.m22 * m.m33;
			float num2 = m.m32 * m.m23;
			float num3 = m.m21 * m.m33;
			float num4 = m.m31 * m.m23;
			float num5 = m.m21 * m.m32;
			float num6 = m.m31 * m.m22;
			float num7 = m.m20 * m.m33;
			float num8 = m.m30 * m.m23;
			float num9 = m.m20 * m.m32;
			float num10 = m.m30 * m.m22;
			float num11 = m.m20 * m.m31;
			float num12 = m.m30 * m.m21;
			float num13 = num - num2;
			float num14 = num3 - num4;
			float num15 = num5 - num6;
			float num16 = num7 - num8;
			float num17 = num9 - num10;
			float num18 = num11 - num12;
			return m.m00 * (m.m11 * num13 - m.m12 * num14 + m.m13 * num15) - m.m01 * (m.m10 * num13 - m.m12 * num16 + m.m13 * num17) + m.m02 * (m.m10 * num14 - m.m11 * num16 + m.m13 * num18) - m.m03 * (m.m10 * num15 - m.m11 * num17 + m.m12 * num18);
		}
	}
}
