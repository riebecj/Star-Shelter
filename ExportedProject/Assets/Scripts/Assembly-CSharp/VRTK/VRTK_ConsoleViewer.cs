using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace VRTK
{
	public class VRTK_ConsoleViewer : MonoBehaviour
	{
		[Tooltip("The size of the font the log text is displayed in.")]
		public int fontSize = 14;

		[Tooltip("The colour of the text for an info log message.")]
		public Color infoMessage = Color.black;

		[Tooltip("The colour of the text for an assertion log message.")]
		public Color assertMessage = Color.black;

		[Tooltip("The colour of the text for a warning log message.")]
		public Color warningMessage = Color.yellow;

		[Tooltip("The colour of the text for an error log message.")]
		public Color errorMessage = Color.red;

		[Tooltip("The colour of the text for an exception log message.")]
		public Color exceptionMessage = Color.red;

		private Dictionary<LogType, Color> logTypeColors;

		private ScrollRect scrollWindow;

		private RectTransform consoleRect;

		private Text consoleOutput;

		private const string NEWLINE = "\n";

		private int lineBuffer = 50;

		private int currentBuffer;

		private string lastMessage;

		private bool collapseLog;

		public void SetCollapse(bool state)
		{
			collapseLog = state;
		}

		public void ClearLog()
		{
			consoleOutput.text = string.Empty;
			currentBuffer = 0;
			lastMessage = string.Empty;
		}

		protected virtual void Awake()
		{
			logTypeColors = new Dictionary<LogType, Color>
			{
				{
					LogType.Assert,
					assertMessage
				},
				{
					LogType.Error,
					errorMessage
				},
				{
					LogType.Exception,
					exceptionMessage
				},
				{
					LogType.Log,
					infoMessage
				},
				{
					LogType.Warning,
					warningMessage
				}
			};
			scrollWindow = base.transform.Find("Panel/Scroll View").GetComponent<ScrollRect>();
			consoleRect = base.transform.Find("Panel/Scroll View/Viewport/Content").GetComponent<RectTransform>();
			consoleOutput = base.transform.Find("Panel/Scroll View/Viewport/Content/ConsoleOutput").GetComponent<Text>();
			consoleOutput.fontSize = fontSize;
			ClearLog();
		}

		protected virtual void OnEnable()
		{
			Application.logMessageReceived += HandleLog;
		}

		protected virtual void OnDisable()
		{
			Application.logMessageReceived -= HandleLog;
			consoleRect.sizeDelta = Vector2.zero;
		}

		private string GetMessage(string message, LogType type)
		{
			string text = ColorUtility.ToHtmlStringRGBA(logTypeColors[type]);
			return "<color=#" + text + ">" + message + "</color>\n";
		}

		private void HandleLog(string message, string stackTrace, LogType type)
		{
			string message2 = GetMessage(message, type);
			if (!collapseLog || lastMessage != message2)
			{
				consoleOutput.text += message2;
				lastMessage = message2;
			}
			consoleRect.sizeDelta = new Vector2(consoleOutput.preferredWidth, consoleOutput.preferredHeight);
			scrollWindow.verticalNormalizedPosition = 0f;
			currentBuffer++;
			if (currentBuffer >= lineBuffer)
			{
				IEnumerable<string> source = Regex.Split(consoleOutput.text, "\n").Skip(lineBuffer / 2);
				consoleOutput.text = string.Join("\n", source.ToArray());
				currentBuffer = lineBuffer / 2;
			}
		}
	}
}
