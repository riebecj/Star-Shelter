using System.Collections;
using UnityEngine;
using VRTK;

public class LootCrate : MonoBehaviour
{
	private Transform key;

	public Transform keySlot;

	public Material unlockedMaterial;

	public MeshRenderer crate;

	private LootSpawner[] lootSpawners;

	public AudioClip unlockAudio;

	private AudioSource audioSource;

	private void Start()
	{
		if (lootSpawners == null)
		{
			lootSpawners = base.transform.parent.GetComponentsInChildren<LootSpawner>();
			LootSpawner[] array = lootSpawners;
			foreach (LootSpawner lootSpawner in array)
			{
				lootSpawner.CancelInvoke("OnSpawn");
			}
		}
		audioSource = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<CrateKey>() && key == null && (other.GetComponent<VRTK_InteractableObject>().IsGrabbed() || DroneHelper.instance.grabbedObject == other.GetComponent<Rigidbody>()))
		{
			key = other.transform;
			key.GetComponent<Collider>().enabled = false;
			key.GetComponent<VRTK_InteractableObject>().previousKinematicState = true;
			key.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			key.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			StartCoroutine("LerpKey");
			key.GetComponentInChildren<UIBillboard>().gameObject.SetActive(false);
		}
	}

	private void OnUnlock()
	{
		key.SetParent(keySlot, true);
		key.localEulerAngles = Vector3.zero;
		Invoke("StartLerp", 0.7f);
		LootSpawner[] array = lootSpawners;
		foreach (LootSpawner lootSpawner in array)
		{
			lootSpawner.OnSpawn();
		}
		Invoke("LiftChildren", 1.2f);
		base.transform.parent.gameObject.layer = 10;
		audioSource.PlayOneShot(unlockAudio);
	}

	private void StartLerp()
	{
		StartCoroutine("LerpOpen");
	}

	private IEnumerator LerpOpen()
	{
		while (base.transform.localEulerAngles.x < 180f || base.transform.localEulerAngles.x > 275f)
		{
			base.transform.RotateAround(base.transform.position, -base.transform.right, 2f);
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator LerpKey()
	{
		while ((key.position - keySlot.position).magnitude > 0.01f)
		{
			key.GetComponent<Collider>().isTrigger = true;
			key.GetComponent<Rigidbody>().isKinematic = true;
			key.position = Vector3.Lerp(key.position, keySlot.position, 15f * Time.deltaTime);
			key.rotation = Quaternion.RotateTowards(key.rotation, keySlot.rotation, 250f * Time.deltaTime);
			yield return new WaitForSeconds(0.02f);
		}
		OnUnlock();
	}

	private void LiftChildren()
	{
		Rigidbody[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f);
		}
	}

	private void OnDisable()
	{
		if (base.transform.parent.gameObject.activeSelf)
		{
			if (lootSpawners != null)
			{
				LootSpawner[] array = lootSpawners;
				foreach (LootSpawner lootSpawner in array)
				{
					lootSpawner.OnSpawn();
				}
			}
			else
			{
				lootSpawners = base.transform.parent.GetComponentsInChildren<LootSpawner>();
				LootSpawner[] array2 = lootSpawners;
				foreach (LootSpawner lootSpawner2 in array2)
				{
					lootSpawner2.OnSpawn();
				}
			}
			LiftChildren();
		}
		base.transform.parent.gameObject.layer = 10;
	}
}
