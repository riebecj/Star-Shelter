using System.Collections;
using UnityEngine;

public class DistanceIconScaler : MonoBehaviour
{
	public Transform distanceIcon;

	public Transform distanceTarget;

	public float startDistance = 10f;

	private float scale;

	public RectTransform Icon;

	private void OnEnable()
	{
		StartCoroutine("UpdateUISmooth");
	}

	private IEnumerator UpdateUISmooth()
	{
		float interval = 0.025f;
		while (true)
		{
			float distance = Vector3.Distance(base.transform.position, distanceTarget.position);
			if (distance > startDistance)
			{
				distanceIcon.gameObject.SetActive(true);
				float num = Mathf.Clamp(distance / 8f, 1f, 8f);
				Icon.localScale = new Vector3(num, num, num);
				Icon.transform.LookAt(distanceTarget, Vector3.up);
			}
			else
			{
				distanceIcon.gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(interval);
		}
	}
}
