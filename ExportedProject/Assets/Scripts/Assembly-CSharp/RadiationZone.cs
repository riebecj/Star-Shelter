using System.Collections;
using UnityEngine;

public class RadiationZone : MonoBehaviour
{
	private Transform player;

	public Collider bounds;

	public bool enableRadiation;

	private int damageTick;

	private int puketick;

	public bool inZone;

	internal bool afterBurn;

	internal bool afterGlow;

	private void Start()
	{
		player = GameManager.instance.Head;
	}

	private void TestRadiation()
	{
		inZone = true;
		SuitManager.instance.inRadiationZone = true;
		RadiationUI.instance.UpdateState(true);
		StartCoroutine("UpdateState");
		SuitManager.instance.RadiationWarning(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.transform.root.tag == "Player") || DroneHelper.instance.VRControlled || GameManager.instance.dead)
		{
			return;
		}
		if (enableRadiation)
		{
			if (!SuitManager.instance.inRadiationZone)
			{
				StopCoroutine("AfterBurn");
				afterBurn = false;
				inZone = true;
				SuitManager.instance.inRadiationZone = true;
				RadiationUI.instance.UpdateState(true);
				StartCoroutine("UpdateState");
				if (!GameAudioManager.instance.suitQueue.Contains(SuitManager.instance.radiationSafe))
				{
					SuitManager.instance.RadiationWarning(true);
				}
			}
		}
		else if (SuitManager.instance.inRadiationZone)
		{
			inZone = false;
			StopCoroutine("UpdateState");
			SuitManager.instance.inRadiationZone = false;
			if (!GameManager.instance.dead && !GameAudioManager.instance.suitQueue.Contains(SuitManager.instance.radiationDangerous))
			{
				SuitManager.instance.RadiationWarning(false);
			}
			if (!afterBurn && SuitManager.instance.radiation > 59f)
			{
				StartCoroutine("AfterBurn");
				return;
			}
			RadiationUI.instance.UpdateState(false);
			RadiationUI.instance.damageWarningLabel.SetActive(false);
			afterBurn = false;
		}
	}

	private IEnumerator AfterGlow()
	{
		float refreshRate = 0.3f;
		afterGlow = true;
		while (SuitManager.instance.radiation > 0f && !GameManager.instance.dead)
		{
			SuitManager.instance.radiation -= 0.4f;
			yield return new WaitForSeconds(refreshRate);
		}
		afterGlow = false;
	}

	private IEnumerator AfterBurn()
	{
		float refreshRate = 0.1f;
		afterBurn = true;
		while (SuitManager.instance.radiation > 59f + SuitManager.instance.radiationResistance && !GameManager.instance.dead)
		{
			SuitManager.instance.radiation -= 0.4f;
			if (damageTick > 9)
			{
				DoRadiationDamage();
				damageTick = 0;
			}
			damageTick++;
			yield return new WaitForSeconds(refreshRate);
		}
		RadiationUI.instance.UpdateState(false);
		RadiationUI.instance.damageWarningLabel.SetActive(false);
		afterBurn = false;
	}

	private IEnumerator UpdateState()
	{
		float refreshRate = 0.1f;
		int pukeRNG = Random.Range(30, 100);
		while (SuitManager.instance.inRadiationZone && !GameManager.instance.dead)
		{
			if (SuitManager.instance.radiation < 90f)
			{
				if (SuitManager.instance.radiationResistance < 0.4f)
				{
					SuitManager.instance.radiation += 0.4f / (float)(SuitManager.instance.RadiationResistance + 1);
				}
				else
				{
					SuitManager.instance.radiation += 0.2f / (float)(SuitManager.instance.RadiationResistance + 1);
					if (SuitManager.instance.radiationResistance > 0f)
					{
						SuitManager.instance.radiationResistance -= 0.2f / (float)(SuitManager.instance.RadiationResistance + 1);
					}
				}
			}
			if (SuitManager.instance.radiation > (float)(44 + SuitManager.instance.RadiationResistance * 10))
			{
				if (!ObjectiveRadiation.instance.complete)
				{
					ObjectiveManager.instance.AddObjective(4);
				}
				RadiationUI.instance.damageWarningLabel.SetActive(true);
				if (damageTick > 9)
				{
					DoRadiationDamage();
					damageTick = 0;
				}
				damageTick++;
				puketick++;
				if (puketick > pukeRNG)
				{
					SpaceMask.instance.OnPuke();
					puketick = 0;
				}
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}

	private void DoRadiationDamage()
	{
		SuitManager.instance.OnTakeDamage(1, 7);
	}
}
