using System.Collections;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
	public float FadeInTime = 1f;

	private Color cachedColour;

	private MeshRenderer r;

	public bool test;

	private void Awake()
	{
		r = GetComponent<MeshRenderer>();
		cachedColour = r.material.color;
		StartFade();
	}

	private void StartFade()
	{
		Color color = cachedColour;
		color.a = 0f;
		r.material.color = color;
		StartCoroutine(FadeInLinear());
	}

	private IEnumerator FadeInLinear()
	{
		Color CurrentColour = cachedColour;
		CurrentColour.a = 0f;
		r.material.color = CurrentColour;
		float t = 0f;
		while (CurrentColour.a < cachedColour.a)
		{
			t += Time.fixedDeltaTime / FadeInTime;
			CurrentColour.a = Mathf.Lerp(0f, cachedColour.a, t);
			r.material.color = CurrentColour;
			yield return new WaitForFixedUpdate();
		}
	}

	private void Update()
	{
		if (test)
		{
			test = false;
			StartFade();
		}
	}
}
