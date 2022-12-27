using UnityEngine;

namespace VRTK.Examples.PanelMenu
{
	public class PanelMenuDemoFlyingSaucer : MonoBehaviour
	{
		private readonly Color[] colors = new Color[8]
		{
			Color.black,
			Color.blue,
			Color.cyan,
			Color.gray,
			Color.green,
			Color.magenta,
			Color.red,
			Color.white
		};

		public void UpdateGridLayoutValue(int selectedIndex)
		{
			base.transform.GetChild(1).GetComponent<MeshRenderer>().materials[0].color = colors[selectedIndex];
		}
	}
}
