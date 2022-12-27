namespace MirzaBeig.ParticleSystems.Demos
{
	public class LoopingParticleSystemsManager : ParticleManager
	{
		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
			particlePrefabs[currentParticlePrefabIndex].gameObject.SetActive(true);
		}

		public override void Next()
		{
			particlePrefabs[currentParticlePrefabIndex].gameObject.SetActive(false);
			base.Next();
			particlePrefabs[currentParticlePrefabIndex].gameObject.SetActive(true);
		}

		public override void Previous()
		{
			particlePrefabs[currentParticlePrefabIndex].gameObject.SetActive(false);
			base.Previous();
			particlePrefabs[currentParticlePrefabIndex].gameObject.SetActive(true);
		}

		protected override void Update()
		{
			base.Update();
		}

		public override int GetParticleCount()
		{
			return particlePrefabs[currentParticlePrefabIndex].getParticleCount();
		}
	}
}
