using System.Collections.Generic;
using UnityEngine;

public class TrailSpawner : MonoBehaviour
{
	public GameObject trailObject;

	public Transform target;

	private int trailCount = 6;

	private bool generated;

	private List<Transform> trails = new List<Transform>();

	private void OnEnable()
	{
		if (!generated)
		{
			GenerateTrail();
		}
	}

	private void GenerateTrail()
	{
		generated = true;
		for (int i = 0; i < trailCount; i++)
		{
			GameObject gameObject = Object.Instantiate(trailObject, base.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0.1f * (float)i);
			trails.Add(gameObject.transform);
		}
		if ((bool)GetComponent<AudioSource>())
		{
			GetComponent<AudioSource>().Play();
		}
	}

	private void Update()
	{
		Vector3 vector = target.position + target.transform.forward * 0.15f;
		for (int i = 0; i < trails.Count; i++)
		{
			trails[i].localPosition = new Vector3(0f, 0f, Vector3.Distance(base.transform.position, vector) / (float)trailCount * (float)i * 1000f);
		}
		base.transform.LookAt(vector);
	}
}
