using UnityEngine;

public class DestructableDoor : MonoBehaviour
{
	public GameObject brokenObject;

	public void OnExplosion()
	{
		if ((bool)brokenObject)
		{
			brokenObject.SetActive(true);
			brokenObject.transform.SetParent(base.transform.parent);
		}
		base.gameObject.SetActive(false);
	}
}
