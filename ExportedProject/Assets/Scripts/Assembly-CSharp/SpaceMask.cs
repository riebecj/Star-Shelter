using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class SpaceMask : MonoBehaviour
{
	public Transform ClosedRot;

	public Transform OpenedRot;

	public GameObject foodPromt;

	public GameObject holsterIcon;

	public GameObject pukeParticle;

	public GameObject speedUI;

	internal bool lerp;

	public Transform target;

	internal bool open;

	public static SpaceMask instance;

	public Animator UI;

	public Animator radiationUI;

	public AudioSource breathAudio;

	public AudioSource openCloseAudioSource;

	public AudioClip openAudio;

	public AudioClip closeAudio;

	internal GameObject newPuke;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		Invoke("Parent", 0.1f);
	}

	private void Parent()
	{
		base.transform.SetParent(target);
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller" && (bool)other.GetComponentInParent<HandController>())
		{
			other.GetComponentInParent<HandController>().triggerTarget = base.transform;
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.GetComponentInParent<VRTK_ControllerEvents>().gameObject), 800f);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller" && (bool)other.GetComponentInParent<HandController>())
		{
			other.GetComponentInParent<HandController>().triggerTarget = null;
		}
	}

	public void OnTrigger()
	{
		if (lerp || DroneHelper.instance.VRControlled)
		{
			return;
		}
		if (!open)
		{
			lerp = true;
			open = true;
			StartCoroutine("LerpToOpenPoint");
			if (!IntroManager.instance)
			{
				StartCoroutine("Suffocate");
			}
			GameAudioManager.instance.ToggleMask(true);
			breathAudio.Stop();
			if (newPuke != null && newPuke.activeSelf)
			{
				newPuke.GetComponent<Puke>().OnOpenMask();
			}
		}
		else
		{
			lerp = true;
			StartCoroutine("LerpToClosestPoint");
			GameAudioManager.instance.ToggleMask(false);
			if (!SuitManager.instance.chokeAudio.isPlaying)
			{
				breathAudio.Play();
			}
		}
	}

	public void SnapClosed()
	{
		StopAllCoroutines();
		open = false;
		base.transform.localRotation = ClosedRot.localRotation;
		GameAudioManager.instance.ToggleMask(false);
		if ((bool)newPuke)
		{
			newPuke.gameObject.SetActive(false);
		}
	}

	public void ToggleFoodInfo(bool value)
	{
		foodPromt.SetActive(value);
	}

	private IEnumerator Suffocate()
	{
		while (open)
		{
			if ((!BaseManager.instance.inBase || BaseManager.instance.currentOxygenGroup.TotalOxygen < 5f) && !SuitManager.instance.canBreath)
			{
				SuitManager.instance.OnTakeDamage(1, 0);
			}
			yield return new WaitForSeconds(0.3f);
		}
	}

	public void ForceClose()
	{
		lerp = true;
		StartCoroutine("LerpToClosestPoint");
		open = false;
		GameAudioManager.instance.ToggleMask(false);
		if ((bool)newPuke)
		{
			newPuke.gameObject.SetActive(false);
		}
	}

	private IEnumerator LerpToClosestPoint()
	{
		openCloseAudioSource.PlayOneShot(closeAudio);
		while (lerp)
		{
			base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, ClosedRot.localRotation, 150f * Time.deltaTime);
			if (Mathf.Abs(base.transform.localEulerAngles.x - ClosedRot.localEulerAngles.x) < 1f)
			{
				lerp = false;
			}
			yield return new WaitForSeconds(0.01f);
		}
		open = false;
	}

	private IEnumerator LerpToOpenPoint()
	{
		openCloseAudioSource.PlayOneShot(openAudio);
		while (lerp)
		{
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, OpenedRot.rotation, 150f * Time.deltaTime);
			if (Mathf.Abs(base.transform.localEulerAngles.x - OpenedRot.localEulerAngles.x) < 1f)
			{
				lerp = false;
			}
			yield return new WaitForSeconds(0.01f);
		}
	}

	public void OnPuke()
	{
		if (IsInvoking("PukeCooldown") || (newPuke != null && !newPuke.GetComponent<Puke>().outOfMask))
		{
			return;
		}
		SuitManager.instance.nutrition -= SuitManager.instance.maxNutrition * 0.66f;
		SuitManager.instance.nutrition = Mathf.Clamp(SuitManager.instance.nutrition, 0f, SuitManager.instance.maxNutrition);
		Invoke("PukeCooldown", 15f);
		if (!DroneHelper.instance.VRControlled)
		{
			newPuke = Object.Instantiate(pukeParticle, base.transform.position, base.transform.rotation);
			newPuke.transform.SetParent(GameManager.instance.Head);
			newPuke.GetComponent<Puke>().StartCoroutine("MoveFromMouth");
			newPuke.transform.localPosition = new Vector3(0f, -0.08f, 0.04f);
			if (open)
			{
				newPuke.GetComponent<Puke>().OnOpenMask();
			}
		}
	}

	public void ToggleSpeedUI(bool value)
	{
		speedUI.SetActive(value);
		speedUI.GetComponent<Text>().text = GameManager.instance.CamRig.GetComponent<Rigidbody>().velocity.magnitude.ToString("F1") + "mps";
	}

	public void FadeBack()
	{
		VRTK_ScreenFade.Start(Color.clear, 0.3f);
	}

	private void PukeCooldown()
	{
	}
}
