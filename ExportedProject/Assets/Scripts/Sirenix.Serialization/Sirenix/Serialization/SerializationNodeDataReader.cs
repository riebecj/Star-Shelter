using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Sirenix.Serialization
{
	public class SerializationNodeDataReader : BaseDataReader
	{
		private string peekedEntryName;

		private EntryType? peekedEntryType;

		private string peekedEntryData;

		private int currentIndex = -1;

		private List<SerializationNode> nodes;

		private Dictionary<Type, Delegate> primitiveTypeReaders;

		private bool IndexIsValid
		{
			get
			{
				if (nodes != null && currentIndex >= 0)
				{
					return currentIndex < nodes.Count;
				}
				return false;
			}
		}

		public List<SerializationNode> Nodes
		{
			get
			{
				if (nodes == null)
				{
					nodes = new List<SerializationNode>();
				}
				return nodes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				nodes = value;
			}
		}

		public override Stream Stream
		{
			get
			{
				throw new NotSupportedException("This data reader has no stream.");
			}
			set
			{
				throw new NotSupportedException("This data reader has no stream.");
			}
		}

		public SerializationNodeDataReader(DeserializationContext context)
			: base(null, context)
		{
			primitiveTypeReaders = new Dictionary<Type, Delegate>
			{
				{
					typeof(char),
					new Func<char>(_003C_002Ector_003Eb__6_0)
				},
				{
					typeof(sbyte),
					new Func<sbyte>(_003C_002Ector_003Eb__6_1)
				},
				{
					typeof(short),
					new Func<short>(_003C_002Ector_003Eb__6_2)
				},
				{
					typeof(int),
					new Func<int>(_003C_002Ector_003Eb__6_3)
				},
				{
					typeof(long),
					new Func<long>(_003C_002Ector_003Eb__6_4)
				},
				{
					typeof(byte),
					new Func<byte>(_003C_002Ector_003Eb__6_5)
				},
				{
					typeof(ushort),
					new Func<ushort>(_003C_002Ector_003Eb__6_6)
				},
				{
					typeof(uint),
					new Func<uint>(_003C_002Ector_003Eb__6_7)
				},
				{
					typeof(ulong),
					new Func<ulong>(_003C_002Ector_003Eb__6_8)
				},
				{
					typeof(decimal),
					new Func<decimal>(_003C_002Ector_003Eb__6_9)
				},
				{
					typeof(bool),
					new Func<bool>(_003C_002Ector_003Eb__6_10)
				},
				{
					typeof(float),
					new Func<float>(_003C_002Ector_003Eb__6_11)
				},
				{
					typeof(double),
					new Func<double>(_003C_002Ector_003Eb__6_12)
				},
				{
					typeof(Guid),
					new Func<Guid>(_003C_002Ector_003Eb__6_13)
				}
			};
		}

		public override void Dispose()
		{
			nodes = null;
			currentIndex = -1;
		}

		public override void PrepareNewSerializationSession()
		{
			currentIndex = -1;
		}

		public override EntryType PeekEntry(out string name)
		{
			if (peekedEntryType.HasValue)
			{
				name = peekedEntryName;
				return peekedEntryType.Value;
			}
			currentIndex++;
			if (IndexIsValid)
			{
				SerializationNode serializationNode = nodes[currentIndex];
				peekedEntryName = serializationNode.Name;
				peekedEntryType = serializationNode.Entry;
				peekedEntryData = serializationNode.Data;
			}
			else
			{
				peekedEntryName = null;
				peekedEntryType = EntryType.EndOfStream;
				peekedEntryData = null;
			}
			name = peekedEntryName;
			return peekedEntryType.Value;
		}

		public override bool EnterArray(out long length)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.StartOfArray)
			{
				PushArray();
				if (!long.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out length))
				{
					length = 0L;
					base.Context.Config.DebugContext.LogError("Failed to parse array length from data '" + peekedEntryData + "'.");
				}
				ConsumeCurrentEntry();
				return true;
			}
			SkipEntry();
			length = 0L;
			return false;
		}

		public override bool EnterNode(out Type type)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.StartOfNode)
			{
				string text = peekedEntryData;
				int id = -1;
				type = null;
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = null;
					int num = text.IndexOf("|", StringComparison.InvariantCulture);
					int result;
					if (num >= 0)
					{
						text2 = text.Substring(num + 1);
						string text3 = text.Substring(0, num);
						if (int.TryParse(text3, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
						{
							id = result;
						}
						else
						{
							base.Context.Config.DebugContext.LogError("Failed to parse id string '" + text3 + "' from data '" + text + "'.");
						}
					}
					else if (int.TryParse(text, out result))
					{
						id = result;
					}
					else
					{
						text2 = text;
					}
					if (text2 != null)
					{
						type = base.Binder.BindToType(text2, base.Context.Config.DebugContext);
					}
				}
				ConsumeCurrentEntry();
				PushNode(peekedEntryName, id, type);
				return true;
			}
			SkipEntry();
			type = null;
			return false;
		}

		public override bool ExitArray()
		{
			PeekEntry();
			while (peekedEntryType != EntryType.EndOfArray && peekedEntryType != EntryType.EndOfStream)
			{
				if (peekedEntryType == EntryType.EndOfNode)
				{
					base.Context.Config.DebugContext.LogError("Data layout mismatch; skipping past node boundary when exiting array.");
					ConsumeCurrentEntry();
				}
				SkipEntry();
			}
			if (peekedEntryType == EntryType.EndOfArray)
			{
				ConsumeCurrentEntry();
				PopArray();
				return true;
			}
			return false;
		}

		public override bool ExitNode()
		{
			PeekEntry();
			while (peekedEntryType != EntryType.EndOfNode && peekedEntryType != EntryType.EndOfStream)
			{
				if (peekedEntryType == EntryType.EndOfArray)
				{
					base.Context.Config.DebugContext.LogError("Data layout mismatch; skipping past array boundary when exiting node.");
					ConsumeCurrentEntry();
				}
				SkipEntry();
			}
			if (peekedEntryType == EntryType.EndOfNode)
			{
				ConsumeCurrentEntry();
				PopNode(base.CurrentNodeName);
				return true;
			}
			return false;
		}

		public override bool ReadBoolean(out bool value)
		{
			PeekEntry();
			try
			{
				if (peekedEntryType == EntryType.Boolean)
				{
					value = peekedEntryData == "true";
					return true;
				}
				value = false;
				return false;
			}
			finally
			{
				ConsumeCurrentEntry();
			}
		}

		public override bool ReadByte(out byte value)
		{
			ulong value2;
			if (ReadUInt64(out value2))
			{
				try
				{
					value = checked((byte)value2);
				}
				catch (OverflowException)
				{
					value = 0;
				}
				return true;
			}
			value = 0;
			return false;
		}

		public override bool ReadChar(out char value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.String)
			{
				try
				{
					if (peekedEntryData.Length == 1)
					{
						value = peekedEntryData[0];
						return true;
					}
					base.Context.Config.DebugContext.LogWarning("Expected string of length 1 for char entry.");
					value = '\0';
					return false;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			value = '\0';
			return false;
		}

		public override bool ReadDecimal(out decimal value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.FloatingPoint || peekedEntryType == EntryType.Integer)
			{
				try
				{
					if (!decimal.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						base.Context.Config.DebugContext.LogError("Failed to parse decimal value from entry data '" + peekedEntryData + "'.");
						return false;
					}
					return true;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			value = default(decimal);
			return false;
		}

		public override bool ReadDouble(out double value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.FloatingPoint || peekedEntryType == EntryType.Integer)
			{
				try
				{
					if (!double.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						base.Context.Config.DebugContext.LogError("Failed to parse double value from entry data '" + peekedEntryData + "'.");
						return false;
					}
					return true;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			value = 0.0;
			return false;
		}

		public override bool ReadExternalReference(out Guid guid)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.ExternalReferenceByGuid)
			{
				try
				{
					if ((guid = new Guid(peekedEntryData)) != Guid.Empty)
					{
						return true;
					}
					guid = Guid.Empty;
					return false;
				}
				catch (FormatException)
				{
					guid = Guid.Empty;
					return false;
				}
				catch (OverflowException)
				{
					guid = Guid.Empty;
					return false;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			guid = Guid.Empty;
			return false;
		}

		public override bool ReadExternalReference(out string id)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.ExternalReferenceByString)
			{
				id = peekedEntryData;
				ConsumeCurrentEntry();
				return true;
			}
			SkipEntry();
			id = null;
			return false;
		}

		public override bool ReadExternalReference(out int index)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.ExternalReferenceByIndex)
			{
				try
				{
					if (!int.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out index))
					{
						base.Context.Config.DebugContext.LogError("Failed to parse external index reference integer value from entry data '" + peekedEntryData + "'.");
						return false;
					}
					return true;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			index = 0;
			return false;
		}

		public override bool ReadGuid(out Guid value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.Guid)
			{
				try
				{
					if ((value = new Guid(peekedEntryData)) != Guid.Empty)
					{
						return true;
					}
					value = Guid.Empty;
					return false;
				}
				catch (FormatException)
				{
					value = Guid.Empty;
					return false;
				}
				catch (OverflowException)
				{
					value = Guid.Empty;
					return false;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			value = Guid.Empty;
			return false;
		}

		public override bool ReadInt16(out short value)
		{
			long value2;
			if (ReadInt64(out value2))
			{
				try
				{
					value = checked((short)value2);
				}
				catch (OverflowException)
				{
					value = 0;
				}
				return true;
			}
			value = 0;
			return false;
		}

		public override bool ReadInt32(out int value)
		{
			long value2;
			if (ReadInt64(out value2))
			{
				try
				{
					value = checked((int)value2);
				}
				catch (OverflowException)
				{
					value = 0;
				}
				return true;
			}
			value = 0;
			return false;
		}

		public override bool ReadInt64(out long value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.Integer)
			{
				try
				{
					if (!long.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						base.Context.Config.DebugContext.LogError("Failed to parse integer value from entry data '" + peekedEntryData + "'.");
						return false;
					}
					return true;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			value = 0L;
			return false;
		}

		public override bool ReadInternalReference(out int id)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.InternalReference)
			{
				try
				{
					if (!int.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out id))
					{
						base.Context.Config.DebugContext.LogError("Failed to parse internal reference id integer value from entry data '" + peekedEntryData + "'.");
						return false;
					}
					return true;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			id = 0;
			return false;
		}

		public override bool ReadNull()
		{
			PeekEntry();
			if (peekedEntryType == EntryType.Null)
			{
				ConsumeCurrentEntry();
				return true;
			}
			SkipEntry();
			return false;
		}

		public override bool ReadPrimitiveArray<T>(out T[] array)
		{
			if (!FormatterUtilities.IsPrimitiveArrayType(typeof(T)))
			{
				throw new ArgumentException("Type " + typeof(T).Name + " is not a valid primitive array type.");
			}
			if (peekedEntryType != EntryType.PrimitiveArray)
			{
				SkipEntry();
				array = null;
				return false;
			}
			if (typeof(T) == typeof(byte))
			{
				array = (T[])(object)ProperBitConverter.HexStringToBytes(peekedEntryData);
				return true;
			}
			PeekEntry();
			if (peekedEntryType != EntryType.PrimitiveArray)
			{
				base.Context.Config.DebugContext.LogError(string.Concat("Expected entry of type '", EntryType.StartOfArray, "' when reading primitive array but got entry of type '", peekedEntryType, "'."));
				SkipEntry();
				array = new T[0];
				return false;
			}
			long result;
			if (!long.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				base.Context.Config.DebugContext.LogError("Failed to parse primitive array length from entry data '" + peekedEntryData + "'.");
				SkipEntry();
				array = new T[0];
				return false;
			}
			ConsumeCurrentEntry();
			PushArray();
			array = new T[result];
			Func<T> func = (Func<T>)primitiveTypeReaders[typeof(T)];
			for (int i = 0; i < result; i++)
			{
				array[i] = func();
			}
			ExitArray();
			return true;
		}

		public override bool ReadSByte(out sbyte value)
		{
			long value2;
			if (ReadInt64(out value2))
			{
				try
				{
					value = checked((sbyte)value2);
				}
				catch (OverflowException)
				{
					value = 0;
				}
				return true;
			}
			value = 0;
			return false;
		}

		public override bool ReadSingle(out float value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.FloatingPoint || peekedEntryType == EntryType.Integer)
			{
				try
				{
					if (!float.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						base.Context.Config.DebugContext.LogError("Failed to parse float value from entry data '" + peekedEntryData + "'.");
						return false;
					}
					return true;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			value = 0f;
			return false;
		}

		public override bool ReadString(out string value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.String)
			{
				value = peekedEntryData;
				ConsumeCurrentEntry();
				return true;
			}
			SkipEntry();
			value = null;
			return false;
		}

		public override bool ReadUInt16(out ushort value)
		{
			ulong value2;
			if (ReadUInt64(out value2))
			{
				try
				{
					value = checked((ushort)value2);
				}
				catch (OverflowException)
				{
					value = 0;
				}
				return true;
			}
			value = 0;
			return false;
		}

		public override bool ReadUInt32(out uint value)
		{
			ulong value2;
			if (ReadUInt64(out value2))
			{
				try
				{
					value = checked((uint)value2);
				}
				catch (OverflowException)
				{
					value = 0u;
				}
				return true;
			}
			value = 0u;
			return false;
		}

		public override bool ReadUInt64(out ulong value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.Integer)
			{
				try
				{
					if (!ulong.TryParse(peekedEntryData, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						base.Context.Config.DebugContext.LogError("Failed to parse integer value from entry data '" + peekedEntryData + "'.");
						return false;
					}
					return true;
				}
				finally
				{
					ConsumeCurrentEntry();
				}
			}
			SkipEntry();
			value = 0uL;
			return false;
		}

		private void ConsumeCurrentEntry()
		{
			if (peekedEntryType.HasValue && peekedEntryType != EntryType.EndOfStream)
			{
				peekedEntryType = null;
			}
		}

		protected override EntryType PeekEntry()
		{
			string name;
			return PeekEntry(out name);
		}

		protected override EntryType ReadToNextEntry()
		{
			ConsumeCurrentEntry();
			string name;
			return PeekEntry(out name);
		}

		[CompilerGenerated]
		private char _003C_002Ector_003Eb__6_0()
		{
			char value;
			ReadChar(out value);
			return value;
		}

		[CompilerGenerated]
		private sbyte _003C_002Ector_003Eb__6_1()
		{
			sbyte value;
			ReadSByte(out value);
			return value;
		}

		[CompilerGenerated]
		private short _003C_002Ector_003Eb__6_2()
		{
			short value;
			ReadInt16(out value);
			return value;
		}

		[CompilerGenerated]
		private int _003C_002Ector_003Eb__6_3()
		{
			int value;
			ReadInt32(out value);
			return value;
		}

		[CompilerGenerated]
		private long _003C_002Ector_003Eb__6_4()
		{
			long value;
			ReadInt64(out value);
			return value;
		}

		[CompilerGenerated]
		private byte _003C_002Ector_003Eb__6_5()
		{
			byte value;
			ReadByte(out value);
			return value;
		}

		[CompilerGenerated]
		private ushort _003C_002Ector_003Eb__6_6()
		{
			ushort value;
			ReadUInt16(out value);
			return value;
		}

		[CompilerGenerated]
		private uint _003C_002Ector_003Eb__6_7()
		{
			uint value;
			ReadUInt32(out value);
			return value;
		}

		[CompilerGenerated]
		private ulong _003C_002Ector_003Eb__6_8()
		{
			ulong value;
			ReadUInt64(out value);
			return value;
		}

		[CompilerGenerated]
		private decimal _003C_002Ector_003Eb__6_9()
		{
			decimal value;
			ReadDecimal(out value);
			return value;
		}

		[CompilerGenerated]
		private bool _003C_002Ector_003Eb__6_10()
		{
			bool value;
			ReadBoolean(out value);
			return value;
		}

		[CompilerGenerated]
		private float _003C_002Ector_003Eb__6_11()
		{
			float value;
			ReadSingle(out value);
			return value;
		}

		[CompilerGenerated]
		private double _003C_002Ector_003Eb__6_12()
		{
			double value;
			ReadDouble(out value);
			return value;
		}

		[CompilerGenerated]
		private Guid _003C_002Ector_003Eb__6_13()
		{
			Guid value;
			ReadGuid(out value);
			return value;
		}
	}
}
