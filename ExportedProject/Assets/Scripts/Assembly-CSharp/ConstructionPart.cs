using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionPart : MonoBehaviour
{
	public float craftTime;

	internal float startTime;

	internal bool constructing;

	internal List<MeshRenderer> visuals = new List<MeshRenderer>();

	internal List<Material> storedMaterials = new List<Material>();

	private void Start()
	{
		StartCoroutine("Constructing");
	}

	public IEnumerator Constructing()
	{
		constructing = true;
		if ((bool)GetComponent<Recipe>())
		{
			craftTime = GetComponent<Recipe>().craftTime;
		}
		startTime = craftTime;
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		Debug.Log("Renderers " + renderers.Length);
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = true;
			storedMaterials.Add(meshRenderer.material);
			meshRenderer.material = GameManager.instance.craftMaterial;
			visuals.Add(meshRenderer);
		}
		foreach (MeshRenderer visual in visuals)
		{
			visual.material.EnableKeyword("_Cutoff");
			visual.material.SetFloat("_Cutoff", 0.75f);
		}
		float waitTime = 0.05f;
		while (craftTime > 0f)
		{
			craftTime -= waitTime;
			float value = 0.25f + (startTime - craftTime) / startTime / 2f;
			foreach (MeshRenderer visual2 in visuals)
			{
				visual2.material.SetFloat("_Cutoff", 1f - value);
			}
			yield return new WaitForSeconds(waitTime);
		}
		for (int j = 0; j < visuals.Count; j++)
		{
			visuals[j].material = storedMaterials[j];
		}
		constructing = false;
		base.gameObject.SendMessage("CraftingComplete", SendMessageOptions.DontRequireReceiver);
	}
}
