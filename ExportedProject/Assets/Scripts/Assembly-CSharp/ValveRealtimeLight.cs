using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class ValveRealtimeLight : MonoBehaviour
{
	[NonSerialized]
	[HideInInspector]
	public static List<ValveRealtimeLight> s_allLights = new List<ValveRealtimeLight>();

	[NonSerialized]
	[HideInInspector]
	public Light m_cachedLight;

	[NonSerialized]
	[HideInInspector]
	public Matrix4x4[] m_shadowTransform = new Matrix4x4[6]
	{
		Matrix4x4.identity,
		Matrix4x4.identity,
		Matrix4x4.identity,
		Matrix4x4.identity,
		Matrix4x4.identity,
		Matrix4x4.identity
	};

	[NonSerialized]
	[HideInInspector]
	public Matrix4x4[] m_lightCookieTransform = new Matrix4x4[6]
	{
		Matrix4x4.identity,
		Matrix4x4.identity,
		Matrix4x4.identity,
		Matrix4x4.identity,
		Matrix4x4.identity,
		Matrix4x4.identity
	};

	[NonSerialized]
	[HideInInspector]
	public int[] m_shadowX = new int[6];

	[NonSerialized]
	[HideInInspector]
	public int[] m_shadowY = new int[6];

	[NonSerialized]
	[HideInInspector]
	public bool m_bRenderShadowsThisFrame;

	[NonSerialized]
	[HideInInspector]
	public bool m_bInCameraFrustum;

	[Range(0f, 100f)]
	public float m_innerSpotPercent = 50f;

	[Range(128f, 8192f)]
	public int m_shadowResolution = 1024;

	public float m_shadowNearClipPlane = 1f;

	public LayerMask m_shadowCastLayerMask = -1;

	public float m_directionalLightShadowRadius = 100f;

	public float m_directionalLightShadowRange = 100f;

	public bool m_useOcclusionCullingForShadows = true;

	private void OnValidate()
	{
		if (m_shadowResolution % 128 != 0)
		{
			m_shadowResolution -= m_shadowResolution % 128;
		}
		if (m_shadowNearClipPlane < 0.01f)
		{
			m_shadowNearClipPlane = 0.01f;
		}
	}

	private void OnEnable()
	{
		if (!s_allLights.Contains(this))
		{
			s_allLights.Add(this);
			m_cachedLight = GetComponent<Light>();
		}
	}

	private void OnDisable()
	{
		s_allLights.Remove(this);
	}

	public bool IsEnabled()
	{
		Light cachedLight = m_cachedLight;
		if (!cachedLight.enabled || !cachedLight.isActiveAndEnabled)
		{
			return false;
		}
		if (cachedLight.intensity <= 0f)
		{
			return false;
		}
		if (cachedLight.range <= 0f)
		{
			return false;
		}
		if (cachedLight.color.linear.r <= 0f && cachedLight.color.linear.g <= 0f && cachedLight.color.linear.b <= 0f)
		{
			return false;
		}
		if (cachedLight.isBaked)
		{
		}
		if (!m_bInCameraFrustum)
		{
			return false;
		}
		return true;
	}

	public bool CastsShadows()
	{
		Light cachedLight = m_cachedLight;
		if ((cachedLight.type == LightType.Spot || cachedLight.type == LightType.Point || cachedLight.type == LightType.Directional) && cachedLight.shadows != 0)
		{
			return true;
		}
		return false;
	}
}
