using System.Collections;
using UnityEngine;

public class Window : MonoBehaviour
{
	public int health = 3;

	public MeshFilter visual;

	internal bool isRepairing;

	internal GameObject airleakParticle;

	internal GameObject leakPrompt;

	private void Start()
	{
		if (health == 3)
		{
			airleakParticle.SetActive(false);
		}
	}

	public void OnTakeDamage(int value)
	{
		if (value <= health)
		{
			health -= value;
		}
		else
		{
			health = 0;
		}
		leakPrompt.SetActive(true);
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
		RepairTile.transform.SetParent(base.transform);
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
			value += interval * 10f * CraftingManager.instance.craftSpeedMultiplier;
			tileVisual.SetBlendShapeWeight(2, value);
			yield return new WaitForSeconds(interval);
		}
		CraftingManager.instance.repairTiles.Add(RepairTile);
		RepairTile.SetActive(false);
		visual.GetComponent<MeshRenderer>().enabled = true;
		health = 3;
		visual.mesh = BaseManager.instance.meshes[health];
		airleakParticle.SetActive(false);
		GetComponent<Collider>().isTrigger = false;
		isRepairing = false;
		leakPrompt.SetActive(false);
	}
}
