using UnityEngine;

public class CenterBody : MonoBehaviour
{
	public Transform PlayerStartPos;

	public Transform Head;

	public Transform CamRig;

	private void Start()
	{
		CamRig.rotation = Quaternion.Inverse(Quaternion.Euler(0f, Head.rotation.eulerAngles.y, 0f));
		Vector3 vector = Head.position - CamRig.position;
		CamRig.transform.position = PlayerStartPos.position - vector;
	}
}
