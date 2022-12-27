using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckManager : MonoBehaviour
{
	public GameObject[] wrecks;

	public int wreckSpawnCount = 3;

	internal int wreckCap = 10;

	internal List<GameObject> activeWrecks = new List<GameObject>();

	public List<GameObject> LooseDebris = new List<GameObject>();

	internal Transform target;

	public static WreckManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (Application.loadedLevelName == "MainScene" && !GameManager.instance.creativeMode)
		{
			OnStart();
		}
	}

	private void OnStart()
	{
		target = BaseManager.instance.transform;
		StartCoroutine("SpawnWreck", 60);
		StartCoroutine("CleanUpWrecks", 1);
	}

	private IEnumerator SpawnWreck(float waitTime)
	{
		while (true)
		{
			if (activeWrecks.Count < 10)
			{
				OnWreckSpawn();
			}
			yield return new WaitForSeconds(waitTime);
		}
	}

	public void OnWreckSpawn()
	{
		int num = 100;
		for (int i = 0; i < wreckSpawnCount; i++)
		{
			Vector3 vector = Vector3.right;
			if (UnityEngine.Random.Range(0, 4) > 2)
			{
				vector = Vector3.left;
			}
			Vector3 vector2 = RandomCircle(target.position + new Vector3(0f, UnityEngine.Random.Range(-25, 25), 0f), UnityEngine.Random.Range(60, 125));
			if (NoOverlap(vector2) && NoCollision(vector2))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(wrecks[UnityEngine.Random.Range(0, wrecks.Length)], vector2, Quaternion.identity);
				Vector3 normalized = (BaseManager.instance.transform.position + UnityEngine.Random.Range(40, 80) * vector - vector2).normalized;
				gameObject.GetComponent<Wreckage>().Direction = normalized;
				Vector3 from = base.transform.position - gameObject.transform.position;
				float num2 = Vector3.Angle(from, normalized);
				if (num2 < 10f)
				{
					if (gameObject.transform.position.y > 0f)
					{
						gameObject.GetComponent<Wreckage>().Direction = new Vector3(normalized.x, 0.2f, normalized.z);
					}
					else
					{
						gameObject.GetComponent<Wreckage>().Direction = new Vector3(normalized.x, -0.2f, normalized.z);
					}
				}
				gameObject.GetComponent<Wreckage>().speed = UnityEngine.Random.Range(1, 2);
				activeWrecks.Add(gameObject);
			}
			else if (num > 0)
			{
				i--;
				num--;
			}
		}
	}

	private IEnumerator CleanUpWrecks(float waitTime)
	{
		while (true)
		{
			if (activeWrecks.Count > 5)
			{
				for (int i = 0; i < 3; i++)
				{
					if (Vector3.Distance(activeWrecks[i].transform.position, GameManager.instance.CamRig.position) > 100f && !I_Can_See(activeWrecks[i]))
					{
						GameObject obj = activeWrecks[i];
						activeWrecks.Remove(activeWrecks[i]);
						UnityEngine.Object.DestroyObject(obj);
					}
				}
			}
			if (LooseDebris.Count > 5)
			{
				for (int j = 0; j < 3; j++)
				{
					if (LooseDebris[j] != null)
					{
						if (Vector3.Distance(LooseDebris[j].transform.position, GameManager.instance.CamRig.position) > 25f && !I_Can_See(LooseDebris[j]))
						{
							GameObject obj2 = LooseDebris[j];
							LooseDebris.Remove(LooseDebris[j]);
							UnityEngine.Object.DestroyObject(obj2);
						}
					}
					else
					{
						LooseDebris.Remove(LooseDebris[j]);
					}
				}
			}
			yield return new WaitForSeconds(waitTime);
		}
	}

	public void ClearWrecks()
	{
		for (int i = 0; i < activeWrecks.Count; i++)
		{
			GameObject obj = activeWrecks[i];
			activeWrecks.Remove(activeWrecks[i]);
			UnityEngine.Object.DestroyObject(obj);
		}
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

	private bool NoCollision(Vector3 newSpawnPos)
	{
		Collider[] array = Physics.OverlapSphere(newSpawnPos, 25f);
		if (array.Length > 0)
		{
			return false;
		}
		return true;
	}

	private bool NoOverlap(Vector3 newSpawnPos)
	{
		float num = float.PositiveInfinity;
		foreach (GameObject activeWreck in activeWrecks)
		{
			float num2 = Vector3.Distance(activeWreck.transform.position, newSpawnPos);
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (num > 30f)
		{
			return true;
		}
		return false;
	}

	private bool I_Can_See(GameObject Object)
	{
		if (Object.GetComponentInChildren<Collider>() == null)
		{
			return true;
		}
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		if (GeometryUtility.TestPlanesAABB(planes, Object.GetComponentInChildren<Collider>().bounds))
		{
			return true;
		}
		return false;
	}
}
