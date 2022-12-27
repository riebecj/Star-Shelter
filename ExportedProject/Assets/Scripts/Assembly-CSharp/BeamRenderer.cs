using UnityEngine;

public class BeamRenderer : MonoBehaviour
{
	public Transform target;

	public int particlesCount = 100;

	private ParticleSystem particleSystem;

	public ForceHook forceHook;

	public float distance = 5f;

	private Perlin noise;

	private float oneOverZigs;

	internal ParticleSystem.Particle[] particles;

	public static BeamRenderer instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	private void LateUpdate()
	{
		particles = new ParticleSystem.Particle[particleSystem.particleCount];
		particleSystem.GetParticles(particles);
		Vector3 position = base.transform.position;
		Vector3 vector = target.position - base.transform.position;
		Vector3 vector2 = target.position - (base.transform.position + base.transform.forward * Vector3.Distance(base.transform.position, target.position));
		int num = particles.Length;
		float num2 = 1f;
		Vector3[] array = new Vector3[num];
		array[0] = position;
		Vector3 vector3 = vector;
		float num3 = num2 / (float)num;
		float num4 = 0f;
		bool lockedObject = forceHook.lockedObject;
		for (int i = 1; i < num; i++)
		{
			array[i] = array[i - 1] + num3 * vector3;
			vector3 += num3 * vector2;
			particles[i].position = array[i];
			if (lockedObject)
			{
				num4 = Vector3.Distance(position, particles[i].position);
				particles[i].size = 0.03f + num4 / 15f;
			}
			else
			{
				particles[i].size = 0.04f;
			}
		}
		particleSystem.SetParticles(particles, particleSystem.particleCount);
	}
}
