using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class DroneArmUIManager : MonoBehaviour
{
	public static DroneArmUIManager instance;

	public GameObject[] DroneUI_On;

	public GameObject[] DroneUI_Off;

	public GameObject VRDroneLight;

	private Vector3 oldPos;

	public Image[] buttonIcons;

	public Color defaultColor;

	public Color selectedColor;

	internal int toolIndex;

	public Transform[] shootPoints;

	private RaycastHit hit;

	public ToggleButton flashLightToggle;

	private void Awake()
	{
		instance = this;
	}

	public void ToggleDroneUI(bool _On)
	{
		for (int i = 0; i < DroneUI_On.Length; i++)
		{
			DroneUI_On[i].gameObject.SetActive(_On);
		}
		for (int j = 0; j < DroneUI_Off.Length; j++)
		{
			DroneUI_Off[j].gameObject.SetActive(!_On);
		}
		if (_On)
		{
			Gun.instance.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			ArmUIManager.instance.HideTabs();
		}
		else
		{
			ArmUIManager.instance.ShowVitalsTab();
			Gun.instance.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
		}
	}

	public void SwapToDrone(bool value)
	{
		if (!value || !(DroneHelper.instance.power <= 0f))
		{
			ToggleDroneUI(value);
			DroneHelper.instance.ToggleTransfer(value);
			if (value)
			{
				oldPos = GameManager.instance.Head.position;
				Vector3 vector = GameManager.instance.Head.position - GameManager.instance.CamRig.position;
				GameManager.instance.CamRig.position = DroneHelper.instance.transform.position - vector;
			}
			else
			{
				DroneHelper.instance.transform.position = GameManager.instance.Head.position;
				Vector3 vector2 = GameManager.instance.Head.position - GameManager.instance.CamRig.position;
				GameManager.instance.CamRig.position = oldPos - vector2;
			}
		}
	}

	public void ToggleFlash(ToggleButton button)
	{
		VRDroneLight.SetActive(button.On);
	}

	public void ToggleCone()
	{
		toolIndex = 0;
		buttonIcons[0].color = selectedColor;
		buttonIcons[1].color = defaultColor;
	}

	public void ToggleGun()
	{
		Debug.Log("ToggleGun");
		toolIndex = 1;
		DeactivateCones();
		buttonIcons[1].color = selectedColor;
		buttonIcons[0].color = defaultColor;
	}

	public void ToggleTool()
	{
		if (toolIndex == 0)
		{
			ToggleGun();
		}
		else
		{
			ToggleCone();
		}
	}

	private void DeactivateCones()
	{
		if ((bool)HandController.currentHand)
		{
			HandController.currentHand.anim.SetBool("Point", false);
			HandController.currentHand.deteriorationCone.SetActive(false);
		}
	}

	public void OnShoot(Transform transform)
	{
		if (DroneHelper.instance.power < 1f)
		{
			return;
		}
		Transform transform2 = ((!transform.name.ToLower().Contains("left")) ? shootPoints[1] : shootPoints[0]);
		GameObject gameObject = Object.Instantiate(DroneHelper.instance.trail, transform2.position, transform2.rotation);
		gameObject.transform.position = transform2.transform.position;
		if (Physics.Raycast(transform2.position, transform2.forward, out hit, 300f, DroneHelper.instance.layerMask))
		{
			DroneHelper.instance.hit = hit;
			if (Vector3.Distance(transform.position, hit.point) > 5f)
			{
				DroneHelper.instance.Invoke("OnHit", 0.1f);
			}
			else
			{
				DroneHelper.instance.OnHit();
			}
		}
		DroneHelper.instance.OnDrawPower(1f);
	}
}
