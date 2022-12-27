using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Utility/Self Destruction")]
	public class SelfDestruction : MonoBehaviour
	{
		public enum ConditionType
		{
			Distance = 0,
			Bounds = 1,
			Time = 2,
			ParticleSystem = 3
		}

		public enum ReferenceType
		{
			Origin = 0,
			Point = 1,
			InitialPosition = 2,
			GameObject = 3,
			GameObjectName = 4
		}

		public ConditionType conditionType;

		public ReferenceType referenceType = ReferenceType.InitialPosition;

		public float maxDistance = 10f;

		public Bounds bounds = new Bounds(Vector3.zero, new Vector3(10f, 10f, 10f));

		public float lifetime = 5f;

		public Vector3 referencePoint;

		public GameObject referenceObject;

		public string referenceName;

		private float timer;

		private Vector3 initialPoint;

		private GameObject referenceObjectCache;

		private Vector3 GetReferencePoint()
		{
			bool isPlaying = Application.isPlaying;
			if (referenceType == ReferenceType.Point)
			{
				return referencePoint;
			}
			if (referenceType == ReferenceType.InitialPosition)
			{
				return (!isPlaying) ? base.transform.position : initialPoint;
			}
			if (referenceType == ReferenceType.GameObject && referenceObject != null)
			{
				return referenceObject.transform.position;
			}
			if (referenceType == ReferenceType.GameObjectName)
			{
				if (!isPlaying || referenceObjectCache == null)
				{
					referenceObjectCache = GameObject.Find(referenceName);
				}
				if (referenceObjectCache != null)
				{
					return referenceObjectCache.transform.position;
				}
			}
			return Vector3.zero;
		}

		private bool IsAlive()
		{
			if (conditionType == ConditionType.Distance)
			{
				return Vector3.Distance(base.transform.position, GetReferencePoint()) <= maxDistance;
			}
			if (conditionType == ConditionType.Bounds)
			{
				return bounds.Contains(base.transform.position - GetReferencePoint());
			}
			if (conditionType == ConditionType.Time)
			{
				return timer < lifetime;
			}
			return GetComponent<ParticleSystem>() != null && GetComponent<ParticleSystem>().IsAlive();
		}

		private void Start()
		{
			initialPoint = base.transform.position;
		}

		private void Update()
		{
			timer += Time.deltaTime;
		}

		private void LateUpdate()
		{
			if (!IsAlive())
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			if (conditionType == ConditionType.Distance)
			{
				Gizmos.DrawWireSphere(GetReferencePoint(), maxDistance);
			}
			if (conditionType == ConditionType.Bounds)
			{
				Gizmos.DrawWireCube(GetReferencePoint() + bounds.center, bounds.size);
			}
		}
	}
}
