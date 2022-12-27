using System;
using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class CometManager : MonoBehaviour
{
	public List<Transform> activeComets = new List<Transform>();

	public GameObject Comet_S1;

	public GameObject Comet_S2;

	public GameObject Comet_S3;

	public GameObject cometRain;

	internal GameObject presetCometSize;

	internal float interval = 400f;

	internal float gracePeriod = 500f;

	public int maxCometSpawn = 3;

	public int spawnCount = 1;

	internal Transform target;

	internal Transform player;

	public bool disalbeCometSpawn;

	public static CometManager instance;

	public Transform TutorialCometPos;

	public LayerMask damageMask;

	private void Awake()
	{
		instance = this;
	}

	public void Restart()
	{
		for (int i = 0; i < activeComets.Count; i++)
		{
			UnityEngine.Object.Destroy(activeComets[i].gameObject);
		}
		PreviewLabs.PlayerPrefs.DeleteKey("cometInterval");
		Start();
	}

	private void Start()
	{
		if (!GameManager.instance.creativeMode)
		{
			player = GameManager.instance.CamRig.transform;
			if (Application.loadedLevelName == "MainScene")
			{
				target = BaseManager.instance.transform;
				StartCoroutine("SpawnWorldComet");
				StartCoroutine("SpawnCometShower");
				OnStart();
			}
			else
			{
				instance = null;
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public void OnStart()
	{
		if (PreviewLabs.PlayerPrefs.HasKey("cometInterval"))
		{
			interval = PreviewLabs.PlayerPrefs.GetFloat("cometInterval");
			spawnCount = PreviewLabs.PlayerPrefs.GetInt("SpawnCount");
			gracePeriod = 400f;
		}
		else
		{
			interval = 400f;
			spawnCount = 1;
			gracePeriod = 1000f;
		}
		if (GameManager.instance.debugComets)
		{
			gracePeriod = 5f;
			interval = 10f;
		}
		Invoke("StartWave", gracePeriod);
	}

	private void StartWave()
	{
	}

	private IEnumerator SpawnComet()
	{
		while (true)
		{
			if (DifficultyManager.instance.GetScore() > 40)
			{
				spawnCount = Mathf.Clamp(DifficultyManager.instance.GetScore() / 12, 1, 3);
			}
			interval = Mathf.Clamp(400f - (float)DifficultyManager.instance.GetScore() * 1.5f, 180f, 400f);
			for (int i = 0; i < spawnCount; i++)
			{
				if (disalbeCometSpawn)
				{
					continue;
				}
				float radius = 400f;
				GameObject original = Comet_S1;
				if (GameManager.instance.debugComets)
				{
					radius = UnityEngine.Random.Range(20, 25);
				}
				float num = UnityEngine.Random.Range(0, 100);
				num += (float)UnityEngine.Random.Range(0, DifficultyManager.instance.GetScore());
				if (DifficultyManager.instance.GetScore() > 30)
				{
					if (num > 50f && num < 99f)
					{
						original = Comet_S2;
					}
					else if (num > 98f)
					{
						original = Comet_S3;
					}
				}
				target = BaseLoader.instance.AllRooms[UnityEngine.Random.Range(0, BaseLoader.instance.AllRooms.Count)].transform;
				Vector3 vector = RandomCircle(target.position + new Vector3(0f, UnityEngine.Random.Range(-1, 1), 0f), radius);
				GameObject gameObject = UnityEngine.Object.Instantiate(original, vector, Quaternion.LookRotation(target.position - vector));
				gameObject.GetComponent<Rigidbody>().velocity = (target.position + new Vector3(0f, UnityEngine.Random.Range(0, 6), 0f) - gameObject.transform.position).normalized * 10f;
				activeComets.Add(gameObject.transform);
			}
			yield return new WaitForSeconds(interval);
		}
	}

	public void SpawnComets()
	{
		if (DifficultyManager.instance.GetScore() > 40)
		{
			spawnCount = Mathf.Clamp(DifficultyManager.instance.GetScore() / 12, 1, 3);
		}
		for (int i = 0; i < spawnCount; i++)
		{
			if (!disalbeCometSpawn)
			{
				float radius = 200f;
				GameObject comet_S = Comet_S1;
				if (presetCometSize != null)
				{
					comet_S = presetCometSize;
				}
				if (GameManager.instance.debugComets)
				{
					radius = UnityEngine.Random.Range(20, 25);
				}
				target = BaseLoader.instance.AllRooms[UnityEngine.Random.Range(0, BaseLoader.instance.AllRooms.Count)].transform;
				Vector3 vector = RandomCircle(target.position + new Vector3(0f, UnityEngine.Random.Range(-1, 1), 0f), radius);
				GameObject gameObject = UnityEngine.Object.Instantiate(comet_S, vector, Quaternion.LookRotation(target.position - vector));
				gameObject.GetComponent<Rigidbody>().velocity = (target.position + new Vector3(0f, UnityEngine.Random.Range(0, 6), 0f) - gameObject.transform.position).normalized * 10f;
				activeComets.Add(gameObject.transform);
			}
		}
	}

	private IEnumerator SpawnWorldComet()
	{
		while (true)
		{
			if (target != null && Vector3.Distance(player.position, target.position) > 25f && Camera.main.farClipPlane > 100f)
			{
				WorldComet();
			}
			yield return new WaitForSeconds(10f);
		}
	}

	private IEnumerator SpawnCometShower()
	{
		while (true)
		{
			if (target != null && Vector3.Distance(player.position, target.position) > 25f && Camera.main.farClipPlane > 100f)
			{
				SuitManager.instance.CometShowerPromt();
				Invoke("CometShower", 7f);
			}
			yield return new WaitForSeconds(600 - DifficultyManager.instance.GetScore() * 4);
		}
	}

	public void TestComet()
	{
		GameObject comet_S = Comet_S1;
		GameObject gameObject = UnityEngine.Object.Instantiate(comet_S, TutorialCometPos.position, Quaternion.LookRotation(target.position - TutorialCometPos.position));
		gameObject.GetComponent<Rigidbody>().velocity = (target.position + new Vector3(0f, UnityEngine.Random.Range(0f, 1.5f), 0f) - gameObject.transform.position).normalized * 10f;
		activeComets.Add(gameObject.transform);
	}

	public void KillComet()
	{
		GameObject comet_S = Comet_S2;
		float radius = 50f;
		float num = UnityEngine.Random.Range(-8, 8);
		num = UnityEngine.Random.Range(1, 1);
		Vector3 vector = RandomCircle(target.position + new Vector3(num, num, num), radius);
		GameObject gameObject = UnityEngine.Object.Instantiate(comet_S, vector, Quaternion.LookRotation(target.position - vector));
		gameObject.GetComponent<Rigidbody>().velocity = (target.position + new Vector3(num, num, num) - gameObject.transform.position).normalized * 10f;
		activeComets.Add(gameObject.transform);
	}

	public void CometShower()
	{
		cometRain.SetActive(true);
		cometRain.transform.parent.localEulerAngles = new Vector3(0f, UnityEngine.Random.Range(0, 360), 0f);
		ParticleSystem[] componentsInChildren = cometRain.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			main.loop = true;
			particleSystem.Play();
		}
		Invoke("StopShower", 20f);
	}

	private void StopShower()
	{
		ParticleSystem[] componentsInChildren = cometRain.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			main.loop = false;
			particleSystem.Clear();
		}
		Invoke("DisableShower", 5f);
	}

	private void DisableShower()
	{
		cometRain.SetActive(false);
	}

	public void WorldComet()
	{
		int num = UnityEngine.Random.Range(0, 2);
		GameObject original = Comet_S1;
		switch (num)
		{
		case 1:
			original = Comet_S2;
			break;
		case 2:
			original = Comet_S3;
			break;
		}
		float radius = 50f;
		Vector3 vector = RandomCircle(player.position + new Vector3(0f, UnityEngine.Random.Range(0, 6), 0f), radius);
		GameObject gameObject = UnityEngine.Object.Instantiate(original, vector, Quaternion.LookRotation(target.position - vector));
		gameObject.GetComponent<Rigidbody>().velocity = (player.position + new Vector3(UnityEngine.Random.Range(-25, 25), UnityEngine.Random.Range(-25, 25), 0f) - gameObject.transform.position).normalized * 10f;
		gameObject.GetComponent<Comet>().CancelInvoke("AddToMap");
		gameObject.GetComponent<Comet>().worldComet = true;
		gameObject.GetComponentInChildren<CometUI>().gameObject.SetActive(false);
		activeComets.Add(gameObject.transform);
	}

	private Vector3 RandomCircle(Vector3 center, float radius)
	{
		float num = UnityEngine.Random.value * 360f;
		Vector3 result = default(Vector3);
		result.x = center.x + radius * Mathf.Sin(num * ((float)Math.PI / 180f));
		result.y = center.y;
		result.z = center.z + radius * Mathf.Cos(num * ((float)Math.PI / 180f));
		return result;
	}

	public void OnSave()
	{
		PreviewLabs.PlayerPrefs.SetFloat("cometInterval", interval);
		PreviewLabs.PlayerPrefs.SetInt("SpawnCount", spawnCount);
	}
}
