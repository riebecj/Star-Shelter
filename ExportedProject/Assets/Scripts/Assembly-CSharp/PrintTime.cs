using UnityEngine;

public class PrintTime : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		MonoBehaviour.print(Time.time);
	}
}
