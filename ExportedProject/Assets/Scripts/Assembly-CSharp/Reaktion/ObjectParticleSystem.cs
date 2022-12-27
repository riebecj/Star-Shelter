using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Utility/Object Particle System")]
	[RequireComponent(typeof(ParticleSystem))]
	public class ObjectParticleSystem : MonoBehaviour
	{
		public GameObject prefab;

		public int maxParticles = 100;

		private ParticleSystem.Particle[] particles;

		private GameObject[] pool;

		private void Start()
		{
			int num = Mathf.Min(maxParticles, GetComponent<ParticleSystem>().maxParticles);
			particles = new ParticleSystem.Particle[num];
			pool = new GameObject[num];
			for (int i = 0; i < num; i++)
			{
				pool[i] = Object.Instantiate(prefab);
			}
		}

		private void LateUpdate()
		{
			int num = GetComponent<ParticleSystem>().GetParticles(particles);
			for (int i = 0; i < num; i++)
			{
				ParticleSystem.Particle particle = particles[i];
				GameObject gameObject = pool[i];
				gameObject.GetComponent<Renderer>().enabled = true;
				gameObject.transform.position = prefab.transform.position + particle.position;
				gameObject.transform.localRotation = Quaternion.AngleAxis(particle.rotation, particle.axisOfRotation) * prefab.transform.rotation;
				gameObject.transform.localScale = prefab.transform.localScale * particle.size;
			}
			for (int j = num; j < pool.Length; j++)
			{
				pool[j].GetComponent<Renderer>().enabled = false;
			}
		}
	}
}
