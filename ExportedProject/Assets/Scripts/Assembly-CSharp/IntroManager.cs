using System;
using System.Collections;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class IntroManager : MonoBehaviour
{
	public static IntroManager instance;

	public string ClimbInfo;

	public string CannisterInfo;

	public string ThrusterInfo;

	public string GunInfo;

	public string[] RepairInfo;

	public string[] SalvageInfo;

	public Transform armPos;

	public Transform maskPos;

	public GameObject TutorialLabel;

	public GameObject Menu;

	public GameObject options;

	public GameObject ContinueQuestion;

	internal bool isOculus;

	internal GameObject newLabel;

	public Text volumeNumber;

	public AudioClip beep;

	public AudioClip deleteBeep;

	public AudioSource audioSource;

	internal bool onClimb;

	internal bool onThrust;

	internal bool onCannister;

	internal bool onSalvage;

	internal bool canThrust;

	internal bool holstered;

	public Transform introPosition;

	public Transform IntroScene;

	public DoorSensor doorSensor;

	internal int saveSlot;

	public Text[] savelots;

	public Text[] playTimes;

	public Text[] gameType;

	public Color activeColor;

	public Color passiveColor;

	public GameObject[] newSlots;

	public GameObject[] activeSlots;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (SteamVR.instance.hmd_TrackingSystemName == "oculus")
		{
			isOculus = true;
		}
		if (!PreviewLabs.PlayerPrefs.GetBool("SkipLogo"))
		{
			Vector3 vector = GameManager.instance.Head.position - GameManager.instance.CamRig.position;
			vector.y = 0f;
			GameManager.instance.CamRig.position = introPosition.position - vector;
			Vector3 vector2 = GameManager.instance.Head.position + GameManager.instance.Head.transform.forward * 35f;
			IntroScene.transform.position = new Vector3(vector2.x, IntroScene.position.y, vector2.z);
			IntroScene.LookAt(new Vector3(GameManager.instance.Head.position.x, IntroScene.position.y, GameManager.instance.Head.position.z));
			Invoke("MoveToStart", 8f);
			Invoke("FadeToBlack", 6.5f);
		}
		else
		{
			SkipToStart();
		}
		if (!GameManager.instance.DemoBuild)
		{
			doorSensor.SetLock(false);
		}
		else
		{
			Menu.transform.parent.parent.gameObject.SetActive(false);
			Invoke("ToggleClimb", 8f);
		}
		LoadStartingVariables();
		GameAudioManager.instance.OnNormal();
	}

	private void FadeToBlack()
	{
		VRTK_ScreenFade.Start(Color.black, 0.8f);
	}

	private void MoveToStart()
	{
		VRTK_ScreenFade.Start(Color.clear, 1.5f);
		Vector3 vector = GameManager.instance.Head.position - GameManager.instance.CamRig.position;
		vector.y = 0f;
		GameManager.instance.CamRig.position = Vector3.zero - vector;
	}

	private void SkipToStart()
	{
		Vector3 vector = GameManager.instance.Head.position - GameManager.instance.CamRig.position;
		vector.y = 0f;
		GameManager.instance.CamRig.position = Vector3.zero - vector;
	}

	public void PlayTutorial()
	{
		doorSensor.SetLock(true);
		Invoke("DisableMenu", 1f);
		Invoke("ToggleClimb", 1f);
		PlayClickAudio();
	}

	public void PlayMainGame()
	{
		Invoke("DisableMenu", 1f);
		IntroCapsuleEvent.instance.OnLoadGame();
		PlayClickAudio();
	}

	public void ChangeVolume(float value)
	{
		AudioListener.volume += value;
		AudioListener.volume = Mathf.Clamp(AudioListener.volume, 0f, 1f);
		volumeNumber.text = (AudioListener.volume * 10f).ToString("F0");
		PlayClickAudio();
	}

	public void ShowOptions()
	{
		options.SetActive(true);
		Menu.SetActive(false);
		PlayClickAudio();
	}

	public void ShowMain()
	{
		options.SetActive(false);
		Menu.SetActive(true);
		PlayClickAudio();
	}

	public void OnExit()
	{
		Application.Quit();
	}

	private void DisableMenu()
	{
		Menu.SetActive(false);
	}

	public void ToggleClimb()
	{
		if (!onClimb)
		{
			onClimb = false;
			newLabel = UnityEngine.Object.Instantiate(TutorialLabel, base.transform.position, base.transform.rotation);
			newLabel.GetComponentInChildren<Text>().text = ClimbInfo;
			newLabel.transform.SetParent(maskPos);
			newLabel.transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 4f);
		}
	}

	public void ClimbComplete()
	{
		if (!onClimb)
		{
			if (newLabel != null)
			{
				newLabel.SetActive(false);
			}
			onClimb = true;
		}
	}

	public void ToggleCannister()
	{
		if (!onCannister && (!(newLabel != null) || !(newLabel.GetComponentInChildren<Text>().text == CannisterInfo)))
		{
			newLabel = UnityEngine.Object.Instantiate(TutorialLabel, base.transform.position, base.transform.rotation);
			newLabel.GetComponentInChildren<Text>().text = CannisterInfo;
			newLabel.transform.SetParent(maskPos);
			newLabel.transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 4f);
		}
	}

	public void CannisterComplete()
	{
		if (!onCannister)
		{
			newLabel.SetActive(false);
			onCannister = true;
		}
	}

	public void ToggleThruster()
	{
		newLabel = UnityEngine.Object.Instantiate(TutorialLabel, base.transform.position, base.transform.rotation);
		newLabel.GetComponentInChildren<Text>().text = ThrusterInfo;
		newLabel.transform.SetParent(maskPos);
		newLabel.transform.localPosition = Vector3.zero;
		Invoke("StartLerp", 4f);
		onThrust = true;
		canThrust = true;
	}

	public void ThrusterComplete()
	{
		if (onThrust)
		{
			newLabel.SetActive(false);
			onThrust = false;
		}
	}

	public void ToggleSalvage()
	{
		if (!onSalvage)
		{
			newLabel = UnityEngine.Object.Instantiate(TutorialLabel, base.transform.position, base.transform.rotation);
			if (!isOculus)
			{
				newLabel.GetComponentInChildren<Text>().text = SalvageInfo[0];
			}
			else
			{
				newLabel.GetComponentInChildren<Text>().text = SalvageInfo[1];
			}
			newLabel.transform.SetParent(maskPos);
			newLabel.transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 4f);
		}
	}

	public void SalvageComplete()
	{
		if (!onSalvage)
		{
			if ((bool)newLabel)
			{
				newLabel.SetActive(false);
			}
			onSalvage = true;
		}
	}

	public void ToggleRepair()
	{
		newLabel = UnityEngine.Object.Instantiate(TutorialLabel, base.transform.position, base.transform.rotation);
		if (!isOculus)
		{
			newLabel.GetComponentInChildren<Text>().text = RepairInfo[0];
		}
		else
		{
			newLabel.GetComponentInChildren<Text>().text = RepairInfo[1];
		}
		newLabel.transform.SetParent(maskPos);
		newLabel.transform.localPosition = Vector3.zero;
		Invoke("StartLerp", 4f);
	}

	public void RepairComplete()
	{
		if (newLabel != null)
		{
			newLabel.SetActive(false);
		}
	}

	public void ToggleGun()
	{
		if (!holstered)
		{
			newLabel = UnityEngine.Object.Instantiate(TutorialLabel, base.transform.position, base.transform.rotation);
			newLabel.GetComponentInChildren<Text>().text = GunInfo;
			newLabel.transform.SetParent(maskPos);
			newLabel.transform.localPosition = Vector3.zero;
			Invoke("StartLerp", 4f);
		}
	}

	public void GunComplete()
	{
		if (!holstered && IntroGunEvent.instance.holdingGun)
		{
			newLabel.SetActive(false);
			IntroGunEvent.instance.OnComplete();
			holstered = true;
		}
	}

	private void StartLerp()
	{
		StartCoroutine("LerpToArm");
	}

	private IEnumerator LerpToArm()
	{
		newLabel.transform.SetParent(armPos);
		float refreshRate = 0.03f;
		while (newLabel.activeSelf)
		{
			newLabel.transform.localPosition = Vector3.Lerp(newLabel.transform.localPosition, Vector3.zero, 5f * refreshRate);
			yield return new WaitForSeconds(refreshRate);
		}
	}

	public void PlayClickAudio()
	{
		if (!GameManager.instance.loading)
		{
			audioSource.PlayOneShot(beep, 0.75f);
		}
	}

	public void ClearPlayTime(int index)
	{
		PreviewLabs.PlayerPrefs.Flush();
		PreviewLabs.PlayerPrefs.DeleteAll();
		PreviewLabs.PlayerPrefs.UpdateSaveSlot(99);
		PreviewLabs.PlayerPrefs.LoadIn();
		PreviewLabs.PlayerPrefs.SetInt("TimePlayed" + index, 0);
		if (PreviewLabs.PlayerPrefs.GetInt("TimePlayed" + index) > 0)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(PreviewLabs.PlayerPrefs.GetInt("TimePlayed" + index));
			string text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			playTimes[index].text = text.ToString();
		}
		else
		{
			playTimes[index].text = "New Game";
		}
		PreviewLabs.PlayerPrefs.Flush();
		SelectSaveSlot(saveSlot);
	}

	private void LoadStartingVariables()
	{
		PreviewLabs.PlayerPrefs.Flush();
		PreviewLabs.PlayerPrefs.DeleteAll();
		PreviewLabs.PlayerPrefs.UpdateSaveSlot(99);
		PreviewLabs.PlayerPrefs.LoadIn();
		saveSlot = PreviewLabs.PlayerPrefs.GetInt("lastSave");
		for (int i = 0; i < 3; i++)
		{
			if (PreviewLabs.PlayerPrefs.GetInt("TimePlayed" + i) > 0)
			{
				activeSlots[i].SetActive(true);
				newSlots[i].SetActive(false);
				TimeSpan timeSpan = TimeSpan.FromSeconds(PreviewLabs.PlayerPrefs.GetInt("TimePlayed" + i));
				string text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				playTimes[i].text = text.ToString();
			}
			else
			{
				playTimes[i].text = "New Game";
			}
			if (PreviewLabs.PlayerPrefs.GetBool("CreativeMode" + i))
			{
				gameType[i].text = "creative";
			}
			else
			{
				gameType[i].text = "normal";
			}
		}
		SelectSaveSlot(saveSlot);
	}

	public void DeleteSaveSlot(int index)
	{
		PreviewLabs.PlayerPrefs.DeleteSaveSlot(index);
		ClearPlayTime(index);
		audioSource.PlayOneShot(deleteBeep);
		activeSlots[index].SetActive(false);
		newSlots[index].SetActive(true);
		playTimes[index].text = "New Game";
	}

	public void SelectSaveSlot(int index)
	{
		saveSlot = index;
		Text[] array = savelots;
		foreach (Text text in array)
		{
			text.color = passiveColor;
		}
		savelots[saveSlot].color = activeColor;
		OnLoadSave();
		PlayClickAudio();
	}

	public void OnStartNewGame(bool creativeMode)
	{
		if (creativeMode)
		{
			PreviewLabs.PlayerPrefs.SetBool("CreativeMode" + saveSlot, true);
		}
		IntroCapsuleEvent.instance.OnLoadGame();
	}

	public void OnContinueQuestion()
	{
		ContinueQuestion.SetActive(true);
	}

	public void OnYes()
	{
		ContinueQuestion.SetActive(false);
		OnContinueGame();
	}

	public void OnCancel()
	{
		ContinueQuestion.SetActive(false);
	}

	public void OnContinueGame()
	{
		IntroCapsuleEvent.instance.OnLoadGame();
	}

	public void OnLoadSave()
	{
		PreviewLabs.PlayerPrefs.Flush();
		PreviewLabs.PlayerPrefs.DeleteAll();
		PreviewLabs.PlayerPrefs.UpdateSaveSlot(99);
		PreviewLabs.PlayerPrefs.LoadIn();
		PreviewLabs.PlayerPrefs.Flush();
		PreviewLabs.PlayerPrefs.DeleteAll();
		PreviewLabs.PlayerPrefs.UpdateSaveSlot(saveSlot);
		PreviewLabs.PlayerPrefs.LoadIn();
	}
}
