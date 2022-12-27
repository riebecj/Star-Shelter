using System;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
	public enColorchannels colorChannel;

	public enWaveFunctions waveFunction;

	public float offset;

	public float amplitude = 1f;

	public float phase;

	public float frequency = 0.5f;

	public bool affectsIntensity = true;

	private Color originalColor;

	private float originalIntensity;

	private void Start()
	{
		originalColor = GetComponent<Light>().color;
		originalIntensity = GetComponent<Light>().intensity;
	}

	private void Update()
	{
		Light component = GetComponent<Light>();
		if (affectsIntensity)
		{
			component.intensity = originalIntensity * EvalWave();
		}
		Color color = originalColor;
		Color color2 = GetComponent<Light>().color;
		if (colorChannel == enColorchannels.all)
		{
			component.color = originalColor * EvalWave();
		}
		else if (colorChannel == enColorchannels.red)
		{
			component.color = new Color(color.r * EvalWave(), color2.g, color2.b, color2.a);
		}
		else if (colorChannel == enColorchannels.green)
		{
			component.color = new Color(color2.r, color.g * EvalWave(), color2.b, color2.a);
		}
		else
		{
			component.color = new Color(color2.r, color2.g, color.b * EvalWave(), color2.a);
		}
	}

	private float EvalWave()
	{
		float num = (Time.time + phase) * frequency;
		num -= Mathf.Floor(num);
		float num2 = ((waveFunction == enWaveFunctions.sinus) ? Mathf.Sin(num * 2f * (float)Math.PI) : ((waveFunction == enWaveFunctions.triangle) ? ((!(num < 0.5f)) ? (-4f * num + 3f) : (4f * num - 1f)) : ((waveFunction == enWaveFunctions.square) ? ((!(num < 0.5f)) ? (-1f) : 1f) : ((waveFunction == enWaveFunctions.sawtooth) ? num : ((waveFunction == enWaveFunctions.inverted_saw) ? (1f - num) : ((waveFunction != enWaveFunctions.noise) ? 1f : (1f - UnityEngine.Random.value * 2f)))))));
		return num2 * amplitude + offset;
	}
}
