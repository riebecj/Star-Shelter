using System;
using UnityEngine;

namespace Reaktion
{
	public class JitterMotion : MonoBehaviour
	{
		public float positionFrequency = 0.2f;

		public float rotationFrequency = 0.2f;

		public float positionAmount = 1f;

		public float rotationAmount = 30f;

		public Vector3 positionComponents = Vector3.one;

		public Vector3 rotationComponents = new Vector3(1f, 1f, 0f);

		public int positionOctave = 3;

		public int rotationOctave = 3;

		private float timePosition;

		private float timeRotation;

		private Vector2[] noiseVectors;

		private Vector3 initialPosition;

		private Quaternion initialRotation;

		private void Awake()
		{
			timePosition = UnityEngine.Random.value * 10f;
			timeRotation = UnityEngine.Random.value * 10f;
			noiseVectors = new Vector2[6];
			for (int i = 0; i < 6; i++)
			{
				float f = UnityEngine.Random.value * (float)Math.PI * 2f;
				noiseVectors[i].Set(Mathf.Cos(f), Mathf.Sin(f));
			}
			initialPosition = base.transform.localPosition;
			initialRotation = base.transform.localRotation;
		}

		private void Update()
		{
			timePosition += Time.deltaTime * positionFrequency;
			timeRotation += Time.deltaTime * rotationFrequency;
			if (positionAmount != 0f)
			{
				Vector3 a = new Vector3(Fbm(noiseVectors[0] * timePosition, positionOctave), Fbm(noiseVectors[1] * timePosition, positionOctave), Fbm(noiseVectors[2] * timePosition, positionOctave));
				a = Vector3.Scale(a, positionComponents) * positionAmount * 2f;
				base.transform.localPosition = initialPosition + a;
			}
			if (rotationAmount != 0f)
			{
				Vector3 a2 = new Vector3(Fbm(noiseVectors[3] * timeRotation, rotationOctave), Fbm(noiseVectors[4] * timeRotation, rotationOctave), Fbm(noiseVectors[5] * timeRotation, rotationOctave));
				a2 = Vector3.Scale(a2, rotationComponents) * rotationAmount * 2f;
				base.transform.localRotation = Quaternion.Euler(a2) * initialRotation;
			}
		}

		private static float Fbm(Vector2 coord, int octave)
		{
			float num = 0f;
			float num2 = 1f;
			for (int i = 0; i < octave; i++)
			{
				num += num2 * Perlin.Noise(coord.x, coord.y) * 0.5f;
				coord *= 2f;
				num2 *= 0.5f;
			}
			return num;
		}
	}
}
