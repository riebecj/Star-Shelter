using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class SuitButton : MonoBehaviour
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
		Invoke("Cooldown", 0.5f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (IsInvoking("Cooldown") || !(other.tag == "Controller"))
		{
			return;
		}
		if (target != other.transform.root)
		{
			target = other.transform.root;
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.parent.gameObject), Vibration);
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

	public void OnHighlight()
	{
		GetComponent<MeshRenderer>().material = Inventory.instance.buttonHighlightedMaterial;
	}

	public void Cooldown()
	{
	}
}
