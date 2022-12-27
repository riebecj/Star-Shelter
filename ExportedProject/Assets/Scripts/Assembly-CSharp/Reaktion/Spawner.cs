using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Utility/Spawner")]
	public class Spawner : MonoBehaviour
	{
		public enum Distribution
		{
			InSphere = 0,
			InBox = 1,
			AtPoints = 2
		}

		public GameObject[] prefabs;

		public float spawnRate;

		public float spawnRateRandomness;

		public Distribution distribution;

		public float sphereRadius;

		public Vector3 boxSize;

		public Transform[] spawnPoints;

		public bool randomRotation;

		public Transform parent;

		private float randomValue;

		private float timer;

		private int spawnPointIndex;

		public void Spawn()
		{
			GameObject gameObject = prefabs[Random.Range(0, prefabs.Length)];
			Vector3 position;
			Quaternion rotation;
			if (distribution == Distribution.AtPoints)
			{
				spawnPointIndex += Random.Range(1, spawnPoints.Length);
				spawnPointIndex %= spawnPoints.Length;
				Transform transform = spawnPoints[spawnPointIndex];
				position = transform.position;
				rotation = ((!randomRotation) ? (gameObject.transform.rotation * transform.rotation) : Random.rotation);
			}
			else
			{
				if (distribution == Distribution.InSphere)
				{
					position = base.transform.TransformPoint(Random.insideUnitSphere * sphereRadius);
				}
				else
				{
					Vector3 vector = new Vector3(Random.value, Random.value, Random.value);
					position = base.transform.TransformPoint(Vector3.Scale(vector - Vector3.one * 0.5f, boxSize));
				}
				rotation = ((!randomRotation) ? (gameObject.transform.rotation * base.transform.rotation) : Random.rotation);
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, position, rotation);
			if (parent != null)
			{
				gameObject2.transform.parent = parent;
			}
		}

		public void Spawn(int count)
		{
			while (count-- > 0)
			{
				Spawn();
			}
		}

		private void Update()
		{
			if (spawnRate > 0f)
			{
				timer += Time.deltaTime;
				while (timer > (1f - randomValue) / spawnRate)
				{
					Spawn();
					timer -= (1f - randomValue) / spawnRate;
					randomValue = Random.value * spawnRateRandomness;
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
			if (distribution == Distribution.AtPoints)
			{
				Transform[] array = spawnPoints;
				foreach (Transform transform in array)
				{
					Gizmos.DrawWireCube(transform.position, Vector3.one * 0.1f);
				}
			}
			else if (distribution == Distribution.InSphere)
			{
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawWireSphere(Vector3.zero, sphereRadius);
			}
			else
			{
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawWireCube(Vector3.zero, boxSize);
			}
		}
	}
}
