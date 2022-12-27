using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightSource : MonoBehaviour
{
	public bool castsShadows;

	public static List<LightSource> lightSources = new List<LightSource>();

	internal Light light;

	public float occludeDistance = 25f;

	private void OnEnable()
	{
		light = GetComponent<Light>();
		lightSources.Add(this);
	}

	private void OnDisable()
	{
		lightSources.Remove(this);
	}
}
