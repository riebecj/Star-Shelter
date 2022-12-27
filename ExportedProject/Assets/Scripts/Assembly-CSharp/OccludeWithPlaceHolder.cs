using System.Collections;
using UnityEngine;

public class OccludeWithPlaceHolder : MonoBehaviour
{
	private Transform player;

	public int distance = 25;

	internal bool changing;

	internal bool active;

	public GameObject placeHolder;

	private void Start()
	{
		player = GameManager.instance.Head;
		active = true;
		StartCoroutine("UpdateOcclusion");
	}

	private IEnumerator UpdateOcclusion()
	{
		while (true)
		{
			if (Vector3.Distance(base.transform.position, player.position) < (float)distance)
			{
				if (!changing && !active)
				{
					ToggleChildren(true);
				}
			}
			else if (Vector3.Distance(base.transform.position, player.position) > (float)(distance + 10) && !changing && active)
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
		placeHolder.SetActive(false);
		changing = false;
		active = true;
	}

	private IEnumerator DisableChildren()
	{
		placeHolder.SetActive(true);
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
}
