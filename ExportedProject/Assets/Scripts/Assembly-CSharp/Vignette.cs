using System.Collections;
using UnityEngine;

public class Vignette : MonoBehaviour
{
	public Transform _vignette;

	private float value;

	public static Vignette instance;

	internal bool fading;

	private void Awake()
	{
		instance = this;
	}

	public void FadeIn()
	{
		if (!fading)
		{
			StopCoroutine("OnFadeOut");
			StartCoroutine("OnFadeIn");
		}
	}

	public void FadeOut()
	{
		StopCoroutine("OnFadeIn");
		StartCoroutine("OnFadeOut");
	}

	private IEnumerator OnFadeIn()
	{
		fading = true;
		value = 1.2f;
		while (value > 0.55f)
		{
			if (value > 0.55f)
			{
				value -= 0.05f;
			}
			_vignette.localScale = Vector3.one * value;
			yield return new WaitForSeconds(0.03f);
		}
		fading = false;
	}

	private IEnumerator OnFadeOut()
	{
		fading = true;
		value = 0.55f;
		while (value < 1.2f)
		{
			if (value < 1.2f)
			{
				value += 0.1f;
			}
			_vignette.localScale = Vector3.one * value;
			yield return new WaitForSeconds(0.03f);
		}
		fading = false;
	}
}
