using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class ArchiveNode : MonoBehaviour
{
	public Text _title;

	public Text _titleColor;

	public string title;

	public string textBody;

	public int index;

	private Transform target;

	internal int Vibration = 800;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller")
		{
			if (target != other.transform.root)
			{
				target = other.transform.root;
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.GetComponentInParent<VRTK_ControllerEvents>().gameObject), Vibration);
				Archive.instance.ShowText(textBody);
			}
			else
			{
				target = null;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller")
		{
			target = null;
		}
	}

	public void UpdateUI()
	{
		_title.text = title;
		_titleColor.text = title;
	}
}
