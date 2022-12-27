using UnityEngine;

namespace VRTK
{
	public class VRTK_PlayAreaCollider : MonoBehaviour
	{
		protected VRTK_PlayAreaCursor parent;

		protected VRTK_PolicyList targetListPolicy;

		public virtual void SetParent(VRTK_PlayAreaCursor setParent)
		{
			parent = setParent;
		}

		public virtual void SetIgnoreTarget(VRTK_PolicyList list = null)
		{
			targetListPolicy = list;
		}

		protected virtual void OnDisable()
		{
			if ((bool)parent)
			{
				parent.SetPlayAreaCursorCollision(false);
			}
		}

		protected virtual void OnTriggerStay(Collider collider)
		{
			if ((bool)parent && parent.enabled && parent.gameObject.activeInHierarchy && ValidTarget(collider))
			{
				parent.SetPlayAreaCursorCollision(true);
			}
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			if ((bool)parent && ValidTarget(collider))
			{
				parent.SetPlayAreaCursorCollision(false);
			}
		}

		protected virtual bool ValidTarget(Collider collider)
		{
			return !VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && !VRTK_PolicyList.Check(collider.gameObject, targetListPolicy);
		}
	}
}
