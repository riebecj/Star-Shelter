using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal
{
	public static class EasyDecalManager
	{
		public delegate IList<T> SceneFinder<T>();

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass2
		{
			public DecalTextureAtlas atlas;

			public bool _003CIsCombined_003Eb__0(CombinedDecalsGroup d)
			{
				return d.Atlas == atlas;
			}
		}

		public static SceneFinder<CombinedDecalsGroup> SceneGroupsFinder;

		public static SceneFinder<EasyDecal> SceneDecalsFinder;

		public static Action<Mesh, float, float> Unwrapper;

		private static List<CombinedDecalsGroup> combinedSceneDecals = new List<CombinedDecalsGroup>();

		private static IDictionary<Material, IList<EasyDecal>> materialGroupedDecals = new Dictionary<Material, IList<EasyDecal>>();

		private static IDictionary<DecalTextureAtlas, IList<EasyDecal>> atlasGroupedDecals = new Dictionary<DecalTextureAtlas, IList<EasyDecal>>();

		private static DecalSet decalSet = new DecalSet();

		private static List<DecalBufferGroup> flagsBoundDecals;

		public static IEnumerable<EasyDecal> SceneDecals
		{
			get
			{
				return decalSet.SceneDecals;
			}
		}

		public static IList<CombinedDecalsGroup> CombinedSceneDecals
		{
			get
			{
				return combinedSceneDecals;
			}
		}

		public static IDictionary<Material, IList<EasyDecal>> MaterialGroupedDecals
		{
			get
			{
				return materialGroupedDecals;
			}
		}

		public static IDictionary<DecalTextureAtlas, IList<EasyDecal>> AtlasGroupedDecals
		{
			get
			{
				return atlasGroupedDecals;
			}
		}

		public static IEnumerable<EasyDecal> DeferredDecals
		{
			get
			{
				return decalSet.DeferredDecals;
			}
		}

		public static HashSet<EasyDecal> GetDecals(ProjectionTechnique technique)
		{
			return decalSet.GetDecals(technique);
		}

		public static HashSet<EasyDecal> GetDecals(ProjectionTechnique technique, DeferredFlags flags)
		{
			return decalSet.GetDecals(technique, flags);
		}

		internal static List<DecalBufferGroup> GetDecalBufferGroups()
		{
			if (flagsBoundDecals == null)
			{
				flagsBoundDecals = new List<DecalBufferGroup>();
				List<DeferredFlags> allEnums = EnumHelper.GetAllEnums<DeferredFlags>();
				foreach (DeferredFlags item in allEnums)
				{
					DecalBufferGroup decalBufferGroup = new DecalBufferGroup();
					decalBufferGroup.Decals = GetDeferredDecals(item);
					decalBufferGroup.RenderTargets = DecalBufferGroup.GetRenderTargets(item, false);
					decalBufferGroup.RenderTargetsHDR = DecalBufferGroup.GetRenderTargets(item, true);
					decalBufferGroup.Flags = item;
					decalBufferGroup.PassIndex = DecalBufferGroup.GetPassIndex(item);
					flagsBoundDecals.Add(decalBufferGroup);
				}
			}
			return flagsBoundDecals;
		}

		public static HashSet<EasyDecal> GetDeferredDecals(DeferredFlags flags)
		{
			return decalSet.GetDeferredDecals(flags);
		}

		public static IEnumerable<EasyDecal> GetDeferredDecals()
		{
			return decalSet.DeferredDecals;
		}

		public static void Add(EasyDecal decal)
		{
			decalSet.Add(decal);
		}

		public static void Remove(EasyDecal decal)
		{
			decalSet.Remove(decal);
		}

		public static void EditorUpdate()
		{
			Initialize();
			if (SceneDecalsFinder != null)
			{
				decalSet.Clear();
				IEnumerable<EasyDecal> enumerable = SceneDecalsFinder();
				foreach (EasyDecal item in enumerable)
				{
					decalSet.Add(item);
				}
			}
			if (SceneDecalsFinder != null)
			{
				combinedSceneDecals.Clear();
				combinedSceneDecals.AddRange(SceneGroupsFinder());
			}
			materialGroupedDecals.Clear();
			atlasGroupedDecals.Clear();
			GroupSceneDecals();
		}

		public static void DeleteSceneDecal(EasyDecal decal)
		{
			if (decal != null)
			{
				UnityEngine.Object.DestroyImmediate(decal.gameObject);
				decalSet.Remove(decal);
			}
		}

		public static void CombineAtlasGroup(DecalTextureAtlas atlas, bool setStatic = true)
		{
			if (!(atlas != null) || !atlasGroupedDecals.ContainsKey(atlas))
			{
				return;
			}
			DynamicMesh dynamicMesh = new DynamicMesh(DecalBase.DecalRoot, RecreationMode.Always);
			GameObject gameObject = new GameObject(atlas.name + " [Combined]");
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			CombinedDecalsGroup combinedDecalsGroup = gameObject.AddComponent<CombinedDecalsGroup>();
			combinedDecalsGroup.Atlas = atlas;
			foreach (EasyDecal item in atlasGroupedDecals[atlas])
			{
				if (item.Projector != null)
				{
					dynamicMesh.Add(item.Projector.Mesh, item.LocalToWorldMatrix, gameObject.transform.worldToLocalMatrix);
					combinedDecalsGroup.References.Add(item);
					item.gameObject.SetActive(false);
				}
			}
			meshRenderer.material = atlas.Material;
			meshFilter.sharedMesh = dynamicMesh.ConvertToMesh(null);
			if (Unwrapper != null)
			{
				Unwrapper(meshFilter.sharedMesh, 88f, 4f);
			}
			gameObject.transform.parent = DecalBase.DecalRoot.transform;
			gameObject.isStatic = setStatic;
			combinedSceneDecals.Add(combinedDecalsGroup);
		}

		public static bool IsCombined(DecalTextureAtlas atlas)
		{
			Predicate<CombinedDecalsGroup> predicate = null;
			_003C_003Ec__DisplayClass2 _003C_003Ec__DisplayClass = new _003C_003Ec__DisplayClass2();
			_003C_003Ec__DisplayClass.atlas = atlas;
			bool result = false;
			try
			{
				List<CombinedDecalsGroup> list = combinedSceneDecals;
				if (predicate == null)
				{
					predicate = _003C_003Ec__DisplayClass._003CIsCombined_003Eb__0;
				}
				result = list.Exists(predicate);
			}
			catch
			{
			}
			return result;
		}

		public static void SeparateAtlasGroup(DecalTextureAtlas atlas)
		{
			if (!(atlas != null) || !atlasGroupedDecals.ContainsKey(atlas))
			{
				return;
			}
			CombinedDecalsGroup combinedDecalsGroup = null;
			foreach (CombinedDecalsGroup combinedSceneDecal in combinedSceneDecals)
			{
				if (combinedSceneDecal.Atlas.Equals(atlas))
				{
					combinedDecalsGroup = combinedSceneDecal;
					break;
				}
			}
			if (combinedDecalsGroup != null)
			{
				Activate(combinedDecalsGroup.References, true);
				if (combinedSceneDecals.Contains(combinedDecalsGroup))
				{
					combinedSceneDecals.Remove(combinedDecalsGroup);
				}
				UnityEngine.Object.DestroyImmediate(combinedDecalsGroup.gameObject);
			}
		}

		public static void BakeAll(bool bake)
		{
			Bake(decalSet.SceneDecals, bake);
		}

		public static void Bake(IEnumerable<EasyDecal> decals, bool bake)
		{
			foreach (EasyDecal decal in decals)
			{
				decal.Baked = bake;
			}
		}

		public static void Activate(IEnumerable<EasyDecal> decals, bool active)
		{
			foreach (EasyDecal decal in decals)
			{
				decal.gameObject.SetActive(active);
			}
		}

		private static void Initialize()
		{
			if (decalSet == null)
			{
				decalSet = new DecalSet();
			}
			if (combinedSceneDecals == null)
			{
				combinedSceneDecals = new List<CombinedDecalsGroup>();
			}
			if (materialGroupedDecals == null)
			{
				materialGroupedDecals = new Dictionary<Material, IList<EasyDecal>>();
			}
			if (atlasGroupedDecals == null)
			{
				atlasGroupedDecals = new Dictionary<DecalTextureAtlas, IList<EasyDecal>>();
			}
		}

		private static void GroupSceneDecals()
		{
			foreach (EasyDecal item in decalSet)
			{
				if (item.Source == SourceMode.Atlas)
				{
					Enrow(item.Atlas, item, atlasGroupedDecals);
				}
				else
				{
					Enrow(item.DecalMaterial, item, materialGroupedDecals);
				}
			}
		}

		private static void Enrow<T>(T item, EasyDecal decal, IDictionary<T, IList<EasyDecal>> groupedDecals)
		{
			if (item != null)
			{
				if (!groupedDecals.ContainsKey(item))
				{
					groupedDecals.Add(item, new List<EasyDecal>());
				}
				groupedDecals[item].Add(decal);
			}
		}
	}
}
