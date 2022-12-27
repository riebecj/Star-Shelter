using PreviewLabs;
using UnityEngine;

public class StartProp : MonoBehaviour
{
	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("StartProp" + base.transform.position))
		{
			if ((bool)GetComponent<Gate>())
			{
				GetComponent<Gate>().OnRemove();
			}
			base.gameObject.SetActive(false);
		}
	}

	public void OnSalvage()
	{
		PreviewLabs.PlayerPrefs.SetBool("StartProp" + base.transform.position, true);
	}

	private void Cleanup()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.5f);
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i].GetComponentInParent<CraftStation>() && array[i].GetComponentInParent<CraftStation>().transform != base.transform)
			{
				GetComponentInParent<Room>().props.Remove(array[i].GetComponentInParent<CraftStation>().gameObject);
				Object.Destroy(array[i].GetComponentInParent<CraftStation>().gameObject);
			}
		}
	}
}
