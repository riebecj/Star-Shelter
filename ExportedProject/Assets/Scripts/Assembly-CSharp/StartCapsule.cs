using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class StartCapsule : MonoBehaviour
{
	internal Transform target;

	private Vector3 offset;

	public static StartCapsule instance;

	public string[] Names;

	private string[] Letters = new string[7] { "E.", "R.", "D.", "K.", "H.", "J", "S" };

	public Text info;

	public Text infoBG;

	private void Awake()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("GameStarted"))
		{
			CryoPodLever.instance.open = true;
			Object.Destroy(base.gameObject);
		}
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
	}

	private void Start()
	{
		target = GameManager.instance.Head;
		offset = new Vector3(0f, -0.5f, 0.15f);
		base.transform.position = target.position;
		base.transform.LookAt(target.position + target.transform.forward * 5f);
		base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
		GenerateName();
	}

	public void GenerateName()
	{
		string text = "Name: " + Letters[Random.Range(0, Letters.Length)] + " " + Names[Random.Range(0, Names.Length)];
		text = text + "\nAge: " + Random.Range(16, 48);
		text = text + "\nLength: " + Random.Range(160, 199);
		text = text + "\nFrom: C-" + Random.Range(12, 999);
		info.text = text;
		infoBG.text = text;
	}

	private void Update()
	{
		base.transform.position = target.position + base.transform.right * offset.x + base.transform.up * offset.y + base.transform.forward * offset.z;
	}
}
