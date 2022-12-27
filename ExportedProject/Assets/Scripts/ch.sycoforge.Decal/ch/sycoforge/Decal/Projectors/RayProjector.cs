using System;
using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal.Projectors
{
	[Serializable]
	internal class RayProjector : Projector
	{
		private float width = 1f;

		private float length = 1f;

		internal Vector3[] sourceVertices;

		internal Vector2[] sourceUVs;

		internal int[] sourceTriangles;

		internal HashSet<int> invalidIndices = new HashSet<int>();

		internal List<int> validIndices = new List<int>();

		internal RayProjector(EasyDecal decal)
			: base(decal, RecreationMode.Always)
		{
			CreateProjectionGrid();
			CreatePlaneMesh();
		}

		private void CreateProjectionGrid()
		{
			int num = base.Parent.resolution + 1;
			int num2 = num * num;
			if (sourceVertices == null || sourceVertices.Length != num2)
			{
				sourceVertices = new Vector3[num2];
			}
			int num3 = 0;
			float num4 = width / (float)base.Parent.resolution;
			float num5 = length / (float)base.Parent.resolution;
			float num6 = width / 2f;
			float num7 = length / 2f;
			for (float num8 = 0f; num8 < (float)num; num8 += 1f)
			{
				for (float num9 = 0f; num9 < (float)num; num9 += 1f)
				{
					sourceVertices[num3++] = new Vector3(num9 * num4 - num6, 0f, num8 * num5 - num7);
				}
			}
		}

		private void CreatePlaneMesh()
		{
			int num = base.Parent.resolution + 1;
			int num2 = base.Parent.resolution * base.Parent.resolution * 6;
			int num3 = num * num;
			sourceTriangles = new int[num2];
			sourceUVs = new Vector2[num3];
			int num4 = 0;
			float num5 = 1f / (float)base.Parent.resolution;
			float num6 = 1f / (float)base.Parent.resolution;
			float num7 = width / (float)base.Parent.resolution;
			float num8 = length / (float)base.Parent.resolution;
			float num9 = width / 2f;
			float num10 = length / 2f;
			for (float num11 = 0f; num11 < (float)num; num11 += 1f)
			{
				for (float num12 = 0f; num12 < (float)num; num12 += 1f)
				{
					sourceUVs[num4++] = new Vector2(num12 * num5, num11 * num6);
				}
			}
			num4 = 0;
			for (int i = 0; i < base.Parent.resolution; i++)
			{
				int num13 = i + 1;
				for (int j = 0; j < base.Parent.resolution; j++)
				{
					int num14 = j + 1;
					int num15 = i * num;
					int num16 = num13 * num;
					sourceTriangles[num4] = num15 + j;
					sourceTriangles[num4 + 1] = num16 + j;
					sourceTriangles[num4 + 2] = num15 + num14;
					sourceTriangles[num4 + 3] = sourceTriangles[num4 + 1];
					sourceTriangles[num4 + 4] = num16 + num14;
					sourceTriangles[num4 + 5] = sourceTriangles[num4 + 2];
					num4 += 6;
				}
			}
		}

		internal override void OnGeometryPropertyChanged()
		{
			base.OnGeometryPropertyChanged();
			CreateProjectionGrid();
			CreatePlaneMesh();
		}

		internal override void Project()
		{
			invalidIndices.Clear();
			validIndices.Clear();
			dynamicMesh.Clear();
			Vector3[] array = new Vector3[sourceVertices.Length];
			Color[] array2 = new Color[sourceVertices.Length];
			HashSet<int> hashSet = new HashSet<int>();
			Matrix4x4 localToWorldMatrix = base.Parent.CachedTransform.localToWorldMatrix;
			Matrix4x4 worldToLocalMatrix = base.Parent.CachedTransform.worldToLocalMatrix;
			Vector3 direction = base.Parent.CachedTransform.TransformDirection(-Vector3.up);
			Vector3 vector = base.Parent.CachedTransform.TransformDirection(Vector3.up);
			Vector3 vector2 = Vector3.up * base.Parent.ProjectionDistance;
			Ray ray = new Ray(Vector3.zero, direction);
			Vector3 vector3 = base.Parent.CachedTransform.TransformDirection(-Vector3.up);
			Color fadeoutColor = base.Parent.GetFadeoutColor();
			float num = Mathf.Cos(base.Parent.AngleConstraint * ((float)Math.PI / 180f));
			bool enableVertexColorBleed = base.Parent.EnableVertexColorBleed;
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 point = sourceVertices[i];
				point = localToWorldMatrix.MultiplyPoint3x4(point);
				ray.origin = point - vector3 * base.Parent.ProjectionDistance;
				bool flag = false;
				GameObject gameObject = null;
				Vector3 vector4 = vector3;
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, float.MaxValue, base.Parent.Mask.value))
				{
					flag = true;
					gameObject = hitInfo.collider.gameObject;
					vector4 = hitInfo.normal;
				}
				list.Add(vector4);
				Vector3 vector5 = Vector3.zero;
				Color color = fadeoutColor;
				float num2 = Vector3.Dot(-vector3, vector4);
				if (flag && (base.Parent.Receiver == null || base.Parent.MultiMeshEnabled || (!base.Parent.MultiMeshEnabled && gameObject != null && gameObject.GetInstanceID() == base.Parent.Receiver.GetInstanceID())) && Mathf.Abs(hitInfo.distance - base.Parent.ProjectionDistance) < base.Parent.MaxDistance && num <= num2)
				{
					if (base.Parent.Mode == ProjectionMode.SurfaceNormal)
					{
						vector5 = hitInfo.point + hitInfo.normal * base.Parent.Distance;
					}
					else if (base.Parent.Mode == ProjectionMode.ProjectionNormal)
					{
						vector5 = hitInfo.point - vector3.normalized * base.Parent.Distance;
					}
					vector5 = worldToLocalMatrix.MultiplyPoint3x4(vector5);
					array2[i] = fadeoutColor;
				}
				else
				{
					invalidIndices.Add(i);
					if (enableVertexColorBleed)
					{
						List<int> list2 = MeshUtil.AdjacentIndices(sourceVertices, sourceTriangles, i);
						foreach (int item4 in list2)
						{
							if (!hashSet.Contains(item4))
							{
								hashSet.Add(item4);
							}
							array2[item4] = Color.clear;
						}
					}
				}
				array[i] = vector5;
			}
			if (enableVertexColorBleed)
			{
				foreach (int item5 in hashSet)
				{
					array2[item5] = Color.clear;
				}
			}
			for (int current2 = 0; current2 < sourceTriangles.Length; current2 += 3)
			{
				int item = sourceTriangles[current2];
				int item2 = sourceTriangles[current2 + 1];
				int item3 = sourceTriangles[current2 + 2];
				if (!invalidIndices.Contains(item) && !invalidIndices.Contains(item2) && !invalidIndices.Contains(item3))
				{
					validIndices.Add(item);
					validIndices.Add(item2);
					validIndices.Add(item3);
				}
			}
			dynamicMesh.Vertices.AddRange(array);
			dynamicMesh.VertexColors.AddRange(array2);
			dynamicMesh.UV.AddRange(sourceUVs);
			dynamicMesh.Normals.AddRange(list);
			dynamicMesh.TriangleIndices.AddRange(validIndices);
			base.Parent.AddDynamicMesh(dynamicMesh);
		}

		internal override void Dispose()
		{
		}

		internal override void DrawGizmos(bool selected)
		{
			base.DrawGizmos(selected);
			GizmoHelper.DrawBox(base.Parent.CachedTransform, GizmoHelper.ColorFromHex("ff0078"), selected);
		}
	}
}
