using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenCheck : MonoBehaviour
{
	private Collider[] cols;

	public List<Collider> colliders = new List<Collider>();

	public static OxygenCheck instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		ColliderCheck();
		StartCoroutine("UpdateState");
	}

	public void ColliderCheck()
	{
		cols = GetComponentsInChildren<Collider>();
		for (int i = 0; i < cols.Length; i++)
		{
			if (cols[i].name.Contains("OxygenCollider"))
			{
				colliders.Add(cols[i]);
			}
		}
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			for (int i = 0; i < colliders.Count; i++)
			{
				if (colliders[i] != null && (colliders[i].bounds.Contains(GameManager.instance.Head.position) || colliders[i].bounds.Contains(DroneHelper.instance.proxyPlayer.transform.position)))
				{
					BaseManager.instance.inBase = true;
					BaseManager.instance.currentOxygenGroup = colliders[i].GetComponentInParent<Room>().group;
					CancelInvoke("Exit");
					Invoke("Exit", 0.5f);
				}
				if (colliders[i] != null && colliders[i].bounds.Contains(DroneHelper.instance.transform.position))
				{
					DroneHelper.instance.inBase = true;
					CancelInvoke("DronExit");
					Invoke("DronExit", 0.5f);
				}
			}
			yield return new WaitForSeconds(0.25f);
		}
	}

	private void DronExit()
	{
		DroneHelper.instance.inBase = false;
	}

	private void Exit()
	{
		BaseManager.instance.inBase = false;
		BaseManager.instance.Invoke("OnExitBase", 0.5f);
	}
}
