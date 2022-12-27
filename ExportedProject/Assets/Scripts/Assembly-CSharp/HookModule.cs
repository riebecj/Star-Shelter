using System.Collections;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class HookModule : MonoBehaviour
{
	internal VRTK_ControllerEvents holdControl;

	public ParticleSystem particleSystem;

	public AudioSource audioSource;

	public GameObject grabProxy;

	public GameObject beam;

	internal bool lockedObject;

	private Rigidbody rigidbody;

	private bool flinging;

	private Vector3 grabPointWorldPosition;

	private Vector3 startPlayAreaWorldOffset;

	private Vector3 grabOffset;

	private Vector3 areaOffset;

	private Vector3 hitOffset;

	public Transform barrel;

	public Transform snapPoint;

	private Transform playArea;

	private Vector3 OldVelocity;

	public LayerMask layerMask;

	private Rigidbody heldBody;

	private Rigidbody wreckage;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
	}

	public void OnSetup(VRTK_ControllerEvents control)
	{
		holdControl = control;
		holdControl.TriggerPressed += OnTriggerPressed;
		holdControl.TriggerReleased += OnTriggerReleased;
		holdControl.TriggerUnclicked += OnTriggerUnclicked;
	}

	private void OnTriggerPressed(object sender, ControllerInteractionEventArgs e)
	{
		if (Gun.instance.shotType == Gun.ShotType.HookShot)
		{
			if (!beam.activeSelf)
			{
				StartCoroutine("FadeInAudio");
				StopCoroutine("FadOutAudio");
			}
			OnShoot();
			Gun.instance.powerRing.transform.parent.gameObject.SetActive(true);
		}
	}

	private void OnLock()
	{
		startPlayAreaWorldOffset = playArea.transform.position - holdControl.transform.position;
		grabPointWorldPosition = holdControl.transform.position;
		flinging = true;
		beam.SetActive(true);
		StartCoroutine("FadeInBeam");
	}

	private void OnPickup(Collider col)
	{
		snapPoint.position = col.bounds.center;
		heldBody = col.GetComponentInParent<Rigidbody>();
		beam.SetActive(true);
		StartCoroutine("FadeInBeam");
	}

	private IEnumerator FadeInBeam()
	{
		float value = 0.1f;
		while (value < 1.2f)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			main.startLifetime = value;
			value += 0.1f;
			yield return new WaitForSeconds(0.02f);
		}
	}

	private void FixedUpdate()
	{
		if ((bool)heldBody)
		{
			grabProxy.transform.position = heldBody.transform.position;
			heldBody.velocity = (snapPoint.transform.position - heldBody.transform.position) * 500f * Time.deltaTime;
			Gun.instance.UpdatePower();
		}
		else if (flinging)
		{
			if ((bool)wreckage)
			{
				grabPointWorldPosition = wreckage.transform.position - grabOffset;
			}
			if ((bool)wreckage)
			{
				grabProxy.transform.position = wreckage.transform.position + hitOffset;
			}
			Vector3 vector = holdControl.transform.position - grabPointWorldPosition;
			Vector3 velocity = (grabPointWorldPosition + startPlayAreaWorldOffset - vector - playArea.position) * 1200f * Time.deltaTime;
			VRTK_BodyPhysics.instance.ApplyBodyVelocity(velocity, true, true);
			Gun.instance.UpdatePower();
		}
		else if (beam.activeSelf)
		{
			grabProxy.transform.position = barrel.position + barrel.transform.forward * 5f;
			OnShoot();
		}
	}

	private void OnShoot()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(barrel.position, barrel.forward), out hitInfo, 20f, layerMask))
		{
			Rigidbody componentInParent = hitInfo.collider.GetComponentInParent<Rigidbody>();
			Transform transform = hitInfo.collider.transform;
			if ((bool)componentInParent)
			{
				if (componentInParent.isKinematic || (bool)componentInParent.GetComponent<Wreckage>())
				{
					if ((bool)componentInParent.GetComponent<Wreckage>())
					{
						wreckage = componentInParent;
						grabOffset = componentInParent.position - holdControl.transform.position;
						hitOffset = hitInfo.point - componentInParent.position;
						areaOffset = componentInParent.position - playArea.position;
					}
					grabProxy.SetActive(true);
					grabProxy.transform.SetParent(null);
					grabProxy.transform.position = hitInfo.point;
					OnLock();
				}
				else
				{
					OnPickup(hitInfo.collider);
				}
			}
			else if ((bool)transform.GetComponent<VRTK_ClimbableGrabAttach>())
			{
				grabProxy.SetActive(true);
				grabProxy.transform.SetParent(null);
				grabProxy.transform.position = hitInfo.point;
				OnLock();
			}
		}
		else
		{
			beam.SetActive(true);
			grabProxy.SetActive(true);
			grabProxy.transform.SetParent(null);
			StartCoroutine("FadeInBeam");
		}
	}

	private void OnTriggerReleased(object sender, ControllerInteractionEventArgs e)
	{
		OnRelease();
	}

	private void OnTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
	{
		OnRelease();
	}

	private void OnRelease()
	{
		flinging = false;
		beam.SetActive(false);
		heldBody = null;
		wreckage = null;
		grabProxy.SetActive(false);
		Gun.instance.powerRing.transform.parent.gameObject.SetActive(false);
		StopCoroutine("FadeInBeam");
		StartCoroutine("FadeOutAudio");
		StopCoroutine("FadeInAudio");
	}

	private IEnumerator FadeOutAudio()
	{
		float waitTime = 0.02f;
		while (audioSource.volume > 0.01f)
		{
			audioSource.volume -= waitTime;
			yield return new WaitForSeconds(waitTime);
		}
		audioSource.Stop();
	}

	private IEnumerator FadeInAudio()
	{
		float waitTime = 0.02f;
		audioSource.volume = 0f;
		audioSource.Play();
		while (audioSource.volume < 0.5f)
		{
			audioSource.volume += waitTime;
			yield return new WaitForSeconds(waitTime);
		}
	}

	private void Cooldown()
	{
	}
}
