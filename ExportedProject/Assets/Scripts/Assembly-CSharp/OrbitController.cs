using System.Collections;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public bool update;

	public float orbitSpeed = 3f;

	public Transform Base;

	private Transform player;

	public static OrbitController instance;

	public Transform[] orbits;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		player = GameManager.instance.CamRig;
		StartCoroutine("Rotate");
	}

	private IEnumerator Rotate()
	{
		while (true)
		{
			orbits[0].RotateAround(base.transform.position, Vector3.up, (0f - orbitSpeed) * Time.deltaTime);
			orbits[1].RotateAround(base.transform.position, Vector3.up, orbitSpeed * Time.deltaTime);
			orbits[2].RotateAround(base.transform.position, Vector3.up, (0f - orbitSpeed) * Time.deltaTime);
			yield return new WaitForSeconds(0.03f);
		}
	}

	public void AssignOrbit(Transform child)
	{
		child.SetParent(orbits[Random.Range(0, 3)]);
	}

	private IEnumerator CheckClosestStation()
	{
		float waitTime = 0.01f;
		while (update)
		{
			if (Vector3.Distance(player.position, Base.position) < 15f)
			{
			}
			yield return new WaitForSeconds(waitTime);
		}
	}
}
