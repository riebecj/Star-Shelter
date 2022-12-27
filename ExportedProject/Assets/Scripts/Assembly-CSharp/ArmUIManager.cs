using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK;
using Wilberforce.VAO;

public class ArmUIManager : MonoBehaviour
{
	public GameObject[] tabs;

	public GameObject[] craftButtons;

	public GameObject[] gridSelectFrames;

	private AudioSource audioSource;

	public AudioClip beep;

	internal int craftIndex;

	public bool canRotate;

	public bool gripSwap;

	public bool speedVignette;

	public bool gradualRotation;

	public Sprite[] craftComponentIcons;

	public Text partCount;

	public Text volumeNumber;

	public Text nanoCap;

	public Text removeAmountUI;

	public static ArmUIManager instance;

	public GameObject main;

	public GameObject options;

	public GameObject debug;

	public GameObject noCraftObjectLabel;

	public GameObject controllers;

	public GameObject audioOptions;

	public GameObject viveController;

	public GameObject oculusController;

	public GameObject cosmosController;

	public GameObject placeCraftObjectButton;

	public GameObject body;

	public GameObject craftedObjectIcon;

	public GameObject craftMaterialIcon;

	public GameObject targetMaterialFrame;

	public GameObject removeButton;

	public GameObject salvageWarning;

	public GameObject inventoryCanvas;

	public Transform inDoorGrid;

	public Transform outDoorGrid;

	public Transform structureGrid;

	public Transform materialGrid;

	public Transform targetFrame;

	public Transform addLabelPos;

	public Transform craftIndicatorPos;

	internal Transform head;

	private GameObject addLabel;

	public ToggleButton[] toggleButtons;

	public List<GameObject> craftedObjectIcons = new List<GameObject>();

	public List<CraftMaterialIcon> craftMaterialIcons = new List<CraftMaterialIcon>();

	public List<CraftedObject> craftedObjects = new List<CraftedObject>();

	public List<CraftMaterial> craftMaterials = new List<CraftMaterial>();

	private CraftMaterial targetedCraftMaterial;

	internal CraftComponent warningComponent;

	internal int debugtick;

	internal int removeAmount = 1;

	public ScrollRect scrollRect;

	private void Awake()
	{
		instance = this;
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		head = GameManager.instance.Head;
		Setup();
		if (Application.isEditor)
		{
			DebugManager.instance.EnableDebugMenu();
		}
	}

	private void Setup()
	{
		if (!PreviewLabs.PlayerPrefs.GetBool("WorldShadows"))
		{
			toggleButtons[0].TurnOff();
		}
		if (!PreviewLabs.PlayerPrefs.GetBool("AmbientOC"))
		{
			toggleButtons[1].TurnOff();
		}
		canRotate = true;
		if (!PreviewLabs.PlayerPrefs.HasKey("RotateView"))
		{
			if (!GameManager.instance.isOculus)
			{
				toggleButtons[2].TurnOff();
			}
		}
		else if (!PreviewLabs.PlayerPrefs.GetBool("RotateView"))
		{
			toggleButtons[2].TurnOff();
		}
		if (PreviewLabs.PlayerPrefs.HasKey("HideUI") && !PreviewLabs.PlayerPrefs.GetBool("HideUI"))
		{
			toggleButtons[3].TurnOff();
		}
		gripSwap = true;
		if (!PreviewLabs.PlayerPrefs.HasKey("GripToGrab") && !GameManager.instance.isOculus)
		{
			toggleButtons[4].TurnOff();
		}
		else if (!PreviewLabs.PlayerPrefs.GetBool("GripToGrab"))
		{
			toggleButtons[4].TurnOff();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("GripToGrab"))
		{
			GameManager.instance.leftController.GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			GameManager.instance.rightController.GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			HeadLightSwitch.instance.GetComponent<VRTK_InteractableObject>().useOverrideButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
		}
		if (PreviewLabs.PlayerPrefs.HasKey("body") && !PreviewLabs.PlayerPrefs.GetBool("body"))
		{
			toggleButtons[5].TurnOff();
		}
		speedVignette = true;
		if (PreviewLabs.PlayerPrefs.HasKey("Vignette") && !PreviewLabs.PlayerPrefs.GetBool("Vignette"))
		{
			toggleButtons[6].TurnOff();
		}
		if (PreviewLabs.PlayerPrefs.HasKey("GradualRotation") && !PreviewLabs.PlayerPrefs.GetBool("GradualRotation"))
		{
			toggleButtons[7].TurnOff();
		}
		else
		{
			gradualRotation = true;
		}
		if (GameManager.instance.isOculus)
		{
			viveController.SetActive(false);
			oculusController.SetActive(true);
			cosmosController.SetActive(false);
		}
		else if (GameManager.instance.isCosmos)
		{
			viveController.SetActive(false);
			oculusController.SetActive(false);
			cosmosController.SetActive(true);
		}
		else
		{
			viveController.SetActive(true);
			oculusController.SetActive(false);
			cosmosController.SetActive(false);
		}
	}

	public void HideTabs()
	{
		for (int i = 0; i < tabs.Length; i++)
		{
			if (i == 0)
			{
				tabs[i].transform.localScale = Vector3.one * 0.5f;
				tabs[0].transform.localPosition = new Vector3(tabs[0].transform.localPosition.x, -0.1f, tabs[0].transform.localPosition.z);
			}
			else
			{
				tabs[i].SetActive(false);
			}
		}
		PlayClickAudio();
		BaseCraftManager.instance.OnCancelCraft();
		CraftingManager.instance.OnCancelCraft();
		GateNode.Toggle(false);
		WallNode.Toggle(false);
		LoseCraftMaterialTarget();
		targetFrame.gameObject.SetActive(false);
	}

	public void PlayClickAudio()
	{
		audioSource.PlayOneShot(beep);
	}

	public void ShowObjectiveTab()
	{
		HideTabs();
		tabs[4].SetActive(true);
		ObjectiveManager.instance.ObjectiveNoticed();
	}

	public void ShowCraftTab()
	{
		HideTabs();
		tabs[1].SetActive(true);
		GameObject[] array = craftButtons;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		UpdateUI();
	}

	public void ShowOptionsTab()
	{
		HideTabs();
		volumeNumber.text = (AudioListener.volume * 10f).ToString("F0");
		tabs[2].SetActive(true);
	}

	public void ShowVitalsTab()
	{
		HideTabs();
		if (!DroneHelper.instance.VRControlled)
		{
			tabs[0].transform.localScale = Vector3.one;
			tabs[0].transform.localPosition = new Vector3(tabs[0].transform.localPosition.x, 0f, tabs[0].transform.localPosition.z);
		}
	}

	public void ShowInventory()
	{
		if (!DroneHelper.instance.VRControlled)
		{
			TutorialManager.instance.ToggleNanoInventory();
			HideTabs();
			tabs[3].SetActive(true);
			inventoryCanvas.SetActive(true);
			salvageWarning.SetActive(false);
			UpdateInventoryUI();
		}
	}

	public void ShowSalvageWarning()
	{
		HideTabs();
		tabs[3].SetActive(true);
		inventoryCanvas.SetActive(false);
		salvageWarning.SetActive(true);
	}

	public void OnSalvageConfirm(bool value)
	{
		if (value && (bool)warningComponent)
		{
			warningComponent.OnCompleteSalvage();
			warningComponent = null;
		}
		else if ((bool)warningComponent)
		{
			warningComponent.CancelSalvage();
			warningComponent = null;
		}
		ShowInventory();
	}

	public void ChangeVolume(float value)
	{
		AudioListener.volume += value;
		AudioListener.volume = Mathf.Clamp(AudioListener.volume, 0f, 1f);
		volumeNumber.text = (AudioListener.volume * 10f).ToString("F0");
		PlayClickAudio();
	}

	public void ToggleShadows(ToggleButton button)
	{
		foreach (LightSource lightSource in LightSource.lightSources)
		{
			if (lightSource.castsShadows)
			{
				if (!button.On)
				{
					lightSource.light.shadows = LightShadows.None;
				}
				else
				{
					lightSource.light.shadows = LightShadows.Hard;
				}
				GameManager.instance.castShadows = button.On;
			}
		}
		PreviewLabs.PlayerPrefs.SetBool("WorldShadows", button.On);
	}

	public void ToggleAmbientOcclusion(ToggleButton button)
	{
		if (!button.On)
		{
			GameManager.instance.Head.GetComponent<VAOEffect>().enabled = false;
		}
		else
		{
			GameManager.instance.Head.GetComponent<VAOEffect>().enabled = true;
		}
		PreviewLabs.PlayerPrefs.SetBool("AmbientOC", button.On);
	}

	public void ToggleMaskUI(ToggleButton button)
	{
		GameObject gameObject = SpaceMask.instance.UI.gameObject;
		if (!button.On)
		{
			gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(true);
		}
		PreviewLabs.PlayerPrefs.SetBool("HideUI", button.On);
	}

	public void ToggleBody(ToggleButton button)
	{
		if (!button.On)
		{
			body.SetActive(false);
		}
		else
		{
			body.SetActive(true);
		}
		PreviewLabs.PlayerPrefs.SetBool("body", button.On);
	}

	public void ToggleRotatableView(ToggleButton button)
	{
		if (!button.On)
		{
			canRotate = false;
			toggleButtons[7].transform.parent.gameObject.SetActive(false);
		}
		else
		{
			canRotate = true;
			toggleButtons[7].transform.parent.gameObject.SetActive(true);
		}
		PreviewLabs.PlayerPrefs.SetBool("RotateView", button.On);
	}

	public void ToggleGripToGrab(ToggleButton button)
	{
		if (!button.On)
		{
			GameManager.instance.leftController.GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
			GameManager.instance.rightController.GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
			HeadLightSwitch.instance.GetComponent<VRTK_InteractableObject>().useOverrideButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
			gripSwap = false;
		}
		else
		{
			GameManager.instance.leftController.GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			GameManager.instance.rightController.GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			HeadLightSwitch.instance.GetComponent<VRTK_InteractableObject>().useOverrideButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			gripSwap = true;
		}
		PreviewLabs.PlayerPrefs.SetBool("GripToGrab", button.On);
	}

	public void ToggleSpeedVignette(ToggleButton button)
	{
		if (!button.On)
		{
			speedVignette = false;
			Vignette.instance.FadeOut();
		}
		else
		{
			speedVignette = true;
		}
		PreviewLabs.PlayerPrefs.SetBool("Vignette", button.On);
	}

	public void ToggleGradualRotation(ToggleButton button)
	{
		if (!button.On)
		{
			gradualRotation = false;
		}
		else
		{
			gradualRotation = true;
		}
		PreviewLabs.PlayerPrefs.SetBool("GradualRotation", button.On);
		debugtick++;
		if (debugtick > 9)
		{
			DebugManager.instance.EnableDebugMenu();
		}
	}

	public void ShowControls()
	{
		controllers.SetActive(true);
		options.SetActive(false);
		main.SetActive(false);
		PlayClickAudio();
	}

	public void ShowAudioOptions()
	{
		audioOptions.SetActive(true);
		controllers.SetActive(false);
		options.SetActive(false);
		main.SetActive(false);
		PlayClickAudio();
	}

	public void ShowOptions()
	{
		options.SetActive(true);
		main.SetActive(false);
		PlayClickAudio();
	}

	public void ShowMain()
	{
		audioOptions.SetActive(false);
		controllers.SetActive(false);
		options.SetActive(false);
		debug.SetActive(false);
		main.SetActive(true);
		PlayClickAudio();
		debugtick = 0;
	}

	public void ShowDebugMenu()
	{
		debug.SetActive(true);
		main.SetActive(false);
		PlayClickAudio();
	}

	public void OnExit()
	{
		GameManager.instance.skipLogo = true;
		GameManager.instance.OnSave();
		PreviewLabs.PlayerPrefs.DeleteAll();
		PreviewLabs.PlayerPrefs.UpdateSaveSlot(99);
		PreviewLabs.PlayerPrefs.LoadIn();
		PreviewLabs.PlayerPrefs.Flush();
		DroneAI.spawnedDrones.Clear();
		Comet.unActiveComets.Clear();
		SpawnNode.spawnNodes.Clear();
		if ((bool)Inventory.instance)
		{
			Inventory.inventory.Clear();
		}
		SceneManager.LoadScene("TutorialScene");
	}

	public void UpdateUI()
	{
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if ((NanoInventory.instance.craftedObjectCounts[i] > 0 || (GameManager.instance.creativeMode && NanoInventory.instance.craftedObjects[i].prefab.GetComponent<Recipe>().addToNanoInventory)) && !craftedObjects.Contains(NanoInventory.instance.craftedObjects[i]))
			{
				craftedObjects.Add(NanoInventory.instance.craftedObjects[i]);
				GameObject gameObject = Object.Instantiate(craftedObjectIcon, base.transform.position, base.transform.rotation);
				Transform transform = null;
				float iconOffsetY = NanoInventory.instance.craftedObjects[i].iconOffsetY;
				if (NanoInventory.instance.craftedObjects[i] is Structure || NanoInventory.instance.craftedObjects[i] is WallStructure)
				{
					transform = structureGrid;
				}
				else if (NanoInventory.instance.craftedObjects[i] is Prop)
				{
					Prop prop = NanoInventory.instance.craftedObjects[i] as Prop;
					transform = ((!prop.outDoor) ? inDoorGrid : outDoorGrid);
				}
				gameObject.transform.SetParent(transform);
				gameObject.GetComponent<CraftedObjectIcon>().OnAssign(NanoInventory.instance.craftedObjects[i]);
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.rotation = transform.rotation;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.GetComponent<CraftedObjectIcon>().filter.transform.rotation = transform.rotation * Quaternion.Euler(NanoInventory.instance.craftedObjects[i].iconRot);
				craftedObjectIcons.Add(gameObject);
				if (NanoInventory.instance.craftedObjects[i].newCraft)
				{
					gameObject.GetComponent<CraftedObjectIcon>().unread.gameObject.SetActive(true);
				}
			}
			else if (craftedObjects.Contains(NanoInventory.instance.craftedObjects[i]) && NanoInventory.instance.craftedObjectCounts[i] == 0 && !GameManager.instance.creativeMode)
			{
				craftedObjects.Remove(NanoInventory.instance.craftedObjects[i]);
				for (int j = 0; j < craftedObjectIcons.Count; j++)
				{
					if (craftedObjectIcons[j].GetComponent<CraftedObjectIcon>().craftedObject == NanoInventory.instance.craftedObjects[i])
					{
						craftedObjectIcons[j].gameObject.SetActive(false);
						craftedObjectIcons.Remove(craftedObjectIcons[j]);
					}
				}
			}
			for (int k = 0; k < craftedObjectIcons.Count; k++)
			{
				if (craftedObjectIcons[k].GetComponent<CraftedObjectIcon>().craftedObject == NanoInventory.instance.craftedObjects[i])
				{
					if (!GameManager.instance.creativeMode)
					{
						craftedObjectIcons[k].GetComponent<CraftedObjectIcon>().count.text = NanoInventory.instance.craftedObjectCounts[i].ToString();
					}
					else
					{
						craftedObjectIcons[k].GetComponent<CraftedObjectIcon>().count.text = string.Empty;
					}
				}
			}
		}
		UpdateInventoryUI();
	}

	public void UpdateInventoryUI()
	{
		for (int i = 0; i < NanoInventory.instance.craftMaterials.Count; i++)
		{
			if (!craftMaterials.Contains(NanoInventory.instance.craftMaterials[i]))
			{
				craftMaterials.Add(NanoInventory.instance.craftMaterials[i]);
				GameObject gameObject = Object.Instantiate(craftMaterialIcon, materialGrid.position, materialGrid.rotation);
				gameObject.transform.SetParent(materialGrid);
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<CraftMaterialIcon>().OnAssign(NanoInventory.instance.craftMaterials[i]);
				craftMaterialIcons.Add(gameObject.GetComponent<CraftMaterialIcon>());
			}
		}
		for (int j = 0; j < craftMaterialIcons.Count; j++)
		{
			craftMaterialIcons[j].UpdateCount();
		}
		nanoCap.text = NanoInventory.instance.nanoMass + "/" + NanoInventory.instance.nanoCap;
		if (NanoInventory.instance.nanoMass >= NanoInventory.instance.nanoCap)
		{
			nanoCap.color = Color.red;
		}
		else
		{
			nanoCap.color = Color.white;
		}
	}

	public void OnAddComponent(GameObject newLabel)
	{
	}

	public void PlaceCraftedObject(CraftedObjectIcon _craftedObjectIcon)
	{
		targetFrame.gameObject.SetActive(true);
		targetFrame.position = _craftedObjectIcon.transform.position;
		CraftingManager.instance.craftObject = null;
		CraftingManager.instance.floorObject = false;
		CraftingManager.instance.proxy.gameObject.SetActive(false);
		GateNode.Toggle(false);
		CraftingManager.instance.proxy.GetComponent<PropProxy>().visualCone.SetActive(false);
		BaseCraftManager.instance.OnCancelCraft();
		if (_craftedObjectIcon.craftedObject is Prop)
		{
			CraftingManager.instance.craftObject = _craftedObjectIcon.craftedObject.prefab;
			GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
			GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(true);
			if ((bool)_craftedObjectIcon.craftedObject.prefab.GetComponent<Gate>())
			{
				BaseCraftManager.instance.currentGate = _craftedObjectIcon.craftedObject.prefab;
				GateNode.Toggle(true);
			}
			else
			{
				CraftingManager.instance.proxy.gameObject.SetActive(true);
				CraftingManager.instance.proxy.GetComponent<MeshFilter>().sharedMesh = _craftedObjectIcon.craftedObject.prefab.GetComponent<MeshFilter>().sharedMesh;
				CraftingManager.instance.proxy.rotation = _craftedObjectIcon.craftedObject.prefab.transform.rotation;
				Prop prop = _craftedObjectIcon.craftedObject as Prop;
				if (prop.floor)
				{
					CraftingManager.instance.floorObject = true;
				}
				if ((bool)_craftedObjectIcon.craftedObject.prefab.GetComponent<Turret>())
				{
					CraftingManager.instance.proxy.GetComponent<PropProxy>().visualCone.SetActive(true);
				}
				if ((bool)_craftedObjectIcon.craftedObject.prefab.transform.Find("PlacementCollider"))
				{
					CraftingManager.instance.proxy.GetComponent<BoxCollider>().center = _craftedObjectIcon.craftedObject.prefab.transform.Find("PlacementCollider").GetComponent<BoxCollider>().center;
					CraftingManager.instance.proxy.GetComponent<BoxCollider>().size = _craftedObjectIcon.craftedObject.prefab.transform.Find("PlacementCollider").GetComponent<BoxCollider>().size;
				}
			}
		}
		else if (_craftedObjectIcon.craftedObject is WallStructure)
		{
			BaseCraftManager.instance.currentWall = _craftedObjectIcon.craftedObject as WallStructure;
			GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
			GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(true);
			WallNode.Toggle(true);
		}
		else
		{
			BaseCraftManager.instance.SpawnProxy(_craftedObjectIcon.craftedObject);
		}
		PlayClickAudio();
	}

	public void ToggleGrid(int value)
	{
		DemarkGridButtons();
		outDoorGrid.gameObject.SetActive(false);
		inDoorGrid.gameObject.SetActive(false);
		structureGrid.gameObject.SetActive(false);
		switch (value)
		{
		case 0:
			outDoorGrid.gameObject.SetActive(true);
			gridSelectFrames[0].SetActive(true);
			break;
		case 1:
			inDoorGrid.gameObject.SetActive(true);
			gridSelectFrames[1].SetActive(true);
			break;
		case 2:
			structureGrid.gameObject.SetActive(true);
			gridSelectFrames[2].SetActive(true);
			break;
		}
		PlayClickAudio();
	}

	private void DemarkGridButtons()
	{
		GameObject[] array = gridSelectFrames;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
	}

	private IEnumerator MoveAddLabel()
	{
		float refreshRate = 0.01f;
		while ((bool)addLabel && addLabel.transform.localPosition.magnitude > 0.05f)
		{
			addLabel.transform.localPosition = Vector3.Lerp(addLabel.transform.localPosition, Vector3.zero, refreshRate * 10f);
			yield return new WaitForSeconds(refreshRate);
		}
	}

	public void TargetCraftMaterial(CraftMaterialIcon craftMaterialIcon)
	{
		targetMaterialFrame.SetActive(true);
		targetedCraftMaterial = craftMaterialIcon.craftedObject;
		targetMaterialFrame.transform.position = craftMaterialIcon.transform.position;
	}

	private void LoseCraftMaterialTarget()
	{
		targetedCraftMaterial = null;
		targetMaterialFrame.SetActive(false);
	}

	public void RemoveMaterial()
	{
		if (targetedCraftMaterial != null)
		{
			for (int i = 0; i < NanoInventory.instance.craftMaterials.Count; i++)
			{
				if (!(targetedCraftMaterial == NanoInventory.instance.craftMaterials[i]))
				{
					continue;
				}
				if (NanoInventory.instance.materialCounts[i] == 0)
				{
					return;
				}
				int num = 0;
				if (NanoInventory.instance.materialCounts[i] >= removeAmount)
				{
					NanoInventory.instance.materialCounts[i] -= removeAmount;
					num = removeAmount;
				}
				else
				{
					num = NanoInventory.instance.materialCounts[i];
					NanoInventory.instance.materialCounts[i] = 0;
				}
				if (SuitManager.instance.power < SuitManager.instance.maxPower)
				{
					SuitManager.instance.power += 2 * num;
					SuitManager.instance.power = Mathf.Clamp(SuitManager.instance.power, 0f, SuitManager.instance.maxPower);
				}
				break;
			}
		}
		NanoInventory.instance.GetNanoMass();
		UpdateUI();
	}

	public void ChangeRemoveAmount(bool increase)
	{
		if (increase)
		{
			if (removeAmount < 10)
			{
				removeAmount++;
			}
		}
		else if (removeAmount > 1)
		{
			removeAmount--;
		}
		removeAmountUI.text = removeAmount.ToString();
	}

	private void DisableAddLabel()
	{
		Object.Destroy(addLabel);
		addLabel = null;
	}

	public void ShowResource(int index)
	{
	}

	public void MasterVolume(float value)
	{
		GameAudioManager.instance.mainMixer.SetFloat("Master", value);
	}

	public void SFXVolume(float value)
	{
		GameAudioManager.instance.mainMixer.SetFloat("SFX", value);
	}

	public void VoiceVolume(float value)
	{
		GameAudioManager.instance.mainMixer.SetFloat("Voice", value);
	}

	public void UpdateObjetivesScrollBar(float value)
	{
		scrollRect.verticalNormalizedPosition += value * (1f / (float)ObjectiveManager.instance.Objectives.Length) * 1.25f;
	}
}
