using UnityEngine;
using UnityEngine.UI;

public class CodePanel : MonoBehaviour
{
	private AudioSource audioSource;

	public AudioClip beep;

	public Text codeBack;

	public Text codeFront;

	public string password = "3416";

	public Animator target;

	public GameObject[] ActivateOnComplete;

	public GameObject[] DeactivateOnComplete;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		password = string.Empty;
		for (int i = 0; i < 6; i++)
		{
			password += Random.Range(1, 10);
		}
	}

	public void GetButton(int number)
	{
		if (IsInvoking("InputCooldown"))
		{
			return;
		}
		audioSource.PlayOneShot(beep, 0.8f);
		if (codeBack.text.StartsWith("E"))
		{
			codeBack.text = string.Empty;
			codeFront.text = string.Empty;
		}
		Invoke("InputCooldown", 0.25f);
		if (number == 0)
		{
			if (codeBack.text.Length > 0)
			{
				codeBack.text = codeBack.text.Substring(0, codeBack.text.Length - 1);
				codeFront.text = codeFront.text.Substring(0, codeFront.text.Length - 1);
			}
		}
		else if (codeBack.text.Length < 4)
		{
			codeBack.text += number;
			codeFront.text += number;
		}
		if (codeBack.text == password)
		{
			Invoke("OnComplete", 0.5f);
		}
	}

	private void OnComplete()
	{
		if (target != null)
		{
			target.SetBool("Open", true);
		}
		GameObject[] deactivateOnComplete = DeactivateOnComplete;
		foreach (GameObject gameObject in deactivateOnComplete)
		{
			gameObject.SetActive(false);
		}
		GameObject[] activateOnComplete = ActivateOnComplete;
		foreach (GameObject gameObject2 in activateOnComplete)
		{
			gameObject2.SetActive(true);
		}
	}

	private void InputCooldown()
	{
	}
}
