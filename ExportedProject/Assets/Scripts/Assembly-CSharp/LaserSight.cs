using System.Collections;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
	public Transform laser;

	private LineRenderer line;

	public GameObject endPoint;

	private void Awake()
	{
		line = laser.GetComponent<LineRenderer>();
	}

	public void ToggleLaser(bool On)
	{
		if (On)
		{
			line.enabled = true;
			StartCoroutine("UpdateLength");
			endPoint.SetActive(true);
		}
		else
		{
			StopCoroutine("UpdateLength");
			line.enabled = false;
			endPoint.SetActive(false);
		}
	}

	private IEnumerator UpdateLength()
	{
		while (true)
		{
			RaycastHit hit;
			if (Physics.Raycast(laser.position, -laser.up, out hit, 300f))
			{
				float num = Vector3.Distance(laser.position, hit.point);
				float num2 = Mathf.Clamp(num / 2000f, 0.01f, 0.1f);
				endPoint.transform.localScale = Vector3.one * num2;
				line.SetPosition(1, new Vector3(0f, 0f - num, 0f));
			}
			else
			{
				endPoint.transform.localScale = Vector3.zero;
				line.SetPosition(1, new Vector3(0f, -100f, 0f));
			}
			yield return new WaitForSeconds(0.05f);
		}
	}
}
