using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	[Serializable]
	public struct SerializationData
	{
		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		public DataFormat SerializedFormat;

		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		public byte[] SerializedBytes;

		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		public List<UnityEngine.Object> ReferencedUnityObjects;

		public const string PrefabModificationsReferencedUnityObjectsFieldName = "PrefabModificationsReferencedUnityObjects";

		public const string PrefabModificationsFieldName = "PrefabModifications";

		public const string PrefabFieldName = "Prefab";

		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		public string SerializedBytesString;

		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		public UnityEngine.Object Prefab;

		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		public List<UnityEngine.Object> PrefabModificationsReferencedUnityObjects;

		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		public List<string> PrefabModifications;

		[SerializeField]
		[HideInInspector]
		[ExcludeDataFromInspector]
		public List<SerializationNode> SerializationNodes;

		public bool HasEditorData
		{
			get
			{
				switch (SerializedFormat)
				{
				case DataFormat.Binary:
				case DataFormat.JSON:
					if (SerializedBytesString.IsNullOrWhitespace())
					{
						return !SerializedBytes.IsNullOrEmpty();
					}
					return true;
				case DataFormat.Nodes:
					return !SerializationNodes.IsNullOrEmpty();
				default:
					throw new NotImplementedException(SerializedFormat.ToString());
				}
			}
		}

		public void Reset()
		{
			SerializedFormat = DataFormat.Binary;
			if (SerializedBytes != null && SerializedBytes.Length != 0)
			{
				SerializedBytes = new byte[0];
			}
			if (ReferencedUnityObjects != null && ReferencedUnityObjects.Count > 0)
			{
				ReferencedUnityObjects.Clear();
			}
			Prefab = null;
			if (SerializationNodes != null && SerializationNodes.Count > 0)
			{
				SerializationNodes.Clear();
			}
			if (SerializedBytesString != null && SerializedBytesString.Length > 0)
			{
				SerializedBytesString = string.Empty;
			}
			if (PrefabModificationsReferencedUnityObjects != null && PrefabModificationsReferencedUnityObjects.Count > 0)
			{
				PrefabModificationsReferencedUnityObjects.Clear();
			}
			if (PrefabModifications != null && PrefabModifications.Count > 0)
			{
				PrefabModifications.Clear();
			}
		}
	}
}
