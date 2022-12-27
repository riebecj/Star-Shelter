using UnityEngine;

namespace VRTK
{
	public class VRTK_HeadsetCollision : MonoBehaviour
	{
		[Tooltip("The radius of the auto generated sphere collider for detecting collisions on the headset.")]
		public float colliderRadius = 0.1f;

		[Tooltip("A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Headset Collision.")]
		public VRTK_PolicyList targetListPolicy;

		[HideInInspector]
		public bool headsetColliding;

		[HideInInspector]
		public Collider collidingWith;

		protected Transform headset;

		protected VRTK_HeadsetCollider headsetColliderScript;

		protected GameObject headsetColliderContainer;

		protected bool generateCollider;

		protected bool generateRigidbody;

		public event HeadsetCollisionEventHandler HeadsetCollisionDetect;

		public event HeadsetCollisionEventHandler HeadsetCollisionEnded;

		public virtual void OnHeadsetCollisionDetect(HeadsetCollisionEventArgs e)
		{
			if (this.HeadsetCollisionDetect != null)
			{
				this.HeadsetCollisionDetect(this, e);
			}
		}

		public virtual void OnHeadsetCollisionEnded(HeadsetCollisionEventArgs e)
		{
			if (this.HeadsetCollisionEnded != null)
			{
				this.HeadsetCollisionEnded(this, e);
			}
		}

		public virtual bool IsColliding()
		{
			return headsetColliding;
		}

		protected virtual void OnEnable()
		{
			VRTK_ObjectCache.registeredHeadsetCollider = this;
			headset = VRTK_DeviceFinder.HeadsetTransform();
			if ((bool)headset)
			{
				headsetColliding = false;
				SetupHeadset();
				VRTK_PlayerObject.SetPlayerObject(headsetColliderContainer.gameObject, VRTK_PlayerObject.ObjectTypes.Headset);
			}
		}

		protected virtual void OnDisable()
		{
			if ((bool)headset)
			{
				headsetColliderScript.EndCollision(collidingWith);
				VRTK_ObjectCache.registeredHeadsetCollider = null;
				TearDownHeadset();
			}
		}

		protected virtual void Update()
		{
			if ((bool)headsetColliderContainer && headsetColliderContainer.transform.parent != headset)
			{
				headsetColliderContainer.transform.SetParent(headset);
				headsetColliderContainer.transform.localPosition = Vector3.zero;
				headsetColliderContainer.transform.localRotation = headset.localRotation;
			}
		}

		protected virtual void CreateHeadsetColliderContainer()
		{
			if (!headsetColliderContainer)
			{
				headsetColliderContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, "HeadsetColliderContainer"));
				headsetColliderContainer.transform.position = Vector3.zero;
				headsetColliderContainer.transform.localRotation = headset.localRotation;
				headsetColliderContainer.transform.localScale = Vector3.one;
				headsetColliderContainer.layer = LayerMask.NameToLayer("Player");
			}
		}

		protected virtual void SetupHeadset()
		{
			Rigidbody rigidbody = null;
			if (!rigidbody)
			{
				CreateHeadsetColliderContainer();
			}
			Collider collider = null;
			if (!collider)
			{
				CreateHeadsetColliderContainer();
				SphereCollider sphereCollider = headsetColliderContainer.gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = colliderRadius;
				collider = sphereCollider;
				generateCollider = true;
			}
			if (!headsetColliderScript)
			{
				GameObject gameObject = ((!headsetColliderContainer) ? headset.gameObject : headsetColliderContainer);
				headsetColliderScript = gameObject.AddComponent<VRTK_HeadsetCollider>();
				headsetColliderScript.SetParent(base.gameObject);
				headsetColliderScript.SetIgnoreTarget(targetListPolicy);
			}
		}

		protected virtual void TearDownHeadset()
		{
			if (generateCollider)
			{
				Object.Destroy(headset.gameObject.GetComponent<BoxCollider>());
			}
			if (generateRigidbody)
			{
				Object.Destroy(headset.gameObject.GetComponent<Rigidbody>());
			}
			if ((bool)headsetColliderScript)
			{
				Object.Destroy(headsetColliderScript);
			}
			if ((bool)headsetColliderContainer)
			{
				Object.Destroy(headsetColliderContainer);
			}
		}
	}
}
