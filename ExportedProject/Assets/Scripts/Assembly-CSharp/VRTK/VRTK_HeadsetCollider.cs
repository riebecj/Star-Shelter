using UnityEngine;

namespace VRTK
{
	public class VRTK_HeadsetCollider : MonoBehaviour
	{
		protected VRTK_HeadsetCollision parent;

		protected VRTK_PolicyList targetListPolicy;

		public virtual void SetParent(GameObject setParent)
		{
			parent = setParent.GetComponent<VRTK_HeadsetCollision>();
		}

		public virtual void SetIgnoreTarget(VRTK_PolicyList list = null)
		{
			targetListPolicy = list;
		}

		public virtual void EndCollision(Collider collider)
		{
			if (!collider || !VRTK_PlayerObject.IsPlayerObject(collider.gameObject))
			{
				parent.headsetColliding = false;
				parent.collidingWith = null;
				parent.OnHeadsetCollisionEnded(SetHeadsetCollisionEvent(collider, base.transform));
			}
		}

		protected virtual void OnTriggerStay(Collider collider)
		{
			if (base.enabled && !VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && ValidTarget(collider.transform))
			{
				parent.headsetColliding = true;
				parent.collidingWith = collider;
				parent.OnHeadsetCollisionDetect(SetHeadsetCollisionEvent(collider, base.transform));
			}
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			EndCollision(collider);
		}

		protected virtual void Update()
		{
			if (parent.headsetColliding && (!parent.collidingWith || !parent.collidingWith.gameObject.activeInHierarchy))
			{
				EndCollision(parent.collidingWith);
			}
		}

		protected virtual HeadsetCollisionEventArgs SetHeadsetCollisionEvent(Collider collider, Transform currentTransform)
		{
			HeadsetCollisionEventArgs result = default(HeadsetCollisionEventArgs);
			result.collider = collider;
			result.currentTransform = currentTransform;
			return result;
		}

		protected virtual bool ValidTarget(Transform target)
		{
			return (bool)target && !VRTK_PolicyList.Check(target.gameObject, targetListPolicy);
		}
	}
}
