using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class RotateView : MonoBehaviour
{
	internal VRTK_ControllerEvents events;

	internal Vector2 touchAxis;

	internal bool active;

	private Transform world;

	public static List<RotateView> RC = new List<RotateView>();

	public static bool cooldown;

	private void Awake()
	{
		RC.Add(this);
	}

	private void Start()
	{
		events = GetComponent<VRTK_ControllerEvents>();
		events.TouchpadAxisChanged += DoTouchpadAxisChanged;
		world = GameObject.FindGameObjectWithTag("World").transform;
	}

	private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
	{
		touchAxis = e.touchpadAxis;
		if (Mathf.Abs(touchAxis.x) < 0.25f)
		{
			active = false;
		}
		if (!(Mathf.Abs(touchAxis.x) > 0.7f) || active)
		{
			return;
		}
		if (base.name.StartsWith("Right") && CraftingManager.instance.proxy.gameObject.activeSelf)
		{
			active = true;
			if (touchAxis.x > 0f)
			{
				CraftingManager.instance.RotateProxy(true);
			}
			else
			{
				CraftingManager.instance.RotateProxy(false);
			}
		}
		else
		{
			if ((bool)GetComponent<VRTK_InteractGrab>().GetGrabbedObject() || !ArmUIManager.instance.canRotate || (base.name.StartsWith("Right") && BaseCraftManager.instance.roomPointer.activeSelf) || cooldown)
			{
				return;
			}
			VRTK_BodyPhysics.instance.motionTimer = 0f;
			active = true;
			cooldown = true;
			if (!ArmUIManager.instance.gradualRotation)
			{
				InstantRotation();
			}
			else if (world.transform.parent != null)
			{
				if (touchAxis.x > 0f)
				{
					StartCoroutine("StartRotation", true);
				}
				else
				{
					StartCoroutine("StartRotation", false);
				}
			}
			Invoke("Cooldown", 0.25f);
		}
	}

	private IEnumerator StartRotation(bool right)
	{
		float timer = 0f;
		float waitTime = 0.02f;
		int value = -1;
		if (right)
		{
			value = 1;
		}
		world.transform.SetParent(null);
		if ((bool)CometManager.instance)
		{
			CometManager.instance.cometRain.transform.parent.SetParent(null);
		}
		while (timer < 90f)
		{
			timer += waitTime * 562.5f;
			GameManager.instance.CamRig.RotateAround(GameManager.instance.Head.transform.position, Vector3.up, timer * (float)value / 4.5f);
			yield return new WaitForSeconds(waitTime);
		}
		world.transform.SetParent(GameManager.instance.CamRig);
		if ((bool)CometManager.instance)
		{
			CometManager.instance.cometRain.transform.parent.SetParent(GameManager.instance.CamRig);
		}
	}

	public void InstantRotation()
	{
		world.transform.SetParent(null);
		if ((bool)CometManager.instance)
		{
			CometManager.instance.cometRain.transform.parent.SetParent(null);
		}
		if (touchAxis.x > 0f)
		{
			GameManager.instance.CamRig.RotateAround(GameManager.instance.Head.transform.position, Vector3.up, 90f);
		}
		else
		{
			GameManager.instance.CamRig.RotateAround(GameManager.instance.Head.transform.position, Vector3.up, -90f);
		}
		world.transform.SetParent(GameManager.instance.CamRig);
		if ((bool)CometManager.instance)
		{
			CometManager.instance.cometRain.transform.parent.SetParent(GameManager.instance.CamRig);
		}
	}

	private void Cooldown()
	{
		cooldown = false;
	}
}
