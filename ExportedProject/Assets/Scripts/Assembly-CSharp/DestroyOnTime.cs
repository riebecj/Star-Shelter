using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
	public float time = 0.25f;

	private void Start()
	{
		if ((bool)GetComponent<AudioSource>())
		{
			GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
		}
		Invoke("Destruct", time);
	}

	private void Destruct()
	{
		Object.Destroy(base.gameObject);
	}
}
