using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockerPanel : MonoBehaviour
{
	private AudioSource audioSource;

	public AudioClip beep;

	public Text codeBack;

	public Text codeFront;

	public Text lockerNumber;

	public string password = "3416";

	public Rigidbody target;

	public GameObject panel;

	public GameObject button;

	public GameObject codeSource;

	internal List<Transform> codeSourcePositions = new List<Transform>();

	public GameObject[] ActivateOnComplete;

	public GameObject[] DeactivateOnComplete;

	public static int lockerIndex;

	private int myIndex;

	private LootSpawner[] lootSpawners;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		password = string.Empty;
		for (int i = 0; i < 4; i++)
		{
			password += Random.Range(1, 10);
		}
		lockerIndex++;
		myIndex = lockerIndex;
		lockerNumber.text = myIndex.ToString();
		Invoke("FindNotePad", 1.5f);
		lootSpawners = base.transform.parent.GetComponentsInChildren<LootSpawner>();
		LootSpawner[] array = lootSpawners;
		foreach (LootSpawner lootSpawner in array)
		{
			lootSpawner.CancelInvoke("OnSpawn");
		}
	}

	private void FindNotePad()
	{
		for (int i = 0; i < SpawnNode.spawnNodes.Count; i++)
		{
			if (Vector3.Distance(base.transform.position, SpawnNode.spawnNodes[i].position) < 50f)
			{
				codeSourcePositions.Add(SpawnNode.spawnNodes[i]);
				SpawnNode.spawnNodes.Remove(SpawnNode.spawnNodes[i]);
			}
		}
		if (codeSourcePositions.Count <= 0)
		{
			return;
		}
		if (codeSource == null)
		{
			codeSource = GameManager.instance.notePad;
		}
		Transform transform = codeSourcePositions[Random.Range(0, codeSourcePositions.Count)];
		GameObject gameObject = (codeSource = Object.Instantiate(codeSource, transform.position, transform.rotation));
		base.gameObject.name = "Derp";
		Text[] componentsInChildren = gameObject.GetComponentsInChildren<Text>();
		Text[] array = componentsInChildren;
		foreach (Text text in array)
		{
			if (!text.text.Contains("Locker"))
			{
				text.text = "<color=yellow>L" + myIndex + "</color>. " + password;
			}
		}
		transform.GetComponent<SpawnNode>().OnSpawn();
		if (transform.GetComponent<SpawnNode>().randomStartForce)
		{
			gameObject.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * 10f);
		}
		base.name = "LockerWithCode";
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
		else if (codeBack.text.Length < 6)
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
		target.isKinematic = false;
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
		panel.SetActive(false);
		GetComponent<Collider>().enabled = false;
		LootSpawner[] array = lootSpawners;
		foreach (LootSpawner lootSpawner in array)
		{
			lootSpawner.OnSpawn();
		}
	}

	public void EnablePanel()
	{
		panel.SetActive(true);
		button.SetActive(false);
	}

	private void InputCooldown()
	{
	}

	private void OnDisable()
	{
		CancelInvoke("FindNotePad");
	}
}
