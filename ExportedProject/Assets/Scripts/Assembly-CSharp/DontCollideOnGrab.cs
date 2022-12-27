using UnityEngine;
using VRTK;

public class DontCollideOnGrab : MonoBehaviour
{
	private VRTK_InteractableObject interact;

	internal int storedLayer;

	private void Start()
	{
		storedLayer = base.gameObject.layer;
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		base.gameObject.layer = 10;
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		base.gameObject.layer = storedLayer;
	}
}
