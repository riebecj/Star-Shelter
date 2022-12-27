using UnityEngine;

public class OutroManager : MonoBehaviour
{
	public static OutroManager instance;

	private void Awake()
	{
		instance = this;
	}
}
