using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Constant Motion Gear")]
	public class ConstantMotionGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public Modifier position = Modifier.Linear(0f, 1f);

		public Modifier rotation = Modifier.Linear(0f, 30f);

		private ConstantMotion motion;

		private void Awake()
		{
			reaktor.Initialize(this);
			motion = GetComponent<ConstantMotion>();
		}

		private void Update()
		{
			if (position.enabled)
			{
				motion.position.velocity = position.Evaluate(reaktor.Output);
			}
			if (rotation.enabled)
			{
				motion.rotation.velocity = rotation.Evaluate(reaktor.Output);
			}
		}
	}
}
