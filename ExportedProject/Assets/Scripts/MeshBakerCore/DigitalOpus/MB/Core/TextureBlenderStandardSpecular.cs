using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderStandardSpecular : TextureBlender
	{
		private enum Prop
		{
			doColor = 0,
			doSpecular = 1,
			doEmission = 2,
			doNone = 3
		}

		private Color m_tintColor;

		private Color m_emission;

		private Prop propertyToDo = Prop.doNone;

		private Color m_defaultColor = Color.white;

		private Color m_defaultSpecular = Color.black;

		private float m_defaultGlossiness = 0.5f;

		private Color m_defaultEmission = Color.black;

		public bool DoesShaderNameMatch(string shaderName)
		{
			return shaderName.Equals("Standard (Specular setup)");
		}

		public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
		{
			if (shaderTexturePropertyName.Equals("_MainTex"))
			{
				propertyToDo = Prop.doColor;
				if (sourceMat.HasProperty(shaderTexturePropertyName))
				{
					m_tintColor = sourceMat.GetColor("_Color");
				}
				else
				{
					m_tintColor = m_defaultColor;
				}
			}
			else if (shaderTexturePropertyName.Equals("_MetallicGlossMap"))
			{
				propertyToDo = Prop.doSpecular;
			}
			else if (shaderTexturePropertyName.Equals("_EmissionMap"))
			{
				propertyToDo = Prop.doEmission;
				if (sourceMat.HasProperty(shaderTexturePropertyName))
				{
					m_emission = sourceMat.GetColor("_EmissionColor");
				}
				else
				{
					m_emission = m_defaultEmission;
				}
			}
			else
			{
				propertyToDo = Prop.doNone;
			}
		}

		public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
		{
			if (propertyToDo == Prop.doColor)
			{
				return new Color(pixelColor.r * m_tintColor.r, pixelColor.g * m_tintColor.g, pixelColor.b * m_tintColor.b, pixelColor.a * m_tintColor.a);
			}
			if (propertyToDo == Prop.doSpecular)
			{
				return pixelColor;
			}
			if (propertyToDo == Prop.doEmission)
			{
				return new Color(pixelColor.r * m_emission.r, pixelColor.g * m_emission.g, pixelColor.b * m_emission.b, pixelColor.a * m_emission.a);
			}
			return pixelColor;
		}

		public bool NonTexturePropertiesAreEqual(Material a, Material b)
		{
			if (!TextureBlenderFallback._compareColor(a, b, m_defaultColor, "_Color"))
			{
				return false;
			}
			if (!TextureBlenderFallback._compareColor(a, b, m_defaultSpecular, "_SpecColor"))
			{
				return false;
			}
			if (!TextureBlenderFallback._compareFloat(a, b, m_defaultGlossiness, "_Glossiness"))
			{
				return false;
			}
			if (!TextureBlenderFallback._compareColor(a, b, m_defaultEmission, "_EmissionColor"))
			{
				return false;
			}
			return true;
		}

		public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
		{
			resultMaterial.SetColor("_Color", m_defaultColor);
			resultMaterial.SetColor("_SpecColor", m_defaultSpecular);
			resultMaterial.SetFloat("_Glossiness", m_defaultGlossiness);
			if (resultMaterial.GetTexture("_EmissionMap") == null)
			{
				resultMaterial.SetColor("_EmissionColor", Color.black);
			}
			else
			{
				resultMaterial.SetColor("_EmissionColor", Color.white);
			}
		}

		public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texPropertyName)
		{
			if (texPropertyName.name.Equals("_BumpMap"))
			{
				return new Color(0.5f, 0.5f, 1f);
			}
			if (texPropertyName.name.Equals("_MainTex"))
			{
				if (mat != null && mat.HasProperty("_Color"))
				{
					try
					{
						return mat.GetColor("_Color");
					}
					catch (Exception)
					{
					}
				}
			}
			else if (texPropertyName.name.Equals("_SpecGlossMap"))
			{
				bool flag = false;
				if (mat != null && mat.HasProperty("_SpecColor"))
				{
					try
					{
						Color color = mat.GetColor("_SpecColor");
						if (mat.HasProperty("_Glossiness"))
						{
							try
							{
								flag = true;
								color.a = mat.GetFloat("_Glossiness");
							}
							catch (Exception)
							{
							}
						}
						Debug.LogWarning(color);
						return color;
					}
					catch (Exception)
					{
					}
				}
				if (!flag)
				{
					return m_defaultSpecular;
				}
			}
			else
			{
				if (texPropertyName.name.Equals("_ParallaxMap"))
				{
					return new Color(0f, 0f, 0f, 0f);
				}
				if (texPropertyName.name.Equals("_OcclusionMap"))
				{
					return new Color(1f, 1f, 1f, 1f);
				}
				if (texPropertyName.name.Equals("_EmissionMap"))
				{
					if (mat != null)
					{
						if (!mat.HasProperty("_EmissionColor"))
						{
							return Color.black;
						}
						try
						{
							return mat.GetColor("_EmissionColor");
						}
						catch (Exception)
						{
						}
					}
				}
				else if (texPropertyName.name.Equals("_DetailMask"))
				{
					return new Color(0f, 0f, 0f, 0f);
				}
			}
			return new Color(1f, 1f, 1f, 0f);
		}
	}
}
