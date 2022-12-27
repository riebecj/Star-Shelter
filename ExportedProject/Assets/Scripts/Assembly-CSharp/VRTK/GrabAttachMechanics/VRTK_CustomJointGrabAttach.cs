using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	public class VRTK_CustomJointGrabAttach : VRTK_BaseJointGrabAttach
	{
		[Tooltip("The joint to use for the grab attach joint.")]
		public Joint customJoint;

		protected GameObject jointHolder;

		protected override void Initialise()
		{
			base.Initialise();
			CopyCustomJoint();
		}

		protected override void CreateJoint(GameObject obj)
		{
			if (!jointHolder)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_CustomJointGrabAttach", "Joint", "customJoint", "the same"));
			}
			else
			{
				Joint component = jointHolder.GetComponent<Joint>();
				string text = base.gameObject.name;
				VRTK_SharedMethods.CloneComponent(component, obj, true);
				base.gameObject.name = text;
				givenJoint = obj.GetComponent(component.GetType()) as Joint;
				base.CreateJoint(obj);
			}
		}

		protected override void DestroyJoint(bool withDestroyImmediate, bool applyGrabbingObjectVelocity)
		{
			base.DestroyJoint(true, true);
		}

		protected virtual void CopyCustomJoint()
		{
			if ((bool)customJoint)
			{
				jointHolder = new GameObject();
				jointHolder.transform.SetParent(base.transform);
				jointHolder.AddComponent<Rigidbody>().isKinematic = true;
				VRTK_SharedMethods.CloneComponent(customJoint, jointHolder, true);
				jointHolder.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "JointHolder");
				jointHolder.SetActive(false);
				Object.Destroy(customJoint);
				customJoint = jointHolder.GetComponent<Joint>();
			}
		}
	}
}
