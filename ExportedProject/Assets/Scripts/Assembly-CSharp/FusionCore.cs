using UnityEngine;
using VRTK;

public class FusionCore : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	private TitanAI titan;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		titan = GetComponentInParent<TitanAI>();
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		interact.previousParent = null;
		interact.previousKinematicState = false;
		if ((bool)titan && !titan.dead)
		{
			GameManager.instance.looseCores++;
			titan.OnDeath();
		}
	}
}
