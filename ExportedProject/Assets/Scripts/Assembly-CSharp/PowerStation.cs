using System.Collections.Generic;
using UnityEngine;

public class PowerStation : MonoBehaviour
{
	public static List<PowerStation> powerStations = new List<PowerStation>();

	public int powerIncrease = 20;

	private void Awake()
	{
		powerStations.Add(this);
	}

	private void Start()
	{
		BaseManager.instance.maxPower += powerIncrease;
	}

	private void OnBreak()
	{
		base.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		BaseManager.instance.maxPower += powerIncrease;
	}
}
