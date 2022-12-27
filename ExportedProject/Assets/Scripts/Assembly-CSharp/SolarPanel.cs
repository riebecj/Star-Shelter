using System.Collections;
using PreviewLabs;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
	public float powerDraw = 0.25f;

	public int health;

	public MeshFilter visual;

	internal bool isRepairing;

	internal bool broken;

	public Mesh brokenMesh;

	private void Start()
	{
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
		if (health == 0 && !broken)
		{
			OnBreak();
		}
	}

	public void OnRepair()
	{
		isRepairing = true;
		StartCoroutine("Repair");
	}

	private IEnumerator Repair()
	{
		float value = 0f;
		float interval = 0.05f;
		GameObject RepairTile = CraftingManager.instance.repairTiles[0];
		RepairTile.SetActive(true);
		visual.GetComponent<MeshRenderer>().enabled = false;
		RepairTile.transform.position = base.transform.position;
		RepairTile.transform.rotation = base.transform.rotation;
		SkinnedMeshRenderer tileVisual = RepairTile.GetComponent<SkinnedMeshRenderer>();
		CraftingManager.instance.repairTiles.Remove(RepairTile);
		if (health == 0)
		{
			value = 0f;
		}
		else if (health == 1)
		{
			value = 33f;
		}
		else if (health == 2)
		{
			value = 66f;
		}
		while (value < 95f)
		{
			value += interval * 10f;
			tileVisual.SetBlendShapeWeight(2, value);
			yield return new WaitForSeconds(interval);
		}
		CraftingManager.instance.repairTiles.Add(RepairTile);
		RepairTile.SetActive(false);
		visual.GetComponent<MeshRenderer>().enabled = true;
		health = 3;
		visual.mesh = BaseManager.instance.meshes[health];
		isRepairing = false;
	}

	private void OnEnable()
	{
		if (BaseManager.instance == null)
		{
			Invoke("OnEnable", 0.1f);
		}
		else if (!BaseManager.instance.SolarPanels.Contains(this))
		{
			BaseManager.instance.SolarPanels.Add(this);
		}
	}

	private void OnDisable()
	{
		if (BaseManager.instance.SolarPanels.Contains(this))
		{
			BaseManager.instance.SolarPanels.Remove(this);
		}
	}

	private void OnBreak()
	{
		GetComponent<MeshFilter>().mesh = brokenMesh;
		broken = true;
		if (BaseManager.instance.SolarPanels.Contains(this))
		{
			BaseManager.instance.SolarPanels.Remove(this);
		}
		if (!GameManager.instance.loading)
		{
			Object.Instantiate(GameManager.instance.breakPraticle, base.transform.position, base.transform.rotation);
			GameAudioManager.instance.AddToSuitQueue(BaseManager.instance.solarPanelBrokenWarning);
			PreviewLabs.PlayerPrefs.SetBool(string.Concat(base.name, base.transform.position, "Broken"), true);
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
