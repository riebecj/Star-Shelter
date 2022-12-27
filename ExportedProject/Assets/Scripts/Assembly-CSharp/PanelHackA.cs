using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelHackA : MonoBehaviour
{
	public float hackTime = 10f;

	internal float restStartTime;

	public Transform[] wheels;

	public Text[] counter;

	public Text[] labels;

	public Image[] unlockImages;

	public Transform upReference;

	internal bool active;

	internal bool countDown;

	public GameObject hackUI;

	public GameObject hackButton;

	public GameObject panelUI;

	public GameObject openButton;

	private Transform currentWheel;

	public Animator target;

	public int powerCost = 10;

	public Color colorIn;

	public Color colorOut;

	internal AudioSource audioSource;

	internal bool hasTarget;

	public bool findDoor = true;

	public AudioClip StartHackAudio;

	public AudioClip HackCompleteAudio;

	public AudioClip HackFailedAudio;

	public AudioClip targetWheelAudio;

	private GameObject[] doors;

	private void Start()
	{
		Text[] array = counter;
		foreach (Text text in array)
		{
			text.text = hackTime.ToString();
		}
		restStartTime = hackTime;
		audioSource = GetComponent<AudioSource>();
		AssignGenerator();
		StartCoroutine("CheckPlayerProximity");
		if (target == null)
		{
			if (!findDoor)
			{
				return;
			}
			doors = GameObject.FindGameObjectsWithTag("Door");
			if (GetClosestDoor() != null)
			{
				target = GetClosestDoor().GetComponent<Animator>();
				if ((bool)target.GetComponent<DoorSensor>())
				{
					target.GetComponent<DoorSensor>().SetLock(false);
				}
			}
		}
		else if ((bool)target.GetComponent<DoorSensor>())
		{
			target.GetComponent<DoorSensor>().SetLock(false);
		}
	}

	private void OnEnable()
	{
		StartCoroutine("CheckPlayerProximity");
	}

	private void AssignGenerator()
	{
		foreach (PowerGenerator powerGenerator in PowerGenerator.powerGenerators)
		{
			if (Vector3.Distance(base.transform.position, powerGenerator.transform.position) < (float)PowerGenerator.generatorDistance)
			{
				powerGenerator.togglePower = (PowerGenerator.TogglePower)Delegate.Combine(powerGenerator.togglePower, new PowerGenerator.TogglePower(TogglePower));
			}
		}
	}

	public void TogglePower(bool on)
	{
		if (on)
		{
			panelUI.SetActive(true);
		}
		else
		{
			panelUI.SetActive(false);
		}
	}

	private void Shuffle()
	{
		Transform[] array = wheels;
		foreach (Transform transform in array)
		{
			transform.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0, 360));
		}
		Shuffle(wheels);
	}

	private void CheckAlignment()
	{
		bool flag = true;
		for (int i = 0; i < wheels.Length; i++)
		{
			if (Vector3.Angle(upReference.up, wheels[i].up) > 8f)
			{
				flag = false;
				wheels[i].GetComponent<PanelWheel>().wheelColor.color = colorOut;
				wheels[i].GetComponent<PanelWheel>().Vibration = 0.025f;
			}
			else
			{
				wheels[i].GetComponent<PanelWheel>().wheelColor.color = colorIn;
				wheels[i].GetComponent<PanelWheel>().Vibration = 0.08f;
			}
		}
		if (flag)
		{
			OnComplete();
		}
	}

	private void OnComplete()
	{
		active = false;
		StopCoroutine("CountDown");
		panelUI.SetActive(false);
		hackUI.GetComponent<Animator>().SetBool("Active", false);
		if (target != null && (bool)target.GetComponent<DoorSensor>())
		{
			target.GetComponent<DoorSensor>().SetLock(true);
		}
		Text[] array = labels;
		foreach (Text text in array)
		{
			text.text = "OPEN";
		}
		Image[] array2 = unlockImages;
		foreach (Image image in array2)
		{
			image.color = colorIn;
		}
		if (openButton != null)
		{
			openButton.SetActive(true);
		}
		audioSource.PlayOneShot(HackCompleteAudio);
	}

	private IEnumerator CheckPlayerProximity()
	{
		Transform head = GameManager.instance.Head;
		bool activated = false;
		while (true)
		{
			if (Vector3.Distance(base.transform.position, head.position) < 2.5f)
			{
				hackButton.GetComponent<Animator>().SetBool("Active", true);
				activated = true;
			}
			else if (activated)
			{
				hackButton.GetComponent<Animator>().SetBool("Active", false);
				activated = false;
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator UpdateState()
	{
		while (active)
		{
			CheckAlignment();
			if (currentWheel == wheels[0])
			{
				wheels[1].transform.eulerAngles = new Vector3(wheels[1].transform.eulerAngles.x, wheels[1].transform.eulerAngles.y, wheels[1].transform.eulerAngles.z + wheels[0].GetComponent<PanelWheel>().deltaAngle);
				wheels[2].transform.eulerAngles = new Vector3(wheels[2].transform.eulerAngles.x, wheels[2].transform.eulerAngles.y, wheels[2].transform.eulerAngles.z + wheels[0].GetComponent<PanelWheel>().deltaAngle);
			}
			else if (currentWheel == wheels[1] && currentWheel == wheels[1])
			{
				wheels[2].transform.eulerAngles = new Vector3(wheels[2].transform.eulerAngles.x, wheels[2].transform.eulerAngles.y, wheels[2].transform.eulerAngles.z + wheels[1].GetComponent<PanelWheel>().deltaAngle);
			}
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator CountDown()
	{
		while (countDown)
		{
			hackTime -= 1f;
			Text[] array = counter;
			foreach (Text text in array)
			{
				text.text = hackTime.ToString();
			}
			if (hackTime < 1f)
			{
				countDown = false;
				Reset();
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public void StartHack()
	{
		if (SuitManager.instance.power > (float)powerCost)
		{
			SuitManager.instance.power -= powerCost;
			Shuffle();
			upReference.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0, 360));
			hackUI.SetActive(true);
			hackButton.SetActive(false);
			countDown = true;
			StartCoroutine("CountDown");
			audioSource.PlayOneShot(StartHackAudio);
			hackUI.GetComponent<Animator>().SetBool("Active", true);
		}
		else
		{
			SuitManager.instance.LowPowerPrompt();
		}
	}

	private void Reset()
	{
		hackTime = restStartTime;
		hackUI.SetActive(false);
		hackButton.SetActive(true);
		audioSource.PlayOneShot(HackFailedAudio);
	}

	public void Activate(Transform wheel)
	{
		currentWheel = wheel;
		if (!active)
		{
			active = true;
			StartCoroutine("UpdateState");
		}
	}

	public void Deactivate()
	{
		active = false;
		StopCoroutine("UpdateState");
		Transform[] array = wheels;
		foreach (Transform transform in array)
		{
			transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
	}

	public static void Shuffle<T>(T[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			T val = array[i];
			int num = UnityEngine.Random.Range(i, array.Length);
			array[i] = array[num];
			array[num] = val;
		}
	}

	private Transform GetClosestDoor()
	{
		Transform result = null;
		float num = 25f;
		GameObject[] array = doors;
		foreach (GameObject gameObject in array)
		{
			float num2 = Vector3.Distance(gameObject.transform.position, base.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = gameObject.transform;
			}
		}
		return result;
	}
}
