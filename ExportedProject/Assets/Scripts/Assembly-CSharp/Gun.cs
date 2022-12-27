using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Gun : MonoBehaviour
{
	public enum ShotType
	{
		BasicShot = 0,
		PushShot = 1,
		RapidShot = 2,
		SplitShot = 3,
		MissileShot = 4,
		HookShot = 5
	}

	internal VRTK_InteractableObject interact;

	internal VRTK_ControllerEvents holdControl;

	public GameObject trail;

	public GameObject impactParticle;

	public Transform barrelPoint;

	public Transform modSlot;

	private float Vibration = 15000f;

	private int shotCost = 6;

	private int pitchIndex;

	public int damage = 3;

	public int charges = 8;

	public int clipSize = 8;

	internal AudioSource audioSource;

	public AudioClip holsterAudio;

	public AudioClip reloading;

	public AudioClip reloaded;

	public AudioClip snapOn;

	public AudioClip snapOff;

	public AudioClip[] blastSounds;

	public AudioClip[] shotTypePromts;

	internal Transform newTrail;

	private Vector3 trailGoal;

	public Image ammoType;

	public Image powerRing;

	private Image ammoBar;

	public Text number;

	public static Gun instance;

	internal float reloadTime = 0.5f;

	internal float reloadDuration;

	internal float holsterDistance = 0.35f;

	internal float reloadMultiplier = 1f;

	internal float shotCostMultiplier = 1f;

	internal bool holstered;

	private RaycastHit hit;

	public Animation overheat;

	public Image[] ammoBars;

	private List<Image> activeAmmoBars = new List<Image>();

	private RectTransform ammoGrid;

	public GameObject basicParticle;

	public GameObject missile;

	internal GameObject fireParticle;

	public ShotType shotType;

	public List<GunModule> shotModules;

	public List<GunModule> sightModules;

	public List<GunModule> saveModules;

	private GunModule gunModule;

	public Collider GrabCollider;

	public Sprite baseIcon;

	internal int moduleIndex;

	public bool topSlotLocked;

	public bool bottomSlotLocked;

	public bool frontSlotLocked;

	public LayerMask layerMask;

	public int basicAmmo = 4;

	public int missileAmmo;

	public int pushAmmo;

	public int splitAmmo;

	public int rapidAmmo;

	private void Awake()
	{
		instance = this;
		interact = GetComponent<VRTK_InteractableObject>();
		ammoGrid = (RectTransform)ammoBars[0].transform.parent;
		ammoGrid.sizeDelta = new Vector2(45f, ammoGrid.sizeDelta.y);
	}

	private void Start()
	{
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
		interact.InteractableObjectTouched += OnTouch;
		interact.InteractableObjectUntouched += OnUnTouch;
		audioSource = GetComponent<AudioSource>();
		if (!IntroManager.instance)
		{
			LoadMods();
		}
		Holster();
		UpdateUI();
		fireParticle = basicParticle;
		OnSetupAmmo();
	}

	public void OnUpgrade(int ammoSize)
	{
		clipSize = ammoSize;
		OnSetupAmmo();
		charges = clipSize;
		UpdateUI();
	}

	public void OnUpdateAmmo(int ammoSize)
	{
		clipSize = ammoSize;
		OnSetupAmmo();
		ammoGrid.sizeDelta = new Vector2(60f, ammoGrid.sizeDelta.y);
		if (shotType == ShotType.BasicShot)
		{
			charges = basicAmmo;
			ammoGrid.sizeDelta = new Vector2(45f, ammoGrid.sizeDelta.y);
		}
		if (shotType == ShotType.MissileShot)
		{
			charges = missileAmmo;
		}
		if (shotType == ShotType.PushShot)
		{
			charges = pushAmmo;
		}
		if (shotType == ShotType.RapidShot)
		{
			charges = rapidAmmo;
		}
		if (shotType == ShotType.SplitShot)
		{
			charges = splitAmmo;
		}
		if (shotType == ShotType.HookShot)
		{
			charges = (int)SuitManager.instance.power;
		}
		UpdateUI();
	}

	private void OnSetupAmmo()
	{
		Image[] array = ammoBars;
		foreach (Image image in array)
		{
			image.fillAmount = 0f;
			image.gameObject.SetActive(false);
		}
		activeAmmoBars.Clear();
		for (int j = 0; j < clipSize; j++)
		{
			activeAmmoBars.Add(ammoBars[j]);
			ammoBars[j].fillAmount = 1f;
			ammoBars[j].gameObject.SetActive(true);
		}
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
		{
			holdControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>();
			if ((bool)ArmUIManager.instance && !ArmUIManager.instance.gripSwap)
			{
				VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
				VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			}
			GunRadial.instance.OnGrab(VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>());
			interact.allowedTouchControllers = VRTK_InteractableObject.AllowedController.LeftOnly;
		}
		else
		{
			holdControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
			if ((bool)ArmUIManager.instance && !ArmUIManager.instance.gripSwap)
			{
				VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
				VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			}
			GunRadial.instance.OnGrab(VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>());
			interact.allowedTouchControllers = VRTK_InteractableObject.AllowedController.RightOnly;
		}
		holdControl.TriggerPressed += OnTriggerPressed;
		holdControl.TriggerReleased += OnTriggerReleased;
		holdControl.GetComponentInParent<HandController>().anim.SetBool("Gun", true);
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(holsterAudio);
		if ((bool)IntroGunEvent.instance)
		{
			IntroGunEvent.instance.OnPickup();
			interact.previousKinematicState = false;
		}
		foreach (GunModule shotModule in shotModules)
		{
			shotModule.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
			if (shotModule.moduleType == GunModule.ModuleType.Hook)
			{
				GetComponentInChildren<HookModule>().OnSetup(instance.holdControl);
			}
		}
		foreach (GunModule sightModule in sightModules)
		{
			sightModule.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
			if (sightModule.moduleType == GunModule.ModuleType.LaserSight)
			{
				sightModule.GetComponent<LaserSight>().ToggleLaser(true);
			}
		}
		Invoke("Cooldown", 0.25f);
		holstered = false;
		GrabCollider.enabled = false;
		BoxCollider boxCollider = (BoxCollider)GrabCollider;
		boxCollider.size = new Vector3(0.21f, 0.3f, 0.4f);
		StartCoroutine("CheckHolster");
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		holdControl.TriggerPressed -= OnTriggerPressed;
		Holster();
		GrabCollider.enabled = true;
		BoxCollider boxCollider = (BoxCollider)GrabCollider;
		boxCollider.size = new Vector3(0.21f, 0.49f, 0.6f);
		interact.allowedTouchControllers = VRTK_InteractableObject.AllowedController.Both;
		if ((bool)holdControl.GetComponentInParent<HandController>())
		{
			holdControl.GetComponentInParent<HandController>().anim.SetBool("Gun", false);
		}
		if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
		{
			GunRadial.instance.OnDrop(VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>());
		}
		else
		{
			GunRadial.instance.OnDrop(VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>());
		}
	}

	private void OnTouch(object sender, InteractableObjectEventArgs e)
	{
		if (!DroneHelper.instance.VRControlled && holstered)
		{
			SpaceMask.instance.holsterIcon.SetActive(true);
		}
	}

	private void OnUnTouch(object sender, InteractableObjectEventArgs e)
	{
		SpaceMask.instance.holsterIcon.SetActive(false);
	}

	private void OnTriggerPressed(object sender, ControllerInteractionEventArgs e)
	{
		if (Vector3.Distance(base.transform.position, GunHolster.instance.transform.position) < holsterDistance)
		{
			if (!holstered && !IsInvoking("Cooldown"))
			{
				Holster();
			}
		}
		else if (charges > 0 && !IsInvoking("OnSnap"))
		{
			if (shotType == ShotType.BasicShot)
			{
				BasicShot();
			}
			else if (shotType == ShotType.PushShot)
			{
				PushShot();
			}
			else if (shotType == ShotType.RapidShot)
			{
				OnRapidShot();
			}
			else if (shotType == ShotType.SplitShot)
			{
				SplitShot();
			}
			else if (shotType == ShotType.MissileShot)
			{
				MissileShot();
			}
		}
	}

	private void OnTriggerReleased(object sender, ControllerInteractionEventArgs e)
	{
		StopCoroutine("UpdateRapidFire");
	}

	private void BasicShot()
	{
		charges--;
		if (basicAmmo > 0)
		{
			basicAmmo--;
		}
		VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration, 0.15f, 0.01f);
		fireParticle.SetActive(false);
		fireParticle.SetActive(true);
		Invoke("DisalbeFire", 0.25f);
		GameObject gameObject = Object.Instantiate(trail, barrelPoint.position, barrelPoint.rotation);
		newTrail = gameObject.transform;
		newTrail.position = barrelPoint.transform.position;
		if (Physics.Raycast(barrelPoint.position, barrelPoint.forward, out hit, 300f, layerMask))
		{
			if (Vector3.Distance(base.transform.position, hit.point) > 5f)
			{
				Invoke("OnHit", 0.1f);
			}
			else
			{
				OnHit();
			}
		}
		else if (!PreviewLabs.PlayerPrefs.GetBool("shootAndMiss"))
		{
			PreviewLabs.PlayerPrefs.SetBool("shootAndMiss", true);
			GameAudioManager.instance.AddToSuitQueue(SuitManager.instance.GunInfo);
		}
		if ((bool)CryoPodLever.instance && CryoPodLever.instance.open)
		{
			GameManager.instance.CamRig.GetComponent<Rigidbody>().AddForce(barrelPoint.forward * -1.5f, ForceMode.VelocityChange);
		}
		audioSource.PlayOneShot(blastSounds[Random.Range(0, blastSounds.Length)]);
		pitchIndex++;
		reloadDuration = 0f;
		UpdateUI();
		if (charges == 0)
		{
			Invoke("OverHeat", 0.25f);
		}
	}

	private void RapidShot()
	{
		charges--;
		rapidAmmo--;
		VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration, 0.15f, 0.01f);
		fireParticle.SetActive(false);
		fireParticle.SetActive(true);
		Invoke("DisalbeFire", 0.25f);
		GameObject gameObject = Object.Instantiate(trail, barrelPoint.position, barrelPoint.rotation);
		newTrail = gameObject.transform;
		newTrail.position = barrelPoint.transform.position;
		if (Physics.Raycast(barrelPoint.position, barrelPoint.forward, out hit, 300f, layerMask))
		{
			if (Vector3.Distance(base.transform.position, hit.point) > 5f)
			{
				Invoke("OnHit", 0.1f);
			}
			else
			{
				OnHit();
			}
		}
		if ((bool)CryoPodLever.instance && CryoPodLever.instance.open)
		{
			GameManager.instance.CamRig.GetComponent<Rigidbody>().AddForce(barrelPoint.forward * -1.5f, ForceMode.VelocityChange);
		}
		audioSource.PlayOneShot(blastSounds[Random.Range(0, blastSounds.Length)]);
		pitchIndex++;
		reloadDuration = 0f;
		UpdateUI();
		if (charges == 0)
		{
			Invoke("OverHeat", 0.25f);
		}
	}

	private void PushShot()
	{
		charges--;
		pushAmmo--;
		VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration, 0.15f, 0.01f);
		fireParticle.SetActive(false);
		fireParticle.SetActive(true);
		Invoke("DisalbeFire", 0.25f);
		if (CryoPodLever.instance.open)
		{
			GameManager.instance.CamRig.GetComponent<Rigidbody>().AddForce(barrelPoint.forward * -4f, ForceMode.VelocityChange);
		}
		audioSource.PlayOneShot(blastSounds[Random.Range(0, blastSounds.Length)]);
		pitchIndex++;
		reloadDuration = 0f;
		UpdateUI();
		if (charges == 0)
		{
			Invoke("OverHeat", 0.25f);
		}
	}

	private void OnRapidShot()
	{
		StartCoroutine("UpdateRapidFire");
	}

	private void SplitShot()
	{
		charges--;
		splitAmmo--;
		VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration, 0.15f, 0.01f);
		fireParticle.SetActive(false);
		fireParticle.SetActive(true);
		Invoke("DisalbeFire", 0.25f);
		for (int i = 0; i < 5; i++)
		{
			Vector3 vector = barrelPoint.forward + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0f);
			GameObject gameObject = Object.Instantiate(trail, barrelPoint.position, barrelPoint.rotation);
			newTrail = gameObject.transform;
			newTrail.position = barrelPoint.transform.position;
			newTrail.LookAt(barrelPoint.transform.position + vector);
			if (Physics.Raycast(barrelPoint.position, vector, out hit, 300f, layerMask))
			{
				if (Vector3.Distance(base.transform.position, hit.point) > 5f)
				{
					Invoke("OnHit", 0.1f);
				}
				else
				{
					OnHit();
				}
			}
		}
		if (CryoPodLever.instance.open)
		{
			GameManager.instance.CamRig.GetComponent<Rigidbody>().AddForce(barrelPoint.forward * -1.5f, ForceMode.VelocityChange);
		}
		audioSource.PlayOneShot(blastSounds[Random.Range(0, blastSounds.Length)]);
		pitchIndex++;
		reloadDuration = 0f;
		UpdateUI();
		if (charges == 0)
		{
			Invoke("OverHeat", 0.25f);
		}
	}

	private void MissileShot()
	{
		VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration, 0.15f, 0.01f);
		charges--;
		missileAmmo--;
		Object.Instantiate(missile, barrelPoint.position, barrelPoint.rotation);
		UpdateUI();
		if (charges == 0)
		{
			Invoke("OverHeat", 0.25f);
		}
	}

	private void DisalbeFire()
	{
		fireParticle.SetActive(false);
	}

	private void OverHeat()
	{
		TutorialManager.instance.ToggleReload();
		overheat.Play();
		StartCoroutine("OverHeatVibration");
	}

	private void OnHit()
	{
		Rigidbody rigidbody = null;
		if ((bool)hit.collider.GetComponent<Rigidbody>())
		{
			rigidbody = hit.collider.GetComponent<Rigidbody>();
		}
		if ((bool)hit.collider.GetComponentInParent<HoloShield>())
		{
			hit.collider.GetComponentInParent<HoloShield>().TakeDamage(damage);
			Object.Instantiate(impactParticle, hit.point, barrelPoint.rotation);
			return;
		}
		if ((bool)hit.collider.GetComponent<DroneAI>())
		{
			hit.collider.GetComponent<DroneAI>().OnTakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<Plate>())
		{
			hit.collider.GetComponent<Plate>().TakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<SolarPanel>())
		{
			hit.collider.GetComponent<SolarPanel>().TakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<Turret>())
		{
			hit.collider.GetComponent<Turret>().TakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<TitanWeakPoint>())
		{
			hit.collider.GetComponent<TitanWeakPoint>().TakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<TitanMissile>())
		{
			hit.collider.GetComponent<TitanMissile>().OnExplode();
		}
		else if ((bool)hit.collider.GetComponentInParent<Room>())
		{
			hit.collider.GetComponentInParent<Room>().OnImpact(hit.point, damage);
		}
		else if ((bool)hit.collider.GetComponent<DroneProjectile>())
		{
			hit.collider.GetComponent<DroneProjectile>().OnDestruct();
		}
		else if (hit.collider.transform.root.tag == "Player" && !IntroManager.instance && !hit.collider.GetComponent<GunModule>())
		{
			Debug.Log(hit.collider.name);
			SuitManager.instance.OnTakeDamage((int)SuitManager.instance.health + 1, 2);
		}
		if ((bool)rigidbody && !rigidbody.isKinematic)
		{
			rigidbody.AddForce(base.transform.forward * 1000f);
		}
		if (rigidbody != null)
		{
			rigidbody.SendMessage("OnTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			rigidbody.SendMessage("OnShot", hit.point, SendMessageOptions.DontRequireReceiver);
			if ((bool)rigidbody.GetComponent<Comet>())
			{
				rigidbody.SendMessage("OnGetShot", SendMessageOptions.DontRequireReceiver);
			}
		}
		Object.Instantiate(impactParticle, hit.point, barrelPoint.rotation);
	}

	private void MoveTrail()
	{
	}

	public void Holster()
	{
		GetComponentInChildren<RadialMenu>().HideMenu(true);
		holstered = true;
		interact.ForceStopInteracting();
		if ((bool)ArmUIManager.instance && !ArmUIManager.instance.gripSwap)
		{
			VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
			VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
		}
		base.transform.position = GunHolster.instance.gunPos.transform.position;
		base.transform.rotation = GunHolster.instance.gunPos.transform.rotation;
		FixedJoint fixedJoint = base.gameObject.AddComponent<FixedJoint>();
		fixedJoint.breakForce = float.PositiveInfinity;
		fixedJoint.connectedBody = GunHolster.instance.gunPos;
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(holsterAudio);
		Invoke("Cooldown", 0.5f);
		if ((bool)IntroManager.instance)
		{
			IntroManager.instance.GunComplete();
		}
		StopCoroutine("CheckHolster");
		SpaceMask.instance.holsterIcon.SetActive(false);
		foreach (GunModule shotModule in shotModules)
		{
			shotModule.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
		}
		foreach (GunModule sightModule in sightModules)
		{
			sightModule.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			if (sightModule.moduleType == GunModule.ModuleType.LaserSight)
			{
				sightModule.GetComponent<LaserSight>().ToggleLaser(false);
			}
		}
	}

	private IEnumerator OverHeatVibration()
	{
		float timer = 0f;
		while (timer < 1f)
		{
			timer += 0.05f;
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration, 0.15f, 0.01f);
			yield return new WaitForSeconds(0.05f);
		}
	}

	private IEnumerator CheckHolster()
	{
		while (true)
		{
			if (Vector3.Distance(base.transform.position, GunHolster.instance.transform.position) < holsterDistance && !holstered)
			{
				SpaceMask.instance.holsterIcon.SetActive(true);
			}
			else
			{
				SpaceMask.instance.holsterIcon.SetActive(false);
			}
			yield return new WaitForSeconds(0.05f);
		}
	}

	public void UpdatePower()
	{
		SuitManager.instance.power -= Time.deltaTime * 0.25f;
		powerRing.fillAmount = SuitManager.instance.power / SuitManager.instance.maxPower;
	}

	public void Reloading()
	{
		if (SuitManager.instance.power < (float)shotCost * shotCostMultiplier || charges == clipSize || shotType == ShotType.HookShot)
		{
			return;
		}
		if (!audioSource.isPlaying)
		{
			audioSource.pitch = 1f;
			audioSource.clip = reloading;
			pitchIndex = 1;
		}
		reloadDuration += Time.deltaTime * reloadMultiplier;
		VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), 0.05f);
		ammoBar = activeAmmoBars[charges];
		ammoBar.fillAmount = reloadDuration;
		if (reloadDuration > 1f && charges < clipSize && SuitManager.instance.power >= (float)shotCost * shotCostMultiplier && charges < clipSize)
		{
			if (!IntroManager.instance)
			{
				SuitManager.instance.power -= (float)shotCost * shotCostMultiplier;
			}
			charges++;
			AddAmmo();
			reloadDuration = 0f;
			audioSource.PlayOneShot(reloaded);
			UpdateUI();
			TutorialManager.instance.OnReloadComplete();
			HintManager.instance.Invoke("GunModsCheck", 5f);
		}
	}

	private void AddAmmo()
	{
		if (shotType == ShotType.BasicShot)
		{
			basicAmmo++;
		}
		else if (shotType == ShotType.MissileShot)
		{
			missileAmmo++;
		}
		else if (shotType == ShotType.PushShot)
		{
			pushAmmo++;
		}
		else if (shotType == ShotType.RapidShot)
		{
			rapidAmmo++;
		}
		else if (shotType == ShotType.SplitShot)
		{
			splitAmmo++;
		}
	}

	public void UpdateUI()
	{
		for (int i = 0; i < clipSize; i++)
		{
			if (i >= charges)
			{
				activeAmmoBars[i].fillAmount = 0f;
			}
		}
		number.text = charges.ToString();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (IsInvoking("SnapCoolDown") || !other.GetComponent<GunModule>() || !other.GetComponent<VRTK_InteractableObject>().IsGrabbed())
		{
			return;
		}
		gunModule = other.GetComponent<GunModule>();
		if (!shotModules.Contains(gunModule) && !sightModules.Contains(gunModule) && !IsInvoking("OnSnap"))
		{
			if (gunModule.SlotCheck())
			{
				other.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
				other.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
				other.isTrigger = true;
				Invoke("OnSnap", 0.1f);
				GetComponentInChildren<RadialMenu>().HideMenu(true);
			}
			HintManager.instance.gunMods = true;
			PreviewLabs.PlayerPrefs.SetBool("gunMods", true);
		}
	}

	private void OnSnap()
	{
		if (gunModule.moduleType == GunModule.ModuleType.LaserSight || gunModule.moduleType == GunModule.ModuleType.ReticleSight)
		{
			sightModules.Add(gunModule);
		}
		else
		{
			shotModules.Add(gunModule);
		}
		gunModule.OnActivateModule();
		gunModule.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
		gunModule.GetComponent<VRTK_InteractableObject>().previousParent = base.transform;
		gunModule.GetComponent<VRTK_InteractableObject>().previousKinematicState = true;
		audioSource.PlayOneShot(snapOn, 0.5f);
	}

	public void OnSnapOff()
	{
		audioSource.PlayOneShot(snapOff, 0.5f);
	}

	private void SnapCoolDown()
	{
	}

	private IEnumerator UpdateRapidFire()
	{
		while (charges > 0)
		{
			RapidShot();
			yield return new WaitForSeconds(0.18f);
		}
	}

	public void PlayShotPromt(int index)
	{
		if (!IsInvoking("PromtCooldown"))
		{
			audioSource.pitch = 1f;
			GameAudioManager.instance.AddToSuitQueue(shotTypePromts[index]);
			Invoke("PromtCooldown", 1f);
		}
	}

	private void LoadMods()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("LaserSight"))
		{
			foreach (GunModule saveModule in saveModules)
			{
				if (saveModule.moduleType == GunModule.ModuleType.LaserSight)
				{
					OnAddModule(saveModule);
				}
			}
		}
		if (PreviewLabs.PlayerPrefs.GetBool("ReticleSight"))
		{
			foreach (GunModule saveModule2 in saveModules)
			{
				if (saveModule2.moduleType == GunModule.ModuleType.ReticleSight)
				{
					OnAddModule(saveModule2);
				}
			}
		}
		if (PreviewLabs.PlayerPrefs.GetBool("RapidFire"))
		{
			foreach (GunModule saveModule3 in saveModules)
			{
				if (saveModule3.moduleType == GunModule.ModuleType.RapidFire)
				{
					OnAddModule(saveModule3);
				}
			}
		}
		if (PreviewLabs.PlayerPrefs.GetBool("Missile"))
		{
			foreach (GunModule saveModule4 in saveModules)
			{
				if (saveModule4.moduleType == GunModule.ModuleType.Missile)
				{
					OnAddModule(saveModule4);
				}
			}
		}
		if (PreviewLabs.PlayerPrefs.GetBool("Pusher"))
		{
			foreach (GunModule saveModule5 in saveModules)
			{
				if (saveModule5.moduleType == GunModule.ModuleType.Pusher)
				{
					OnAddModule(saveModule5);
				}
			}
		}
		if (PreviewLabs.PlayerPrefs.GetBool("Splitter"))
		{
			foreach (GunModule saveModule6 in saveModules)
			{
				if (saveModule6.moduleType == GunModule.ModuleType.Splitter)
				{
					OnAddModule(saveModule6);
				}
			}
		}
		if (!PreviewLabs.PlayerPrefs.GetBool("Hook"))
		{
			return;
		}
		foreach (GunModule saveModule7 in saveModules)
		{
			if (saveModule7.moduleType == GunModule.ModuleType.Hook)
			{
				OnAddModule(saveModule7);
			}
		}
	}

	private void OnAddModule(GunModule module)
	{
		module.gameObject.SetActive(true);
		if (module.moduleType == GunModule.ModuleType.LaserSight || module.moduleType == GunModule.ModuleType.ReticleSight)
		{
			topSlotLocked = true;
			sightModules.Add(module);
		}
		else
		{
			shotModules.Add(module);
			if (module.moduleType == GunModule.ModuleType.Pusher || module.moduleType == GunModule.ModuleType.Splitter)
			{
				frontSlotLocked = true;
			}
			else
			{
				bottomSlotLocked = true;
			}
		}
		module.OnActivateModule();
	}

	public void ClearModules()
	{
		List<GunModule> list = new List<GunModule>();
		for (int i = 0; i < sightModules.Count; i++)
		{
			list.Add(sightModules[i]);
		}
		for (int j = 0; j < shotModules.Count; j++)
		{
			list.Add(shotModules[j]);
		}
		for (int k = 0; k < list.Count; k++)
		{
			GunModule gunModule = list[k];
			gunModule.OnRemoveModule();
			Object.Destroy(gunModule.gameObject);
		}
		sightModules.Clear();
		shotModules.Clear();
	}

	private void PromtCooldown()
	{
	}

	private void Cooldown()
	{
	}
}
