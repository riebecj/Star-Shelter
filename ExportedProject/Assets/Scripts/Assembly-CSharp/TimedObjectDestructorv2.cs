using System.Collections;
using UnityEngine;

public class TimedObjectDestructorv2 : MonoBehaviour
{
	public float timeOut = 1f;

	public bool onlyDisable;

	public bool detachChildren;

	private void Awake()
	{
		StartCoroutine("_DestroyNow");
	}

	private IEnumerator _DestroyNow()
	{
		yield return new WaitForSeconds(timeOut);
		if (detachChildren)
		{
			base.transform.DetachChildren();
		}
		if (onlyDisable)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			Object.DestroyObject(base.gameObject);
		}
	}
}
