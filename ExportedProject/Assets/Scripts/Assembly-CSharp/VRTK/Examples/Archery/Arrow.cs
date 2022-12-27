using UnityEngine;

namespace VRTK.Examples.Archery
{
	public class Arrow : MonoBehaviour
	{
		public float maxArrowLife = 10f;

		[HideInInspector]
		public bool inFlight;

		private bool collided;

		private Rigidbody rigidBody;

		private GameObject arrowHolder;

		private Vector3 originalPosition;

		private Quaternion originalRotation;

		private Vector3 originalScale;

		public void SetArrowHolder(GameObject holder)
		{
			arrowHolder = holder;
			arrowHolder.SetActive(false);
		}

		public void OnNock()
		{
			collided = false;
			inFlight = false;
		}

		public void Fired()
		{
			DestroyArrow(maxArrowLife);
		}

		public void ResetArrow()
		{
			collided = true;
			inFlight = false;
			RecreateNotch();
			ResetTransform();
		}

		private void Start()
		{
			rigidBody = GetComponent<Rigidbody>();
			SetOrigns();
		}

		private void SetOrigns()
		{
			originalPosition = base.transform.localPosition;
			originalRotation = base.transform.localRotation;
			originalScale = base.transform.localScale;
		}

		private void FixedUpdate()
		{
			if (!collided)
			{
				base.transform.LookAt(base.transform.position + rigidBody.velocity);
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (inFlight)
			{
				ResetArrow();
			}
		}

		private void RecreateNotch()
		{
			arrowHolder.transform.SetParent(null);
			arrowHolder.SetActive(true);
			base.transform.SetParent(arrowHolder.transform);
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<Collider>().enabled = false;
			arrowHolder.GetComponent<Rigidbody>().isKinematic = false;
		}

		private void ResetTransform()
		{
			arrowHolder.transform.position = base.transform.position;
			arrowHolder.transform.rotation = base.transform.rotation;
			base.transform.localPosition = originalPosition;
			base.transform.localRotation = originalRotation;
			base.transform.localScale = originalScale;
		}

		private void DestroyArrow(float time)
		{
			Object.Destroy(arrowHolder, time);
			Object.Destroy(base.gameObject, time);
		}
	}
}
