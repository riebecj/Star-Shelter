using UnityEngine;

public class AutoRotate : MonoBehaviour
{
	public float speed = 50f;

	private void Update()
	{
		base.transform.Rotate(Vector3.up, Time.deltaTime * speed, Space.World);
	}
}
