using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class AddToInventory : MonoBehaviour
{
	private VRTK_InteractableObject interact;

	internal VRTK_ControllerEvents holdControl;

	private SteamVR_ControllerManager controllers;

	private Vector3 startPos;

	public float InventoryScale = 1f;

	internal int parentIndex;

	public static List<AddToInventory> inventoryObjects = new List<AddToInventory>();

	private void Start()
	{
		startPos = base.transform.position;
		controllers = Object.FindObjectOfType<SteamVR_ControllerManager>();
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
		inventoryObjects.Add(this);
	}

	public void OnRelease()
	{
		if (!(holdControl != null))
		{
		}
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		StopCoroutine("UpdateLayer");
		if (!Inventory.inventory.Contains(base.gameObject))
		{
			base.gameObject.layer = LayerMask.NameToLayer("NoPlayer");
			Invoke("Restore", 0.1f);
		}
		else
		{
			base.gameObject.layer = 21;
		}
	}

	private void Restore()
	{
		if (!Inventory.inventory.Contains(base.gameObject))
		{
			base.gameObject.layer = LayerMask.NameToLayer("NoPlayer");
		}
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().velocity = GameManager.instance.CamRig.GetComponent<Rigidbody>().velocity;
		base.gameObject.layer = LayerMask.NameToLayer("NoPlayer");
		if (!Inventory.inventory.Contains(base.gameObject))
		{
			base.transform.SetParent(null);
			base.transform.localScale = Vector3.one;
			TutorialManager.instance.TogglePhysicalInventory();
		}
		else
		{
			Inventory.inventory.Remove(base.gameObject);
			GetComponent<VRTK_InteractableObject>().previousParent = null;
			GetComponent<VRTK_InteractableObject>().previousKinematicState = false;
			base.transform.SetParent(null);
			base.gameObject.SendMessage("OnRemoveFromInventory", SendMessageOptions.DontRequireReceiver);
			GetComponent<Rigidbody>().velocity = GameManager.instance.CamRig.GetComponent<Rigidbody>().velocity;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		for (int i = 0; i < StorageBox.storageBoxes.Count; i++)
		{
			if (StorageBox.storageBoxes[i].inventory.Contains(base.gameObject))
			{
				StorageBox.storageBoxes[i].inventory.Remove(base.gameObject);
				GetComponent<VRTK_InteractableObject>().previousParent = null;
				GetComponent<VRTK_InteractableObject>().previousKinematicState = false;
				base.transform.SetParent(null);
				base.gameObject.SendMessage("OnRemoveFromInventory", SendMessageOptions.DontRequireReceiver);
			}
		}
		StartCoroutine("UpdateLayer");
	}

	private IEnumerator UpdateLayer()
	{
		while (true)
		{
			if (Inventory.instance.inventoryBubble.activeSelf && Vector3.Distance(base.transform.position, Inventory.instance.inventoryBubble.transform.position) < 0.25f)
			{
				base.gameObject.layer = 21;
			}
			else
			{
				base.gameObject.layer = LayerMask.NameToLayer("NoPlayer");
			}
			yield return new WaitForSeconds(0.03f);
		}
	}

	private void OnDestroy()
	{
		inventoryObjects.Remove(this);
		if (Inventory.inventory.Contains(base.gameObject))
		{
			Inventory.inventory.Remove(base.gameObject);
		}
		for (int i = 0; i < StorageBox.storageBoxes.Count; i++)
		{
			if (StorageBox.storageBoxes[i].inventory.Contains(base.gameObject))
			{
				StorageBox.storageBoxes[i].inventory.Remove(base.gameObject);
			}
		}
	}
}
