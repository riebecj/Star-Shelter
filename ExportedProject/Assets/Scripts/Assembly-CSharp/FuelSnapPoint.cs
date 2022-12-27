using UnityEngine;
using VRTK;

public class FuelSnapPoint : MonoBehaviour
{
	internal Transform head;

	public Vector3 offset;

	internal Transform target;

	public static FuelSnapPoint instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		head = GameManager.instance.Head;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(target != null) && (((bool)other.GetComponent<PowerContainer>() && !other.GetComponent<PowerContainer>().IsInvoking("SnapCooldown")) || ((bool)other.GetComponent<OxygenContainer>() && !other.GetComponent<OxygenContainer>().IsInvoking("SnapCooldown")) || ((bool)other.GetComponent<HealthContainer>() && !other.GetComponent<HealthContainer>().IsInvoking("SnapCooldown")) || ((bool)other.GetComponent<RareContainer>() && !other.GetComponent<RareContainer>().IsInvoking("SnapCooldown"))))
		{
			other.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			other.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			other.isTrigger = true;
			target = other.transform;
			Invoke("Snap", 0.1f);
			if ((bool)IntroOxygenEvent.instance)
			{
				IntroOxygenEvent.instance.OxygenEvent();
				return;
			}
			TutorialManager.instance.OnCannisterComplete();
			ObjectiveOxygenCanister.instance.OnCannisterComplete();
		}
	}

	private void Snap()
	{
		target.SetParent(base.transform);
		if (target.parent != null)
		{
			target.localPosition = new Vector3(0f, 0f, -0.01f);
			target.localEulerAngles = new Vector3(180f, 0f, 0f);
			target.GetComponent<VRTK_InteractableObject>().previousParent = base.transform;
			target.GetComponent<VRTK_InteractableObject>().previousKinematicState = true;
			target.GetComponent<Rigidbody>().isKinematic = true;
			if ((bool)target.GetComponent<PowerContainer>())
			{
				target.GetComponent<PowerContainer>().StartCoroutine("Drain");
				target.GetComponent<PowerContainer>().locked = true;
			}
			else if ((bool)target.GetComponent<OxygenContainer>())
			{
				target.GetComponent<OxygenContainer>().StartCoroutine("Drain");
				target.GetComponent<OxygenContainer>().locked = true;
			}
			else if ((bool)target.GetComponent<HealthContainer>())
			{
				target.GetComponent<HealthContainer>().StartCoroutine("Drain");
				target.GetComponent<HealthContainer>().locked = true;
			}
			else if ((bool)target.GetComponent<RareContainer>())
			{
				target.GetComponent<RareContainer>().StartCoroutine("Drain");
				target.GetComponent<RareContainer>().locked = true;
			}
		}
		target.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
		target.gameObject.layer = 21;
	}

	private void Update()
	{
		base.transform.position = head.position + base.transform.right * offset.x + base.transform.up * offset.y + base.transform.forward * offset.z;
		base.transform.rotation = Quaternion.Euler(0f, head.transform.rotation.eulerAngles.y, 0f);
	}
}
