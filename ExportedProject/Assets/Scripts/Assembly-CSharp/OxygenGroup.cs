using System.Collections.Generic;
using UnityEngine;

public class OxygenGroup : ScriptableObject
{
	public float MaxOxygen;

	public float TotalOxygen;

	public List<Room> Rooms = new List<Room>();

	internal float drainRate;

	public void Setup()
	{
		foreach (Room room in Rooms)
		{
			MaxOxygen += room.OxgenCapacity;
			TotalOxygen += room.Oxygen;
			room.group = this;
		}
		UpdateRoomStates(1f);
	}

	public void UpdateRoomStates(float refreshRate)
	{
		if (BaseLoader.instance.isLoading)
		{
			return;
		}
		TotalOxygen = 0f;
		foreach (Room room in Rooms)
		{
			TotalOxygen += room.Oxygen;
		}
		if (BaseManager.instance.currentOxygenGroup == this && BaseManager.instance.autoFillOxygen && SuitManager.instance.oxygen < SuitManager.instance.maxOxygen)
		{
			BaseManager.instance.OnAutofillSuit(refreshRate, this);
			if (SuitManager.instance.oxygen < SuitManager.instance.maxOxygen - 3f)
			{
				drainRate = BaseManager.instance.oxygenChargeSpeed;
			}
			else
			{
				drainRate = 0.3f;
			}
		}
		else
		{
			drainRate = 0f;
		}
		float num = TotalOxygen / MaxOxygen;
		if (num > 1f)
		{
			num = 1f;
		}
		foreach (Room room2 in Rooms)
		{
			room2.Oxygen = room2.OxgenCapacity * num;
			room2.UpdateState(refreshRate);
		}
	}
}
