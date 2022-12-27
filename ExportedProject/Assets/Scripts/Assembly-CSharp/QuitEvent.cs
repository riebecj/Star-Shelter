using System.Collections;
using UnityEngine;

public class QuitEvent : MonoBehaviour
{
	public SkinnedMeshRenderer[] parts;

	public Material fadeMat;

	private float fadeTime = 6f;

	internal float startTime;

	private void Start()
	{
		startTime = fadeTime;
	}

	public void OnDissolve()
	{
		StartCoroutine("Dissolve");
	}

	private IEnumerator Dissolve()
	{
		float waitTime = 0.05f;
		for (int i = 0; i < parts.Length; i++)
		{
			parts[i].material = fadeMat;
		}
		while (fadeTime > 0.1f)
		{
			fadeTime -= waitTime;
			float value = 0.25f + (startTime - fadeTime) / startTime / 2f;
			for (int j = 0; j < parts.Length; j++)
			{
				parts[j].material.SetFloat("_Cutoff", Mathf.Clamp(value / (fadeTime * 4f) + 0.45f, 0.45f, 0.7f));
			}
			yield return new WaitForSeconds(waitTime);
		}
		if (Application.isEditor)
		{
			Debug.Break();
		}
		else
		{
			Application.Quit();
		}
	}

	private void Update()
	{
		if (!IsInvoking("Tick"))
		{
			Invoke("Tick", 1f);
			MonoBehaviour.print(Time.time);
		}
	}

	private void Tick()
	{
	}
}
