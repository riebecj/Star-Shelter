using UnityEngine;

namespace MirzaBeig.ParticleSystems
{
	public class DestroyOnTrailsDestroyed : TrailRenderers
	{
		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
			bool flag = true;
			for (int i = 0; i < trailRenderers.Length; i++)
			{
				if (trailRenderers[i] != null)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
