using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public abstract class Serializer
	{
		private static readonly Dictionary<Type, Type> PrimitiveReaderWriterTypes = new Dictionary<Type, Type>
		{
			{
				typeof(char),
				typeof(CharSerializer)
			},
			{
				typeof(string),
				typeof(StringSerializer)
			},
			{
				typeof(sbyte),
				typeof(SByteSerializer)
			},
			{
				typeof(short),
				typeof(Int16Serializer)
			},
			{
				typeof(int),
				typeof(Int32Serializer)
			},
			{
				typeof(long),
				typeof(Int64Serializer)
			},
			{
				typeof(byte),
				typeof(ByteSerializer)
			},
			{
				typeof(ushort),
				typeof(UInt16Serializer)
			},
			{
				typeof(uint),
				typeof(UInt32Serializer)
			},
			{
				typeof(ulong),
				typeof(UInt64Serializer)
			},
			{
				typeof(decimal),
				typeof(DecimalSerializer)
			},
			{
				typeof(bool),
				typeof(BooleanSerializer)
			},
			{
				typeof(float),
				typeof(SingleSerializer)
			},
			{
				typeof(double),
				typeof(DoubleSerializer)
			},
			{
				typeof(IntPtr),
				typeof(IntPtrSerializer)
			},
			{
				typeof(UIntPtr),
				typeof(UIntPtrSerializer)
			},
			{
				typeof(Guid),
				typeof(GuidSerializer)
			}
		};

		private static readonly object LOCK = new object();

		private static readonly Dictionary<Type, Serializer> ReaderWriterCache = new Dictionary<Type, Serializer>();

		[Conditional("UNITY_EDITOR")]
		protected static void FireOnSerializedType(Type type)
		{
		}

		public static Serializer GetForValue(object value)
		{
			if (value == null)
			{
				return Get(typeof(object));
			}
			return Get(value.GetType());
		}

		public static Serializer<T> Get<T>()
		{
			return (Serializer<T>)Get(typeof(T));
		}

		public static Serializer Get(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException();
			}
			lock (LOCK)
			{
				Serializer value;
				if (!ReaderWriterCache.TryGetValue(type, out value))
				{
					value = Create(type);
					ReaderWriterCache.Add(type, value);
					return value;
				}
				return value;
			}
		}

		public abstract object ReadValueWeak(IDataReader reader);

		public void WriteValueWeak(object value, IDataWriter writer)
		{
			WriteValueWeak(null, value, writer);
		}

		public abstract void WriteValueWeak(string name, object value, IDataWriter writer);

		private static Serializer Create(Type type)
		{
			try
			{
				Type type2 = null;
				if (type.IsEnum)
				{
					type2 = typeof(EnumSerializer<>).MakeGenericType(type);
				}
				else if (FormatterUtilities.IsPrimitiveType(type))
				{
					try
					{
						type2 = PrimitiveReaderWriterTypes[type];
					}
					catch (KeyNotFoundException)
					{
						UnityEngine.Debug.LogError("Failed to find primitive serializer for " + type.Name);
					}
				}
				else
				{
					type2 = typeof(ComplexTypeSerializer<>).MakeGenericType(type);
				}
				return (Serializer)Activator.CreateInstance(type2);
			}
			catch (TargetInvocationException ex2)
			{
				if (ex2.GetBaseException() is ExecutionEngineException)
				{
					LogAOTError(type, ex2.GetBaseException() as ExecutionEngineException);
					return null;
				}
				throw ex2;
			}
			catch (TypeInitializationException ex3)
			{
				if (ex3.GetBaseException() is ExecutionEngineException)
				{
					LogAOTError(type, ex3.GetBaseException() as ExecutionEngineException);
					return null;
				}
				throw ex3;
			}
			catch (ExecutionEngineException ex4)
			{
				LogAOTError(type, ex4);
				return null;
			}
		}

		private static void LogAOTError(Type type, ExecutionEngineException ex)
		{
			UnityEngine.Debug.LogError("No AOT serializer was pre-generated for the type '" + type.GetNiceFullName() + "'. Please use Odin's AOT generation feature to generate an AOT dll before building, and ensure that '" + type.GetNiceFullName() + "' is in the list of supported types after a scan. If it is not, please report an issue and add it to the list manually.");
			throw new SerializationAbortException("AOT serializer was missing for type '" + type.GetNiceFullName() + "'.");
		}
	}
	public abstract class Serializer<T> : Serializer
	{
		public override object ReadValueWeak(IDataReader reader)
		{
			return ReadValue(reader);
		}

		public override void WriteValueWeak(string name, object value, IDataWriter writer)
		{
			WriteValue(name, (T)value, writer);
		}

		public abstract T ReadValue(IDataReader reader);

		public void WriteValue(T value, IDataWriter writer)
		{
			WriteValue(null, value, writer);
		}

		public abstract void WriteValue(string name, T value, IDataWriter writer);

		[Conditional("UNITY_EDITOR")]
		protected static void FireOnSerializedType()
		{
		}
	}
}
