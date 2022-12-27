using PreviewLabs;
using UnityEngine;

public class HoloShield : MonoBehaviour
{
	public float powerDraw = 0.15f;

	public int health;

	public int startPowerCost = 5;

	private int startHeath;

	public GameObject shield;

	public GameObject powerIcon;

	internal bool isRepairing;

	internal ShieldButton shieldButton;

	internal bool craftComplete;

	internal bool broken;

	public bool active;

	public Material[] _materials;

	public Mesh brokenMesh;

	private void Start()
	{
		startHeath = health;
		if (GameManager.instance.debugMode || !GetComponent<CraftComponent>().constructing)
		{
			CraftingComplete();
		}
		if (PreviewLabs.PlayerPrefs.GetBool(string.Concat(base.name, base.transform.position, "Broken")))
		{
			OnBreak();
		}
	}

	public void OnTakeDamage(int value)
	{
		TakeDamage(value);
	}

	public void TakeDamage(int value)
	{
		if (value <= health)
		{
			health -= value;
		}
		else
		{
			health = 0;
		}
		if (health < 2)
		{
			shield.SetActive(true);
		}
		if (health == 0)
		{
			OnBreak();
		}
		UpdateTexture();
	}

	private void UpdateTexture()
	{
		shield.GetComponent<MeshRenderer>().material = _materials[Mathf.Clamp(health - 1, 0, 4)];
	}

	private void OnEnable()
	{
		if (BaseManager.instance == null)
		{
			Invoke("OnEnable", 0.1f);
		}
		else if (!BaseManager.instance.HoloShields.Contains(this))
		{
			BaseManager.instance.HoloShields.Add(this);
		}
	}

	private void OnDisable()
	{
		if (BaseManager.instance.HoloShields.Contains(this))
		{
			BaseManager.instance.HoloShields.Remove(this);
		}
	}

	public void OnRepair()
	{
		health = startHeath;
		UpdateTexture();
	}

	public void OnRestore()
	{
		shield.SetActive(true);
		health = startHeath;
		UpdateTexture();
	}

	public void CraftingComplete()
	{
		if (BaseManager.instance.power > (float)startPowerCost)
		{
			BaseManager.instance.power -= startPowerCost;
			OnRestore();
			GetComponent<MeshRenderer>().enabled = true;
		}
		for (int i = 0; i < HoloMap.holoMaps.Count; i++)
		{
			HoloMap.holoMaps[i].AddShield(this);
		}
		craftComplete = true;
		active = true;
	}

	public void OnActivate()
	{
		if (!broken)
		{
			shield.SetActive(true);
			powerIcon.SetActive(false);
			if ((bool)shieldButton)
			{
				shieldButton.ToggleOn();
			}
			active = true;
		}
	}

	public void OnDeactivate()
	{
		shield.SetActive(false);
		powerIcon.SetActive(true);
		if ((bool)shieldButton)
		{
			shieldButton.ToggleOff();
		}
		active = false;
	}

	private void OnBreak()
	{
		GetComponent<MeshFilter>().mesh = brokenMesh;
		broken = true;
		active = false;
		shield.SetActive(false);
		if (BaseManager.instance.HoloShields.Contains(this))
		{
			BaseManager.instance.HoloShields.Remove(this);
		}
		if (!GameManager.instance.loading)
		{
			Object.Instantiate(GameManager.instance.breakPraticle, base.transform.position, base.transform.rotation);
			GameAudioManager.instance.AddToSuitQueue(BaseManager.instance.holoShieldBrokenWarning);
			PreviewLabs.PlayerPrefs.SetBool(string.Concat(base.name, base.transform.position, "Broken"), true);
		}
	}

	public void OnSignal()
	{
		if (craftComplete)
		{
			if (shield.activeSelf)
			{
				OnDeactivate();
			}
			else
			{
				OnActivate();
			}
		}
	}

	public void OnSalvage()
	{
		OnRemove();
	}

	public void OnRemove()
	{
		PreviewLabs.PlayerPrefs.SetBool(string.Concat(base.name, base.transform.position, "Broken"), false);
	}
}
