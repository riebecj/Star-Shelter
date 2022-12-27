using System.Collections;
using PreviewLabs;
using UnityEngine;

public class HintManager : MonoBehaviour
{
	public AudioClip thrusterUsageAudio;

	public AudioClip inventorySavingAudio;

	public AudioClip lowOxygen_IntactShipAudio;

	public AudioClip lowOxygen_ShipAudio;

	public AudioClip lowOxygen_GrowPlantAudio;

	public AudioClip lowFoodAudio;

	public AudioClip sleep1Audio;

	public AudioClip sleep2Audio;

	public AudioClip gunModsAudio;

	public AudioClip prepareDefensesAudio;

	internal bool thrusterUsage;

	internal bool inventorySaving;

	internal bool lowOxygen_IntactShip;

	internal bool lowOxygen_Ship;

	internal bool lowOxygen_GrowPlant;

	internal bool lowFood;

	internal bool sleep1;

	internal bool sleep2;

	internal bool gunMods;

	internal bool prepareDefenses;

	private float thrusterDuration;

	private int inventorySavingCount;

	private int lowOxygen_IntactShipCount;

	private int lowOxygen_ShipCount;

	private int lowOxygen_GrowPlantCount;

	private int lowFoodCount;

	private int sleep1Count;

	private int sleep2Count;

	private int gunModsCount;

	private int prepareDefensesCount;

	public static HintManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		LoadState();
		StartCoroutine("UpdateState");
	}

	private void LoadState()
	{
		thrusterUsage = PreviewLabs.PlayerPrefs.GetBool("thrusterUsage");
		inventorySaving = PreviewLabs.PlayerPrefs.GetBool("inventorySaving");
		lowOxygen_IntactShip = PreviewLabs.PlayerPrefs.GetBool("lowOxygen_IntactShip");
		lowOxygen_Ship = PreviewLabs.PlayerPrefs.GetBool("lowOxygen_Ship");
		lowOxygen_GrowPlant = PreviewLabs.PlayerPrefs.GetBool("lowOxygen_GrowPlant");
		lowFood = PreviewLabs.PlayerPrefs.GetBool("lowFood");
		sleep1 = PreviewLabs.PlayerPrefs.GetBool("sleep1");
		sleep2 = PreviewLabs.PlayerPrefs.GetBool("sleep2");
		gunMods = PreviewLabs.PlayerPrefs.GetBool("gunMods");
		prepareDefenses = PreviewLabs.PlayerPrefs.GetBool("prepareDefenses");
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			DoChecks();
			yield return new WaitForSeconds(5f);
		}
	}

	private void DoChecks()
	{
		if (!inventorySaving && SuitManager.instance.health < SuitManager.instance.maxHealth * 0.75f && !IsInvoking("InventorySavingCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(inventorySavingAudio);
			inventorySavingCount++;
			if (inventorySavingCount < 3)
			{
				Invoke("InventorySavingCooldown", 1200f);
				return;
			}
			PreviewLabs.PlayerPrefs.SetBool("inventorySaving", true);
			inventorySaving = true;
		}
		else if (!lowOxygen_IntactShip && SuitManager.instance.oxygen < SuitManager.instance.maxOxygen * 0.5f && !IsInvoking("LowOxygenCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(lowOxygen_IntactShipAudio);
			lowOxygen_IntactShipCount++;
			if (lowOxygen_IntactShipCount < 2)
			{
				Invoke("LowOxygenCooldown", 1200f);
				return;
			}
			PreviewLabs.PlayerPrefs.SetBool("lowOxygen_IntactShip", true);
			lowOxygen_IntactShip = true;
		}
		else if (!lowOxygen_Ship && SuitManager.instance.oxygen < SuitManager.instance.maxOxygen * 0.5f && !IsInvoking("LowOxygenCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(lowOxygen_ShipAudio);
			lowOxygen_ShipCount++;
			if (lowOxygen_ShipCount < 2)
			{
				Invoke("LowOxygenCooldown", 1200f);
				return;
			}
			PreviewLabs.PlayerPrefs.SetBool("lowOxygen_Ship", true);
			lowOxygen_Ship = true;
		}
		else if (!lowOxygen_GrowPlant && SuitManager.instance.oxygen < SuitManager.instance.maxOxygen * 0.5f && !IsInvoking("LowOxygenCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(lowOxygen_GrowPlantAudio);
			lowOxygen_GrowPlantCount++;
			if (lowOxygen_GrowPlantCount < 2)
			{
				Invoke("LowOxygenCooldown", 1200f);
				return;
			}
			PreviewLabs.PlayerPrefs.SetBool("lowOxygen_GrowPlant", true);
			lowOxygen_GrowPlant = true;
		}
		else if (!lowFood && SuitManager.instance.nutrition < SuitManager.instance.maxNutrition * 0.5f && !IsInvoking("LowFoodCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(lowFoodAudio);
			lowFoodCount++;
			if (lowFoodCount < 3)
			{
				Invoke("LowFoodCooldown", 1200f);
				return;
			}
			PreviewLabs.PlayerPrefs.SetBool("lowFood", true);
			lowFood = true;
		}
		else if (!sleep1 && GameManager.instance.timePlayed > 2000f && !IsInvoking("SleepCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(sleep1Audio);
			sleep1Count++;
			if (sleep1Count < 3)
			{
				Invoke("SleepCooldown", 1200f);
				return;
			}
			PreviewLabs.PlayerPrefs.SetBool("sleep1", true);
			sleep1 = true;
		}
		else if (!sleep2 && GameManager.instance.timePlayed > 2000f && !IsInvoking("SleepCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(sleep2Audio);
			sleep2Count++;
			if (sleep2Count > 3)
			{
				Invoke("SleepCooldown", 1200f);
				return;
			}
			PreviewLabs.PlayerPrefs.SetBool("sleep2", true);
			sleep2 = true;
		}
	}

	public void ThrusterCheck()
	{
		if (!thrusterUsage)
		{
			thrusterDuration += Time.deltaTime;
			if (thrusterDuration > 60f)
			{
				GameAudioManager.instance.AddToSuitQueue(thrusterUsageAudio);
				PreviewLabs.PlayerPrefs.SetBool("thrusterUsage", true);
				thrusterUsage = true;
			}
		}
	}

	public void GunModsCheck()
	{
		if (!gunMods && !IsInvoking("GunModsCooldown"))
		{
			GameAudioManager.instance.AddToSuitQueue(gunModsAudio);
			gunMods = true;
			gunModsCount++;
			if (gunModsCount < 3)
			{
				Invoke("GunModsCooldown", 1200f);
			}
			else
			{
				PreviewLabs.PlayerPrefs.SetBool("gunMods", true);
			}
		}
	}

	public void PrepareDefensesCheck()
	{
		if (!prepareDefenses && !IsInvoking("PrepareDefensesCount"))
		{
			GameAudioManager.instance.AddToSuitQueue(prepareDefensesAudio);
			PreviewLabs.PlayerPrefs.SetBool("prepareDefenses", true);
			prepareDefenses = true;
			prepareDefensesCount++;
			if (prepareDefensesCount < 3)
			{
				Invoke("PrepareDefensesCount", 1200f);
			}
		}
	}

	private void InventorySavingCooldown()
	{
	}

	private void LowOxygen_IntactShipCooldown()
	{
	}

	private void LowOxygenCooldown()
	{
	}

	private void LowFoodCooldown()
	{
	}

	private void SleepCooldown()
	{
	}

	private void GunModsCooldown()
	{
	}

	private void PrepareDefensesCount()
	{
	}
}
