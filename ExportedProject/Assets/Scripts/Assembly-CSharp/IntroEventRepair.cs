using System.Collections;
using UnityEngine;
using VRTK;

public class IntroEventRepair : MonoBehaviour
{
	public static IntroEventRepair instance;

	public GameObject leakPrompt;

	public DoorSensor doorSensor;

	public SkinnedMeshRenderer tileVisual;

	public GameObject[] objects;

	internal bool active;

	internal Collider triggerCol;

	private bool isRepairing;

	private float value = 100f;

	private void Awake()
	{
		instance = this;
		doorSensor.SetLock(false);
		value = 100f;
	}

	public void TogglePointer()
	{
		if (!(GameManager.instance == null))
		{
			GameManager.instance.leftController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
			GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
			GameManager.instance.leftController.GetComponent<VRTK_Pointer>().Toggle(true);
			GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(true);
			GameManager.instance.leftController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
			GameManager.instance.rightController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
		}
	}

	public void OnRepair()
	{
		if (active && !isRepairing)
		{
			StartCoroutine("RepairHole");
		}
		else if (isRepairing)
		{
			value -= Time.deltaTime * 30f;
		}
	}

	private IEnumerator RepairHole()
	{
		isRepairing = true;
		float interval = 0.75f;
		leakPrompt.GetComponentInChildren<LeakPrompt>().repairObject.SetActive(true);
		leakPrompt.GetComponentInChildren<LeakPrompt>().DisableInfo();
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().Toggle(false);
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(false);
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
		while (value > 0f && active && isRepairing)
		{
			leakPrompt.GetComponentInChildren<LeakPrompt>().repairBar.fillAmount = 1f - value / 100f;
			tileVisual.SetBlendShapeWeight(0, value);
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(triggerCol.GetComponentInParent<HandController>().gameObject), 0.1f);
			yield return new WaitForSeconds(0.01f);
		}
		IntroManager.instance.RepairComplete();
		GameObject[] array = objects;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		doorSensor.SetLock(true);
	}

	public void OnCancelRepair()
	{
		leakPrompt.SetActive(true);
		isRepairing = false;
		CraftingManager.instance.isRepairing = false;
		if ((bool)leakPrompt.GetComponentInChildren<MeshRenderer>())
		{
			leakPrompt.GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", Color.red);
		}
		StopCoroutine("RepairHole");
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<SalvageCone>())
		{
			triggerCol = other;
			active = true;
			leakPrompt.GetComponentInChildren<LeakPrompt>().RepairCheck(true);
			other.GetComponentInParent<HandController>().ToggleRepairCone();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<SalvageCone>() && other == triggerCol)
		{
			active = false;
			StopAllCoroutines();
			isRepairing = false;
			OnCancelRepair();
			leakPrompt.GetComponentInChildren<LeakPrompt>().RepairCheck(false);
			other.GetComponentInParent<HandController>().ToggleSalvageCone();
		}
	}
}
