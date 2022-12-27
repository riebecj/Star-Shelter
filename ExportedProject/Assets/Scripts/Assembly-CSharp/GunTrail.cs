using System.Collections;
using UnityEngine;

public class GunTrail : MonoBehaviour
{
	internal float speed = 100f;

	public float destructTime = 0.5f;

	private void Start()
	{
		StartCoroutine("Move");
		Invoke("OnDestruct", destructTime);
	}

	public IEnumerator Move()
	{
		while (true)
		{
			base.transform.Translate(Vector3.forward * Time.deltaTime * speed);
			yield return new WaitForSeconds(0.01f);
		}
	}

	private void OnDestruct()
	{
		base.gameObject.SetActive(false);
	}
}
