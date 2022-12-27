using System;
using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal.Projectors
{
	internal class SkinnedBoxProjector : BoxProjector
	{
		internal SkinnedBoxProjector(EasyDecal decal)
			: base(decal)
		{
			meshCutter = new SkinnedMeshCutter(decal);
			dynamicMesh = new DynamicSkinnedMesh(decal, RecreationMode.Always);
		}

		internal override void Project()
		{
			if (!base.Parent.Baked)
			{
				DynamicSkinnedMesh dynamicSkinnedMesh = (DynamicSkinnedMesh)dynamicMesh;
				processedIds.Clear();
				dynamicMesh.Clear();
				radius = Math.Max(base.Parent.transform.lossyScale.x, Math.Max(base.Parent.transform.lossyScale.y, base.Parent.transform.lossyScale.z));
				Collider[] array = FindCandidates();
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					GameObject gameObject = collider.gameObject;
					processedIds.Add(gameObject.GetInstanceID());
					ProcessReceiver(gameObject, 0, 0);
				}
				meshCutter.CutMesh(dynamicMesh);
				base.Parent.AddDynamicMesh(dynamicMesh);
				if (dynamicMesh.VerticesAmount > 0)
				{
					base.Parent.SkinnedDecalRenderer.updateWhenOffscreen = true;
					base.Parent.SkinnedDecalRenderer.bones = dynamicSkinnedMesh.Bones;
				}
				else
				{
					base.Parent.SkinnedDecalRenderer.updateWhenOffscreen = false;
					base.Parent.SkinnedDecalRenderer.bones = new Transform[1] { base.Parent.transform };
				}
				base.Parent.SkinnedDecalRenderer.localBounds = base.Parent.SharedMesh.bounds;
				base.Parent.SkinnedDecalRenderer.quality = base.Parent.SkinningQuality;
			}
		}

		protected override void ProcessReceiver(GameObject receiver, int recursiveStepUp, int recursiveStepDown)
		{
			if (recursiveStepUp > base.Parent.RecursiveLookupSteps || recursiveStepDown > base.Parent.RecursiveLookupSteps)
			{
				return;
			}
			MeshFilter componentInChildren = receiver.GetComponentInChildren<MeshFilter>();
			SkinnedMeshRenderer componentInChildren2 = receiver.GetComponentInChildren<SkinnedMeshRenderer>();
			MeshLink componentInChildren3 = receiver.GetComponentInChildren<MeshLink>();
			bool cullInvisibles = base.Parent.CullInvisibles;
			if (componentInChildren2 != null)
			{
				Renderer renderer = null;
				if (cullInvisibles)
				{
					renderer = receiver.GetComponent<Renderer>();
				}
				if (!cullInvisibles || (cullInvisibles && renderer != null && renderer.enabled))
				{
					AddMesh(componentInChildren2, receiver.transform, receiver.transform.localToWorldMatrix);
				}
			}
			else
			{
				if (!(componentInChildren3 != null) || !(componentInChildren3 is SkinnedMeshLink))
				{
					return;
				}
				SkinnedMeshLink skinnedMeshLink = (SkinnedMeshLink)componentInChildren3;
				foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshLink.SkinnedMeshes)
				{
					AddMesh(skinnedMesh, skinnedMesh.transform, skinnedMesh.transform.localToWorldMatrix);
				}
			}
		}

		private void AddMesh(SkinnedMeshRenderer renderer, Transform receiver, Matrix4x4 localToWorld)
		{
			if (renderer != null && renderer.sharedMesh != null && receiver != null)
			{
				Mesh mesh = BakeNoScale(renderer);
				((DynamicSkinnedMesh)dynamicMesh).Add(mesh, renderer.bones, localToWorld);
			}
		}

		private IList<Transform> GetBones(SkinnedMeshRenderer renderer)
		{
			Transform rootBone = renderer.rootBone;
			return GetAllChildren(rootBone, null);
		}

		private List<Transform> GetAllChildren(Transform root, List<Transform> r)
		{
			if (r == null)
			{
				r = new List<Transform>();
			}
			foreach (Transform item in root)
			{
				GetAllChildren(item, r);
				r.Add(item);
			}
			return r;
		}

		internal override void Dispose()
		{
		}

		public static Mesh BakeNoScale(SkinnedMeshRenderer skin)
		{
			Mesh mesh = new Mesh();
			Mesh sharedMesh = skin.sharedMesh;
			GameObject gameObject = skin.gameObject;
			Vector3[] vertices = sharedMesh.vertices;
			Matrix4x4[] bindposes = sharedMesh.bindposes;
			BoneWeight[] boneWeights = sharedMesh.boneWeights;
			Transform[] bones = skin.bones;
			Vector3[] array = new Vector3[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				BoneWeight boneWeight = boneWeights[i];
				if (Mathf.Abs(boneWeight.weight0) > float.Epsilon)
				{
					Vector3 point = bindposes[boneWeight.boneIndex0].MultiplyPoint3x4(vertices[i]);
					array[i] += gameObject.transform.InverseTransformPoint(bones[boneWeight.boneIndex0].localToWorldMatrix.MultiplyPoint3x4(point)) * boneWeight.weight0;
				}
				if (Mathf.Abs(boneWeight.weight1) > float.Epsilon)
				{
					Vector3 point = bindposes[boneWeight.boneIndex1].MultiplyPoint3x4(vertices[i]);
					array[i] += gameObject.transform.InverseTransformPoint(bones[boneWeight.boneIndex1].localToWorldMatrix.MultiplyPoint3x4(point)) * boneWeight.weight1;
				}
				if (Mathf.Abs(boneWeight.weight2) > float.Epsilon)
				{
					Vector3 point = bindposes[boneWeight.boneIndex2].MultiplyPoint3x4(vertices[i]);
					array[i] += gameObject.transform.InverseTransformPoint(bones[boneWeight.boneIndex2].localToWorldMatrix.MultiplyPoint3x4(point)) * boneWeight.weight2;
				}
				if (Mathf.Abs(boneWeight.weight3) > float.Epsilon)
				{
					Vector3 point = bindposes[boneWeight.boneIndex3].MultiplyPoint3x4(vertices[i]);
					array[i] += gameObject.transform.InverseTransformPoint(bones[boneWeight.boneIndex3].localToWorldMatrix.MultiplyPoint3x4(point)) * boneWeight.weight3;
				}
			}
			mesh.vertices = array;
			mesh.normals = sharedMesh.normals;
			mesh.uv = sharedMesh.uv;
			mesh.uv2 = sharedMesh.uv2;
			mesh.tangents = sharedMesh.tangents;
			mesh.triangles = sharedMesh.triangles;
			mesh.bindposes = bindposes;
			mesh.boneWeights = boneWeights;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}
