using UnityEngine;

namespace VRTK.Examples
{
	public class Menu_Object_Spawner : VRTK_InteractableObject
	{
		public enum PrimitiveTypes
		{
			Cube = 0,
			Sphere = 1
		}

		public PrimitiveTypes shape;

		private Color selectedColor;

		public void SetSelectedColor(Color color)
		{
			selectedColor = color;
			base.gameObject.GetComponent<MeshRenderer>().material.color = color;
		}

		public override void StartUsing(GameObject usingObject)
		{
			base.StartUsing(usingObject);
			if (shape == PrimitiveTypes.Cube)
			{
				CreateShape(PrimitiveType.Cube, selectedColor);
			}
			else if (shape == PrimitiveTypes.Sphere)
			{
				CreateShape(PrimitiveType.Sphere, selectedColor);
			}
			ResetMenuItems();
		}

		private void CreateShape(PrimitiveType shape, Color color)
		{
			GameObject gameObject = GameObject.CreatePrimitive(shape);
			gameObject.transform.position = base.transform.position;
			gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			gameObject.GetComponent<MeshRenderer>().material.color = color;
			gameObject.AddComponent<Rigidbody>();
		}

		private void ResetMenuItems()
		{
			Menu_Object_Spawner[] array = Object.FindObjectsOfType<Menu_Object_Spawner>();
			foreach (Menu_Object_Spawner menu_Object_Spawner in array)
			{
				menu_Object_Spawner.StopUsing(null);
			}
		}
	}
}
