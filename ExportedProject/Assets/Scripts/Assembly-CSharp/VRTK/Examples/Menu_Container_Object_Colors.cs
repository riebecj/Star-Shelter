using UnityEngine;

namespace VRTK.Examples
{
	public class Menu_Container_Object_Colors : VRTK_InteractableObject
	{
		public void SetSelectedColor(Color color)
		{
			Menu_Object_Spawner[] componentsInChildren = base.gameObject.GetComponentsInChildren<Menu_Object_Spawner>();
			foreach (Menu_Object_Spawner menu_Object_Spawner in componentsInChildren)
			{
				menu_Object_Spawner.SetSelectedColor(color);
			}
		}

		protected void Start()
		{
			SetSelectedColor(Color.red);
			SaveCurrentState();
		}
	}
}
