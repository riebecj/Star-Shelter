using UnityEngine;

public class Wreckage : MonoBehaviour
{
	public float speed = 1f;

	internal Vector3 Direction;

	internal Vector3 previousPosition;

	internal Vector3 velocity;

	private Rigidbody rigidbody;

	public Transform rotatedObject;

	private void OnEnable()
	{
		if (GameManager.instance.loading)
		{
			Invoke("AddToMap", 1f);
		}
		else
		{
			AddToMap();
		}
	}

	private void AddToMap()
	{
		for (int i = 0; i < HoloMap.holoMaps.Count; i++)
		{
			HoloMap.holoMaps[i].AddWreck(base.transform);
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < HoloMap.holoMaps.Count; i++)
		{
			if (HoloMap.holoMaps[i].debrisTransforms.Contains(base.transform))
			{
				HoloMap.holoMaps[i].RemoveWreck(base.transform);
			}
		}
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			if (Vector3.Distance(base.transform.position, componentsInChildren[j].position) > 20f && (bool)componentsInChildren[j].GetComponentInChildren<Collider>())
			{
				WreckManager.instance.LooseDebris.Add(componentsInChildren[j].gameObject);
				componentsInChildren[j].SetParent(null);
			}
		}
	}

	private void Start()
	{
		if ((bool)rigidbody)
		{
			rigidbody = GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.drag = 0f;
			rigidbody.AddForce(Direction.normalized * speed, ForceMode.VelocityChange);
		}
		if (rotatedObject != null)
		{
			Quaternion rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
			rotatedObject.rotation = rotation;
		}
	}
}
