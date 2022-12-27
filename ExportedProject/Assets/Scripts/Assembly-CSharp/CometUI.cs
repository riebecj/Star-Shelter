using System.Collections;
using UnityEngine;

public class CometUI : MonoBehaviour
{
	internal Transform cam;

	private LineRenderer trajectoryLine;

	private Transform target;

	private Vector3 goal;

	private Rigidbody body;

	private void OnEnable()
	{
		Invoke("Setup", 0.1f);
	}

	private void Setup()
	{
		cam = Camera.main.transform;
		body = base.transform.parent.GetComponent<Rigidbody>();
		target = BaseManager.instance.transform;
		GameObject gameObject = Object.Instantiate(GameManager.instance.cometLine, base.transform.position, base.transform.rotation);
		gameObject.transform.SetParent(base.transform);
		trajectoryLine = gameObject.GetComponent<LineRenderer>();
		if (base.isActiveAndEnabled)
		{
			StartCoroutine("UpdateUI");
		}
	}

	private IEnumerator UpdateUI()
	{
		while (true)
		{
			base.transform.LookAt(base.transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
			if (Vector3.Distance(base.transform.position, target.position) < 10f || Vector3.Distance(base.transform.position, cam.position) < 15f)
			{
				base.gameObject.SetActive(false);
			}
			trajectoryLine.SetPosition(0, base.transform.position);
			float distance = Vector3.Distance(base.transform.position, target.position) - 3f;
			goal = base.transform.position + body.velocity.normalized * distance;
			trajectoryLine.SetPosition(1, goal);
			yield return new WaitForSeconds(0.05f);
		}
	}
}
