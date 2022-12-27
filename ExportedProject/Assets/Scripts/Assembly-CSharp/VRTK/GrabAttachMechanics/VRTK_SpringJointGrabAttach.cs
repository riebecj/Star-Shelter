using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	public class VRTK_SpringJointGrabAttach : VRTK_BaseJointGrabAttach
	{
		[Tooltip("Maximum force the joint can withstand before breaking. Infinity means unbreakable.")]
		public float breakForce = 1500f;

		[Tooltip("The strength of the spring.")]
		public float strength = 500f;

		[Tooltip("The amount of dampening to apply to the spring.")]
		public float damper = 50f;

		protected override void CreateJoint(GameObject obj)
		{
			SpringJoint springJoint = obj.AddComponent<SpringJoint>();
			springJoint.breakForce = ((!grabbedObjectScript.IsDroppable()) ? float.PositiveInfinity : breakForce);
			springJoint.spring = strength;
			springJoint.damper = damper;
			givenJoint = springJoint;
			base.CreateJoint(obj);
		}
	}
}
