using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class HorizontalGroupAttributeExamples : MonoBehaviour
	{
		[HorizontalGroup(0f, 0, 0, 0)]
		public int A;

		[HideLabel]
		[LabelWidth(150f)]
		[HorizontalGroup(150f, 0, 0, 0)]
		public LayerMask B;

		[HorizontalGroup("Group 1", 0f, 0, 0, 0, LabelWidth = 20f)]
		public int C;

		[HorizontalGroup("Group 1", 0f, 0, 0, 0)]
		public int D;

		[HorizontalGroup("Group 1", 0f, 0, 0, 0)]
		public int E;

		[HorizontalGroup("Split", 0.5f, 0, 0, 0, LabelWidth = 20f)]
		[BoxGroup("Split/Left", true, false, 0)]
		public int L;

		[BoxGroup("Split/Right", true, false, 0)]
		public int M;

		[BoxGroup("Split/Left", true, false, 0)]
		public int N;

		[BoxGroup("Split/Right", true, false, 0)]
		public int O;

		[HorizontalGroup("MyButton", 0f, 0, 0, 0, MarginLeft = 0.25f, MarginRight = 0.25f)]
		public void SomeButton()
		{
		}
	}
}
