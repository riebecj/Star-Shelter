using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VRTK
{
	public class VRTK_Door : VRTK_Control
	{
		[Tooltip("The axis on which the door should open.")]
		public Direction direction;

		[Tooltip("The game object for the door. Can also be an empty parent or left empty if the script is put onto the actual door mesh. If no colliders exist yet a collider will tried to be automatically attached to all children that expose renderers.")]
		public GameObject door;

		[Tooltip("The game object for the handles. Can also be an empty parent or left empty. If empty the door can only be moved using the rigidbody mode of the controller. If no collider exists yet a compound collider made up of all children will try to be calculated but this will fail if the door is rotated. In that case a manual collider will need to be assigned.")]
		public GameObject handles;

		[Tooltip("The game object for the frame to which the door is attached. Should only be set if the frame will move as well to ensure that the door moves along with the frame.")]
		public GameObject frame;

		[Tooltip("The parent game object for the door content elements.")]
		public GameObject content;

		[Tooltip("Makes the content invisible while the door is closed.")]
		public bool hideContent = true;

		[Tooltip("The maximum opening angle of the door.")]
		public float maxAngle = 120f;

		[Tooltip("Can the door be pulled to open.")]
		public bool openInward;

		[Tooltip("Can the door be pushed to open.")]
		public bool openOutward = true;

		[Tooltip("The range at which the door must be to being closed before it snaps shut. Only works if either inward or outward is selected, not both.")]
		[Range(0f, 1f)]
		public float minSnapClose = 1f;

		[Tooltip("The amount of friction the door will have whilst swinging when it is not grabbed.")]
		public float releasedFriction = 10f;

		[Tooltip("The amount of friction the door will have whilst swinging when it is grabbed.")]
		public float grabbedFriction = 100f;

		[Tooltip("If this is checked then only the door handle is grabbale to operate the door.")]
		public bool handleInteractableOnly;

		protected float stepSize = 1f;

		protected Rigidbody doorRigidbody;

		protected HingeJoint doorHinge;

		protected ConstantForce doorSnapForce;

		protected Rigidbody frameRigidbody;

		protected Direction finalDirection;

		protected float subDirection = 1f;

		protected Vector3 secondaryDirection;

		protected bool doorHingeCreated;

		protected bool doorSnapForceCreated;

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (!base.enabled || !setupSuccessful)
			{
				return;
			}
			Bounds bounds = default(Bounds);
			Bounds bounds2 = VRTK_SharedMethods.GetBounds(GetDoor().transform, GetDoor().transform);
			float num = 0.5f;
			if ((bool)handles)
			{
				bounds = VRTK_SharedMethods.GetBounds(handles.transform, handles.transform);
			}
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			Vector3 thirdDirection = GetThirdDirection(Direction2Axis(finalDirection), secondaryDirection);
			bool flag = false;
			switch (finalDirection)
			{
			case Direction.x:
				if (thirdDirection == Vector3.up)
				{
					vector = base.transform.up.normalized;
					vector2 = base.transform.forward.normalized;
					num *= bounds2.extents.z;
				}
				else
				{
					vector = base.transform.forward.normalized;
					vector2 = base.transform.up.normalized;
					num *= bounds2.extents.y;
					flag = true;
				}
				break;
			case Direction.y:
				if (thirdDirection == Vector3.right)
				{
					vector = base.transform.right.normalized;
					vector2 = base.transform.forward.normalized;
					num *= bounds2.extents.z;
					flag = true;
				}
				else
				{
					vector = base.transform.forward.normalized;
					vector2 = base.transform.right.normalized;
					num *= bounds2.extents.x;
				}
				break;
			case Direction.z:
				if (thirdDirection == Vector3.up)
				{
					vector = base.transform.up.normalized;
					vector2 = base.transform.right.normalized;
					num *= bounds2.extents.x;
					flag = true;
				}
				else
				{
					vector = base.transform.right.normalized;
					vector2 = base.transform.up.normalized;
					num *= bounds2.extents.y;
				}
				break;
			}
			if ((!flag && openInward) || (flag && openOutward))
			{
				Vector3 vector3 = ((!handles) ? bounds2.center : bounds.center);
				Vector3 vector4 = vector3 + vector2 * num * subDirection - vector * (num / 2f) * subDirection;
				Gizmos.DrawLine(vector3, vector4);
				Gizmos.DrawSphere(vector4, num / 8f);
			}
			if ((!flag && openOutward) || (flag && openInward))
			{
				Vector3 vector5 = ((!handles) ? bounds2.center : bounds.center);
				Vector3 vector6 = vector5 + vector2 * num * subDirection + vector * (num / 2f) * subDirection;
				Gizmos.DrawLine(vector5, vector6);
				Gizmos.DrawSphere(vector6, num / 8f);
			}
		}

		protected override void InitRequiredComponents()
		{
			InitFrame();
			InitDoor();
			InitHandle();
			SetContent(content, hideContent);
		}

		protected override bool DetectSetup()
		{
			doorHinge = GetDoor().GetComponent<HingeJoint>();
			if ((bool)doorHinge && !doorHingeCreated)
			{
				direction = Direction.autodetect;
			}
			finalDirection = ((direction != 0) ? direction : DetectDirection());
			if (finalDirection == Direction.autodetect)
			{
				return false;
			}
			if ((bool)doorHinge && !doorHingeCreated)
			{
				direction = finalDirection;
			}
			Bounds bounds = VRTK_SharedMethods.GetBounds(GetDoor().transform, base.transform);
			if (doorHinge == null || doorHingeCreated)
			{
				if ((bool)handles)
				{
					Bounds bounds2 = VRTK_SharedMethods.GetBounds(handles.transform, base.transform);
					switch (finalDirection)
					{
					case Direction.x:
						if (bounds2.center.z + bounds2.extents.z > bounds.center.z + bounds.extents.z || bounds2.center.z - bounds2.extents.z < bounds.center.z - bounds.extents.z)
						{
							subDirection = ((!(bounds2.center.y > bounds.center.y)) ? 1 : (-1));
							secondaryDirection = Vector3.up;
						}
						else
						{
							subDirection = ((!(bounds2.center.z > bounds.center.z)) ? 1 : (-1));
							secondaryDirection = Vector3.forward;
						}
						break;
					case Direction.y:
						if (bounds2.center.z + bounds2.extents.z > bounds.center.z + bounds.extents.z || bounds2.center.z - bounds2.extents.z < bounds.center.z - bounds.extents.z)
						{
							subDirection = ((!(bounds2.center.x > bounds.center.x)) ? 1 : (-1));
							secondaryDirection = Vector3.right;
						}
						else
						{
							subDirection = ((!(bounds2.center.z > bounds.center.z)) ? 1 : (-1));
							secondaryDirection = Vector3.forward;
						}
						break;
					case Direction.z:
						if (bounds2.center.x + bounds2.extents.x > bounds.center.x + bounds.extents.x || bounds2.center.x - bounds2.extents.x < bounds.center.x - bounds.extents.x)
						{
							subDirection = ((!(bounds2.center.y > bounds.center.y)) ? 1 : (-1));
							secondaryDirection = Vector3.up;
						}
						else
						{
							subDirection = ((!(bounds2.center.x > bounds.center.x)) ? 1 : (-1));
							secondaryDirection = Vector3.right;
						}
						break;
					}
				}
				else
				{
					switch (finalDirection)
					{
					case Direction.x:
						secondaryDirection = ((!(bounds.extents.y > bounds.extents.z)) ? Vector3.forward : Vector3.up);
						break;
					case Direction.y:
						secondaryDirection = ((!(bounds.extents.x > bounds.extents.z)) ? Vector3.forward : Vector3.right);
						break;
					case Direction.z:
						secondaryDirection = ((!(bounds.extents.y > bounds.extents.x)) ? Vector3.right : Vector3.up);
						break;
					}
					subDirection = 1f;
				}
			}
			else
			{
				Vector3 vector = bounds.center - doorHinge.connectedAnchor;
				if (vector.x != 0f)
				{
					secondaryDirection = Vector3.right;
					subDirection = ((vector.x <= 0f) ? 1 : (-1));
				}
				else if (vector.y != 0f)
				{
					secondaryDirection = Vector3.up;
					subDirection = ((vector.y <= 0f) ? 1 : (-1));
				}
				else if (vector.z != 0f)
				{
					secondaryDirection = Vector3.forward;
					subDirection = ((vector.z <= 0f) ? 1 : (-1));
				}
			}
			if (doorHingeCreated)
			{
				float num = 0f;
				num = ((secondaryDirection == Vector3.right) ? (bounds.extents.x / GetDoor().transform.lossyScale.x) : ((!(secondaryDirection == Vector3.up)) ? (bounds.extents.z / GetDoor().transform.lossyScale.z) : (bounds.extents.y / GetDoor().transform.lossyScale.y)));
				doorHinge.anchor = secondaryDirection * subDirection * num;
				doorHinge.axis = Direction2Axis(finalDirection);
			}
			if ((bool)doorHinge)
			{
				doorHinge.useLimits = true;
				doorHinge.enableCollision = true;
				JointLimits limits = doorHinge.limits;
				limits.min = ((!openInward) ? 0f : (0f - maxAngle));
				limits.max = ((!openOutward) ? 0f : maxAngle);
				limits.bounciness = 0f;
				doorHinge.limits = limits;
			}
			if (doorSnapForceCreated)
			{
				float num2 = -5f * GetDirectionFromJoint();
				doorSnapForce.relativeForce = GetThirdDirection(doorHinge.axis, secondaryDirection) * (subDirection * num2);
			}
			return true;
		}

		protected override ControlValueRange RegisterValueRange()
		{
			ControlValueRange result = default(ControlValueRange);
			result.controlMin = doorHinge.limits.min;
			result.controlMax = doorHinge.limits.max;
			return result;
		}

		protected override void HandleUpdate()
		{
			value = CalculateValue();
			doorSnapForce.enabled = (openOutward ^ openInward) && Mathf.Abs(value) < minSnapClose * 100f;
		}

		protected virtual float GetDirectionFromJoint()
		{
			return (!(doorHinge.limits.min < 0f)) ? 1f : (-1f);
		}

		protected virtual Vector3 Direction2Axis(Direction givenDirection)
		{
			Vector3 result = Vector3.zero;
			switch (givenDirection)
			{
			case Direction.x:
				result = new Vector3(1f, 0f, 0f);
				break;
			case Direction.y:
				result = new Vector3(0f, 1f, 0f);
				break;
			case Direction.z:
				result = new Vector3(0f, 0f, 1f);
				break;
			}
			return result;
		}

		protected virtual Direction DetectDirection()
		{
			Direction result = Direction.autodetect;
			if ((bool)doorHinge && !doorHingeCreated)
			{
				if (doorHinge.axis == Vector3.right)
				{
					result = Direction.x;
				}
				else if (doorHinge.axis == Vector3.up)
				{
					result = Direction.y;
				}
				else if (doorHinge.axis == Vector3.forward)
				{
					result = Direction.z;
				}
			}
			else if ((bool)handles)
			{
				Bounds bounds = VRTK_SharedMethods.GetBounds(handles.transform, base.transform);
				Bounds bounds2 = VRTK_SharedMethods.GetBounds(GetDoor().transform, base.transform, handles.transform);
				result = ((bounds.center.y + bounds.extents.y > bounds2.center.y + bounds2.extents.y || bounds.center.y - bounds.extents.y < bounds2.center.y - bounds2.extents.y) ? Direction.x : Direction.y);
			}
			return result;
		}

		protected virtual void InitFrame()
		{
			if (!(frame == null))
			{
				frameRigidbody = frame.GetComponent<Rigidbody>();
				if (frameRigidbody == null)
				{
					frameRigidbody = frame.AddComponent<Rigidbody>();
					frameRigidbody.isKinematic = true;
					frameRigidbody.angularDrag = releasedFriction;
				}
			}
		}

		protected virtual void InitDoor()
		{
			GameObject gameObject = GetDoor();
			VRTK_SharedMethods.CreateColliders(gameObject);
			doorRigidbody = gameObject.GetComponent<Rigidbody>();
			if (doorRigidbody == null)
			{
				doorRigidbody = gameObject.AddComponent<Rigidbody>();
				doorRigidbody.angularDrag = releasedFriction;
			}
			doorRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			doorRigidbody.isKinematic = false;
			doorHinge = gameObject.GetComponent<HingeJoint>();
			if (doorHinge == null)
			{
				doorHinge = gameObject.AddComponent<HingeJoint>();
				doorHingeCreated = true;
			}
			doorHinge.connectedBody = frameRigidbody;
			doorSnapForce = gameObject.GetComponent<ConstantForce>();
			if (doorSnapForce == null)
			{
				doorSnapForce = gameObject.AddComponent<ConstantForce>();
				doorSnapForce.enabled = false;
				doorSnapForceCreated = true;
			}
			if (!handleInteractableOnly)
			{
				CreateInteractableObject(gameObject);
			}
		}

		protected virtual void InitHandle()
		{
			if (!(handles == null))
			{
				if (handles.GetComponentInChildren<Collider>() == null)
				{
					VRTK_SharedMethods.CreateColliders(handles);
				}
				Rigidbody rigidbody = handles.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					rigidbody = handles.AddComponent<Rigidbody>();
				}
				rigidbody.isKinematic = false;
				rigidbody.useGravity = false;
				FixedJoint component = handles.GetComponent<FixedJoint>();
				if (component == null)
				{
					component = handles.AddComponent<FixedJoint>();
					component.connectedBody = doorRigidbody;
				}
				if (handleInteractableOnly)
				{
					CreateInteractableObject(handles);
				}
			}
		}

		protected virtual void CreateInteractableObject(GameObject target)
		{
			VRTK_InteractableObject vRTK_InteractableObject = target.GetComponent<VRTK_InteractableObject>();
			if (vRTK_InteractableObject == null)
			{
				vRTK_InteractableObject = target.AddComponent<VRTK_InteractableObject>();
			}
			vRTK_InteractableObject.isGrabbable = true;
			vRTK_InteractableObject.grabAttachMechanicScript = target.AddComponent<VRTK_RotatorTrackGrabAttach>();
			vRTK_InteractableObject.grabAttachMechanicScript.precisionGrab = true;
			vRTK_InteractableObject.secondaryGrabActionScript = target.AddComponent<VRTK_SwapControllerGrabAction>();
			vRTK_InteractableObject.stayGrabbedOnTeleport = false;
			vRTK_InteractableObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
			vRTK_InteractableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
		}

		protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
		{
			doorRigidbody.angularDrag = grabbedFriction;
		}

		protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
		{
			doorRigidbody.angularDrag = releasedFriction;
		}

		protected virtual float CalculateValue()
		{
			return Mathf.Round(doorHinge.angle / stepSize) * stepSize;
		}

		protected virtual GameObject GetDoor()
		{
			return (!door) ? base.gameObject : door;
		}
	}
}
