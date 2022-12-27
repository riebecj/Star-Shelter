using System;
using UnityEngine;

namespace Reaktion
{
	public class VariableMotion : MonoBehaviour
	{
		public enum TransformMode
		{
			Off = 0,
			XAxis = 1,
			YAxis = 2,
			ZAxis = 3,
			Arbitrary = 4,
			Random = 5
		}

		[Serializable]
		public class TransformElement
		{
			public TransformMode mode;

			public AnimationCurve curve = AnimationCurve.Linear(0f, -1f, 1f, 1f);

			public float amplitude = 1f;

			public float speed = 1f;

			public Vector3 arbitraryVector = Vector3.up;

			public float randomness;

			private Vector3 randomVector;

			private float randomAmplitude;

			private float time;

			public Vector3 Vector
			{
				get
				{
					switch (mode)
					{
					case TransformMode.XAxis:
						return Vector3.right;
					case TransformMode.YAxis:
						return Vector3.up;
					case TransformMode.ZAxis:
						return Vector3.forward;
					case TransformMode.Arbitrary:
						return arbitraryVector;
					case TransformMode.Random:
						return randomVector;
					default:
						return Vector3.zero;
					}
				}
			}

			public float Scalar
			{
				get
				{
					float num = amplitude * (1f - randomAmplitude * randomness);
					return curve.Evaluate(time) * num;
				}
			}

			public void Initialize()
			{
				randomVector = UnityEngine.Random.onUnitSphere;
				randomAmplitude = UnityEngine.Random.value;
				time = 0f;
			}

			public void Step()
			{
				time += Time.deltaTime * speed;
			}
		}

		public TransformElement position = new TransformElement();

		public TransformElement rotation = new TransformElement
		{
			amplitude = 90f
		};

		public TransformElement scale = new TransformElement
		{
			curve = AnimationCurve.Linear(0f, 0f, 1f, 1f),
			arbitraryVector = Vector3.one
		};

		public bool useLocalCoordinate = true;

		public bool useDifferentials;

		private Vector3 previousPosition;

		private Quaternion previousRotation;

		private Vector3 initialScale;

		public void Rewind()
		{
			position.Initialize();
			rotation.Initialize();
			scale.Initialize();
			previousPosition = position.Vector * position.Scalar;
			previousRotation = Quaternion.AngleAxis(rotation.Scalar, rotation.Vector);
			initialScale = base.transform.localScale;
			ApplyTransform();
		}

		private void OnEnable()
		{
			Rewind();
		}

		private void Update()
		{
			position.Step();
			rotation.Step();
			scale.Step();
			ApplyTransform();
		}

		private void ApplyTransform()
		{
			Vector3 vector = position.Vector * position.Scalar;
			Quaternion quaternion = Quaternion.AngleAxis(rotation.Scalar, rotation.Vector);
			if (position.mode != 0)
			{
				if (useDifferentials)
				{
					if (useLocalCoordinate)
					{
						base.transform.localPosition += vector - previousPosition;
					}
					else
					{
						base.transform.position += vector - previousPosition;
					}
				}
				else if (useLocalCoordinate)
				{
					base.transform.localPosition = vector;
				}
				else
				{
					base.transform.position = vector;
				}
			}
			if (rotation.mode != 0)
			{
				if (useDifferentials)
				{
					Quaternion quaternion2 = quaternion * Quaternion.Inverse(previousRotation);
					if (useLocalCoordinate)
					{
						base.transform.localRotation = quaternion2 * base.transform.localRotation;
					}
					else
					{
						base.transform.rotation = quaternion2 * base.transform.rotation;
					}
				}
				else if (useLocalCoordinate)
				{
					base.transform.localRotation = quaternion;
				}
				else
				{
					base.transform.rotation = quaternion;
				}
			}
			previousPosition = vector;
			previousRotation = quaternion;
			if (scale.mode != 0)
			{
				Vector3 a = ((!useDifferentials) ? Vector3.one : initialScale);
				Vector3 b = Vector3.one + scale.Vector * (scale.Scalar - 1f);
				base.transform.localScale = Vector3.Scale(a, b);
			}
		}
	}
}
