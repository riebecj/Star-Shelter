using System.Collections;
using System.Diagnostics;
using PreviewLabs;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class GameManager : MonoBehaviour
{
	public delegate void DeathDelegate();

	public static GameManager instance;

	public Transform CamRig;

	public Transform Head;

	public Transform leftController;

	public Transform rightController;

	public Transform stationObjects;

	public Transform[] CoreSpawnPosition;

	public GameObject NodeProxy;

	public GameObject buildMarkerInterior;

	public GameObject buildMarkerExterior;

	public GameObject airleakParticle;

	public GameObject componentUI;

	public GameObject leakPromt;

	public GameObject fpsCounter;

	public GameObject cryoPod;

	public GameObject notePad;

	public GameObject cometLine;

	public GameObject mapCometLine;

	public GameObject impactParticle;

	public GameObject gunModuleUI;

	public GameObject Core;

	public GameObject cometImpactParticle;

	public GameObject breakPraticle;

	public Material atlasMaterial;

	public Material craftMaterial;

	public Material outlineMaterial;

	public Material plantDeadMaterial;

	public Light sunLight;

	internal bool isOculus;

	internal bool isCosmos;

	internal bool inMenu;

	internal bool dead;

	internal bool skipLogo;

	internal bool inTitanEvent;

	internal bool castShadows = true;

	public bool debugMode;

	public bool deleteSaves;

	public bool debugComets;

	public bool dontSave;

	public bool DemoBuild;

	public bool creativeMode;

	public bool spawnOutside;

	internal bool loading;

	internal float timePlayed;

	internal int previousTimePlayed;

	internal int saveslot;

	internal int looseCores;

	public Material lightOn;

	public Material lightOff;

	public Material lootDeteriorate;

	public Material CometLine;

	public LayerMask physical;

	public Stopwatch timer = new Stopwatch();

	public DeathDelegate deathDelegate;

	private void Awake()
	{
		instance = this;
		if (VRTK_DeviceFinder.GetHeadsetType() == VRTK_DeviceFinder.Headsets.OculusRiftCV1)
		{
			isOculus = true;
		}
		else if (VRTK_DeviceFinder.GetHeadsetType() == VRTK_DeviceFinder.Headsets.Unknown)
		{
			isCosmos = true;
		}
		loading = true;
		Invoke("Loaded", 1f);
		creativeMode = PreviewLabs.PlayerPrefs.GetBool("CreativeMode" + PreviewLabs.PlayerPrefs.saveSlot);
		previousTimePlayed = PreviewLabs.PlayerPrefs.GetInt("PreviousTimePlayed");
		timer.Start();
	}

	private void Start()
	{
		StartCoroutine("UpdateUIs");
		if (Application.loadedLevelName == "MainScene")
		{
			OnStartMainGame();
		}
	}

	public void OnStartMainGame()
	{
		Inventory.instance.Invoke("OnLoad", 0.25f);
		if (debugMode)
		{
			CryoPodLever.instance.OnOpen();
			StartCapsule.instance.gameObject.SetActive(false);
			CryoPodLever.instance.open = true;
		}
		if (PreviewLabs.PlayerPrefs.GetBool("GameStarted"))
		{
			CamRig.transform.position = new Vector3(0f, -1f, 0f);
			GameAudioManager.instance.OnNormal();
		}
		else
		{
			SpawnPlayer();
		}
		looseCores = PreviewLabs.PlayerPrefs.GetInt("LooseCores");
		for (int i = 0; i < looseCores; i++)
		{
			Object.Instantiate(Core, CoreSpawnPosition[i].position, Quaternion.identity);
		}
		inTitanEvent = PreviewLabs.PlayerPrefs.GetBool("InTitanEvent");
		if (spawnOutside)
		{
			CamRig.transform.position = new Vector3(15f, 2f, 0f);
		}
	}

	private void SpawnPlayer()
	{
		Vector3 vector = Vector3.zero;
		int i = 0;
		while (vector == Vector3.zero)
		{
			for (; Physics.OverlapSphere(new Vector3(22 + i, Random.Range(-5, 5), Random.Range(-20, 20)), 1f).Length > 0; i += 5)
			{
			}
			vector = new Vector3(22 + i, 0f, 0f);
		}
		CamRig.transform.position = vector;
	}

	private void Loaded()
	{
		loading = false;
		if (!PreviewLabs.PlayerPrefs.GetBool("GameStarted") && Application.loadedLevelName == "MainScene")
		{
			OxygenGroup group = BaseManager.instance.GetComponent<Room>().group;
			group.TotalOxygen += 50f;
		}
	}

	private void OnApplicationQuit()
	{
		if (!dontSave)
		{
			OnSave();
		}
	}

	public void OnSave()
	{
		if (DemoBuild)
		{
			return;
		}
		if (Application.loadedLevelName == "MainScene")
		{
			PreviewLabs.PlayerPrefs.SetInt("PreviousTimePlayed", (int)Time.timeSinceLevelLoad + previousTimePlayed);
			PreviewLabs.PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
			if (PreviewLabs.PlayerPrefs.GetBool("OnGrab"))
			{
				PreviewLabs.PlayerPrefs.SetBool("GameStarted", true);
			}
			if ((bool)Inventory.instance)
			{
				Inventory.instance.OnSave();
			}
			PreviewLabs.PlayerPrefs.SetBool("InTitanEvent", inTitanEvent);
			for (int i = 0; i < UpgradeManager.upgradeManagers.Count; i++)
			{
				UpgradeManager.upgradeManagers[i].SaveUpgrades();
			}
			UpgradeManager.upgradeManagers.Clear();
			UpgradeManager.Unload();
			for (int j = 0; j < StorageBox.storageBoxes.Count; j++)
			{
				StorageBox.storageBoxes[j].OnSave();
			}
			StorageBox.storageBoxes.Clear();
			for (int k = 0; k < SeedSlot.seedSlots.Count; k++)
			{
				SeedSlot.seedSlots[k].OnSave();
			}
			SeedSlot.seedSlots.Clear();
			for (int l = 0; l < NanoStorage.nanoStorages.Count; l++)
			{
				NanoStorage.nanoStorages[l].OnSave();
			}
			NanoStorage.nanoStorages.Clear();
			for (int m = 0; m < Pressurizer.pressurizers.Count; m++)
			{
				Pressurizer.pressurizers[m].OnSave();
			}
			Pressurizer.pressurizers.Clear();
			if ((bool)NanoInventory.instance)
			{
				NanoInventory.instance.OnSave();
			}
			if ((bool)BaseLoader.instance)
			{
				BaseLoader.instance.OnSave();
			}
			if ((bool)CometManager.instance)
			{
				CometManager.instance.OnSave();
			}
			if ((bool)SuitManager.instance)
			{
				SuitManager.instance.OnSave();
			}
			if ((bool)BaseManager.instance)
			{
				BaseManager.instance.OnSave();
			}
			if ((bool)StatManager.instance)
			{
				StatManager.instance.OnSave();
			}
			Speaker.speakers.Clear();
			RoomNode.roomNodes.Clear();
			RoomNode.gateProxyPositions.Clear();
			RoomSave.startRooms.Clear();
			PreviewLabs.PlayerPrefs.SetInt("LooseCores", looseCores);
			if (deleteSaves && Application.isEditor)
			{
				PreviewLabs.PlayerPrefs.DeleteAll();
			}
			PreviewLabs.PlayerPrefs.Flush();
		}
		int saveSlot = PreviewLabs.PlayerPrefs.saveSlot;
		PreviewLabs.PlayerPrefs.DeleteAll();
		PreviewLabs.PlayerPrefs.UpdateSaveSlot(99);
		PreviewLabs.PlayerPrefs.LoadIn();
		saveslot = saveSlot;
		if (!IntroManager.instance)
		{
			PreviewLabs.PlayerPrefs.SetInt("lastSave", saveslot);
			PreviewLabs.PlayerPrefs.SetBool("SkipLogo", skipLogo);
			PreviewLabs.PlayerPrefs.SetInt("TimePlayed" + saveslot, (int)Time.timeSinceLevelLoad + previousTimePlayed);
			PreviewLabs.PlayerPrefs.SetBool("CreativeMode" + saveslot, creativeMode);
			PreviewLabs.PlayerPrefs.Flush();
		}
		PreviewLabs.PlayerPrefs.DeleteAll();
		PreviewLabs.PlayerPrefs.UpdateSaveSlot(saveSlot);
		PreviewLabs.PlayerPrefs.LoadIn();
	}

	private IEnumerator UpdateUIs()
	{
		while (true)
		{
			for (int i = 0; i < CraftComponent.craftComponents.Count; i++)
			{
				if ((bool)HandController.currentHand && HandController.currentHand.deteriorationCone.activeSelf && Vector3.Distance(CraftComponent.craftComponents[i].transform.position, Head.position) < 4f)
				{
					CraftComponent.craftComponents[i].ToggleOutline(true);
				}
				else
				{
					CraftComponent.craftComponents[i].ToggleOutline(false);
				}
			}
			for (int j = 0; j < GunModuleUI.moduleUIs.Count; j++)
			{
				if (Vector3.Distance(GunModuleUI.moduleUIs[j].transform.position, Head.position) < 1.5f && !GunModuleUI.moduleUIs[j].parentModule.locked)
				{
					GunModuleUI.moduleUIs[j].gameObject.SetActive(true);
				}
				else
				{
					GunModuleUI.moduleUIs[j].gameObject.SetActive(false);
				}
			}
			if (!IntroManager.instance)
			{
				for (int k = 0; k < PlantUI.plantUIs.Count; k++)
				{
					if (Vector3.Distance(PlantUI.plantUIs[k].transform.position, Head.position) < 4f)
					{
						PlantUI.plantUIs[k].gameObject.SetActive(true);
					}
					else
					{
						PlantUI.plantUIs[k].gameObject.SetActive(false);
					}
				}
				for (int l = 0; l < LightSource.lightSources.Count; l++)
				{
					float num = Vector3.Distance(LightSource.lightSources[l].transform.position, Head.position);
					if (!castShadows)
					{
						continue;
					}
					if (num > LightSource.lightSources[l].occludeDistance)
					{
						if (LightSource.lightSources[l].castsShadows)
						{
							LightSource.lightSources[l].light.shadows = LightShadows.None;
						}
					}
					else if (LightSource.lightSources[l].castsShadows)
					{
						LightSource.lightSources[l].light.shadows = LightShadows.Hard;
					}
				}
				for (int m = 0; m < ShipLight.lightSources.Count; m++)
				{
					float num2 = Vector3.Distance(ShipLight.lightSources[m].transform.position, Head.position);
					if (num2 < ShipLight.lightSources[m].occludeDistance)
					{
						ShipLight.lightSources[m]._light.enabled = true;
					}
					else
					{
						ShipLight.lightSources[m]._light.enabled = false;
					}
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void ToggleFPSCounter()
	{
		fpsCounter.SetActive(!fpsCounter.activeSelf);
	}

	public void OnDeathReload()
	{
		SpawnPlayer();
		SpaceMask.instance.UI.SetBool("Skip", false);
		PreviewLabs.PlayerPrefs.SetBool("GameStarted", false);
		Object.Instantiate(cryoPod, CamRig.position, Quaternion.identity);
		timer.Reset();
		previousTimePlayed = 0;
		foreach (GameObject item in Inventory.inventory)
		{
			Object.Destroy(item);
		}
		Inventory.inventory.Clear();
		Inventory.instance.inventoryBubble.transform.parent.gameObject.SetActive(false);
		SuitManager.instance.survivalTime.transform.parent.gameObject.SetActive(false);
		if ((bool)StationManager.instance)
		{
			for (int i = 0; i < StationManager.instance.spawnedStations.Count; i++)
			{
				SceneManager.UnloadSceneAsync(StationManager.instance.spawnedStations[i].gameObject.scene.name);
			}
		}
		if (FuelSnapPoint.instance.target != null)
		{
			FuelSnapPoint.instance.target.gameObject.SetActive(false);
			FuelSnapPoint.instance.target = null;
		}
		StationManager.instance.spawnedStations.Clear();
		StationManager.instance.ResetPostions();
		StationManager.instance.OnSpawnStations();
		SuitManager.instance.ResetUpgrades();
		SuitManager.instance.nutritionRing.transform.parent.GetComponent<Animator>().SetBool("Empty", false);
		SuitManager.instance.nutritionRing.transform.parent.GetComponent<Animator>().SetBool("Almost", false);
		SuitManager.instance.oxygenRing.transform.parent.GetComponent<Animator>().SetBool("Empty", false);
		SuitManager.instance.oxygenRing.transform.parent.GetComponent<Animator>().SetBool("Almost", false);
		BaseManager.instance.power = BaseManager.instance.maxPower;
		SpawnNode.spawnNodes.Clear();
		Camera.main.farClipPlane = 1000f;
		instance.sunLight.enabled = true;
		CamRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
		CometManager.instance.Restart();
		GameAudioManager.instance.OnNormal();
		instance.dead = false;
		GameAudioManager.instance.Invoke("Restart", 1f);
		SuitManager.instance.OnStart();
		NanoInventory.instance.ClearComponents();
		LockerPanel.lockerIndex = 0;
		SpaceMask.instance.ForceClose();
		for (int j = 0; j < AddToInventory.inventoryObjects.Count; j++)
		{
			if ((bool)AddToInventory.inventoryObjects[j].GetComponent<Rigidbody>() && !AddToInventory.inventoryObjects[j].GetComponent<Rigidbody>().isKinematic)
			{
				Object.Destroy(AddToInventory.inventoryObjects[j].gameObject);
			}
		}
		for (int k = 0; k < DroneManager.instance.drones.Count; k++)
		{
			Object.Destroy(DroneManager.instance.drones[k]);
		}
		for (int l = 0; l < stationObjects.childCount; l++)
		{
			Object.Destroy(stationObjects.GetChild(l).gameObject);
		}
		Gun.instance.interact.ForceStopInteracting();
		if ((bool)ArmUIManager.instance)
		{
			ArmUIManager.instance.ShowVitalsTab();
		}
		ArmUIManager.instance.nanoCap.text = "0/0";
		instance.CamRig.GetComponent<Rigidbody>().drag = 0f;
		SuitManager.instance.canBreath = false;
		SuitManager.instance.oxygenWarning.SetBool("Oxygen", false);
		for (int m = 0; m < DroneManager.instance.drones.Count; m++)
		{
			Object.Destroy(DroneManager.instance.drones[m]);
		}
		BaseComputer.instance.DisableOtherUI();
		BaseCraftManager.instance.OnCancelCraft();
		CraftingManager.instance.OnCancelCraft();
		deathDelegate();
	}

	public int GetMinutesPlayed()
	{
		double num = timer.Elapsed.TotalSeconds + (double)previousTimePlayed;
		num += (double)previousTimePlayed;
		return Mathf.FloorToInt((int)(num / 60.0));
	}

	private string GetTimePlayedString()
	{
		int num = (int)Time.timeSinceLevelLoad + previousTimePlayed;
		num += previousTimePlayed;
		int num2 = Mathf.FloorToInt((float)num / 360f);
		int num3 = Mathf.FloorToInt((float)(num - num2 * 360) / 60f);
		int num4 = Mathf.FloorToInt((float)num - (float)num3 * 60f);
		return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", num2, num3, num4);
	}
}
