using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Sirenix.Serialization
{
	public static class UnitySerializationUtility
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Comparison<PrefabModification> _003C_003E9__21_0;

			internal int _003CSerializePrefabModifications_003Eb__21_0(PrefabModification a, PrefabModification b)
			{
				int num = a.Path.CompareTo(b.Path);
				if (num == 0)
				{
					if ((a.ModificationType == PrefabModificationType.ListLength || a.ModificationType == PrefabModificationType.Dictionary) && b.ModificationType == PrefabModificationType.Value)
					{
						return 1;
					}
					if (a.ModificationType == PrefabModificationType.Value && (b.ModificationType == PrefabModificationType.ListLength || b.ModificationType == PrefabModificationType.Dictionary))
					{
						return -1;
					}
				}
				return num;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass27_0
		{
			public MemberInfo member;

			internal object _003CGetCachedUnityMemberGetter_003Eb__0(ref object instance)
			{
				return FormatterUtilities.GetMemberValue(member, instance);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass28_0
		{
			public MemberInfo member;

			internal void _003CGetCachedUnityMemberSetter_003Eb__0(ref object instance, object value)
			{
				FormatterUtilities.SetMemberValue(member, instance, value);
			}
		}

		private static readonly Dictionary<DataFormat, IDataReader> UnityReaders = new Dictionary<DataFormat, IDataReader>();

		private static readonly Dictionary<DataFormat, IDataWriter> UnityWriters = new Dictionary<DataFormat, IDataWriter>();

		private static readonly Dictionary<MemberInfo, WeakValueGetter> UnityMemberGetters = new Dictionary<MemberInfo, WeakValueGetter>();

		private static readonly Dictionary<MemberInfo, WeakValueSetter> UnityMemberSetters = new Dictionary<MemberInfo, WeakValueSetter>();

		private static readonly Dictionary<MemberInfo, bool> UnityWillSerializeMembersCache = new Dictionary<MemberInfo, bool>();

		private static readonly Dictionary<Type, bool> UnityWillSerializeTypesCache = new Dictionary<Type, bool>();

		private static readonly HashSet<Type> UnityNeverSerializesTypes = new HashSet<Type> { typeof(Coroutine) };

		public static bool OdinWillSerialize(MemberInfo member, bool serializeUnityFields)
		{
			if (member is FieldInfo && member.HasCustomAttribute<OdinSerializeAttribute>())
			{
				return true;
			}
			if (GuessIfUnityWillSerialize(member))
			{
				return serializeUnityFields;
			}
			return SerializationPolicies.Unity.ShouldSerializeMember(member);
		}

		public static bool GuessIfUnityWillSerialize(MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			bool value;
			if (!UnityWillSerializeMembersCache.TryGetValue(member, out value))
			{
				value = GuessIfUnityWillSerializePrivate(member);
				UnityWillSerializeMembersCache[member] = value;
			}
			return value;
		}

		private static bool GuessIfUnityWillSerializePrivate(MemberInfo member)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo == null)
			{
				return false;
			}
			if (!typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType) && fieldInfo.FieldType == fieldInfo.DeclaringType)
			{
				return false;
			}
			if (fieldInfo.IsDefined<NonSerializedAttribute>() || (!fieldInfo.IsPublic && !fieldInfo.IsDefined<SerializeField>()))
			{
				return false;
			}
			if (fieldInfo.IsDefined<FixedBufferAttribute>())
			{
				return UnityVersion.IsVersionOrGreater(2017, 1);
			}
			return GuessIfUnityWillSerialize(fieldInfo.FieldType);
		}

		public static bool GuessIfUnityWillSerialize(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool value;
			if (!UnityWillSerializeTypesCache.TryGetValue(type, out value))
			{
				value = GuessIfUnityWillSerializePrivate(type);
				UnityWillSerializeTypesCache[type] = value;
			}
			return value;
		}

		private static bool GuessIfUnityWillSerializePrivate(Type type)
		{
			if (UnityNeverSerializesTypes.Contains(type))
			{
				return false;
			}
			if (typeof(UnityEngine.Object).IsAssignableFrom(type) && type.GetGenericArguments().Length == 0)
			{
				return true;
			}
			if (type.IsAbstract || type.IsInterface || type == typeof(object))
			{
				return false;
			}
			if (type.IsEnum)
			{
				Type underlyingType = Enum.GetUnderlyingType(type);
				if (UnityVersion.IsVersionOrGreater(5, 6))
				{
					if (underlyingType != typeof(long))
					{
						return underlyingType != typeof(ulong);
					}
					return false;
				}
				if (underlyingType != typeof(int))
				{
					return underlyingType == typeof(byte);
				}
				return true;
			}
			if (type.IsPrimitive || type == typeof(string))
			{
				return true;
			}
			if (typeof(Delegate).IsAssignableFrom(type))
			{
				return false;
			}
			if (typeof(UnityEventBase).IsAssignableFrom(type))
			{
				return !type.IsGenericType;
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() == 1 && !type.GetElementType().IsArray)
				{
					return GuessIfUnityWillSerialize(type.GetElementType());
				}
				return false;
			}
			if (type.Assembly.FullName.StartsWith("UnityEngine", StringComparison.InvariantCulture) || type.Assembly.FullName.StartsWith("UnityEditor", StringComparison.InvariantCulture))
			{
				return true;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			{
				Type type2 = type.GetArgumentsOfInheritedOpenGenericClass(typeof(List<>))[0];
				if (type2.IsArray || type2.InheritsFrom(typeof(IList<>)))
				{
					return false;
				}
				return GuessIfUnityWillSerialize(type2);
			}
			if (type.IsGenericType)
			{
				return false;
			}
			if (type.IsDefined<SerializableAttribute>(false) && type != typeof(Guid))
			{
				if (UnityVersion.IsVersionOrGreater(4, 5))
				{
					return true;
				}
				return type.IsClass;
			}
			Type baseType = type.BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().FullName == "UnityEngine.Networking.SyncListStruct`1")
				{
					return true;
				}
				baseType = baseType.BaseType;
			}
			return false;
		}

		public static void SerializeUnityObject(UnityEngine.Object unityObject, ref SerializationData data, bool serializeUnityFields = false, SerializationContext context = null)
		{
			if (unityObject == null)
			{
				throw new ArgumentNullException("unityObject");
			}
			IOverridesSerializationFormat overridesSerializationFormat = unityObject as IOverridesSerializationFormat;
			DataFormat dataFormat = ((overridesSerializationFormat != null) ? overridesSerializationFormat.GetFormatToSerializeAs(true) : (GlobalConfig<GlobalSerializationConfig>.HasInstanceLoaded ? GlobalConfig<GlobalSerializationConfig>.Instance.BuildSerializationFormat : DataFormat.Binary));
			if (dataFormat == DataFormat.Nodes)
			{
				Debug.LogWarning("The serialization format '" + dataFormat.ToString() + "' is disabled outside of the editor. Defaulting to the format '" + DataFormat.Binary.ToString() + "' instead.");
				dataFormat = DataFormat.Binary;
			}
			SerializeUnityObject(unityObject, ref data.SerializedBytes, ref data.ReferencedUnityObjects, dataFormat);
			data.SerializedFormat = dataFormat;
		}

		public static void SerializeUnityObject(UnityEngine.Object unityObject, ref string base64Bytes, ref List<UnityEngine.Object> referencedUnityObjects, DataFormat format, bool serializeUnityFields = false, SerializationContext context = null)
		{
			byte[] bytes = null;
			SerializeUnityObject(unityObject, ref bytes, ref referencedUnityObjects, format, serializeUnityFields, context);
			base64Bytes = Convert.ToBase64String(bytes);
		}

		public static void SerializeUnityObject(UnityEngine.Object unityObject, ref byte[] bytes, ref List<UnityEngine.Object> referencedUnityObjects, DataFormat format, bool serializeUnityFields = false, SerializationContext context = null)
		{
			if (unityObject == null)
			{
				throw new ArgumentNullException("unityObject");
			}
			if (format == DataFormat.Nodes)
			{
				Debug.LogError("The serialization data format '" + format.ToString() + "' is not supported by this method. You must create your own writer.");
				return;
			}
			if (referencedUnityObjects == null)
			{
				referencedUnityObjects = new List<UnityEngine.Object>();
			}
			else
			{
				referencedUnityObjects.Clear();
			}
			using (Cache<CachedMemoryStream> cache2 = Cache<CachedMemoryStream>.Claim())
			{
				using (Cache<UnityReferenceResolver> cache = Cache<UnityReferenceResolver>.Claim())
				{
					cache.Value.SetReferencedUnityObjects(referencedUnityObjects);
					if (context != null)
					{
						context.IndexReferenceResolver = cache.Value;
						SerializeUnityObject(unityObject, GetCachedUnityWriter(format, cache2.Value.MemoryStream, context), serializeUnityFields);
					}
					else
					{
						using (Cache<SerializationContext> cache3 = Cache<SerializationContext>.Claim())
						{
							cache3.Value.Config.SerializationPolicy = SerializationPolicies.Unity;
							if (GlobalConfig<GlobalSerializationConfig>.HasInstanceLoaded)
							{
								cache3.Value.Config.DebugContext.ErrorHandlingPolicy = GlobalConfig<GlobalSerializationConfig>.Instance.ErrorHandlingPolicy;
								cache3.Value.Config.DebugContext.LoggingPolicy = GlobalConfig<GlobalSerializationConfig>.Instance.LoggingPolicy;
								cache3.Value.Config.DebugContext.Logger = GlobalConfig<GlobalSerializationConfig>.Instance.Logger;
							}
							else
							{
								cache3.Value.Config.DebugContext.ErrorHandlingPolicy = ErrorHandlingPolicy.Resilient;
								cache3.Value.Config.DebugContext.LoggingPolicy = LoggingPolicy.LogErrors;
								cache3.Value.Config.DebugContext.Logger = DefaultLoggers.UnityLogger;
							}
							cache3.Value.IndexReferenceResolver = cache.Value;
							SerializeUnityObject(unityObject, GetCachedUnityWriter(format, cache2.Value.MemoryStream, cache3), serializeUnityFields);
						}
					}
					bytes = cache2.Value.MemoryStream.ToArray();
				}
			}
		}

		public static void SerializeUnityObject(UnityEngine.Object unityObject, IDataWriter writer, bool serializeUnityFields = false)
		{
			if (unityObject == null)
			{
				throw new ArgumentNullException("unityObject");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			try
			{
				writer.PrepareNewSerializationSession();
				MemberInfo[] serializableMembers = FormatterUtilities.GetSerializableMembers(unityObject.GetType(), writer.Context.Config.SerializationPolicy);
				object instance = unityObject;
				foreach (MemberInfo memberInfo in serializableMembers)
				{
					WeakValueGetter weakValueGetter = null;
					if (!OdinWillSerialize(memberInfo, serializeUnityFields) || (weakValueGetter = GetCachedUnityMemberGetter(memberInfo)) == null)
					{
						continue;
					}
					object obj = weakValueGetter(ref instance);
					bool flag = obj == null;
					if (flag || obj.GetType() != typeof(SerializationData))
					{
						Serializer serializer = ((flag || !FormatterUtilities.IsPrimitiveType(obj.GetType()) || FormatterUtilities.GetContainedType(memberInfo) != typeof(object)) ? Serializer.GetForValue(obj) : Serializer.Get<object>());
						try
						{
							serializer.WriteValueWeak(memberInfo.Name, obj, writer);
						}
						catch (Exception exception)
						{
							writer.Context.Config.DebugContext.LogException(exception);
						}
					}
				}
				writer.FlushToStream();
			}
			catch (SerializationAbortException innerException)
			{
				throw new SerializationAbortException("Serialization of type '" + unityObject.GetType().GetNiceFullName() + "' aborted.", innerException);
			}
			catch (Exception ex)
			{
				Debug.LogException(new Exception("Exception thrown while serializing type '" + unityObject.GetType().GetNiceFullName() + "': " + ex.Message, ex));
			}
		}

		public static void DeserializeUnityObject(UnityEngine.Object unityObject, ref SerializationData data, DeserializationContext context = null)
		{
			DeserializeUnityObject(unityObject, ref data, context, false, null);
		}

		private static void DeserializeUnityObject(UnityEngine.Object unityObject, ref SerializationData data, DeserializationContext context, bool isPrefabData, List<UnityEngine.Object> prefabInstanceUnityObjects)
		{
			if (unityObject == null)
			{
				throw new ArgumentNullException("unityObject");
			}
			if (isPrefabData && prefabInstanceUnityObjects == null)
			{
				throw new ArgumentNullException("prefabInstanceUnityObjects", "prefabInstanceUnityObjects cannot be null when isPrefabData is true.");
			}
			if (data.SerializedBytes != null && data.SerializedBytes.Length != 0 && (data.SerializationNodes == null || data.SerializationNodes.Count == 0))
			{
				if (data.SerializedFormat == DataFormat.Nodes)
				{
					DataFormat dataFormat = ((data.SerializedBytes[0] == 123) ? DataFormat.JSON : DataFormat.Binary);
					try
					{
						string text = ProperBitConverter.BytesToHexString(data.SerializedBytes);
						Debug.LogWarning(string.Concat("Serialization data has only bytes stored, but the serialized format is marked as being 'Nodes', which is incompatible with data stored as a byte array. Based on the appearance of the serialized bytes, Odin has guessed that the data format is '", dataFormat, "', and will attempt to deserialize the bytes using that format. The serialized bytes follow, converted to a hex string: ", text));
					}
					catch
					{
					}
					DeserializeUnityObject(unityObject, ref data.SerializedBytes, ref data.ReferencedUnityObjects, dataFormat, context);
				}
				else
				{
					DeserializeUnityObject(unityObject, ref data.SerializedBytes, ref data.ReferencedUnityObjects, data.SerializedFormat, context);
				}
				return;
			}
			Cache<DeserializationContext> cache = null;
			try
			{
				if (context == null)
				{
					cache = Cache<DeserializationContext>.Claim();
					context = cache;
					context.Config.SerializationPolicy = SerializationPolicies.Unity;
					if (GlobalConfig<GlobalSerializationConfig>.HasInstanceLoaded)
					{
						context.Config.DebugContext.ErrorHandlingPolicy = GlobalConfig<GlobalSerializationConfig>.Instance.ErrorHandlingPolicy;
						context.Config.DebugContext.LoggingPolicy = GlobalConfig<GlobalSerializationConfig>.Instance.LoggingPolicy;
						context.Config.DebugContext.Logger = GlobalConfig<GlobalSerializationConfig>.Instance.Logger;
					}
					else
					{
						context.Config.DebugContext.ErrorHandlingPolicy = ErrorHandlingPolicy.Resilient;
						context.Config.DebugContext.LoggingPolicy = LoggingPolicy.LogErrors;
						context.Config.DebugContext.Logger = DefaultLoggers.UnityLogger;
					}
				}
				if (!isPrefabData && !data.Prefab.SafeIsUnityNull())
				{
					if (data.Prefab is ISupportsPrefabSerialization)
					{
						if ((object)data.Prefab != unityObject || data.PrefabModifications == null || data.PrefabModifications.Count <= 0)
						{
							SerializationData data2 = (data.Prefab as ISupportsPrefabSerialization).SerializationData;
							if (!data2.HasEditorData)
							{
								DeserializeUnityObject(unityObject, ref data, context, true, data.ReferencedUnityObjects);
							}
							else
							{
								DeserializeUnityObject(unityObject, ref data2, context, true, data.ReferencedUnityObjects);
							}
							ApplyPrefabModifications(unityObject, data.PrefabModifications, data.PrefabModificationsReferencedUnityObjects);
							return;
						}
					}
					else if (data.Prefab.GetType() != typeof(UnityEngine.Object))
					{
						Debug.LogWarning("The type " + data.Prefab.GetType().GetNiceName() + " no longer supports special prefab serialization (the interface " + typeof(ISupportsPrefabSerialization).GetNiceName() + ") upon deserialization of an instance of a prefab; prefab data may be lost. Has a type been lost?");
					}
				}
				List<UnityEngine.Object> referencedUnityObjects = (isPrefabData ? prefabInstanceUnityObjects : data.ReferencedUnityObjects);
				if (data.SerializedFormat == DataFormat.Nodes)
				{
					using (SerializationNodeDataReader serializationNodeDataReader = new SerializationNodeDataReader(context))
					{
						using (Cache<UnityReferenceResolver> cache2 = Cache<UnityReferenceResolver>.Claim())
						{
							cache2.Value.SetReferencedUnityObjects(referencedUnityObjects);
							context.IndexReferenceResolver = cache2.Value;
							serializationNodeDataReader.Nodes = data.SerializationNodes;
							DeserializeUnityObject(unityObject, serializationNodeDataReader);
						}
					}
				}
				else
				{
					DeserializeUnityObject(unityObject, ref data.SerializedBytesString, ref referencedUnityObjects, data.SerializedFormat, context);
				}
				if (data.PrefabModifications != null && data.PrefabModifications.Count > 0)
				{
					ApplyPrefabModifications(unityObject, data.PrefabModifications, data.PrefabModificationsReferencedUnityObjects);
				}
			}
			finally
			{
				if (cache != null)
				{
					Cache<DeserializationContext>.Release(cache);
				}
			}
		}

		public static void DeserializeUnityObject(UnityEngine.Object unityObject, ref string base64Bytes, ref List<UnityEngine.Object> referencedUnityObjects, DataFormat format, DeserializationContext context = null)
		{
			if (!string.IsNullOrEmpty(base64Bytes))
			{
				byte[] bytes = Convert.FromBase64String(base64Bytes);
				DeserializeUnityObject(unityObject, ref bytes, ref referencedUnityObjects, format, context);
			}
		}

		public static void DeserializeUnityObject(UnityEngine.Object unityObject, ref byte[] bytes, ref List<UnityEngine.Object> referencedUnityObjects, DataFormat format, DeserializationContext context = null)
		{
			if (unityObject == null)
			{
				throw new ArgumentNullException("unityObject");
			}
			if (bytes == null || bytes.Length == 0)
			{
				return;
			}
			if (format == DataFormat.Nodes)
			{
				try
				{
					Debug.LogError("The serialization data format '" + format.ToString() + "' is not supported by this method. You must create your own reader.");
					return;
				}
				catch
				{
					return;
				}
			}
			if (referencedUnityObjects == null)
			{
				referencedUnityObjects = new List<UnityEngine.Object>();
			}
			using (Cache<CachedMemoryStream> cache = Cache<CachedMemoryStream>.Claim())
			{
				using (Cache<UnityReferenceResolver> cache2 = Cache<UnityReferenceResolver>.Claim())
				{
					cache.Value.MemoryStream.Write(bytes, 0, bytes.Length);
					cache.Value.MemoryStream.Position = 0L;
					cache2.Value.SetReferencedUnityObjects(referencedUnityObjects);
					if (context != null)
					{
						context.IndexReferenceResolver = cache2.Value;
						DeserializeUnityObject(unityObject, GetCachedUnityReader(format, cache.Value.MemoryStream, context));
						return;
					}
					using (Cache<DeserializationContext> cache3 = Cache<DeserializationContext>.Claim())
					{
						cache3.Value.Config.SerializationPolicy = SerializationPolicies.Unity;
						if (GlobalConfig<GlobalSerializationConfig>.HasInstanceLoaded)
						{
							cache3.Value.Config.DebugContext.ErrorHandlingPolicy = GlobalConfig<GlobalSerializationConfig>.Instance.ErrorHandlingPolicy;
							cache3.Value.Config.DebugContext.LoggingPolicy = GlobalConfig<GlobalSerializationConfig>.Instance.LoggingPolicy;
							cache3.Value.Config.DebugContext.Logger = GlobalConfig<GlobalSerializationConfig>.Instance.Logger;
						}
						else
						{
							cache3.Value.Config.DebugContext.ErrorHandlingPolicy = ErrorHandlingPolicy.Resilient;
							cache3.Value.Config.DebugContext.LoggingPolicy = LoggingPolicy.LogErrors;
							cache3.Value.Config.DebugContext.Logger = DefaultLoggers.UnityLogger;
						}
						cache3.Value.IndexReferenceResolver = cache2.Value;
						DeserializeUnityObject(unityObject, GetCachedUnityReader(format, cache.Value.MemoryStream, cache3));
					}
				}
			}
		}

		public static void DeserializeUnityObject(UnityEngine.Object unityObject, IDataReader reader)
		{
			if (unityObject == null)
			{
				throw new ArgumentNullException("unityObject");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			try
			{
				reader.PrepareNewSerializationSession();
				Dictionary<string, MemberInfo> serializableMembersMap = FormatterUtilities.GetSerializableMembersMap(unityObject.GetType(), reader.Context.Config.SerializationPolicy);
				int num = 0;
				object instance = unityObject;
				EntryType entryType;
				string name;
				while ((entryType = reader.PeekEntry(out name)) != EntryType.EndOfNode && entryType != EntryType.EndOfArray && entryType != EntryType.EndOfStream)
				{
					MemberInfo value = null;
					WeakValueSetter weakValueSetter = null;
					bool flag = false;
					if (string.IsNullOrEmpty(name))
					{
						reader.Context.Config.DebugContext.LogError(string.Concat("Entry of type \"", entryType, "\" in node \"", reader.CurrentNodeName, "\" is missing a name."));
						flag = true;
					}
					else if (!serializableMembersMap.TryGetValue(name, out value) || (weakValueSetter = GetCachedUnityMemberSetter(value)) == null)
					{
						flag = true;
					}
					if (flag)
					{
						reader.SkipEntry();
						continue;
					}
					Type containedType = FormatterUtilities.GetContainedType(value);
					Serializer serializer = Serializer.Get(containedType);
					try
					{
						object value2 = serializer.ReadValueWeak(reader);
						weakValueSetter(ref instance, value2);
					}
					catch (Exception exception)
					{
						reader.Context.Config.DebugContext.LogException(exception);
					}
					num++;
					if (num <= 1000)
					{
						continue;
					}
					reader.Context.Config.DebugContext.LogError("Breaking out of infinite reading loop! (Read more than a thousand entries for one type!)");
					break;
				}
			}
			catch (SerializationAbortException innerException)
			{
				throw new SerializationAbortException("Deserialization of type '" + unityObject.GetType().GetNiceFullName() + "' aborted.", innerException);
			}
			catch (Exception ex)
			{
				Debug.LogException(new Exception("Exception thrown while deserializing type '" + unityObject.GetType().GetNiceFullName() + "': " + ex.Message, ex));
			}
		}

		public static List<string> SerializePrefabModifications(List<PrefabModification> modifications, ref List<UnityEngine.Object> referencedUnityObjects)
		{
			if (modifications == null || modifications.Count == 0)
			{
				return new List<string>();
			}
			if (referencedUnityObjects == null)
			{
				referencedUnityObjects = new List<UnityEngine.Object>();
			}
			else if (referencedUnityObjects.Count > 0)
			{
				referencedUnityObjects.Clear();
			}
			modifications.Sort(_003C_003Ec._003C_003E9__21_0 ?? (_003C_003Ec._003C_003E9__21_0 = _003C_003Ec._003C_003E9._003CSerializePrefabModifications_003Eb__21_0));
			List<string> list = new List<string>();
			using (Cache<SerializationContext> cache = Cache<SerializationContext>.Claim())
			{
				using (MemoryStream stream = new MemoryStream())
				{
					using (JsonDataWriter jsonDataWriter = (JsonDataWriter)GetCachedUnityWriter(DataFormat.JSON, stream, cache))
					{
						using (Cache<UnityReferenceResolver> cache2 = Cache<UnityReferenceResolver>.Claim())
						{
							jsonDataWriter.PrepareNewSerializationSession();
							jsonDataWriter.FormatAsReadable = false;
							jsonDataWriter.EnableTypeOptimization = false;
							cache2.Value.SetReferencedUnityObjects(referencedUnityObjects);
							jsonDataWriter.Context.IndexReferenceResolver = cache2.Value;
							for (int i = 0; i < modifications.Count; i++)
							{
								PrefabModification prefabModification = modifications[i];
								if (prefabModification.ModificationType == PrefabModificationType.ListLength)
								{
									jsonDataWriter.MarkJustStarted();
									jsonDataWriter.WriteString("path", prefabModification.Path);
									jsonDataWriter.WriteInt32("length", prefabModification.NewLength);
									jsonDataWriter.FlushToStream();
									list.Add(GetStringFromStreamAndReset(stream));
								}
								else if (prefabModification.ModificationType == PrefabModificationType.Value)
								{
									jsonDataWriter.MarkJustStarted();
									jsonDataWriter.WriteString("path", prefabModification.Path);
									if (prefabModification.ReferencePaths != null && prefabModification.ReferencePaths.Count > 0)
									{
										jsonDataWriter.BeginStructNode("references", null);
										for (int j = 0; j < prefabModification.ReferencePaths.Count; j++)
										{
											jsonDataWriter.WriteString(null, prefabModification.ReferencePaths[j]);
										}
										jsonDataWriter.EndNode("references");
									}
									Serializer<object> serializer = Serializer.Get<object>();
									serializer.WriteValueWeak("value", prefabModification.ModifiedValue, jsonDataWriter);
									jsonDataWriter.FlushToStream();
									list.Add(GetStringFromStreamAndReset(stream));
								}
								else if (prefabModification.ModificationType == PrefabModificationType.Dictionary)
								{
									jsonDataWriter.MarkJustStarted();
									jsonDataWriter.WriteString("path", prefabModification.Path);
									Serializer.Get<object[]>().WriteValue("add_keys", prefabModification.DictionaryKeysAdded, jsonDataWriter);
									Serializer.Get<object[]>().WriteValue("remove_keys", prefabModification.DictionaryKeysRemoved, jsonDataWriter);
									jsonDataWriter.FlushToStream();
									list.Add(GetStringFromStreamAndReset(stream));
								}
								jsonDataWriter.Context.ResetInternalReferences();
							}
							return list;
						}
					}
				}
			}
		}

		private static string GetStringFromStreamAndReset(Stream stream)
		{
			byte[] array = new byte[stream.Position];
			stream.Position = 0L;
			stream.Read(array, 0, array.Length);
			stream.Position = 0L;
			return Encoding.UTF8.GetString(array);
		}

		public static List<PrefabModification> DeserializePrefabModifications(List<string> modifications, List<UnityEngine.Object> referencedUnityObjects)
		{
			if (modifications == null || modifications.Count == 0)
			{
				return new List<PrefabModification>();
			}
			List<PrefabModification> list = new List<PrefabModification>();
			int num = 0;
			for (int i = 0; i < modifications.Count; i++)
			{
				int num2 = modifications[i].Length * 2;
				if (num2 > num)
				{
					num = num2;
				}
			}
			using (Cache<DeserializationContext> cache = Cache<DeserializationContext>.Claim())
			{
				using (MemoryStream memoryStream = new MemoryStream(num))
				{
					using (JsonDataReader jsonDataReader = (JsonDataReader)GetCachedUnityReader(DataFormat.JSON, memoryStream, cache))
					{
						using (Cache<UnityReferenceResolver> cache2 = Cache<UnityReferenceResolver>.Claim())
						{
							cache2.Value.SetReferencedUnityObjects(referencedUnityObjects);
							jsonDataReader.Context.IndexReferenceResolver = cache2.Value;
							for (int j = 0; j < modifications.Count; j++)
							{
								string text = modifications[j];
								byte[] bytes = Encoding.UTF8.GetBytes(text);
								memoryStream.SetLength(bytes.Length);
								memoryStream.Position = 0L;
								memoryStream.Write(bytes, 0, bytes.Length);
								memoryStream.Position = 0L;
								PrefabModification prefabModification = new PrefabModification();
								jsonDataReader.PrepareNewSerializationSession();
								string name;
								EntryType entryType = jsonDataReader.PeekEntry(out name);
								if (entryType == EntryType.EndOfStream)
								{
									jsonDataReader.SkipEntry();
								}
								while ((entryType = jsonDataReader.PeekEntry(out name)) != EntryType.EndOfNode && entryType != EntryType.EndOfArray && entryType != EntryType.EndOfStream)
								{
									if (name == null)
									{
										Debug.LogError(string.Concat("Unexpected entry of type ", entryType, " without a name."));
										jsonDataReader.SkipEntry();
									}
									else if (name.Equals("path", StringComparison.InvariantCultureIgnoreCase))
									{
										jsonDataReader.ReadString(out prefabModification.Path);
									}
									else if (name.Equals("length", StringComparison.InvariantCultureIgnoreCase))
									{
										jsonDataReader.ReadInt32(out prefabModification.NewLength);
										prefabModification.ModificationType = PrefabModificationType.ListLength;
									}
									else if (name.Equals("references", StringComparison.InvariantCultureIgnoreCase))
									{
										prefabModification.ReferencePaths = new List<string>();
										Type type;
										jsonDataReader.EnterNode(out type);
										while (jsonDataReader.PeekEntry(out name) == EntryType.String)
										{
											string value;
											jsonDataReader.ReadString(out value);
											prefabModification.ReferencePaths.Add(value);
										}
										jsonDataReader.ExitNode();
									}
									else if (name.Equals("value", StringComparison.InvariantCultureIgnoreCase))
									{
										prefabModification.ModifiedValue = Serializer.Get<object>().ReadValue(jsonDataReader);
										prefabModification.ModificationType = PrefabModificationType.Value;
									}
									else if (name.Equals("add_keys", StringComparison.InvariantCultureIgnoreCase))
									{
										prefabModification.DictionaryKeysAdded = Serializer.Get<object[]>().ReadValue(jsonDataReader);
										prefabModification.ModificationType = PrefabModificationType.Dictionary;
									}
									else if (name.Equals("remove_keys", StringComparison.InvariantCultureIgnoreCase))
									{
										prefabModification.DictionaryKeysRemoved = Serializer.Get<object[]>().ReadValue(jsonDataReader);
										prefabModification.ModificationType = PrefabModificationType.Dictionary;
									}
									else
									{
										Debug.LogError("Unexpected entry name '" + name + "' while deserializing prefab modifications.");
										jsonDataReader.SkipEntry();
									}
								}
								if (prefabModification.Path == null)
								{
									Debug.LogWarning("Error when deserializing prefab modification; no path found. Modification lost; string was: '" + text + "'.");
								}
								else
								{
									list.Add(prefabModification);
								}
							}
							return list;
						}
					}
				}
			}
		}

		public static object CreateDefaultUnityInitializedObject(Type type)
		{
			return CreateDefaultUnityInitializedObject(type, 0);
		}

		private static object CreateDefaultUnityInitializedObject(Type type, int depth)
		{
			if (depth > 10)
			{
				return null;
			}
			if (!GuessIfUnityWillSerialize(type))
			{
				if (!type.IsValueType)
				{
					return null;
				}
				return Activator.CreateInstance(type);
			}
			if (type == typeof(string))
			{
				return "";
			}
			if (type.IsEnum)
			{
				Array values = Enum.GetValues(type);
				if (values.Length <= 0)
				{
					return Enum.ToObject(type, 0);
				}
				return values.GetValue(0);
			}
			if (type.IsPrimitive)
			{
				return Activator.CreateInstance(type);
			}
			if (type.IsArray)
			{
				return Array.CreateInstance(type.GetElementType(), 0);
			}
			if (type.ImplementsOpenGenericClass(typeof(List<>)) || typeof(UnityEventBase).IsAssignableFrom(type))
			{
				try
				{
					return Activator.CreateInstance(type);
				}
				catch
				{
					return null;
				}
			}
			if (typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				return null;
			}
			if ((type.Assembly.GetName().Name.StartsWith("UnityEngine") || type.Assembly.GetName().Name.StartsWith("UnityEditor")) && type.GetConstructor(Type.EmptyTypes) != null)
			{
				try
				{
					return Activator.CreateInstance(type);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					return null;
				}
			}
			object obj2 = ((type.GetConstructor(Type.EmptyTypes) == null) ? FormatterServices.GetUninitializedObject(type) : Activator.CreateInstance(type));
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (GuessIfUnityWillSerialize(fieldInfo))
				{
					fieldInfo.SetValue(obj2, CreateDefaultUnityInitializedObject(fieldInfo.FieldType, depth + 1));
				}
			}
			return obj2;
		}

		private static void ApplyPrefabModifications(UnityEngine.Object unityObject, List<string> modificationData, List<UnityEngine.Object> referencedUnityObjects)
		{
			if (unityObject == null)
			{
				throw new ArgumentNullException("unityObject");
			}
			if (modificationData == null || modificationData.Count == 0)
			{
				return;
			}
			List<PrefabModification> list = DeserializePrefabModifications(modificationData, referencedUnityObjects);
			for (int i = 0; i < list.Count; i++)
			{
				PrefabModification prefabModification = list[i];
				try
				{
					prefabModification.Apply(unityObject);
				}
				catch (Exception exception)
				{
					Debug.Log("The following exception was thrown when trying to apply a prefab modification for path '" + prefabModification.Path + "':");
					Debug.LogException(exception);
				}
			}
		}

		private static WeakValueGetter GetCachedUnityMemberGetter(MemberInfo member)
		{
			_003C_003Ec__DisplayClass27_0 _003C_003Ec__DisplayClass27_ = new _003C_003Ec__DisplayClass27_0();
			_003C_003Ec__DisplayClass27_.member = member;
			WeakValueGetter value;
			if (!UnityMemberGetters.TryGetValue(_003C_003Ec__DisplayClass27_.member, out value))
			{
				value = ((_003C_003Ec__DisplayClass27_.member is FieldInfo) ? EmitUtilities.CreateWeakInstanceFieldGetter(_003C_003Ec__DisplayClass27_.member.DeclaringType, _003C_003Ec__DisplayClass27_.member as FieldInfo) : ((!(_003C_003Ec__DisplayClass27_.member is PropertyInfo)) ? new WeakValueGetter(_003C_003Ec__DisplayClass27_._003CGetCachedUnityMemberGetter_003Eb__0) : EmitUtilities.CreateWeakInstancePropertyGetter(_003C_003Ec__DisplayClass27_.member.DeclaringType, _003C_003Ec__DisplayClass27_.member as PropertyInfo)));
				UnityMemberGetters.Add(_003C_003Ec__DisplayClass27_.member, value);
			}
			return value;
		}

		private static WeakValueSetter GetCachedUnityMemberSetter(MemberInfo member)
		{
			_003C_003Ec__DisplayClass28_0 _003C_003Ec__DisplayClass28_ = new _003C_003Ec__DisplayClass28_0();
			_003C_003Ec__DisplayClass28_.member = member;
			WeakValueSetter value;
			if (!UnityMemberSetters.TryGetValue(_003C_003Ec__DisplayClass28_.member, out value))
			{
				value = ((_003C_003Ec__DisplayClass28_.member is FieldInfo) ? EmitUtilities.CreateWeakInstanceFieldSetter(_003C_003Ec__DisplayClass28_.member.DeclaringType, _003C_003Ec__DisplayClass28_.member as FieldInfo) : ((!(_003C_003Ec__DisplayClass28_.member is PropertyInfo)) ? new WeakValueSetter(_003C_003Ec__DisplayClass28_._003CGetCachedUnityMemberSetter_003Eb__0) : EmitUtilities.CreateWeakInstancePropertySetter(_003C_003Ec__DisplayClass28_.member.DeclaringType, _003C_003Ec__DisplayClass28_.member as PropertyInfo)));
				UnityMemberSetters.Add(_003C_003Ec__DisplayClass28_.member, value);
			}
			return value;
		}

		private static IDataWriter GetCachedUnityWriter(DataFormat format, Stream stream, SerializationContext context)
		{
			IDataWriter value;
			if (!UnityWriters.TryGetValue(format, out value))
			{
				value = SerializationUtility.CreateWriter(stream, context, format);
				UnityWriters.Add(format, value);
			}
			else
			{
				value.Context = context;
				value.Stream = stream;
			}
			return value;
		}

		private static IDataReader GetCachedUnityReader(DataFormat format, Stream stream, DeserializationContext context)
		{
			IDataReader value;
			if (!UnityReaders.TryGetValue(format, out value))
			{
				value = SerializationUtility.CreateReader(stream, context, format);
				UnityReaders.Add(format, value);
			}
			else
			{
				value.Context = context;
				value.Stream = stream;
			}
			return value;
		}
	}
}
