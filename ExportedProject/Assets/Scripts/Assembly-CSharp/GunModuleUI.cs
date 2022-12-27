using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunModuleUI : MonoBehaviour
{
	public Image icon;

	public Text Info;

	public static List<GunModuleUI> moduleUIs = new List<GunModuleUI>();

	private Transform head;

	internal GunModule parentModule;

	private Collider Core;

	private void Start()
	{
		moduleUIs.Add(this);
		head = GameManager.instance.Head;
		base.gameObject.SetActive(false);
		Core = base.transform.parent.GetComponent<Collider>();
	}

	public void Setup(GunModule module)
	{
		icon.sprite = module.icon;
		Info.text = module.info;
		parentModule = module;
	}

	private void Update()
	{
		base.transform.position = Core.bounds.center + Vector3.up * 0.06f;
		base.transform.LookAt(head);
	}

	private void OnDestroy()
	{
		moduleUIs.Remove(this);
	}
}
