using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MedBay : MonoBehaviour
{
	public Animator anim;

	public Text powerCostInfo;

	internal bool healing;

	private void Start()
	{
		StartCoroutine("UpdateCost");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.tag == "Player" && !GameManager.instance.dead && SuitManager.instance.health < SuitManager.instance.maxHealth && !healing)
		{
			if (BaseManager.instance.power > 4f)
			{
				StartHealing();
			}
			else
			{
				SuitManager.instance.LowPowerPrompt();
			}
		}
	}

	private void StartHealing()
	{
		healing = true;
		anim.SetTrigger("Heal");
		StartCoroutine("Healing");
		CancelInvoke("EndProcess");
		Invoke("EndProcess", 10f);
	}

	private IEnumerator Healing()
	{
		while (BaseManager.instance.power > 4f && SuitManager.instance.health < SuitManager.instance.maxHealth)
		{
			float distance = Vector3.Distance(base.transform.position, GameManager.instance.Head.position);
			if (SuitManager.instance.health < SuitManager.instance.maxHealth && distance < 1.5f)
			{
				SuitManager.instance.health += 4f;
				BaseManager.instance.power -= 4f;
				if (SuitManager.instance.health > SuitManager.instance.maxHealth)
				{
					SuitManager.instance.health = SuitManager.instance.maxHealth;
				}
			}
			yield return new WaitForSeconds(0.25f);
		}
		anim.SetTrigger("Stop");
		healing = false;
	}

	private IEnumerator UpdateCost()
	{
		while (true)
		{
			if (SuitManager.instance.health < SuitManager.instance.maxHealth)
			{
				powerCostInfo.text = (SuitManager.instance.maxHealth - SuitManager.instance.health).ToString("F0");
			}
			else
			{
				powerCostInfo.text = "Full Heath";
			}
			yield return new WaitForSeconds(5f);
		}
	}

	private void EndProcess()
	{
		StopCoroutine("Healing");
		anim.SetTrigger("Stop");
		healing = false;
	}
}
