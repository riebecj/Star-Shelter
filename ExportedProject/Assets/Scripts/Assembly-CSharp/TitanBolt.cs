using UnityEngine;

public class TitanBolt : MonoBehaviour
{
	public TitanAI titan;

	private void OnDisable()
	{
		if (titan.gameObject.activeSelf)
		{
			titan.OnReleaseBolt();
		}
	}
}
