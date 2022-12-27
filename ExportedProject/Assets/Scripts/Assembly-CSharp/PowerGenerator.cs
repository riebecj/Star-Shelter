using System.Collections.Generic;
using UnityEngine;

public class PowerGenerator : MonoBehaviour
{
	public delegate void TogglePower(bool _on);

	public static List<PowerGenerator> powerGenerators = new List<PowerGenerator>();

	public TogglePower togglePower;

	public static int generatorDistance = 40;

	private void Awake()
	{
		powerGenerators.Add(this);
	}

	private void TurnOff()
	{
		OnTogglePower(false);
	}

	private void TurnOn()
	{
		OnTogglePower(true);
	}

	public void OnTogglePower(bool _on)
	{
		togglePower(_on);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, generatorDistance);
	}

	private void OnDestroy()
	{
		powerGenerators.Remove(this);
	}
}
