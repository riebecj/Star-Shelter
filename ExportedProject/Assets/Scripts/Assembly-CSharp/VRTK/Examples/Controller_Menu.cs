using UnityEngine;

namespace VRTK.Examples
{
	public class Controller_Menu : MonoBehaviour
	{
		public GameObject menuObject;

		private GameObject clonedMenuObject;

		private bool menuInit;

		private bool menuActive;

		private void Start()
		{
			GetComponent<VRTK_ControllerEvents>().AliasMenuOn += DoMenuOn;
			GetComponent<VRTK_ControllerEvents>().AliasMenuOff += DoMenuOff;
			menuInit = false;
			menuActive = false;
		}

		private void InitMenu()
		{
			clonedMenuObject = Object.Instantiate(menuObject, base.transform.position, Quaternion.identity);
			clonedMenuObject.SetActive(true);
			menuInit = true;
		}

		private void DoMenuOn(object sender, ControllerInteractionEventArgs e)
		{
			if (!menuInit)
			{
				InitMenu();
			}
			clonedMenuObject.SetActive(true);
			menuActive = true;
		}

		private void DoMenuOff(object sender, ControllerInteractionEventArgs e)
		{
			clonedMenuObject.SetActive(false);
			menuActive = false;
		}

		private void Update()
		{
			if (menuActive)
			{
				clonedMenuObject.transform.rotation = base.transform.rotation;
				clonedMenuObject.transform.position = base.transform.position;
			}
		}
	}
}
