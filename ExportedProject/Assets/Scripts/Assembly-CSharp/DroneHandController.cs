using UnityEngine;
using VRTK;

public class DroneHandController : MonoBehaviour
{
	private float gripSmoothing;

	public VRTK_ControllerEvents events;

	public GameObject cone;

	public Animator handAnim;

	private Animator anim;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		if (DroneHelper.instance.VRControlled)
		{
			gripSmoothing = Mathf.MoveTowards(gripSmoothing, events.GetTriggerAxis(), 5f * Time.deltaTime);
			anim.SetFloat("Grip", gripSmoothing);
			anim.SetBool("Point", handAnim.GetBool("Point"));
		}
	}

	public void PoseTrigger()
	{
	}
}
