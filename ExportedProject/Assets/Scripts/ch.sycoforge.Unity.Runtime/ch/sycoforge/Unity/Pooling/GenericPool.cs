using System;
using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Unity.Pooling
{
	[Serializable]
	public class GenericPool
	{
		private Action<GenericPool> OnPoolChanged_;

		[SerializeField]
		private bool hasFixedSize = true;

		[SerializeField]
		private int size;

		[SerializeField]
		private int overflowCount;

		private Dictionary<int, GameObject> sceneItems;

		private Stack<GameObject> poolItems;

		private Action<GameObject, GenericPool> onInitialized;

		[SerializeField]
		private List<GameObject> items;

		[SerializeField]
		private string name;

		[SerializeField]
		private HideFlags flags;

		private GameObject prototype;

		public Stack<GameObject> PoolItems
		{
			get
			{
				return poolItems;
			}
			protected set
			{
				poolItems = value;
			}
		}

		public Dictionary<int, GameObject> SceneItems
		{
			get
			{
				return sceneItems;
			}
			protected set
			{
				sceneItems = value;
			}
		}

		public List<GameObject> Items
		{
			get
			{
				return items;
			}
			protected set
			{
				items = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			protected set
			{
				name = value;
			}
		}

		public bool HasFixedSize
		{
			get
			{
				return hasFixedSize;
			}
			set
			{
				hasFixedSize = value;
			}
		}

		public int OverflowCount
		{
			get
			{
				return overflowCount;
			}
		}

		public int SpawnedCount
		{
			get
			{
				return (sceneItems != null) ? sceneItems.Count : 0;
			}
		}

		public int ReadyCount
		{
			get
			{
				return (poolItems != null) ? poolItems.Count : 0;
			}
		}

		public int Size
		{
			get
			{
				return size;
			}
		}

		public int AbsoluteSize
		{
			get
			{
				return (items != null) ? items.Count : (-1);
			}
		}

		public event Action<GenericPool> OnPoolChanged
		{
			add
			{
				OnPoolChanged_ = (Action<GenericPool>)Delegate.Combine(OnPoolChanged_, value);
			}
			remove
			{
				OnPoolChanged_ = (Action<GenericPool>)Delegate.Remove(OnPoolChanged_, value);
			}
		}

		public GenericPool()
			: this(string.Empty)
		{
		}

		public GenericPool(string name)
		{
			this.name = name;
		}

		public void Rebuild(Action<GameObject, GenericPool> onInitialized = null)
		{
			if (sceneItems != null)
			{
				sceneItems.Clear();
			}
			else
			{
				sceneItems = new Dictionary<int, GameObject>();
			}
			if (poolItems != null)
			{
				poolItems.Clear();
			}
			else
			{
				poolItems = new Stack<GameObject>();
			}
			foreach (GameObject item in items)
			{
				if (!(item != null))
				{
					continue;
				}
				if (item.activeSelf)
				{
					int instanceID = item.GetInstanceID();
					if (!sceneItems.ContainsKey(instanceID))
					{
						sceneItems.Add(instanceID, item);
					}
				}
				else
				{
					poolItems.Push(item);
				}
				if (onInitialized != null)
				{
					onInitialized(item, this);
				}
			}
		}

		public void Initialize<T>(int count, Action<GameObject, GenericPool> onOnitialized, HideFlags flags = HideFlags.None) where T : MonoBehaviour
		{
			Type typeFromHandle = typeof(T);
			string text = typeFromHandle.Name;
			InternalInitialize(count, new GameObject(text, typeFromHandle), onOnitialized, flags);
		}

		public void Initialize(GameObject prefab, int count, Action<GameObject, GenericPool> onIntialized, HideFlags flags = HideFlags.None)
		{
			InternalInitialize(count, prefab, onIntialized, flags);
		}

		private void InternalInitialize(int count, GameObject prototype, Action<GameObject, GenericPool> onIntialized, HideFlags flags)
		{
			size = Math.Max(count, 0);
			this.prototype = prototype;
			this.prototype.SetActive(false);
			this.prototype.hideFlags = flags;
			this.flags = flags;
			onInitialized = onIntialized;
			sceneItems = new Dictionary<int, GameObject>(count);
			poolItems = new Stack<GameObject>(count);
			items = new List<GameObject>(count);
			FillPool(flags);
		}

		private void FillPool(HideFlags flags)
		{
			for (int i = 0; i < size; i++)
			{
				poolItems.Push(InstantiatePrototype(i, flags));
			}
		}

		private GameObject InstantiatePrototype(int id, HideFlags flags)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prototype);
			gameObject.name = gameObject.name + " " + id;
			gameObject.SetActive(false);
			gameObject.hideFlags = flags;
			if (Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			if (onInitialized != null)
			{
				onInitialized(gameObject, this);
			}
			items.Add(gameObject);
			return gameObject;
		}

		public void Clear(bool destroy = true)
		{
			if (sceneItems != null)
			{
				sceneItems.Clear();
			}
			if (poolItems != null)
			{
				poolItems.Clear();
			}
			foreach (GameObject item in items)
			{
				if (destroy)
				{
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(item);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(item);
					}
				}
			}
			if (destroy)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(prototype);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(prototype);
				}
			}
			items.Clear();
		}

		public GameObject Spawn(Vector3 position, Vector3 scale, Quaternion rotation)
		{
			CheckValidity();
			GameObject gameObject = null;
			if (poolItems == null)
			{
				return null;
			}
			if (poolItems.Count == 0)
			{
				if (!hasFixedSize)
				{
					gameObject = InstantiatePrototype(overflowCount++, flags);
				}
			}
			else
			{
				gameObject = poolItems.Pop();
			}
			if (gameObject != null)
			{
				gameObject.SetActive(true);
				int instanceID = gameObject.GetInstanceID();
				if (!sceneItems.ContainsKey(instanceID))
				{
					sceneItems.Add(instanceID, gameObject);
				}
				sceneItems[instanceID] = gameObject;
			}
			CallOnPoolChanged();
			return gameObject;
		}

		public GameObject Spawn(Vector3 position, Quaternion rotation)
		{
			return Spawn(position, Vector3.one, rotation);
		}

		public GameObject Spawn(Vector3 position)
		{
			return Spawn(position, Vector3.one, Quaternion.identity);
		}

		public GameObject Spawn()
		{
			return Spawn(Vector3.one, Vector3.one, Quaternion.identity);
		}

		public void Despawn(GameObject obj)
		{
			CheckValidity();
			int instanceID = obj.GetInstanceID();
			if (sceneItems.ContainsKey(instanceID))
			{
				GameObject gameObject = sceneItems[instanceID];
				gameObject.SetActive(false);
				sceneItems.Remove(instanceID);
				poolItems.Push(gameObject);
				CallOnPoolChanged();
			}
		}

		public override string ToString()
		{
			return name;
		}

		private void CheckValidity()
		{
			if (sceneItems == null || poolItems == null)
			{
				Rebuild();
			}
		}

		private void CallOnPoolChanged()
		{
			if (OnPoolChanged_ != null)
			{
				OnPoolChanged_(this);
			}
		}
	}
}
