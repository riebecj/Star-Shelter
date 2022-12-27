using UnityEngine;

namespace VRTK
{
	public class VRTK_SpringLever : VRTK_Lever
	{
		[Tooltip("The strength of the spring force that will be applied upon the lever.")]
		public float springStrength = 10f;

		[Tooltip("The damper of the spring force that will be applied upon the lever.")]
		public float springDamper = 10f;

		[Tooltip("If this is checked then the spring will snap the lever to the nearest end point (either min or max angle). If it is unchecked, the lever will always snap to the min angle position.")]
		public bool snapToNearestLimit;

		[Tooltip("If this is checked then the spring will always be active even when grabbing the lever.")]
		public bool alwaysActive;

		protected bool wasTowardZero = true;

		protected bool isGrabbed;

		protected override void InitRequiredComponents()
		{
			base.InitRequiredComponents();
			if (!leverHingeJoint.useSpring)
			{
				leverHingeJoint.useSpring = true;
				JointSpring spring = leverHingeJoint.spring;
				spring.spring = springStrength;
				spring.damper = springDamper;
				spring.targetPosition = minAngle;
				leverHingeJoint.spring = spring;
			}
			else
			{
				springStrength = leverHingeJoint.spring.spring;
			}
		}

		protected override void HandleUpdate()
		{
			base.HandleUpdate();
			ApplySpringForce();
		}

		protected override void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
		{
			base.InteractableObjectGrabbed(sender, e);
			isGrabbed = true;
		}

		protected override void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
		{
			base.InteractableObjectUngrabbed(sender, e);
			isGrabbed = false;
		}

		protected virtual float GetSpringTarget(bool towardZero)
		{
			return (!towardZero) ? maxAngle : minAngle;
		}

		protected virtual void ApplySpringForce()
		{
			leverHingeJoint.useSpring = alwaysActive || !isGrabbed;
			if (leverHingeJoint.useSpring)
			{
				bool flag = !snapToNearestLimit || GetNormalizedValue() <= 50f;
				if (flag != wasTowardZero)
				{
					JointSpring spring = leverHingeJoint.spring;
					spring.targetPosition = GetSpringTarget(flag);
					leverHingeJoint.spring = spring;
					wasTowardZero = flag;
				}
			}
		}
	}
}
