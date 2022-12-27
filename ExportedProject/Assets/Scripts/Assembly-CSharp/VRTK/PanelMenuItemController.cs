using UnityEngine;

namespace VRTK
{
	public class PanelMenuItemController : MonoBehaviour
	{
		public event PanelMenuItemControllerEventHandler PanelMenuItemShowing;

		public event PanelMenuItemControllerEventHandler PanelMenuItemHiding;

		public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeLeft;

		public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeRight;

		public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeTop;

		public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeBottom;

		public event PanelMenuItemControllerEventHandler PanelMenuItemTriggerPressed;

		public virtual void OnPanelMenuItemShowing(PanelMenuItemControllerEventArgs e)
		{
			if (this.PanelMenuItemShowing != null)
			{
				this.PanelMenuItemShowing(this, e);
			}
		}

		public virtual void OnPanelMenuItemHiding(PanelMenuItemControllerEventArgs e)
		{
			if (this.PanelMenuItemHiding != null)
			{
				this.PanelMenuItemHiding(this, e);
			}
		}

		public virtual void OnPanelMenuItemSwipeLeft(PanelMenuItemControllerEventArgs e)
		{
			if (this.PanelMenuItemSwipeLeft != null)
			{
				this.PanelMenuItemSwipeLeft(this, e);
			}
		}

		public virtual void OnPanelMenuItemSwipeRight(PanelMenuItemControllerEventArgs e)
		{
			if (this.PanelMenuItemSwipeRight != null)
			{
				this.PanelMenuItemSwipeRight(this, e);
			}
		}

		public virtual void OnPanelMenuItemSwipeTop(PanelMenuItemControllerEventArgs e)
		{
			if (this.PanelMenuItemSwipeTop != null)
			{
				this.PanelMenuItemSwipeTop(this, e);
			}
		}

		public virtual void OnPanelMenuItemSwipeBottom(PanelMenuItemControllerEventArgs e)
		{
			if (this.PanelMenuItemSwipeBottom != null)
			{
				this.PanelMenuItemSwipeBottom(this, e);
			}
		}

		public virtual PanelMenuItemControllerEventArgs SetPanelMenuItemEvent(GameObject interactableObject)
		{
			PanelMenuItemControllerEventArgs result = default(PanelMenuItemControllerEventArgs);
			result.interactableObject = interactableObject;
			return result;
		}

		public virtual void Show(GameObject interactableObject)
		{
			base.gameObject.SetActive(true);
			OnPanelMenuItemShowing(SetPanelMenuItemEvent(interactableObject));
		}

		public virtual void Hide(GameObject interactableObject)
		{
			base.gameObject.SetActive(false);
			OnPanelMenuItemHiding(SetPanelMenuItemEvent(interactableObject));
		}

		public virtual void SwipeLeft(GameObject interactableObject)
		{
			OnPanelMenuItemSwipeLeft(SetPanelMenuItemEvent(interactableObject));
		}

		public virtual void SwipeRight(GameObject interactableObject)
		{
			OnPanelMenuItemSwipeRight(SetPanelMenuItemEvent(interactableObject));
		}

		public virtual void SwipeTop(GameObject interactableObject)
		{
			OnPanelMenuItemSwipeTop(SetPanelMenuItemEvent(interactableObject));
		}

		public virtual void SwipeBottom(GameObject interactableObject)
		{
			OnPanelMenuItemSwipeBottom(SetPanelMenuItemEvent(interactableObject));
		}

		public virtual void TriggerPressed(GameObject interactableObject)
		{
			OnPanelMenuItemTriggerPressed(SetPanelMenuItemEvent(interactableObject));
		}

		protected virtual void OnPanelMenuItemTriggerPressed(PanelMenuItemControllerEventArgs e)
		{
			if (this.PanelMenuItemTriggerPressed != null)
			{
				this.PanelMenuItemTriggerPressed(this, e);
			}
		}
	}
}
