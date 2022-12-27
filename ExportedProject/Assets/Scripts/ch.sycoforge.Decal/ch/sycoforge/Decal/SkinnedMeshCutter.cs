using System;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal
{
	public class SkinnedMeshCutter : MeshCutter<DynamicSkinnedMesh>
	{
		private IndexedBone[] indexedBones = new IndexedBone[8]
		{
			new IndexedBone(),
			new IndexedBone(),
			new IndexedBone(),
			new IndexedBone(),
			new IndexedBone(),
			new IndexedBone(),
			new IndexedBone(),
			new IndexedBone()
		};

		public SkinnedMeshCutter(EasyDecal parent)
			: base(parent)
		{
		}

		protected override void Cut(int indexA, int indexB, sbyte vertexPosA, MathPlane plane)
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
			dynamicMesh.BonesWeights.Add(LerpBoneWeights(dynamicMesh.BonesWeights[indexA], dynamicMesh.BonesWeights[indexB], num));
		}

		private BoneWeight LerpBoneWeights(BoneWeight boneWeightA, BoneWeight boneWeightB, float stretchFactor)
		{
			float num = 1f - stretchFactor;
			indexedBones[0].Index = boneWeightA.boneIndex0;
			indexedBones[0].Weight = boneWeightA.weight0 * num;
			indexedBones[1].Index = boneWeightA.boneIndex1;
			indexedBones[1].Weight = boneWeightA.weight1 * num;
			indexedBones[2].Index = boneWeightA.boneIndex2;
			indexedBones[2].Weight = boneWeightA.weight2 * num;
			indexedBones[3].Index = boneWeightA.boneIndex3;
			indexedBones[3].Weight = boneWeightA.weight3 * num;
			indexedBones[4].Index = boneWeightB.boneIndex0;
			indexedBones[4].Weight = boneWeightB.weight0 * stretchFactor;
			indexedBones[5].Index = boneWeightB.boneIndex1;
			indexedBones[5].Weight = boneWeightB.weight1 * stretchFactor;
			indexedBones[6].Index = boneWeightB.boneIndex2;
			indexedBones[6].Weight = boneWeightB.weight2 * stretchFactor;
			indexedBones[7].Index = boneWeightB.boneIndex3;
			indexedBones[7].Weight = boneWeightB.weight3 * stretchFactor;
			Array.Sort(indexedBones);
			float num2 = 1f / (indexedBones[0].Weight + indexedBones[1].Weight + indexedBones[2].Weight + indexedBones[3].Weight);
			BoneWeight result = default(BoneWeight);
			result.boneIndex0 = indexedBones[0].Index;
			result.weight0 = indexedBones[0].Weight * num2;
			result.boneIndex1 = indexedBones[1].Index;
			result.weight1 = indexedBones[1].Weight * num2;
			result.boneIndex2 = indexedBones[2].Index;
			result.weight2 = indexedBones[2].Weight * num2;
			result.boneIndex3 = indexedBones[3].Index;
			result.weight3 = indexedBones[3].Weight * num2;
			return result;
		}
	}
}
