using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.Scripting.Effects
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticlePlexus : MonoBehaviour
	{
		public float maxDistance = 1f;

		public int maxConnections = 5;

		public int maxLineRenderers = 100;

		[Range(0f, 1f)]
		public float widthFromParticle = 0.125f;

		[Range(0f, 1f)]
		public float colourFromParticle = 1f;

		[Range(0f, 1f)]
		public float alphaFromParticle = 1f;

		private ParticleSystem particleSystem;

		private ParticleSystem.Particle[] particles;

		private Vector3[] particlePositions;

		private Color[] particleColours;

		private float[] particleSizes;

		private ParticleSystem.MainModule particleSystemMainModule;

		public LineRenderer lineRendererTemplate;

		private List<LineRenderer> lineRenderers = new List<LineRenderer>();

		private Transform _transform;

		[Header("General Performance Settings")]
		[Range(0f, 1f)]
		public float delay;

		private float timer;

		public bool alwaysUpdate;

		private bool visible;

		private void Start()
		{
			particleSystem = GetComponent<ParticleSystem>();
			particleSystemMainModule = particleSystem.main;
			_transform = base.transform;
		}

		private void OnDisable()
		{
			for (int i = 0; i < lineRenderers.Count; i++)
			{
				lineRenderers[i].enabled = false;
			}
		}

		private void OnBecameVisible()
		{
			visible = true;
		}

		private void OnBecameInvisible()
		{
			visible = false;
		}

		private void LateUpdate()
		{
			int num = lineRenderers.Count;
			if (num > maxLineRenderers)
			{
				for (int i = maxLineRenderers; i < num; i++)
				{
					UnityEngine.Object.Destroy(lineRenderers[i].gameObject);
				}
				lineRenderers.RemoveRange(maxLineRenderers, num - maxLineRenderers);
				num -= num - maxLineRenderers;
			}
			if (!alwaysUpdate && !visible)
			{
				return;
			}
			int maxParticles = particleSystemMainModule.maxParticles;
			if (particles == null || particles.Length < maxParticles)
			{
				particles = new ParticleSystem.Particle[maxParticles];
				particlePositions = new Vector3[maxParticles];
				particleColours = new Color[maxParticles];
				particleSizes = new float[maxParticles];
			}
			timer += Time.deltaTime;
			if (!(timer >= delay))
			{
				return;
			}
			timer = 0f;
			int num2 = 0;
			if (maxConnections > 0 && maxLineRenderers > 0)
			{
				particleSystem.GetParticles(particles);
				int particleCount = particleSystem.particleCount;
				float num3 = maxDistance * maxDistance;
				ParticleSystemSimulationSpace simulationSpace = particleSystemMainModule.simulationSpace;
				ParticleSystemScalingMode scalingMode = particleSystemMainModule.scalingMode;
				Transform customSimulationSpace = particleSystemMainModule.customSimulationSpace;
				Color startColor = lineRendererTemplate.startColor;
				Color endColor = lineRendererTemplate.endColor;
				float a = lineRendererTemplate.startWidth * lineRendererTemplate.widthMultiplier;
				float a2 = lineRendererTemplate.endWidth * lineRendererTemplate.widthMultiplier;
				for (int j = 0; j < particleCount; j++)
				{
					particlePositions[j] = particles[j].position;
					particleColours[j] = particles[j].GetCurrentColor(particleSystem);
					particleSizes[j] = particles[j].GetCurrentSize(particleSystem);
				}
				Vector3 vector = default(Vector3);
				if (simulationSpace == ParticleSystemSimulationSpace.World)
				{
					for (int k = 0; k < particleCount; k++)
					{
						if (num2 == maxLineRenderers)
						{
							break;
						}
						Color b = particleColours[k];
						Color startColor2 = Color.LerpUnclamped(startColor, b, colourFromParticle);
						startColor2.a = Mathf.LerpUnclamped(startColor.a, b.a, alphaFromParticle);
						float startWidth = Mathf.LerpUnclamped(a, particleSizes[k], widthFromParticle);
						int num4 = 0;
						for (int l = k + 1; l < particleCount; l++)
						{
							vector.x = particlePositions[k].x - particlePositions[l].x;
							vector.y = particlePositions[k].y - particlePositions[l].y;
							vector.z = particlePositions[k].z - particlePositions[l].z;
							float num5 = vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
							if (num5 <= num3)
							{
								LineRenderer item;
								if (num2 == num)
								{
									item = UnityEngine.Object.Instantiate(lineRendererTemplate, _transform, false);
									lineRenderers.Add(item);
									num++;
								}
								item = lineRenderers[num2];
								item.enabled = true;
								item.SetPosition(0, particlePositions[k]);
								item.SetPosition(1, particlePositions[l]);
								item.startColor = startColor2;
								b = particleColours[l];
								Color endColor2 = Color.LerpUnclamped(endColor, b, colourFromParticle);
								endColor2.a = Mathf.LerpUnclamped(endColor.a, b.a, alphaFromParticle);
								item.endColor = endColor2;
								item.startWidth = startWidth;
								item.endWidth = Mathf.LerpUnclamped(a2, particleSizes[l], widthFromParticle);
								num2++;
								num4++;
								if (num4 == maxConnections || num2 == maxLineRenderers)
								{
									break;
								}
							}
						}
					}
				}
				else
				{
					Vector3 zero = Vector3.zero;
					Quaternion identity = Quaternion.identity;
					Vector3 one = Vector3.one;
					Transform transform = _transform;
					switch (simulationSpace)
					{
					case ParticleSystemSimulationSpace.Local:
						zero = transform.position;
						identity = transform.rotation;
						one = transform.localScale;
						break;
					case ParticleSystemSimulationSpace.Custom:
						transform = customSimulationSpace;
						zero = transform.position;
						identity = transform.rotation;
						one = transform.localScale;
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported scaling mode '{0}'.", simulationSpace));
					}
					Vector3 vector2 = Vector3.zero;
					Vector3 vector3 = Vector3.zero;
					for (int m = 0; m < particleCount; m++)
					{
						if (num2 == maxLineRenderers)
						{
							break;
						}
						if (simulationSpace == ParticleSystemSimulationSpace.Local || simulationSpace == ParticleSystemSimulationSpace.Custom)
						{
							switch (scalingMode)
							{
							case ParticleSystemScalingMode.Hierarchy:
								vector2 = transform.TransformPoint(particlePositions[m]);
								break;
							case ParticleSystemScalingMode.Local:
								vector2.x = particlePositions[m].x * one.x;
								vector2.y = particlePositions[m].y * one.y;
								vector2.z = particlePositions[m].z * one.z;
								vector2 = identity * vector2;
								vector2.x += zero.x;
								vector2.y += zero.y;
								vector2.z += zero.z;
								break;
							case ParticleSystemScalingMode.Shape:
								vector2 = identity * particlePositions[m];
								vector2.x += zero.x;
								vector2.y += zero.y;
								vector2.z += zero.z;
								break;
							default:
								throw new NotSupportedException(string.Format("Unsupported scaling mode '{0}'.", scalingMode));
							}
						}
						Color b2 = particleColours[m];
						Color startColor3 = Color.LerpUnclamped(startColor, b2, colourFromParticle);
						startColor3.a = Mathf.LerpUnclamped(startColor.a, b2.a, alphaFromParticle);
						float startWidth2 = Mathf.LerpUnclamped(a, particleSizes[m], widthFromParticle);
						int num6 = 0;
						for (int n = m + 1; n < particleCount; n++)
						{
							if (simulationSpace == ParticleSystemSimulationSpace.Local || simulationSpace == ParticleSystemSimulationSpace.Custom)
							{
								switch (scalingMode)
								{
								case ParticleSystemScalingMode.Hierarchy:
									vector3 = transform.TransformPoint(particlePositions[n]);
									break;
								case ParticleSystemScalingMode.Local:
									vector3.x = particlePositions[n].x * one.x;
									vector3.y = particlePositions[n].y * one.y;
									vector3.z = particlePositions[n].z * one.z;
									vector3 = identity * vector3;
									vector3.x += zero.x;
									vector3.y += zero.y;
									vector3.z += zero.z;
									break;
								case ParticleSystemScalingMode.Shape:
									vector3 = identity * particlePositions[n];
									vector3.x += zero.x;
									vector3.y += zero.y;
									vector3.z += zero.z;
									break;
								default:
									throw new NotSupportedException(string.Format("Unsupported scaling mode '{0}'.", scalingMode));
								}
							}
							vector.x = particlePositions[m].x - particlePositions[n].x;
							vector.y = particlePositions[m].y - particlePositions[n].y;
							vector.z = particlePositions[m].z - particlePositions[n].z;
							float num7 = vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
							if (num7 <= num3)
							{
								LineRenderer item2;
								if (num2 == num)
								{
									item2 = UnityEngine.Object.Instantiate(lineRendererTemplate, _transform, false);
									lineRenderers.Add(item2);
									num++;
								}
								item2 = lineRenderers[num2];
								item2.enabled = true;
								item2.SetPosition(0, vector2);
								item2.SetPosition(1, vector3);
								item2.startColor = startColor3;
								b2 = particleColours[n];
								Color endColor3 = Color.LerpUnclamped(endColor, b2, colourFromParticle);
								endColor3.a = Mathf.LerpUnclamped(endColor.a, b2.a, alphaFromParticle);
								item2.endColor = endColor3;
								item2.startWidth = startWidth2;
								item2.endWidth = Mathf.LerpUnclamped(a2, particleSizes[n], widthFromParticle);
								num2++;
								num6++;
								if (num6 == maxConnections || num2 == maxLineRenderers)
								{
									break;
								}
							}
						}
					}
				}
			}
			for (int num8 = num2; num8 < num; num8++)
			{
				if (lineRenderers[num8].enabled)
				{
					lineRenderers[num8].enabled = false;
				}
			}
		}
	}
}
