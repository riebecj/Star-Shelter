using UnityEngine;

[ExecuteInEditMode]
public class ValveOverrideLightmap : MonoBehaviour
{
	public Color m_colorTint = Color.white;

	[Range(0f, 16f)]
	public float m_brightness = 1f;

	private void Start()
	{
		UpdateConstants();
	}

	private void UpdateConstants()
	{
		Shader.SetGlobalColor("g_vOverrideLightmapScale", m_brightness * m_colorTint.linear);
	}
}
