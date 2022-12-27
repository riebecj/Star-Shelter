using System;
using UnityEngine;
using UnityEngine.Events;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Generic Gradient Gear")]
	public class GenericGradientGear : MonoBehaviour
	{
		[Serializable]
		public class ColorEvent : UnityEvent<Color>
		{
		}

		public ReaktorLink reaktor;

		public Gradient gradient;

		public ColorEvent target;

		private void Awake()
		{
			reaktor.Initialize(this);
			UpdateTarget(0f);
		}

		private void Update()
		{
			UpdateTarget(reaktor.Output);
		}

		private void UpdateTarget(float param)
		{
			target.Invoke(gradient.Evaluate(param));
		}
	}
}
