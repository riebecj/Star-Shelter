using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
	internal Material skyBox;

	public Color[] colors;

	public bool randomizeAtStart;

	private void Start()
	{
		skyBox = RenderSettings.skybox;
		skyBox.SetColor("_Tint", colors[1]);
		if (randomizeAtStart)
		{
			RandomizeColor();
		}
	}

	public void RandomizeColor()
	{
		skyBox.SetColor("_Tint", colors[Random.Range(0, colors.Length)]);
	}
}
