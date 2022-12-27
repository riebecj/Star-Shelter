using UnityEngine;

public class ShieldButton : MonoBehaviour
{
	public Renderer renderer;

	public Color onColor;

	public Color offColor;

	internal bool on;

	internal HoloShield shield;

	public void AssignIndex(int index)
	{
	}

	public void OnClick()
	{
		if (!on && BaseManager.instance.power > 5f)
		{
			shield.OnActivate();
			ToggleOn();
			on = true;
		}
		else
		{
			shield.OnDeactivate();
			ToggleOff();
			on = false;
		}
	}

	public void ToggleOn()
	{
		renderer.material.color = onColor;
	}

	public void ToggleOff()
	{
		renderer.material.color = offColor;
	}
}
