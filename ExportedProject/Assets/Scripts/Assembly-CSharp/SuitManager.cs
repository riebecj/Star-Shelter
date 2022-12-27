using System;
using System.Collections;
using PreviewLabs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK;

public class SuitManager : MonoBehaviour
{
	public float maxOxygen = 200f;

	public float oxygen = 200f;

	public float maxPower = 100f;

	public float power = 100f;

	public float health = 100f;

	public float maxHealth = 100f;

	public float nutrition = 100f;

	public float maxNutrition = 100f;

	public float radiationResistance;

	public float maxRadiationResistance = 50f;

	public float radiation;

	internal float warningCooldown = 20f;

	public Transform HandUI;

	public Transform powerCan;

	public Transform oxygenCan;

	public Image oxygenRing;

	public Image powerRing;

	public Image healthRing;

	public Image radiationRing;

	public Image nutritionRing;

	public Image maskOxygen;

	public Image radiationIcon;

	public Animator oxygenWarning;

	public Animator blinkTrigger;

	public AudioSource audioSource;

	public AudioSource chokeAudio;

	public AudioSource powerGain;

	public AudioSource oxygenGain;

	public AudioSource canClick;

	public static SuitManager instance;

	public Text survivalTime;

	public Text[] survivalNumbers;

	public AudioClip[] oxygenWarningClips;

	public AudioClip[] nutritionWarningClips;

	public AudioClip[] powerWarningClips;

	public AudioClip[] cometWarningClips;

	public AudioClip[] bodyImpacts;

	public AudioClip healthTickAudio;

	public AudioClip choke;

	public AudioClip introDialugue;

	public AudioClip inhale;

	public AudioClip notEnoughMaterials;

	public AudioClip cometAverted;

	public AudioClip radiationDangerous;

	public AudioClip radiationSafe;

	public AudioClip cometShower;

	public AudioClip droneWarning;

	public AudioClip nanoInventoryFull;

	public AudioClip GunInfo;

	public AudioClip plantDeath;

	public GameObject damageVignette;

	public GameObject shotDamageAudio;

	private int oxygenWarningIndex;

	private int nutritionWarningIndex;

	private int powerWarningIndex;

	public int RadiationResistance;

	internal bool oxygen50;

	internal bool oxygen25;

	internal bool oxygenEmpty;

	internal bool nutrition50;

	internal bool nutrition25;

	internal bool nutritionEmpty;

	internal bool canBreath;

	internal bool inOxygenZone;

	internal bool inRadiationZone;

	internal int PowerCapacity;

	internal int OxygenCapacity;

	internal int ThrusterSpeed;

	private Vector3 sunPos;

	internal int causeOfDeathIndex;

	public string[] causeOfDeath;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (Application.loadedLevelName == "MainScene")
		{
			OnLoad();
			Invoke("OnStart", 0.1f);
		}
		oxygenWarning.SetBool("Oxygen", false);
	}

	public void OnSave()
	{
		if (!IntroManager.instance)
		{
			PreviewLabs.PlayerPrefs.SetInt("suitOxygen", (int)oxygen);
			PreviewLabs.PlayerPrefs.SetInt("suitPower", (int)power);
			if (health > 0f)
			{
				PreviewLabs.PlayerPrefs.SetInt("suitHealth", (int)health);
			}
			PreviewLabs.PlayerPrefs.SetInt("suitNutrition", (int)nutrition);
			PreviewLabs.PlayerPrefs.SetInt("suitRadiation", (int)radiation);
			PreviewLabs.PlayerPrefs.SetInt("suitRadiationResistance", (int)radiationResistance);
			PreviewLabs.PlayerPrefs.SetInt("PowerCapacity", instance.PowerCapacity);
			PreviewLabs.PlayerPrefs.SetInt("OxygenCapacity", instance.OxygenCapacity);
			PreviewLabs.PlayerPrefs.SetInt("ThrusterSpeed", instance.ThrusterSpeed);
		}
	}

	public void OnLoad()
	{
		if (PreviewLabs.PlayerPrefs.HasKey("suitOxygen"))
		{
			oxygen = PreviewLabs.PlayerPrefs.GetInt("suitOxygen");
			power = PreviewLabs.PlayerPrefs.GetInt("suitPower");
			health = PreviewLabs.PlayerPrefs.GetInt("suitHealth");
			nutrition = PreviewLabs.PlayerPrefs.GetInt("suitNutrition");
			radiation = PreviewLabs.PlayerPrefs.GetInt("suitRadiation");
			radiationResistance = PreviewLabs.PlayerPrefs.GetInt("suitRadiationResistance");
			radiationResistance = Mathf.Clamp(radiationResistance, 0f, 300f);
		}
		int num = PreviewLabs.PlayerPrefs.GetInt("PowerCapacity");
		if (Mathf.Abs(num) > 10)
		{
			num = 1;
		}
		for (int i = 0; i < num; i++)
		{
			instance.UpgradePowerCapacity();
		}
		int num2 = PreviewLabs.PlayerPrefs.GetInt("OxygenCapacity");
		if (Mathf.Abs(num2) > 10)
		{
			num2 = 1;
		}
		for (int j = 0; j < num2; j++)
		{
			instance.UpgradeOxygenCapacity();
		}
		int num3 = PreviewLabs.PlayerPrefs.GetInt("ThrusterSpeed");
		if (Mathf.Abs(num3) > 10)
		{
			num3 = 1;
		}
		for (int k = 0; k < num3; k++)
		{
			instance.UpgradeThrusterSpeed();
		}
		if (radiation < 1f)
		{
			radiationRing.fillAmount = 0f;
			radiationIcon.gameObject.SetActive(false);
		}
	}

	public void OnStart()
	{
		StartCoroutine("UpdateState", 0.1f);
		sunPos = new Vector3(3500f, 0f, 0f);
		if (!PreviewLabs.PlayerPrefs.GetBool("GameStarted"))
		{
			SpaceMask.instance.UI.SetBool("Skip", false);
			SpaceMask.instance.lerp = true;
			blinkTrigger.SetBool("Wakeup", true);
			Invoke("PlayIntro", 1f);
			foreach (Thruster thruster in Thruster.thrusters)
			{
				thruster.deactivated = true;
			}
		}
		else
		{
			SpaceMask.instance.UI.SetBool("Skip", true);
			canBreath = false;
		}
		if (radiation > 0f)
		{
			StartCoroutine("AfterGlow");
		}
	}

	public void PlayIntro()
	{
		CryoPodLever.instance.interact.isGrabbable = false;
		blinkTrigger.SetBool("Wakeup", false);
		audioSource.clip = introDialugue;
		audioSource.Play();
		Invoke("EndIntro", introDialugue.length);
	}

	private void EndIntro()
	{
		audioSource.clip = null;
		CryoPodLever.instance.interact.isGrabbable = true;
		SpaceMask.instance.lerp = false;
		SpaceMask.instance.UI.SetBool("Skip", true);
		TutorialManager.instance.ToggleGrab();
	}

	private IEnumerator UpdateState(float waitTime)
	{
		while (true)
		{
			if (GameManager.instance.creativeMode)
			{
				power = maxPower;
				health = maxHealth;
				oxygen = maxOxygen;
				nutrition = maxNutrition;
				yield return new WaitForSeconds(waitTime);
			}
			if (oxygen > 0f)
			{
				if ((BaseManager.instance.currentOxygenGroup == null || BaseManager.instance.currentOxygenGroup.TotalOxygen < 1f) && !GameManager.instance.debugMode && !IntroManager.instance && !inOxygenZone)
				{
					oxygen -= waitTime * 0.3f;
				}
				if (chokeAudio.isPlaying && chokeAudio.clip == choke)
				{
					chokeAudio.Stop();
					chokeAudio.clip = null;
					if (!SpaceMask.instance.open)
					{
						SpaceMask.instance.breathAudio.Play();
					}
				}
				if (!chokeAudio.isPlaying && oxygen < 3f && !IsInvoking("InhaleCoodown"))
				{
					chokeAudio.PlayOneShot(inhale);
					Invoke("InhaleCoodown", 10f);
				}
			}
			else if (!BaseManager.instance.currentOxygenGroup || !(BaseManager.instance.currentOxygenGroup.TotalOxygen > 1f) || !SpaceMask.instance.open)
			{
				if (chokeAudio.clip == null && !chokeAudio.isPlaying)
				{
					chokeAudio.clip = choke;
					chokeAudio.Play();
					SpaceMask.instance.breathAudio.Stop();
				}
				if (health > 0f)
				{
					HealthTick(waitTime * 4f);
				}
				else
				{
					causeOfDeathIndex = 0;
					OnDeath();
				}
			}
			if (nutrition > 0f)
			{
				nutrition -= 0.06f * waitTime;
			}
			else if (health > 0f)
			{
				HealthTick(waitTime * 2f);
			}
			else
			{
				causeOfDeathIndex = 1;
				OnDeath();
			}
			if (health < maxHealth && !GameManager.instance.dead)
			{
				if (nutrition > 1f && oxygen > 1f)
				{
					blinkTrigger.SetFloat("Eyes", 0f);
				}
				else
				{
					float value = 1f - health / maxHealth * 2f;
					blinkTrigger.SetFloat("Eyes", value);
				}
			}
			else
			{
				blinkTrigger.SetFloat("Eyes", 0f);
			}
			if (power < maxPower && !Physics.Raycast(GameManager.instance.Head.position, sunPos - GameManager.instance.Head.position, 20f))
			{
				power += 0.25f * waitTime;
			}
			if (!canBreath && !GameManager.instance.dead && (((bool)BaseManager.instance.currentOxygenGroup && BaseManager.instance.currentOxygenGroup.TotalOxygen > 5f) || inOxygenZone))
			{
				canBreath = true;
				oxygenWarning.SetBool("Oxygen", true);
			}
			else if (canBreath && (BaseManager.instance.currentOxygenGroup == null || ((bool)BaseManager.instance.currentOxygenGroup && BaseManager.instance.currentOxygenGroup.TotalOxygen < 1f)) && !inOxygenZone)
			{
				canBreath = false;
				oxygenWarning.SetBool("Oxygen", false);
			}
			UpdateUI();
			CheckVitals();
			Cursor.visible = false;
			yield return new WaitForSeconds(waitTime);
		}
	}

	private IEnumerator AfterGlow()
	{
		float refreshRate = 0.3f;
		while (radiation > 0f && !GameManager.instance.dead)
		{
			radiation -= 0.4f;
			yield return new WaitForSeconds(refreshRate);
		}
	}

	public void UpdateUI()
	{
		float num = oxygen / maxOxygen;
		oxygenRing.fillAmount = num;
		survivalNumbers[0].text = oxygen.ToString("F0") + "<color=white>/</color>" + maxOxygen.ToString("F0");
		oxygenCan.localScale = new Vector3(1f, 1f, num);
		float num2 = power / maxPower;
		powerRing.fillAmount = num2;
		survivalNumbers[1].text = power.ToString("F0") + "<color=white>/</color>" + maxPower.ToString("F0");
		powerCan.localScale = new Vector3(1f, 1f, num2);
		float fillAmount = nutrition / maxNutrition;
		nutritionRing.fillAmount = fillAmount;
		survivalNumbers[3].text = nutrition.ToString("F0") + "<color=white>/</color>" + maxNutrition.ToString("F0");
		float fillAmount2 = health / maxHealth;
		healthRing.fillAmount = fillAmount2;
		survivalNumbers[2].text = health.ToString("F0") + "<color=white>/</color>" + maxHealth.ToString("F0");
		if (radiation > 0f || radiationIcon.gameObject.activeSelf)
		{
			float fillAmount3 = radiation / 180f;
			radiationRing.fillAmount = fillAmount3;
			if (radiation < 1f)
			{
				radiationIcon.gameObject.SetActive(false);
			}
			else
			{
				radiationIcon.gameObject.SetActive(true);
			}
		}
	}

	public void DrainOxygen(float speed)
	{
		oxygen -= speed * Time.deltaTime;
	}

	public void OnTakeDamage(int damage, int deathCause)
	{
		if (GameManager.instance.creativeMode)
		{
			return;
		}
		health -= damage;
		if (health <= 0f && !GameManager.instance.debugMode)
		{
			causeOfDeathIndex = deathCause;
			OnDeath();
			if ((bool)StatManager.instance)
			{
				StatManager.instance.OnDeath(deathCause);
			}
		}
		if (!IsInvoking("DisableDamageVignette"))
		{
			SpaceMask.instance.UI.SetBool("Damage", true);
			damageVignette.SetActive(true);
			Invoke("DisableDamageVignette", 0.5f);
		}
	}

	private void DisableDamageVignette()
	{
		damageVignette.SetActive(false);
		SpaceMask.instance.UI.SetBool("Damage", false);
	}

	public void OnDeath()
	{
		if (!GameManager.instance.dead && !GameManager.instance.creativeMode)
		{
			if (GameManager.instance.DemoBuild)
			{
				PreviewLabs.PlayerPrefs.DeleteAll();
				PreviewLabs.PlayerPrefs.Flush();
				SceneManager.LoadScene("TutorialScene");
			}
			if (DroneHelper.instance.VRControlled)
			{
				DroneArmUIManager.instance.SwapToDrone(false);
			}
			DroneHelper.instance.ReturnToBase();
			GameManager.instance.dead = true;
			StopCoroutine("UpdateState");
			blinkTrigger.SetFloat("Eyes", 0f);
			chokeAudio.Stop();
			audioSource.Stop();
			survivalTime.transform.parent.gameObject.SetActive(true);
			TimeSpan timeSpan = TimeSpan.FromSeconds(Time.timeSinceLevelLoad + (float)GameManager.instance.previousTimePlayed);
			string text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			survivalTime.text = "Time Played :\n" + text;
			Text text2 = survivalTime;
			text2.text = text2.text + "\n Cause of Death: " + causeOfDeath[causeOfDeathIndex];
			GameManager.instance.leftController.GetComponent<VRTK_InteractGrab>().ForceRelease();
			GameManager.instance.rightController.GetComponent<VRTK_InteractGrab>().ForceRelease();
			if ((bool)FuelSnapPoint.instance.target)
			{
				UnityEngine.Object.Destroy(FuelSnapPoint.instance.target);
			}
			GameManager.instance.inTitanEvent = false;
			GameAudioManager.instance.suitQueue.Clear();
			instance.inRadiationZone = false;
			RadiationUI.instance.UpdateState(false);
			RadiationUI.instance.damageWarningLabel.SetActive(false);
			instance.RadiationWarning(false);
			instance.inOxygenZone = false;
			canBreath = false;
			oxygenWarning.SetBool("Oxygen", false);
			StatManager.instance.playerDeaths++;
			Gun.instance.basicAmmo = 4;
			Gun.instance.ClearModules();
			GameManager.instance.Invoke("OnDeathReload", 6f);
		}
	}

	private void CheckVitals()
	{
		if ((!IsInvoking("OxygenWarningCooldown") || oxygen < 1f) && oxygen < maxOxygen / 1.25f)
		{
			if (!audioSource.isPlaying)
			{
				if (oxygen < maxOxygen / 2f && oxygen > maxOxygen / 4f)
				{
					if (!oxygen50)
					{
						audioSource.PlayOneShot(oxygenWarningClips[0]);
						oxygen50 = true;
						Invoke("OxygenWarningCooldown", warningCooldown);
					}
				}
				else if (oxygen < maxOxygen / 4f && oxygen > 5f)
				{
					if (!oxygen25)
					{
						audioSource.PlayOneShot(oxygenWarningClips[1]);
						oxygen25 = true;
						Invoke("OxygenWarningCooldown", warningCooldown);
						oxygenRing.transform.parent.GetComponent<Animator>().SetBool("Almost", true);
					}
				}
				else if (oxygen < 5f && !oxygenEmpty)
				{
					audioSource.PlayOneShot(oxygenWarningClips[2]);
					oxygenEmpty = true;
					Invoke("OxygenWarningCooldown", warningCooldown);
					oxygenRing.transform.parent.GetComponent<Animator>().SetBool("Empty", true);
				}
			}
			if (oxygen > 5f)
			{
				oxygenEmpty = false;
				oxygenRing.transform.parent.GetComponent<Animator>().SetBool("Empty", false);
			}
			if (oxygen > maxOxygen / 4f)
			{
				oxygen25 = false;
				oxygenRing.transform.parent.GetComponent<Animator>().SetBool("Almost", false);
			}
			if (oxygen > maxOxygen / 2f)
			{
				oxygen50 = false;
			}
		}
		if ((IsInvoking("NutritionWarningCooldown") && !(nutrition < 1f)) || !(nutrition < maxNutrition / 1.25f))
		{
			return;
		}
		if (!audioSource.isPlaying)
		{
			if (nutrition < maxNutrition / 2f && nutrition > maxNutrition / 4f)
			{
				if (!nutrition50)
				{
					audioSource.PlayOneShot(nutritionWarningClips[0]);
					nutrition50 = true;
					Invoke("NutritionWarningCooldown", warningCooldown);
				}
			}
			else if (nutrition < maxNutrition / 4f && nutrition > 1f)
			{
				if (!nutrition25)
				{
					audioSource.PlayOneShot(nutritionWarningClips[1]);
					nutrition25 = true;
					Invoke("NutritionWarningCooldown", warningCooldown);
					nutritionRing.transform.parent.GetComponent<Animator>().SetBool("Almost", true);
				}
			}
			else if (nutrition < 1f && !nutritionEmpty)
			{
				audioSource.PlayOneShot(nutritionWarningClips[2]);
				nutritionEmpty = true;
				Invoke("NutritionWarningCooldown", warningCooldown);
				nutritionRing.transform.parent.GetComponent<Animator>().SetBool("Empty", true);
			}
		}
		if (nutrition > 5f)
		{
			nutritionEmpty = false;
			nutritionRing.transform.parent.GetComponent<Animator>().SetBool("Empty", false);
		}
		if (nutrition > maxNutrition / 4f)
		{
			nutrition25 = false;
			nutritionRing.transform.parent.GetComponent<Animator>().SetBool("Almost", false);
		}
		if (nutrition > maxNutrition / 2f)
		{
			nutrition50 = false;
		}
	}

	public void LowPowerPrompt()
	{
		if (!IsInvoking("PromtCooldown"))
		{
			audioSource.PlayOneShot(powerWarningClips[powerWarningIndex]);
			powerWarningIndex++;
			if (powerWarningIndex > powerWarningClips.Length - 1)
			{
				powerWarningIndex = 0;
			}
			Invoke("PromtCooldown", 1.5f);
		}
	}

	public void LowResourcePrompt()
	{
		if (!IsInvoking("PromtCooldown"))
		{
			audioSource.PlayOneShot(notEnoughMaterials);
			Invoke("PromtCooldown", 1.5f);
		}
	}

	public void InventoryFull()
	{
		if (!IsInvoking("PromtCooldown"))
		{
			audioSource.PlayOneShot(nanoInventoryFull);
			Invoke("PromtCooldown", 1.5f);
		}
	}

	public void CometPromt(int index)
	{
		if (!IsInvoking("CometPromtCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(cometWarningClips[index]);
			Invoke("CometPromtCooldown", 5f);
		}
	}

	public void AvertedCometPromt()
	{
		if (!IsInvoking("CometPromtCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(cometAverted);
			Invoke("CometPromtCooldown", 5f);
		}
	}

	public void DronePromt()
	{
		GameAudioManager.instance.AddToSuitQueue(droneWarning);
	}

	public void CometShowerPromt()
	{
		GameAudioManager.instance.AddToSuitQueue(cometShower);
	}

	public void HealthTick(float damage)
	{
		health -= damage;
		if (!IsInvoking("TickCooldown"))
		{
			audioSource.PlayOneShot(healthTickAudio);
			Invoke("TickCooldown", 2f);
		}
	}

	public void OnBodyImpact(int damage)
	{
		audioSource.PlayOneShot(bodyImpacts[UnityEngine.Random.Range(0, 3)]);
		OnTakeDamage(damage, 8);
	}

	public void ToggleMaskOxygen()
	{
		if ((bool)maskOxygen)
		{
			maskOxygen.transform.parent.parent.gameObject.SetActive(true);
			maskOxygen.fillAmount = oxygen / maxOxygen;
			CancelInvoke("DisableMaskOxygen");
			Invoke("DisableMaskOxygen", 1.5f);
		}
	}

	public void RadiationWarning(bool value)
	{
		if (value)
		{
			GameAudioManager.instance.AddToSuitQueue(radiationDangerous);
		}
		else
		{
			GameAudioManager.instance.AddToSuitQueue(radiationSafe);
		}
	}

	private void DisableMaskOxygen()
	{
		maskOxygen.transform.parent.parent.gameObject.SetActive(false);
	}

	internal void UpgradePowerCapacity()
	{
		PowerCapacity++;
		instance.maxPower += 25f;
	}

	internal void UpgradeOxygenCapacity()
	{
		OxygenCapacity++;
		instance.maxOxygen += 25f;
	}

	internal void UpgradeThrusterSpeed()
	{
		ThrusterSpeed++;
		foreach (Thruster thruster in Thruster.thrusters)
		{
			thruster.maxSpeed += 1f;
			thruster.oxygenCost -= 0.25f;
		}
	}

	internal void UpgradeRadiationResistance()
	{
		RadiationResistance++;
	}

	internal void ResetUpgrades()
	{
		maxPower = 50f;
		maxHealth = 100f;
		maxNutrition = 100f;
		maxOxygen = 200f;
		oxygen = maxOxygen;
		power = maxPower;
		health = maxHealth;
		nutrition = maxNutrition;
		foreach (Thruster thruster in Thruster.thrusters)
		{
			thruster.maxSpeed = 8f;
			thruster.oxygenCost = 2f;
		}
		PowerCapacity = 0;
		OxygenCapacity = 0;
		ThrusterSpeed = 0;
	}

	private void TickCooldown()
	{
	}

	private void PromtCooldown()
	{
	}

	private void CometPromtCooldown()
	{
	}

	private void WarningCooldown()
	{
	}

	private void NutritionWarningCooldown()
	{
	}

	private void OxygenWarningCooldown()
	{
	}

	private void InhaleCoodown()
	{
	}
}
