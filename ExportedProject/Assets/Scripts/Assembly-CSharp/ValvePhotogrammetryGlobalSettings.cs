using UnityEngine;

[ExecuteInEditMode]
public class ValvePhotogrammetryGlobalSettings : MonoBehaviour
{
	public Color shadowColor;

	private int shadowColorID;

	private void Awake()
	{
		shadowColorID = Shader.PropertyToID("g_vPhotogrammetryShadowColor");
	}

	private void Update()
	{
		Shader.SetGlobalVector(shadowColorID, shadowColor.linear);
	}
}
