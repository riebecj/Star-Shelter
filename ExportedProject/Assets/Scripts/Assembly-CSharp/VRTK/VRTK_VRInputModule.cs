using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRTK
{
	public class VRTK_VRInputModule : PointerInputModule
	{
		public List<VRTK_UIPointer> pointers = new List<VRTK_UIPointer>();

		public virtual void Initialise()
		{
			pointers.Clear();
		}

		public override bool IsModuleSupported()
		{
			return false;
		}

		public override void Process()
		{
			for (int i = 0; i < pointers.Count; i++)
			{
				VRTK_UIPointer vRTK_UIPointer = pointers[i];
				if (vRTK_UIPointer.gameObject.activeInHierarchy && vRTK_UIPointer.enabled)
				{
					List<RaycastResult> results = new List<RaycastResult>();
					if (vRTK_UIPointer.PointerActive())
					{
						results = CheckRaycasts(vRTK_UIPointer);
					}
					Hover(vRTK_UIPointer, results);
					Click(vRTK_UIPointer, results);
					Drag(vRTK_UIPointer, results);
					Scroll(vRTK_UIPointer, results);
				}
			}
		}

		protected virtual List<RaycastResult> CheckRaycasts(VRTK_UIPointer pointer)
		{
			RaycastResult pointerCurrentRaycast = default(RaycastResult);
			pointerCurrentRaycast.worldPosition = pointer.GetOriginPosition();
			pointerCurrentRaycast.worldNormal = pointer.GetOriginForward();
			pointer.pointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
			List<RaycastResult> list = new List<RaycastResult>();
			base.eventSystem.RaycastAll(pointer.pointerEventData, list);
			return list;
		}

		protected virtual bool CheckTransformTree(Transform target, Transform source)
		{
			if (target == null)
			{
				return false;
			}
			if (target.Equals(source))
			{
				return true;
			}
			return CheckTransformTree(target.transform.parent, source);
		}

		protected virtual bool NoValidCollision(VRTK_UIPointer pointer, List<RaycastResult> results)
		{
			return results.Count == 0 || !CheckTransformTree(results[0].gameObject.transform, pointer.pointerEventData.pointerEnter.transform);
		}

		protected virtual bool IsHovering(VRTK_UIPointer pointer)
		{
			foreach (GameObject item in pointer.pointerEventData.hovered)
			{
				if ((bool)pointer.pointerEventData.pointerEnter && (bool)item && CheckTransformTree(item.transform, pointer.pointerEventData.pointerEnter.transform))
				{
					return true;
				}
			}
			return false;
		}

		protected virtual bool ValidElement(GameObject obj)
		{
			VRTK_UICanvas componentInParent = obj.GetComponentInParent<VRTK_UICanvas>();
			return ((bool)componentInParent && componentInParent.enabled) ? true : false;
		}

		protected virtual void CheckPointerHoverClick(VRTK_UIPointer pointer, List<RaycastResult> results)
		{
			if (pointer.hoverDurationTimer > 0f)
			{
				pointer.hoverDurationTimer -= Time.deltaTime;
			}
			if (pointer.canClickOnHover && pointer.hoverDurationTimer <= 0f)
			{
				pointer.canClickOnHover = false;
				ClickOnDown(pointer, results, true);
			}
		}

		protected virtual void Hover(VRTK_UIPointer pointer, List<RaycastResult> results)
		{
			if ((bool)pointer.pointerEventData.pointerEnter)
			{
				CheckPointerHoverClick(pointer, results);
				if (!ValidElement(pointer.pointerEventData.pointerEnter))
				{
					pointer.pointerEventData.pointerEnter = null;
				}
				else if (NoValidCollision(pointer, results))
				{
					ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerEnter, pointer.pointerEventData, ExecuteEvents.pointerExitHandler);
					pointer.pointerEventData.hovered.Remove(pointer.pointerEventData.pointerEnter);
					pointer.pointerEventData.pointerEnter = null;
				}
				return;
			}
			foreach (RaycastResult result in results)
			{
				if (!ValidElement(result.gameObject))
				{
					continue;
				}
				GameObject gameObject = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.pointerEnterHandler);
				if (gameObject != null)
				{
					Selectable component = gameObject.GetComponent<Selectable>();
					if ((bool)component)
					{
						Navigation navigation = default(Navigation);
						navigation.mode = Navigation.Mode.None;
						component.navigation = navigation;
					}
					pointer.OnUIPointerElementEnter(pointer.SetUIPointerEvent(result, gameObject, pointer.hoveringElement));
					pointer.hoveringElement = gameObject;
					pointer.pointerEventData.pointerCurrentRaycast = result;
					pointer.pointerEventData.pointerEnter = gameObject;
					pointer.pointerEventData.hovered.Add(pointer.pointerEventData.pointerEnter);
					break;
				}
				if (result.gameObject != pointer.hoveringElement)
				{
					pointer.OnUIPointerElementEnter(pointer.SetUIPointerEvent(result, result.gameObject, pointer.hoveringElement));
				}
				pointer.hoveringElement = result.gameObject;
			}
			if ((bool)pointer.hoveringElement && results.Count == 0)
			{
				pointer.OnUIPointerElementExit(pointer.SetUIPointerEvent(default(RaycastResult), null, pointer.hoveringElement));
				pointer.hoveringElement = null;
			}
		}

		protected virtual void Click(VRTK_UIPointer pointer, List<RaycastResult> results)
		{
			switch (pointer.clickMethod)
			{
			case VRTK_UIPointer.ClickMethods.ClickOnButtonUp:
				ClickOnUp(pointer, results);
				break;
			case VRTK_UIPointer.ClickMethods.ClickOnButtonDown:
				ClickOnDown(pointer, results);
				break;
			}
		}

		protected virtual void ClickOnUp(VRTK_UIPointer pointer, List<RaycastResult> results)
		{
			pointer.pointerEventData.eligibleForClick = pointer.ValidClick(false);
			if (!AttemptClick(pointer))
			{
				IsEligibleClick(pointer, results);
			}
		}

		protected virtual void ClickOnDown(VRTK_UIPointer pointer, List<RaycastResult> results, bool forceClick = false)
		{
			pointer.pointerEventData.eligibleForClick = forceClick || pointer.ValidClick(true);
			if (IsEligibleClick(pointer, results))
			{
				pointer.pointerEventData.eligibleForClick = false;
				AttemptClick(pointer);
			}
		}

		protected virtual bool IsEligibleClick(VRTK_UIPointer pointer, List<RaycastResult> results)
		{
			if (pointer.pointerEventData.eligibleForClick)
			{
				foreach (RaycastResult result in results)
				{
					if (ValidElement(result.gameObject))
					{
						GameObject gameObject = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.pointerDownHandler);
						if (gameObject != null)
						{
							pointer.pointerEventData.pressPosition = pointer.pointerEventData.position;
							pointer.pointerEventData.pointerPressRaycast = result;
							pointer.pointerEventData.pointerPress = gameObject;
							return true;
						}
					}
				}
			}
			return false;
		}

		protected virtual bool AttemptClick(VRTK_UIPointer pointer)
		{
			if ((bool)pointer.pointerEventData.pointerPress)
			{
				if (!ValidElement(pointer.pointerEventData.pointerPress))
				{
					return true;
				}
				if (pointer.pointerEventData.eligibleForClick)
				{
					if (!IsHovering(pointer))
					{
						ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerUpHandler);
						pointer.pointerEventData.pointerPress = null;
					}
				}
				else
				{
					pointer.OnUIPointerElementClick(pointer.SetUIPointerEvent(pointer.pointerEventData.pointerPressRaycast, pointer.pointerEventData.pointerPress));
					ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerClickHandler);
					ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerUpHandler);
					pointer.pointerEventData.pointerPress = null;
				}
				return true;
			}
			return false;
		}

		protected virtual void Drag(VRTK_UIPointer pointer, List<RaycastResult> results)
		{
			pointer.pointerEventData.dragging = pointer.IsSelectionButtonPressed() && pointer.pointerEventData.delta != Vector2.zero;
			if ((bool)pointer.pointerEventData.pointerDrag)
			{
				if (!ValidElement(pointer.pointerEventData.pointerDrag))
				{
					return;
				}
				if (pointer.pointerEventData.dragging)
				{
					if (IsHovering(pointer))
					{
						ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.dragHandler);
					}
					return;
				}
				ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.dragHandler);
				ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.endDragHandler);
				foreach (RaycastResult result in results)
				{
					ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.dropHandler);
				}
				pointer.pointerEventData.pointerDrag = null;
			}
			else
			{
				if (!pointer.pointerEventData.dragging)
				{
					return;
				}
				foreach (RaycastResult result2 in results)
				{
					if (ValidElement(result2.gameObject))
					{
						ExecuteEvents.ExecuteHierarchy(result2.gameObject, pointer.pointerEventData, ExecuteEvents.initializePotentialDrag);
						ExecuteEvents.ExecuteHierarchy(result2.gameObject, pointer.pointerEventData, ExecuteEvents.beginDragHandler);
						GameObject gameObject = ExecuteEvents.ExecuteHierarchy(result2.gameObject, pointer.pointerEventData, ExecuteEvents.dragHandler);
						if (gameObject != null)
						{
							pointer.pointerEventData.pointerDrag = gameObject;
							break;
						}
					}
				}
			}
		}

		protected virtual void Scroll(VRTK_UIPointer pointer, List<RaycastResult> results)
		{
			pointer.pointerEventData.scrollDelta = pointer.controller.GetTouchpadAxis();
			bool state = false;
			foreach (RaycastResult result in results)
			{
				if (pointer.pointerEventData.scrollDelta != Vector2.zero)
				{
					GameObject gameObject = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.scrollHandler);
					if ((bool)gameObject)
					{
						state = true;
					}
				}
			}
			if ((bool)pointer.controllerRenderModel)
			{
				VRTK_SDK_Bridge.SetControllerRenderModelWheel(pointer.controllerRenderModel, state);
			}
		}
	}
}
