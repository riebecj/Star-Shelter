using System;
using System.Collections;
using UnityEngine;
using VRTK;

public class AirLockLever : MonoBehaviour
{
	public Transform parent;

	private VRTK_InteractableObject interact;

	private float value;

	internal bool dropped;

	internal bool isHeld;

	public Animator airLock;

	internal bool open;

	internal bool active = true;

	private AudioSource audioSource;

	public AudioClip click;

	public bool autoClose;

	public bool toggleSpaceAudio;

	public float autoCloseTime = 5f;

	internal AirlockSync airlockSync;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		AssignGenerator();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		audioSource = GetComponent<AudioSource>();
		if ((bool)GetComponent<AirlockSync>())
		{
			airlockSync = GetComponent<AirlockSync>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<DroneCone>())
		{
			Open();
		}
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		Thruster.thrusters[0].body.velocity = Vector3.zero;
	}

	private void AssignGenerator()
	{
		foreach (PowerGenerator powerGenerator in PowerGenerator.powerGenerators)
		{
			if (Vector3.Distance(base.transform.position, powerGenerator.transform.position) < (float)PowerGenerator.generatorDistance)
			{
				powerGenerator.togglePower = (PowerGenerator.TogglePower)Delegate.Combine(powerGenerator.togglePower, new PowerGenerator.TogglePower(TogglePower));
			}
		}
	}

	public void TogglePower(bool on)
	{
		if (on)
		{
			active = true;
		}
		else
		{
			active = false;
		}
	}

	public bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void Update()
	{
		if (IsHeld() || isHeld)
		{
			UpdateValue();
			dropped = false;
			GetComponent<HingeJoint>().useSpring = false;
		}
		else if (Mathf.Abs(value) > 0.01f && !dropped)
		{
			open = false;
			dropped = true;
			GetComponent<HingeJoint>().useSpring = true;
		}
	}

	private void UpdateValue()
	{
		value = Vector3.Cross(base.transform.forward, parent.forward).magnitude;
		if (value > 0.5f && !open)
		{
			Open();
		}
	}

	private void Open()
	{
		open = true;
		if (!OnBlocked())
		{
			airLock.SetBool("Open", !airLock.GetBool("Open"));
			StartCoroutine("CheckBlockState");
		}
		else
		{
			audioSource.PlayOneShot(GameAudioManager.instance.errorSound);
		}
		if ((bool)airlockSync)
		{
			airlockSync.Invoke("CheckState", 0.5f);
		}
		audioSource.PlayOneShot(click);
		if (autoClose && airLock.GetBool("Open"))
		{
			Invoke("Close", autoCloseTime);
		}
		if (toggleSpaceAudio)
		{
			GameAudioManager.instance.ToggleSpace();
		}
	}

	private void Close()
	{
		airLock.SetBool("Open", false);
	}

	private bool OnBlocked()
	{
		if (!airLock.GetBool("Open"))
		{
			return false;
		}
		Collider[] array = Physics.OverlapSphere(airLock.transform.position, 0.15f);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.transform.root.tag == "Player")
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator CheckBlockState()
	{
		float tick = 0f;
		while (!airLock.GetBool("Open") && tick < 1f)
		{
			Collider[] colliders = Physics.OverlapSphere(airLock.transform.position, 0.15f);
			Collider[] array = colliders;
			foreach (Collider collider in array)
			{
				if (collider.transform.root.tag == "Player" && !airLock.GetBool("Open"))
				{
					airLock.SetBool("Open", true);
				}
			}
			tick += 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
	}
}
