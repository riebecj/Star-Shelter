using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class VerticalGroupExamples : MonoBehaviour
	{
		[HorizontalGroup("Split", 0f, 0, 0, 0)]
		[VerticalGroup("Split/Left", 0)]
		public InfoMessageType First;

		[VerticalGroup("Split/Left", 0)]
		public InfoMessageType Second;

		[HideLabel]
		[VerticalGroup("Split/Right", 0)]
		public int A;

		[HideLabel]
		[VerticalGroup("Split/Right", 0)]
		public int B;
	}
}
