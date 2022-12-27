using System.Collections;
using UnityEngine;
using VRTK;

public class HandShield : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	public GameObject shield;

	public int health;

	private int startHealth;

	private Animator anim;

	public GameObject impactParticle;

	internal ShieldSnapPoint snapPoint;

	public LineRenderer helpLine;

	private Transform target;

	private bool updateHelpLine;

	private void Awake()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		anim = GetComponent<Animator>();
	}

	private void Start()
	{
		startHealth = health;
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
	}

	public void OnTakeDamage(int damage, Transform impactTransform)
	{
		health -= damage;
		if (health < 0)
		{
			health = 0;
			OnBreak();
		}
		float num = (float)health / 100f;
		shield.transform.localScale = new Vector3(num, num, num);
		SpawnImpactParticle(impactTransform);
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		interact.previousParent = null;
		interact.previousKinematicState = false;
		interact.allowedGrabControllers = VRTK_InteractableObject.AllowedController.Both;
		if ((bool)snapPoint)
		{
			snapPoint.UpdateState(false);
			anim.SetBool("On", false);
			snapPoint.UI.SetActive(false);
		}
		Invoke("SnapCooldown", 0.5f);
		helpLine.gameObject.SetActive(true);
		if (!VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
		{
			target = VRTK_DeviceFinder.GetControllerLeftHand().GetComponentInChildren<ShieldSnapPoint>().transform;
		}
		else
		{
			target = VRTK_DeviceFinder.GetControllerRightHand().GetComponentInChildren<ShieldSnapPoint>().transform;
		}
		target.GetComponent<ShieldSnapPoint>().holoplace.SetActive(true);
		updateHelpLine = true;
		StartCoroutine("UpdateLine");
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		helpLine.gameObject.SetActive(false);
		updateHelpLine = false;
		if ((bool)target)
		{
			target.GetComponent<ShieldSnapPoint>().holoplace.SetActive(false);
		}
	}

	public void ToggleShield(bool value)
	{
		anim.SetBool("On", value);
		interact.isGrabbable = !value;
	}

	private void OnBreak()
	{
		AddComponent();
		interact.isGrabbable = true;
		snapPoint.UI.SetActive(false);
	}

	private void AddComponent()
	{
		CraftComponent craftComponent = base.gameObject.AddComponent<CraftComponent>();
		craftComponent.craftMaterials.Add(NanoInventory.instance.craftMaterials[10]);
		craftComponent.materialCounts[0] = 2;
	}

	public void SpawnImpactParticle(Transform impactTransform)
	{
		GameObject gameObject = Object.Instantiate(impactParticle, base.transform.position, Quaternion.identity);
		gameObject.transform.position = impactTransform.position;
		gameObject.transform.LookAt(impactTransform.position + (impactTransform.position - base.transform.position));
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<ShieldSnapPoint>() && !IsInvoking("SnapCooldown"))
		{
			interact.ForceStopInteracting();
			interact.isGrabbable = false;
			snapPoint = other.GetComponent<ShieldSnapPoint>();
			snapPoint.shield = this;
			Invoke("OnSnap", 0.1f);
			Invoke("SnapCooldown", 0.5f);
		}
	}

	public void EnableUI()
	{
		interact.isGrabbable = true;
		snapPoint.UI.SetActive(true);
		anim = snapPoint.shield.GetComponent<Animator>();
	}

	private void OnSnap()
	{
		base.transform.SetParent(snapPoint.transform);
		interact.previousParent = base.transform;
		interact.previousKinematicState = true;
		LockHandInteraction();
		GetComponent<Rigidbody>().isKinematic = true;
		snapPoint.UpdateState(true);
		StartCoroutine("LerpToHand");
		Invoke("EnableUI", 1f);
	}

	public void LockHandInteraction()
	{
		if (snapPoint.GetComponentInParent<VRTK_InteractTouch>().transform == GameManager.instance.rightController)
		{
			interact.allowedGrabControllers = VRTK_InteractableObject.AllowedController.LeftOnly;
		}
		else if (snapPoint.GetComponentInParent<VRTK_InteractTouch>().transform == GameManager.instance.leftController)
		{
			interact.allowedGrabControllers = VRTK_InteractableObject.AllowedController.RightOnly;
		}
	}

	private void SnapCooldown()
	{
	}

	private IEnumerator UpdateLine()
	{
		while (updateHelpLine)
		{
			helpLine.SetPosition(0, base.transform.position);
			helpLine.SetPosition(1, target.position);
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator LerpToHand()
	{
		while ((base.transform.position - snapPoint.transform.position).magnitude > 0.01f)
		{
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, Vector3.zero, 30f * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, snapPoint.transform.rotation, 250f * Time.deltaTime);
			yield return new WaitForSeconds(0.02f);
		}
		base.transform.localPosition = Vector3.zero;
		base.transform.rotation = snapPoint.transform.rotation;
	}

	private void OnDisable()
	{
		if ((bool)snapPoint)
		{
			snapPoint.UpdateState(false);
			anim.SetBool("On", false);
			snapPoint.UI.SetActive(false);
			snapPoint.On = false;
		}
	}
}
