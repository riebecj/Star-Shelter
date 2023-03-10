using UnityEngine;

namespace VRTK.Examples
{
	public class LightSaber : VRTK_InteractableObject
	{
		private bool beamActive;

		private Vector2 beamLimits = new Vector2(0f, 1.2f);

		private float currentBeamSize;

		private float beamExtendSpeed;

		private GameObject blade;

		private Color activeColor;

		private Color targetColor;

		private Color[] bladePhaseColors;

		public override void StartUsing(GameObject usingObject)
		{
			base.StartUsing(usingObject);
			beamExtendSpeed = 5f;
			bladePhaseColors = new Color[2]
			{
				Color.blue,
				Color.cyan
			};
			activeColor = bladePhaseColors[0];
			targetColor = bladePhaseColors[1];
		}

		public override void StopUsing(GameObject usingObject)
		{
			base.StopUsing(usingObject);
			beamExtendSpeed = -5f;
		}

		protected void Start()
		{
			blade = base.transform.Find("Blade").gameObject;
			currentBeamSize = beamLimits.x;
			SetBeamSize();
		}

		protected override void Update()
		{
			base.Update();
			currentBeamSize = Mathf.Clamp(blade.transform.localScale.y + beamExtendSpeed * Time.deltaTime, beamLimits.x, beamLimits.y);
			SetBeamSize();
			PulseBeam();
		}

		private void SetBeamSize()
		{
			blade.transform.localScale = new Vector3(1f, currentBeamSize, 1f);
			beamActive = ((currentBeamSize >= beamLimits.y) ? true : false);
		}

		private void PulseBeam()
		{
			if (beamActive)
			{
				Color color = Color.Lerp(activeColor, targetColor, Mathf.PingPong(Time.time, 1f));
				blade.transform.Find("Beam").GetComponent<MeshRenderer>().material.color = color;
				if (color == targetColor)
				{
					Color color2 = activeColor;
					activeColor = targetColor;
					targetColor = color2;
				}
			}
		}
	}
}
