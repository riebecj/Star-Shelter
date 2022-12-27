using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal.Projectors.Geometry
{
	public class DynamicSkinnedMesh : DynamicMesh
	{
		private SkinQuality skinQuality = SkinQuality.Auto;

		private Transform[] bones;

		private Matrix4x4[] bindPoses;

		private List<BoneWeight> bonesWeights = new List<BoneWeight>();

		private BoneWeight[] originalBonesWeights;

		public Transform[] Bones
		{
			get
			{
				return bones;
			}
		}

		public Matrix4x4[] BindPoses
		{
			get
			{
				return bindPoses;
			}
		}

		public List<BoneWeight> BonesWeights
		{
			get
			{
				return bonesWeights;
			}
		}

		public SkinQuality SkinQuality
		{
			get
			{
				return skinQuality;
			}
			set
			{
				skinQuality = value;
			}
		}

		internal DynamicSkinnedMesh(DecalBase parent, RecreationMode mode)
			: base(parent, mode)
		{
		}

		public void Add(Mesh mesh, Transform[] bones, Matrix4x4 localToWorld)
		{
			Matrix4x4 inverse = (Parent.WorldToLocalMatrix * localToWorld).inverse;
			this.bones = bones;
			bindPoses = mesh.bindposes;
			for (int i = 0; i < bones.Length; i++)
			{
				bindPoses[i] *= inverse;
			}
			originalBonesWeights = mesh.boneWeights;
			Add(mesh, localToWorld);
		}

		internal override void RemoveRange(int startIndex, int count)
		{
			base.RemoveRange(startIndex, count);
			bonesWeights.RemoveRange(startIndex, count);
		}

		protected override void OnProcessVertexIndex(int index, bool valid)
		{
			base.OnProcessVertexIndex(index, valid);
			if (valid)
			{
				BoneWeight item = originalBonesWeights[index];
				bonesWeights.Add(item);
			}
		}

		public override Mesh ConvertToMesh(Mesh target, bool reset = false)
		{
			Mesh mesh = base.ConvertToMesh(target);
			if (mesh.vertexCount > 0)
			{
				if (bonesWeights != null && bonesWeights.Count == base.Vertices.Count)
				{
					mesh.boneWeights = bonesWeights.ToArray();
				}
				if (bindPoses != null)
				{
					mesh.bindposes = bindPoses;
				}
			}
			else
			{
				mesh.vertices = new Vector3[1];
				mesh.normals = new Vector3[1];
				mesh.tangents = new Vector4[1];
				mesh.uv = new Vector2[1];
				mesh.uv2 = new Vector2[1];
				mesh.colors = new Color[1];
				mesh.boneWeights = new BoneWeight[1];
				mesh.bindposes = new Matrix4x4[1];
			}
			return mesh;
		}

		public override void Clear()
		{
			base.Clear();
			bonesWeights.Clear();
		}
	}
}
