using UnityEngine;

public class Bob : MonoBehaviour
{
	private Vector3 MinPos;

	public Vector3 MaxPos;

	public float CycleTime = 2f;

	private Vector3 CurrentPos;

	private Vector3 StartPos;

	private float currentTime;

	private bool Up = true;

	private void OnEnable()
	{
		StartPos = Vector3.zero;
		MinPos = base.transform.localPosition;
		MaxPos = base.transform.localPosition + MaxPos;
	}

	private void LerpToPosition(Vector3 _pos)
	{
		if (StartPos == Vector3.zero)
		{
			StartPos = base.transform.localPosition;
		}
		CurrentPos = Vector3.Lerp(StartPos, _pos, currentTime / (CycleTime / 2f));
		base.transform.localPosition = CurrentPos;
	}

	private void Update()
	{
		CurrentPos = base.transform.localPosition;
		if (Up)
		{
			LerpToPosition(MaxPos);
		}
		else
		{
			LerpToPosition(MinPos);
		}
		currentTime += Time.deltaTime;
		if (currentTime >= CycleTime / 2f)
		{
			currentTime = 0f;
			Up = !Up;
			StartPos = Vector3.zero;
		}
	}
}
