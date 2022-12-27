using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class BaseManager : MonoBehaviour
{
	public float maxPower = 100f;

	public float power = 100f;

	public float oxygenChargeSpeed = 40f;

	public float powerChargeSpeed = 20f;

	public float collectiveOxygen;

	public float collectiveOxygenCap;

	internal float oxygenRechargeSpeed = 20f;

	internal float powerRechargeSpeed = 20f;

	internal float leakSpeed = 1f;

	internal float defaultPowerGain = 0.15f;

	public List<SolarPanel> SolarPanels = new List<SolarPanel>();

	public List<HoloShield> HoloShields = new List<HoloShield>();

	public List<Turret> Turrets = new List<Turret>();

	public List<PowerStation> PowerStations = new List<PowerStation>();

	public Animator[] airlocks;

	public Mesh[] meshes;

	public GameObject repairShieldsButton;

	public static BaseManager instance;

	public Collider baseBounds;

	public Transform oxygenBar;

	public Transform powerBar;

	public Text oxygenInfo;

	public Text powerInfo;

	public Text distanceInfo;

	public Text shieldRepairCost;

	public Text powerGain;

	public Text powerLoss;

	public Text oxygenGain;

	public Text oxygenLoss;

	public Text minPower;

	public AudioClip hullBreach;

	public AudioClip zeroOxygen;

	public AudioClip shuttingDownDefense;

	public AudioClip PieHome;

	public AudioClip stationOxygenWarning;

	public AudioClip stationPowerWarning;

	public AudioClip holoShieldBrokenWarning;

	public AudioClip turretBrokenWarning;

	public AudioClip solarPanelBrokenWarning;

	public AudioClip magPushBrokenWarning;

	internal bool atBase;

	internal bool autoFillPower = true;

	internal bool oxygenWarned;

	public bool inBase;

	public bool autoFillOxygen = true;

	public bool defensesOn;

	public ToggleButton defenseButton;

	public OxygenGroup currentOxygenGroup;

	internal int coresInPlace;

	internal int nanoCapacity;

	private Transform player;

	public static int researchPoints;

	private void Awake()
	{
		instance = this;
		researchPoints = PreviewLabs.PlayerPrefs.GetInt("ResearchPoints");
	}

	private void Start()
	{
		player = GameManager.instance.Head;
		Invoke("StartRoutines", 0.5f);
	}

	public void OnSave()
	{
		if (!IntroManager.instance)
		{
			PreviewLabs.PlayerPrefs.SetInt("basePower", (int)power);
			PreviewLabs.PlayerPrefs.SetInt("ResearchPoints", researchPoints);
			PreviewLabs.PlayerPrefs.SetInt("Cores", coresInPlace);
			PreviewLabs.PlayerPrefs.SetBool("DefensesOn", defensesOn);
		}
	}

	public void OnLoad()
	{
		if (PreviewLabs.PlayerPrefs.HasKey("basePower"))
		{
			power = PreviewLabs.PlayerPrefs.GetInt("basePower");
		}
		if (PreviewLabs.PlayerPrefs.HasKey("DefensesOn"))
		{
			defensesOn = PreviewLabs.PlayerPrefs.GetBool("DefensesOn");
			if (!defensesOn)
			{
				defenseButton.TurnOff();
			}
		}
		else
		{
			defensesOn = true;
		}
		coresInPlace = PreviewLabs.PlayerPrefs.GetInt("Cores");
	}

	private void StartRoutines()
	{
		OnLoad();
		powerGain.text = defaultPowerGain + "/sec";
		StartCoroutine("CheckPower");
		StartCoroutine("UpdateUI");
		StartCoroutine("UpdateUISmooth");
	}

	public void UpdateOxygenUI(float oxygenLossValue, float oxygenGainValue)
	{
		if (oxygenLossValue > 19f && SuitManager.instance.oxygen < SuitManager.instance.maxOxygen - 3f)
		{
			oxygenLoss.text = " " + oxygenLossValue.ToString("F0");
		}
		else
		{
			oxygenLoss.text = " " + oxygenLossValue.ToString("F2") + "/sec";
		}
		oxygenGain.text = " " + oxygenGainValue.ToString("F2") + "/sec";
	}

	private IEnumerator CheckPower()
	{
		float waitTime = 0.05f;
		while (true)
		{
			int DrainCap = 5 * HoloShields.Count + 5 * Turrets.Count;
			float powerLossValue = 0f;
			if (SolarPanels.Count > 0)
			{
				foreach (SolarPanel solarPanel in SolarPanels)
				{
					if (power < maxPower)
					{
						power += solarPanel.powerDraw * waitTime;
					}
				}
				powerGain.text = (float)SolarPanels.Count * 0.25f + "/sec";
			}
			else
			{
				if (power < maxPower)
				{
					power += defaultPowerGain * waitTime;
				}
				powerGain.text = defaultPowerGain + "/sec";
			}
			if (defensesOn && power < (float)(DrainCap + 3))
			{
				defensesOn = false;
				GameAudioManager.instance.AddToSuitQueue(shuttingDownDefense);
				defenseButton.TurnOff();
			}
			int powerCost = 0;
			foreach (HoloShield holoShield in HoloShields)
			{
				powerCost += (4 - holoShield.health) * 5;
				if (power > (float)DrainCap)
				{
					if (holoShield.active && defensesOn)
					{
						power -= holoShield.powerDraw * waitTime;
					}
					if (!holoShield.active && defensesOn)
					{
						holoShield.OnActivate();
					}
				}
				else if (holoShield.active)
				{
					holoShield.OnDeactivate();
				}
				if (holoShield.active)
				{
					powerLossValue += holoShield.powerDraw * waitTime;
				}
			}
			if (powerCost > 0)
			{
				shieldRepairCost.text = powerCost.ToString();
				repairShieldsButton.SetActive(true);
			}
			else
			{
				repairShieldsButton.SetActive(false);
			}
			foreach (Turret turret in Turrets)
			{
				if (power >= (float)DrainCap)
				{
					if (turret.active && defensesOn)
					{
						power -= turret.powerDraw * waitTime;
					}
					if (!turret.active && defensesOn)
					{
						turret.OnActivate();
					}
				}
				else if (turret.active)
				{
					turret.OnDeactivate();
				}
				if (turret.active)
				{
					powerLossValue += turret.powerDraw * waitTime;
				}
			}
			minPower.text = DrainCap.ToString();
			if (inBase && autoFillPower && power > 1f && SuitManager.instance.power < SuitManager.instance.maxPower)
			{
				if (SuitManager.instance.power + waitTime * powerChargeSpeed > SuitManager.instance.maxPower)
				{
					SuitManager.instance.power = SuitManager.instance.maxPower;
				}
				else
				{
					SuitManager.instance.power += waitTime * powerChargeSpeed;
					power -= waitTime * powerChargeSpeed;
					powerLossValue += powerChargeSpeed * waitTime;
				}
			}
			powerLoss.text = (powerLossValue * 20f).ToString("F2") + "/sec";
			yield return new WaitForSeconds(waitTime);
		}
	}

	public void OnAutofillSuit(float waitTime, OxygenGroup group)
	{
		if (!inBase || !autoFillOxygen || !(group.TotalOxygen > 1f) || !(SuitManager.instance.oxygen + 2f < SuitManager.instance.maxOxygen))
		{
			return;
		}
		if (group.TotalOxygen > 0f)
		{
			if (group.TotalOxygen > 10f)
			{
				oxygenWarned = false;
			}
		}
		else if (!oxygenWarned)
		{
			GameAudioManager.instance.AddToSuitQueue(zeroOxygen);
			oxygenWarned = true;
		}
		group.TotalOxygen -= waitTime * oxygenChargeSpeed;
		if (SuitManager.instance.oxygen + waitTime * oxygenChargeSpeed > SuitManager.instance.maxOxygen)
		{
			SuitManager.instance.oxygen = SuitManager.instance.maxOxygen;
		}
		else
		{
			SuitManager.instance.oxygen += waitTime * oxygenChargeSpeed;
		}
	}

	private IEnumerator CheckIfInBase()
	{
		float interval = 0.1f;
		while (true)
		{
			if (!GameManager.instance.dead && baseBounds.bounds.Contains(player.position) && baseBounds.bounds.Contains(GunHolster.instance.transform.position))
			{
				Debug.Log("A");
				inBase = true;
			}
			else
			{
				inBase = false;
			}
			if (Vector3.Distance(player.position, base.transform.position) > 8f || player.position.y < -2f)
			{
				if (atBase)
				{
					atBase = false;
					ToggleScavangeMode();
				}
			}
			else if (!atBase)
			{
				atBase = true;
				ToggleScavangeMode();
			}
			yield return new WaitForSeconds(interval);
		}
	}

	private IEnumerator UpdateUISmooth()
	{
		float interval = 0.025f;
		while (true)
		{
			float distance = Vector3.Distance(base.transform.position, GameManager.instance.Head.position);
			if (distance > 10f)
			{
				distanceInfo.transform.parent.gameObject.SetActive(true);
				float num = Mathf.Clamp(distance / 8f, 1f, 8f);
				distanceInfo.rectTransform.localScale = new Vector3(0f - num, num, num);
				distanceInfo.transform.parent.LookAt(GameManager.instance.Head.position, Vector3.up);
				distanceInfo.text = "Distance \n" + distance.ToString("F0") + " M";
			}
			else
			{
				distanceInfo.transform.parent.gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(interval);
		}
	}

	private IEnumerator UpdateUI()
	{
		float interval = 0.05f;
		while (true)
		{
			float powerValue = power / maxPower;
			if (powerValue > 0f)
			{
				powerBar.localScale = new Vector3(powerBar.localScale.x, powerBar.localScale.y, powerValue);
			}
			else
			{
				powerBar.localScale = new Vector3(powerBar.localScale.x, powerBar.localScale.y, 1E-06f);
			}
			powerInfo.text = Mathf.Clamp(power, 0f, 100000f).ToString("F0");
			float oxygenValue = collectiveOxygen / collectiveOxygenCap;
			if (oxygenValue > 0f)
			{
				oxygenBar.localScale = new Vector3(oxygenBar.localScale.x, oxygenBar.localScale.y, oxygenValue);
			}
			else
			{
				oxygenBar.localScale = new Vector3(oxygenBar.localScale.x, oxygenBar.localScale.y, 1E-06f);
			}
			oxygenInfo.text = Mathf.Clamp(collectiveOxygen, 0f, 100000f).ToString("F0");
			if (collectiveOxygen < 2f && !IsInvoking("BaseOxygenWarning"))
			{
				Invoke("BaseOxygenWarning", 180f);
				GameAudioManager.instance.AddToSuitQueue(stationOxygenWarning);
			}
			if (power < 2f && !IsInvoking("BasePowerWarning"))
			{
				Invoke("BasePowerWarning", 180f);
				GameAudioManager.instance.AddToSuitQueue(stationPowerWarning);
			}
			yield return new WaitForSeconds(interval);
		}
	}

	public void ToggleShield(int index)
	{
		Debug.Log("kewler " + index);
	}

	public void ToggleOxygenCharge(ToggleButton button)
	{
		autoFillOxygen = button.On;
		BaseComputer.instance.Beep();
	}

	public void TogglePowerCharge(ToggleButton button)
	{
		autoFillPower = button.On;
		BaseComputer.instance.Beep();
	}

	public void OnAddCore()
	{
		coresInPlace++;
		if (coresInPlace >= 3)
		{
			GameAudioManager.instance.AddToSuitQueue(PieHome);
		}
	}

	public void ToggleDefenses(ToggleButton button)
	{
		if (button.On)
		{
			foreach (HoloShield holoShield in HoloShields)
			{
				holoShield.OnActivate();
			}
			foreach (Turret turret in Turrets)
			{
				turret.OnActivate();
			}
			defensesOn = true;
		}
		else
		{
			foreach (HoloShield holoShield2 in HoloShields)
			{
				holoShield2.OnDeactivate();
			}
			foreach (Turret turret2 in Turrets)
			{
				turret2.OnDeactivate();
			}
			defensesOn = false;
		}
		BaseComputer.instance.Beep();
	}

	public void HullBreachWarning()
	{
		GameAudioManager.instance.AddToSuitQueue(hullBreach);
	}

	private void ToggleScavangeMode()
	{
		if (atBase)
		{
			GameManager.instance.leftController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
			GameManager.instance.rightController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
			return;
		}
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().Toggle(false);
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(false);
		GameManager.instance.leftController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
	}

	public void OnExitBase()
	{
		currentOxygenGroup = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.tag == "Player" && inBase && GameAudioManager.instance.inSpace)
		{
			GameAudioManager.instance.OnNormal();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.root.tag == "Player" && !GameAudioManager.instance.inSpace)
		{
			GameAudioManager.instance.OnSpace();
		}
	}

	public void OnRepairShields()
	{
		int num = 0;
		foreach (HoloShield holoShield in HoloShields)
		{
			num += (4 - holoShield.health) * 5;
		}
		if (power > (float)num)
		{
			foreach (HoloShield holoShield2 in HoloShields)
			{
				holoShield2.OnRepair();
			}
			power -= num;
		}
		else
		{
			SuitManager.instance.LowPowerPrompt();
		}
		BaseComputer.instance.Beep();
	}

	internal void UpgradeNanoStorage()
	{
		nanoCapacity++;
		for (int i = 0; i < NanoStorage.nanoStorages.Count; i++)
		{
			NanoStorage.nanoStorages[i].nanoCap += 15;
			NanoStorage.nanoStorages[i].UpdateUI();
		}
	}

	private void BaseOxygenWarning()
	{
	}

	private void BasePowerWarning()
	{
	}
}
