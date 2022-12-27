using UnityEngine;

namespace VRTK
{
	public class VRTK_HeadsetFade : MonoBehaviour
	{
		protected Transform headset;

		protected bool isTransitioning;

		protected bool isFaded;

		public event HeadsetFadeEventHandler HeadsetFadeStart;

		public event HeadsetFadeEventHandler HeadsetFadeComplete;

		public event HeadsetFadeEventHandler HeadsetUnfadeStart;

		public event HeadsetFadeEventHandler HeadsetUnfadeComplete;

		public virtual void OnHeadsetFadeStart(HeadsetFadeEventArgs e)
		{
			if (this.HeadsetFadeStart != null)
			{
				this.HeadsetFadeStart(this, e);
			}
		}

		public virtual void OnHeadsetFadeComplete(HeadsetFadeEventArgs e)
		{
			if (this.HeadsetFadeComplete != null)
			{
				this.HeadsetFadeComplete(this, e);
			}
		}

		public virtual void OnHeadsetUnfadeStart(HeadsetFadeEventArgs e)
		{
			if (this.HeadsetUnfadeStart != null)
			{
				this.HeadsetUnfadeStart(this, e);
			}
		}

		public virtual void OnHeadsetUnfadeComplete(HeadsetFadeEventArgs e)
		{
			if (this.HeadsetUnfadeComplete != null)
			{
				this.HeadsetUnfadeComplete(this, e);
			}
		}

		public virtual bool IsFaded()
		{
			return isFaded;
		}

		public virtual bool IsTransitioning()
		{
			return isTransitioning;
		}

		public virtual void Fade(Color color, float duration)
		{
			isFaded = false;
			isTransitioning = true;
			VRTK_SDK_Bridge.HeadsetFade(color, duration);
			OnHeadsetFadeStart(SetHeadsetFadeEvent(headset, duration));
			CancelInvoke("UnfadeComplete");
			Invoke("FadeComplete", duration);
		}

		public virtual void Unfade(float duration)
		{
			isFaded = true;
			isTransitioning = true;
			VRTK_SDK_Bridge.HeadsetFade(Color.clear, duration);
			OnHeadsetUnfadeStart(SetHeadsetFadeEvent(headset, duration));
			CancelInvoke("FadeComplete");
			Invoke("UnfadeComplete", duration);
		}

		protected virtual void Start()
		{
			headset = VRTK_DeviceFinder.HeadsetCamera();
			isTransitioning = false;
			isFaded = false;
			VRTK_SharedMethods.AddCameraFade();
			if (!VRTK_SDK_Bridge.HasHeadsetFade(headset))
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_HeadsetFade", "compatible fade", "Camera"));
			}
		}

		protected virtual HeadsetFadeEventArgs SetHeadsetFadeEvent(Transform currentTransform, float duration)
		{
			HeadsetFadeEventArgs result = default(HeadsetFadeEventArgs);
			result.timeTillComplete = duration;
			result.currentTransform = currentTransform;
			return result;
		}

		protected virtual void FadeComplete()
		{
			isFaded = true;
			isTransitioning = false;
			OnHeadsetFadeComplete(SetHeadsetFadeEvent(headset, 0f));
		}

		protected virtual void UnfadeComplete()
		{
			isFaded = false;
			isTransitioning = false;
			OnHeadsetUnfadeComplete(SetHeadsetFadeEvent(headset, 0f));
		}
	}
}
