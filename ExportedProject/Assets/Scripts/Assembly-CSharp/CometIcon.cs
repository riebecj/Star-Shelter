using System.Collections;
using UnityEngine;

public class CometIcon : MonoBehaviour
{
	internal Transform impactPoint;

	internal LayerMask layerMask;

	internal Transform cam;

	private LineRenderer trajectoryLine;

	private Transform target;

	private Vector3 goal;

	internal HoloMap map;

	private void Awake()
	{
		Invoke("DelayScale", 1f);
		Setup();
	}

	private void DelayScale()
	{
		base.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
	}

	private void Start()
	{
	}

	private void Setup()
	{
		cam = Camera.main.transform;
		GameObject gameObject = Object.Instantiate(GameManager.instance.mapCometLine, base.transform.position, base.transform.rotation);
		gameObject.transform.SetParent(base.transform);
		trajectoryLine = gameObject.GetComponent<LineRenderer>();
		trajectoryLine.widthMultiplier = 0.005f;
	}

	private IEnumerator UpdateImpactPoint()
	{
		while (true)
		{
			RaycastHit hit;
			if (Physics.Raycast(base.transform.position, base.transform.forward, out hit, 10f, layerMask))
			{
				impactPoint.position = hit.point;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator UpdateUI()
	{
		while (true)
		{
			Debug.Log("UpdateUI");
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			base.transform.LookAt(base.transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
			trajectoryLine.SetPosition(0, base.transform.position);
			goal = map.rooms[0].position;
			trajectoryLine.SetPosition(1, goal);
			yield return new WaitForSeconds(0.05f);
		}
	}
}
