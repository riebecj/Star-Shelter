using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class FileBox : MonoBehaviour
{
	internal bool sorted;

	internal int value;

	public int type;

	public Image symbol;

	public Image symbolColor;

	internal List<Transform> files = new List<Transform>();

	internal FileHackA source;

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<ResearchFile>())
		{
			if (other.GetComponent<ResearchFile>().type == type)
			{
				other.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
				other.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
				GetComponentInParent<FileHackA>().OnFileSorted();
				sorted = true;
				files.Add(other.transform);
				StartCoroutine("LerpToCenter");
			}
			else
			{
				other.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
				source.FailAudio();
				source.Invoke("Shuffle", 0.1f);
			}
		}
	}

	private IEnumerator LerpToCenter()
	{
		while (true)
		{
			foreach (Transform file in files)
			{
				file.transform.position = Vector3.Lerp(file.transform.position, base.transform.position, 8f * Time.deltaTime);
			}
			yield return new WaitForSeconds(0.02f);
		}
	}

	public void SetSymbol(Sprite newSymbol, int newType)
	{
		symbol.sprite = newSymbol;
		symbolColor.sprite = newSymbol;
		type = newType;
	}
}
