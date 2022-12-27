using System.Collections;
using UnityEngine;
using VRTK;

public class OxygenContainer : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	private VRTK_ControllerEvents holdControl;

	internal float Vibration = 0.062f;

	public float oxygenValue = 100f;

	internal float startValue;

	internal bool locked;

	internal bool broken;

	public float drainSpeed = 10f;

	internal static int invetoryCount;

	public Transform oxygenVisual;

	internal FuelStation fuelStation;

	public Mesh brokenMesh;

	public AudioSource breakAudio;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectTouched += DoObjectTouch;
		if (startValue == 0f)
		{
			startValue = oxygenValue;
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
			SuitManager.instance.oxygenGain.gameObject.SetActive(false);
		}
		if ((bool)IntroManager.instance)
		{
			IntroManager.instance.ToggleCannister();
		}
		else
		{
			TutorialManager.instance.ToggleCannister();
		}
		GetComponent<Collider>().isTrigger = false;
		StopCoroutine("Drain");
		StopCoroutine("DrainToBase");
		Invoke("SnapCooldown", 0.5f);
	}

	private void DoObjectTouch(object sender, InteractableObjectEventArgs e)
	{
	}

	public IEnumerator Drain()
	{
		float refreshRate = 0.05f;
		SuitManager.instance.canClick.PlayOneShot(SuitManager.instance.canClick.clip);
		while (oxygenValue > 0f)
		{
			if (SuitManager.instance.oxygen < SuitManager.instance.maxOxygen)
			{
				if (!SuitManager.instance.oxygenGain.gameObject.activeSelf)
				{
					SuitManager.instance.oxygenGain.gameObject.SetActive(true);
				}
				oxygenValue -= refreshRate * drainSpeed;
				SuitManager.instance.oxygen += refreshRate * drainSpeed;
				float z = oxygenValue / startValue;
				oxygenVisual.localScale = new Vector3(1f, 1f, z);
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration);
			}
			else if (SuitManager.instance.oxygenGain.gameObject.activeSelf)
			{
				SuitManager.instance.oxygenGain.gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(refreshRate);
		}
		SuitManager.instance.oxygenGain.gameObject.SetActive(false);
		if (oxygenValue <= 0f && IntroManager.instance == null)
		{
			AddComponent();
			OnBreak();
		}
	}

	public IEnumerator DrainToBase()
	{
		float refreshRate = 0.05f;
		OxygenGroup oxygenGroup = BaseManager.instance.GetComponent<Room>().group;
		while (oxygenValue > 0f)
		{
			if (oxygenGroup.TotalOxygen < oxygenGroup.MaxOxygen)
			{
				oxygenValue -= refreshRate * drainSpeed;
				oxygenGroup.TotalOxygen += refreshRate * drainSpeed;
				float z = oxygenValue / startValue;
				oxygenVisual.localScale = new Vector3(1f, 1f, z);
			}
			yield return new WaitForSeconds(refreshRate);
		}
		if (oxygenValue <= 0f)
		{
			AddComponent();
			OnBreak();
		}
		yield return new WaitForSeconds(1f);
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
