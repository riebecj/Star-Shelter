using UnityEngine;

namespace VRTK.Examples
{
	public class Sphere_Spawner : MonoBehaviour
	{
		private GameObject spawnMe;

		private Vector3 position;

		private void Start()
		{
			if (GetComponent<VRTK_ControllerEvents>() == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "Sphere_Spawner", "VRTK_ControllerEvents", "the same"));
			}
			else
			{
				GetComponent<VRTK_ControllerEvents>().TriggerPressed += DoTriggerPressed;
				GetComponent<VRTK_ControllerEvents>().TouchpadPressed += DoTouchpadPressed;
				spawnMe = GameObject.Find("SpawnMe");
				position = spawnMe.transform.position;
			}
		}

		private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
		{
			Invoke("CreateSphere", 0f);
		}

		private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
		{
			for (int i = 0; i < 20; i++)
			{
				Invoke("CreateSphere", 0f);
			}
		}

		private void CreateSphere()
		{
			Object.Instantiate(spawnMe, position, Quaternion.identity);
		}
	}
}
