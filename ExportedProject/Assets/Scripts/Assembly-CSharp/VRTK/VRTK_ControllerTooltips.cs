using UnityEngine;

namespace VRTK
{
	public class VRTK_ControllerTooltips : MonoBehaviour
	{
		public enum TooltipButtons
		{
			None = 0,
			TriggerTooltip = 1,
			GripTooltip = 2,
			TouchpadTooltip = 3,
			ButtonOneTooltip = 4,
			ButtonTwoTooltip = 5,
			StartMenuTooltip = 6
		}

		[Tooltip("The text to display for the trigger button action.")]
		public string triggerText;

		[Tooltip("The text to display for the grip button action.")]
		public string gripText;

		[Tooltip("The text to display for the touchpad action.")]
		public string touchpadText;

		[Tooltip("The text to display for button one action.")]
		public string buttonOneText;

		[Tooltip("The text to display for button two action.")]
		public string buttonTwoText;

		[Tooltip("The text to display for the start menu action.")]
		public string startMenuText;

		[Tooltip("The colour to use for the tooltip background container.")]
		public Color tipBackgroundColor = Color.black;

		[Tooltip("The colour to use for the text within the tooltip.")]
		public Color tipTextColor = Color.white;

		[Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
		public Color tipLineColor = Color.black;

		[Tooltip("The transform for the position of the trigger button on the controller.")]
		public Transform trigger;

		[Tooltip("The transform for the position of the grip button on the controller.")]
		public Transform grip;

		[Tooltip("The transform for the position of the touchpad button on the controller.")]
		public Transform touchpad;

		[Tooltip("The transform for the position of button one on the controller.")]
		public Transform buttonOne;

		[Tooltip("The transform for the position of button two on the controller.")]
		public Transform buttonTwo;

		[Tooltip("The transform for the position of the start menu on the controller.")]
		public Transform startMenu;

		protected bool triggerInitialised;

		protected bool gripInitialised;

		protected bool touchpadInitialised;

		protected bool buttonOneInitialised;

		protected bool buttonTwoInitialised;

		protected bool startMenuInitialised;

		protected TooltipButtons[] availableButtons;

		protected VRTK_ObjectTooltip[] buttonTooltips;

		protected bool[] tooltipStates;

		protected VRTK_ControllerEvents controllerEvents;

		protected VRTK_HeadsetControllerAware headsetControllerAware;

		public virtual void ResetTooltip()
		{
			triggerInitialised = false;
			gripInitialised = false;
			touchpadInitialised = false;
			buttonOneInitialised = false;
			buttonTwoInitialised = false;
			startMenuInitialised = false;
		}

		public virtual void UpdateText(TooltipButtons element, string newText)
		{
			switch (element)
			{
			case TooltipButtons.ButtonOneTooltip:
				buttonOneText = newText;
				break;
			case TooltipButtons.ButtonTwoTooltip:
				buttonTwoText = newText;
				break;
			case TooltipButtons.StartMenuTooltip:
				startMenuText = newText;
				break;
			case TooltipButtons.GripTooltip:
				gripText = newText;
				break;
			case TooltipButtons.TouchpadTooltip:
				touchpadText = newText;
				break;
			case TooltipButtons.TriggerTooltip:
				triggerText = newText;
				break;
			}
			ResetTooltip();
		}

		public virtual void ToggleTips(bool state, TooltipButtons element = TooltipButtons.None)
		{
			if (element == TooltipButtons.None)
			{
				for (int i = 1; i < buttonTooltips.Length; i++)
				{
					if (buttonTooltips[i].displayText.Length > 0)
					{
						buttonTooltips[i].gameObject.SetActive(state);
					}
				}
			}
			else if (buttonTooltips[(int)element].displayText.Length > 0)
			{
				buttonTooltips[(int)element].gameObject.SetActive(state);
			}
		}

		protected virtual void Awake()
		{
			controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();
			triggerInitialised = false;
			gripInitialised = false;
			touchpadInitialised = false;
			buttonOneInitialised = false;
			buttonTwoInitialised = false;
			startMenuInitialised = false;
			availableButtons = new TooltipButtons[7]
			{
				TooltipButtons.None,
				TooltipButtons.TriggerTooltip,
				TooltipButtons.GripTooltip,
				TooltipButtons.TouchpadTooltip,
				TooltipButtons.ButtonOneTooltip,
				TooltipButtons.ButtonTwoTooltip,
				TooltipButtons.StartMenuTooltip
			};
			buttonTooltips = new VRTK_ObjectTooltip[availableButtons.Length];
			tooltipStates = new bool[availableButtons.Length];
			for (int i = 1; i < availableButtons.Length; i++)
			{
				buttonTooltips[i] = base.transform.Find(availableButtons[i].ToString()).GetComponent<VRTK_ObjectTooltip>();
			}
			InitialiseTips();
		}

		protected virtual void OnEnable()
		{
			if (controllerEvents != null)
			{
				controllerEvents.ControllerVisible += DoControllerVisible;
				controllerEvents.ControllerHidden += DoControllerInvisible;
			}
			headsetControllerAware = Object.FindObjectOfType<VRTK_HeadsetControllerAware>();
			if ((bool)headsetControllerAware)
			{
				headsetControllerAware.ControllerGlanceEnter += DoGlanceEnterController;
				headsetControllerAware.ControllerGlanceExit += DoGlanceExitController;
				ToggleTips(false);
			}
		}

		protected virtual void OnDisable()
		{
			if (controllerEvents != null)
			{
				controllerEvents.ControllerVisible -= DoControllerVisible;
				controllerEvents.ControllerHidden -= DoControllerInvisible;
			}
			if ((bool)headsetControllerAware)
			{
				headsetControllerAware.ControllerGlanceEnter -= DoGlanceEnterController;
				headsetControllerAware.ControllerGlanceExit -= DoGlanceExitController;
			}
		}

		protected virtual void Update()
		{
			if (!TipsInitialised() && controllerEvents != null)
			{
				GameObject actualController = VRTK_DeviceFinder.GetActualController(controllerEvents.gameObject);
				if ((bool)actualController && actualController.activeInHierarchy)
				{
					InitialiseTips();
				}
			}
		}

		protected virtual void DoControllerVisible(object sender, ControllerInteractionEventArgs e)
		{
			for (int i = 0; i < availableButtons.Length; i++)
			{
				ToggleTips(tooltipStates[i], availableButtons[i]);
			}
		}

		protected virtual void DoControllerInvisible(object sender, ControllerInteractionEventArgs e)
		{
			for (int i = 1; i < buttonTooltips.Length; i++)
			{
				tooltipStates[i] = buttonTooltips[i].gameObject.activeSelf;
			}
			ToggleTips(false);
		}

		protected virtual void DoGlanceEnterController(object sender, HeadsetControllerAwareEventArgs e)
		{
			if (controllerEvents != null)
			{
				uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(controllerEvents.gameObject);
				if (controllerIndex == e.controllerIndex)
				{
					ToggleTips(true);
				}
			}
		}

		protected virtual void DoGlanceExitController(object sender, HeadsetControllerAwareEventArgs e)
		{
			if (controllerEvents != null)
			{
				uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(controllerEvents.gameObject);
				if (controllerIndex == e.controllerIndex)
				{
					ToggleTips(false);
				}
			}
		}

		protected virtual void InitialiseTips()
		{
			VRTK_ObjectTooltip[] componentsInChildren = GetComponentsInChildren<VRTK_ObjectTooltip>(true);
			foreach (VRTK_ObjectTooltip vRTK_ObjectTooltip in componentsInChildren)
			{
				string empty = string.Empty;
				Transform transform = null;
				switch (vRTK_ObjectTooltip.name.Replace("Tooltip", string.Empty).ToLower())
				{
				case "trigger":
					empty = triggerText;
					transform = GetTransform(trigger, SDK_BaseController.ControllerElements.Trigger);
					if (transform != null)
					{
						triggerInitialised = true;
					}
					break;
				case "grip":
					empty = gripText;
					transform = GetTransform(grip, SDK_BaseController.ControllerElements.GripLeft);
					if (transform != null)
					{
						gripInitialised = true;
					}
					break;
				case "touchpad":
					empty = touchpadText;
					transform = GetTransform(touchpad, SDK_BaseController.ControllerElements.Touchpad);
					if (transform != null)
					{
						touchpadInitialised = true;
					}
					break;
				case "buttonone":
					empty = buttonOneText;
					transform = GetTransform(buttonOne, SDK_BaseController.ControllerElements.ButtonOne);
					if (transform != null)
					{
						buttonOneInitialised = true;
					}
					break;
				case "buttontwo":
					empty = buttonTwoText;
					transform = GetTransform(buttonTwo, SDK_BaseController.ControllerElements.ButtonTwo);
					if (transform != null)
					{
						buttonTwoInitialised = true;
					}
					break;
				case "startmenu":
					empty = startMenuText;
					transform = GetTransform(startMenu, SDK_BaseController.ControllerElements.StartMenu);
					if (transform != null)
					{
						startMenuInitialised = true;
					}
					break;
				}
				vRTK_ObjectTooltip.displayText = empty;
				vRTK_ObjectTooltip.drawLineTo = transform;
				vRTK_ObjectTooltip.containerColor = tipBackgroundColor;
				vRTK_ObjectTooltip.fontColor = tipTextColor;
				vRTK_ObjectTooltip.lineColor = tipLineColor;
				vRTK_ObjectTooltip.ResetTooltip();
				if (empty.Trim().Length == 0)
				{
					vRTK_ObjectTooltip.gameObject.SetActive(false);
				}
			}
		}

		protected virtual bool TipsInitialised()
		{
			return triggerInitialised && gripInitialised && touchpadInitialised && (buttonOneInitialised || buttonTwoInitialised || startMenuInitialised);
		}

		protected virtual Transform GetTransform(Transform setTransform, SDK_BaseController.ControllerElements findElement)
		{
			Transform result = null;
			if ((bool)setTransform)
			{
				result = setTransform;
			}
			else if (controllerEvents != null)
			{
				GameObject modelAliasController = VRTK_DeviceFinder.GetModelAliasController(controllerEvents.gameObject);
				if ((bool)modelAliasController && modelAliasController.activeInHierarchy)
				{
					SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerEvents.gameObject);
					string controllerElementPath = VRTK_SDK_Bridge.GetControllerElementPath(findElement, controllerHand, true);
					result = modelAliasController.transform.Find(controllerElementPath);
				}
			}
			return result;
		}
	}
}
