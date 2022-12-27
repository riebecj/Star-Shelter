using UnityEngine;

public class CursorManager : MonoBehaviour
{
	public bool ShowCursor = true;

	private void Start()
	{
		UpdateCursor();
	}

	private void Update()
	{
		if (Cursor.visible != ShowCursor)
		{
			UpdateCursor();
		}
	}

	private void UpdateCursor()
	{
		Cursor.visible = ShowCursor;
		Cursor.lockState = ((!ShowCursor) ? CursorLockMode.Locked : CursorLockMode.None);
	}
}
