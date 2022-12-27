using UnityEngine;

public class IntroGunEvent : MonoBehaviour
{
	public GameObject label;

	public static IntroGunEvent instance;

	internal bool holdingGun;

	public DoorSensor doorSensor;

	private void Awake()
	{
		instance = this;
		doorSensor.SetLock(false);
	}

	private void Start()
	{
		Invoke("Move", 1.5f);
	}

	private void Move()
	{
		Object.Destroy(Gun.instance.GetComponent<FixedJoint>());
		Gun.instance.GetComponent<Rigidbody>().isKinematic = true;
		Gun.instance.transform.position = base.transform.position;
		Gun.instance.transform.rotation = base.transform.rotation;
	}

	public void OnPickup()
	{
		IntroManager.instance.ToggleGun();
		holdingGun = true;
	}

	public void OnComplete()
	{
		doorSensor.SetLock(true);
		label.SetActive(false);
	}
}
