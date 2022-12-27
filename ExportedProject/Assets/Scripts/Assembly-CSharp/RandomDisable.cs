using UnityEngine;

public class RandomDisable : MonoBehaviour
{
	public int chanceToDisable = 50;

	private void Start()
	{
		if (Random.Range(0, 100) < chanceToDisable)
		{
			base.gameObject.SetActive(false);
		}
	}
}
