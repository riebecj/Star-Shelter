using UnityEngine;

[RequireComponent(typeof(VRButton))]
public class VRGrippedButton : MonoBehaviour
{
	public Animation ButtonAnim;

	private VRButton Button;

	private void OnEnable()
	{
		Button = GetComponent<VRButton>();
		if (Button == null)
		{
			Debug.LogError("VRButton is null");
		}
		Collider component = GetComponent<Collider>();
		component.isTrigger = true;
	}

	private void OnTriggerEnter(Collider _collider)
	{
		if (Button.Interactable)
		{
			ActivateButton(_collider.attachedRigidbody);
		}
	}

	public void ActivateButton(Rigidbody _controllerBody)
	{
		if (_controllerBody == null)
		{
			return;
		}
		SteamVR_TrackedObject component = _controllerBody.gameObject.GetComponent<SteamVR_TrackedObject>();
		if (!(component == null))
		{
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)component.index);
			if (device.GetHairTrigger() && ButtonAnim != null)
			{
				ButtonAnim.Play();
			}
		}
	}
}
