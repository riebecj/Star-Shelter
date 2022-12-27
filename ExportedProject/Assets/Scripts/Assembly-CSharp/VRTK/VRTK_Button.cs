using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRTK
{
	public class VRTK_Button : VRTK_Control
	{
		[Serializable]
		[Obsolete("`VRTK_Control.ButtonEvents` has been replaced with delegate events. `VRTK_Button_UnityEvents` is now required to access Unity events. This method will be removed in a future version of VRTK.")]
		public class ButtonEvents
		{
			public UnityEvent OnPush;
		}

		public enum ButtonDirection
		{
			autodetect = 0,
			x = 1,
			y = 2,
			z = 3,
			negX = 4,
			negY = 5,
			negZ = 6
		}

		[Tooltip("An optional game object to which the button will be connected. If the game object moves the button will follow along.")]
		public GameObject connectedTo;

		[Tooltip("The axis on which the button should move. All other axis will be frozen.")]
		public ButtonDirection direction;

		[Tooltip("The local distance the button needs to be pushed until a push event is triggered.")]
		public float activationDistance = 1f;

		[Tooltip("The amount of force needed to push the button down as well as the speed with which it will go back into its original position.")]
		public float buttonStrength = 5f;

		[Tooltip("The events specific to the button control. This parameter is deprecated and will be removed in a future version of VRTK.")]
		[Obsolete("`VRTK_Control.events` has been replaced with delegate events. `VRTK_Button_UnityEvents` is now required to access Unity events. This method will be removed in a future version of VRTK.")]
		public ButtonEvents events;

		protected const float MAX_AUTODETECT_ACTIVATION_LENGTH = 4f;

		protected ButtonDirection finalDirection;

		protected Vector3 restingPosition;

		protected Vector3 activationDir;

		protected Rigidbody buttonRigidbody;

		protected ConfigurableJoint buttonJoint;

		protected ConstantForce buttonForce;

		protected int forceCount;

		public event Button3DEventHandler Pushed;

		public virtual void OnPushed(Control3DEventArgs e)
		{
			if (this.Pushed != null)
			{
				this.Pushed(this, e);
			}
		}

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (base.enabled && setupSuccessful)
			{
				Gizmos.DrawLine(bounds.center, bounds.center + activationDir);
			}
		}

		protected override void InitRequiredComponents()
		{
			restingPosition = base.transform.position;
			if (!GetComponent<Collider>())
			{
				base.gameObject.AddComponent<BoxCollider>();
			}
			buttonRigidbody = GetComponent<Rigidbody>();
			if (buttonRigidbody == null)
			{
				buttonRigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			buttonRigidbody.isKinematic = false;
			buttonRigidbody.useGravity = false;
			buttonForce = GetComponent<ConstantForce>();
			if (buttonForce == null)
			{
				buttonForce = base.gameObject.AddComponent<ConstantForce>();
			}
			if ((bool)connectedTo)
			{
				Rigidbody rigidbody = connectedTo.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					rigidbody = connectedTo.AddComponent<Rigidbody>();
				}
				rigidbody.useGravity = false;
			}
		}

		protected override bool DetectSetup()
		{
			finalDirection = ((direction != 0) ? direction : DetectDirection());
			if (finalDirection == ButtonDirection.autodetect)
			{
				activationDir = Vector3.zero;
				return false;
			}
			if (direction != 0)
			{
				activationDir = CalculateActivationDir();
			}
			if ((bool)buttonForce)
			{
				buttonForce.force = GetForceVector();
			}
			if (Application.isPlaying)
			{
				buttonJoint = GetComponent<ConfigurableJoint>();
				bool flag = false;
				Rigidbody connectedBody = null;
				Vector3 anchor = Vector3.zero;
				Vector3 axis = Vector3.zero;
				if ((bool)buttonJoint)
				{
					connectedBody = buttonJoint.connectedBody;
					anchor = buttonJoint.anchor;
					axis = buttonJoint.axis;
					UnityEngine.Object.DestroyImmediate(buttonJoint);
					flag = true;
				}
				base.transform.position = base.transform.position + activationDir.normalized * activationDistance * 0.5f;
				buttonJoint = base.gameObject.AddComponent<ConfigurableJoint>();
				if (flag)
				{
					buttonJoint.connectedBody = connectedBody;
					buttonJoint.anchor = anchor;
					buttonJoint.axis = axis;
				}
				if ((bool)connectedTo)
				{
					buttonJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
				}
				SoftJointLimit linearLimit = default(SoftJointLimit);
				linearLimit.limit = activationDistance * 0.501f;
				buttonJoint.linearLimit = linearLimit;
				buttonJoint.angularXMotion = ConfigurableJointMotion.Locked;
				buttonJoint.angularYMotion = ConfigurableJointMotion.Locked;
				buttonJoint.angularZMotion = ConfigurableJointMotion.Locked;
				buttonJoint.xMotion = ConfigurableJointMotion.Locked;
				buttonJoint.yMotion = ConfigurableJointMotion.Locked;
				buttonJoint.zMotion = ConfigurableJointMotion.Locked;
				switch (finalDirection)
				{
				case ButtonDirection.x:
				case ButtonDirection.negX:
					if (Mathf.RoundToInt(Mathf.Abs(base.transform.right.x)) == 1)
					{
						buttonJoint.xMotion = ConfigurableJointMotion.Limited;
					}
					else if (Mathf.RoundToInt(Mathf.Abs(base.transform.up.x)) == 1)
					{
						buttonJoint.yMotion = ConfigurableJointMotion.Limited;
					}
					else if (Mathf.RoundToInt(Mathf.Abs(base.transform.forward.x)) == 1)
					{
						buttonJoint.zMotion = ConfigurableJointMotion.Limited;
					}
					break;
				case ButtonDirection.y:
				case ButtonDirection.negY:
					if (Mathf.RoundToInt(Mathf.Abs(base.transform.right.y)) == 1)
					{
						buttonJoint.xMotion = ConfigurableJointMotion.Limited;
					}
					else if (Mathf.RoundToInt(Mathf.Abs(base.transform.up.y)) == 1)
					{
						buttonJoint.yMotion = ConfigurableJointMotion.Limited;
					}
					else if (Mathf.RoundToInt(Mathf.Abs(base.transform.forward.y)) == 1)
					{
						buttonJoint.zMotion = ConfigurableJointMotion.Limited;
					}
					break;
				case ButtonDirection.z:
				case ButtonDirection.negZ:
					if (Mathf.RoundToInt(Mathf.Abs(base.transform.right.z)) == 1)
					{
						buttonJoint.xMotion = ConfigurableJointMotion.Limited;
					}
					else if (Mathf.RoundToInt(Mathf.Abs(base.transform.up.z)) == 1)
					{
						buttonJoint.yMotion = ConfigurableJointMotion.Limited;
					}
					else if (Mathf.RoundToInt(Mathf.Abs(base.transform.forward.z)) == 1)
					{
						buttonJoint.zMotion = ConfigurableJointMotion.Limited;
					}
					break;
				}
			}
			return true;
		}

		protected override ControlValueRange RegisterValueRange()
		{
			ControlValueRange result = default(ControlValueRange);
			result.controlMin = 0f;
			result.controlMax = 1f;
			return result;
		}

		protected override void HandleUpdate()
		{
			float num = value;
			if (ReachedActivationDistance())
			{
				if (num == 0f)
				{
					value = 1f;
					events.OnPush.Invoke();
					OnPushed(SetControlEvent());
				}
			}
			else
			{
				value = 0f;
			}
		}

		protected virtual void FixedUpdate()
		{
			if (forceCount == 0 && (bool)buttonJoint.connectedBody)
			{
				restingPosition = base.transform.position;
			}
		}

		protected virtual void OnCollisionExit(Collision collision)
		{
			forceCount--;
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			forceCount++;
		}

		protected virtual ButtonDirection DetectDirection()
		{
			ButtonDirection buttonDirection = ButtonDirection.autodetect;
			Bounds bounds = VRTK_SharedMethods.GetBounds(base.transform);
			RaycastHit hitInfo;
			Physics.Raycast(bounds.center, Vector3.forward, out hitInfo, bounds.extents.z * 4f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo2;
			Physics.Raycast(bounds.center, Vector3.back, out hitInfo2, bounds.extents.z * 4f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo3;
			Physics.Raycast(bounds.center, Vector3.left, out hitInfo3, bounds.extents.x * 4f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo4;
			Physics.Raycast(bounds.center, Vector3.right, out hitInfo4, bounds.extents.x * 4f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo5;
			Physics.Raycast(bounds.center, Vector3.up, out hitInfo5, bounds.extents.y * 4f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo6;
			Physics.Raycast(bounds.center, Vector3.down, out hitInfo6, bounds.extents.y * 4f, -5, QueryTriggerInteraction.UseGlobal);
			float num = ((!(hitInfo4.collider != null)) ? float.MaxValue : hitInfo4.distance);
			float num2 = ((!(hitInfo6.collider != null)) ? float.MaxValue : hitInfo6.distance);
			float num3 = ((!(hitInfo2.collider != null)) ? float.MaxValue : hitInfo2.distance);
			float num4 = ((!(hitInfo3.collider != null)) ? float.MaxValue : hitInfo3.distance);
			float num5 = ((!(hitInfo5.collider != null)) ? float.MaxValue : hitInfo5.distance);
			float num6 = ((!(hitInfo.collider != null)) ? float.MaxValue : hitInfo.distance);
			float num7 = 0f;
			Vector3 vector = Vector3.zero;
			if (VRTK_SharedMethods.IsLowest(num, new float[5] { num2, num3, num4, num5, num6 }))
			{
				buttonDirection = ButtonDirection.negX;
				vector = hitInfo4.point;
				num7 = bounds.extents.x;
			}
			else if (VRTK_SharedMethods.IsLowest(num2, new float[5] { num, num3, num4, num5, num6 }))
			{
				buttonDirection = ButtonDirection.y;
				vector = hitInfo6.point;
				num7 = bounds.extents.y;
			}
			else if (VRTK_SharedMethods.IsLowest(num3, new float[5] { num, num2, num4, num5, num6 }))
			{
				buttonDirection = ButtonDirection.z;
				vector = hitInfo2.point;
				num7 = bounds.extents.z;
			}
			else if (VRTK_SharedMethods.IsLowest(num4, new float[5] { num, num2, num3, num5, num6 }))
			{
				buttonDirection = ButtonDirection.x;
				vector = hitInfo3.point;
				num7 = bounds.extents.x;
			}
			else if (VRTK_SharedMethods.IsLowest(num5, new float[5] { num, num2, num3, num4, num6 }))
			{
				buttonDirection = ButtonDirection.negY;
				vector = hitInfo5.point;
				num7 = bounds.extents.y;
			}
			else if (VRTK_SharedMethods.IsLowest(num6, new float[5] { num, num2, num3, num4, num5 }))
			{
				buttonDirection = ButtonDirection.negZ;
				vector = hitInfo.point;
				num7 = bounds.extents.z;
			}
			activationDistance = (Vector3.Distance(vector, bounds.center) - num7) * 0.95f;
			if (buttonDirection == ButtonDirection.autodetect || activationDistance < 0.001f)
			{
				buttonDirection = ButtonDirection.autodetect;
				activationDistance = 0f;
			}
			else
			{
				activationDir = vector - bounds.center;
			}
			return buttonDirection;
		}

		protected virtual Vector3 CalculateActivationDir()
		{
			Bounds bounds = VRTK_SharedMethods.GetBounds(base.transform, base.transform);
			Vector3 vector = Vector3.zero;
			float num = 0f;
			switch (direction)
			{
			case ButtonDirection.x:
			case ButtonDirection.negX:
				if (Mathf.RoundToInt(Mathf.Abs(base.transform.right.x)) == 1)
				{
					vector = base.transform.right;
					num = bounds.extents.x;
				}
				else if (Mathf.RoundToInt(Mathf.Abs(base.transform.up.x)) == 1)
				{
					vector = base.transform.up;
					num = bounds.extents.y;
				}
				else if (Mathf.RoundToInt(Mathf.Abs(base.transform.forward.x)) == 1)
				{
					vector = base.transform.forward;
					num = bounds.extents.z;
				}
				vector *= (float)((direction != ButtonDirection.x) ? 1 : (-1));
				break;
			case ButtonDirection.y:
			case ButtonDirection.negY:
				if (Mathf.RoundToInt(Mathf.Abs(base.transform.right.y)) == 1)
				{
					vector = base.transform.right;
					num = bounds.extents.x;
				}
				else if (Mathf.RoundToInt(Mathf.Abs(base.transform.up.y)) == 1)
				{
					vector = base.transform.up;
					num = bounds.extents.y;
				}
				else if (Mathf.RoundToInt(Mathf.Abs(base.transform.forward.y)) == 1)
				{
					vector = base.transform.forward;
					num = bounds.extents.z;
				}
				vector *= (float)((direction != ButtonDirection.y) ? 1 : (-1));
				break;
			case ButtonDirection.z:
			case ButtonDirection.negZ:
				if (Mathf.RoundToInt(Mathf.Abs(base.transform.right.z)) == 1)
				{
					vector = base.transform.right;
					num = bounds.extents.x;
				}
				else if (Mathf.RoundToInt(Mathf.Abs(base.transform.up.z)) == 1)
				{
					vector = base.transform.up;
					num = bounds.extents.y;
				}
				else if (Mathf.RoundToInt(Mathf.Abs(base.transform.forward.z)) == 1)
				{
					vector = base.transform.forward;
					num = bounds.extents.z;
				}
				vector *= (float)((direction != ButtonDirection.z) ? 1 : (-1));
				break;
			}
			return vector * (num + activationDistance);
		}

		protected virtual bool ReachedActivationDistance()
		{
			return Vector3.Distance(base.transform.position, restingPosition) >= activationDistance;
		}

		protected virtual Vector3 GetForceVector()
		{
			return -activationDir.normalized * buttonStrength;
		}
	}
}
