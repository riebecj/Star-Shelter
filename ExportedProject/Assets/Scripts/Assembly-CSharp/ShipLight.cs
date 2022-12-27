using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipLight : MonoBehaviour
{
	public GameObject light;

	internal Light _light;

	public static List<ShipLight> lightSources = new List<ShipLight>();

	public MeshRenderer renderer;

	public int materialIndex;

	public float occludeDistance = 100f;

	internal bool active;

	private void Start()
	{
		AssignGenerator();
	}

	private void AssignGenerator()
	{
		foreach (PowerGenerator powerGenerator in PowerGenerator.powerGenerators)
		{
			if (Vector3.Distance(base.transform.position, powerGenerator.transform.position) < (float)PowerGenerator.generatorDistance)
			{
				powerGenerator.togglePower = (PowerGenerator.TogglePower)Delegate.Combine(powerGenerator.togglePower, new PowerGenerator.TogglePower(TogglePower));
			}
		}
	}

	public void TogglePower(bool on)
	{
		if (on)
		{
			renderer.material.SetColor("_EmissionColor", Color.white * 2f);
			light.SetActive(true);
			active = true;
		}
		else
		{
			renderer.material.SetColor("_EmissionColor", Color.black);
			light.SetActive(false);
			active = false;
		}
	}

	private void OnEnable()
	{
		lightSources.Add(this);
		if (_light == null)
		{
			_light = light.GetComponent<Light>();
		}
	}

	private void OnDisable()
	{
		lightSources.Remove(this);
	}
}
