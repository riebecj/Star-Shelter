using System.Collections;
using UnityEngine;

public class LeakParticle : MonoBehaviour
{
	internal Room room;

	private void OnEnable()
	{
		Invoke("StartChecking", 1f);
	}

	private void StartChecking()
	{
		if (base.isActiveAndEnabled)
		{
			StartCoroutine("CheckForOxygen");
		}
	}

	private IEnumerator CheckForOxygen()
	{
		while (true)
		{
			if (room.Oxygen < 2f)
			{
				base.gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(2f);
		}
	}
}
