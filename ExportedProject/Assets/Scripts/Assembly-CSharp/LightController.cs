using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightController : MonoBehaviour
{
	private Light mLight;

	private MeshRenderer mMeshRenderer;

	private Animation mAnimation;

	public Color[] Colours;

	public Material[] Materials;

	public int CurrentColour;

	private void OnEnable()
	{
		mLight = GetComponent<Light>();
		if (mLight == null)
		{
			Debug.LogError("Liight is missing from " + base.name);
		}
		mMeshRenderer = GetComponent<MeshRenderer>();
		if (mMeshRenderer == null)
		{
			Debug.LogError("MeshRenderer is missing from " + base.name);
		}
		mAnimation = GetComponent<Animation>();
		if (mAnimation == null)
		{
			Debug.LogError("Animation is missing from " + base.name);
		}
		else
		{
			mAnimation["Bob"].time = Random.Range(0f, mAnimation["Bob"].length);
		}
		SetLightColour(Colours[CurrentColour]);
		SetModelColour(Materials[CurrentColour]);
	}

	private void SetIntensity(float _value)
	{
		mLight.intensity = _value * 8f;
	}

	private void SetLightColour(Color _colour)
	{
		mLight.color = _colour;
	}

	private void SetModelColour(Material _mat)
	{
		mMeshRenderer.material = _mat;
	}

	public void ColourChanged()
	{
		CurrentColour = ((CurrentColour < Colours.Length - 1) ? (CurrentColour + 1) : 0);
		SetLightColour(Colours[CurrentColour]);
		SetModelColour(Materials[CurrentColour]);
	}

	public void IntensityChanged(VRLever _lever, float _currentValue, float _lastValue)
	{
		SetIntensity(_currentValue);
	}

	public void IntensityChanged(VRLever _lever)
	{
		if (_lever == null)
		{
			Debug.LogError("_lever is null");
		}
		else
		{
			SetIntensity(_lever.Value);
		}
	}
}
