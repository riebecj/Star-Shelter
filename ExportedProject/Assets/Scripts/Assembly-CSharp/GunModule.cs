using PreviewLabs;
using UnityEngine;
using VRTK;

public class GunModule : MonoBehaviour
{
	public enum ModuleType
	{
		LaserSight = 0,
		RapidFire = 1,
		Missile = 2,
		Pusher = 3,
		Splitter = 4,
		ReticleSight = 5,
		Hook = 6
	}

	public enum SnapPoint
	{
		Front = 0,
		Bottom = 1,
		Top = 2
	}

	public Sprite icon;

	public string info;

	public ModuleType moduleType;

	public SnapPoint snapPoint;

	internal VRTK_InteractableObject interact;

	public bool locked;

	private GameObject uiObject;

	private void Awake()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		SetupUI();
	}

	private void SetupUI()
	{
		uiObject = Object.Instantiate(GameManager.instance.gunModuleUI, base.transform.position, base.transform.rotation);
		Vector3 center = GetComponentInChildren<Collider>().bounds.center;
		uiObject.transform.position = new Vector3(center.x, center.y, center.z) + Vector3.up * 0.15f;
		uiObject.transform.SetParent(base.transform);
		uiObject.GetComponent<GunModuleUI>().Setup(this);
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		interact.previousParent = null;
		interact.previousKinematicState = false;
		interact.LoadPreviousState();
		GetComponent<Collider>().isTrigger = false;
		if (locked)
		{
			OnRemoveModule();
		}
	}

	public void OnActivateModule()
	{
		base.transform.SetParent(Gun.instance.modSlot);
		GetComponent<Rigidbody>().isKinematic = true;
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localPosition = Vector3.zero;
		Gun.instance.moduleIndex++;
		if (moduleType == ModuleType.LaserSight)
		{
			GetComponent<LaserSight>().ToggleLaser(true);
			PreviewLabs.PlayerPrefs.SetBool("LaserSight", true);
		}
		else if (moduleType == ModuleType.ReticleSight)
		{
			PreviewLabs.PlayerPrefs.SetBool("ReticleSight", true);
		}
		else if (moduleType == ModuleType.Missile)
		{
			PreviewLabs.PlayerPrefs.SetBool("Missile", true);
			GunRadial.instance.CreateButtons();
		}
		else if (moduleType == ModuleType.Pusher)
		{
			PreviewLabs.PlayerPrefs.SetBool("Pusher", true);
			GunRadial.instance.CreateButtons();
		}
		else if (moduleType == ModuleType.RapidFire)
		{
			PreviewLabs.PlayerPrefs.SetBool("RapidFire", true);
			GunRadial.instance.CreateButtons();
		}
		else if (moduleType == ModuleType.Splitter)
		{
			PreviewLabs.PlayerPrefs.SetBool("Splitter", true);
			GunRadial.instance.CreateButtons();
		}
		else if (moduleType == ModuleType.Hook)
		{
			PreviewLabs.PlayerPrefs.SetBool("Hook", true);
			GunRadial.instance.CreateButtons();
		}
		if (snapPoint == SnapPoint.Front)
		{
			Gun.instance.frontSlotLocked = true;
		}
		else if (snapPoint == SnapPoint.Bottom)
		{
			Gun.instance.bottomSlotLocked = true;
		}
		else if (snapPoint == SnapPoint.Top)
		{
			Gun.instance.topSlotLocked = true;
		}
		locked = true;
		Invoke("ChangeLayer", 0.05f);
		uiObject.SetActive(false);
	}

	private void ChangeLayer()
	{
		base.gameObject.layer = 22;
	}

	public void OnRemoveModule()
	{
		Gun.instance.GetComponentInChildren<RadialMenu>().HideMenu(true);
		Gun.instance.GetComponentInChildren<RadialMenu>().buttons.Clear();
		if (moduleType == ModuleType.LaserSight)
		{
			PreviewLabs.PlayerPrefs.SetBool("LaserSight", false);
			GetComponent<LaserSight>().ToggleLaser(false);
		}
		else if (moduleType == ModuleType.ReticleSight)
		{
			PreviewLabs.PlayerPrefs.SetBool("ReticleSight", false);
		}
		else if (moduleType == ModuleType.Missile)
		{
			PreviewLabs.PlayerPrefs.SetBool("Missile", false);
			if (Gun.instance.shotType == Gun.ShotType.MissileShot)
			{
				SetShotTypeBasic();
			}
		}
		else if (moduleType == ModuleType.Pusher)
		{
			PreviewLabs.PlayerPrefs.SetBool("Pusher", false);
			if (Gun.instance.shotType == Gun.ShotType.PushShot)
			{
				SetShotTypeBasic();
			}
		}
		else if (moduleType == ModuleType.RapidFire)
		{
			PreviewLabs.PlayerPrefs.SetBool("RapidFire", false);
			if (Gun.instance.shotType == Gun.ShotType.RapidShot)
			{
				SetShotTypeBasic();
			}
		}
		else if (moduleType == ModuleType.Splitter)
		{
			PreviewLabs.PlayerPrefs.SetBool("Splitter", false);
			if (Gun.instance.shotType == Gun.ShotType.SplitShot)
			{
				SetShotTypeBasic();
			}
		}
		else if (moduleType == ModuleType.Hook)
		{
			PreviewLabs.PlayerPrefs.SetBool("Hook", false);
			Gun.instance.powerRing.transform.parent.gameObject.SetActive(false);
			if (Gun.instance.shotType == Gun.ShotType.HookShot)
			{
				SetShotTypeBasic();
			}
		}
		if (snapPoint == SnapPoint.Front)
		{
			Gun.instance.frontSlotLocked = false;
		}
		else if (snapPoint == SnapPoint.Bottom)
		{
			Gun.instance.bottomSlotLocked = false;
		}
		else if (snapPoint == SnapPoint.Top)
		{
			Gun.instance.topSlotLocked = false;
		}
		locked = false;
		Gun.instance.moduleIndex--;
		if (Gun.instance.shotModules.Contains(this))
		{
			Gun.instance.shotModules.Remove(this);
		}
		else if (Gun.instance.sightModules.Contains(this))
		{
			Gun.instance.sightModules.Remove(this);
		}
		Gun.instance.OnSnapOff();
		Gun.instance.Invoke("SnapCoolDown", 0.5f);
		GunRadial.instance.CreateButtons();
	}

	public bool SlotCheck()
	{
		if (snapPoint == SnapPoint.Front && Gun.instance.frontSlotLocked)
		{
			return false;
		}
		if (snapPoint == SnapPoint.Bottom && Gun.instance.bottomSlotLocked)
		{
			return false;
		}
		if (snapPoint == SnapPoint.Top && Gun.instance.topSlotLocked)
		{
			return false;
		}
		return true;
	}

	public void SetShotTypeMissile()
	{
		Gun.instance.shotType = Gun.ShotType.MissileShot;
		Gun.instance.OnUpdateAmmo(1 + UpgradeManager.AmmoCapacity);
		Gun.instance.fireParticle = Gun.instance.basicParticle;
		Gun.instance.PlayShotPromt(1);
		Gun.instance.ammoType.sprite = icon;
		Gun.instance.reloadMultiplier = 0.5f;
		Gun.instance.shotCostMultiplier = 2f;
	}

	public void SetShotTypePusher()
	{
		Gun.instance.shotType = Gun.ShotType.PushShot;
		Gun.instance.fireParticle = base.transform.GetChild(0).gameObject;
		Gun.instance.OnUpdateAmmo(4 + UpgradeManager.AmmoCapacity * 2);
		Gun.instance.PlayShotPromt(2);
		Gun.instance.ammoType.sprite = icon;
		Gun.instance.reloadMultiplier = 1f;
		Gun.instance.shotCostMultiplier = 1f;
	}

	public void SetShotTypeRapidFire()
	{
		Gun.instance.shotType = Gun.ShotType.RapidShot;
		Gun.instance.OnUpdateAmmo(8 + UpgradeManager.AmmoCapacity * 2);
		Gun.instance.damage = 2;
		Gun.instance.fireParticle = Gun.instance.basicParticle;
		Gun.instance.PlayShotPromt(3);
		Gun.instance.ammoType.sprite = icon;
		Gun.instance.reloadMultiplier = 2f;
		Gun.instance.shotCostMultiplier = 0.5f;
	}

	public void SetShotTypeSplitter()
	{
		Gun.instance.shotType = Gun.ShotType.SplitShot;
		Gun.instance.OnUpdateAmmo(4 + UpgradeManager.AmmoCapacity * 2);
		Gun.instance.damage = 2;
		Gun.instance.fireParticle = Gun.instance.basicParticle;
		Gun.instance.PlayShotPromt(4);
		Gun.instance.ammoType.sprite = icon;
		Gun.instance.reloadMultiplier = 0.75f;
		Gun.instance.shotCostMultiplier = 1.5f;
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

	public void SetShotTypeHook()
	{
		Gun.instance.shotType = Gun.ShotType.HookShot;
		Gun.instance.OnUpdateAmmo(0);
		Gun.instance.fireParticle = base.transform.GetChild(0).gameObject;
		Gun.instance.PlayShotPromt(5);
		Gun.instance.ammoType.sprite = icon;
		GetComponent<HookModule>().OnSetup(Gun.instance.holdControl);
	}

	public void AssignListner(RadialMenu.RadialMenuButton button)
	{
		if (moduleType == ModuleType.LaserSight)
		{
			button.OnClick.AddListener(SetShotTypeBasic);
		}
		else if (moduleType == ModuleType.Missile)
		{
			button.OnClick.AddListener(SetShotTypeMissile);
		}
		else if (moduleType == ModuleType.Pusher)
		{
			button.OnClick.AddListener(SetShotTypePusher);
		}
		else if (moduleType == ModuleType.RapidFire)
		{
			button.OnClick.AddListener(SetShotTypeRapidFire);
		}
		else if (moduleType == ModuleType.Splitter)
		{
			button.OnClick.AddListener(SetShotTypeSplitter);
		}
		else if (moduleType == ModuleType.Hook)
		{
			button.OnClick.AddListener(SetShotTypeHook);
		}
	}
}
