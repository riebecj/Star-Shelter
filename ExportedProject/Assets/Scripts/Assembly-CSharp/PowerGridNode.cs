using System.Collections;
using UnityEngine;
using VRTK;

public class PowerGridNode : MonoBehaviour
{
	public PowerGrid grid;

	public int index;

	internal Transform powerCard;

	internal bool full;

	private void OnTriggerEnter(Collider other)
	{
		if (other.name.StartsWith("PowerCard") && !full)
		{
			powerCard = other.transform;
			powerCard.GetComponent<Collider>().isTrigger = true;
			powerCard.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			powerCard.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			full = true;
			StartCoroutine("LerpPowerCard");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (full && other.transform == powerCard)
		{
			full = false;
		}
	}

	private IEnumerator LerpPowerCard()
	{
		float lerpTime = 0.02f;
		while ((powerCard.position - base.transform.position).magnitude > 0.01f)
		{
			powerCard.position = Vector3.Lerp(powerCard.position, base.transform.position, 15f * lerpTime);
			powerCard.rotation = Quaternion.RotateTowards(powerCard.rotation, base.transform.rotation, 400f * lerpTime);
			yield return new WaitForSeconds(lerpTime);
		}
		powerCard.position = base.transform.position;
		powerCard.rotation = base.transform.rotation;
		CircuitClosed();
	}

	private void CircuitClosed()
	{
		grid.circuits[index] = 1;
		grid.CheckCircuit();
	}
}
