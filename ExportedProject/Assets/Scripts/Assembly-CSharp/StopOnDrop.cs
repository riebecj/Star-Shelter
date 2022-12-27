using UnityEngine;
using VRTK;

public class StopOnDrop : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	private Rigidbody rigidbody;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectUngrabbed += DoObjectDrop;
		rigidbody = GetComponent<Rigidbody>();
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
	}
}
