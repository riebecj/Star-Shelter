using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRTK.Examples
{
	public class UI_Interactions : MonoBehaviour
	{
		private const int EXISTING_CANVAS_COUNT = 4;

		public void Button_Red()
		{
			VRTK_Logger.Info("Red Button Clicked");
		}

		public void Button_Pink()
		{
			VRTK_Logger.Info("Pink Button Clicked");
		}

		public void Toggle(bool state)
		{
			VRTK_Logger.Info("The toggle state is " + ((!state) ? "off" : "on"));
		}

		public void Dropdown(int value)
		{
			VRTK_Logger.Info("Dropdown option selected was ID " + value);
		}

		public void SetDropText(BaseEventData data)
		{
			PointerEventData pointerEventData = data as PointerEventData;
			GameObject gameObject = GameObject.Find("ActionText");
			if ((bool)gameObject)
			{
				Text component = gameObject.GetComponent<Text>();
				component.text = pointerEventData.pointerDrag.name + " Dropped On " + pointerEventData.pointerEnter.name;
			}
		}

		public void CreateCanvas()
		{
			StartCoroutine(CreateCanvasOnNextFrame());
		}

		private IEnumerator CreateCanvasOnNextFrame()
		{
			yield return null;
			int canvasCount = Object.FindObjectsOfType<Canvas>().Length - 4;
			GameObject newCanvasGO = new GameObject("TempCanvas")
			{
				layer = 5
			};
			Canvas canvas = newCanvasGO.AddComponent<Canvas>();
			RectTransform canvasRT = canvas.GetComponent<RectTransform>();
			canvasRT.position = new Vector3(-4f, 2f, 3f + (float)canvasCount);
			canvasRT.sizeDelta = new Vector2(300f, 400f);
			canvasRT.localScale = new Vector3(0.005f, 0.005f, 0.005f);
			canvasRT.eulerAngles = new Vector3(0f, 270f, 0f);
			GameObject newButtonGO = new GameObject("TempButton", typeof(RectTransform));
			newButtonGO.transform.SetParent(newCanvasGO.transform);
			newButtonGO.layer = 5;
			RectTransform buttonRT = newButtonGO.GetComponent<RectTransform>();
			buttonRT.position = new Vector3(0f, 0f, 0f);
			buttonRT.anchoredPosition = new Vector3(0f, 0f, 0f);
			buttonRT.localPosition = new Vector3(0f, 0f, 0f);
			buttonRT.sizeDelta = new Vector2(180f, 60f);
			buttonRT.localScale = new Vector3(1f, 1f, 1f);
			buttonRT.localEulerAngles = new Vector3(0f, 0f, 0f);
			newButtonGO.AddComponent<Image>();
			Button canvasButton = newButtonGO.AddComponent<Button>();
			ColorBlock buttonColourBlock = canvasButton.colors;
			buttonColourBlock.highlightedColor = Color.red;
			canvasButton.colors = buttonColourBlock;
			GameObject newTextGO = new GameObject("BtnText", typeof(RectTransform));
			newTextGO.transform.SetParent(newButtonGO.transform);
			newTextGO.layer = 5;
			RectTransform textRT = newTextGO.GetComponent<RectTransform>();
			textRT.position = new Vector3(0f, 0f, 0f);
			textRT.anchoredPosition = new Vector3(0f, 0f, 0f);
			textRT.localPosition = new Vector3(0f, 0f, 0f);
			textRT.sizeDelta = new Vector2(180f, 60f);
			textRT.localScale = new Vector3(1f, 1f, 1f);
			textRT.localEulerAngles = new Vector3(0f, 0f, 0f);
			Text txt = newTextGO.AddComponent<Text>();
			txt.text = "New Button";
			txt.color = Color.black;
			txt.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			newCanvasGO.AddComponent<VRTK_UICanvas>();
		}
	}
}
