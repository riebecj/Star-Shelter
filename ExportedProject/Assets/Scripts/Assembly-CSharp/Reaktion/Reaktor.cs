using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Reaktor/Reaktor")]
	public class Reaktor : MonoBehaviour
	{
		public InjectorLink injector;

		public AnimationCurve audioCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public Remote gain;

		public Remote offset;

		public float sensitivity = 0.95f;

		public float decaySpeed = 0.5f;

		public bool showAudioOptions;

		public float headroom = 1f;

		public float dynamicRange = 17f;

		public float lowerBound = -60f;

		public float falldown = 0.5f;

		private float output;

		private float peak;

		private float rawInput;

		private float fakeInput = -1f;

		private static int activeInstanceCount;

		public float Output
		{
			get
			{
				return output;
			}
		}

		public float Peak
		{
			get
			{
				return peak;
			}
		}

		public float RawInput
		{
			get
			{
				return rawInput;
			}
		}

		public float Gain
		{
			get
			{
				return gain.level;
			}
		}

		public float Offset
		{
			get
			{
				return offset.level;
			}
		}

		public float Override
		{
			get
			{
				return Mathf.Clamp01(fakeInput);
			}
			set
			{
				fakeInput = value;
			}
		}

		public bool IsOverridden
		{
			get
			{
				return fakeInput >= 0f;
			}
		}

		public bool Bang
		{
			get
			{
				return fakeInput > 1f;
			}
			set
			{
				fakeInput = ((!value) ? (-1f) : 10f);
			}
		}

		public static int ActiveInstanceCount
		{
			get
			{
				return activeInstanceCount;
			}
		}

		private void Start()
		{
			injector.Initialize(this);
			gain.Reset(1f);
			offset.Reset(0f);
			peak = lowerBound + dynamicRange + headroom;
			rawInput = -1E+12f;
		}

		private void Update()
		{
			float num = 0f;
			rawInput = injector.DbLevel;
			peak -= Time.deltaTime * falldown;
			peak = Mathf.Max(peak, Mathf.Max(rawInput, lowerBound + dynamicRange + headroom));
			num = (rawInput - peak + headroom + dynamicRange) / dynamicRange;
			num = audioCurve.Evaluate(Mathf.Clamp01(num));
			gain.Update();
			offset.Update();
			num *= gain.level;
			num += offset.level;
			num = Mathf.Clamp01((!(fakeInput < 0f)) ? fakeInput : num);
			if (sensitivity < 1f)
			{
				float num2 = Mathf.Pow(sensitivity, 2.3f) * -128f;
				num -= (num - output) * Mathf.Exp(num2 * Time.deltaTime);
			}
			float num3 = ((!(decaySpeed < 1f)) ? 100f : (decaySpeed * 10f + 0.5f));
			output = Mathf.Max(num, output - Time.deltaTime * num3);
		}

		private void OnEnable()
		{
			activeInstanceCount++;
		}

		private void OnDisable()
		{
			activeInstanceCount--;
		}

		public void StopOverride()
		{
			fakeInput = -1f;
		}
	}
}
