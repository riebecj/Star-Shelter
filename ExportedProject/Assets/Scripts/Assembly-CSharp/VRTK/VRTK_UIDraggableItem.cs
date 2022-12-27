using UnityEngine;
using UnityEngine.EventSystems;

namespace VRTK
{
	[RequireComponent(typeof(CanvasGroup))]
	public class VRTK_UIDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
	{
		[Tooltip("If checked then the UI element can only be dropped in valid a VRTK_UIDropZone object and must start as a child of a VRTK_UIDropZone object. If unchecked then the UI element can be dropped anywhere on the canvas.")]
		public bool restrictToDropZone;

		[Tooltip("If checked then the UI element can only be dropped on the original parent canvas. If unchecked the UI element can be dropped on any valid VRTK_UICanvas.")]
		public bool restrictToOriginalCanvas;

		[Tooltip("The offset to bring the UI element forward when it is being dragged.")]
		public float forwardOffset = 0.1f;

		[HideInInspector]
		public GameObject validDropZone;

		protected RectTransform dragTransform;

		protected Vector3 startPosition;

		protected Quaternion startRotation;

		protected GameObject startDropZone;

		protected Transform startParent;

		protected Canvas startCanvas;

		protected CanvasGroup canvasGroup;

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			startPosition = base.transform.position;
			startRotation = base.transform.rotation;
			startParent = base.transform.parent;
			startCanvas = GetComponentInParent<Canvas>();
			canvasGroup.blocksRaycasts = false;
			if (restrictToDropZone)
			{
				startDropZone = GetComponentInParent<VRTK_UIDropZone>().gameObject;
				validDropZone = startDropZone;
			}
			SetDragPosition(eventData);
			VRTK_UIPointer pointer = GetPointer(eventData);
			if ((bool)pointer)
			{
				pointer.OnUIPointerElementDragStart(pointer.SetUIPointerEvent(pointer.pointerEventData.pointerPressRaycast, base.gameObject));
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			SetDragPosition(eventData);
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			canvasGroup.blocksRaycasts = true;
			dragTransform = null;
			base.transform.position += base.transform.forward * forwardOffset;
			bool flag = true;
			if (restrictToDropZone)
			{
				if ((bool)validDropZone && validDropZone != startDropZone)
				{
					base.transform.SetParent(validDropZone.transform);
				}
				else
				{
					ResetElement();
					flag = false;
				}
			}
			Canvas canvas = ((!eventData.pointerEnter) ? null : eventData.pointerEnter.GetComponentInParent<Canvas>());
			if (restrictToOriginalCanvas && (bool)canvas && canvas != startCanvas)
			{
				ResetElement();
				flag = false;
			}
			if (canvas == null)
			{
				ResetElement();
				flag = false;
			}
			if (flag)
			{
				VRTK_UIPointer pointer = GetPointer(eventData);
				if ((bool)pointer)
				{
					pointer.OnUIPointerElementDragEnd(pointer.SetUIPointerEvent(pointer.pointerEventData.pointerPressRaycast, base.gameObject));
				}
			}
			validDropZone = null;
			startParent = null;
			startCanvas = null;
		}

		protected virtual void OnEnable()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			if (restrictToDropZone && !GetComponentInParent<VRTK_UIDropZone>())
			{
				base.enabled = false;
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_UIDraggableItem", "VRTK_UIDropZone", "the parent", " if `freeDrop = false`"));
			}
		}

		protected virtual VRTK_UIPointer GetPointer(PointerEventData eventData)
		{
			GameObject controllerByIndex = VRTK_DeviceFinder.GetControllerByIndex((uint)eventData.pointerId, false);
			return (!controllerByIndex) ? null : controllerByIndex.GetComponent<VRTK_UIPointer>();
		}

		protected virtual void SetDragPosition(PointerEventData eventData)
		{
			if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
			{
				dragTransform = eventData.pointerEnter.transform as RectTransform;
			}
			Vector3 worldPoint;
			if ((bool)dragTransform && RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTransform, eventData.position, eventData.pressEventCamera, out worldPoint))
			{
				base.transform.position = worldPoint - base.transform.forward * forwardOffset;
				base.transform.rotation = dragTransform.rotation;
			}
		}

		protected virtual void ResetElement()
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.SetParent(startParent);
		}
	}
}
