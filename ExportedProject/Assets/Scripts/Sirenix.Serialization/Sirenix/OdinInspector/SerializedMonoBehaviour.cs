using Sirenix.Serialization;
using UnityEngine;

namespace Sirenix.OdinInspector
{
	[ShowOdinSerializedPropertiesInInspector]
	public abstract class SerializedMonoBehaviour : MonoBehaviour, ISerializationCallbackReceiver, ISupportsPrefabSerialization
	{
		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		private SerializationData serializationData;

		SerializationData ISupportsPrefabSerialization.SerializationData
		{
			get
			{
				return serializationData;
			}
			set
			{
				serializationData = value;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData);
			OnAfterDeserialize();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			UnitySerializationUtility.SerializeUnityObject(this, ref serializationData);
			OnBeforeSerialize();
		}

		protected virtual void OnAfterDeserialize()
		{
		}

		protected virtual void OnBeforeSerialize()
		{
		}
	}
}
