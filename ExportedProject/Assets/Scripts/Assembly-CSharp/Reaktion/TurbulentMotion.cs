using UnityEngine;

namespace Reaktion
{
	public class TurbulentMotion : MonoBehaviour
	{
		public float density = 0.1f;

		public Vector3 linearFlow = Vector3.up * 0.2f;

		public Vector3 displacement = Vector3.one;

		public Vector3 rotation = Vector3.one * 60f;

		public Vector3 scale = Vector3.zero;

		public float coeffDisplacement = 1f;

		public float coeffRotation = 1.1f;

		public float coeffScale = 1.2f;

		public bool useLocalCoordinate = true;

		private Vector3 initialPosition;

		private Quaternion initialRotation;

		private Vector3 initialScale;

		private void OnEnable()
		{
			if (useLocalCoordinate)
			{
				initialPosition = base.transform.localPosition;
				initialRotation = base.transform.localRotation;
			}
			else
			{
				initialPosition = base.transform.position;
				initialRotation = base.transform.rotation;
			}
			initialScale = base.transform.localScale;
			ApplyTransform();
		}

		private void Update()
		{
			ApplyTransform();
		}

		private void ApplyTransform()
		{
			Vector3 vector = initialPosition * density + linearFlow * Time.time;
			Vector3 vector2 = new Vector3(13f, 17f, 19f);
			if (displacement != Vector3.zero)
			{
				Vector3 vector3 = vector * coeffDisplacement;
				Vector3 vector4 = new Vector3((displacement.x != 0f) ? (displacement.x * Perlin.Noise(vector3)) : 0f, (displacement.y != 0f) ? (displacement.y * Perlin.Noise(vector3 + vector2)) : 0f, (displacement.z != 0f) ? (displacement.z * Perlin.Noise(vector3 + vector2 * 2f)) : 0f);
				if (useLocalCoordinate)
				{
					base.transform.localPosition = initialPosition + vector4;
				}
				else
				{
					base.transform.position = initialPosition + vector4;
				}
			}
			if (rotation != Vector3.zero)
			{
				Vector3 vector5 = vector * coeffRotation;
				Vector3 euler = new Vector3((rotation.x != 0f) ? (rotation.x * Perlin.Noise(vector5 + vector2 * 3f)) : 0f, (rotation.y != 0f) ? (rotation.y * Perlin.Noise(vector5 + vector2 * 4f)) : 0f, (rotation.z != 0f) ? (rotation.z * Perlin.Noise(vector5 + vector2 * 5f)) : 0f);
				if (useLocalCoordinate)
				{
					base.transform.localRotation = Quaternion.Euler(euler) * initialRotation;
				}
				else
				{
					base.transform.rotation = Quaternion.Euler(euler) * initialRotation;
				}
			}
			if (scale != Vector3.zero)
			{
				Vector3 vector6 = vector * coeffScale;
				Vector3 b = new Vector3((scale.x != 0f) ? Mathf.Lerp(1f, (Perlin.Noise(vector6 + vector2 * 6f) * 1.25f + 1f) * 0.5f, scale.x) : 1f, (scale.y != 0f) ? Mathf.Lerp(1f, (Perlin.Noise(vector6 + vector2 * 7f) * 1.25f + 1f) * 0.5f, scale.y) : 1f, (scale.z != 0f) ? Mathf.Lerp(1f, (Perlin.Noise(vector6 + vector2 * 8f) * 1.25f + 1f) * 0.5f, scale.z) : 1f);
				base.transform.localScale = Vector3.Scale(initialScale, b);
			}
		}
	}
}
