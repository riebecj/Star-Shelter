using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VRTK
{
	public class VRTK_Drawer : VRTK_Control
	{
		[Tooltip("An optional game object to which the drawer will be connected. If the game object moves the drawer will follow along.")]
		public GameObject connectedTo;

		[Tooltip("The axis on which the drawer should open. All other axis will be frozen.")]
		public Direction direction;

		[Tooltip("The game object for the body.")]
		public GameObject body;

		[Tooltip("The game object for the handle.")]
		public GameObject handle;

		[Tooltip("The parent game object for the drawer content elements.")]
		public GameObject content;

		[Tooltip("Makes the content invisible while the drawer is closed.")]
		public bool hideContent = true;

		[Tooltip("If the extension of the drawer is below this percentage then the drawer will snap shut.")]
		[Range(0f, 1f)]
		public float minSnapClose = 1f;

		[Tooltip("The maximum percentage of the drawer's total length that the drawer will open to.")]
		[Range(0f, 1f)]
		public float maxExtend = 1f;

		protected Rigidbody drawerRigidbody;

		protected Rigidbody handleRigidbody;

		protected FixedJoint handleFixedJoint;

		protected ConfigurableJoint drawerJoint;

		protected VRTK_InteractableObject drawerInteractableObject;

		protected ConstantForce drawerSnapForce;

		protected Direction finalDirection;

		protected float subDirection = 1f;

		protected float pullDistance;

		protected Vector3 initialPosition;

		protected bool drawerJointCreated;

		protected bool drawerSnapForceCreated;

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (base.enabled && setupSuccessful)
			{
				Bounds bounds = VRTK_SharedMethods.GetBounds(GetHandle().transform, GetHandle().transform);
				float num = bounds.extents.y * ((!handle) ? 1f : 5f);
				Vector3 center = bounds.center;
				switch (finalDirection)
				{
				case Direction.x:
					center -= base.transform.right.normalized * (num * subDirection);
					break;
				case Direction.y:
					center -= base.transform.up.normalized * (num * subDirection);
					break;
				case Direction.z:
					center -= base.transform.forward.normalized * (num * subDirection);
					break;
				}
				Gizmos.DrawLine(bounds.center, center);
				Gizmos.DrawSphere(center, num / 4f);
			}
		}

		protected override void InitRequiredComponents()
		{
			initialPosition = base.transform.position;
			InitBody();
			InitHandle();
			SetContent(content, hideContent);
		}

		protected override bool DetectSetup()
		{
			finalDirection = ((direction != 0) ? direction : DetectDirection());
			if (finalDirection == Direction.autodetect)
			{
				return false;
			}
			Bounds bounds = VRTK_SharedMethods.GetBounds(GetHandle().transform, base.transform);
			Bounds bounds2 = VRTK_SharedMethods.GetBounds(GetBody().transform, base.transform);
			switch (finalDirection)
			{
			case Direction.x:
				subDirection = ((!(bounds.center.x > bounds2.center.x)) ? 1 : (-1));
				pullDistance = bounds2.extents.x;
				break;
			case Direction.y:
				subDirection = ((!(bounds.center.y > bounds2.center.y)) ? 1 : (-1));
				pullDistance = bounds2.extents.y;
				break;
			case Direction.z:
				subDirection = ((!(bounds.center.z > bounds2.center.z)) ? 1 : (-1));
				pullDistance = bounds2.extents.z;
				break;
			}
			if (((bool)body & (bool)handle) && handle.transform.IsChildOf(body.transform))
			{
				return false;
			}
			if (drawerJointCreated)
			{
				drawerJoint.xMotion = ConfigurableJointMotion.Locked;
				drawerJoint.yMotion = ConfigurableJointMotion.Locked;
				drawerJoint.zMotion = ConfigurableJointMotion.Locked;
				switch (finalDirection)
				{
				case Direction.x:
					drawerJoint.axis = Vector3.right;
					drawerJoint.xMotion = ConfigurableJointMotion.Limited;
					break;
				case Direction.y:
					drawerJoint.axis = Vector3.up;
					drawerJoint.yMotion = ConfigurableJointMotion.Limited;
					break;
				case Direction.z:
					drawerJoint.axis = Vector3.forward;
					drawerJoint.zMotion = ConfigurableJointMotion.Limited;
					break;
				}
				drawerJoint.anchor = drawerJoint.axis * ((0f - subDirection) * pullDistance);
			}
			if ((bool)drawerJoint)
			{
				drawerJoint.angularXMotion = ConfigurableJointMotion.Locked;
				drawerJoint.angularYMotion = ConfigurableJointMotion.Locked;
				drawerJoint.angularZMotion = ConfigurableJointMotion.Locked;
				pullDistance *= maxExtend * 1.8f;
				SoftJointLimit linearLimit = drawerJoint.linearLimit;
				linearLimit.limit = pullDistance;
				drawerJoint.linearLimit = linearLimit;
				if ((bool)connectedTo)
				{
					drawerJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
				}
			}
			if (drawerSnapForceCreated)
			{
				drawerSnapForce.force = GetThirdDirection(drawerJoint.axis, drawerJoint.secondaryAxis) * (subDirection * -50f);
			}
			return true;
		}

		protected override ControlValueRange RegisterValueRange()
		{
			ControlValueRange result = default(ControlValueRange);
			result.controlMin = 0f;
			result.controlMax = 100f;
			return result;
		}

		protected override void HandleUpdate()
		{
			value = CalculateValue();
			bool flag = Mathf.Abs(value) < minSnapClose * 100f;
			drawerSnapForce.enabled = flag;
			if ((bool)autoTriggerVolume)
			{
				autoTriggerVolume.isEnabled = !flag;
			}
		}

		protected virtual void InitBody()
		{
			drawerRigidbody = GetComponent<Rigidbody>();
			if (drawerRigidbody == null)
			{
				drawerRigidbody = base.gameObject.AddComponent<Rigidbody>();
				drawerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
			drawerRigidbody.isKinematic = false;
			drawerInteractableObject = GetComponent<VRTK_InteractableObject>();
			if (drawerInteractableObject == null)
			{
				drawerInteractableObject = base.gameObject.AddComponent<VRTK_InteractableObject>();
			}
			drawerInteractableObject.isGrabbable = true;
			drawerInteractableObject.grabAttachMechanicScript = base.gameObject.AddComponent<VRTK_SpringJointGrabAttach>();
			drawerInteractableObject.grabAttachMechanicScript.precisionGrab = true;
			drawerInteractableObject.secondaryGrabActionScript = base.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
			drawerInteractableObject.stayGrabbedOnTeleport = false;
			if ((bool)connectedTo)
			{
				Rigidbody component = connectedTo.GetComponent<Rigidbody>();
				if (component == null)
				{
					component = connectedTo.AddComponent<Rigidbody>();
					component.useGravity = false;
					component.isKinematic = true;
				}
			}
			drawerJoint = GetComponent<ConfigurableJoint>();
			if (drawerJoint == null)
			{
				drawerJoint = base.gameObject.AddComponent<ConfigurableJoint>();
				drawerJointCreated = true;
			}
			drawerSnapForce = GetComponent<ConstantForce>();
			if (drawerSnapForce == null)
			{
				drawerSnapForce = base.gameObject.AddComponent<ConstantForce>();
				drawerSnapForce.enabled = false;
				drawerSnapForceCreated = true;
			}
		}

		protected virtual void InitHandle()
		{
			handleRigidbody = GetHandle().GetComponent<Rigidbody>();
			if (handleRigidbody == null)
			{
				handleRigidbody = GetHandle().AddComponent<Rigidbody>();
			}
			handleRigidbody.isKinematic = false;
			handleRigidbody.useGravity = false;
			handleFixedJoint = GetHandle().GetComponent<FixedJoint>();
			if (handleFixedJoint == null)
			{
				handleFixedJoint = GetHandle().AddComponent<FixedJoint>();
				handleFixedJoint.connectedBody = drawerRigidbody;
			}
		}

		protected virtual Direction DetectDirection()
		{
			Direction result = Direction.autodetect;
			Bounds bounds = VRTK_SharedMethods.GetBounds(GetHandle().transform, base.transform);
			Bounds bounds2 = VRTK_SharedMethods.GetBounds(GetBody().transform, base.transform);
			float num = Mathf.Abs(bounds.center.x - (bounds2.center.x + bounds2.extents.x));
			float num2 = Mathf.Abs(bounds.center.y - (bounds2.center.y + bounds2.extents.y));
			float num3 = Mathf.Abs(bounds.center.z - (bounds2.center.z + bounds2.extents.z));
			float num4 = Mathf.Abs(bounds.center.x - (bounds2.center.x - bounds2.extents.x));
			float num5 = Mathf.Abs(bounds.center.y - (bounds2.center.y - bounds2.extents.y));
			float num6 = Mathf.Abs(bounds.center.z - (bounds2.center.z - bounds2.extents.z));
			if (VRTK_SharedMethods.IsLowest(num, new float[5] { num2, num3, num4, num5, num6 }))
			{
				result = Direction.x;
			}
			else if (VRTK_SharedMethods.IsLowest(num4, new float[5] { num, num2, num3, num5, num6 }))
			{
				result = Direction.x;
			}
			else if (VRTK_SharedMethods.IsLowest(num2, new float[5] { num, num3, num4, num5, num6 }))
			{
				result = Direction.y;
			}
			else if (VRTK_SharedMethods.IsLowest(num5, new float[5] { num, num2, num3, num4, num6 }))
			{
				result = Direction.y;
			}
			else if (VRTK_SharedMethods.IsLowest(num3, new float[5] { num, num2, num4, num5, num6 }))
			{
				result = Direction.z;
			}
			else if (VRTK_SharedMethods.IsLowest(num6, new float[5] { num, num2, num3, num5, num4 }))
			{
				result = Direction.z;
			}
			return result;
		}

		protected virtual float CalculateValue()
		{
			return Mathf.Round((base.transform.position - initialPosition).magnitude / pullDistance * 100f);
		}

		protected virtual GameObject GetBody()
		{
			return (!body) ? base.gameObject : body;
		}

		protected virtual GameObject GetHandle()
		{
			return (!handle) ? base.gameObject : handle;
		}
	}
}
