using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class HandTrigger : MonoBehaviour
{
	public enum Pose
	{
		Idle = 0,
		Grip = 1,
		Point = 2
	}

	public List<Collider> colliders = new List<Collider>();

	public Pose pose;

	public static List<HandTrigger> handTriggers = new List<HandTrigger>();

	public bool deParent;

	public bool dontTogglePointer;

	private void Start()
	{
		if (deParent)
		{
			base.transform.parent = base.transform.parent.parent;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller" && (bool)other.GetComponentInParent<Animator>() && !other.GetComponentInParent<VRTK_Pointer>().IsPointerActive() && !other.GetComponentInParent<HandController>().deteriorationCone.activeSelf)
		{
			colliders.Add(other);
			if (!dontTogglePointer)
			{
				other.SendMessageUpwards("PoseTrigger", pose.ToString());
			}
			other.GetComponentInParent<Animator>().SetBool(pose.ToString(), true);
			if (other.GetComponentInParent<VRTK_Pointer>().IsPointerActive() && !dontTogglePointer)
			{
				other.GetComponentInParent<VRTK_Pointer>().currentActivationState = 0;
				other.GetComponentInParent<VRTK_Pointer>().Toggle(false);
				CraftingManager.instance.proxy.gameObject.SetActive(false);
			}
			if (!handTriggers.Contains(this))
			{
				handTriggers.Add(this);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!colliders.Contains(other))
		{
			return;
		}
		if (!other.GetComponentInParent<VRTK_Pointer>().IsPointerActive() && !other.GetComponentInParent<HandController>().deteriorationCone.activeSelf)
		{
			if (handTriggers.Contains(this))
			{
				handTriggers.Remove(this);
			}
			if (colliders.Contains(other))
			{
				colliders.Remove(other);
			}
			if (handTriggers.Count == 0)
			{
				other.GetComponentInParent<Animator>().SetBool(pose.ToString(), false);
			}
		}
		else if (handTriggers.Contains(this))
		{
			handTriggers.Remove(this);
		}
	}

	private void OnDisable()
	{
		if (handTriggers.Contains(this))
		{
			handTriggers.Remove(this);
		}
		if (handTriggers.Count != 0)
		{
			return;
		}
		for (int i = 0; i < colliders.Count; i++)
		{
			if (colliders[i] != null && (bool)colliders[i].GetComponentInParent<Animator>())
			{
				colliders[i].GetComponentInParent<Animator>().SetBool(pose.ToString(), false);
			}
		}
	}
}
