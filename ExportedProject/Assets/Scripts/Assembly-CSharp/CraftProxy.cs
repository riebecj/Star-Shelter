using UnityEngine;
using VRTK;

public class CraftProxy : MonoBehaviour
{
	public CraftedObject craftedObject;

	public CraftMaterial craftMaterial;

	private VRTK_InteractableObject interact;

	internal CraftBound craftBound;

	internal CraftStation craftStation;

	internal bool inBound;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
		base.transform.position = base.transform.parent.position;
		if (!ArmUIManager.instance.gripSwap)
		{
			GetComponent<VRTK_InteractableObject>().useOverrideButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
		}
		else
		{
			GetComponent<VRTK_InteractableObject>().useOverrideButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
		}
	}

	public void OnSetup(CraftedObject _craftObject, CraftMaterial _craftMaterial, CraftStation station)
	{
		craftStation = station;
		if (_craftObject != null)
		{
			GetComponent<MeshFilter>().sharedMesh = _craftObject.icon;
			craftedObject = _craftObject;
		}
		else
		{
			GetComponent<MeshFilter>().sharedMesh = _craftMaterial.prefab.GetComponent<MeshFilter>().sharedMesh;
			craftMaterial = _craftMaterial;
		}
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		OnGrab();
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		OnDrop();
	}

	public void OnGrab()
	{
		if (craftStation.craftQueue.Contains(this))
		{
			if (craftStation.craftQueue.Count > 0 && craftStation.craftQueue[0] == this)
			{
				craftStation.CancelCrafting();
			}
			craftStation.craftQueue.Remove(this);
		}
		if (craftStation.oldProxy == this)
		{
			craftStation.oldProxy = null;
		}
	}

	public void OnDrop()
	{
		if ((bool)craftBound && craftBound.GetComponent<Collider>().bounds.Contains(base.transform.position))
		{
			craftStation.AddToQueue(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		if (craftStation.oldProxy == null)
		{
			if ((bool)craftedObject)
			{
				craftStation.SpawnProxy(craftedObject);
			}
			else
			{
				craftStation.SpawnProxy(craftMaterial);
			}
		}
	}

	private void OnDisable()
	{
		craftStation.activeProxies.Remove(base.gameObject);
	}
}
