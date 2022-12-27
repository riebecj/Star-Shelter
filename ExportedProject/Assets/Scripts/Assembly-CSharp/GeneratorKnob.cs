using UnityEngine;
using VRTK;

public class GeneratorKnob : MonoBehaviour
{
	private VRTK_InteractableObject interact;

	internal float value;

	public PowerGrid powerGrid;

	private HingeJoint joint;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		joint = GetComponent<HingeJoint>();
	}

	public bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void Update()
	{
		if (IsHeld())
		{
			UpdateValue();
		}
	}

	private void UpdateValue()
	{
		if (joint.angle > 120f && powerGrid.complete)
		{
			TurnOn();
		}
	}

	private void TurnOn()
	{
		interact.ForceStopInteracting();
		GetComponent<Collider>().enabled = false;
		powerGrid.Activate();
	}
}
