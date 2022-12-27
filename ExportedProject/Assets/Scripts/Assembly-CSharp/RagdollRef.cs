using UnityEngine;

public class RagdollRef : MonoBehaviour
{
	public static RagdollRef instance;

	public Transform FootTargetR;

	public Transform FootTargetL;

	public Transform PelvisTarget;

	private void Awake()
	{
		instance = this;
	}
}
