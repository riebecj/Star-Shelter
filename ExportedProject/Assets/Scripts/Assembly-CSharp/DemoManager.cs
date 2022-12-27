using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
	public GameObject demoPromt;

	private Text info;

	private int CountDown = 30;

	private void Start()
	{
		info = demoPromt.GetComponent<Text>();
		if (GameManager.instance.DemoBuild)
		{
			Invoke("StartCountDown", 600f);
		}
	}

	private void StartCountDown()
	{
		StartCoroutine("Countdown");
	}

	private IEnumerator Countdown()
	{
		demoPromt.SetActive(true);
		while (true)
		{
			info.text = "Demo Ends in : " + CountDown;
			CountDown--;
			if (CountDown < 0)
			{
				OnQuit();
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void OnQuit()
	{
		SceneManager.LoadScene("TutorialScene");
	}
}
