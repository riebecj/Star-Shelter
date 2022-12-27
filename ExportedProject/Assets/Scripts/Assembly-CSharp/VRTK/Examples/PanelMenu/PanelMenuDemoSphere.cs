using UnityEngine;

namespace VRTK.Examples.PanelMenu
{
	public class PanelMenuDemoSphere : MonoBehaviour
	{
		private readonly Color[] colors = new Color[10]
		{
			Color.black,
			Color.blue,
			Color.cyan,
			Color.gray,
			Color.green,
			Color.magenta,
			Color.red,
			Color.white,
			Color.yellow,
			Color.black
		};

		public void UpdateSliderValue(float value)
		{
			GetComponent<MeshRenderer>().materials[0].color = colors[(int)(value - 1f)];
		}
	}
}
