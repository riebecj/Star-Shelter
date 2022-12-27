using System.Collections;
using UnityEngine;

public class WorldTarget : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine("UpdateScale");
	}

	private IEnumerator UpdateScale()
	{
		while (true)
		{
			base.transform.localScale = Vector3.one * Vector3.Distance(base.transform.position, GameManager.instance.Head.position) / 5f;
			yield return new WaitForSeconds(0.1f);
		}
	}
}
