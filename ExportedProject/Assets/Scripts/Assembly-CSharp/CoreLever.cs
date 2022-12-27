using System.Collections;
using UnityEngine;
using VRTK;

public class CoreLever : MonoBehaviour
{
	public int index;

	public Transform ref1;

	public Transform ref2;

	private VRTK_InteractableObject interact;

	public JumpShip jumpShip;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		StartCoroutine("UpdateState");
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		StopCoroutine("UpdateState");
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			Vector3 _ref = ref1.position - ref2.position;
			float powerValue = Vector3.SignedAngle(_ref, base.transform.forward, Vector3.up);
			jumpShip.RegulateCorePower(index, powerValue / 3f);
			yield return new WaitForSeconds(0.02f);
		}
	}
}
