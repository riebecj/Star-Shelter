using UnityEngine;
using VRTK;

public class HandBender : MonoBehaviour
{
	public Animator anim;

	public VRTK_ControllerEvents events;

	private float smooth;

	public bool left;

	private void Start()
	{
		events = GetComponentInChildren<VRTK_ControllerEvents>();
	}

	private void Update()
	{
		if (events.GetTriggerAxis() < 0.05f)
		{
			smooth = Mathf.MoveTowards(smooth, 0f, 5f * Time.deltaTime);
		}
		else
		{
			smooth = Mathf.MoveTowards(smooth, events.GetTriggerAxis(), 5f * Time.deltaTime);
		}
		if (left)
		{
			Debug.Log(events.GetTriggerAxis());
			Debug.Log(events.GetGripAxis());
		}
		anim.SetFloat("Grip", smooth);
	}
}
