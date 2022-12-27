using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.ParticleSystems.Demos
{
	public class OneshotParticleSystemsManager : ParticleManager
	{
		public LayerMask mouseRaycastLayerMask;

		private List<ParticleSystems> spawnedPrefabs;

		public bool disableSpawn { get; set; }

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
			disableSpawn = false;
			spawnedPrefabs = new List<ParticleSystems>();
		}

		private void OnEnable()
		{
		}

		public void Clear()
		{
			if (spawnedPrefabs == null)
			{
				return;
			}
			for (int i = 0; i < spawnedPrefabs.Count; i++)
			{
				if ((bool)spawnedPrefabs[i])
				{
					Object.Destroy(spawnedPrefabs[i].gameObject);
				}
			}
			spawnedPrefabs.Clear();
		}

		protected override void Update()
		{
			base.Update();
		}

		public void InstantiateParticlePrefab(Vector2 mousePosition, float maxDistance)
		{
			if (spawnedPrefabs != null && !disableSpawn)
			{
				Vector3 position = mousePosition;
				position.z = maxDistance;
				Vector3 vector = Camera.main.ScreenToWorldPoint(position);
				Vector3 direction = vector - Camera.main.transform.position;
				RaycastHit hitInfo;
				Physics.Raycast(Camera.main.transform.position + Camera.main.transform.forward * 0.01f, direction, out hitInfo, maxDistance);
				Vector3 position2 = ((!hitInfo.collider) ? vector : hitInfo.point);
				ParticleSystems particleSystems = particlePrefabs[currentParticlePrefabIndex];
				ParticleSystems particleSystems2 = Object.Instantiate(particleSystems, position2, particleSystems.transform.rotation);
				particleSystems2.gameObject.SetActive(true);
				particleSystems2.transform.parent = base.transform;
				spawnedPrefabs.Add(particleSystems2);
			}
		}

		public void Randomize()
		{
			currentParticlePrefabIndex = Random.Range(0, particlePrefabs.Count);
		}

		public override int GetParticleCount()
		{
			int num = 0;
			if (spawnedPrefabs != null)
			{
				for (int i = 0; i < spawnedPrefabs.Count; i++)
				{
					if ((bool)spawnedPrefabs[i])
					{
						num += spawnedPrefabs[i].getParticleCount();
					}
					else
					{
						spawnedPrefabs.RemoveAt(i);
					}
				}
			}
			return num;
		}
	}
}
