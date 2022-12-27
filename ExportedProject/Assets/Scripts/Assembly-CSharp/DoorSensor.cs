using System.Collections;
using UnityEngine;

public class DoorSensor : MonoBehaviour
{
	public float radius = 2.5f;

	private Transform player;

	public Animator animator;

	internal bool active = true;

	public GameObject open;

	public GameObject close;

	private void Start()
	{
		player = GameManager.instance.Head;
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
		while (active)
		{
			if (Vector3.Distance(base.transform.position, player.position) < radius || Vector3.Distance(base.transform.position, DroneHelper.instance.transform.position) < radius)
			{
				Open();
			}
			else
			{
				Close();
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void Open()
	{
		animator.SetBool("Open", true);
	}

	private void Close()
	{
		animator.SetBool("Open", false);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
