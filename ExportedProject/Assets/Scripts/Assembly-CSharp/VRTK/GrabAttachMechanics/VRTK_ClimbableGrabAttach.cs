using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	[RequireComponent(typeof(VRTK_InteractableObject))]
	public class VRTK_ClimbableGrabAttach : VRTK_BaseGrabAttach
	{
		[Tooltip("Will respect the grabbed climbing object's rotation if it changes dynamically")]
		public bool useObjectRotation;

		protected override void Initialise()
		{
			tracked = false;
			climbable = true;
			kinematic = true;
		}
	}
}
