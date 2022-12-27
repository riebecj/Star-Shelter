using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
	public Transform target;

	private void LateUpdate()
	{
		Quaternion quaternion = Quaternion.LookRotation(target.position - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(quaternion, quaternion, Time.deltaTime * 5f);
	}
}
