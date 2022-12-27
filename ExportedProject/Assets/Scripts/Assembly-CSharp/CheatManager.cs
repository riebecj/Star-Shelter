using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour
{
	public GameObject maskUI;

	public GameObject interior;

	internal bool singleCamMask;

	public GameObject[] Cameras;

	public GameObject[] buildStages;

	public Vector3[] StartPositions;

	internal AudioSource PieAudio;

	internal int buildIndex;

	internal bool active;

	public GameObject ArmUI;

	private void Start()
	{
		PieAudio = SuitManager.instance.audioSource;
		StartPositions[0] = Cameras[0].transform.GetChild(0).position;
		StartPositions[1] = Cameras[1].transform.GetChild(0).position;
		if (Application.isEditor)
		{
			active = true;
		}
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.R))
		{
			active = true;
		}
		if (GameManager.instance.DemoBuild || Application.isEditor)
		{
			if (Input.GetKeyDown(KeyCode.D))
			{
				GameObject gameObject = Object.Instantiate(DroneManager.instance.Drone, GameManager.instance.Head.position + Random.onUnitSphere * 10f, base.transform.rotation);
				gameObject.GetComponent<DroneAI>().target = GameManager.instance.Head;
				gameObject.GetComponent<DroneAI>().attacking = true;
				DroneManager.instance.drones.Add(gameObject);
			}
			if (Input.GetKeyDown(KeyCode.C))
			{
				CometManager.instance.KillComet();
			}
		}
		else if (!Input.GetKeyDown(KeyCode.C))
		{
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			if (maskUI.GetComponent<Canvas>().enabled)
			{
				Canvas[] componentsInChildren = maskUI.GetComponentsInChildren<Canvas>();
				Canvas[] array = componentsInChildren;
				foreach (Canvas canvas in array)
				{
					canvas.enabled = false;
				}
			}
			else
			{
				Canvas[] componentsInChildren2 = maskUI.GetComponentsInChildren<Canvas>();
				Canvas[] array2 = componentsInChildren2;
				foreach (Canvas canvas2 in array2)
				{
					canvas2.enabled = true;
				}
			}
		}
		if (!active)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			SpaceMask.instance.OnPuke();
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			for (int k = 0; k < NanoInventory.instance.craftMaterials.Count; k++)
			{
				NanoInventory.instance.materialCounts[k] += 5;
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			DisableOthers();
			Cameras[0].SetActive(!Cameras[0].activeSelf);
			Cameras[0].GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
			Cameras[0].transform.GetChild(0).position = StartPositions[0];
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			DisableOthers();
			Cameras[1].SetActive(!Cameras[1].activeSelf);
			Cameras[1].GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
			Cameras[1].transform.GetChild(0).position = StartPositions[1];
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			DisableOthers();
			Cameras[2].SetActive(!Cameras[2].activeSelf);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			DisableOthers();
			Cameras[3].SetActive(!Cameras[3].activeSelf);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			DisableOthers();
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			CometManager.instance.TestComet();
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			interior.SetActive(!interior.activeSelf);
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			if (PieAudio.volume == 0.5f)
			{
				PieAudio.volume = 0f;
			}
			else
			{
				PieAudio.volume = 0.5f;
			}
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if (buildIndex > buildStages.Length)
			{
				buildIndex = buildStages.Length - 1;
			}
			buildIndex--;
			if (buildIndex < -1)
			{
				buildIndex = -1;
			}
			for (int l = 0; l < buildStages.Length; l++)
			{
				if (l > buildIndex)
				{
					buildStages[l].SetActive(false);
				}
			}
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			if (buildIndex < 0)
			{
				buildIndex = 0;
			}
			buildIndex++;
			if (buildIndex > buildStages.Length)
			{
				buildIndex = buildStages.Length;
			}
			for (int m = 0; m < buildStages.Length; m++)
			{
				if (m < buildIndex)
				{
					buildStages[m].SetActive(true);
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			SuitManager.instance.oxygen = Mathf.Clamp(SuitManager.instance.oxygen - 25f, 0f, SuitManager.instance.maxOxygen);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			SuitManager.instance.oxygen = Mathf.Clamp(SuitManager.instance.oxygen + 25f, 0f, SuitManager.instance.maxOxygen);
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			if (maskUI.GetComponent<Canvas>().enabled)
			{
				Canvas[] componentsInChildren3 = maskUI.GetComponentsInChildren<Canvas>();
				Canvas[] array3 = componentsInChildren3;
				foreach (Canvas canvas3 in array3)
				{
					canvas3.enabled = false;
				}
			}
			else
			{
				Canvas[] componentsInChildren4 = maskUI.GetComponentsInChildren<Canvas>();
				Canvas[] array4 = componentsInChildren4;
				foreach (Canvas canvas4 in array4)
				{
					canvas4.enabled = true;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			singleCamMask = !singleCamMask;
			if (!singleCamMask)
			{
				maskUI.transform.parent.localScale = new Vector3(0.5f, 0.4f, 0.5f);
				maskUI.transform.parent.localPosition = new Vector3(-0.035f, 0f, 0f);
			}
			else
			{
				maskUI.transform.parent.localScale = new Vector3(1f, 1f, 1f);
				maskUI.transform.parent.localPosition = new Vector3(0f, 0f, 0f);
			}
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Delete))
		{
			PlayerPrefs.DeleteAll();
			GameManager.instance.deleteSaves = true;
			SceneManager.LoadScene("MainScene");
		}
	}

	private void DisableOthers()
	{
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = false;
		GameObject[] cameras = Cameras;
		foreach (GameObject gameObject in cameras)
		{
			gameObject.SetActive(false);
		}
	}
}
