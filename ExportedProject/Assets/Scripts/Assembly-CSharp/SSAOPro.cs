using UnityEngine;

[ImageEffectAllowedInSceneView]
[HelpURL("http://www.thomashourdel.com/ssaopro/doc/")]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/SSAO Pro")]
[RequireComponent(typeof(Camera))]
public class SSAOPro : MonoBehaviour
{
	public enum BlurMode
	{
		None = 0,
		Gaussian = 1,
		HighQualityBilateral = 2
	}

	public enum SampleCount
	{
		VeryLow = 0,
		Low = 1,
		Medium = 2,
		High = 3,
		Ultra = 4
	}

	protected enum Pass
	{
		Clear = 0,
		GaussianBlur = 5,
		HighQualityBilateralBlur = 6,
		Composite = 7
	}

	public Texture2D NoiseTexture;

	public bool UseHighPrecisionDepthMap;

	public SampleCount Samples = SampleCount.Medium;

	[Range(1f, 4f)]
	public int Downsampling = 1;

	[Range(0.01f, 1.25f)]
	public float Radius = 0.12f;

	[Range(0f, 16f)]
	public float Intensity = 2.5f;

	[Range(0f, 10f)]
	public float Distance = 1f;

	[Range(0f, 1f)]
	public float Bias = 0.1f;

	[Range(0f, 1f)]
	public float LumContribution = 0.5f;

	[ColorUsage(false)]
	public Color OcclusionColor = Color.black;

	public float CutoffDistance = 150f;

	public float CutoffFalloff = 50f;

	public BlurMode Blur = BlurMode.HighQualityBilateral;

	public bool BlurDownsampling;

	[Range(1f, 4f)]
	public int BlurPasses = 1;

	[Range(1f, 20f)]
	public float BlurBilateralThreshold = 10f;

	public bool DebugAO;

	protected Shader m_ShaderSSAO;

	protected Material m_Material;

	protected Camera m_Camera;

	public Material Material
	{
		get
		{
			if (m_Material == null)
			{
				m_Material = new Material(ShaderSSAO)
				{
					hideFlags = HideFlags.HideAndDontSave
				};
			}
			return m_Material;
		}
	}

	public Shader ShaderSSAO
	{
		get
		{
			if (m_ShaderSSAO == null)
			{
				m_ShaderSSAO = Shader.Find("Hidden/SSAO Pro V2");
			}
			return m_ShaderSSAO;
		}
	}

	private void OnEnable()
	{
		m_Camera = GetComponent<Camera>();
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarning("Image Effects are not supported on this device.");
			base.enabled = false;
		}
		else if (ShaderSSAO == null)
		{
			Debug.LogWarning("Missing shader (SSAO).");
			base.enabled = false;
		}
		else if (!ShaderSSAO.isSupported)
		{
			Debug.LogWarning("Unsupported shader (SSAO).");
			base.enabled = false;
		}
		else if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarning("Depth textures aren't supported on this device.");
			base.enabled = false;
		}
	}

	private void OnPreRender()
	{
		m_Camera.depthTextureMode |= DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
	}

	private void OnDisable()
	{
		if (m_Material != null)
		{
			Object.DestroyImmediate(m_Material);
		}
		m_Material = null;
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (ShaderSSAO == null || Mathf.Approximately(Intensity, 0f))
		{
			Graphics.Blit(source, destination);
			return;
		}
		Material.shaderKeywords = null;
		switch (Samples)
		{
		case SampleCount.Low:
			Material.EnableKeyword("SAMPLES_LOW");
			break;
		case SampleCount.Medium:
			Material.EnableKeyword("SAMPLES_MEDIUM");
			break;
		case SampleCount.High:
			Material.EnableKeyword("SAMPLES_HIGH");
			break;
		case SampleCount.Ultra:
			Material.EnableKeyword("SAMPLES_ULTRA");
			break;
		}
		int num = 0;
		if (NoiseTexture != null)
		{
			num = 1;
		}
		if (!Mathf.Approximately(LumContribution, 0f))
		{
			num += 2;
		}
		num++;
		Material.SetMatrix("_InverseViewProject", (m_Camera.projectionMatrix * m_Camera.worldToCameraMatrix).inverse);
		Material.SetMatrix("_CameraModelView", m_Camera.cameraToWorldMatrix);
		Material.SetTexture("_NoiseTex", NoiseTexture);
		Material.SetVector("_Params1", new Vector4((!(NoiseTexture == null)) ? ((float)NoiseTexture.width) : 0f, Radius, Intensity, Distance));
		Material.SetVector("_Params2", new Vector4(Bias, LumContribution, CutoffDistance, CutoffFalloff));
		Material.SetColor("_OcclusionColor", OcclusionColor);
		if (Blur == BlurMode.None)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width / Downsampling, source.height / Downsampling, 0, RenderTextureFormat.ARGB32);
			Graphics.Blit(temporary, temporary, Material, 0);
			if (DebugAO)
			{
				Graphics.Blit(source, temporary, Material, num);
				Graphics.Blit(temporary, destination);
				RenderTexture.ReleaseTemporary(temporary);
			}
			else
			{
				Graphics.Blit(source, temporary, Material, num);
				Material.SetTexture("_SSAOTex", temporary);
				Graphics.Blit(source, destination, Material, 7);
				RenderTexture.ReleaseTemporary(temporary);
			}
			return;
		}
		Pass pass = ((Blur != BlurMode.HighQualityBilateral) ? Pass.GaussianBlur : Pass.HighQualityBilateralBlur);
		int num2 = ((!BlurDownsampling) ? 1 : Downsampling);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / num2, source.height / num2, 0, RenderTextureFormat.ARGB32);
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / Downsampling, source.height / Downsampling, 0, RenderTextureFormat.ARGB32);
		Graphics.Blit(temporary2, temporary2, Material, 0);
		Graphics.Blit(source, temporary2, Material, num);
		Material.SetFloat("_BilateralThreshold", BlurBilateralThreshold * 5f);
		for (int i = 0; i < BlurPasses; i++)
		{
			Material.SetVector("_Direction", new Vector2(1f / (float)source.width, 0f));
			Graphics.Blit(temporary2, temporary3, Material, (int)pass);
			temporary2.DiscardContents();
			Material.SetVector("_Direction", new Vector2(0f, 1f / (float)source.height));
			Graphics.Blit(temporary3, temporary2, Material, (int)pass);
			temporary3.DiscardContents();
		}
		if (!DebugAO)
		{
			Material.SetTexture("_SSAOTex", temporary2);
			Graphics.Blit(source, destination, Material, 7);
		}
		else
		{
			Graphics.Blit(temporary2, destination);
		}
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
	}
}
