using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MirzaBeig.ParticleSystems.Demos
{
	public class ParticleManager : MonoBehaviour
	{
		protected List<ParticleSystems> particlePrefabs;

		public int currentParticlePrefabIndex;

		public List<ParticleSystems> particlePrefabsAppend;

		public int prefabNameUnderscoreCountCutoff = 4;

		public bool disableChildrenAtStart = true;

		private bool initialized;

		public void Init()
		{
			particlePrefabs = GetComponentsInChildren<ParticleSystems>(true).ToList();
			particlePrefabs.AddRange(particlePrefabsAppend);
			if (disableChildrenAtStart)
			{
				for (int i = 0; i < particlePrefabs.Count; i++)
				{
					particlePrefabs[i].gameObject.SetActive(false);
				}
			}
			initialized = true;
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			if (initialized)
			{
				Init();
			}
		}

		public virtual void Next()
		{
			currentParticlePrefabIndex++;
			if (currentParticlePrefabIndex > particlePrefabs.Count - 1)
			{
				currentParticlePrefabIndex = 0;
			}
		}

		public virtual void Previous()
		{
			currentParticlePrefabIndex--;
			if (currentParticlePrefabIndex < 0)
			{
				currentParticlePrefabIndex = particlePrefabs.Count - 1;
			}
		}

		public string GetCurrentPrefabName(bool shorten = false)
		{
			string text = particlePrefabs[currentParticlePrefabIndex].name;
			if (shorten)
			{
				int num = 0;
				for (int i = 0; i < prefabNameUnderscoreCountCutoff; i++)
				{
					num = text.IndexOf("_", num) + 1;
					if (num == 0)
					{
						MonoBehaviour.print("Iteration of underscore not found.");
						break;
					}
				}
				text = text.Substring(num, text.Length - num);
			}
			return "PARTICLE SYSTEM: #" + (currentParticlePrefabIndex + 1).ToString("00") + " / " + particlePrefabs.Count.ToString("00") + " (" + text + ")";
		}

		public virtual int GetParticleCount()
		{
			return 0;
		}

		protected virtual void Update()
		{
		}
	}
}
