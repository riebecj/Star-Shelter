using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK;

public class JumpShip : MonoBehaviour
{
	public Animator anim;

	public AudioSource audioSource;

	public AudioClip cryoSleepAudio;

	internal float corePowerA;

	internal float corePowerB;

	internal float corePowerC;

	public Image coreRingA;

	public Image coreRingB;

	public Image coreRingC;

	public GameObject stabilizedCoresPromt;

	public GameObject warningPromt;

	public GameObject coreMissingPromt;

	public GameObject lightTunnel;

	public GameObject jumpButton;

	public GameObject meters;

	public GameObject startUI;

	public GameObject hangar;

	public Transform headPos;

	public Transform footTargetL;

	public Transform footTargetR;

	public Transform pelvisTarget;

	public GameObject[] unloadObjects;

	internal int craftedPartIndex;

	public float regulationSpeed = 0.01f;

	public float tempSpeed = 0.004f;

	public AudioSource[] EngineAudioSources;

	internal bool fase2;

	private void Start()
	{
	}

	public void CheckIfComplete()
	{
		craftedPartIndex++;
		if (craftedPartIndex >= 4)
		{
			startUI.SetActive(true);
		}
	}

	public void StartShip()
	{
		if (BaseManager.instance.coresInPlace >= 3 || GameManager.instance.debugMode)
		{
			Debug.Log("Cores " + BaseManager.instance.coresInPlace);
			anim.SetBool("Open", true);
			StartCoroutine("MovetoGoal");
			StartCoroutine("LerpBodyParts");
			coreRingA.rectTransform.localScale = Vector3.one * 0.25f;
			coreRingB.rectTransform.localScale = Vector3.one * 0.25f;
			coreRingC.rectTransform.localScale = Vector3.one * 0.25f;
		}
		else
		{
			coreMissingPromt.SetActive(true);
		}
	}

	public void LaunchShip()
	{
		jumpButton.SetActive(false);
		Invoke("FadeOut", 12f);
		Invoke("ActivateTunnel", 1f);
		Invoke("FusionEvent", 22f);
	}

	private void UnloadWorld()
	{
		for (int i = 0; i < StationManager.instance.spawnedStations.Count; i++)
		{
			Object.Destroy(StationManager.instance.spawnedStations[i].gameObject);
		}
		StationManager.instance.spawnedStations.Clear();
		WreckManager.instance.StopCoroutine("SpawnWreck");
		for (int j = 0; j < WreckManager.instance.activeWrecks.Count; j++)
		{
			WreckManager.instance.activeWrecks[j].SetActive(false);
		}
		WreckManager.instance.ClearWrecks();
		for (int k = 0; k < AddToInventory.inventoryObjects.Count; k++)
		{
			Object.Destroy(AddToInventory.inventoryObjects[k].gameObject);
		}
		for (int l = 0; l < DroneAI.spawnedDrones.Count; l++)
		{
			Object.Destroy(DroneAI.spawnedDrones[l].gameObject);
		}
		for (int m = 0; m < unloadObjects.Length; m++)
		{
			unloadObjects[m].SetActive(false);
		}
		CometManager.instance.StopAllCoroutines();
		for (int n = 0; n < CometManager.instance.activeComets.Count; n++)
		{
			CometManager.instance.activeComets[n].gameObject.SetActive(false);
		}
		Renderer[] componentsInChildren = BaseManager.instance.GetComponentsInChildren<Renderer>();
		for (int num = 0; num < componentsInChildren.Length; num++)
		{
			componentsInChildren[num].enabled = false;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("World");
		gameObject.SetActive(false);
	}

	private void LoadEarth()
	{
		SceneManager.LoadScene("EarthScene");
	}

	private void FusionEvent()
	{
		FadeIn();
		Invoke("StartFusion", 1f);
	}

	public void RegulateCorePower(int index, float power)
	{
		power = power / 15f * regulationSpeed;
		switch (index)
		{
		case 0:
			corePowerA += power;
			corePowerA = Mathf.Clamp(corePowerA, 0.05f, 2f);
			coreRingA.rectTransform.localScale = Vector3.one * corePowerA;
			EngineAudioSources[0].pitch = 0.15f + corePowerA * 2f;
			break;
		case 1:
			corePowerB += power;
			corePowerB = Mathf.Clamp(corePowerB, 0.05f, 2f);
			coreRingB.rectTransform.localScale = Vector3.one * corePowerB;
			EngineAudioSources[1].pitch = 0.15f + corePowerB * 2f;
			break;
		case 2:
			corePowerC += power;
			corePowerC = Mathf.Clamp(corePowerC, 0.05f, 2f);
			coreRingC.rectTransform.localScale = Vector3.one * corePowerC;
			EngineAudioSources[2].pitch = 0.15f + corePowerC * 2f;
			break;
		}
		CheckCoreBalance();
	}

	private void CheckCoreBalance()
	{
		bool flag = corePowerA > 0.5f && corePowerA < 1.45f;
		bool flag2 = corePowerB > 0.5f && corePowerB < 1.45f;
		bool flag3 = corePowerC > 0.5f && corePowerC < 1.45f;
		if (flag && flag2 && flag3)
		{
			OnBalance();
		}
	}

	private void OnBalance()
	{
		if (meters.activeSelf)
		{
			anim.enabled = false;
			Invoke("FadeMeters", 0.5f);
			warningPromt.GetComponent<Animator>().SetBool("Stable", true);
			stabilizedCoresPromt.SetActive(false);
			if (fase2)
			{
				Invoke("ContinueTravel", 3f);
			}
		}
	}

	public void ContinueTravel()
	{
		FadeOut();
		Invoke("LoadEarth", 3f);
	}

	private void FadeMeters()
	{
		StartCoroutine("ScaleDownMeters");
	}

	private void ActivateTunnel()
	{
		lightTunnel.SetActive(true);
		StartCoroutine("LerpBase");
		Invoke("UnloadWorld", 0.5f);
	}

	private void FadeIn()
	{
		VRTK_ScreenFade.Start(Color.clear, 3f);
	}

	private void FadeOut()
	{
		VRTK_ScreenFade.Start(Color.black, 3f);
	}

	private void StartFusion()
	{
		fase2 = true;
		corePowerA = 0.05f;
		corePowerB = 1.5f;
		corePowerC = 1.5f;
		coreRingA.rectTransform.localScale = Vector3.one * corePowerA;
		coreRingB.rectTransform.localScale = Vector3.one * corePowerB;
		coreRingC.rectTransform.localScale = Vector3.one * corePowerC;
		warningPromt.SetActive(true);
		meters.SetActive(true);
		meters.transform.localScale = Vector3.one;
		StartCoroutine("ColdFusionEvent");
		StartCoroutine("HotFusionEvent");
	}

	private IEnumerator ColdFusionEvent()
	{
		while (true)
		{
			if (corePowerA > 0.05f)
			{
				corePowerA -= tempSpeed;
			}
			coreRingA.rectTransform.localScale = Vector3.one * corePowerA;
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator HotFusionEvent()
	{
		while (true)
		{
			if (corePowerC < 1.5f)
			{
				corePowerC += tempSpeed;
			}
			coreRingC.rectTransform.localScale = Vector3.one * corePowerC;
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator LerpBase()
	{
		base.transform.SetParent(null);
		Collider[] colliders = hangar.GetComponentsInChildren<Collider>();
		Collider[] array = colliders;
		foreach (Collider collider in array)
		{
			collider.enabled = false;
		}
		while (true)
		{
			hangar.transform.position += base.transform.forward * 0.02f * -3f;
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator MovetoGoal()
	{
		int count = 0;
		GameManager.instance.CamRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
		foreach (Thruster thruster in Thruster.thrusters)
		{
			thruster.deactivated = true;
		}
		VRTK_PlayerClimb.instance.disabled = true;
		while (count < 100)
		{
			count++;
			Vector3 relativePos = GameManager.instance.Head.position - GameManager.instance.CamRig.position;
			GameManager.instance.CamRig.position = Vector3.Lerp(GameManager.instance.CamRig.position, headPos.position - relativePos, Time.deltaTime * 3f);
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator ScaleDownMeters()
	{
		while (meters.transform.localScale.x > 0.05f)
		{
			meters.transform.localScale = Vector3.Lerp(meters.transform.localScale, Vector3.zero, 0.02f);
			yield return new WaitForSeconds(0.02f);
		}
		meters.SetActive(false);
		if (!fase2)
		{
			jumpButton.SetActive(true);
		}
	}

	private IEnumerator LerpBodyParts()
	{
		Transform FR = RagdollRef.instance.FootTargetR;
		Transform FL = RagdollRef.instance.FootTargetL;
		Transform PT = RagdollRef.instance.PelvisTarget;
		FR.SetParent(null);
		FL.SetParent(null);
		PT.SetParent(null);
		while (true)
		{
			FR.transform.position = Vector3.Lerp(FR.transform.position, footTargetR.position, 15f * Time.deltaTime);
			FL.transform.position = Vector3.Lerp(FL.transform.position, footTargetL.position, 15f * Time.deltaTime);
			PT.transform.position = Vector3.Lerp(PT.transform.position, pelvisTarget.position, 15f * Time.deltaTime);
			FR.transform.rotation = Quaternion.Lerp(FR.transform.rotation, footTargetR.rotation, 15f * Time.deltaTime);
			FL.transform.rotation = Quaternion.Lerp(FL.transform.rotation, footTargetL.rotation, 15f * Time.deltaTime);
			PT.transform.rotation = Quaternion.Lerp(PT.transform.rotation, pelvisTarget.rotation, 15f * Time.deltaTime);
			yield return new WaitForSeconds(0.02f);
		}
	}
}
