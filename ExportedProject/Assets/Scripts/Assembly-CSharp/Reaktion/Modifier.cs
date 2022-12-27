using System;
using UnityEngine;

namespace Reaktion
{
	[Serializable]
	public class Modifier
	{
		public bool enabled;

		public float min;

		public float max = 1f;

		public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public float Evaluate(float i)
		{
			return Mathf.Lerp(min, max, curve.Evaluate(i));
		}

		public static Modifier Linear(float min, float max)
		{
			Modifier modifier = new Modifier();
			modifier.min = min;
			modifier.max = max;
			return modifier;
		}
	}
}
