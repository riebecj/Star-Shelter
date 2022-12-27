using UnityEngine;
using UnityEngine.UI;

namespace VRTK.Examples
{
	public class UI_Keyboard : MonoBehaviour
	{
		private InputField input;

		public void ClickKey(string character)
		{
			input.text += character;
		}

		public void Backspace()
		{
			if (input.text.Length > 0)
			{
				input.text = input.text.Substring(0, input.text.Length - 1);
			}
		}

		public void Enter()
		{
			VRTK_Logger.Info("You've typed [" + input.text + "]");
			input.text = string.Empty;
		}

		private void Start()
		{
			input = GetComponentInChildren<InputField>();
		}
	}
}
