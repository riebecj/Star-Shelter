using System.Collections;
using UnityEngine;

public class RadiationUI : MonoBehaviour
{
	public Transform pointer;

	public Transform limit;

	public GameObject damageWarningLabel;

	public static RadiationUI instance;

	public Animator anim;

	private void Awake()
	{
		instance = this;
	}

	private IEnumerator UpdatePointer()
	{
		limit.localEulerAngles = new Vector3(0f, 0f, -(45 + SuitManager.instance.RadiationResistance * 10));
		while (true)
		{
			pointer.localEulerAngles = new Vector3(0f, 0f, 0f - SuitManager.instance.radiation);
			yield return new WaitForSeconds(0.25f);
		}
	}

	public void UpdateState(bool active)
	{
		if (active)
		{
			anim.SetBool("On", true);
			SpaceMask.instance.radiationUI.SetBool("FadeIn", true);
			StartCoroutine("UpdatePointer");
		}
		else
		{
			anim.SetBool("On", false);
			SpaceMask.instance.radiationUI.SetBool("FadeIn", false);
			StopCoroutine("UpdatePointer");
		}
	}
}
