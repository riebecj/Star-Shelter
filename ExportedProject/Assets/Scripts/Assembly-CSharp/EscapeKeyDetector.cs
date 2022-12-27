using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeKeyDetector : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (SceneManager.GetActiveScene().buildIndex == 0)
			{
				Application.Quit();
			}
			else
			{
				SceneManager.LoadScene(0);
			}
		}
	}
}
