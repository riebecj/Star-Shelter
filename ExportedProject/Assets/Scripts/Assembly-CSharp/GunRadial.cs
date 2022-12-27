using UnityEngine;
using VRTK;

public class GunRadial : MonoBehaviour
{
	public static GunRadial instance;

	public RadialMenu.RadialMenuButton[] buttons;

	private RadialMenuController radialMenuController;

	private void Awake()
	{
		instance = this;
		radialMenuController = GetComponent<RadialMenuController>();
	}

	private void Start()
	{
		GetComponent<RadialMenu>().AddButton(new RadialMenu.RadialMenuButton(), Gun.instance.baseIcon);
		GetComponent<RadialMenu>().buttons[0].OnClick.AddListener(SetShotTypeBasic);
	}

	public void CreateButtons()
	{
		GetComponent<RadialMenu>().buttons.Clear();
		GetComponent<RadialMenu>().AddButton(new RadialMenu.RadialMenuButton(), Gun.instance.baseIcon);
		GetComponent<RadialMenu>().buttons[0].OnClick.AddListener(SetShotTypeBasic);
		for (int i = 0; i < Gun.instance.shotModules.Count; i++)
		{
			GetComponent<RadialMenu>().AddButton(new RadialMenu.RadialMenuButton(), Gun.instance.shotModules[i].icon);
		}
		buttons = GetComponent<RadialMenu>().buttons.ToArray();
		for (int j = 0; j < Gun.instance.shotModules.Count; j++)
		{
			Gun.instance.shotModules[j].AssignListner(buttons[j + 1]);
		}
	}

	public void SetShotTypeBasic()
	{
		Gun.instance.shotType = Gun.ShotType.BasicShot;
		Gun.instance.OnUpdateAmmo(4 + UpgradeManager.AmmoCapacity * 2);
		Gun.instance.damage = 3;
		Gun.instance.fireParticle = Gun.instance.basicParticle;
		Gun.instance.PlayShotPromt(0);
		Gun.instance.ammoType.sprite = Gun.instance.baseIcon;
		Gun.instance.reloadMultiplier = 1f;
		Gun.instance.shotCostMultiplier = 1f;
	}

	public void OnGrab(VRTK_ControllerEvents events)
	{
		radialMenuController.OnGrab(events);
	}

	public void OnDrop(VRTK_ControllerEvents events)
	{
		radialMenuController.OnDrop(events);
	}
}
