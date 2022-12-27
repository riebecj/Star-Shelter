using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VRTK.Examples.PanelMenu
{
	public class PanelMenuUIGrid : MonoBehaviour
	{
		public enum Direction
		{
			None = 0,
			Up = 1,
			Down = 2,
			Left = 3,
			Right = 4
		}

		private readonly Color colorDefault = Color.white;

		private readonly Color colorSelected = Color.green;

		private readonly float colorAlpha = 0.25f;

		private GridLayoutGroup gridLayoutGroup;

		private int selectedIndex;

		private void Start()
		{
			gridLayoutGroup = GetComponent<GridLayoutGroup>();
			if (gridLayoutGroup == null)
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PanelMenuUIGrid", "GridLayoutGroup", "the same"));
			}
			else
			{
				GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeTop += OnPanelMenuItemSwipeTop;
				GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeBottom += OnPanelMenuItemSwipeBottom;
				GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeLeft += OnPanelMenuItemSwipeLeft;
				GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeRight += OnPanelMenuItemSwipeRight;
				GetComponentInParent<PanelMenuItemController>().PanelMenuItemTriggerPressed += OnPanelMenuItemTriggerPressed;
				SetGridLayoutItemSelectedState(selectedIndex);
			}
		}

		public bool MoveSelectGridLayoutItem(Direction direction, GameObject interactableObject)
		{
			int num = FindNextItemBasedOnMoveDirection(direction);
			if (num != selectedIndex)
			{
				SetGridLayoutItemSelectedState(num);
				selectedIndex = num;
			}
			return true;
		}

		private int FindNextItemBasedOnMoveDirection(Direction direction)
		{
			float preferredWidth = gridLayoutGroup.preferredWidth;
			float x = gridLayoutGroup.cellSize.x;
			float x2 = gridLayoutGroup.spacing.x;
			int num = (int)Mathf.Floor(preferredWidth / (x + x2 / 2f));
			int childCount = gridLayoutGroup.transform.childCount;
			switch (direction)
			{
			case Direction.Up:
			{
				int num4 = selectedIndex - num;
				return (num4 < 0) ? selectedIndex : num4;
			}
			case Direction.Down:
			{
				int num3 = selectedIndex + num;
				return (num3 >= childCount) ? selectedIndex : num3;
			}
			case Direction.Left:
			{
				int num5 = selectedIndex - 1;
				return (num5 < 0) ? selectedIndex : num5;
			}
			case Direction.Right:
			{
				int num2 = selectedIndex + 1;
				return (num2 >= childCount) ? selectedIndex : num2;
			}
			default:
				return selectedIndex;
			}
		}

		private void SetGridLayoutItemSelectedState(int index)
		{
			IEnumerator enumerator = gridLayoutGroup.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					GameObject gameObject = transform.gameObject;
					if (gameObject != null)
					{
						Color color = colorDefault;
						color.a = colorAlpha;
						gameObject.GetComponent<Image>().color = color;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			Transform child = gridLayoutGroup.transform.GetChild(index);
			if (child != null)
			{
				Color color2 = colorSelected;
				color2.a = colorAlpha;
				child.GetComponent<Image>().color = color2;
			}
		}

		private void OnPanelMenuItemSwipeTop(object sender, PanelMenuItemControllerEventArgs e)
		{
			MoveSelectGridLayoutItem(Direction.Up, e.interactableObject);
		}

		private void OnPanelMenuItemSwipeBottom(object sender, PanelMenuItemControllerEventArgs e)
		{
			MoveSelectGridLayoutItem(Direction.Down, e.interactableObject);
		}

		private void OnPanelMenuItemSwipeLeft(object sender, PanelMenuItemControllerEventArgs e)
		{
			MoveSelectGridLayoutItem(Direction.Left, e.interactableObject);
		}

		private void OnPanelMenuItemSwipeRight(object sender, PanelMenuItemControllerEventArgs e)
		{
			MoveSelectGridLayoutItem(Direction.Right, e.interactableObject);
		}

		private void OnPanelMenuItemTriggerPressed(object sender, PanelMenuItemControllerEventArgs e)
		{
			SendMessageToInteractableObject(e.interactableObject);
		}

		private void SendMessageToInteractableObject(GameObject interactableObject)
		{
			interactableObject.SendMessage("UpdateGridLayoutValue", selectedIndex);
		}
	}
}
