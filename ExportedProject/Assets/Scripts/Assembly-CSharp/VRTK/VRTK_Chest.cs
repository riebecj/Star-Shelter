using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VRTK
{
	public class VRTK_Chest : VRTK_Control
	{
		[Tooltip("The axis on which the chest should open. All other axis will be frozen.")]
		public Direction direction;

		[Tooltip("The game object for the lid.")]
		public GameObject lid;

		[Tooltip("The game object for the body.")]
		public GameObject body;

		[Tooltip("The game object for the handle.")]
		public GameObject handle;

		[Tooltip("The parent game object for the chest content elements.")]
		public GameObject content;

		[Tooltip("Makes the content invisible while the chest is closed.")]
		public bool hideContent = true;

		[Tooltip("The maximum opening angle of the chest.")]
		public float maxAngle = 160f;

		protected float minAngle;

		protected float stepSize = 1f;

		protected Rigidbody bodyRigidbody;

		protected Rigidbody handleRigidbody;

		protected FixedJoint handleJoint;

		protected Rigidbody lidRigidbody;

		protected HingeJoint lidJoint;

		protected bool lidJointCreated;

		protected Direction finalDirection;

		protected float subDirection = 1f;

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (base.enabled && setupSuccessful)
			{
				Bounds bounds = ((!handle) ? VRTK_SharedMethods.GetBounds(lid.transform, lid.transform) : VRTK_SharedMethods.GetBounds(handle.transform, handle.transform));
				float num = bounds.extents.y * 5f;
				Vector3 vector = bounds.center + new Vector3(0f, num, 0f);
				switch (finalDirection)
				{
				case Direction.x:
					vector += base.transform.right.normalized * (num / 2f) * subDirection;
					break;
				case Direction.y:
					vector += base.transform.up.normalized * (num / 2f) * subDirection;
					break;
				case Direction.z:
					vector += base.transform.forward.normalized * (num / 2f) * subDirection;
					break;
				}
				Gizmos.DrawLine(bounds.center + new Vector3(0f, bounds.extents.y, 0f), vector);
				Gizmos.DrawSphere(vector, num / 8f);
			}
		}

		protected override void InitRequiredComponents()
		{
			InitBody();
			InitLid();
			InitHandle();
			SetContent(content, hideContent);
		}

		protected override bool DetectSetup()
		{
			if (lid == null || body == null)
			{
				return false;
			}
			finalDirection = ((direction != 0) ? direction : DetectDirection());
			if (finalDirection == Direction.autodetect)
			{
				return false;
			}
			Bounds bounds = VRTK_SharedMethods.GetBounds(lid.transform, base.transform);
			if ((bool)handle)
			{
				Bounds bounds2 = VRTK_SharedMethods.GetBounds(handle.transform, base.transform);
				switch (finalDirection)
				{
				case Direction.x:
					subDirection = ((!(bounds2.center.x > bounds.center.x)) ? 1 : (-1));
					break;
				case Direction.y:
					subDirection = ((!(bounds2.center.y > bounds.center.y)) ? 1 : (-1));
					break;
				case Direction.z:
					subDirection = ((!(bounds2.center.z > bounds.center.z)) ? 1 : (-1));
					break;
				}
				if (handle.transform.IsChildOf(lid.transform))
				{
					return false;
				}
			}
			else
			{
				subDirection = -1f;
			}
			if (lidJointCreated)
			{
				lidJoint.useLimits = true;
				lidJoint.enableCollision = true;
				JointLimits limits = lidJoint.limits;
				switch (finalDirection)
				{
				case Direction.x:
					lidJoint.anchor = new Vector3(subDirection * bounds.extents.x, 0f, 0f);
					lidJoint.axis = new Vector3(0f, 0f, 1f);
					if (subDirection > 0f)
					{
						limits.min = 0f - maxAngle;
						limits.max = minAngle;
					}
					else
					{
						limits.min = minAngle;
						limits.max = maxAngle;
					}
					break;
				case Direction.y:
					lidJoint.anchor = new Vector3(0f, subDirection * bounds.extents.y, 0f);
					lidJoint.axis = new Vector3(0f, 1f, 0f);
					if (subDirection > 0f)
					{
						limits.min = 0f - maxAngle;
						limits.max = minAngle;
					}
					else
					{
						limits.min = minAngle;
						limits.max = maxAngle;
					}
					break;
				case Direction.z:
					lidJoint.anchor = new Vector3(0f, 0f, subDirection * bounds.extents.z);
					lidJoint.axis = new Vector3(1f, 0f, 0f);
					if (subDirection < 0f)
					{
						limits.min = 0f - maxAngle;
						limits.max = minAngle;
					}
					else
					{
						limits.min = minAngle;
						limits.max = maxAngle;
					}
					break;
				}
				lidJoint.limits = limits;
			}
			return true;
		}

		protected override ControlValueRange RegisterValueRange()
		{
			ControlValueRange result = default(ControlValueRange);
			result.controlMin = lidJoint.limits.min;
			result.controlMax = lidJoint.limits.max;
			return result;
		}

		protected override void HandleUpdate()
		{
			value = CalculateValue();
		}

		protected virtual Direction DetectDirection()
		{
			Direction result = Direction.autodetect;
			if (!handle)
			{
				return result;
			}
			Bounds bounds = VRTK_SharedMethods.GetBounds(handle.transform, base.transform);
			Bounds bounds2 = VRTK_SharedMethods.GetBounds(lid.transform, base.transform);
			float num = Mathf.Abs(bounds.center.x - (bounds2.center.x + bounds2.extents.x));
			float num2 = Mathf.Abs(bounds.center.z - (bounds2.center.z + bounds2.extents.z));
			float num3 = Mathf.Abs(bounds.center.x - (bounds2.center.x - bounds2.extents.x));
			float num4 = Mathf.Abs(bounds.center.z - (bounds2.center.z - bounds2.extents.z));
			if (VRTK_SharedMethods.IsLowest(num, new float[3] { num2, num3, num4 }))
			{
				result = Direction.x;
			}
			else if (VRTK_SharedMethods.IsLowest(num3, new float[3] { num, num2, num4 }))
			{
				result = Direction.x;
			}
			else if (VRTK_SharedMethods.IsLowest(num2, new float[3] { num, num3, num4 }))
			{
				result = Direction.z;
			}
			else if (VRTK_SharedMethods.IsLowest(num4, new float[3] { num, num2, num3 }))
			{
				result = Direction.z;
			}
			return result;
		}

		protected virtual void InitBody()
		{
			bodyRigidbody = body.GetComponent<Rigidbody>();
			if (bodyRigidbody == null)
			{
				bodyRigidbody = body.AddComponent<Rigidbody>();
				bodyRigidbody.isKinematic = true;
			}
		}

		protected virtual void InitLid()
		{
			lidRigidbody = lid.GetComponent<Rigidbody>();
			if (lidRigidbody == null)
			{
				lidRigidbody = lid.AddComponent<Rigidbody>();
			}
			lidJoint = lid.GetComponent<HingeJoint>();
			if (lidJoint == null)
			{
				lidJoint = lid.AddComponent<HingeJoint>();
				lidJointCreated = true;
			}
			lidJoint.connectedBody = bodyRigidbody;
			if (!handle)
			{
				CreateInteractableObject(lid);
			}
		}

		protected virtual void InitHandle()
		{
			if ((bool)handle)
			{
				handleRigidbody = handle.GetComponent<Rigidbody>();
				if (handleRigidbody == null)
				{
					handleRigidbody = handle.AddComponent<Rigidbody>();
				}
				handleRigidbody.isKinematic = false;
				handleRigidbody.useGravity = false;
				handleJoint = handle.GetComponent<FixedJoint>();
				if (handleJoint == null)
				{
					handleJoint = handle.AddComponent<FixedJoint>();
					handleJoint.connectedBody = lidRigidbody;
				}
				CreateInteractableObject(handle);
			}
		}

		protected virtual void CreateInteractableObject(GameObject targetGameObject)
		{
			VRTK_InteractableObject vRTK_InteractableObject = targetGameObject.GetComponent<VRTK_InteractableObject>();
			if (vRTK_InteractableObject == null)
			{
				vRTK_InteractableObject = targetGameObject.AddComponent<VRTK_InteractableObject>();
			}
			vRTK_InteractableObject.isGrabbable = true;
			vRTK_InteractableObject.grabAttachMechanicScript = base.gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
			vRTK_InteractableObject.secondaryGrabActionScript = base.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
			vRTK_InteractableObject.grabAttachMechanicScript.precisionGrab = true;
			vRTK_InteractableObject.stayGrabbedOnTeleport = false;
		}

		protected virtual float CalculateValue()
		{
			return Mathf.Round((minAngle + Mathf.Clamp01(Mathf.Abs(lidJoint.angle / (lidJoint.limits.max - lidJoint.limits.min))) * (maxAngle - minAngle)) / stepSize) * stepSize;
		}
	}
}
