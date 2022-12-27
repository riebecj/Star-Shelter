using UnityEngine;

namespace VRTK
{
	[RequireComponent(typeof(VRTK_HeadsetCollision))]
	[RequireComponent(typeof(VRTK_HeadsetFade))]
	public class VRTK_HeadsetCollisionFade : MonoBehaviour
	{
		[Tooltip("The amount of time to wait until a fade occurs.")]
		public float timeTillFade;

		[Tooltip("The fade blink speed on collision.")]
		public float blinkTransitionSpeed = 0.1f;

		[Tooltip("The colour to fade the headset to on collision.")]
		public Color fadeColor = Color.black;

		protected VRTK_HeadsetCollision headsetCollision;

		protected VRTK_HeadsetFade headsetFade;

		protected virtual void OnEnable()
		{
			headsetFade = GetComponent<VRTK_HeadsetFade>();
			headsetCollision = GetComponent<VRTK_HeadsetCollision>();
			headsetCollision.HeadsetCollisionDetect += OnHeadsetCollisionDetect;
			headsetCollision.HeadsetCollisionEnded += OnHeadsetCollisionEnded;
		}

		protected virtual void OnDisable()
		{
			headsetCollision.HeadsetCollisionDetect -= OnHeadsetCollisionDetect;
			headsetCollision.HeadsetCollisionEnded -= OnHeadsetCollisionEnded;
		}

		protected virtual void OnHeadsetCollisionDetect(object sender, HeadsetCollisionEventArgs e)
		{
			Invoke("StartFade", timeTillFade);
		}

		protected virtual void OnHeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
		{
			CancelInvoke("StartFade");
			headsetFade.Unfade(blinkTransitionSpeed);
		}

		protected virtual void StartFade()
		{
			headsetFade.Fade(fadeColor, blinkTransitionSpeed);
		}
	}
}
