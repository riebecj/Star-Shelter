using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	internal class DecalSet : IEnumerable<EasyDecal>, IEnumerable
	{
		private HashSet<EasyDecal> sceneDecals;

		private Dictionary<ProjectionTechnique, HashSet<EasyDecal>> techGroupedDecals;

		private Dictionary<ProjectionTechnique, Dictionary<DeferredFlags, HashSet<EasyDecal>>> flagsGroupedDecals;

		private HashSet<EasyDecal> empty;

		private ProjectionTechniqueComparer ptc;

		private DeferredFlagsComparer dfc;

		private static int nextID;

		public IEnumerable<EasyDecal> SceneDecals
		{
			get
			{
				return sceneDecals;
			}
		}

		public IEnumerable<EasyDecal> DeferredDecals
		{
			get
			{
				IEnumerable<EasyDecal> result = empty;
				if (techGroupedDecals.ContainsKey(ProjectionTechnique.Deferred))
				{
					result = techGroupedDecals[ProjectionTechnique.Deferred];
				}
				return result;
			}
		}

		internal DecalSet()
		{
			ptc = default(ProjectionTechniqueComparer);
			dfc = default(DeferredFlagsComparer);
			sceneDecals = new HashSet<EasyDecal>();
			techGroupedDecals = new Dictionary<ProjectionTechnique, HashSet<EasyDecal>>(ptc);
			flagsGroupedDecals = new Dictionary<ProjectionTechnique, Dictionary<DeferredFlags, HashSet<EasyDecal>>>(ptc);
			empty = new HashSet<EasyDecal>();
			Initialize();
		}

		public HashSet<EasyDecal> GetDecals(ProjectionTechnique technique)
		{
			HashSet<EasyDecal> result = empty;
			if (techGroupedDecals.ContainsKey(technique))
			{
				result = techGroupedDecals[technique];
			}
			return result;
		}

		public HashSet<EasyDecal> GetDecals(ProjectionTechnique technique, DeferredFlags flags)
		{
			HashSet<EasyDecal> result = empty;
			if (flagsGroupedDecals.ContainsKey(technique) && flagsGroupedDecals[technique].ContainsKey(flags))
			{
				result = flagsGroupedDecals[technique][flags];
			}
			return result;
		}

		public void Clear()
		{
			sceneDecals.Clear();
			techGroupedDecals.Clear();
			flagsGroupedDecals.Clear();
		}

		public void Add(EasyDecal decal)
		{
			bool flag = sceneDecals.Contains(decal);
			decal.OnTechniqueChanged += decal_OnTechniqueChanged;
			decal.OnDeferredFlagsChanged += decal_OnDeferredFlagsChanged;
			decal.OnDestroyed += decal_OnDeactivated;
			decal.ID = nextID++;
			sceneDecals.Add(decal);
			if (Application.isEditor && !Application.isPlaying)
			{
				decal_OnTechniqueChanged(decal, decal.Technique, decal.Technique);
				decal_OnDeferredFlagsChanged(decal, decal.DeferredFlags, decal.DeferredFlags);
			}
		}

		public void Remove(EasyDecal decal)
		{
			decal.OnTechniqueChanged -= decal_OnTechniqueChanged;
			decal.OnDeferredFlagsChanged -= decal_OnDeferredFlagsChanged;
			decal.OnDestroyed -= decal_OnDeactivated;
			ProjectionTechnique technique = decal.Technique;
			DeferredFlags deferredFlags = decal.DeferredFlags;
			sceneDecals.Remove(decal);
			if (techGroupedDecals.ContainsKey(technique))
			{
				techGroupedDecals[technique].Remove(decal);
			}
			if (flagsGroupedDecals.ContainsKey(technique) && flagsGroupedDecals[technique].ContainsKey(deferredFlags))
			{
				flagsGroupedDecals[technique][deferredFlags].Remove(decal);
			}
		}

		private void decal_OnDeactivated(DecalBase obj)
		{
			Remove(obj as EasyDecal);
		}

		private void decal_OnDeferredFlagsChanged(DecalBase decal, DeferredFlags lastFlags, DeferredFlags currFlags)
		{
			EasyDecal easyDecal = decal as EasyDecal;
			ProjectionTechnique technique = easyDecal.Technique;
			if (!techGroupedDecals.ContainsKey(technique))
			{
				techGroupedDecals.Add(technique, new HashSet<EasyDecal>());
			}
			if (flagsGroupedDecals.ContainsKey(technique) && flagsGroupedDecals[technique].ContainsKey(lastFlags))
			{
				flagsGroupedDecals[technique][lastFlags].Remove(easyDecal);
			}
			if (!flagsGroupedDecals.ContainsKey(technique))
			{
				flagsGroupedDecals.Add(technique, new Dictionary<DeferredFlags, HashSet<EasyDecal>>(dfc));
			}
			if (!flagsGroupedDecals[technique].ContainsKey(currFlags))
			{
				flagsGroupedDecals[technique].Add(currFlags, new HashSet<EasyDecal>());
			}
			flagsGroupedDecals[technique][currFlags].Add(easyDecal);
		}

		private void decal_OnTechniqueChanged(DecalBase decal, ProjectionTechnique lastTech, ProjectionTechnique currTech)
		{
			EasyDecal easyDecal = decal as EasyDecal;
			DeferredFlags deferredFlags = easyDecal.DeferredFlags;
			if (techGroupedDecals.ContainsKey(lastTech))
			{
				techGroupedDecals[lastTech].Remove(easyDecal);
			}
			if (flagsGroupedDecals.ContainsKey(lastTech) && flagsGroupedDecals[lastTech].ContainsKey(deferredFlags))
			{
				flagsGroupedDecals[lastTech][deferredFlags].Remove(easyDecal);
			}
			if (!techGroupedDecals.ContainsKey(currTech))
			{
				techGroupedDecals.Add(currTech, new HashSet<EasyDecal>());
			}
			if (!flagsGroupedDecals.ContainsKey(currTech))
			{
				flagsGroupedDecals.Add(currTech, new Dictionary<DeferredFlags, HashSet<EasyDecal>>(dfc));
			}
			if (!flagsGroupedDecals[currTech].ContainsKey(deferredFlags))
			{
				flagsGroupedDecals[currTech].Add(deferredFlags, new HashSet<EasyDecal>());
			}
			techGroupedDecals[currTech].Add(easyDecal);
			flagsGroupedDecals[currTech][deferredFlags].Add(easyDecal);
		}

		public HashSet<EasyDecal> GetDeferredDecals(DeferredFlags flags)
		{
			HashSet<EasyDecal> result = empty;
			if (flagsGroupedDecals.ContainsKey(ProjectionTechnique.Deferred) && flagsGroupedDecals[ProjectionTechnique.Deferred].ContainsKey(flags))
			{
				result = flagsGroupedDecals[ProjectionTechnique.Deferred][flags];
			}
			return result;
		}

		public IEnumerator<EasyDecal> GetEnumerator()
		{
			return sceneDecals.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return sceneDecals.GetEnumerator();
		}

		private void Initialize()
		{
			flagsGroupedDecals = new Dictionary<ProjectionTechnique, Dictionary<DeferredFlags, HashSet<EasyDecal>>>(ptc);
			ProjectionTechnique[] array = Enum.GetValues(typeof(ProjectionTechnique)) as ProjectionTechnique[];
			ProjectionTechnique[] array2 = array;
			foreach (ProjectionTechnique key in array2)
			{
				if (!techGroupedDecals.ContainsKey(key))
				{
					techGroupedDecals.Add(key, new HashSet<EasyDecal>());
				}
				if (!flagsGroupedDecals.ContainsKey(key))
				{
					flagsGroupedDecals.Add(key, new Dictionary<DeferredFlags, HashSet<EasyDecal>>(dfc));
				}
				IList<DeferredFlags> allEnums = EnumHelper.GetAllEnums<DeferredFlags>();
				foreach (DeferredFlags item in allEnums)
				{
					if (!flagsGroupedDecals[key].ContainsKey(item))
					{
						flagsGroupedDecals[key].Add(item, new HashSet<EasyDecal>());
					}
				}
			}
		}
	}
}
