using UnityEngine;

public class CopyTransformv2 : MonoBehaviour
{
	public Transform from;

	private Transform to;

	public Vector3 offset = Vector3.zero;

	private Vector3 Pos;

	private void Start()
	{
		if (from == null)
		{
			from = GameObject.FindGameObjectWithTag("Player").transform;
		}
		to = base.transform;
	}

	private void LateUpdate()
	{
		if (from == null)
		{
			from = GameObject.FindGameObjectWithTag("Player").transform;
		}
		if (!(from == null))
		{
			Pos = from.position + offset;
			if (Pos != to.position)
			{
				to.position = Pos;
			}
		}
	}
}
