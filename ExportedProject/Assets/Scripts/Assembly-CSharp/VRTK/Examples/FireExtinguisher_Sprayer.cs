using UnityEngine;

namespace VRTK.Examples
{
	public class FireExtinguisher_Sprayer : VRTK_InteractableObject
	{
		public FireExtinguisher_Base baseCan;

		public float breakDistance = 0.12f;

		public float maxSprayPower = 5f;

		private GameObject waterSpray;

		private ParticleSystem particles;

		public void Spray(float power)
		{
			if (power <= 0f)
			{
				particles.Stop();
			}
			if (power > 0f)
			{
				if (particles.isPaused || particles.isStopped)
				{
					particles.Play();
				}
				ParticleSystem.MainModule main = particles.main;
				main.startSpeedMultiplier = maxSprayPower * power;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			waterSpray = base.transform.Find("WaterSpray").gameObject;
			particles = waterSpray.GetComponent<ParticleSystem>();
			particles.Stop();
		}

		protected override void Update()
		{
			base.Update();
			if (Vector3.Distance(base.transform.position, baseCan.transform.position) > breakDistance)
			{
				ForceStopInteracting();
			}
		}
	}
}
