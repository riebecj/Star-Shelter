using UnityEngine;

namespace VRTK.Examples
{
	public class Breakable_Cube : MonoBehaviour
	{
		private float breakForce = 150f;

		private void Start()
		{
			GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
		}

		private void OnCollisionEnter(Collision collision)
		{
			float collisionForce = GetCollisionForce(collision);
			if (collisionForce > 0f)
			{
				ExplodeCube(collisionForce);
			}
		}

		private float GetCollisionForce(Collision collision)
		{
			if (collision.collider.name.Contains("Sword") && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce)
			{
				return collision.collider.GetComponent<Sword>().CollisionForce() * 1.2f;
			}
			if (collision.collider.name.Contains("Arrow"))
			{
				return 500f;
			}
			return 0f;
		}

		private void ExplodeCube(float force)
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (!(transform.transform.name == base.transform.name))
				{
					ExplodeFace(transform, force);
				}
			}
			Object.Destroy(base.gameObject, 10f);
		}

		private void ExplodeFace(Transform face, float force)
		{
			face.transform.SetParent(null);
			Rigidbody rigidbody = face.gameObject.AddComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
			rigidbody.AddExplosionForce(force, Vector3.zero, 0f);
			Object.Destroy(face.gameObject, 10f);
		}
	}
}
