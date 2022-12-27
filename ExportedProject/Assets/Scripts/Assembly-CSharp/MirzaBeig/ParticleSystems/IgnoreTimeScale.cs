using UnityEngine;

namespace MirzaBeig.ParticleSystems
{
	public class IgnoreTimeScale : MonoBehaviour
	{
		private ParticleSystem particleSystem;

		private void Awake()
		{
		}

		private void Start()
		{
			particleSystem = GetComponent<ParticleSystem>();
		}

		private void Update()
		{
			particleSystem.Simulate(Time.unscaledDeltaTime, true, false);
		}
	}
}
