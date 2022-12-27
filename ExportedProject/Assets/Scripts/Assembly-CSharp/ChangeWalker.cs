using UnityEngine;

public class ChangeWalker : MonoBehaviour
{
	[Tooltip("Drag here the standard walker (it can walk, run and jump)")]
	public Transform FPCStandard;

	[Tooltip("Drag here the fly walker (it just fly)")]
	public Transform FPCFly;

	[Header("Runtime variable watcher")]
	[SerializeField]
	private bool flyMode;

	private void Start()
	{
		flyMode = false;
		FPCStandard.position = FPCFly.position;
		if (!FPCFly.root.gameObject.activeSelf)
		{
			FPCFly.root.gameObject.SetActive(true);
		}
		UpdateWalkers();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Y))
		{
			flyMode = !flyMode;
			if (flyMode)
			{
				FPCFly.position = FPCStandard.position;
			}
			else
			{
				FPCStandard.position = FPCFly.position;
			}
			UpdateWalkers();
		}
	}

	private void UpdateWalkers()
	{
		FPCStandard.gameObject.SetActive(!flyMode);
		FPCFly.gameObject.SetActive(flyMode);
	}
}
