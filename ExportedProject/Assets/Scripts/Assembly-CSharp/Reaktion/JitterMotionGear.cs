using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Jitter Motion Gear")]
	public class JitterMotionGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public Modifier positionFrequency = Modifier.Linear(0f, 0.1f);

		public Modifier rotationFrequency = Modifier.Linear(0f, 0.1f);

		public Modifier positionAmount = Modifier.Linear(0f, 1f);

		public Modifier rotationAmount = Modifier.Linear(0f, 30f);

		private JitterMotion jitter;

		private void Awake()
		{
			reaktor.Initialize(this);
			jitter = GetComponent<JitterMotion>();
		}

		private void Update()
		{
			float output = reaktor.Output;
			if (positionFrequency.enabled)
			{
				jitter.positionFrequency = positionFrequency.Evaluate(output);
			}
			if (rotationFrequency.enabled)
			{
				jitter.rotationFrequency = rotationFrequency.Evaluate(output);
			}
			if (positionAmount.enabled)
			{
				jitter.positionAmount = positionAmount.Evaluate(output);
			}
			if (rotationAmount.enabled)
			{
				jitter.rotationAmount = rotationAmount.Evaluate(output);
			}
		}
	}
}
