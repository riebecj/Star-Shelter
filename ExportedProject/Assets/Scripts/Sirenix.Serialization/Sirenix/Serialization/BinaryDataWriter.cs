using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Sirenix.Utilities.Unsafe;

namespace Sirenix.Serialization
{
	public class BinaryDataWriter : BaseDataWriter
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			internal void _003C_002Ecctor_003Eb__37_0(byte[] b, int i, char v)
			{
				ProperBitConverter.GetBytes(b, i, v);
			}

			internal void _003C_002Ecctor_003Eb__37_1(byte[] b, int i, byte v)
			{
				b[i] = v;
			}

			internal void _003C_002Ecctor_003Eb__37_2(byte[] b, int i, sbyte v)
			{
				b[i] = (byte)v;
			}

			internal void _003C_002Ecctor_003Eb__37_3(byte[] b, int i, bool v)
			{
				b[i] = (byte)(v ? 1 : 0);
			}
		}

		private static readonly Dictionary<Type, Delegate> PrimitiveGetBytesMethods = new Dictionary<Type, Delegate>
		{
			{
				typeof(char),
				new Action<byte[], int, char>(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__37_0)
			},
			{
				typeof(byte),
				new Action<byte[], int, byte>(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__37_1)
			},
			{
				typeof(sbyte),
				new Action<byte[], int, sbyte>(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__37_2)
			},
			{
				typeof(bool),
				new Action<byte[], int, bool>(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__37_3)
			},
			{
				typeof(short),
				new Action<byte[], int, short>(ProperBitConverter.GetBytes)
			},
			{
				typeof(int),
				new Action<byte[], int, int>(ProperBitConverter.GetBytes)
			},
			{
				typeof(long),
				new Action<byte[], int, long>(ProperBitConverter.GetBytes)
			},
			{
				typeof(ushort),
				new Action<byte[], int, ushort>(ProperBitConverter.GetBytes)
			},
			{
				typeof(uint),
				new Action<byte[], int, uint>(ProperBitConverter.GetBytes)
			},
			{
				typeof(ulong),
				new Action<byte[], int, ulong>(ProperBitConverter.GetBytes)
			},
			{
				typeof(decimal),
				new Action<byte[], int, decimal>(ProperBitConverter.GetBytes)
			},
			{
				typeof(float),
				new Action<byte[], int, float>(ProperBitConverter.GetBytes)
			},
			{
				typeof(double),
				new Action<byte[], int, double>(ProperBitConverter.GetBytes)
			},
			{
				typeof(Guid),
				new Action<byte[], int, Guid>(ProperBitConverter.GetBytes)
			}
		};

		private static readonly Dictionary<Type, int> PrimitiveSizes = new Dictionary<Type, int>
		{
			{
				typeof(char),
				2
			},
			{
				typeof(byte),
				1
			},
			{
				typeof(sbyte),
				1
			},
			{
				typeof(bool),
				1
			},
			{
				typeof(short),
				2
			},
			{
				typeof(int),
				4
			},
			{
				typeof(long),
				8
			},
			{
				typeof(ushort),
				2
			},
			{
				typeof(uint),
				4
			},
			{
				typeof(ulong),
				8
			},
			{
				typeof(decimal),
				16
			},
			{
				typeof(float),
				4
			},
			{
				typeof(double),
				8
			},
			{
				typeof(Guid),
				16
			}
		};

		private readonly byte[] buffer = new byte[16];

		private readonly Dictionary<Type, int> types = new Dictionary<Type, int>(16);

		public BinaryDataWriter(Stream stream, SerializationContext context)
			: base(stream, context)
		{
		}

		public override void BeginArrayNode(long length)
		{
			Stream.WriteByte(6);
			ProperBitConverter.GetBytes(buffer, 0, length);
			Stream.Write(buffer, 0, 8);
			PushArray();
		}

		public override void BeginReferenceNode(string name, Type type, int id)
		{
			if (name != null)
			{
				Stream.WriteByte(1);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(2);
			}
			WriteType(type);
			WriteIntValue(id);
			PushNode(name, id, type);
		}

		public override void BeginStructNode(string name, Type type)
		{
			if (name != null)
			{
				Stream.WriteByte(3);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(4);
			}
			WriteType(type);
			PushNode(name, -1, type);
		}

		public override void Dispose()
		{
			Stream.Dispose();
		}

		public override void EndArrayNode()
		{
			PopArray();
			Stream.WriteByte(7);
		}

		public override void EndNode(string name)
		{
			PopNode(name);
			Stream.WriteByte(5);
		}

		public override void WritePrimitiveArray<T>(T[] array)
		{
			if (!FormatterUtilities.IsPrimitiveArrayType(typeof(T)))
			{
				throw new ArgumentException("Type " + typeof(T).Name + " is not a valid primitive array type.");
			}
			int num = PrimitiveSizes[typeof(T)];
			int num2 = array.Length * num;
			Stream.WriteByte(8);
			ProperBitConverter.GetBytes(this.buffer, 0, array.Length);
			Stream.Write(this.buffer, 0, 4);
			ProperBitConverter.GetBytes(this.buffer, 0, num);
			Stream.Write(this.buffer, 0, 4);
			if (typeof(T) == typeof(byte))
			{
				byte[] array2 = (byte[])(object)array;
				Stream.Write(array2, 0, num2);
				return;
			}
			using (Buffer<byte> buffer = Buffer<byte>.Claim(num2))
			{
				if (!BitConverter.IsLittleEndian)
				{
					UnsafeUtilities.MemoryCopy(array, buffer.Array, num2, 0, 0);
				}
				else
				{
					Action<byte[], int, T> action = (Action<byte[], int, T>)PrimitiveGetBytesMethods[typeof(T)];
					byte[] array3 = buffer.Array;
					for (int i = 0; i < array.Length; i++)
					{
						action(array3, i * num, array[i]);
					}
				}
				Stream.Write(buffer.Array, 0, num2);
			}
		}

		public override void WriteBoolean(string name, bool value)
		{
			if (name != null)
			{
				Stream.WriteByte(43);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(44);
			}
			Stream.WriteByte((byte)(value ? 1 : 0));
		}

		public override void WriteByte(string name, byte value)
		{
			if (name != null)
			{
				Stream.WriteByte(17);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(18);
			}
			Stream.WriteByte(value);
		}

		public override void WriteChar(string name, char value)
		{
			if (name != null)
			{
				Stream.WriteByte(37);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(38);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 2);
		}

		public override void WriteDecimal(string name, decimal value)
		{
			if (name != null)
			{
				Stream.WriteByte(35);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(36);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 16);
		}

		public override void WriteDouble(string name, double value)
		{
			if (name != null)
			{
				Stream.WriteByte(33);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(34);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 8);
		}

		public override void WriteGuid(string name, Guid value)
		{
			if (name != null)
			{
				Stream.WriteByte(41);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(42);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 16);
		}

		public override void WriteExternalReference(string name, Guid guid)
		{
			if (name != null)
			{
				Stream.WriteByte(13);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(14);
			}
			ProperBitConverter.GetBytes(buffer, 0, guid);
			Stream.Write(buffer, 0, 16);
		}

		public override void WriteExternalReference(string name, int index)
		{
			if (name != null)
			{
				Stream.WriteByte(11);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(12);
			}
			WriteIntValue(index);
		}

		public override void WriteExternalReference(string name, string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (name != null)
			{
				Stream.WriteByte(50);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(51);
			}
			WriteStringValue(id);
		}

		public override void WriteInt32(string name, int value)
		{
			if (name != null)
			{
				Stream.WriteByte(23);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(24);
			}
			WriteIntValue(value);
		}

		public override void WriteInt64(string name, long value)
		{
			if (name != null)
			{
				Stream.WriteByte(27);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(28);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 8);
		}

		public override void WriteNull(string name)
		{
			if (name != null)
			{
				Stream.WriteByte(45);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(46);
			}
		}

		public override void WriteInternalReference(string name, int id)
		{
			if (name != null)
			{
				Stream.WriteByte(9);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(10);
			}
			ProperBitConverter.GetBytes(buffer, 0, id);
			Stream.Write(buffer, 0, 4);
		}

		public override void WriteSByte(string name, sbyte value)
		{
			if (name != null)
			{
				Stream.WriteByte(15);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(16);
			}
			Stream.WriteByte((byte)value);
		}

		public override void WriteInt16(string name, short value)
		{
			if (name != null)
			{
				Stream.WriteByte(19);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(20);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 2);
		}

		public override void WriteSingle(string name, float value)
		{
			if (name != null)
			{
				Stream.WriteByte(31);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(32);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 4);
		}

		public override void WriteString(string name, string value)
		{
			if (name != null)
			{
				Stream.WriteByte(39);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(40);
			}
			WriteStringValue(value);
		}

		public override void WriteUInt32(string name, uint value)
		{
			if (name != null)
			{
				Stream.WriteByte(25);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(26);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 4);
		}

		public override void WriteUInt64(string name, ulong value)
		{
			if (name != null)
			{
				Stream.WriteByte(29);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(30);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 8);
		}

		public override void WriteUInt16(string name, ushort value)
		{
			if (name != null)
			{
				Stream.WriteByte(21);
				WriteStringValue(name);
			}
			else
			{
				Stream.WriteByte(22);
			}
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 2);
		}

		public override void PrepareNewSerializationSession()
		{
			types.Clear();
		}

		private void WriteType(Type type)
		{
			if (type == null)
			{
				WriteNull(null);
				return;
			}
			int value;
			if (types.TryGetValue(type, out value))
			{
				Stream.WriteByte(48);
				WriteIntValue(value);
				return;
			}
			value = types.Count;
			types.Add(type, value);
			Stream.WriteByte(47);
			WriteIntValue(value);
			WriteStringValue(base.Binder.BindToName(type, base.Context.Config.DebugContext));
		}

		private void WriteStringValue(string value)
		{
			if (StringRequires16BitSupport(value))
			{
				Stream.WriteByte(1);
				ProperBitConverter.GetBytes(this.buffer, 0, value.Length);
				Stream.Write(this.buffer, 0, 4);
				using (Buffer<byte> buffer = Buffer<byte>.Claim(value.Length * 2))
				{
					byte[] array = buffer.Array;
					UnsafeUtilities.StringToBytes(array, value, true);
					Stream.Write(array, 0, value.Length * 2);
					return;
				}
			}
			Stream.WriteByte(0);
			ProperBitConverter.GetBytes(this.buffer, 0, value.Length);
			Stream.Write(this.buffer, 0, 4);
			using (Buffer<byte> buffer2 = Buffer<byte>.Claim(value.Length))
			{
				byte[] array2 = buffer2.Array;
				for (int i = 0; i < value.Length; i++)
				{
					array2[i] = (byte)value[i];
				}
				Stream.Write(array2, 0, value.Length);
			}
		}

		private void WriteIntValue(int value)
		{
			ProperBitConverter.GetBytes(buffer, 0, value);
			Stream.Write(buffer, 0, 4);
		}

		private bool StringRequires16BitSupport(string value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] > 'ÿ')
				{
					return true;
				}
			}
			return false;
		}
	}
}
