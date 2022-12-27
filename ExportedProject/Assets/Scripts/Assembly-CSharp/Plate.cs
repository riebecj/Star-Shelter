using System.Collections;
using UnityEngine;

public class Plate : MonoBehaviour
{
	public int health;

	public MeshFilter visual;

	internal bool isRepairing;

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
		if (health == 0)
		{
			base.gameObject.SetActive(false);
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

	private void OnBreak()
	{
		base.gameObject.SetActive(false);
	}
}
