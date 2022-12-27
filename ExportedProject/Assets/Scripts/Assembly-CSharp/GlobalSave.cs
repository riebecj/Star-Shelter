using PreviewLabs;
using UnityEngine;

public class GlobalSave : MonoBehaviour
{
	public static GlobalSave instance;

	private void Awake()
	{
		instance = this;
	}

	public void OnSave()
	{
		UnityEngine.PlayerPrefs.SetInt("DERP" + PreviewLabs.PlayerPrefs.saveSlot, (int)Time.timeSinceLevelLoad + GameManager.instance.previousTimePlayed);
		UnityEngine.PlayerPrefs.Save();
		Debug.Log("lul");
	}
}
