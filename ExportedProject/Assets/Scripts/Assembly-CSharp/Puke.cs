using System.Collections;
using UnityEngine;

public class Puke : MonoBehaviour
{
	internal float scale;

	internal Vector3 direction;

	private float speed = 0.25f;

	internal ParticleSystem particleSystem;

	internal ParticleSystem.Particle[] particles;

	internal bool outOfMask;

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
		StartCoroutine("FadeIn");
	}

	private IEnumerator FadeIn()
	{
		float refreshRate = 0.03f;
		while (scale < 0.1f)
		{
			scale += refreshRate * 0.5f;
			base.transform.localScale = Vector3.one * scale;
			yield return new WaitForSeconds(refreshRate);
		}
	}

	private IEnumerator MoveAway()
	{
		float refreshRate = 0.02f;
		while (true)
		{
			base.transform.position += direction * Time.deltaTime * speed;
			yield return new WaitForSeconds(refreshRate);
		}
	}

	private IEnumerator FadeOut()
	{
		float refreshRate = 0.02f;
		particles = new ParticleSystem.Particle[particleSystem.particleCount];
		particleSystem.GetParticles(particles);
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].velocity = Random.onUnitSphere * 0.8f;
		}
		particleSystem.SetParticles(particles, particleSystem.particleCount);
		while (scale > 0f)
		{
			scale -= refreshRate * 0.003f;
			yield return new WaitForSeconds(refreshRate);
		}
		base.gameObject.SetActive(false);
	}

	public void OnOpenMask()
	{
		direction = GameManager.instance.Head.forward.normalized;
		base.transform.SetParent(null);
		StartCoroutine("MoveAway");
		StartCoroutine("FadeOut");
		outOfMask = true;
	}

	public IEnumerator MoveFromMouth()
	{
		while (base.transform.parent != null)
		{
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, 0f, 0.08f), 0.15f);
			yield return new WaitForSeconds(0.02f);
		}
	}
}
