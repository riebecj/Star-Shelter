using UnityEngine;

public class InitDemo_1 : MonoBehaviour
{
	public GameObject[] Prefabs;

	public int PrefabNum;

	public float PosY;

	private void Awake()
	{
		GenerateLevel();
	}

	private void GenerateLevel()
	{
		for (int i = 0; i < PrefabNum; i++)
		{
			GameObject gameObject = Object.Instantiate(position: new Vector3(Random.Range(70f, 930f), PosY, Random.Range(70f, 930f)), original: Prefabs[Random.Range(0, Prefabs.Length)], rotation: Quaternion.identity);
			gameObject.transform.Rotate(Vector3.up, Random.Range(0f, 360f));
		}
	}
}
