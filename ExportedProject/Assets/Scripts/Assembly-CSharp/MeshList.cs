using UnityEngine;

public class MeshList : MonoBehaviour
{
	public Mesh[] meshes;

	public static MeshList instance;

	private void Awake()
	{
		instance = this;
	}
}
