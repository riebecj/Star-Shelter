using UnityEngine;

namespace VRTK
{
	[RequireComponent(typeof(RadialMenu))]
	public class RadialMenuController : MonoBehaviour
	{
		public VRTK_ControllerEvents events;

		protected RadialMenu menu;

		protected float currentAngle;

		protected bool touchpadTouched;

		protected virtual void Awake()
		{
			menu = GetComponent<RadialMenu>();
			Initialize();
		}

		protected virtual void Initialize()
		{
			if (events == null)
			{
				events = GetComponentInParent<VRTK_ControllerEvents>();
			}
		}

		protected virtual void OnEnable()
		{
			if (!(events == null))
			{
				events.TouchpadPressed += DoTouchpadClicked;
				events.TouchpadReleased += DoTouchpadUnclicked;
				events.TouchpadTouchStart += DoTouchpadTouched;
				events.TouchpadTouchEnd += DoTouchpadUntouched;
				events.TouchpadAxisChanged += DoTouchpadAxisChanged;
				menu.FireHapticPulse += AttemptHapticPulse;
			}
		}

		protected virtual void OnDisable()
		{
			if (!(events == null))
			{
				events.TouchpadPressed -= DoTouchpadClicked;
				events.TouchpadReleased -= DoTouchpadUnclicked;
				events.TouchpadTouchStart -= DoTouchpadTouched;
				events.TouchpadTouchEnd -= DoTouchpadUntouched;
				events.TouchpadAxisChanged -= DoTouchpadAxisChanged;
				menu.FireHapticPulse -= AttemptHapticPulse;
			}
		}

		public void OnGrab(VRTK_ControllerEvents newEvents)
		{
			events = newEvents;
			events.TouchpadPressed += DoTouchpadClicked;
			events.TouchpadReleased += DoTouchpadUnclicked;
			events.TouchpadTouchStart += DoTouchpadTouched;
			events.TouchpadTouchEnd += DoTouchpadUntouched;
			events.TouchpadAxisChanged += DoTouchpadAxisChanged;
			menu.FireHapticPulse += AttemptHapticPulse;
		}

		public void OnDrop(VRTK_ControllerEvents newEvents)
		{
			events = newEvents;
			events.TouchpadPressed -= DoTouchpadClicked;
			events.TouchpadReleased -= DoTouchpadUnclicked;
			events.TouchpadTouchStart -= DoTouchpadTouched;
			events.TouchpadTouchEnd -= DoTouchpadUntouched;
			events.TouchpadAxisChanged -= DoTouchpadAxisChanged;
			menu.FireHapticPulse -= AttemptHapticPulse;
		}

		protected virtual void DoClickButton(object sender = null)
		{
			menu.ClickButton(currentAngle);
		}

		protected virtual void DoUnClickButton(object sender = null)
		{
			menu.UnClickButton(currentAngle);
		}

		protected virtual void DoShowMenu(float initialAngle, object sender = null)
		{
			menu.ShowMenu();
			DoChangeAngle(initialAngle);
		}

		protected virtual void DoHideMenu(bool force, object sender = null)
		{
			menu.StopTouching();
			menu.HideMenu(force);
		}

		protected virtual void DoChangeAngle(float angle, object sender = null)
		{
			currentAngle = angle;
			menu.HoverButton(currentAngle);
		}

		protected virtual void AttemptHapticPulse(float strength)
		{
			if ((bool)events)
			{
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(events.gameObject), strength);
			}
		}

		protected virtual void DoTouchpadClicked(object sender, ControllerInteractionEventArgs e)
		{
			DoClickButton();
		}

		protected virtual void DoTouchpadUnclicked(object sender, ControllerInteractionEventArgs e)
		{
			DoUnClickButton();
		}

		protected virtual void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
		{
			touchpadTouched = true;
			DoShowMenu(CalculateAngle(e));
		}

		protected virtual void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
		{
			touchpadTouched = false;
			DoHideMenu(false);
		}

		protected virtual void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			if (touchpadTouched)
			{
				DoChangeAngle(CalculateAngle(e));
			}
		}

		protected virtual float CalculateAngle(ControllerInteractionEventArgs e)
		{
			return 360f - e.touchpadAngle;
		}
	}
}
