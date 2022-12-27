using UnityEngine;
using VRTK;

public class IntroOxygenEvent : MonoBehaviour
{
	public static IntroOxygenEvent instance;

	public GameObject prompt;

	public DoorSensor doorSensor;

	private void Awake()
	{
		instance = this;
		doorSensor.SetLock(false);
	}

	private void Start()
	{
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().Toggle(false);
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(false);
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
	}

	public void OxygenEvent()
	{
		doorSensor.SetLock(true);
		prompt.SetActive(false);
		IntroManager.instance.CannisterComplete();
	}
}
