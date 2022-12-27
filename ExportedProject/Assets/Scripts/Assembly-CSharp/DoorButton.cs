using System;
using System.Collections;
using UnityEngine;
using VRTK;

public class DoorButton : MonoBehaviour
{
	public Animator target;

	internal int Vibration = 800;

	public AudioSource audioSource;

	public AudioClip beep;

	public bool oneShot;

	private GameObject[] doors;

	private void Start()
	{
		if (GetComponent<AnimateButton>() == this)
		{
			AssignGenerator();
		}
		if (target == null)
		{
			doors = GameObject.FindGameObjectsWithTag("Door");
			target = GetClosestDoor().GetComponent<Animator>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller" && !IsInvoking("Cooldown") && target != other.transform.root)
		{
			Invoke("Cooldown", 0.5f);
			if ((bool)other.transform.GetComponentInParent<VRTK_ControllerEvents>())
			{
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.GetComponentInParent<VRTK_ControllerEvents>().gameObject), Vibration);
			}
			if (!oneShot)
			{
				target.SetBool("Open", !target.GetBool("Open"));
			}
			else
			{
				target.SetBool("Open", true);
				Invoke("Reset", 0.25f);
			}
			audioSource.PlayOneShot(beep);
		}
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

	private void Reset()
	{
		target.SetBool("Open", false);
	}

	public void TogglePower(bool on)
	{
		if (on)
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					transform.gameObject.SetActive(true);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			GetComponent<Collider>().enabled = true;
			return;
		}
		IEnumerator enumerator2 = base.transform.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				Transform transform2 = (Transform)enumerator2.Current;
				transform2.gameObject.SetActive(false);
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = enumerator2 as IDisposable) != null)
			{
				disposable2.Dispose();
			}
		}
		GetComponent<Collider>().enabled = false;
	}

	private Transform GetClosestDoor()
	{
		Transform result = null;
		float num = 25f;
		GameObject[] array = doors;
		foreach (GameObject gameObject in array)
		{
			float num2 = Vector3.Distance(gameObject.transform.position, base.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = gameObject.transform;
			}
		}
		return result;
	}

	private void Cooldown()
	{
	}
}
