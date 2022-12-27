using System.Collections;
using UnityEngine;
using VRTK;

public class RareContainer : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	private VRTK_ControllerEvents holdControl;

	internal float Vibration = 0.062f;

	public float healthValue = 100f;

	internal float startValue;

	internal bool locked;

	internal bool broken;

	public float drainSpeed = 10f;

	internal static int invetoryCount;

	public Transform healthVisual;

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
			startValue = healthValue;
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
		Invoke("SnapCooldown", 0.5f);
	}

	private void DoObjectTouch(object sender, InteractableObjectEventArgs e)
	{
	}

	public IEnumerator Drain()
	{
		float refreshRate = 0.05f;
		SuitManager.instance.canClick.PlayOneShot(SuitManager.instance.canClick.clip);
		while (healthValue > 0f)
		{
			if (SuitManager.instance.radiationResistance < SuitManager.instance.maxRadiationResistance)
			{
				RadiationUI.instance.UpdateState(true);
				healthValue -= refreshRate * drainSpeed;
				SuitManager.instance.radiationResistance += refreshRate * drainSpeed;
				float z = healthValue / startValue;
				healthVisual.localScale = new Vector3(1f, 1f, z);
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration);
			}
			else
			{
				RadiationUI.instance.UpdateState(false);
			}
			yield return new WaitForSeconds(refreshRate);
		}
		RadiationUI.instance.UpdateState(false);
		if (healthValue <= 0f && IntroManager.instance == null)
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
