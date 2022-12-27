using MidiJack;
using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Injector/Beat Injector")]
	public class BeatInjector : InjectorBase
	{
		public float bpm = 120f;

		public AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 0.5f, 0f);

		public int tapNote = -1;

		public MidiChannel tapChannel = MidiChannel.All;

		public string tapButton;

		private float time;

		private float tapTime;

		private void Update()
		{
			if (tapNote >= 0 && MidiMaster.GetKeyDown(tapChannel, tapNote))
			{
				Tap();
			}
			if (!string.IsNullOrEmpty(tapButton) && Input.GetButtonDown(tapButton))
			{
				Tap();
			}
			float num = 60f / bpm;
			time = (time + Time.deltaTime) % num;
			dbLevel = (curve.Evaluate(time / num) - 1f) * 18f;
		}

		public void Tap()
		{
			float num = Time.time - tapTime;
			if (tapTime > 0.2f && num < 3f)
			{
				bpm = Mathf.Lerp(bpm, 60f / num, 0.15f);
				time = ((!(time > 0.2f)) ? (time * 0.5f) : 0f);
			}
			tapTime = Time.time;
		}
	}
}
