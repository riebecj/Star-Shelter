using UnityEngine;

namespace VRTK.SecondaryControllerGrabActions
{
	public class VRTK_AxisScaleGrabAction : VRTK_BaseGrabAction
	{
		[Tooltip("The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.")]
		public float ungrabDistance = 1f;

		[Tooltip("If checked the current X Axis of the object won't be scaled")]
		public bool lockXAxis;

		[Tooltip("If checked the current Y Axis of the object won't be scaled")]
		public bool lockYAxis;

		[Tooltip("If checked the current Z Axis of the object won't be scaled")]
		public bool lockZAxis;

		[Tooltip("If checked all the axes will be scaled together (unless locked)")]
		public bool uniformScaling;

		protected Vector3 initialScale;

		protected float initalLength;

		protected float initialScaleFactor;

		public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
		{
			base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
			initialScale = currentGrabbdObject.transform.localScale;
			initalLength = (grabbedObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
			initialScaleFactor = currentGrabbdObject.transform.localScale.x / initalLength;
		}

		public override void ProcessUpdate()
		{
			base.ProcessUpdate();
			CheckForceStopDistance(ungrabDistance);
		}

		public override void ProcessFixedUpdate()
		{
			base.ProcessFixedUpdate();
			if (initialised)
			{
				if (uniformScaling)
				{
					UniformScale();
				}
				else
				{
					NonUniformScale();
				}
			}
		}

		protected virtual void ApplyScale(Vector3 newScale)
		{
			Vector3 localScale = grabbedObject.transform.localScale;
			float num = ((!lockXAxis) ? newScale.x : localScale.x);
			float num2 = ((!lockYAxis) ? newScale.y : localScale.y);
			float num3 = ((!lockZAxis) ? newScale.z : localScale.z);
			if (num > 0f && num2 > 0f && num3 > 0f)
			{
				grabbedObject.transform.localScale = new Vector3(num, num2, num3);
			}
		}

		protected virtual void NonUniformScale()
		{
			Vector3 vector = grabbedObject.transform.rotation * grabbedObject.transform.position;
			Vector3 vector2 = grabbedObject.transform.rotation * secondaryInitialGrabPoint.position;
			Vector3 vector3 = grabbedObject.transform.rotation * secondaryGrabbingObject.transform.position;
			float x = CalculateAxisScale(vector.x, vector2.x, vector3.x);
			float y = CalculateAxisScale(vector.y, vector2.y, vector3.y);
			float z = CalculateAxisScale(vector.z, vector2.z, vector3.z);
			Vector3 newScale = new Vector3(x, y, z) + initialScale;
			ApplyScale(newScale);
		}

		protected virtual void UniformScale()
		{
			float magnitude = (grabbedObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
			float num = initialScaleFactor * magnitude;
			Vector3 newScale = new Vector3(num, num, num);
			ApplyScale(newScale);
		}

		protected virtual float CalculateAxisScale(float centerPosition, float initialPosition, float currentPosition)
		{
			float num = currentPosition - initialPosition;
			return (!(centerPosition < initialPosition)) ? (0f - num) : num;
		}
	}
}
