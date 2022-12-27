using UnityEngine;

public class EnableOne : MonoBehaviour
{
	public GameObject[] _objects;

	private void Start()
	{
		Invoke("Enable", 0.1f);
	}

	private void Enable()
	{
		int num = Random.Range(0, _objects.Length);
		_objects[num].SetActive(true);
	}
}
