using System.Collections;
using UnityEngine;

public class GateSensor : MonoBehaviour
{
	public float radius = 2.5f;

	private Transform player;

	public Animator animator;

	public bool active = true;

	public GameObject open;

	public GameObject close;

	private Gate gate;

	private void Start()
	{
		player = GameManager.instance.Head;
		gate = GetComponent<Gate>();
		StartCoroutine("CheckStatus");
	}

	public void SetLock(bool state)
	{
		if (state)
		{
			active = true;
			open.SetActive(true);
			close.SetActive(false);
			if (player == null)
			{
				player = GameManager.instance.Head;
			}
			StartCoroutine("CheckStatus");
		}
		else
		{
			active = false;
			open.SetActive(false);
			close.SetActive(true);
			StopCoroutine("CheckStatus");
		}
	}

	private IEnumerator CheckStatus()
	{
		while (true)
		{
			if (active && gate.automatic)
			{
				if (CloseToPlayerEntity())
				{
					Open();
				}
				else
				{
					Close();
				}
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void Open()
	{
		gate.OnOpen();
		animator.SetBool("Open", true);
	}

	private void Close()
	{
		gate.OnClose();
		animator.SetBool("Open", false);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}

	private bool CloseToPlayerEntity()
	{
		if (Vector3.Distance(base.transform.position, player.position) < radius)
		{
			return true;
		}
		if ((bool)DroneHelper.instance && Vector3.Distance(base.transform.position, DroneHelper.instance.transform.position) < radius)
		{
			return true;
		}
		return false;
	}
}
