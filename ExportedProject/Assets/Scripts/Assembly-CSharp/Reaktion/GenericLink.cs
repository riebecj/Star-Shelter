using UnityEngine;

namespace Reaktion
{
	public class GenericLink<T> : GenericLinkBase where T : Component
	{
		public enum Mode
		{
			Null = 0,
			Automatic = 1,
			ByReference = 2,
			ByName = 3
		}

		[SerializeField]
		private Mode _mode = Mode.Automatic;

		[SerializeField]
		private T _reference;

		[SerializeField]
		private string _name;

		[SerializeField]
		private bool _forceUpdate;

		private MonoBehaviour master;

		private T _linkedObject;

		public Mode mode
		{
			get
			{
				return _mode;
			}
			set
			{
				_mode = value;
				Update();
			}
		}

		public T reference
		{
			get
			{
				return _reference;
			}
			set
			{
				_reference = value;
				Update();
			}
		}

		public string name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				Update();
			}
		}

		public T linkedObject
		{
			get
			{
				if (_forceUpdate)
				{
					Update();
				}
				return _linkedObject;
			}
		}

		public void Initialize(MonoBehaviour master)
		{
			this.master = master;
			Update();
		}

		public void Update()
		{
			_linkedObject = FindLinkedObject();
			_forceUpdate = false;
		}

		private T FindLinkedObject()
		{
			if (_mode == Mode.Automatic && (bool)master)
			{
				T component = master.GetComponent<T>();
				if ((bool)(Object)component)
				{
					return component;
				}
				component = master.GetComponentInParent<T>();
				if ((bool)(Object)component)
				{
					return component;
				}
				component = master.GetComponentInChildren<T>();
				if ((bool)(Object)component)
				{
					return component;
				}
				return Object.FindObjectOfType<T>();
			}
			if (_mode == Mode.ByReference)
			{
				return _reference;
			}
			if (_mode == Mode.ByName)
			{
				GameObject gameObject = GameObject.Find(_name);
				if ((bool)gameObject)
				{
					return gameObject.GetComponent<T>();
				}
			}
			return (T)null;
		}
	}
}
