using UnityEngine;
using UnityEngine.Events;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Generic Trigger Gear")]
	public class GenericTriggerGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public float threshold = 0.9f;

		public float interval = 0.1f;

		public UnityEvent target;

		private float previousOutput;

		private float triggerTimer;

		private void Awake()
		{
			reaktor.Initialize(this);
		}

		private void Update()
		{
			if (triggerTimer <= 0f && reaktor.Output >= threshold && previousOutput < threshold)
			{
				target.Invoke();
				triggerTimer = interval;
			}
			else
			{
				triggerTimer -= Time.deltaTime;
			}
			previousOutput = reaktor.Output;
		}
	}
}
