using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Seed : MonoBehaviour
{
	public enum SeedType
	{
		Grape = 0,
		Apple = 1,
		Orange = 2,
		Pear = 3,
		Oxygen = 4,
		Fruit = 5
	}

	public SeedType seedType;

	public GameObject infoCanvas;

	internal VRTK_InteractableObject interact;

	internal int value;

	internal static string[] seedInfo = new string[5] { "Grape: Nutrition: 3, Healing: 8", "Apple: Nutrition: 8, Healing: 3", "Orange: Nutrition: 5, Healing: 5", "Pear: Nutrition: 10, Healing: 0", "0.25/sec" };

	public Text[] fruitInfo;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
		if (seedType == SeedType.Fruit)
		{
			Invoke("SetType", 0.25f);
		}
		else
		{
			SetLabel();
		}
	}

	private void SetType()
	{
		value = Random.Range(0, 4);
		OnSetType();
	}

	public void SetType(int _value)
	{
		value = _value;
		OnSetType();
	}

	private void OnSetType()
	{
		if (value == 0)
		{
			seedType = SeedType.Grape;
			base.name = "GrapeSeedCapsule";
		}
		else if (value == 1)
		{
			seedType = SeedType.Apple;
			base.name = "AppleSeedCapsule";
		}
		else if (value == 2)
		{
			seedType = SeedType.Orange;
			base.name = "OrangeSeedCapsule";
		}
		else if (value == 3)
		{
			seedType = SeedType.Pear;
			base.name = "PearSeedCapsule";
		}
		SetLabel();
	}

	private void SetLabel()
	{
		string text = string.Empty;
		if (seedType == SeedType.Grape)
		{
			text = "Grape Seed";
		}
		else if (seedType == SeedType.Apple)
		{
			text = "Apple Seed";
		}
		else if (seedType == SeedType.Orange)
		{
			text = "Orange Seed";
		}
		else if (seedType == SeedType.Pear)
		{
			text = "Pear Seed";
		}
		if (text != string.Empty)
		{
			Text[] array = fruitInfo;
			foreach (Text text2 in array)
			{
				text2.text = text;
			}
		}
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		infoCanvas.SetActive(true);
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		infoCanvas.SetActive(false);
	}
}
