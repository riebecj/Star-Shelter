using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Thruster : MonoBehaviour
{
	public static List<Thruster> thrusters = new List<Thruster>();

	public Transform forwardRef;

	internal VRTK_ControllerEvents events;

	internal VRTK_InteractGrab grabController;

	public Rigidbody body;

	public Thruster other;

	public GameObject particle;

	public float force = 3f;

	internal float Vibration = 0.05f;

	internal float maxSpeed = 8f;

	internal float oxygenCost = 1.5f;

	internal bool deactivated;

	internal float thrustValue;

	private void Awake()
	{
		thrusters.Add(this);
	}

	private void Start()
	{
		events = GetComponentInChildren<VRTK_ControllerEvents>();
		events.TriggerPressed += OnTriggerPressed;
		grabController = GetComponentInChildren<VRTK_InteractGrab>();
		if ((bool)IntroManager.instance)
		{
			maxSpeed = 4f;
		}
	}

	private void OnTriggerPressed(object sender, ControllerInteractionEventArgs e)
	{
	}

	private void Update()
	{
		if (deactivated || ((bool)IntroManager.instance && !IntroManager.instance.canThrust) || !ArmUIManager.instance)
		{
			return;
		}
		if ((!ArmUIManager.instance.gripSwap && events.gripPressed) || (ArmUIManager.instance.gripSwap && events.triggerPressed && !grabController.GetGrabbedObject()))
		{
			if (SuitManager.instance.oxygen > 0f)
			{
				GameManager.instance.CamRig.GetComponent<Rigidbody>().drag = 0f;
				if (DroneHelper.instance == null || !DroneHelper.instance.VRControlled)
				{
					particle.SetActive(true);
				}
				body.AddForce(forwardRef.forward * force);
				if (DroneHelper.instance == null || !DroneHelper.instance.VRControlled)
				{
					if (!GameManager.instance.debugMode && !IntroManager.instance && !SuitManager.instance.inOxygenZone)
					{
						SuitManager.instance.DrainOxygen(oxygenCost);
					}
					VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(base.gameObject), Vibration);
				}
				if ((bool)IntroManager.instance)
				{
					thrustValue += 1f * Time.deltaTime;
					if (thrustValue > 2f)
					{
						IntroManager.instance.ThrusterComplete();
					}
				}
				else
				{
					HintManager.instance.ThrusterCheck();
					TutorialManager.instance.OnThrust();
				}
			}
			else if (particle.activeSelf)
			{
				particle.SetActive(false);
			}
			if (DroneHelper.instance == null || !DroneHelper.instance.VRControlled)
			{
				SuitManager.instance.ToggleMaskOxygen();
			}
		}
		else if (particle.activeSelf)
		{
			particle.SetActive(false);
		}
		if (body.velocity.magnitude > 8f)
		{
			body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
		}
	}

	private void OnDestroy()
	{
		thrusters.Remove(this);
	}
}
