using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VRTK
{
	[ExecuteInEditMode]
	public class RadialMenu : MonoBehaviour
	{
		[Serializable]
		public class RadialMenuButton
		{
			public Sprite ButtonIcon;

			public UnityEvent OnClick = new UnityEvent();

			public UnityEvent OnHold = new UnityEvent();

			public UnityEvent OnHoverEnter = new UnityEvent();

			public UnityEvent OnHoverExit = new UnityEvent();
		}

		public enum ButtonEvent
		{
			hoverOn = 0,
			hoverOff = 1,
			click = 2,
			unclick = 3
		}

		[Tooltip("An array of Buttons that define the interactive buttons required to be displayed as part of the radial menu.")]
		public List<RadialMenuButton> buttons;

		[Tooltip("The base for each button in the menu, by default set to a dynamic circle arc that will fill up a portion of the menu.")]
		public GameObject buttonPrefab;

		[Tooltip("If checked, then the buttons will be auto generated on awake.")]
		public bool generateOnAwake = true;

		[Tooltip("Percentage of the menu the buttons should fill, 1.0 is a pie slice, 0.1 is a thin ring.")]
		[Range(0f, 1f)]
		public float buttonThickness = 0.5f;

		[Tooltip("The background colour of the buttons, default is white.")]
		public Color buttonColor = Color.white;

		[Tooltip("The distance the buttons should move away from the centre. This creates space between the individual buttons.")]
		public float offsetDistance = 1f;

		[Tooltip("The additional rotation of the Radial Menu.")]
		[Range(0f, 359f)]
		public float offsetRotation;

		[Tooltip("Whether button icons should rotate according to their arc or be vertical compared to the controller.")]
		public bool rotateIcons;

		[Tooltip("The margin in pixels that the icon should keep within the button.")]
		public float iconMargin;

		[Tooltip("Whether the buttons are shown")]
		public bool isShown;

		[Tooltip("Whether the buttons should be visible when not in use.")]
		public bool hideOnRelease;

		[Tooltip("Whether the button action should happen when the button is released, as opposed to happening immediately when the button is pressed.")]
		internal bool executeOnUnclick;

		[Tooltip("The base strength of the haptic pulses when the selected button is changed, or a button is pressed. Set to zero to disable.")]
		[Range(0f, 1f)]
		public float baseHapticStrength;

		[Tooltip("The actual GameObjects that make up the radial menu.")]
		public List<GameObject> menuButtons;

		protected int currentHover = -1;

		protected int currentPress = -1;

		public event HapticPulseEventHandler FireHapticPulse;

		public virtual void HoverButton(float angle)
		{
			InteractButton(angle, ButtonEvent.hoverOn);
		}

		public virtual void ClickButton(float angle)
		{
			InteractButton(angle, ButtonEvent.click);
		}

		public virtual void UnClickButton(float angle)
		{
			InteractButton(angle, ButtonEvent.unclick);
		}

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

		public virtual void StopTouching()
		{
			if (currentHover != -1)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				ExecuteEvents.Execute(menuButtons[currentHover], eventData, ExecuteEvents.pointerExitHandler);
				buttons[currentHover].OnHoverExit.Invoke();
				currentHover = -1;
			}
		}

		public virtual void ShowMenu()
		{
			if (!isShown)
			{
				isShown = true;
				StopCoroutine("TweenMenuScale");
				StartCoroutine("TweenMenuScale", isShown);
			}
		}

		public virtual RadialMenuButton GetButton(int id)
		{
			if (id < buttons.Count)
			{
				return buttons[id];
			}
			return null;
		}

		public virtual void HideMenu(bool force)
		{
			if (isShown && (hideOnRelease || force))
			{
				isShown = false;
				StopCoroutine("TweenMenuScale");
				StartCoroutine("TweenMenuScale", isShown);
			}
		}

		public void RegenerateButtons()
		{
			RemoveAllButtons();
			for (int i = 0; i < buttons.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				gameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				UICircle component = gameObject.GetComponent<UICircle>();
				if (buttonThickness == 1f)
				{
					component.fill = true;
				}
				else
				{
					component.thickness = (int)(buttonThickness * (GetComponent<RectTransform>().rect.width / 2f));
				}
				int fillPercent = (int)(100f / (float)buttons.Count);
				component.fillPercent = fillPercent;
				component.color = buttonColor;
				float num = (float)(360 / buttons.Count * i) + offsetRotation;
				gameObject.transform.localEulerAngles = new Vector3(0f, 0f, num);
				gameObject.layer = 4;
				gameObject.transform.localPosition = Vector3.zero;
				if (component.fillPercent < 55)
				{
					float f = num * (float)Math.PI / 180f;
					Vector2 vector = new Vector2(0f - Mathf.Cos(f), 0f - Mathf.Sin(f));
					gameObject.transform.localPosition += (Vector3)vector * offsetDistance;
				}
				GameObject gameObject2 = gameObject.GetComponentInChildren<RadialButtonIcon>().gameObject;
				if (buttons[i].ButtonIcon == null)
				{
					gameObject2.SetActive(false);
				}
				else
				{
					gameObject2.GetComponent<Image>().sprite = buttons[i].ButtonIcon;
					gameObject2.transform.localPosition = new Vector2(-1f * (gameObject.GetComponent<RectTransform>().rect.width / 2f - (float)component.thickness / 2f), 0f);
					float a = Mathf.Abs(component.thickness);
					float num2 = Mathf.Abs(gameObject2.transform.localPosition.x);
					float num3 = 359f * (float)component.fillPercent * 0.01f * (float)Math.PI / 180f;
					float b = num2 * 2f * Mathf.Sin(num3 / 2f);
					if (component.fillPercent > 24)
					{
						b = float.MaxValue;
					}
					float num4 = Mathf.Min(a, b) - iconMargin;
					gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(num4, num4);
					if (!rotateIcons)
					{
						gameObject2.transform.eulerAngles = GetComponentInParent<Canvas>().transform.eulerAngles;
					}
				}
				menuButtons.Add(gameObject);
			}
		}

		public void AddButton(RadialMenuButton newButton, Sprite newIcon)
		{
			buttons.Add(newButton);
			newButton.ButtonIcon = newIcon;
			RegenerateButtons();
		}

		protected virtual void Awake()
		{
			if (Application.isPlaying)
			{
				if (!isShown)
				{
					base.transform.localScale = Vector3.zero;
				}
				if (generateOnAwake)
				{
					RegenerateButtons();
				}
			}
		}

		protected virtual void Update()
		{
			if (currentPress != -1 && !Gun.instance.holstered)
			{
				buttons[currentPress].OnHold.Invoke();
			}
		}

		protected virtual void InteractButton(float angle, ButtonEvent evt)
		{
			float num = 360f / (float)buttons.Count;
			angle = VRTK_SharedMethods.Mod(angle + (0f - offsetRotation), 360f);
			int num2 = (int)VRTK_SharedMethods.Mod((angle + num / 2f) / num, buttons.Count);
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			if (currentHover != num2 && currentHover != -1)
			{
				ExecuteEvents.Execute(menuButtons[currentHover], eventData, ExecuteEvents.pointerUpHandler);
				ExecuteEvents.Execute(menuButtons[currentHover], eventData, ExecuteEvents.pointerExitHandler);
				buttons[currentHover].OnHoverExit.Invoke();
				if (executeOnUnclick && currentPress != -1)
				{
					ExecuteEvents.Execute(menuButtons[num2], eventData, ExecuteEvents.pointerDownHandler);
					AttempHapticPulse(baseHapticStrength * 1.666f);
				}
			}
			switch (evt)
			{
			case ButtonEvent.click:
				ExecuteEvents.Execute(menuButtons[num2], eventData, ExecuteEvents.pointerDownHandler);
				currentPress = num2;
				if (!executeOnUnclick)
				{
					buttons[num2].OnClick.Invoke();
					AttempHapticPulse(baseHapticStrength * 2.5f);
				}
				break;
			case ButtonEvent.unclick:
				ExecuteEvents.Execute(menuButtons[num2], eventData, ExecuteEvents.pointerUpHandler);
				currentPress = -1;
				if (executeOnUnclick)
				{
					AttempHapticPulse(baseHapticStrength * 2.5f);
					buttons[num2].OnClick.Invoke();
				}
				break;
			case ButtonEvent.hoverOn:
				if (currentHover != num2)
				{
					ExecuteEvents.Execute(menuButtons[num2], eventData, ExecuteEvents.pointerEnterHandler);
					buttons[num2].OnHoverEnter.Invoke();
					AttempHapticPulse(baseHapticStrength);
				}
				break;
			}
			currentHover = num2;
		}

		protected virtual IEnumerator TweenMenuScale(bool show)
		{
			float targetScale = 0f;
			Vector3 Dir = -1f * Vector3.one;
			if (show)
			{
				targetScale = 1f;
				Dir = Vector3.one;
			}
			for (int i = 0; i < 250; i++)
			{
				if ((!show || !(base.transform.localScale.x < targetScale)) && (show || !(base.transform.localScale.x > targetScale)))
				{
					break;
				}
				base.transform.localScale += Dir * Time.deltaTime * 4f;
				yield return true;
			}
			base.transform.localScale = Dir * targetScale;
			StopCoroutine("TweenMenuScale");
		}

		protected virtual void AttempHapticPulse(float strength)
		{
			if (strength > 0f && this.FireHapticPulse != null)
			{
				this.FireHapticPulse(strength);
			}
		}

		protected virtual void RemoveAllButtons()
		{
			if (menuButtons == null)
			{
				menuButtons = new List<GameObject>();
			}
			for (int i = 0; i < menuButtons.Count; i++)
			{
				UnityEngine.Object.Destroy(menuButtons[i]);
			}
			menuButtons = new List<GameObject>();
		}
	}
}
