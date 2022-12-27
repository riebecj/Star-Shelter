using System.Collections.Generic;
using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Utility/Planter")]
	public class Planter : MonoBehaviour
	{
		public enum DistributionMode
		{
			Single = 0,
			Random = 1,
			Grid = 2
		}

		public enum RotationMode
		{
			Keep = 0,
			Planter = 1,
			Random = 2
		}

		public enum IntervalMode
		{
			Distance = 0,
			Time = 1
		}

		public GameObject[] prefabs;

		[SerializeField]
		private int _maxObjects = 100;

		public DistributionMode distributionMode;

		[SerializeField]
		private Vector2 _distributionRange = new Vector2(3f, 0f);

		[SerializeField]
		private float _gridSpace = 1f;

		public RotationMode rotationMode;

		public IntervalMode intervalMode;

		[SerializeField]
		private float _interval = 1f;

		private Queue<GameObject> objectPool;

		private float intervalCounter;

		private Vector3 previousPosition;

		private Quaternion previousRotation;

		public int maxObjects
		{
			get
			{
				return Mathf.Max(1, _maxObjects);
			}
			set
			{
				_maxObjects = value;
			}
		}

		public Vector2 distributionRange
		{
			get
			{
				return Vector2.Max(Vector2.zero, _distributionRange);
			}
			set
			{
				_distributionRange = value;
			}
		}

		public float gridSpace
		{
			get
			{
				return Mathf.Max(0.01f, _gridSpace);
			}
			set
			{
				_gridSpace = value;
			}
		}

		public float interval
		{
			get
			{
				return Mathf.Max(0.01f, _interval);
			}
			set
			{
				_interval = value;
			}
		}

		private void PutInstance(Vector3 position, Quaternion rotation)
		{
			if (rotationMode == RotationMode.Random)
			{
				rotation = Random.rotation;
			}
			if (objectPool.Count >= maxObjects)
			{
				GameObject gameObject = objectPool.Dequeue();
				gameObject.SetActive(false);
				gameObject.transform.position = position;
				if (rotationMode != 0)
				{
					gameObject.transform.rotation = rotation;
				}
				gameObject.SetActive(true);
				objectPool.Enqueue(gameObject);
			}
			else
			{
				GameObject gameObject2 = prefabs[Random.Range(0, prefabs.Length)];
				if (rotationMode == RotationMode.Keep)
				{
					rotation = gameObject2.transform.rotation;
				}
				GameObject item = Object.Instantiate(gameObject2, position, rotation);
				objectPool.Enqueue(item);
			}
		}

		private void PlantAlongGrid(Vector3 position, Quaternion rotation)
		{
			Vector3 vector = rotation * Vector3.right;
			Vector3 vector2 = rotation * Vector3.up;
			int num = Mathf.Max(Mathf.FloorToInt(distributionRange.x / gridSpace), 0);
			int num2 = Mathf.Max(Mathf.FloorToInt(distributionRange.y / gridSpace), 0);
			for (int i = 0; i <= num2; i++)
			{
				float num3 = gridSpace * ((float)i - 0.5f * (float)num2);
				for (int j = 0; j <= num; j++)
				{
					float num4 = gridSpace * ((float)j - 0.5f * (float)num);
					PutInstance(position + vector * num4 + vector2 * num3, rotation);
				}
			}
		}

		private void PlantRandom(Vector3 position, Quaternion rotation)
		{
			Vector3 vector = rotation * Vector3.right;
			Vector3 vector2 = rotation * Vector3.up;
			float num = (Random.value - 0.5f) * distributionRange.x;
			float num2 = (Random.value - 0.5f) * distributionRange.y;
			PutInstance(position + vector * num + vector2 * num2, rotation);
		}

		private void Awake()
		{
			objectPool = new Queue<GameObject>();
		}

		private void Start()
		{
			previousPosition = base.transform.position;
			previousRotation = base.transform.rotation;
		}

		private void Update()
		{
			if (prefabs == null || prefabs.Length == 0)
			{
				return;
			}
			float num = ((intervalMode != 0) ? Time.deltaTime : Vector3.Distance(base.transform.position, previousPosition));
			for (float num2 = interval; num2 < intervalCounter + num; num2 += interval)
			{
				float t = (num2 - intervalCounter) / num;
				Vector3 position = Vector3.Lerp(previousPosition, base.transform.position, t);
				Quaternion rotation = Quaternion.Slerp(previousRotation, base.transform.rotation, t);
				if (distributionMode == DistributionMode.Grid)
				{
					PlantAlongGrid(position, rotation);
				}
				else if (distributionMode == DistributionMode.Random)
				{
					PlantRandom(position, rotation);
				}
				else
				{
					PutInstance(position, rotation);
				}
			}
			intervalCounter = (intervalCounter + num) % interval;
			previousPosition = base.transform.position;
			previousRotation = base.transform.rotation;
			while (objectPool.Count > maxObjects)
			{
				Object.Destroy(objectPool.Dequeue());
			}
		}

		private void OnDrawGizmos()
		{
			if (distributionMode != 0)
			{
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireCube(Vector3.zero, distributionRange);
			}
		}
	}
}
