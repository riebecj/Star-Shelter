using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class IntroCapsuleEvent : MonoBehaviour
{
	public static IntroCapsuleEvent instance;

	public Animation buttonArm;

	private void Awake()
	{
		instance = this;
	}

	public void OnPressButton()
	{
		buttonArm.Play();
		Invoke("OnLoadGame", 13f);
	}

	public void OnLoadGame()
	{
		VRTK_ScreenFade.Start(Color.black, 1.5f);
		Invoke("SceneLoad", 2f);
	}

	private void SceneLoad()
	{
		SceneManager.LoadScene("MainScene");
	}
}
