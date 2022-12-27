using System;
using UnityEngine;
using UnityEngine.Events;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Generic Curve Gear")]
	public class GenericCurveGear : MonoBehaviour
	{
		public enum OptionType
		{
			Bool = 0,
			Int = 1,
			Float = 2,
			Vector = 3
		}

		[Serializable]
		public class BoolEvent : UnityEvent<bool>
		{
		}

		[Serializable]
		public class IntEvent : UnityEvent<int>
		{
		}

		[Serializable]
		public class FloatEvent : UnityEvent<float>
		{
		}

		[Serializable]
		public class VectorEvent : UnityEvent<Vector3>
		{
		}

		public ReaktorLink reaktor;

		public OptionType optionType = OptionType.Float;

		public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public Vector3 origin;

		public Vector3 direction = Vector3.right;

		public BoolEvent boolTarget;

		public IntEvent intTarget;

		public FloatEvent floatTarget;

		public VectorEvent vectorTarget;

		private void Awake()
		{
			reaktor.Initialize(this);
		}

		private void Update()
		{
			float num = curve.Evaluate(reaktor.Output);
			if (optionType == OptionType.Bool)
			{
				boolTarget.Invoke(0.5f <= num);
			}
			else if (optionType == OptionType.Int)
			{
				intTarget.Invoke((int)num);
			}
			else if (optionType == OptionType.Vector)
			{
				vectorTarget.Invoke(origin + direction * num);
			}
			else
			{
				floatTarget.Invoke(num);
			}
		}
	}
}
