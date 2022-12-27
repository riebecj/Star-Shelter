using PreviewLabs;
using UnityEngine;

public class MagPush : MonoBehaviour
{
	public Mesh brokenMesh;

	public int health = 3;

	internal bool broken;

	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool(string.Concat(base.name, base.transform.position, "Broken")))
		{
			OnBreak();
		}
	}

	public void OnTakeDamage(int value)
	{
		TakeDamage(value);
	}

	public void TakeDamage(int value)
	{
		if (value <= health)
		{
			health -= value;
		}
		else
		{
			health = 0;
		}
		if (health == 0 && !broken)
		{
			OnBreak();
		}
	}

	private void OnBreak()
	{
		GetComponent<MeshFilter>().mesh = brokenMesh;
		broken = true;
		if (!GameManager.instance.loading)
		{
			Object.Instantiate(GameManager.instance.breakPraticle, base.transform.position, base.transform.rotation);
			GameAudioManager.instance.AddToSuitQueue(BaseManager.instance.magPushBrokenWarning);
			PreviewLabs.PlayerPrefs.SetBool(string.Concat(base.name, base.transform.position, "Broken"), true);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!broken && (bool)other.GetComponent<Wreckage>())
		{
			other.GetComponent<Rigidbody>().velocity = base.transform.forward.normalized * 0.5f;
		}
	}

	public void OnSalvage()
	{
		OnRemove();
	}

	public void OnRemove()
	{
		PreviewLabs.PlayerPrefs.SetBool(string.Concat(base.name, base.transform.position, "Broken"), false);
	}
}
