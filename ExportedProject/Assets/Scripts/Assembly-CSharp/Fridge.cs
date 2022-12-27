using UnityEngine;

public class Fridge : MonoBehaviour
{
	public GameObject[] foodItems;

	public Transform spawnPos;

	public int[] foodItemCount;

	public GameObject emptyLabel;

	public GameObject buttons;

	public int foodMin;

	public int foodMax;

	public AudioClip beep;

	private AudioSource audioSource;

	private void Start()
	{
		RandomizeFood();
		audioSource = GetComponent<AudioSource>();
	}

	private void RandomizeFood()
	{
		for (int i = 0; i < foodItemCount.Length; i++)
		{
			foodItemCount[i] = Random.Range(foodMin, foodMax);
		}
	}

	public void SpawnFood(int i)
	{
		if (foodItemCount[i] > 0)
		{
			audioSource.PlayOneShot(beep);
			GameObject gameObject = Object.Instantiate(foodItems[i], spawnPos.position, spawnPos.rotation);
			gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * -10f);
			foodItemCount[i]--;
		}
		else
		{
			buttons.SetActive(false);
			emptyLabel.SetActive(true);
			Invoke("Reset", 2f);
		}
	}

	private void Reset()
	{
		buttons.SetActive(true);
		emptyLabel.SetActive(false);
	}
}
