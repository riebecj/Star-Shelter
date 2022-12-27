using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
	public static StatManager instance;

	public Text deathSheet;

	public Text salvageSheet;

	public Text craftSheet;

	internal int playerDeaths;

	internal int deathsBySuffocation;

	internal int deathsByStarvation;

	internal int deathsBySuicide;

	internal int deathsByComet;

	internal int deathsByDrone;

	internal int deathsByExplosion;

	internal int deathsByTurret;

	internal int deathsByRadiation;

	internal int deathsByImpact;

	internal int dronesKilled;

	internal int turretsKilled;

	internal int oxygenCansConsumed;

	internal int powerCansConsumed;

	internal int platesCrafted;

	internal int solarPanelsCrafted;

	internal int shieldsCrafted;

	internal int turretsCrafted;

	internal int airCondensersCrafted;

	internal int powerStationsCrafted;

	internal int plantsPotsCrafted;

	internal int beaconsCrafted;

	internal int solarCellsCrafted;

	internal int batteriesCrafted;

	internal int plasmaCrafted;

	internal int crystalCrafted;

	internal int circuitBoardsCrafted;

	internal int metalSheetsCrafted;

	internal int airCompressorsCrafted;

	internal int suitOxygenUpgraded;

	internal int suitPowerUpgraded;

	internal int thrusterSpeedUpgraded;

	internal int metalSalvaged;

	internal int crystalSalvaged;

	internal int plasmaSalvaged;

	internal int glassSalvaged;

	internal int biomassSalvaged;

	internal int energyCellSalvaged;

	internal int metalSheetSalvaged;

	internal int solarCellSalvaged;

	internal int batterySalvaged;

	internal int circuitBoardSalvaged;

	internal int airCompressorSalvaged;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		OnLoad();
	}

	public void OnDeath(int deathCause)
	{
		switch (deathCause)
		{
		case 0:
			Analytics.CustomEvent("Suffocation");
			deathsBySuffocation++;
			break;
		case 1:
			Analytics.CustomEvent("Starvation");
			deathsByStarvation++;
			break;
		case 2:
			Analytics.CustomEvent("Suicide");
			deathsBySuicide++;
			break;
		case 3:
			Analytics.CustomEvent("Comet");
			deathsByComet++;
			break;
		case 4:
			Analytics.CustomEvent("Drone");
			deathsByDrone++;
			break;
		case 5:
			Analytics.CustomEvent("Explosion");
			deathsByExplosion++;
			break;
		case 6:
			Analytics.CustomEvent("Turret");
			deathsByTurret++;
			break;
		case 7:
			Analytics.CustomEvent("Radiation");
			deathsByRadiation++;
			break;
		case 8:
			Analytics.CustomEvent("Impact");
			deathsByImpact++;
			break;
		}
		playerDeaths++;
		UpdateText();
	}

	public void OnLoad()
	{
		playerDeaths = PlayerPrefs.GetInt("playerDeaths");
		deathsBySuffocation = PlayerPrefs.GetInt("deathsBySuffocation");
		deathsByStarvation = PlayerPrefs.GetInt("deathsByStarvation");
		deathsBySuicide = PlayerPrefs.GetInt("deathsBySuicide");
		deathsByComet = PlayerPrefs.GetInt("deathsByComet");
		deathsByDrone = PlayerPrefs.GetInt("deathsByDrone");
		deathsByExplosion = PlayerPrefs.GetInt("deathsByExplosion");
		deathsByTurret = PlayerPrefs.GetInt("deathsByTurret");
		deathsByRadiation = PlayerPrefs.GetInt("deathsByRadiation");
		deathsByImpact = PlayerPrefs.GetInt("deathsByImpact");
		metalSalvaged = PlayerPrefs.GetInt("metalSalvaged");
		crystalSalvaged = PlayerPrefs.GetInt("crystalSalvaged");
		plasmaSalvaged = PlayerPrefs.GetInt("plasmaSalvaged");
		glassSalvaged = PlayerPrefs.GetInt("glassSalvaged");
		biomassSalvaged = PlayerPrefs.GetInt("biomassSalvaged");
		energyCellSalvaged = PlayerPrefs.GetInt("energyCellSalvaged");
		metalSheetSalvaged = PlayerPrefs.GetInt("metalSheetSalvaged");
		solarCellSalvaged = PlayerPrefs.GetInt("solarCellSalvaged");
		batterySalvaged = PlayerPrefs.GetInt("batterySalvaged");
		circuitBoardSalvaged = PlayerPrefs.GetInt("circuitBoardSalvaged");
		airCompressorSalvaged = PlayerPrefs.GetInt("airCompressorSalvaged");
		platesCrafted = PlayerPrefs.GetInt("platesCrafted");
		solarPanelsCrafted = PlayerPrefs.GetInt("solarPanelsCrafted");
		shieldsCrafted = PlayerPrefs.GetInt("shieldsCrafted");
		turretsCrafted = PlayerPrefs.GetInt("turretsCrafted");
		airCondensersCrafted = PlayerPrefs.GetInt("airCondensersCrafted");
		powerStationsCrafted = PlayerPrefs.GetInt("powerStationsCrafted");
		plantsPotsCrafted = PlayerPrefs.GetInt("plantsPotsCrafted");
		solarCellsCrafted = PlayerPrefs.GetInt("solarCellsCrafted");
		batteriesCrafted = PlayerPrefs.GetInt("batteriesCrafted");
		plasmaCrafted = PlayerPrefs.GetInt("plasmaCrafted");
		crystalCrafted = PlayerPrefs.GetInt("crystalCrafted");
		circuitBoardsCrafted = PlayerPrefs.GetInt("circuitBoardsCrafted");
		metalSheetsCrafted = PlayerPrefs.GetInt("metalSheetsCrafted");
		airCompressorsCrafted = PlayerPrefs.GetInt("airCompressorsCrafted");
		suitOxygenUpgraded = PlayerPrefs.GetInt("suitOxygenUpgraded");
		suitPowerUpgraded = PlayerPrefs.GetInt("suitPowerUpgraded");
		thrusterSpeedUpgraded = PlayerPrefs.GetInt("thrusterSpeedUpgraded");
		UpdateText();
	}

	public void OnSave()
	{
		PlayerPrefs.SetInt("playerDeaths", playerDeaths);
		PlayerPrefs.SetInt("deathsBySuffocatiom", deathsBySuffocation);
		PlayerPrefs.SetInt("deathsByStarvation", deathsByStarvation);
		PlayerPrefs.SetInt("deathsBySuicide", deathsBySuicide);
		PlayerPrefs.SetInt("deathsByComet", deathsByComet);
		PlayerPrefs.SetInt("deathsByDrone", deathsByDrone);
		PlayerPrefs.SetInt("deathsByExplosion", deathsByExplosion);
		PlayerPrefs.SetInt("deathsByTurret", deathsByTurret);
		PlayerPrefs.SetInt("deathsByRadiation", deathsByRadiation);
		PlayerPrefs.SetInt("deathsByImpact", deathsByImpact);
		PlayerPrefs.SetInt("metalSalvaged", metalSalvaged);
		PlayerPrefs.SetInt("crystalSalvaged", crystalSalvaged);
		PlayerPrefs.SetInt("plasmaSalvaged", plasmaSalvaged);
		PlayerPrefs.SetInt("glassSalvaged", glassSalvaged);
		PlayerPrefs.SetInt("biomassSalvaged", biomassSalvaged);
		PlayerPrefs.SetInt("energyCellSalvaged", energyCellSalvaged);
		PlayerPrefs.SetInt("metalSheetSalvaged", metalSheetSalvaged);
		PlayerPrefs.SetInt("solarCellSalvaged", solarCellSalvaged);
		PlayerPrefs.SetInt("batterySalvaged", batterySalvaged);
		PlayerPrefs.SetInt("circuitBoardSalvaged", circuitBoardSalvaged);
		PlayerPrefs.SetInt("airCompressorSalvaged", airCompressorSalvaged);
		PlayerPrefs.SetInt("platesCrafted", platesCrafted);
		PlayerPrefs.SetInt("solarPanelsCrafted", solarPanelsCrafted);
		PlayerPrefs.SetInt("shieldsCrafted", shieldsCrafted);
		PlayerPrefs.SetInt("turretsCrafted", turretsCrafted);
		PlayerPrefs.SetInt("airCondensersCrafted", airCondensersCrafted);
		PlayerPrefs.SetInt("powerStationsCrafted", powerStationsCrafted);
		PlayerPrefs.SetInt("plantsPotsCrafted", plantsPotsCrafted);
		PlayerPrefs.SetInt("solarCellsCrafted", solarCellsCrafted);
		PlayerPrefs.SetInt("batteriesCrafted", batteriesCrafted);
		PlayerPrefs.SetInt("plasmaCrafted", plasmaCrafted);
		PlayerPrefs.SetInt("crystalCrafted", crystalCrafted);
		PlayerPrefs.SetInt("circuitBoardsCrafted", circuitBoardsCrafted);
		PlayerPrefs.SetInt("metalSheetsCrafted", metalSheetsCrafted);
		PlayerPrefs.SetInt("airCompressorsCrafted", airCompressorsCrafted);
		PlayerPrefs.SetInt("suitOxygenUpgraded", suitOxygenUpgraded);
		PlayerPrefs.SetInt("suitPowerUpgraded", suitPowerUpgraded);
		PlayerPrefs.SetInt("thrusterSpeedUpgraded", thrusterSpeedUpgraded);
	}

	public void UpdateText()
	{
		if (!IntroManager.instance)
		{
			deathSheet.text = "FATALITIES";
			Text text = deathSheet;
			text.text = text.text + "\n Total Deaths: " + playerDeaths;
			Text text2 = deathSheet;
			text2.text = text2.text + "\n Suffocation: " + deathsBySuffocation;
			Text text3 = deathSheet;
			text3.text = text3.text + "\n Starvation: " + deathsByStarvation;
			Text text4 = deathSheet;
			text4.text = text4.text + "\n Suicide: " + deathsBySuicide;
			Text text5 = deathSheet;
			text5.text = text5.text + "\n Comet: " + deathsByComet;
			Text text6 = deathSheet;
			text6.text = text6.text + "\n Drone: " + deathsByDrone;
			Text text7 = deathSheet;
			text7.text = text7.text + "\n Explosion: " + deathsByExplosion;
			Text text8 = deathSheet;
			text8.text = text8.text + "\n Turret: " + deathsByTurret;
			Text text9 = deathSheet;
			text9.text = text9.text + "\n Radiation: " + deathsByRadiation;
			Text text10 = deathSheet;
			text10.text = text10.text + "\n Impact: " + deathsByImpact;
			salvageSheet.text = "SALVAGED";
			Text text11 = salvageSheet;
			text11.text = text11.text + "\n Metal: " + metalSalvaged;
			Text text12 = salvageSheet;
			text12.text = text12.text + "\n Crystals: " + crystalSalvaged;
			Text text13 = salvageSheet;
			text13.text = text13.text + "\n Plasma: " + plasmaSalvaged;
			Text text14 = salvageSheet;
			text14.text = text14.text + "\n Glass: " + glassSalvaged;
			Text text15 = salvageSheet;
			text15.text = text15.text + "\n BioMass: " + biomassSalvaged;
			Text text16 = salvageSheet;
			text16.text = text16.text + "\n EnergyCells: " + energyCellSalvaged;
			Text text17 = salvageSheet;
			text17.text = text17.text + "\n MetalSheets: " + metalSheetSalvaged;
			Text text18 = salvageSheet;
			text18.text = text18.text + "\n SolarCells: " + solarCellSalvaged;
			Text text19 = salvageSheet;
			text19.text = text19.text + "\n Batteries: " + batterySalvaged;
			Text text20 = salvageSheet;
			text20.text = text20.text + "\n CircuitBoards: " + circuitBoardSalvaged;
			Text text21 = salvageSheet;
			text21.text = text21.text + "\n AirCompressors: " + airCompressorSalvaged;
			craftSheet.text = "CRAFTED";
			Text text22 = craftSheet;
			text22.text = text22.text + "\n Plates: " + platesCrafted;
			Text text23 = craftSheet;
			text23.text = text23.text + "\n Solar Panels: " + solarPanelsCrafted;
			Text text24 = craftSheet;
			text24.text = text24.text + "\n Shields: " + shieldsCrafted;
			Text text25 = craftSheet;
			text25.text = text25.text + "\n Turrets: " + turretsCrafted;
			Text text26 = craftSheet;
			text26.text = text26.text + "\n AirCondesers: " + airCondensersCrafted;
			Text text27 = craftSheet;
			text27.text = text27.text + "\n PowerStations: " + powerStationsCrafted;
			Text text28 = craftSheet;
			text28.text = text28.text + "\n PlantsPots: " + plantsPotsCrafted;
			Text text29 = craftSheet;
			text29.text = text29.text + "\n SolarCells: " + solarCellsCrafted;
			Text text30 = craftSheet;
			text30.text = text30.text + "\n Batteries: " + batteriesCrafted;
			Text text31 = craftSheet;
			text31.text = text31.text + "\n Plasma: " + plasmaCrafted;
			Text text32 = craftSheet;
			text32.text = text32.text + "\n Crystals: " + crystalCrafted;
			Text text33 = craftSheet;
			text33.text = text33.text + "\n CircuitBoards: " + circuitBoardsCrafted;
			Text text34 = craftSheet;
			text34.text = text34.text + "\n MetalSheets: " + metalSheetsCrafted;
			Text text35 = craftSheet;
			text35.text = text35.text + "\n AirCompressors: " + airCompressorsCrafted;
			Text text36 = craftSheet;
			text36.text = text36.text + "\n OxygenUpgrades: " + suitOxygenUpgraded;
			Text text37 = craftSheet;
			text37.text = text37.text + "\n SuitPowerUpgrades: " + suitPowerUpgraded;
			Text text38 = craftSheet;
			text38.text = text38.text + "\n ThrusterSpeedUpgrades: " + thrusterSpeedUpgraded;
		}
	}
}
