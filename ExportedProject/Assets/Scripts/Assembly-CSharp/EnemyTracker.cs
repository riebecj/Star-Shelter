using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTracker : MonoBehaviour
{
	public GameObject tracker;

	internal bool tracking;

	public Transform UIBase;

	internal Transform head;

	public List<Transform> Enemies = new List<Transform>();

	public List<Transform> trackers = new List<Transform>();

	public static EnemyTracker instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		head = GameManager.instance.Head;
	}

	public void OnAddTracker(Transform newEnemy)
	{
		if (!Enemies.Contains(newEnemy))
		{
			Enemies.Add(newEnemy);
			GameObject gameObject = Object.Instantiate(tracker, base.transform.position, base.transform.rotation);
			gameObject.transform.SetParent(UIBase);
			gameObject.transform.localScale = Vector3.one * 2f;
			trackers.Add(gameObject.transform);
			if (!tracking)
			{
				tracking = true;
				StartCoroutine("UpdateState");
				SuitManager.instance.DronePromt();
			}
		}
	}

	public void OnRemoveTracker(Transform newEnemy)
	{
		if (!Enemies.Contains(newEnemy))
		{
			return;
		}
		GameObject gameObject = null;
		for (int i = 0; i < Enemies.Count; i++)
		{
			if (Enemies[i] == newEnemy)
			{
				gameObject = trackers[i].gameObject;
				Enemies.Remove(newEnemy);
				trackers.Remove(trackers[i]);
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
			}
		}
	}

	private IEnumerator UpdateState()
	{
		while (tracking)
		{
			for (int i = 0; i < Enemies.Count; i++)
			{
				Vector3 vector = Enemies[i].position - head.position;
				if (vector.magnitude > 50f)
				{
					OnRemoveTracker(Enemies[i]);
					continue;
				}
				trackers[i].transform.position = UIBase.position + Vector3.ClampMagnitude(vector, 0.24f);
				trackers[i].localPosition = new Vector3(Mathf.Clamp(trackers[i].localPosition.x, -30f, 30f), Mathf.Clamp(trackers[i].localPosition.y, -15f, 15f), 0f);
				if (trackers[i].localPosition.magnitude < 10f)
				{
					trackers[i].GetComponent<Image>().enabled = false;
				}
				else
				{
					trackers[i].GetComponent<Image>().enabled = true;
				}
			}
			yield return new WaitForSeconds(0.011f);
		}
	}
}
