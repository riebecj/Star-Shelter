using System;
using Sirenix.Utilities;

namespace Sirenix.Serialization
{
	public sealed class ComplexTypeSerializer<T> : Serializer<T>
	{
		private static readonly bool ComplexTypeIsObject = typeof(T) == typeof(object);

		private static readonly bool ComplexTypeIsAbstract = typeof(T).IsAbstract || typeof(T).IsInterface;

		private static readonly bool ComplexTypeIsNullable = typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>);

		public override T ReadValue(IDataReader reader)
		{
			DeserializationContext context = reader.Context;
			if (!context.Config.SerializationPolicy.AllowNonSerializableTypes && !typeof(T).IsSerializable)
			{
				context.Config.DebugContext.LogError("The type " + typeof(T).Name + " is not marked as serializable.");
				return default(T);
			}
			bool flag = true;
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (typeof(T).IsValueType)
			{
				switch (entryType)
				{
				case EntryType.Null:
					context.Config.DebugContext.LogWarning("Expecting complex struct of type " + typeof(T).GetNiceFullName() + " but got null value.");
					reader.ReadNull();
					return default(T);
				default:
					context.Config.DebugContext.LogWarning(string.Concat("Unexpected entry '", name, "' of type ", entryType.ToString(), ", when ", EntryType.StartOfNode, " was expected. A value has likely been lost."));
					reader.SkipEntry();
					return default(T);
				case EntryType.StartOfNode:
					try
					{
						Type typeFromHandle = typeof(T);
						Type type;
						if (reader.EnterNode(out type))
						{
							if (type != typeFromHandle)
							{
								if (type != null)
								{
									context.Config.DebugContext.LogWarning("Expected complex struct value " + typeFromHandle.Name + " but the serialized value is of type " + type.Name + ".");
									if (type.IsCastableTo(typeFromHandle))
									{
										object obj = FormatterLocator.GetFormatter(type, context.Config.SerializationPolicy).Deserialize(reader);
										bool flag2 = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
										Func<object, object> func = ((!ComplexTypeIsNullable && !flag2) ? type.GetCastMethodDelegate(typeFromHandle) : null);
										if (func != null)
										{
											return (T)func(obj);
										}
										return (T)obj;
									}
									context.Config.DebugContext.LogWarning("Can't cast serialized type " + type.Name + " into expected type " + typeFromHandle.Name + ". Value lost for node '" + name + "'.");
									return default(T);
								}
								context.Config.DebugContext.LogWarning("Expected complex struct value " + typeFromHandle.Name + " but the serialized type could not be resolved.");
								return default(T);
							}
							return FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Deserialize(reader);
						}
						context.Config.DebugContext.LogError("Failed to enter node '" + name + "'.");
						return default(T);
					}
					catch (SerializationAbortException ex)
					{
						flag = false;
						throw ex;
					}
					catch (Exception exception)
					{
						context.Config.DebugContext.LogException(exception);
						return default(T);
					}
					finally
					{
						if (flag)
						{
							reader.ExitNode();
						}
					}
				}
			}
			switch (entryType)
			{
			case EntryType.Null:
				reader.ReadNull();
				return default(T);
			case EntryType.ExternalReferenceByIndex:
			{
				int index;
				reader.ReadExternalReference(out index);
				object externalObject3 = context.GetExternalObject(index);
				try
				{
					return (T)externalObject3;
				}
				catch (InvalidCastException)
				{
					context.Config.DebugContext.LogWarning("Can't cast external reference type " + externalObject3.GetType().Name + " into expected type " + typeof(T).Name + ". Value lost for node '" + name + "'.");
					return default(T);
				}
			}
			case EntryType.ExternalReferenceByGuid:
			{
				Guid guid;
				reader.ReadExternalReference(out guid);
				object externalObject2 = context.GetExternalObject(guid);
				try
				{
					return (T)externalObject2;
				}
				catch (InvalidCastException)
				{
					context.Config.DebugContext.LogWarning("Can't cast external reference type " + externalObject2.GetType().Name + " into expected type " + typeof(T).Name + ". Value lost for node '" + name + "'.");
					return default(T);
				}
			}
			case EntryType.ExternalReferenceByString:
			{
				string id2;
				reader.ReadExternalReference(out id2);
				object externalObject = context.GetExternalObject(id2);
				try
				{
					return (T)externalObject;
				}
				catch (InvalidCastException)
				{
					context.Config.DebugContext.LogWarning("Can't cast external reference type " + externalObject.GetType().Name + " into expected type " + typeof(T).Name + ". Value lost for node '" + name + "'.");
					return default(T);
				}
			}
			case EntryType.InternalReference:
			{
				int id;
				reader.ReadInternalReference(out id);
				object internalReference = context.GetInternalReference(id);
				try
				{
					return (T)internalReference;
				}
				catch (InvalidCastException)
				{
					context.Config.DebugContext.LogWarning("Can't cast internal reference type " + internalReference.GetType().Name + " into expected type " + typeof(T).Name + ". Value lost for node '" + name + "'.");
					return default(T);
				}
			}
			case EntryType.StartOfNode:
				try
				{
					Type typeFromHandle2 = typeof(T);
					Type type2;
					if (reader.EnterNode(out type2))
					{
						int currentNodeId = reader.CurrentNodeId;
						T val;
						if (type2 == null || typeFromHandle2 == type2)
						{
							val = ((!ComplexTypeIsAbstract) ? FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Deserialize(reader) : default(T));
						}
						else
						{
							bool flag3 = false;
							if (ComplexTypeIsObject && FormatterUtilities.IsPrimitiveType(type2))
							{
								Serializer serializer = Serializer.Get(type2);
								val = (T)serializer.ReadValueWeak(reader);
								flag3 = true;
							}
							else if (type2.IsCastableTo(typeFromHandle2))
							{
								try
								{
									IFormatter formatter = FormatterLocator.GetFormatter(type2, context.Config.SerializationPolicy);
									if (type2.IsValueType)
									{
										object obj2 = formatter.Deserialize(reader);
										if (ComplexTypeIsObject)
										{
											val = (T)obj2;
										}
										else
										{
											Func<object, object> castMethodDelegate = type2.GetCastMethodDelegate(typeFromHandle2);
											val = ((castMethodDelegate == null) ? ((T)obj2) : ((T)castMethodDelegate(obj2)));
										}
									}
									else
									{
										val = (T)formatter.Deserialize(reader);
									}
									flag3 = true;
								}
								catch (SerializationAbortException ex6)
								{
									flag = false;
									throw ex6;
								}
								catch (InvalidCastException)
								{
									flag3 = false;
									val = default(T);
								}
							}
							else
							{
								IFormatter formatter2 = FormatterLocator.GetFormatter(type2, context.Config.SerializationPolicy);
								object reference = formatter2.Deserialize(reader);
								if (currentNodeId >= 0)
								{
									context.RegisterInternalReference(currentNodeId, reference);
								}
								val = default(T);
							}
							if (!flag3)
							{
								context.Config.DebugContext.LogWarning("Can't cast serialized type " + type2.Name + " into expected type " + typeFromHandle2.Name + ". Value lost for node '" + name + "'.");
								val = default(T);
							}
						}
						if (currentNodeId >= 0)
						{
							context.RegisterInternalReference(currentNodeId, val);
						}
						return val;
					}
					context.Config.DebugContext.LogError("Failed to enter node '" + name + "'.");
					return default(T);
				}
				catch (SerializationAbortException ex8)
				{
					flag = false;
					throw ex8;
				}
				catch (Exception exception2)
				{
					context.Config.DebugContext.LogException(exception2);
					return default(T);
				}
				finally
				{
					if (flag)
					{
						reader.ExitNode();
					}
				}
			case EntryType.Boolean:
				if (ComplexTypeIsObject)
				{
					bool value5;
					reader.ReadBoolean(out value5);
					return (T)(object)value5;
				}
				break;
			case EntryType.FloatingPoint:
				if (ComplexTypeIsObject)
				{
					double value4;
					reader.ReadDouble(out value4);
					return (T)(object)value4;
				}
				break;
			case EntryType.Integer:
				if (ComplexTypeIsObject)
				{
					long value3;
					reader.ReadInt64(out value3);
					return (T)(object)value3;
				}
				break;
			case EntryType.String:
				if (ComplexTypeIsObject)
				{
					string value2;
					reader.ReadString(out value2);
					return (T)(object)value2;
				}
				break;
			case EntryType.Guid:
				if (ComplexTypeIsObject)
				{
					Guid value;
					reader.ReadGuid(out value);
					return (T)(object)value;
				}
				break;
			}
			context.Config.DebugContext.LogWarning("Unexpected entry of type " + entryType.ToString() + ", when a reference or node start was expected. A value has been lost.");
			reader.SkipEntry();
			return default(T);
		}

		public override void WriteValue(string name, T value, IDataWriter writer)
		{
			SerializationContext context = writer.Context;
			if (!context.Config.SerializationPolicy.AllowNonSerializableTypes && !typeof(T).IsSerializable)
			{
				context.Config.DebugContext.LogError("The type " + typeof(T).Name + " is not marked as serializable.");
				return;
			}
			if (typeof(T).IsValueType)
			{
				bool flag = true;
				try
				{
					writer.BeginStructNode(name, typeof(T));
					FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Serialize(value, writer);
					return;
				}
				catch (SerializationAbortException ex)
				{
					flag = false;
					throw ex;
				}
				finally
				{
					if (flag)
					{
						writer.EndNode(name);
					}
				}
			}
			bool flag2 = true;
			if (value == null)
			{
				writer.WriteNull(name);
				return;
			}
			int index;
			if (context.TryRegisterExternalReference((object)value, out index))
			{
				writer.WriteExternalReference(name, index);
				return;
			}
			Guid guid;
			if (context.TryRegisterExternalReference((object)value, out guid))
			{
				writer.WriteExternalReference(name, guid);
				return;
			}
			string id;
			if (context.TryRegisterExternalReference((object)value, out id))
			{
				writer.WriteExternalReference(name, id);
				return;
			}
			int id2;
			if (context.TryRegisterInternalReference(value, out id2))
			{
				Type type = value.GetType();
				if (ComplexTypeIsObject && FormatterUtilities.IsPrimitiveType(type))
				{
					try
					{
						writer.BeginReferenceNode(name, type, id2);
						Serializer serializer = Serializer.Get(type);
						serializer.WriteValueWeak(value, writer);
						return;
					}
					catch (SerializationAbortException ex2)
					{
						flag2 = false;
						throw ex2;
					}
					finally
					{
						if (flag2)
						{
							writer.EndNode(name);
						}
					}
				}
				IFormatter formatter = FormatterLocator.GetFormatter(type, context.Config.SerializationPolicy);
				try
				{
					writer.BeginReferenceNode(name, type, id2);
					formatter.Serialize(value, writer);
					return;
				}
				catch (SerializationAbortException ex3)
				{
					flag2 = false;
					throw ex3;
				}
				finally
				{
					if (flag2)
					{
						writer.EndNode(name);
					}
				}
			}
			writer.WriteInternalReference(name, id2);
		}
	}
}
