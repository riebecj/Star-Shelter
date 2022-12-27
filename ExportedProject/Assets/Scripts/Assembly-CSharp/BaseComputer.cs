using UnityEngine;

public class BaseComputer : MonoBehaviour
{
	[Tooltip("MenuButton")]
	public GameObject menu;

	[Tooltip("MenuButton")]
	public GameObject archive;

	[Tooltip("MenuButton")]
	public GameObject help;

	[Tooltip("MenuButton")]
	public GameObject craft;

	[Tooltip("MenuButton")]
	public GameObject info;

	[Tooltip("MenuButton")]
	public GameObject stats;

	public static BaseComputer instance;

	internal bool zoomedIn;

	private AudioSource audioSource;

	public AudioClip beep;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void OnMenu()
	{
		DisableOtherUI();
		menu.SetActive(true);
	}

	public void OnCraft()
	{
		DisableOtherUI();
		craft.SetActive(true);
		craft.GetComponentInChildren<CraftBound>(true).gameObject.SetActive(true);
	}

	public void OnArchive()
	{
		DisableOtherUI();
		archive.SetActive(true);
	}

	public void OnHelp()
	{
		stats.SetActive(false);
		info.SetActive(false);
		help.SetActive(true);
	}

	public void OnShowStats()
	{
		stats.SetActive(true);
		info.SetActive(false);
		help.SetActive(false);
	}

	public void OnShowInfo()
	{
		stats.SetActive(false);
		help.SetActive(false);
		info.SetActive(true);
	}

	public void DisableOtherUI()
	{
		menu.SetActive(false);
		archive.SetActive(false);
		craft.SetActive(false);
		audioSource.PlayOneShot(beep);
	}

	public void Beep()
	{
		audioSource.PlayOneShot(beep);
	}
}
