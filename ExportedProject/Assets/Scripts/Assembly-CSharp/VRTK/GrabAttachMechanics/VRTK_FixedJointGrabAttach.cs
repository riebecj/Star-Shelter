using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	public class VRTK_FixedJointGrabAttach : VRTK_BaseJointGrabAttach
	{
		[Tooltip("Maximum force the joint can withstand before breaking. Infinity means unbreakable.")]
		public float breakForce = 1500f;

		protected override void CreateJoint(GameObject obj)
		{
			givenJoint = obj.AddComponent<FixedJoint>();
			givenJoint.breakForce = ((!grabbedObjectScript.IsDroppable()) ? float.PositiveInfinity : breakForce);
			base.CreateJoint(obj);
		}
	}
}
