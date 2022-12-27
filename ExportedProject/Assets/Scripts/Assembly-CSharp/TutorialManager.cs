using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class TutorialManager : MonoBehaviour
{
	public List<GameObject> tutorials = new List<GameObject>();

	public static TutorialManager instance;

	internal bool Grabbed;

	internal bool Thrusted;

	internal bool Climbed;

	internal bool Menued;

	internal bool PInventory;

	internal bool NInventory;

	internal bool Ate;

	internal bool Canned;

	internal bool Scavenged;

	internal bool Repaired;

	internal bool Reloaded;

	public Text GrabText;

	public Text ThrusterText;

	public Text PhysicalInventoryText;

	public Text ScavengingText;

	public string[] PhysicalInventoryInfo;

	public string[] ScavengingInfo;

	public string[] RepairInfo;

	public string[] ReloadInfo;

	public string GrabInfo;

	public string ThrusterInfo;

	public string ClimbInfo;

	public string NanoInventoryInfo;

	public string EatingInfo;

	public string CannisterInfo;

	public Text ClimbText;

	public Text NanoInventoryText;

	public Text EatingText;

	public Text CannisterText;

	public Text RepairText;

	public Text ReloadText;

	public AudioClip thrusterAudio;

	public AudioClip physicalInventoryAudio;

	public AudioClip foodAudio;

	public AudioClip canisterAudio;

	public AudioClip nanoInventoryAudio;

	public AudioClip scavengeAudio;

	public AudioClip repairAudio;

	public Transform maskPos;

	public Transform armPos;

	private int currentIndex;

	private float ShowTime = 20f;

	internal bool isOculus;

	private void Awake()
	{
		instance = this;
		if (VRTK_DeviceFinder.GetHeadsetType() == VRTK_DeviceFinder.Headsets.OculusRiftCV1)
		{
			isOculus = true;
		}
	}

	private void Start()
	{
		Setup();
		SetupText();
	}

	private void SetupText()
	{
		GrabText.text = GrabInfo;
		ThrusterText.text = ThrusterInfo;
		ClimbText.text = ClimbInfo;
		if (!isOculus)
		{
			PhysicalInventoryText.text = PhysicalInventoryInfo[0];
		}
		else
		{
			PhysicalInventoryText.text = PhysicalInventoryInfo[1];
		}
		NanoInventoryText.text = NanoInventoryInfo;
		EatingText.text = EatingInfo;
		CannisterText.text = CannisterInfo;
		if (!isOculus)
		{
			RepairText.text = RepairInfo[0];
		}
		else
		{
			RepairText.text = RepairInfo[1];
		}
		if (!isOculus)
		{
			ScavengingText.text = ScavengingInfo[0];
		}
		else
		{
			ScavengingText.text = ScavengingInfo[1];
		}
		if (!isOculus)
		{
			ReloadText.text = ReloadInfo[0];
		}
		else
		{
			ReloadText.text = ReloadInfo[1];
		}
	}

	private void Setup()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("OnGrab"))
		{
			OnGrab();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("OnThrust"))
		{
			OnThrust();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("OnClimb"))
		{
			OnClimb();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("OnPhysicalInventory"))
		{
			OnPhysicalInventoryComplete();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("OnNanoInventory"))
		{
			OnNanolInventoryComplete();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("OnEat"))
		{
			OnEatComplete();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("OnCan"))
		{
			OnCannisterComplete();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("OnRepair"))
		{
			OnRepairComplete();
		}
		if (PreviewLabs.PlayerPrefs.GetBool("OnReload"))
		{
			OnReloadComplete();
		}
	}

	public void ToggleGrab()
	{
		if (!Grabbed)
		{
			tutorials[0].SetActive(true);
			currentIndex = 0;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
		}
	}

	public void OnGrab()
	{
		tutorials[0].SetActive(false);
		PreviewLabs.PlayerPrefs.SetBool("OnGrab", true);
		Grabbed = true;
		if (!PreviewLabs.PlayerPrefs.GetBool("OnThrust"))
		{
			Invoke("ToggleThrust", 4f);
		}
	}

	private void ToggleThrust()
	{
		tutorials[1].SetActive(true);
		currentIndex = 1;
		tutorials[currentIndex].transform.SetParent(maskPos);
		tutorials[currentIndex].transform.localPosition = Vector3.zero;
		Invoke("StartLerp", 3f);
		Invoke("ThrusterAudio", 6f);
	}

	public void OnThrust()
	{
		if (!Thrusted && Grabbed)
		{
			CancelInvoke("ToggleThrust");
			tutorials[1].SetActive(false);
			PreviewLabs.PlayerPrefs.SetBool("OnThrust", true);
			if (!PreviewLabs.PlayerPrefs.GetBool("OnClimb"))
			{
				Invoke("ToggleClimb", 2f);
				CancelInvoke("ThrusterAudio");
			}
			Thrusted = true;
		}
	}

	private void ToggleClimb()
	{
		if (!Climbed)
		{
			tutorials[2].SetActive(true);
			currentIndex = 2;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
		}
	}

	public void OnClimb()
	{
		if (!Climbed)
		{
			tutorials[2].SetActive(false);
			PreviewLabs.PlayerPrefs.SetBool("OnClimb", true);
			Climbed = true;
		}
	}

	public void TogglePhysicalInventory()
	{
		if ((!PInventory || !Climbed) && Canned && !IsInvoking("DisableTimer") && !IntroManager.instance)
		{
			tutorials[3].SetActive(true);
			currentIndex = 3;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
			if (!IsInvoking("DisableTimer"))
			{
				GameAudioManager.instance.AddToSuitQueue(physicalInventoryAudio);
				Invoke("DisableTimer", ShowTime);
			}
		}
	}

	public void OnPhysicalInventoryComplete()
	{
		if (!PInventory)
		{
			tutorials[3].SetActive(false);
			PreviewLabs.PlayerPrefs.SetBool("OnPhysicalInventory", true);
			CancelInvoke("DisableTimer");
			PInventory = true;
		}
	}

	public void ToggleNanoInventory()
	{
		if (!NInventory && !IsInvoking("DisableTimer") && !IntroManager.instance)
		{
			tutorials[4].SetActive(true);
			currentIndex = 4;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
			GameAudioManager.instance.AddToSuitQueue(nanoInventoryAudio);
			Invoke("DisableTimer", ShowTime);
			OnNanolInventoryComplete();
		}
	}

	public void OnNanolInventoryComplete()
	{
		tutorials[4].SetActive(false);
		PreviewLabs.PlayerPrefs.SetBool("OnNanoInventory", true);
		CancelInvoke("DisableTimer");
		NInventory = true;
	}

	public void ToggleEat()
	{
		if (!Ate && !IsInvoking("DisableTimer"))
		{
			tutorials[5].SetActive(true);
			currentIndex = 5;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
			if (!IsInvoking("DisableTimer"))
			{
				GameAudioManager.instance.AddToSuitQueue(nanoInventoryAudio);
				Invoke("DisableTimer", ShowTime);
			}
		}
	}

	public void OnEatComplete()
	{
		tutorials[5].SetActive(false);
		PreviewLabs.PlayerPrefs.SetBool("OnEat", true);
		CancelInvoke("DisableTimer");
		Ate = true;
	}

	public void ToggleCannister()
	{
		if (!Canned && !IsInvoking("DisableTimer") && !IntroManager.instance)
		{
			tutorials[6].SetActive(true);
			currentIndex = 6;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
			if (!IsInvoking("DisableTimer"))
			{
				GameAudioManager.instance.AddToSuitQueue(canisterAudio);
				Invoke("DisableTimer", ShowTime);
			}
		}
	}

	public void OnCannisterComplete()
	{
		if (!Canned)
		{
			tutorials[6].SetActive(false);
			PreviewLabs.PlayerPrefs.SetBool("OnCan", true);
			CancelInvoke("DisableTimer");
			Canned = true;
		}
	}

	public void ToggleScavenging()
	{
		if (!Scavenged)
		{
			tutorials[7].SetActive(true);
			currentIndex = 7;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
			GameAudioManager.instance.AddToSuitQueue(scavengeAudio);
			Invoke("DisableTimer", ShowTime);
		}
	}

	public void OnScavengingComplete()
	{
		if (!Scavenged && !IntroManager.instance)
		{
			if (tutorials[7] != null)
			{
				tutorials[7].SetActive(false);
			}
			PreviewLabs.PlayerPrefs.SetBool("OnCan", true);
			CancelInvoke("DisableTimer");
			Scavenged = true;
		}
	}

	public void ToggleRepair()
	{
		if (!Repaired)
		{
			tutorials[8].SetActive(true);
			currentIndex = 8;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
			GameAudioManager.instance.AddToSuitQueue(repairAudio);
			Invoke("DisableTimer", ShowTime);
		}
	}

	public void OnRepairComplete()
	{
		if (!Repaired && !IntroManager.instance)
		{
			tutorials[8].SetActive(false);
			PreviewLabs.PlayerPrefs.SetBool("OnRepair", true);
			CancelInvoke("DisableTimer");
			Repaired = true;
			if (PreviewLabs.PlayerPrefs.GetInt("CurrentObjective") == 0)
			{
				ObjectiveManager.instance.OnObjectiveComplete(0);
			}
		}
	}

	public void ToggleReload()
	{
		if (!Reloaded)
		{
			tutorials[9].SetActive(true);
			currentIndex = 9;
			tutorials[currentIndex].transform.SetParent(maskPos);
			tutorials[currentIndex].transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 3f);
			Invoke("DisableTimer", ShowTime);
		}
	}

	public void OnReloadComplete()
	{
		if (!Reloaded)
		{
			if (tutorials.Count > 9)
			{
				tutorials[9].SetActive(false);
			}
			if (!IntroManager.instance)
			{
				PreviewLabs.PlayerPrefs.SetBool("OnReload", true);
			}
			CancelInvoke("DisableTimer");
			Reloaded = true;
		}
	}

	private void DisableTimer()
	{
		tutorials[currentIndex].SetActive(false);
	}

	private void ThrusterAudio()
	{
		GameAudioManager.instance.AddToSuitQueue(thrusterAudio);
	}

	private void StartLerp()
	{
		StartCoroutine("LerpToArm");
	}

	private IEnumerator LerpToArm()
	{
		tutorials[currentIndex].transform.SetParent(armPos);
		float refreshRate = 0.03f;
		while (tutorials[currentIndex].activeSelf)
		{
			tutorials[currentIndex].transform.localPosition = Vector3.Lerp(tutorials[currentIndex].transform.localPosition, Vector3.zero, 5f * refreshRate);
			yield return new WaitForSeconds(refreshRate);
		}
	}
}
