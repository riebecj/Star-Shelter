using System.Collections;
using UnityEngine;

namespace VRTK
{
	public class PanelMenuController : MonoBehaviour
	{
		public enum TouchpadPressPosition
		{
			None = 0,
			Top = 1,
			Bottom = 2,
			Left = 3,
			Right = 4
		}

		[Tooltip("The GameObject the panel should rotate towards, which is the Camera (eye) by default.")]
		public GameObject rotateTowards;

		[Tooltip("The scale multiplier, which relates to the scale of parent interactable object.")]
		public float zoomScaleMultiplier = 1f;

		[Tooltip("The top PanelMenuItemController, which is triggered by pressing up on the controller touchpad.")]
		public PanelMenuItemController topPanelMenuItemController;

		[Tooltip("The bottom PanelMenuItemController, which is triggered by pressing down on the controller touchpad.")]
		public PanelMenuItemController bottomPanelMenuItemController;

		[Tooltip("The left PanelMenuItemController, which is triggered by pressing left on the controller touchpad.")]
		public PanelMenuItemController leftPanelMenuItemController;

		[Tooltip("The right PanelMenuItemController, which is triggered by pressing right on the controller touchpad.")]
		public PanelMenuItemController rightPanelMenuItemController;

		protected const float CanvasScaleSize = 0.001f;

		protected const float AngleTolerance = 30f;

		protected const float SwipeMinDist = 0.2f;

		protected const float SwipeMinVelocity = 4f;

		protected VRTK_ControllerEvents controllerEvents;

		protected PanelMenuItemController currentPanelMenuItemController;

		protected GameObject interactableObject;

		protected GameObject canvasObject;

		protected readonly Vector2 xAxis = new Vector2(1f, 0f);

		protected readonly Vector2 yAxis = new Vector2(0f, 1f);

		protected Vector2 touchStartPosition;

		protected Vector2 touchEndPosition;

		protected float touchStartTime;

		protected float currentAngle;

		protected bool isTrackingSwipe;

		protected bool isPendingSwipeCheck;

		protected bool isGrabbed;

		protected bool isShown;

		public virtual void ToggleMenu()
		{
			if (isShown)
			{
				HideMenu(true);
			}
			else
			{
				ShowMenu();
			}
		}

		public virtual void ShowMenu()
		{
			if (!isShown)
			{
				isShown = true;
				StopCoroutine("TweenMenuScale");
				if (base.enabled)
				{
					StartCoroutine("TweenMenuScale", isShown);
				}
			}
		}

		public virtual void HideMenu(bool force)
		{
			if (isShown && force)
			{
				isShown = false;
				StopCoroutine("TweenMenuScale");
				if (base.enabled)
				{
					StartCoroutine("TweenMenuScale", isShown);
				}
			}
		}

		public virtual void HideMenuImmediate()
		{
			if (currentPanelMenuItemController != null && isShown)
			{
				HandlePanelMenuItemControllerVisibility(currentPanelMenuItemController);
			}
			base.transform.localScale = Vector3.zero;
			canvasObject.transform.localScale = Vector3.zero;
			isShown = false;
		}

		protected virtual void Awake()
		{
			Initialize();
		}

		protected virtual void Start()
		{
			interactableObject = base.gameObject.transform.parent.gameObject;
			if (interactableObject == null || interactableObject.GetComponent<VRTK_InteractableObject>() == null)
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PanelMenuController", "VRTK_InteractableObject", "a parent"));
				return;
			}
			interactableObject.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += DoInteractableObjectIsGrabbed;
			interactableObject.GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += DoInteractableObjectIsUngrabbed;
			canvasObject = base.gameObject.transform.GetChild(0).gameObject;
			if (canvasObject == null || canvasObject.GetComponent<Canvas>() == null)
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PanelMenuController", "Canvas", "a child"));
			}
		}

		protected virtual void Update()
		{
			if (!(interactableObject != null))
			{
				return;
			}
			if (rotateTowards == null)
			{
				rotateTowards = VRTK_DeviceFinder.HeadsetTransform().gameObject;
				if (rotateTowards == null)
				{
					VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.COULD_NOT_FIND_OBJECT_FOR_ACTION, "PanelMenuController", "an object", "rotate towards"));
				}
			}
			if (isShown && rotateTowards != null)
			{
				base.transform.rotation = Quaternion.LookRotation((rotateTowards.transform.position - base.transform.position) * -1f, Vector3.up);
			}
			if (isPendingSwipeCheck)
			{
				CalculateSwipeAction();
			}
		}

		protected virtual void Initialize()
		{
			if (Application.isPlaying && !isShown)
			{
				base.transform.localScale = Vector3.zero;
			}
			if (controllerEvents == null)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z);
				controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();
			}
		}

		protected virtual void BindControllerEvents()
		{
			controllerEvents.TouchpadPressed += DoTouchpadPress;
			controllerEvents.TouchpadTouchStart += DoTouchpadTouched;
			controllerEvents.TouchpadTouchEnd += DoTouchpadUntouched;
			controllerEvents.TouchpadAxisChanged += DoTouchpadAxisChanged;
			controllerEvents.TriggerPressed += DoTriggerPressed;
		}

		protected virtual void UnbindControllerEvents()
		{
			controllerEvents.TouchpadPressed -= DoTouchpadPress;
			controllerEvents.TouchpadTouchStart -= DoTouchpadTouched;
			controllerEvents.TouchpadTouchEnd -= DoTouchpadUntouched;
			controllerEvents.TouchpadAxisChanged -= DoTouchpadAxisChanged;
			controllerEvents.TriggerPressed -= DoTriggerPressed;
		}

		protected virtual void HandlePanelMenuItemControllerVisibility(PanelMenuItemController targetPanelItemController)
		{
			if (isShown)
			{
				if (currentPanelMenuItemController == targetPanelItemController)
				{
					targetPanelItemController.Hide(interactableObject);
					currentPanelMenuItemController = null;
					HideMenu(true);
				}
				else
				{
					currentPanelMenuItemController.Hide(interactableObject);
					currentPanelMenuItemController = targetPanelItemController;
				}
			}
			else
			{
				currentPanelMenuItemController = targetPanelItemController;
			}
			if (currentPanelMenuItemController != null)
			{
				currentPanelMenuItemController.Show(interactableObject);
				ShowMenu();
			}
		}

		protected virtual IEnumerator TweenMenuScale(bool show)
		{
			float targetScale = 0f;
			Vector3 direction = -1f * Vector3.one;
			if (show)
			{
				canvasObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
				targetScale = zoomScaleMultiplier;
				direction = Vector3.one;
			}
			for (int i = 0; i < 250; i++)
			{
				if ((!show || !(base.transform.localScale.x < targetScale)) && (show || !(base.transform.localScale.x > targetScale)))
				{
					break;
				}
				base.transform.localScale += direction * Time.deltaTime * 4f * zoomScaleMultiplier;
				yield return true;
			}
			base.transform.localScale = direction * targetScale;
			StopCoroutine("TweenMenuScale");
			if (!show)
			{
				canvasObject.transform.localScale = Vector3.zero;
			}
		}

		protected virtual void DoInteractableObjectIsGrabbed(object sender, InteractableObjectEventArgs e)
		{
			controllerEvents = e.interactingObject.GetComponentInParent<VRTK_ControllerEvents>();
			if (controllerEvents != null)
			{
				BindControllerEvents();
			}
			isGrabbed = true;
		}

		protected virtual void DoInteractableObjectIsUngrabbed(object sender, InteractableObjectEventArgs e)
		{
			isGrabbed = false;
			if (isShown)
			{
				HideMenuImmediate();
			}
			if (controllerEvents != null)
			{
				UnbindControllerEvents();
				controllerEvents = null;
			}
		}

		protected virtual void DoTouchpadPress(object sender, ControllerInteractionEventArgs e)
		{
			if (!isGrabbed)
			{
				return;
			}
			switch (CalculateTouchpadPressPosition())
			{
			case TouchpadPressPosition.Top:
				if (topPanelMenuItemController != null)
				{
					HandlePanelMenuItemControllerVisibility(topPanelMenuItemController);
				}
				break;
			case TouchpadPressPosition.Bottom:
				if (bottomPanelMenuItemController != null)
				{
					HandlePanelMenuItemControllerVisibility(bottomPanelMenuItemController);
				}
				break;
			case TouchpadPressPosition.Left:
				if (leftPanelMenuItemController != null)
				{
					HandlePanelMenuItemControllerVisibility(leftPanelMenuItemController);
				}
				break;
			case TouchpadPressPosition.Right:
				if (rightPanelMenuItemController != null)
				{
					HandlePanelMenuItemControllerVisibility(rightPanelMenuItemController);
				}
				break;
			}
		}

		protected virtual void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
		{
			touchStartPosition = new Vector2(e.touchpadAxis.x, e.touchpadAxis.y);
			touchStartTime = Time.time;
			isTrackingSwipe = true;
		}

		protected virtual void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
		{
			isTrackingSwipe = false;
			isPendingSwipeCheck = true;
		}

		protected virtual void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			ChangeAngle(CalculateAngle(e));
			if (isTrackingSwipe)
			{
				touchEndPosition = new Vector2(e.touchpadAxis.x, e.touchpadAxis.y);
			}
		}

		protected virtual void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
		{
			if (isGrabbed)
			{
				OnTriggerPressed();
			}
		}

		protected virtual void ChangeAngle(float angle, object sender = null)
		{
			currentAngle = angle;
		}

		protected virtual void CalculateSwipeAction()
		{
			isPendingSwipeCheck = false;
			float num = Time.time - touchStartTime;
			Vector2 lhs = touchEndPosition - touchStartPosition;
			float num2 = lhs.magnitude / num;
			if (!(num2 > 4f) || !(lhs.magnitude > 0.2f))
			{
				return;
			}
			lhs.Normalize();
			float f = Vector2.Dot(lhs, xAxis);
			f = Mathf.Acos(f) * 57.29578f;
			if (f < 30f)
			{
				OnSwipeRight();
				return;
			}
			if (180f - f < 30f)
			{
				OnSwipeLeft();
				return;
			}
			f = Vector2.Dot(lhs, yAxis);
			f = Mathf.Acos(f) * 57.29578f;
			if (f < 30f)
			{
				OnSwipeTop();
			}
			else if (180f - f < 30f)
			{
				OnSwipeBottom();
			}
		}

		protected virtual TouchpadPressPosition CalculateTouchpadPressPosition()
		{
			if (CheckAnglePosition(currentAngle, 30f, 0f))
			{
				return TouchpadPressPosition.Top;
			}
			if (CheckAnglePosition(currentAngle, 30f, 180f))
			{
				return TouchpadPressPosition.Bottom;
			}
			if (CheckAnglePosition(currentAngle, 30f, 270f))
			{
				return TouchpadPressPosition.Left;
			}
			if (CheckAnglePosition(currentAngle, 30f, 90f))
			{
				return TouchpadPressPosition.Right;
			}
			return TouchpadPressPosition.None;
		}

		protected virtual void OnSwipeLeft()
		{
			if (currentPanelMenuItemController != null)
			{
				currentPanelMenuItemController.SwipeLeft(interactableObject);
			}
		}

		protected virtual void OnSwipeRight()
		{
			if (currentPanelMenuItemController != null)
			{
				currentPanelMenuItemController.SwipeRight(interactableObject);
			}
		}

		protected virtual void OnSwipeTop()
		{
			if (currentPanelMenuItemController != null)
			{
				currentPanelMenuItemController.SwipeTop(interactableObject);
			}
		}

		protected virtual void OnSwipeBottom()
		{
			if (currentPanelMenuItemController != null)
			{
				currentPanelMenuItemController.SwipeBottom(interactableObject);
			}
		}

		protected virtual void OnTriggerPressed()
		{
			if (currentPanelMenuItemController != null)
			{
				currentPanelMenuItemController.TriggerPressed(interactableObject);
			}
		}

		protected virtual float CalculateAngle(ControllerInteractionEventArgs e)
		{
			return e.touchpadAngle;
		}

		protected virtual float NormAngle(float currentDegree, float maxAngle = 360f)
		{
			if (currentDegree < 0f)
			{
				currentDegree += maxAngle;
			}
			return currentDegree % maxAngle;
		}

		protected virtual bool CheckAnglePosition(float currentDegree, float tolerance, float targetDegree)
		{
			float num = NormAngle(currentDegree - tolerance);
			float num2 = NormAngle(currentDegree + tolerance);
			if (num > num2)
			{
				return targetDegree >= num || targetDegree <= num2;
			}
			return targetDegree >= num && targetDegree <= num2;
		}
	}
}
