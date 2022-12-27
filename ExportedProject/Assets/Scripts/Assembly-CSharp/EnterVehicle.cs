using UnityEngine;
using VRTK;

public class EnterVehicle : MonoBehaviour
{
	public GameObject cockpit;

	public GameObject StarRover;

	public GameObject player;

	private bool entering;

	private bool isDriving;

	private void enterVehicle()
	{
		entering = true;
	}

	private void OnEntering()
	{
		player.transform.position = cockpit.transform.position + new Vector3(-1f, 0.2f, 0.2f);
		player.transform.parent = cockpit.gameObject.transform;
		player.GetComponent<VRTK_TouchpadWalking>().enabled = false;
		isDriving = true;
	}

	private void Start()
	{
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		if (StarRover == null)
		{
			StarRover = GameObject.FindWithTag("StarRover");
		}
		if (cockpit == null)
		{
			cockpit = GameObject.FindWithTag("Cockpit");
		}
	}

	private void Update()
	{
		if (entering)
		{
			OnEntering();
			entering = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isDriving)
		{
			player.transform.parent = null;
			player.transform.position = StarRover.transform.position + new Vector3(0f, 1f, -5f);
			player.GetComponent<VRTK_TouchpadWalking>().enabled = true;
			isDriving = false;
		}
		else if (!isDriving)
		{
			enterVehicle();
		}
	}
}
