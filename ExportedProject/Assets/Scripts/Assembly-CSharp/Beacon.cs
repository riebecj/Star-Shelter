using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Beacon : MonoBehaviour
{
	public GameObject Titan;

	public AudioSource audioSource;

	public AudioClip beep;

	public AudioClip addDiscAudio;

	public AudioClip detectionWarningAudio;

	public AudioClip preWarning;

	public Text codeBack;

	public Text codeFront;

	public Text numpadBack;

	public Text numpadFront;

	public Text sendButtonBack;

	public Text sendButtonFront;

	private Color storedButtonColor;

	public Image sendBorder;

	public string password = "3416";

	public Animation anim;

	public Transform discPosition;

	internal bool discAdded;

	internal bool signalSent;

	private Transform disc;

	public GameObject tempDisc;

	public Color buttonColor;

	private Vector3 startPos;

	private void Start()
	{
		storedButtonColor = sendButtonBack.color;
		startPos = base.transform.position;
		if (PreviewLabs.PlayerPrefs.GetBool(startPos.ToString() + "BeaconDisc"))
		{
			disc = tempDisc.transform;
			tempDisc.SetActive(true);
			OnEnterDisc();
		}
		Invoke("CheckTitanSummon", 3f);
	}

	private void CheckTitanSummon()
	{
		if (GameManager.instance.inTitanEvent)
		{
			OnSummonTitan();
		}
	}

	public void GetButton(int number)
	{
		if (IsInvoking("InputCooldown"))
		{
			return;
		}
		audioSource.PlayOneShot(beep, 0.8f);
		if (numpadBack.text.StartsWith("E"))
		{
			numpadBack.text = string.Empty;
			numpadFront.text = string.Empty;
		}
		Invoke("InputCooldown", 0.25f);
		if (number == 0)
		{
			if (numpadBack.text.Length > 0)
			{
				numpadBack.text = numpadBack.text.Substring(0, numpadBack.text.Length - 1);
				numpadFront.text = numpadFront.text.Substring(0, numpadFront.text.Length - 1);
			}
		}
		else if (numpadBack.text.Length < 4)
		{
			numpadBack.text += number;
			numpadFront.text += number;
		}
		if (numpadBack.text == password && discAdded)
		{
			Invoke("OnComplete", 0.5f);
		}
	}

	public void OnComplete()
	{
		sendButtonBack.color = buttonColor;
		sendButtonFront.color = buttonColor;
		sendBorder.color = buttonColor;
	}

	public void StartSearch()
	{
		if (numpadBack.text == password && discAdded && !signalSent)
		{
			audioSource.PlayOneShot(beep, 0.8f);
			signalSent = true;
			anim.Play();
			codeBack.text = "****";
			codeFront.text = "****";
			numpadBack.text = string.Empty;
			numpadFront.text = string.Empty;
			sendButtonBack.color = storedButtonColor;
			sendButtonFront.color = storedButtonColor;
			sendBorder.color = storedButtonColor;
			Invoke("OnSummonTitan", 14f);
		}
	}

	private void OnEnterDisc()
	{
		discAdded = true;
		password = disc.GetComponent<BeaconDisc>().code.ToString();
		codeBack.text = password;
		codeFront.text = password;
		PreviewLabs.PlayerPrefs.SetBool(startPos.ToString() + "BeaconDisc", true);
		GameAudioManager.instance.AddToSuitQueue(preWarning);
		if ((bool)disc.GetComponentInChildren<UIBillboard>())
		{
			disc.GetComponentInChildren<UIBillboard>().gameObject.SetActive(false);
		}
	}

	private void OnSummonTitan()
	{
		float num = 85f;
		if (Random.Range(0, 1) == 1)
		{
			num *= -1f;
		}
		Vector3 position = base.transform.position + new Vector3(num, 0f, num);
		Object.Instantiate(Titan, position, Quaternion.identity);
		if ((bool)disc)
		{
			disc.gameObject.SetActive(false);
			PreviewLabs.PlayerPrefs.SetBool(startPos.ToString() + "BeaconDisc", false);
			GameManager.instance.inTitanEvent = true;
			discAdded = false;
			disc = null;
			signalSent = false;
		}
		Invoke("DetectWarning", 1f);
	}

	private void DetectWarning()
	{
		GameAudioManager.instance.AddToSuitQueue(detectionWarningAudio);
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<BeaconDisc>() && other.GetComponent<VRTK_InteractableObject>().IsGrabbed() && !GameManager.instance.inTitanEvent && disc == null)
		{
			disc = other.transform;
			disc.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			disc.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			disc.GetComponent<VRTK_InteractableObject>().previousParent = base.transform;
			disc.GetComponent<VRTK_InteractableObject>().previousKinematicState = true;
			disc.GetComponent<Collider>().enabled = false;
			other.isTrigger = true;
			disc.position = discPosition.position;
			disc.rotation = discPosition.rotation;
			OnEnterDisc();
			audioSource.PlayOneShot(addDiscAudio, 0.8f);
			Invoke("SetDisc", 0.25f);
		}
	}

	private void SetDisc()
	{
		disc.position = discPosition.position;
		disc.rotation = discPosition.rotation;
	}
}
