using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThreatManager : MonoBehaviour
{
	[Tooltip("Threshold base on general progress before threats can spawn")]
	public int difficultyThreshold = 40;

	[Tooltip("Period when starting game and between threats, before new threats can start")]
	public int gracePeriod = 300;

	internal float timeToThreat = 15f;

	private float interval;

	private int threatIndex;

	private int threatRoll;

	private int cometSize;

	public GameObject countDownLabel;

	public Text countDownText;

	public static ThreatManager instance;

	public AudioClip droneWarning;

	public bool enableUI;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		float time = (float)gracePeriod * Random.RandomRange(1f, 5f);
		Invoke("NewThreat", time);
	}

	private void NewThreat()
	{
		threatRoll = Random.Range(0, 4);
		if (threatRoll < 3)
		{
			threatIndex = 0;
			float num = Random.Range(0, 100);
			num += (float)Random.Range(0, DifficultyManager.instance.GetScore());
			cometSize = 1;
			if (DifficultyManager.instance.GetScore() > 30)
			{
				if (num > 50f && num < 99f)
				{
					CometManager.instance.presetCometSize = CometManager.instance.Comet_S2;
					cometSize = 2;
				}
				else if (num > 98f)
				{
					CometManager.instance.presetCometSize = CometManager.instance.Comet_S3;
					cometSize = 3;
				}
			}
		}
		else
		{
			threatIndex = 1;
		}
		interval = Mathf.Clamp(400f - (float)DifficultyManager.instance.GetScore() * 1.5f, 180f, 400f);
		ThreatAnnouncement();
	}

	private IEnumerator ThreatCountdown()
	{
		while (timeToThreat > 0f)
		{
			timeToThreat -= 1f;
			if (threatIndex == 0)
			{
				if (cometSize < 2)
				{
					countDownText.text = "Class 1 comet incoming: " + timeToThreat;
				}
				else if (cometSize == 2)
				{
					countDownText.text = "Class 2 comet incoming: " + timeToThreat;
				}
				else if (cometSize == 3)
				{
					countDownText.text = "Class 3 comet incoming: " + timeToThreat;
				}
			}
			else if (threatIndex == 1)
			{
				countDownText.text = "Drone Attack incoming: " + timeToThreat;
			}
			yield return new WaitForSecondsRealtime(1f);
		}
		SpawnThreat();
		Invoke("NewThreat", gracePeriod * Random.RandomRange(1, 5));
	}

	private void SpawnThreat()
	{
		if (threatIndex == 0)
		{
			CometManager.instance.SpawnComets();
		}
		else
		{
			Debug.Log("SpawnDrones");
			int count = Random.Range(1, DifficultyManager.instance.GetScore() / 12);
			DroneManager.instance.SpawnAttackDrones(count);
		}
		countDownLabel.SetActive(false);
	}

	public void OnEnableUI()
	{
		enableUI = true;
	}

	private void ThreatAnnouncement()
	{
		if (enableUI)
		{
			countDownLabel.SetActive(true);
		}
		if (threatIndex == 0)
		{
			SuitManager.instance.CometPromt(Mathf.Clamp(cometSize - 1, 0, 3));
		}
		else if (threatIndex == 1)
		{
			GameAudioManager.instance.AddToSuitQueue(droneWarning);
		}
		StartCoroutine("ThreatCountdown");
	}
}
