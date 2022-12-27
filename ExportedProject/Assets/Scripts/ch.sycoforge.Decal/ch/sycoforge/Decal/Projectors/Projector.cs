using System;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal.Projectors
{
	[Serializable]
	public abstract class Projector
	{
		protected DynamicMesh dynamicMesh;

		protected int id;

		protected EasyDecal Parent { get; set; }

		public DynamicMesh Mesh
		{
			get
			{
				return dynamicMesh;
			}
			set
			{
				dynamicMesh = value;
			}
		}

		public virtual bool IsVisible
		{
			get
			{
				return Parent != null && Parent.DecalRenderer != null && Parent.DecalRenderer.isVisible;
			}
		}

		internal Projector(EasyDecal decal, RecreationMode mode)
		{
			Parent = decal;
			dynamicMesh = new DynamicMesh(decal, mode);
			id = decal.GetInstanceID();
		}

		internal virtual void Reset()
		{
			dynamicMesh.Unfreeze();
		}

		internal virtual bool IsVisibleBy(Camera camera)
		{
			return IsVisible;
		}

		internal virtual void Update()
		{
		}

		internal void UpdateVertexColor()
		{
			EasyDecal parent = Parent;
			Mesh sharedMesh = parent.SharedMesh;
			if (sharedMesh == null)
			{
				return;
			}
			Color[] vertexColorsFreezed = dynamicMesh.vertexColorsFreezed;
			int verticesAmount = dynamicMesh.VerticesAmount;
			if (verticesAmount == vertexColorsFreezed.Length)
			{
				Color white = Color.white;
				white.a = parent.Alpha;
				for (int i = 0; i < verticesAmount; i++)
				{
					vertexColorsFreezed[i] = white;
				}
				sharedMesh.colors = vertexColorsFreezed;
			}
		}

		internal virtual void OnGeometryPropertyChanged()
		{
		}

		internal virtual void OnAlphaChanged()
		{
			UpdateVertexColor();
		}

		internal virtual void OnBakeStatusChanged()
		{
			if (Parent.Baked)
			{
				dynamicMesh.Freeze();
			}
			else
			{
				dynamicMesh.Unfreeze();
			}
		}

		internal virtual void OnFadeOutStarted()
		{
			dynamicMesh.Freeze();
		}

		internal virtual void DrawGizmos(bool selected)
		{
			if (Parent.ShowDir)
			{
				Vector3 vector = -Parent.InverseProjectionDirection;
				Ray ray = new Ray(Parent.transform.position, vector);
				float num = Parent.CachedTransform.localScale.y * 0.5f;
				Gizmos.color = Color.red;
				Gizmos.DrawRay(Parent.transform.position, vector * num);
			}
		}

		internal abstract void Project();

		internal abstract void Dispose();
	}
}
