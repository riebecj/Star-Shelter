using UnityEngine;
using VRTK.UnityEventHelper;

namespace VRTK.Examples
{
	public class ControlReactor : MonoBehaviour
	{
		public TextMesh go;

		private VRTK_Control_UnityEvents controlEvents;

		private void Start()
		{
			controlEvents = GetComponent<VRTK_Control_UnityEvents>();
			if (controlEvents == null)
			{
				controlEvents = base.gameObject.AddComponent<VRTK_Control_UnityEvents>();
			}
			controlEvents.OnValueChanged.AddListener(HandleChange);
		}

		private void HandleChange(object sender, Control3DEventArgs e)
		{
			go.text = e.value + "(" + e.normalizedValue + "%)";
		}
	}
}
