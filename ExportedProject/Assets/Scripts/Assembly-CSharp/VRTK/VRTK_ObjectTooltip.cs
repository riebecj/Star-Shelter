using UnityEngine;
using UnityEngine.UI;

namespace VRTK
{
	public class VRTK_ObjectTooltip : MonoBehaviour
	{
		[Tooltip("The text that is displayed on the tooltip.")]
		public string displayText;

		[Tooltip("The size of the text that is displayed.")]
		public int fontSize = 14;

		[Tooltip("The size of the tooltip container where `x = width` and `y = height`.")]
		public Vector2 containerSize = new Vector2(0.1f, 0.03f);

		[Tooltip("An optional transform of where to start drawing the line from. If one is not provided the centre of the tooltip is used for the initial line position.")]
		public Transform drawLineFrom;

		[Tooltip("A transform of another object in the scene that a line will be drawn from the tooltip to, this helps denote what the tooltip is in relation to. If no transform is provided and the tooltip is a child of another object, then the parent object's transform will be used as this destination position.")]
		public Transform drawLineTo;

		[Tooltip("The width of the line drawn between the tooltip and the destination transform.")]
		public float lineWidth = 0.001f;

		[Tooltip("The colour to use for the text on the tooltip.")]
		public Color fontColor = Color.black;

		[Tooltip("The colour to use for the background container of the tooltip.")]
		public Color containerColor = Color.black;

		[Tooltip("The colour to use for the line drawn between the tooltip and the destination transform.")]
		public Color lineColor = Color.black;

		private LineRenderer line;

		public void ResetTooltip()
		{
			SetContainer();
			SetText("UITextFront");
			SetText("UITextReverse");
			SetLine();
			if (drawLineTo == null && base.transform.parent != null)
			{
				drawLineTo = base.transform.parent;
			}
		}

		public void UpdateText(string newText)
		{
			displayText = newText;
			ResetTooltip();
		}

		protected virtual void Start()
		{
			ResetTooltip();
		}

		protected virtual void Update()
		{
			DrawLine();
		}

		private void SetContainer()
		{
			base.transform.Find("TooltipCanvas").GetComponent<RectTransform>().sizeDelta = containerSize;
			Transform transform = base.transform.Find("TooltipCanvas/UIContainer");
			transform.GetComponent<RectTransform>().sizeDelta = containerSize;
			transform.GetComponent<Image>().color = containerColor;
		}

		private void SetText(string name)
		{
			Text component = base.transform.Find("TooltipCanvas/" + name).GetComponent<Text>();
			component.material = Resources.Load("UIText") as Material;
			component.text = displayText.Replace("\\n", "\n");
			component.color = fontColor;
			component.fontSize = fontSize;
		}

		private void SetLine()
		{
			line = base.transform.Find("Line").GetComponent<LineRenderer>();
			line.material = Resources.Load("TooltipLine") as Material;
			line.material.color = lineColor;
			line.startColor = lineColor;
			line.endColor = lineColor;
			line.startWidth = lineWidth;
			line.endWidth = lineWidth;
			if (drawLineFrom == null)
			{
				drawLineFrom = base.transform;
			}
		}

		private void DrawLine()
		{
			if ((bool)drawLineTo)
			{
				line.SetPosition(0, drawLineFrom.position);
				line.SetPosition(1, drawLineTo.position);
			}
		}
	}
}
