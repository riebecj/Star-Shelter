using UnityEngine;

public class PowerGrid : MonoBehaviour
{
	public PowerGridNode[] nodes;

	public GameObject[] lights;

	public int activeLight;

	public Material lightOn;

	public int[] circuits = new int[9];

	private AudioSource audioSource;

	public AudioClip startup;

	public AudioClip buzz;

	public AudioClip click;

	internal bool complete;

	public PowerGenerator powerGenerator;

	private void Start()
	{
		PickLight();
		audioSource = GetComponent<AudioSource>();
	}

	private void PickLight()
	{
		int num = (activeLight = Random.Range(0, 6));
		lights[num].GetComponent<MeshRenderer>().material = lightOn;
	}

	public void CheckCircuit()
	{
		if (activeLight == 0)
		{
			if (circuits[6] == 1 && circuits[7] == 1 && circuits[8] == 1)
			{
				OnComplete();
			}
		}
		else if (activeLight == 1)
		{
			if (circuits[3] == 1 && circuits[4] == 1 && circuits[5] == 1)
			{
				OnComplete();
			}
		}
		else if (activeLight == 2)
		{
			if (circuits[0] == 1 && circuits[1] == 1 && circuits[2] == 1)
			{
				OnComplete();
			}
		}
		else if (activeLight == 3)
		{
			if (circuits[0] == 1 && circuits[3] == 1 && circuits[6] == 1)
			{
				OnComplete();
			}
		}
		else if (activeLight == 4)
		{
			if (circuits[1] == 1 && circuits[4] == 1 && circuits[7] == 1)
			{
				OnComplete();
			}
		}
		else if (activeLight == 5 && circuits[2] == 1 && circuits[5] == 1 && circuits[8] == 1)
		{
			OnComplete();
		}
		audioSource.PlayOneShot(click);
	}

	private void OnComplete()
	{
		PowerGridNode[] array = nodes;
		foreach (PowerGridNode powerGridNode in array)
		{
			if (powerGridNode.powerCard != null)
			{
				powerGridNode.powerCard.GetComponent<Collider>().enabled = false;
			}
		}
		complete = true;
	}

	public void Activate()
	{
		if (complete)
		{
			powerGenerator.togglePower(true);
			audioSource.PlayOneShot(startup);
			Invoke("StartBuzz", 2f);
			audioSource.PlayOneShot(click);
		}
	}

	private void StartBuzz()
	{
		audioSource.clip = buzz;
		audioSource.Play();
	}
}
