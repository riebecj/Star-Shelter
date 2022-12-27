using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
	public int plantValue = 1;

	public int reseachValue = 1;

	public int suitUpgradeValue = 2;

	public int defenseValue = 2;

	public int gunModValue = 2;

	public int generatorValue = 1;

	private int Score;

	public static DifficultyManager instance;

	private void Awake()
	{
		instance = this;
	}

	public int GetScore()
	{
		Score = 0;
		for (int i = 0; i < BaseLoader.instance.AllRooms.Count; i++)
		{
			for (int j = 0; j < BaseLoader.instance.AllRooms[i].oxygenPlants.Count; j++)
			{
				Score += plantValue;
			}
			for (int k = 0; k < BaseLoader.instance.AllRooms[i].fruitPlants.Count; k++)
			{
				Score += plantValue;
			}
		}
		Score += UpgradeManager.InventoryCapacity * reseachValue;
		Score += UpgradeManager.ConstructionSpeed * reseachValue;
		Score += UpgradeManager.SalvageSpeed * reseachValue;
		Score += UpgradeManager.AmmoCapacity * reseachValue;
		Score += UpgradeManager.RadiationResistance * reseachValue;
		Score += UpgradeManager.NanoStorageCapacity * reseachValue;
		Score += UpgradeManager.SuitNanoCapacity * reseachValue;
		Score += SuitManager.instance.PowerCapacity * suitUpgradeValue;
		Score += SuitManager.instance.OxygenCapacity * suitUpgradeValue;
		Score += SuitManager.instance.ThrusterSpeed * suitUpgradeValue;
		for (int l = 0; l < BaseManager.instance.SolarPanels.Count; l++)
		{
			Score += defenseValue;
		}
		for (int m = 0; m < BaseManager.instance.Turrets.Count; m++)
		{
			Score += defenseValue;
		}
		for (int n = 0; n < BaseManager.instance.HoloShields.Count; n++)
		{
			Score += defenseValue;
		}
		for (int num = 0; num < PowerStation.powerStations.Count; num++)
		{
			Score += generatorValue;
		}
		UpdateSpawnRates();
		Score = Mathf.Clamp(Score, 0, 100);
		return Score;
	}

	private void UpdateSpawnRates()
	{
		LootSpawnManager.instance.Tier2DropChance[25] = 7 - Mathf.Clamp(Score / 8, 0, 5);
		LootSpawnManager.instance.Tier1DropChance[2] = 15 - Mathf.Clamp(Score / 8, 0, 13);
		LootSpawnManager.instance.Tier1DropChance[3] = 15 - Mathf.Clamp(Score / 8, 0, 13);
		LootSpawnManager.instance.Tier1DropChance[4] = 7 - Mathf.Clamp(Score / 8, 0, 5);
		LootSpawnManager.instance.Tier1DropChance[5] = 7 - Mathf.Clamp(Score / 8, 0, 5);
		LootSpawnManager.instance.Tier1DropChance[8] = 15 - Mathf.Clamp(Score / 8, 0, 13);
		LootSpawnManager.instance.Tier1DropChance[9] = 15 - Mathf.Clamp(Score / 8, 0, 13);
	}
}
