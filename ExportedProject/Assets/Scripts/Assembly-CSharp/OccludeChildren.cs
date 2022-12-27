using System.Collections;
using UnityEngine;

public class OccludeChildren : MonoBehaviour
{
	private Transform player;

	public int distance = 25;

	internal bool changing;

	internal bool active;

	public Transform center;

	private void Start()
	{
		player = GameManager.instance.Head;
		active = true;
		if (center == null)
		{
			center = base.transform;
		}
		StartCoroutine("UpdateOcclusion");
	}

	private IEnumerator UpdateOcclusion()
	{
		while (true)
		{
			if (DistanceToPlayer() < (float)distance)
			{
				if (!changing && !active)
				{
					ToggleChildren(true);
				}
			}
			else if (DistanceToPlayer() > (float)(distance + 10) && !changing && active)
			{
				ToggleChildren(false);
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void ToggleChildren(bool On)
	{
		if (On)
		{
			StartCoroutine("EnableChildren");
		}
		else
		{
			StartCoroutine("DisableChildren");
		}
	}

	private IEnumerator EnableChildren()
	{
		changing = true;
		int index = 0;
		while (index < base.transform.childCount - 1)
		{
			MeshRenderer[] renderers = base.transform.GetChild(index).GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = renderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = true;
			}
			index++;
			yield return new WaitForSeconds(0.001f);
		}
		changing = false;
		active = true;
	}

	private IEnumerator DisableChildren()
	{
		changing = true;
		int index = 0;
		while (index < base.transform.childCount - 1)
		{
			for (int i = 0; i < 5; i++)
			{
				if (index < base.transform.childCount - 1)
				{
					MeshRenderer[] componentsInChildren = base.transform.GetChild(index).GetComponentsInChildren<MeshRenderer>();
					MeshRenderer[] array = componentsInChildren;
					foreach (MeshRenderer meshRenderer in array)
					{
						meshRenderer.enabled = false;
					}
				}
				index++;
			}
			yield return new WaitForSeconds(0.001f);
		}
		changing = false;
		active = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (center != null)
		{
			Gizmos.DrawWireSphere(center.position, distance);
		}
		else
		{
			Gizmos.DrawWireSphere(base.transform.position, distance);
		}
	}

	private float DistanceToPlayer()
	{
		float num = Vector3.Distance(center.position, player.position);
		if (DroneHelper.instance.PCParts.activeSelf && Vector3.Distance(center.position, DroneHelper.instance.transform.position) < num)
		{
			num = Vector3.Distance(center.position, DroneHelper.instance.transform.position);
		}
		return num;
	}
}
