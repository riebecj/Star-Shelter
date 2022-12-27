using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : MonoBehaviour
{
	public int Damage;

	public int size;

	internal bool active = true;

	internal bool worldComet;

	private Rigidbody rigidbody;

	internal static List<GameObject> unActiveComets = new List<GameObject>();

	public GameObject brokenComet;

	private void Start()
	{
		if (GameManager.instance.debugMode && GetComponent<Rigidbody>().velocity == Vector3.zero)
		{
			GetComponent<Rigidbody>().velocity = base.transform.forward * 5f;
		}
		rigidbody = GetComponent<Rigidbody>();
		float num = 25f;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!active)
		{
			return;
		}
		if ((bool)other.collider.GetComponentInParent<Recipe>() || (bool)other.collider.GetComponentInParent<Room>())
		{
			if (!other.collider.GetComponentInParent<HoloShield>() || !other.collider.GetComponentInParent<HoloShield>().active)
			{
				other.collider.SendMessageUpwards("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);
				other.collider.GetComponentInParent<Room>().OnCometImpact(base.gameObject);
			}
			else if ((bool)other.collider.GetComponentInParent<HoloShield>())
			{
				other.collider.GetComponentInParent<HoloShield>().OnTakeDamage(Damage);
				Debug.Log("Kewl");
			}
		}
		else if (other.transform.root.tag == "Player")
		{
			SuitManager.instance.OnTakeDamage(Damage * 40, 3);
		}
		else if (!worldComet)
		{
			SuitManager.instance.AvertedCometPromt();
		}
		active = false;
		if (CometManager.instance.activeComets.Contains(base.transform))
		{
			CometManager.instance.activeComets.Remove(base.transform);
		}
		GetComponent<Collider>().isTrigger = true;
		GetComponent<MeshRenderer>().enabled = false;
		IEnumerator enumerator = base.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.gameObject.SetActive(false);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		UnityEngine.Object.Instantiate(brokenComet, base.transform.position, base.transform.rotation);
		ClearFromMap();
		Invoke("Deactivate", 1f);
	}

	public void OnGetShot()
	{
		if ((bool)GetComponentInChildren<CometUI>() && GetComponentInChildren<CometUI>().gameObject.activeSelf)
		{
			SuitManager.instance.AvertedCometPromt();
		}
		active = false;
		if (CometManager.instance.activeComets.Contains(base.transform))
		{
			CometManager.instance.activeComets.Remove(base.transform);
		}
		UnityEngine.Object.Instantiate(brokenComet, base.transform.position, base.transform.rotation);
		base.gameObject.SetActive(false);
	}

	private void Split()
	{
		GameObject original = CometManager.instance.Comet_S1;
		if (size == 3)
		{
			original = CometManager.instance.Comet_S2;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(original, base.transform.position + base.transform.right * 0.25f, base.transform.rotation);
		GameObject gameObject2 = UnityEngine.Object.Instantiate(original, base.transform.position - base.transform.right * 0.25f, base.transform.rotation);
		float num = UnityEngine.Random.Range(0, 1);
		gameObject.GetComponent<Rigidbody>().velocity = rigidbody.velocity;
		gameObject.GetComponent<Rigidbody>().AddRelativeForce(UnityEngine.Random.onUnitSphere * 800f);
		num = UnityEngine.Random.Range(0, 1);
		gameObject2.GetComponent<Rigidbody>().velocity = rigidbody.velocity;
		gameObject2.GetComponent<Rigidbody>().AddRelativeForce(UnityEngine.Random.onUnitSphere * 800f);
	}

	private void Deactivate()
	{
		if (CometManager.instance.activeComets.Contains(base.transform))
		{
			CometManager.instance.activeComets.Remove(base.transform);
		}
		unActiveComets.Add(base.gameObject);
	}

	private void OnEnable()
	{
		Invoke("AddToMap", 0.1f);
	}

	private void AddToMap()
	{
		for (int i = 0; i < HoloMap.holoMaps.Count; i++)
		{
			HoloMap.holoMaps[i].AddComet(base.transform, size);
		}
		SuitManager.instance.CometPromt(size);
	}

	private void OnDisable()
	{
		ClearFromMap();
		if (CometManager.instance.activeComets.Contains(base.transform))
		{
			CometManager.instance.activeComets.Remove(base.transform);
		}
	}

	public void ClearFromMap()
	{
		for (int i = 0; i < HoloMap.holoMaps.Count; i++)
		{
			if (HoloMap.holoMaps[i].cometTransforms.Contains(base.transform))
			{
				HoloMap.holoMaps[i].RemoveComet(base.transform);
			}
		}
	}
}
