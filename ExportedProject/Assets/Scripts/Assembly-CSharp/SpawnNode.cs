using System.Collections.Generic;
using UnityEngine;

public class SpawnNode : MonoBehaviour
{
	public bool randomStartForce;

	public static List<Transform> spawnNodes = new List<Transform>();

	private void Start()
	{
		spawnNodes.Add(base.transform);
	}

	public void OnSpawn()
	{
		spawnNodes.Remove(base.transform);
	}
}
