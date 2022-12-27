using UnityEngine;
using VRTK;

public class CryoPodLever : MonoBehaviour
{
	public GameObject Pod;

	public GameObject Lid;

	public GameObject particle;

	public Transform parent;

	internal VRTK_InteractableObject interact;

	private float value;

	internal bool dropped;

	internal bool isHeld;

	internal bool open;

	public AudioClip openSound;

	public AudioClip SSOrigin;

	private AudioSource audioSource;

	public GameObject openQue;

	public static CryoPodLever instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		interact = GetComponent<VRTK_InteractableObject>();
	}

	public bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void Update()
	{
		if (IsHeld() || isHeld)
		{
			UpdateValue();
			dropped = false;
			GetComponent<HingeJoint>().useSpring = false;
		}
		else if (Mathf.Abs(value) > 0.01f && !dropped)
		{
			dropped = true;
			GetComponent<HingeJoint>().useSpring = true;
		}
	}

	private void UpdateValue()
	{
		value = Vector3.Cross(base.transform.forward, parent.forward).magnitude;
		if (value > 0.95f && !open)
		{
			TutorialManager.instance.OnGrab();
			open = true;
			audioSource.PlayOneShot(openSound);
			Invoke("OnOpen", 1f);
		}
	}

	public void OnOpen()
	{
		interact.ForceStopInteracting();
		Pod.transform.parent.GetComponent<StartCapsule>().enabled = false;
		Pod.GetComponent<Rigidbody>().isKinematic = false;
		Pod.GetComponent<Rigidbody>().AddForce(Pod.transform.up * 750f);
		Lid.GetComponent<Rigidbody>().isKinematic = false;
		Lid.GetComponent<Rigidbody>().AddForce(-Lid.transform.up * 750f);
		particle.transform.SetParent(null);
		particle.SetActive(true);
		Invoke("Destruct", 10f);
		openQue.SetActive(true);
		Invoke("PlayOriginAudio", 8f);
		GameAudioManager.instance.ToggleSpace();
		foreach (Thruster thruster in Thruster.thrusters)
		{
			thruster.deactivated = false;
		}
	}

	private void PlayOriginAudio()
	{
		GameAudioManager.instance.AddToSuitQueue(SSOrigin);
	}

	private void Destruct()
	{
		Pod.SetActive(false);
		Lid.SetActive(false);
	}
}
