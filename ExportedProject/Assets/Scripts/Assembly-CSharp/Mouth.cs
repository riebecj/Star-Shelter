using UnityEngine;

public class Mouth : MonoBehaviour
{
	private SphereCollider collider;

	private Food lastFood;

	public AudioClip eatAudio;

	private void Start()
	{
		Invoke("AddColldier", 0.5f);
	}

	private void AddColldier()
	{
		collider = base.gameObject.AddComponent<SphereCollider>();
		collider.radius = 0.1f;
		collider.center = new Vector3(0f, 0f, -0.03f);
		collider.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<Food>() && other.GetComponent<Food>().value > 0 && !IsInvoking("Eat") && SpaceMask.instance.open)
		{
			lastFood = other.GetComponent<Food>();
			Invoke("Eat", 0.1f);
		}
	}

	private void Eat()
	{
		if (lastFood.value > 0)
		{
			if (lastFood.value > 0)
			{
				SuitManager.instance.nutrition = Mathf.Clamp(SuitManager.instance.nutrition + lastFood.nutritionValue / 2f, 0f, SuitManager.instance.maxNutrition);
				SuitManager.instance.health = Mathf.Clamp(SuitManager.instance.health + lastFood.healingValue / 2f, 0f, SuitManager.instance.maxHealth);
				lastFood.OnEat();
				SuitManager.instance.chokeAudio.PlayOneShot(eatAudio);
				TutorialManager.instance.OnEatComplete();
			}
			Invoke("Eat", 1.5f);
		}
	}
}
