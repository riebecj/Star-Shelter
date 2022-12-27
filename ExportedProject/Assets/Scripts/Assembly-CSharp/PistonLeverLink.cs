using UnityEngine;

public class PistonLeverLink : MonoBehaviour
{
	public VRLever Lever;

	private Piston CurrentPiston;

	private void OnEnable()
	{
		CurrentPiston = GetComponent<Piston>();
	}

	public void LeverValueChange(VRLever _lever, float _newValue, float _oldValue)
	{
		CurrentPiston.Value = _newValue;
	}

	public void LeverValueChange(VRLever _lever)
	{
		CurrentPiston.Value = _lever.Value;
	}

	private void OnDisable()
	{
	}
}
