using System.Collections;
using UnityEngine;
using VRTK;

public class PowerContainer : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	private VRTK_ControllerEvents holdControl;

	internal float Vibration = 0.062f;

	public float powerValue = 50f;

	public float drainSpeed = 10f;

	internal float startValue;

	internal bool locked;

	internal bool broken;

	internal static int invetoryCount;

	public Transform powerVisual;

	internal FuelStation fuelStation;

	public Mesh brokenMesh;

	public AudioSource breakAudio;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		if (startValue == 0f)
		{
			startValue = powerValue;
		}
	}

	public bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
		{
			holdControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>();
		}
		else
		{
			holdControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
		}
		interact.previousParent = null;
		interact.previousKinematicState = false;
		if (locked)
		{
			if (fuelStation == null)
			{
				FuelSnapPoint.instance.target = null;
			}
			else
			{
				fuelStation.StopAllCoroutines();
				fuelStation.target = null;
				fuelStation = null;
			}
			locked = false;
			SuitManager.instance.powerGain.gameObject.SetActive(false);
		}
		TutorialManager.instance.ToggleCannister();
		GetComponent<Collider>().isTrigger = false;
		StopCoroutine("Drain");
		StopCoroutine("DrainToBase");
		StopCoroutine("DrainToDrone");
		Invoke("SnapCooldown", 0.5f);
	}

	public IEnumerator Drain()
	{
		float refreshRate = 0.05f;
		SuitManager.instance.canClick.PlayOneShot(SuitManager.instance.canClick.clip);
		while (powerValue > 0f)
		{
			if (SuitManager.instance.power < SuitManager.instance.maxPower)
			{
				if (!SuitManager.instance.powerGain.gameObject.activeSelf)
				{
					SuitManager.instance.powerGain.gameObject.SetActive(true);
				}
				powerValue -= refreshRate * drainSpeed;
				SuitManager.instance.power += refreshRate * drainSpeed;
				float z = powerValue / startValue;
				powerVisual.localScale = new Vector3(1f, 1f, z);
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration);
			}
			else if (SuitManager.instance.powerGain.gameObject.activeSelf)
			{
				SuitManager.instance.powerGain.gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(refreshRate);
		}
		SuitManager.instance.powerGain.gameObject.SetActive(false);
		if (powerValue <= 0f)
		{
			AddComponent();
			OnBreak();
		}
	}

	public IEnumerator DrainToDrone()
	{
		float refreshRate = 0.05f;
		while (powerValue > 0f)
		{
			if (DroneHelper.instance.power < DroneHelper.instance.maxPower)
			{
				powerValue -= refreshRate * drainSpeed;
				DroneHelper.instance.power += refreshRate * drainSpeed;
				float z = powerValue / startValue;
				powerVisual.localScale = new Vector3(1f, 1f, z);
			}
			yield return new WaitForSeconds(refreshRate);
		}
		if (powerValue <= 0f)
		{
			AddComponent();
			OnBreak();
		}
	}

	public IEnumerator DrainToBase()
	{
		float refreshRate = 0.05f;
		while (powerValue > 0f)
		{
			if (BaseManager.instance.power < BaseManager.instance.maxPower)
			{
				powerValue -= refreshRate * drainSpeed;
				BaseManager.instance.power += refreshRate * drainSpeed;
				float z = powerValue / startValue;
				powerVisual.localScale = new Vector3(1f, 1f, z);
			}
			yield return new WaitForSeconds(refreshRate);
		}
		if (powerValue <= 0f)
		{
			AddComponent();
			OnBreak();
		}
	}

	public void OnBreak()
	{
		GetComponent<MeshFilter>().mesh = brokenMesh;
		broken = true;
		if (fuelStation != null)
		{
			fuelStation.target = null;
			fuelStation = null;
			locked = false;
			GetComponent<Collider>().isTrigger = false;
			GetComponent<Rigidbody>().isKinematic = false;
			GetComponent<Rigidbody>().AddForce(Vector3.down * 5f);
		}
		breakAudio.gameObject.SetActive(true);
		Invoke("DisableBreakAudio", 3f);
	}

	private void DisableBreakAudio()
	{
		breakAudio.gameObject.SetActive(false);
	}

	private void AddComponent()
	{
		CraftComponent craftComponent = base.gameObject.AddComponent<CraftComponent>();
		craftComponent.craftMaterials.Add(NanoInventory.instance.craftMaterials[1]);
		craftComponent.materialCounts[0] = (int)Mathf.Clamp(startValue / 25f, 1f, 4f);
	}

	public void SnapCooldown()
	{
	}

	public void OnAddedToInventory()
	{
		invetoryCount++;
	}

	public void OnRemoveFromInventory()
	{
		invetoryCount--;
	}
}
