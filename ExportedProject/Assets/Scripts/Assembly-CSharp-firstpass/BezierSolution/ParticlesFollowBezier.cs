using System.Collections.Generic;
using UnityEngine;

namespace BezierSolution
{
	[ExecuteInEditMode]
	public class ParticlesFollowBezier : MonoBehaviour
	{
		public enum FollowMode
		{
			Relaxed = 0,
			Strict = 1
		}

		private const int MAX_PARTICLE_COUNT = 25000;

		public BezierSpline spline;

		public FollowMode followMode;

		private Transform cachedTransform;

		private ParticleSystem cachedPS;

		private ParticleSystem.MainModule cachedMainModule;

		private ParticleSystem.Particle[] particles;

		private List<Vector4> particleData;

		private void Awake()
		{
			cachedTransform = base.transform;
			cachedPS = GetComponent<ParticleSystem>();
			cachedMainModule = cachedPS.main;
			particles = new ParticleSystem.Particle[cachedMainModule.maxParticles];
			if (followMode == FollowMode.Relaxed)
			{
				particleData = new List<Vector4>(particles.Length);
			}
		}

		private void LateUpdate()
		{
			if (spline == null || cachedPS == null)
			{
				return;
			}
			if (particles.Length < cachedMainModule.maxParticles && particles.Length < 25000)
			{
				particles = new ParticleSystem.Particle[Mathf.Min(cachedMainModule.maxParticles, 25000)];
			}
			bool flag = cachedMainModule.simulationSpace != ParticleSystemSimulationSpace.World;
			int num = cachedPS.GetParticles(particles);
			if (followMode == FollowMode.Relaxed)
			{
				if (particleData == null)
				{
					particleData = new List<Vector4>(particles.Length);
				}
				cachedPS.GetCustomParticleData(particleData, ParticleSystemCustomData.Custom1);
				for (int i = 0; i < num; i++)
				{
					Vector4 vector = particleData[i];
					Vector3 vector2 = spline.GetPoint(1f - particles[i].remainingLifetime / particles[i].startLifetime);
					if (flag)
					{
						vector2 = cachedTransform.InverseTransformPoint(vector2);
					}
					if (vector.w != 0f)
					{
						particles[i].position += vector2 - (Vector3)vector;
					}
					vector = vector2;
					vector.w = 1f;
					particleData[i] = vector;
				}
				cachedPS.SetCustomParticleData(particleData, ParticleSystemCustomData.Custom1);
			}
			else
			{
				Vector3 vector3 = cachedTransform.position - spline.GetPoint(0f);
				for (int j = 0; j < num; j++)
				{
					Vector3 position = spline.GetPoint(1f - particles[j].remainingLifetime / particles[j].startLifetime) + vector3;
					if (flag)
					{
						position = cachedTransform.InverseTransformPoint(position);
					}
					particles[j].position = position;
				}
			}
			cachedPS.SetParticles(particles, num);
		}
	}
}
