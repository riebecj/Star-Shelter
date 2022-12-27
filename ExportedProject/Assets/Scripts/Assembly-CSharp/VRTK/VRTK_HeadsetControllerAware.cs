using UnityEngine;

namespace VRTK
{
	public class VRTK_HeadsetControllerAware : MonoBehaviour
	{
		[Tooltip("If this is checked then the left controller will be checked if items obscure it's path from the headset.")]
		public bool trackLeftController = true;

		[Tooltip("If this is checked then the right controller will be checked if items obscure it's path from the headset.")]
		public bool trackRightController = true;

		[Tooltip("The radius of the accepted distance from the controller origin point to determine if the controller is being looked at.")]
		public float controllerGlanceRadius = 0.15f;

		[Tooltip("A custom transform to provide the world space position of the right controller.")]
		public Transform customRightControllerOrigin;

		[Tooltip("A custom transform to provide the world space position of the left controller.")]
		public Transform customLeftControllerOrigin;

		[Tooltip("A custom raycaster to use when raycasting to find controllers.")]
		public VRTK_CustomRaycast customRaycast;

		protected GameObject leftController;

		protected GameObject rightController;

		protected Transform headset;

		protected bool leftControllerObscured;

		protected bool rightControllerObscured;

		protected bool leftControllerLastState;

		protected bool rightControllerLastState;

		protected bool leftControllerGlance;

		protected bool rightControllerGlance;

		protected bool leftControllerGlanceLastState;

		protected bool rightControllerGlanceLastState;

		public event HeadsetControllerAwareEventHandler ControllerObscured;

		public event HeadsetControllerAwareEventHandler ControllerUnobscured;

		public event HeadsetControllerAwareEventHandler ControllerGlanceEnter;

		public event HeadsetControllerAwareEventHandler ControllerGlanceExit;

		public virtual void OnControllerObscured(HeadsetControllerAwareEventArgs e)
		{
			if (this.ControllerObscured != null)
			{
				this.ControllerObscured(this, e);
			}
		}

		public virtual void OnControllerUnobscured(HeadsetControllerAwareEventArgs e)
		{
			if (this.ControllerUnobscured != null)
			{
				this.ControllerUnobscured(this, e);
			}
		}

		public virtual void OnControllerGlanceEnter(HeadsetControllerAwareEventArgs e)
		{
			if (this.ControllerGlanceEnter != null)
			{
				this.ControllerGlanceEnter(this, e);
			}
		}

		public virtual void OnControllerGlanceExit(HeadsetControllerAwareEventArgs e)
		{
			if (this.ControllerGlanceExit != null)
			{
				this.ControllerGlanceExit(this, e);
			}
		}

		public virtual bool LeftControllerObscured()
		{
			return leftControllerObscured;
		}

		public virtual bool RightControllerObscured()
		{
			return rightControllerObscured;
		}

		public virtual bool LeftControllerGlanced()
		{
			return leftControllerGlance;
		}

		public virtual bool RightControllerGlanced()
		{
			return rightControllerGlance;
		}

		protected virtual void OnEnable()
		{
			VRTK_ObjectCache.registeredHeadsetControllerAwareness = this;
			headset = VRTK_DeviceFinder.HeadsetTransform();
			leftController = VRTK_DeviceFinder.GetControllerLeftHand();
			rightController = VRTK_DeviceFinder.GetControllerRightHand();
		}

		protected virtual void OnDisable()
		{
			VRTK_ObjectCache.registeredHeadsetControllerAwareness = null;
			leftController = null;
			rightController = null;
		}

		protected virtual void Update()
		{
			if (trackLeftController)
			{
				RayCastToController(leftController, customLeftControllerOrigin, ref leftControllerObscured, ref leftControllerLastState);
			}
			if (trackRightController)
			{
				RayCastToController(rightController, customRightControllerOrigin, ref rightControllerObscured, ref rightControllerLastState);
			}
			CheckHeadsetView(leftController, customLeftControllerOrigin, ref leftControllerGlance, ref leftControllerGlanceLastState);
			CheckHeadsetView(rightController, customRightControllerOrigin, ref rightControllerGlance, ref rightControllerGlanceLastState);
		}

		protected virtual HeadsetControllerAwareEventArgs SetHeadsetControllerAwareEvent(RaycastHit raycastHit, uint controllerIndex)
		{
			HeadsetControllerAwareEventArgs result = default(HeadsetControllerAwareEventArgs);
			result.raycastHit = raycastHit;
			result.controllerIndex = controllerIndex;
			return result;
		}

		protected virtual void RayCastToController(GameObject controller, Transform customDestination, ref bool obscured, ref bool lastState)
		{
			obscured = false;
			if ((bool)controller && controller.gameObject.activeInHierarchy)
			{
				Vector3 endPosition = ((!customDestination) ? controller.transform.position : customDestination.position);
				RaycastHit hitData;
				if (VRTK_CustomRaycast.Linecast(customRaycast, headset.position, endPosition, out hitData, default(LayerMask)))
				{
					obscured = true;
				}
				if (lastState != obscured)
				{
					ObscuredStateChanged(controller.gameObject, obscured, hitData);
				}
				lastState = obscured;
			}
		}

		protected virtual void ObscuredStateChanged(GameObject controller, bool obscured, RaycastHit hitInfo)
		{
			if (obscured)
			{
				OnControllerObscured(SetHeadsetControllerAwareEvent(hitInfo, VRTK_DeviceFinder.GetControllerIndex(controller)));
			}
			else
			{
				OnControllerUnobscured(SetHeadsetControllerAwareEvent(hitInfo, VRTK_DeviceFinder.GetControllerIndex(controller)));
			}
		}

		protected virtual void CheckHeadsetView(GameObject controller, Transform customDestination, ref bool controllerGlance, ref bool controllerGlanceLastState)
		{
			controllerGlance = false;
			if ((bool)controller && controller.gameObject.activeInHierarchy)
			{
				Vector3 vector = ((!customDestination) ? controller.transform.position : customDestination.position);
				float num = Vector3.Distance(headset.position, vector);
				Vector3 b = headset.position + headset.forward * num;
				if (Vector3.Distance(vector, b) <= controllerGlanceRadius)
				{
					controllerGlance = true;
				}
				if (controllerGlanceLastState != controllerGlance)
				{
					GlanceStateChanged(controller.gameObject, controllerGlance);
				}
				controllerGlanceLastState = controllerGlance;
			}
		}

		protected virtual void GlanceStateChanged(GameObject controller, bool glance)
		{
			RaycastHit raycastHit = default(RaycastHit);
			if (glance)
			{
				OnControllerGlanceEnter(SetHeadsetControllerAwareEvent(raycastHit, VRTK_DeviceFinder.GetControllerIndex(controller)));
			}
			else
			{
				OnControllerGlanceExit(SetHeadsetControllerAwareEvent(raycastHit, VRTK_DeviceFinder.GetControllerIndex(controller)));
			}
		}
	}
}
