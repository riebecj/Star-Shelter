using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Particle System Gear")]
	public class ParticleSystemGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public Trigger burst;

		public int burstNumber = 10;

		public Modifier emissionRate = Modifier.Linear(0f, 20f);

		public Modifier size = Modifier.Linear(0.5f, 1.5f);

		private ParticleSystem.Particle[] tempArray;

		private void Awake()
		{
			reaktor.Initialize(this);
		}

		private void Update()
		{
			if (burst.Update(reaktor.Output))
			{
				GetComponent<ParticleSystem>().Emit(burstNumber);
				GetComponent<ParticleSystem>().Play();
			}
			if (emissionRate.enabled)
			{
				GetComponent<ParticleSystem>().emissionRate = emissionRate.Evaluate(reaktor.Output);
			}
			if (size.enabled)
			{
				ResizeParticles(size.Evaluate(reaktor.Output));
			}
		}

		private void ResizeParticles(float newSize)
		{
			if (tempArray == null || tempArray.Length != GetComponent<ParticleSystem>().maxParticles)
			{
				tempArray = new ParticleSystem.Particle[GetComponent<ParticleSystem>().maxParticles];
			}
			int particles = GetComponent<ParticleSystem>().GetParticles(tempArray);
			for (int i = 0; i < particles; i++)
			{
				tempArray[i].size = newSize;
			}
			GetComponent<ParticleSystem>().SetParticles(tempArray, particles);
		}
	}
}
