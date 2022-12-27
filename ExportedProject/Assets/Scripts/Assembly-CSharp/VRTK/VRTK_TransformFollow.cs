using System;
using UnityEngine;

namespace VRTK
{
	public class VRTK_TransformFollow : VRTK_ObjectFollow
	{
		public enum FollowMoment
		{
			OnUpdate = 0,
			OnLateUpdate = 1,
			OnPreRender = 2,
			OnPreCull = 3
		}

		[Tooltip("The moment at which to follow.")]
		[SerializeField]
		private FollowMoment _moment = FollowMoment.OnPreRender;

		protected Transform transformToFollow;

		protected Transform transformToChange;

		public FollowMoment moment
		{
			get
			{
				return _moment;
			}
			set
			{
				if (_moment == value)
				{
					return;
				}
				if (base.isActiveAndEnabled)
				{
					if (_moment == FollowMoment.OnPreRender && value != FollowMoment.OnPreRender)
					{
						Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(OnCamPreRender));
					}
					if (_moment != FollowMoment.OnPreRender && value == FollowMoment.OnPreRender)
					{
						Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(OnCamPreRender));
					}
					if (_moment == FollowMoment.OnPreCull && value != FollowMoment.OnPreCull)
					{
						Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(OnCamPreCull));
					}
					if (_moment != FollowMoment.OnPreCull && value == FollowMoment.OnPreCull)
					{
						Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(OnCamPreCull));
					}
				}
				_moment = value;
			}
		}

		public override void Follow()
		{
			CacheTransforms();
			base.Follow();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (moment == FollowMoment.OnPreRender)
			{
				Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(OnCamPreRender));
			}
			if (moment == FollowMoment.OnPreCull)
			{
				Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(OnCamPreCull));
			}
		}

		protected virtual void OnDisable()
		{
			transformToFollow = null;
			transformToChange = null;
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(OnCamPreRender));
			Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(OnCamPreCull));
		}

		protected void Update()
		{
			if (moment == FollowMoment.OnUpdate)
			{
				Follow();
			}
		}

		protected virtual void LateUpdate()
		{
			if (moment == FollowMoment.OnLateUpdate)
			{
				Follow();
			}
		}

		protected virtual void OnCamPreRender(Camera cam)
		{
			if (cam.gameObject.transform == VRTK_SDK_Bridge.GetHeadsetCamera())
			{
				Follow();
			}
		}

		protected virtual void OnCamPreCull(Camera cam)
		{
			if (cam.gameObject.transform == VRTK_SDK_Bridge.GetHeadsetCamera())
			{
				Follow();
			}
		}

		protected override Vector3 GetPositionToFollow()
		{
			return transformToFollow.position;
		}

		protected override void SetPositionOnGameObject(Vector3 newPosition)
		{
			transformToChange.position = newPosition;
		}

		protected override Quaternion GetRotationToFollow()
		{
			return transformToFollow.rotation;
		}

		protected override void SetRotationOnGameObject(Quaternion newRotation)
		{
			transformToChange.rotation = newRotation;
		}

		protected virtual void CacheTransforms()
		{
			if (!(gameObjectToFollow == null) && !(gameObjectToChange == null) && (!(transformToFollow != null) || !(transformToChange != null)))
			{
				transformToFollow = gameObjectToFollow.transform;
				transformToChange = gameObjectToChange.transform;
			}
		}
	}
}
