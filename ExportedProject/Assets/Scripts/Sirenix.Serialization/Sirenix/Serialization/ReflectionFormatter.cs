using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sirenix.Serialization
{
	public class ReflectionFormatter<T> : BaseFormatter<T>
	{
		protected override void DeserializeImplementation(ref T value, IDataReader reader)
		{
			object obj = value;
			Dictionary<string, MemberInfo> serializableMembersMap = FormatterUtilities.GetSerializableMembersMap(typeof(T), reader.Context.Config.SerializationPolicy);
			EntryType entryType;
			string name;
			while ((entryType = reader.PeekEntry(out name)) != EntryType.EndOfNode && entryType != EntryType.EndOfArray && entryType != EntryType.EndOfStream)
			{
				MemberInfo value2;
				if (string.IsNullOrEmpty(name))
				{
					reader.Context.Config.DebugContext.LogError(string.Concat("Entry of type \"", entryType, "\" in node \"", reader.CurrentNodeName, "\" is missing a name."));
					reader.SkipEntry();
				}
				else if (!serializableMembersMap.TryGetValue(name, out value2))
				{
					reader.Context.Config.DebugContext.LogWarning(string.Concat("Lost serialization data for entry \"", name, "\" of type \"", entryType, "\"in node \"", reader.CurrentNodeName, "\"."));
					reader.SkipEntry();
				}
				else
				{
					Type containedType = FormatterUtilities.GetContainedType(value2);
					try
					{
						Serializer serializer = Serializer.Get(containedType);
						object value3 = serializer.ReadValueWeak(reader);
						FormatterUtilities.SetMemberValue(value2, obj, value3);
					}
					catch (Exception exception)
					{
						reader.Context.Config.DebugContext.LogException(exception);
					}
				}
			}
			value = (T)obj;
		}

		protected override void SerializeImplementation(ref T value, IDataWriter writer)
		{
			MemberInfo[] serializableMembers = FormatterUtilities.GetSerializableMembers(typeof(T), writer.Context.Config.SerializationPolicy);
			foreach (MemberInfo memberInfo in serializableMembers)
			{
				object memberValue = FormatterUtilities.GetMemberValue(memberInfo, value);
				Type type = ((memberValue != null) ? memberValue.GetType() : FormatterUtilities.GetContainedType(memberInfo));
				Serializer serializer = Serializer.Get(type);
				try
				{
					serializer.WriteValueWeak(memberInfo.Name, memberValue, writer);
				}
				catch (Exception exception)
				{
					writer.Context.Config.DebugContext.LogException(exception);
				}
			}
		}
	}
}
