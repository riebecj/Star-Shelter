using System.Collections;
using PreviewLabs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRTK;

public class SuitSlider : MonoBehaviour
{
	public enum VolumeType
	{
		master = 0,
		sfx = 1,
		voice = 2
	}

	public UnityEvent OnClick;

	private Transform target;

	internal int Vibration = 800;

	internal VRTK_InteractableObject interact;

	public VolumeType volumeType;

	public Text number;

	public Text numberColor;

	private void OnEnable()
	{
		if ((bool)ArmUIManager.instance)
		{
			if (!ArmUIManager.instance.gripSwap)
			{
				GetComponent<VRTK_InteractableObject>().useOverrideButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
			}
			else
			{
				GetComponent<VRTK_InteractableObject>().useOverrideButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			}
		}
	}

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectUsed += OnUse;
		interact.InteractableObjectUnused += OnStopUse;
		OnLoad();
	}

	private void OnLoad()
	{
		if (volumeType == VolumeType.master)
		{
			int @int = PreviewLabs.PlayerPrefs.GetInt("Master");
			base.transform.localPosition = new Vector3(-50 + @int * 10, 0f, -2f);
			number.text = @int.ToString();
			numberColor.text = @int.ToString();
		}
		else if (volumeType == VolumeType.sfx)
		{
			int int2 = PreviewLabs.PlayerPrefs.GetInt("SFX");
			base.transform.localPosition = new Vector3(-50 + int2 * 10, 0f, -2f);
			number.text = int2.ToString();
			numberColor.text = int2.ToString();
		}
		else if (volumeType == VolumeType.voice)
		{
			int int3 = PreviewLabs.PlayerPrefs.GetInt("Voice");
			base.transform.localPosition = new Vector3(-50 + int3 * 10, 0f, -2f);
			number.text = int3.ToString();
			numberColor.text = int3.ToString();
		}
	}

	private bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void OnUse(object sender, InteractableObjectEventArgs e)
	{
		StartCoroutine("UpdateState");
		GetComponent<Rigidbody>().isKinematic = false;
	}

	private void OnStopUse(object sender, InteractableObjectEventArgs e)
	{
		GetComponent<Rigidbody>().isKinematic = true;
		StopCoroutine("UpdateState");
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			float value = 0f;
			if (IsHeld())
			{
				value = Mathf.Round((base.transform.localPosition.x + 50f) / 10f);
			}
			UpdateState((int)value);
			yield return new WaitForSeconds(0.02f);
		}
	}

	private void UpdateState(int value)
	{
		number.text = value.ToString();
		numberColor.text = value.ToString();
		if (volumeType == VolumeType.master)
		{
			GameAudioManager.instance.UpdateMainVolume(value);
		}
		else if (volumeType == VolumeType.sfx)
		{
			GameAudioManager.instance.UpdateSFXVolume(value);
		}
		else if (volumeType == VolumeType.voice)
		{
			GameAudioManager.instance.UpdateVoiceVolume(value);
		}
	}

	public void OnHighlight()
	{
		GetComponent<MeshRenderer>().material = Inventory.instance.buttonHighlightedMaterial;
	}
}
