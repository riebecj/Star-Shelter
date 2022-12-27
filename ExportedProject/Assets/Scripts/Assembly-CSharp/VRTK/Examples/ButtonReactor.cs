using UnityEngine;
using VRTK.UnityEventHelper;

namespace VRTK.Examples
{
	public class ButtonReactor : MonoBehaviour
	{
		public GameObject go;

		public Transform dispenseLocation;

		private VRTK_Button_UnityEvents buttonEvents;

		private void Start()
		{
			buttonEvents = GetComponent<VRTK_Button_UnityEvents>();
			if (buttonEvents == null)
			{
				buttonEvents = base.gameObject.AddComponent<VRTK_Button_UnityEvents>();
			}
			buttonEvents.OnPushed.AddListener(handlePush);
		}

		private void handlePush(object sender, Control3DEventArgs e)
		{
			VRTK_Logger.Info("Pushed");
			GameObject obj = Object.Instantiate(go, dispenseLocation.position, Quaternion.identity);
			Object.Destroy(obj, 10f);
		}
	}
}
