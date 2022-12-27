using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class PCButton : MonoBehaviour
{
	public UnityEvent OnClick;

	private Transform target;

	internal int Vibration = 800;

	private Animator animator;

	private void Start()
	{
		if ((bool)GetComponent<Animator>())
		{
			animator = GetComponent<Animator>();
		}
	}

	private void OnEnable()
	{
		target = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag == "Controller") || IsInvoking("Cooldown"))
		{
			return;
		}
		if (target != other.transform.root)
		{
			Invoke("Cooldown", 0.5f);
			target = other.transform.root;
			if ((bool)other.transform.GetComponentInParent<VRTK_ControllerEvents>())
			{
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.GetComponentInParent<VRTK_ControllerEvents>().gameObject), Vibration);
			}
			OnClick.Invoke();
			if ((bool)animator)
			{
				animator.SetBool("Pressed", true);
			}
		}
		else
		{
			target = null;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller")
		{
			target = null;
			if ((bool)animator)
			{
				animator.SetBool("Pressed", false);
			}
		}
	}

	private void Cooldown()
	{
	}
}
