using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class WheelLock : MonoBehaviour
{
	private float currentPull;

	private float previousPull;

	private float Vibration = 800f;

	private VRTK_InteractableObject interact;

	public Rigidbody _lock;

	public Transform wheel;

	public string code;

	public Text codeVisual;

	private int index;

	private int oldNumber;

	public int currentNumber = 1;

	private int lastNumber;

	public Transform tracker;

	public RectTransform[] numbersLabels;

	public bool seeking = true;

	public bool right = true;

	public bool lastMove;

	private bool oldDirection;

	private float oldDir;

	private Vector3 oldFwd;

	private Color storedColor;

	public Color MarkedColor = Color.blue;

	private AudioSource audioSource;

	public AudioClip unlockSound;

	public AudioClip deepThud;

	public AudioClip click;

	public AudioClip failSound;

	public AudioClip beep;

	private SteamVR_ControllerManager controllers;

	private VRTK_ControllerEvents holdControl;

	private void Start()
	{
		controllers = Object.FindObjectOfType<SteamVR_ControllerManager>();
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		storedColor = numbersLabels[0].transform.GetComponent<Text>().color;
		audioSource = GetComponent<AudioSource>();
	}

	public bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
		{
			holdControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>();
		}
		else
		{
			holdControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
		}
	}

	private void Update()
	{
		if (IsHeld())
		{
			UpdateWheel();
		}
	}

	private void CheckCode()
	{
		if (right != oldDirection)
		{
			index++;
			oldDirection = right;
			audioSource.PlayOneShot(beep, 0.8f);
			codeVisual.text += lastNumber;
		}
		else
		{
			if (codeVisual.text.Length > index)
			{
				codeVisual.text.Remove(codeVisual.text.Length);
			}
			codeVisual.text += lastNumber;
		}
		bool flag = true;
		if (codeVisual.text != code)
		{
			flag = false;
		}
		if (index == 4 && !flag)
		{
			index = 0;
			audioSource.PlayOneShot(failSound, 0.8f);
			for (int i = 0; i < 4; i++)
			{
				codeVisual.text = string.Empty;
			}
			oldDirection = false;
			right = false;
		}
		if (flag)
		{
			Open();
		}
	}

	private void CheckDirection()
	{
		Vector3 lhs = Vector3.Cross(wheel.transform.forward, oldFwd);
		if (Vector3.Dot(lhs, Vector3.right) > 0f)
		{
			right = true;
		}
		else
		{
			right = false;
		}
		oldFwd = wheel.transform.forward;
		if (!seeking && right != oldDirection)
		{
			seeking = true;
		}
	}

	private void Open()
	{
		GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
		Object.Destroy(GetComponent<VRTK_InteractableObject>());
		_lock.isKinematic = false;
		VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration);
		RectTransform rectTransform = numbersLabels[currentNumber - 1];
		rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, 0f);
		rectTransform.transform.GetComponent<Text>().color = storedColor;
		audioSource.pitch = 0.4f;
		audioSource.PlayOneShot(unlockSound, 0.8f);
		for (int i = 0; i < numbersLabels.Length; i++)
		{
			RectTransform rectTransform2 = numbersLabels[i];
			rectTransform2.anchoredPosition3D = new Vector3(rectTransform2.anchoredPosition3D.x, rectTransform2.anchoredPosition3D.y, 0f);
			rectTransform2.transform.GetComponent<Text>().color = storedColor;
		}
	}

	private void UpdateWheel()
	{
		float a = (currentPull = AxisFromPosition());
		float num = 40f;
		a = mod(a, 360f);
		if (!(currentPull / 15f).ToString("F0").Equals((previousPull / 15f).ToString("F0")) && IsHeld())
		{
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration);
		}
		lastNumber = currentNumber;
		previousPull = currentPull;
		if (a < 40f)
		{
			currentNumber = 9;
		}
		else if (a > 40f && a < 80f)
		{
			currentNumber = 8;
		}
		else if (a > 80f && a < 120f)
		{
			currentNumber = 7;
		}
		else if (a > 120f && a < 160f)
		{
			currentNumber = 6;
		}
		else if (a > 160f && a < 200f)
		{
			currentNumber = 5;
		}
		else if (a > 200f && a < 240f)
		{
			currentNumber = 4;
		}
		else if (a > 240f && a < 280f)
		{
			currentNumber = 3;
		}
		else if (a > 280f && a < 320f)
		{
			currentNumber = 2;
		}
		else if (a > 320f)
		{
			currentNumber = 1;
		}
		if (currentNumber != oldNumber)
		{
			CheckDirection();
			UpdateNumbers(currentNumber);
			CheckCode();
			oldNumber = currentNumber;
		}
	}

	private void UpdateNumbers(int currentNumber)
	{
		for (int i = 0; i < numbersLabels.Length; i++)
		{
			RectTransform rectTransform = numbersLabels[i];
			rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, 0f);
			rectTransform.transform.GetComponent<Text>().color = storedColor;
		}
		RectTransform rectTransform2 = numbersLabels[currentNumber - 1];
		rectTransform2.anchoredPosition3D = new Vector3(rectTransform2.anchoredPosition3D.x, rectTransform2.anchoredPosition3D.y, -15f);
		rectTransform2.transform.GetComponent<Text>().color = MarkedColor;
		audioSource.PlayOneShot(click, 0.8f);
	}

	private float mod(float a, float b)
	{
		return a - b * Mathf.Floor(a / b);
	}

	private float AxisFromPosition()
	{
		float num = 0f;
		Vector3 normalized = (tracker.position - wheel.position).normalized;
		float num2 = Mathf.Atan2(normalized.y, normalized.z) * 57.29578f;
		num2 = 90f - num2;
		if (num2 < 0f)
		{
			num2 += 360f;
		}
		return 360f - num2;
	}

	private void EndRot()
	{
		seeking = true;
	}
}
