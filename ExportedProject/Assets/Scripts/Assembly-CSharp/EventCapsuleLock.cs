using System.Collections;
using UnityEngine;
using VRTK;

public class EventCapsuleLock : MonoBehaviour
{
	public GameObject button;

	public Transform headPos;

	internal bool locked;

	private void OnTriggerEnter(Collider other)
	{
		if (!locked && other.transform.root.tag == "Player")
		{
			OnLock();
			if ((bool)other.transform.root.GetComponent<Rigidbody>())
			{
				other.transform.root.GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}

	private void OnLock()
	{
		locked = true;
		foreach (Thruster thruster in Thruster.thrusters)
		{
			thruster.deactivated = true;
		}
		VRTK_PlayerClimb.instance.disabled = true;
		button.SetActive(true);
		StartCoroutine("MovetoGoal");
	}

	private IEnumerator MovetoGoal()
	{
		int count = 0;
		while (count < 100)
		{
			count++;
			Vector3 relativePos = GameManager.instance.Head.position - GameManager.instance.CamRig.position;
			GameManager.instance.CamRig.position = Vector3.Lerp(GameManager.instance.CamRig.position, headPos.position - relativePos, Time.deltaTime * 8f);
			yield return new WaitForSeconds(0.02f);
		}
	}
}
