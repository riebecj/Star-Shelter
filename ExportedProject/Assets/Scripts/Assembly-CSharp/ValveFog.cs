using UnityEngine;

[ExecuteInEditMode]
public class ValveFog : MonoBehaviour
{
	[Header("Gradient Fog")]
	public Gradient gradient = new Gradient();

	public float startDistance;

	public float endDistance = 100f;

	public int textureWidth = 32;

	[Header("Height Fog")]
	public Color heightFogColor = Color.grey;

	public float heightFogThickness = 1.15f;

	public float heightFogFalloff = 0.1f;

	public float heightFogBaseHeight = -40f;

	private Texture2D gradientFogTexture;

	private void Start()
	{
		UpdateConstants();
	}

	private void OnEnable()
	{
		Shader.EnableKeyword("D_VALVE_FOG");
	}

	private void OnDisable()
	{
		Shader.DisableKeyword("D_VALVE_FOG");
	}

	private void UpdateConstants()
	{
		if (gradientFogTexture == null)
		{
			GenerateTexture();
		}
		float x = 1f / (endDistance - startDistance);
		float y = (0f - startDistance) / (endDistance - startDistance);
		Shader.SetGlobalVector("gradientFogScaleAdd", new Vector4(x, y, 0f, 0f));
		Shader.SetGlobalColor("gradientFogLimitColor", gradient.Evaluate(1f).linear);
		Shader.SetGlobalVector("heightFogParams", new Vector4(heightFogThickness, heightFogFalloff, heightFogBaseHeight, 0f));
		Shader.SetGlobalColor("heightFogColor", heightFogColor.linear);
	}

	public void GenerateTexture()
	{
		gradientFogTexture = new Texture2D(textureWidth, 1, TextureFormat.ARGB32, false);
		gradientFogTexture.wrapMode = TextureWrapMode.Clamp;
		float num = 1f / (float)(textureWidth - 1);
		float num2 = 0f;
		for (int i = 0; i < textureWidth; i++)
		{
			gradientFogTexture.SetPixel(i, 0, gradient.Evaluate(num2));
			num2 += num;
		}
		gradientFogTexture.Apply();
		Shader.SetGlobalTexture("gradientFogTexture", gradientFogTexture);
	}
}
