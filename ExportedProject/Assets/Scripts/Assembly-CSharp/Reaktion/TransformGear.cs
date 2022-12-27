using System;
using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Transform Gear")]
	public class TransformGear : MonoBehaviour
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

			public float min;

			public float max = 1f;

			public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

			public Vector3 arbitraryVector = Vector3.up;

			public float randomness;

			private Vector3 randomVector;

			private float randomScalar;

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

			public void Initialize()
			{
				randomVector = UnityEngine.Random.onUnitSphere;
				randomScalar = UnityEngine.Random.value;
			}

			public float GetScalar(float x)
			{
				float num = 1f - randomness * randomScalar;
				return Mathf.Lerp(min, max, num * curve.Evaluate(x));
			}
		}

		public ReaktorLink reaktor;

		public TransformElement position = new TransformElement();

		public TransformElement rotation = new TransformElement
		{
			max = 90f
		};

		public TransformElement scale = new TransformElement
		{
			arbitraryVector = Vector3.one,
			min = 1f,
			max = 2f
		};

		public bool addInitialValue = true;

		private Vector3 initialPosition;

		private Quaternion initialRotation;

		private Vector3 initialScale;

		private void Awake()
		{
			reaktor.Initialize(this);
			position.Initialize();
			rotation.Initialize();
			scale.Initialize();
		}

		private void OnEnable()
		{
			initialPosition = base.transform.localPosition;
			initialRotation = base.transform.localRotation;
			initialScale = base.transform.localScale;
		}

		private void Update()
		{
			float output = reaktor.Output;
			if (position.mode != 0)
			{
				base.transform.localPosition = position.Vector * position.GetScalar(output);
				if (addInitialValue)
				{
					base.transform.localPosition += initialPosition;
				}
			}
			if (rotation.mode != 0)
			{
				base.transform.localRotation = Quaternion.AngleAxis(rotation.GetScalar(output), rotation.Vector);
				if (addInitialValue)
				{
					base.transform.localRotation *= initialRotation;
				}
			}
			if (scale.mode != 0)
			{
				Vector3 b = Vector3.one + scale.Vector * (scale.GetScalar(output) - 1f);
				base.transform.localScale = Vector3.Scale((!addInitialValue) ? Vector3.one : initialScale, b);
			}
		}
	}
}
