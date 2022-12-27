using UnityEngine;

public class Rotate : MonoBehaviour
{
	[Tooltip("rotation to apply")]
	public Vector3 rotation = new Vector3(0f, 0f, 0.02f);

	public bool RandomizeRotationRange;

	private void OnEnable()
	{
		if (RandomizeRotationRange)
		{
			Vector3 vector = new Vector3(RandomRamngeBinamial(rotation.x), RandomRamngeBinamial(rotation.y), RandomRamngeBinamial(rotation.z));
			rotation = vector;
		}
	}

	public float RandomRamngeBinamial(float _range)
	{
		float num = RandomBinamial() * _range;
		return Random.Range(0f - num, num);
	}

	public float RandomBinamial()
	{
		return Random.Range(-1f, 1f) - (float)Random.Range(0, 1);
	}

	private void Update()
	{
		base.gameObject.transform.Rotate(rotation);
	}
}
