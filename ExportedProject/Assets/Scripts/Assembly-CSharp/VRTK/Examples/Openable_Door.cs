using UnityEngine;

namespace VRTK.Examples
{
	public class Openable_Door : VRTK_InteractableObject
	{
		public bool flipped;

		public bool rotated;

		private float sideFlip = -1f;

		private float side = -1f;

		private float smooth = 270f;

		private float doorOpenAngle = -90f;

		private bool open;

		private Vector3 defaultRotation;

		private Vector3 openRotation;

		public override void StartUsing(GameObject usingObject)
		{
			base.StartUsing(usingObject);
			SetDoorRotation(usingObject.transform.position);
			SetRotation();
			open = !open;
		}

		protected void Start()
		{
			defaultRotation = base.transform.eulerAngles;
			SetRotation();
			sideFlip = (flipped ? 1 : (-1));
		}

		protected override void Update()
		{
			base.Update();
			if (open)
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(openRotation), Time.deltaTime * smooth);
			}
			else
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(defaultRotation), Time.deltaTime * smooth);
			}
		}

		private void SetRotation()
		{
			openRotation = new Vector3(defaultRotation.x, defaultRotation.y + doorOpenAngle * (sideFlip * side), defaultRotation.z);
		}

		private void SetDoorRotation(Vector3 interacterPosition)
		{
			side = (((rotated || !(interacterPosition.z > base.transform.position.z)) && (!rotated || !(interacterPosition.x > base.transform.position.x))) ? 1 : (-1));
		}
	}
}
