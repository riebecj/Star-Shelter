using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Spawner Gear")]
	public class SpawnerGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public Trigger burst;

		public int burstNumber = 5;

		public Modifier spawnRate = Modifier.Linear(0f, 10f);

		private Spawner spawner;

		private void Awake()
		{
			reaktor.Initialize(this);
			spawner = GetComponent<Spawner>();
		}

		private void Update()
		{
			if (burst.Update(reaktor.Output))
			{
				spawner.Spawn(burstNumber);
			}
			if (spawnRate.enabled)
			{
				spawner.spawnRate = spawnRate.Evaluate(reaktor.Output);
			}
		}
	}
}
