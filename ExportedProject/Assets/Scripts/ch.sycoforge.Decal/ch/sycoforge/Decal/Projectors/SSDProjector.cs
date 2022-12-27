using System;
using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal.Projectors
{
	[Serializable]
	internal class SSDProjector : Projector
	{
		private static Vector3[] vertices;

		private static int[] triangles;

		internal Vector3[] normals;

		internal Vector2[] uv;

		internal Vector2[] uv2;

		internal Color[] colors;

		private bool isVisible;

		private TransformObserver transformObserver;

		private static Vector3 extents = Vector3.one * 0.5f;

		public override bool IsVisible
		{
			get
			{
				return isVisible;
			}
		}

		internal SSDProjector(EasyDecal decal)
			: base(decal, RecreationMode.Never)
		{
			InitializeProjectionCube();
			decal.OnOrientationChanged += decal_OnOrientationChanged;
			if (Camera.main == null)
			{
				Debug.LogError("Easy Decal: No main camera found in scene. Deferred decals cannot be rendered. Please tag your camera as 'MainCamera'.");
			}
			else
			{
				transformObserver = new TransformObserver(Camera.main.transform);
			}
		}

		internal override bool IsVisibleBy(Camera camera)
		{
			return InsideFrustum(camera);
		}

		private void decal_OnOrientationChanged(DecalBase decal, OrientationChange change)
		{
			UpdateSSD();
		}

		private void UpdateSSD()
		{
			isVisible = InsideFrustum(Camera.main);
		}

		private bool InsideFrustum(Camera camera)
		{
			bool result = false;
			if (Application.isEditor && !Application.isPlaying)
			{
				return true;
			}
			if (camera != null)
			{
				bool flag = false;
				Vector3[] array = vertices;
				foreach (Vector3 position in array)
				{
					Vector3 position2 = base.Parent.CachedTransform.TransformPoint(position);
					Vector3 vector = Camera.main.WorldToViewportPoint(position2);
					bool flag2 = vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f && vector.z >= 0f;
					flag = flag || flag2;
				}
				result = flag;
			}
			return result;
		}

		private void CreateStatics()
		{
			if (vertices == null)
			{
				Vector3 zero = Vector3.zero;
				vertices = new Vector3[8]
				{
					zero + extents,
					zero + new Vector3(0f - extents.x, extents.y, extents.z),
					zero + new Vector3(extents.x, extents.y, 0f - extents.z),
					zero + new Vector3(0f - extents.x, extents.y, 0f - extents.z),
					zero + new Vector3(extents.x, 0f - extents.y, extents.z),
					zero + new Vector3(0f - extents.x, 0f - extents.y, extents.z),
					zero + new Vector3(extents.x, 0f - extents.y, 0f - extents.z),
					zero - extents
				};
				triangles = new int[36]
				{
					0, 3, 1, 0, 2, 3, 6, 5, 7, 6,
					4, 5, 4, 1, 5, 4, 0, 1, 2, 7,
					3, 2, 6, 7, 1, 7, 5, 1, 3, 7,
					4, 2, 0, 4, 6, 2
				};
			}
			uv = new Vector2[8]
			{
				Vector2.one,
				Vector2.one,
				Vector2.one,
				Vector2.one,
				Vector2.one,
				Vector2.one,
				Vector2.one,
				Vector2.one
			};
			uv2 = new Vector2[8]
			{
				Vector2.zero,
				Vector2.zero,
				Vector2.zero,
				Vector2.zero,
				Vector2.zero,
				Vector2.zero,
				Vector2.zero,
				Vector2.zero
			};
			colors = new Color[8]
			{
				Color.white,
				Color.white,
				Color.white,
				Color.white,
				Color.white,
				Color.white,
				Color.white,
				Color.white
			};
		}

		private void CreateDynamics()
		{
			float x = Mathf.Cos(base.Parent.AngleConstraint * ((float)Math.PI / 180f));
			Vector3 vector = new Vector3(x, 0f, 0f);
			normals = new Vector3[8] { vector, vector, vector, vector, vector, vector, vector, vector };
		}

		private void CreateCubeMesh()
		{
			CreateDynamics();
			CreateStatics();
		}

		internal override void OnGeometryPropertyChanged()
		{
			base.OnGeometryPropertyChanged();
			CreateDynamics();
			dynamicMesh.Normals = new List<Vector3>(normals);
			base.Parent.AddDynamicMesh(dynamicMesh);
		}

		internal override void Update()
		{
			base.Update();
			bool flag = Time.frameCount % 2 == 0;
			bool evenID = base.Parent.evenID;
			if (((evenID && flag) || (!evenID && !flag)) && transformObserver.CheckTransformChange() != 0)
			{
				isVisible = InsideFrustum(Camera.main);
			}
		}

		internal override void Project()
		{
			if (!(Camera.main == null))
			{
				base.Parent.AddDynamicMesh(dynamicMesh);
			}
		}

		private void InitializeProjectionCube()
		{
			CreateCubeMesh();
			dynamicMesh.Clear();
			dynamicMesh.Vertices.AddRange(vertices);
			dynamicMesh.TriangleIndices.AddRange(triangles);
			dynamicMesh.Normals.AddRange(normals);
			dynamicMesh.UV.AddRange(uv);
			dynamicMesh.UV2.AddRange(uv2);
			dynamicMesh.VertexColors.AddRange(colors);
		}

		internal override void Dispose()
		{
		}

		internal override void DrawGizmos(bool selected)
		{
			base.DrawGizmos(selected);
			GizmoHelper.DrawBox(base.Parent.CachedTransform, GizmoHelper.ColorFromHex("8bed00"), selected);
		}
	}
}
