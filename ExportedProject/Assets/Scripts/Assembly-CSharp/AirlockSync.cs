using UnityEngine;

public class AirlockSync : MonoBehaviour
{
	public Animator upperAirLock;

	public Animator lowerAirLock;

	public void CheckState()
	{
		if (upperAirLock.GetBool("Open"))
		{
			lowerAirLock.SetBool("Open", false);
		}
	}
}
