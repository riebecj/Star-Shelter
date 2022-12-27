using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Animator Gear")]
	public class AnimatorGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public Modifier speed;

		public Trigger trigger;

		public string triggerName;

		private Animator animator;

		private void Awake()
		{
			reaktor.Initialize(this);
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			if (speed.enabled)
			{
				animator.speed = speed.Evaluate(reaktor.Output);
			}
			if (trigger.Update(reaktor.Output))
			{
				animator.SetTrigger(triggerName);
			}
		}
	}
}
