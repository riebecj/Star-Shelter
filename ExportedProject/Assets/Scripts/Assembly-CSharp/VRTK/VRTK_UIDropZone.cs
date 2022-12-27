using UnityEngine;
using UnityEngine.EventSystems;

namespace VRTK
{
	public class VRTK_UIDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
	{
		protected VRTK_UIDraggableItem droppableItem;

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			if ((bool)eventData.pointerDrag)
			{
				VRTK_UIDraggableItem component = eventData.pointerDrag.GetComponent<VRTK_UIDraggableItem>();
				if ((bool)component && component.restrictToDropZone)
				{
					component.validDropZone = base.gameObject;
					droppableItem = component;
				}
			}
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			if ((bool)droppableItem)
			{
				droppableItem.validDropZone = null;
			}
			droppableItem = null;
		}
	}
}
