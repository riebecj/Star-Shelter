using UnityEngine;

public class PlanetSkybox : MonoBehaviour
{
	public Material skyOne;

	public Material skyTwo;

	public GameObject rig;

	private void Start()
	{
		RenderSettings.skybox = skyOne;
	}

	private void Update()
	{
		if (!(rig != null))
		{
		}
	}
}
