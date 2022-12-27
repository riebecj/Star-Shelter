using UnityEngine;

namespace VRTK
{
	public class VRTK_CollisionTracker : MonoBehaviour
	{
		public event CollisionTrackerEventHandler CollisionEnter;

		public event CollisionTrackerEventHandler CollisionStay;

		public event CollisionTrackerEventHandler CollisionExit;

		public event CollisionTrackerEventHandler TriggerEnter;

		public event CollisionTrackerEventHandler TriggerStay;

		public event CollisionTrackerEventHandler TriggerExit;

		protected void OnCollisionEnterEvent(CollisionTrackerEventArgs e)
		{
			if (this.CollisionEnter != null)
			{
				this.CollisionEnter(this, e);
			}
		}

		protected void OnCollisionStayEvent(CollisionTrackerEventArgs e)
		{
			if (this.CollisionStay != null)
			{
				this.CollisionStay(this, e);
			}
		}

		protected void OnCollisionExitEvent(CollisionTrackerEventArgs e)
		{
			if (this.CollisionExit != null)
			{
				this.CollisionExit(this, e);
			}
		}

		protected void OnTriggerEnterEvent(CollisionTrackerEventArgs e)
		{
			if (this.TriggerEnter != null)
			{
				this.TriggerEnter(this, e);
			}
		}

		protected void OnTriggerStayEvent(CollisionTrackerEventArgs e)
		{
			if (this.TriggerStay != null)
			{
				this.TriggerStay(this, e);
			}
		}

		protected void OnTriggerExitEvent(CollisionTrackerEventArgs e)
		{
			if (this.TriggerExit != null)
			{
				this.TriggerExit(this, e);
			}
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			OnCollisionEnterEvent(SetCollisionTrackerEvent(false, collision, collision.collider));
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			OnCollisionStayEvent(SetCollisionTrackerEvent(false, collision, collision.collider));
		}

		protected virtual void OnCollisionExit(Collision collision)
		{
			OnCollisionExitEvent(SetCollisionTrackerEvent(false, collision, collision.collider));
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			OnTriggerEnterEvent(SetCollisionTrackerEvent(true, null, collider));
		}

		protected virtual void OnTriggerStay(Collider collider)
		{
			OnTriggerStayEvent(SetCollisionTrackerEvent(true, null, collider));
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			OnTriggerExitEvent(SetCollisionTrackerEvent(true, null, collider));
		}

		protected virtual CollisionTrackerEventArgs SetCollisionTrackerEvent(bool isTrigger, Collision givenCollision, Collider givenCollider)
		{
			CollisionTrackerEventArgs result = default(CollisionTrackerEventArgs);
			result.isTrigger = isTrigger;
			result.collision = givenCollision;
			result.collider = givenCollider;
			return result;
		}
	}
}
