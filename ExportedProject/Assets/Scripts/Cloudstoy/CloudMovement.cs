using UnityEngine;

public class CloudMovement : MonoBehaviour
{
	public Vector3 Velocity;

	private Transform myTransform;

	private void Start()
	{
		myTransform = base.transform;
	}

	private void Update()
	{
		if (!(Velocity == Vector3.zero))
		{
			myTransform.Translate(Velocity * Time.deltaTime, Space.World);
		}
	}
}
