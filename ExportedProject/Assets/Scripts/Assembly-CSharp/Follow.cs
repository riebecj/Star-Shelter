using UnityEngine;

public class Follow : MonoBehaviour
{
	public Transform target;

	private void FixedUpdate()
	{
		base.transform.position = target.position;
	}
}
