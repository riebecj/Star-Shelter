using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class IOChud : MonoBehaviour
{
	private Texture2D Icon;

	private bool iocActive;

	private IOCcam ioc;

	private bool realtimeShadows;

	private bool hud;

	private bool dirty;

	private Color colEnabled;

	private Color colDisabld;

	private void Awake()
	{
		Icon = (Texture2D)Resources.Load("Icon");
		hud = false;
		dirty = false;
		colEnabled = GUI.color;
		colDisabld = new Color(colEnabled.r, colEnabled.g, colEnabled.b, 0.2f);
	}

	private void Start()
	{
		ioc = Camera.main.transform.GetComponent<IOCcam>();
		iocActive = ioc.enabled;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.I))
		{
			iocActive = !iocActive;
			ToggleIOC();
		}
		if (Input.GetKeyUp(KeyCode.H))
		{
			ToggleHUD();
		}
		if (Input.GetMouseButtonUp(0) && dirty)
		{
			ToggleIOC();
			dirty = false;
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(25f, 10f, 80f, 20f), "Toggle HUD"))
		{
			ToggleHUD();
		}
		GUI.Label(new Rect(120f, 10f, 360f, 20f), "Press 'i' to toggle InstantOC - Press 'h' to toggle HUD");
		if (hud)
		{
			GUI.Label(new Rect(25f, 35f, 320f, 20f), "Samples");
			ioc.samples = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(25f, 55f, 150f, 20f), ioc.samples, 10f, 2000f));
			GUI.Label(new Rect(180f, 50f, 50f, 20f), ioc.samples.ToString());
			GUI.Label(new Rect(25f, 65f, 320f, 20f), "Hide delay");
			ioc.hideDelay = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(25f, 85f, 150f, 20f), ioc.hideDelay, 10f, 300f));
			GUI.Label(new Rect(180f, 80f, 50f, 20f), ioc.hideDelay.ToString());
			GUI.Label(new Rect(25f, 95f, 320f, 20f), "View Distance");
			ioc.viewDistance = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(25f, 115f, 150f, 20f), ioc.viewDistance, 100f, 3000f));
			GUI.Label(new Rect(180f, 110f, 50f, 20f), ioc.viewDistance.ToString());
			GUI.Label(new Rect(25f, 125f, 320f, 20f), "Lod 1");
			ioc.lod1Distance = Mathf.Round(GUI.HorizontalSlider(new Rect(25f, 145f, 150f, 20f), ioc.lod1Distance, 10f, 300f));
			GUI.Label(new Rect(180f, 140f, 50f, 20f), ioc.lod1Distance.ToString());
			GUI.Label(new Rect(25f, 155f, 320f, 20f), "Lod 2");
			ioc.lod2Distance = Mathf.Round(GUI.HorizontalSlider(new Rect(25f, 175f, 150f, 20f), ioc.lod2Distance, 10f, 600f));
			GUI.Label(new Rect(180f, 170f, 50f, 20f), ioc.lod2Distance.ToString());
		}
		if (iocActive)
		{
			GUI.color = colEnabled;
		}
		else
		{
			GUI.color = colDisabld;
		}
		if (GUI.Button(new Rect((float)Screen.width - 74f, 10f, 64f, 64f), Icon, string.Empty))
		{
			iocActive = !iocActive;
			ToggleIOC();
		}
		if (GUI.changed)
		{
			dirty = true;
		}
	}

	private void ToggleHUD()
	{
		switch (hud)
		{
		case true:
			hud = false;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			break;
		case false:
			hud = true;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			break;
		}
		try
		{
			ioc.transform.parent.GetComponent<FirstPersonController>().enabled = !ioc.transform.parent.GetComponent<FirstPersonController>().enabled;
		}
		catch
		{
		}
	}

	private void ToggleIOC()
	{
		ioc.enabled = iocActive;
		GameObject[] array = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			IOClod component = gameObject.GetComponent<IOClod>();
			IOClight component2 = gameObject.GetComponent<IOClight>();
			IOCterrain component3 = gameObject.GetComponent<IOCterrain>();
			if (component != null)
			{
				switch (iocActive)
				{
				case true:
					component.UpdateValues();
					component.Initialize();
					component.enabled = true;
					break;
				case false:
					component.enabled = false;
					component.UpdateValues();
					component.Initialize();
					break;
				}
			}
			if (component2 != null)
			{
				switch (iocActive)
				{
				case true:
					component2.Initialize();
					component2.enabled = true;
					break;
				case false:
					component2.enabled = false;
					component2.GetComponent<Light>().enabled = true;
					break;
				}
			}
			if (component3 != null)
			{
				switch (iocActive)
				{
				case true:
					component3.GetComponent<Terrain>().enabled = false;
					component3.enabled = true;
					break;
				case false:
					component3.enabled = false;
					component3.GetComponent<Terrain>().enabled = true;
					break;
				}
			}
		}
	}
}
