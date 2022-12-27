using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Sirenix.Serialization
{
	public class JsonDataReader : BaseDataReader
	{
		private JsonTextReader reader;

		private EntryType? peekedEntryType;

		private string peekedEntryName;

		private string peekedEntryContent;

		private Dictionary<int, Type> seenTypes = new Dictionary<int, Type>(16);

		private readonly Dictionary<Type, Delegate> primitiveArrayReaders;

		public override Stream Stream
		{
			get
			{
				return base.Stream;
			}
			set
			{
				base.Stream = value;
				reader = new JsonTextReader(base.Stream, base.Context);
			}
		}

		public JsonDataReader(Stream stream, DeserializationContext context)
			: base(stream, context)
		{
			primitiveArrayReaders = new Dictionary<Type, Delegate>
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
			reader.Dispose();
		}

		public override EntryType PeekEntry(out string name)
		{
			if (peekedEntryType.HasValue)
			{
				name = peekedEntryName;
				return peekedEntryType.Value;
			}
			EntryType entry;
			reader.ReadToNextEntry(out name, out peekedEntryContent, out entry);
			peekedEntryName = name;
			peekedEntryType = entry;
			return entry;
		}

		public override bool EnterNode(out Type type)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.StartOfNode)
			{
				string name = peekedEntryName;
				int result = -1;
				ReadToNextEntry();
				if (peekedEntryName == "$id")
				{
					if (!int.TryParse(peekedEntryContent, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
					{
						base.Context.Config.DebugContext.LogError("Failed to parse id: " + peekedEntryContent);
						result = -1;
					}
					ReadToNextEntry();
				}
				if (peekedEntryName == "$type" && peekedEntryContent != null && peekedEntryContent.Length > 0)
				{
					if (peekedEntryType == EntryType.Integer)
					{
						int value;
						if (ReadInt32(out value))
						{
							if (!seenTypes.TryGetValue(value, out type))
							{
								base.Context.Config.DebugContext.LogError("Missing type id for node with reference id " + result + ": " + value);
							}
						}
						else
						{
							base.Context.Config.DebugContext.LogError("Failed to read type id for node with reference id " + result);
							type = null;
						}
					}
					else
					{
						int num = 1;
						int result2 = -1;
						int num2 = peekedEntryContent.IndexOf('|');
						if (num2 >= 0)
						{
							num = num2 + 1;
							string s = peekedEntryContent.Substring(1, num2 - 1);
							if (!int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result2))
							{
								result2 = -1;
							}
						}
						type = base.Binder.BindToType(peekedEntryContent.Substring(num, peekedEntryContent.Length - (1 + num)), base.Context.Config.DebugContext);
						if (result2 >= 0)
						{
							seenTypes[result2] = type;
						}
						peekedEntryType = null;
					}
				}
				else
				{
					type = null;
				}
				PushNode(name, result, type);
				return true;
			}
			SkipEntry();
			type = null;
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
					MarkEntryConsumed();
				}
				SkipEntry();
			}
			if (peekedEntryType == EntryType.EndOfNode)
			{
				peekedEntryType = null;
				PopNode(base.CurrentNodeName);
				return true;
			}
			return false;
		}

		public override bool EnterArray(out long length)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.StartOfArray)
			{
				PushArray();
				if (peekedEntryName != "$rlength")
				{
					base.Context.Config.DebugContext.LogError("Array entry wasn't preceded by an array length entry!");
					length = 0L;
					return true;
				}
				int result;
				if (!int.TryParse(peekedEntryContent, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
				{
					base.Context.Config.DebugContext.LogError("Failed to parse array length: " + peekedEntryContent);
					length = 0L;
					return true;
				}
				length = result;
				ReadToNextEntry();
				if (peekedEntryName != "$rcontent")
				{
					base.Context.Config.DebugContext.LogError("Failed to find regular array content entry after array length entry!");
					length = 0L;
					return true;
				}
				peekedEntryType = null;
				return true;
			}
			SkipEntry();
			length = 0L;
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
					MarkEntryConsumed();
				}
				SkipEntry();
			}
			if (peekedEntryType == EntryType.EndOfArray)
			{
				peekedEntryType = null;
				PopArray();
				return true;
			}
			return false;
		}

		public override bool ReadPrimitiveArray<T>(out T[] array)
		{
			if (!FormatterUtilities.IsPrimitiveArrayType(typeof(T)))
			{
				throw new ArgumentException("Type " + typeof(T).Name + " is not a valid primitive array type.");
			}
			PeekEntry();
			if (peekedEntryType == EntryType.PrimitiveArray)
			{
				PushArray();
				if (peekedEntryName != "$plength")
				{
					base.Context.Config.DebugContext.LogError("Array entry wasn't preceded by an array length entry!");
					array = null;
					return false;
				}
				int result;
				if (!int.TryParse(peekedEntryContent, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
				{
					base.Context.Config.DebugContext.LogError("Failed to parse array length: " + peekedEntryContent);
					array = null;
					return false;
				}
				ReadToNextEntry();
				if (peekedEntryName != "$pcontent")
				{
					base.Context.Config.DebugContext.LogError("Failed to find primitive array content entry after array length entry!");
					array = null;
					return false;
				}
				peekedEntryType = null;
				Func<T> func = (Func<T>)primitiveArrayReaders[typeof(T)];
				array = new T[result];
				for (int i = 0; i < result; i++)
				{
					array[i] = func();
				}
				ExitArray();
				return true;
			}
			SkipEntry();
			array = null;
			return false;
		}

		public override bool ReadBoolean(out bool value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.Boolean)
			{
				try
				{
					value = peekedEntryContent == "true";
					return true;
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = false;
			return false;
		}

		public override bool ReadInternalReference(out int id)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.InternalReference)
			{
				try
				{
					return ReadAnyIntReference(out id);
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			id = -1;
			return false;
		}

		public override bool ReadExternalReference(out int index)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.ExternalReferenceByIndex)
			{
				try
				{
					return ReadAnyIntReference(out index);
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			index = -1;
			return false;
		}

		public override bool ReadExternalReference(out Guid guid)
		{
			PeekEntry();
			EntryType? entryType = peekedEntryType;
			EntryType entryType2 = EntryType.ExternalReferenceByGuid;
			if (entryType.GetValueOrDefault() == entryType2 && entryType.HasValue && (guid = new Guid(peekedEntryContent)) != Guid.Empty)
			{
				try
				{
					return true;
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
					MarkEntryConsumed();
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
				id = peekedEntryContent;
				MarkEntryConsumed();
				return true;
			}
			SkipEntry();
			id = null;
			return false;
		}

		public override bool ReadChar(out char value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.String)
			{
				try
				{
					value = peekedEntryContent[1];
					return true;
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = '\0';
			return false;
		}

		public override bool ReadString(out string value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.String)
			{
				try
				{
					value = peekedEntryContent.Substring(1, peekedEntryContent.Length - 2);
					return true;
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = null;
			return false;
		}

		public override bool ReadGuid(out Guid value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.Guid)
			{
				try
				{
					value = new Guid(peekedEntryContent);
					return true;
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
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = Guid.Empty;
			return false;
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
					if (long.TryParse(peekedEntryContent, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						return true;
					}
					base.Context.Config.DebugContext.LogError("Failed to parse long from: " + peekedEntryContent);
					return false;
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = 0L;
			return false;
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
					if (ulong.TryParse(peekedEntryContent, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						return true;
					}
					base.Context.Config.DebugContext.LogError("Failed to parse ulong from: " + peekedEntryContent);
					return false;
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = 0uL;
			return false;
		}

		public override bool ReadDecimal(out decimal value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.FloatingPoint || peekedEntryType == EntryType.Integer)
			{
				try
				{
					if (decimal.TryParse(peekedEntryContent, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						return true;
					}
					base.Context.Config.DebugContext.LogError("Failed to parse decimal from: " + peekedEntryContent);
					return false;
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = default(decimal);
			return false;
		}

		public override bool ReadSingle(out float value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.FloatingPoint || peekedEntryType == EntryType.Integer)
			{
				try
				{
					if (float.TryParse(peekedEntryContent, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						return true;
					}
					base.Context.Config.DebugContext.LogError("Failed to parse float from: " + peekedEntryContent);
					return false;
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = 0f;
			return false;
		}

		public override bool ReadDouble(out double value)
		{
			PeekEntry();
			if (peekedEntryType == EntryType.FloatingPoint || peekedEntryType == EntryType.Integer)
			{
				try
				{
					if (double.TryParse(peekedEntryContent, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					{
						return true;
					}
					base.Context.Config.DebugContext.LogError("Failed to parse double from: " + peekedEntryContent);
					return false;
				}
				finally
				{
					MarkEntryConsumed();
				}
			}
			SkipEntry();
			value = 0.0;
			return false;
		}

		public override bool ReadNull()
		{
			PeekEntry();
			if (peekedEntryType == EntryType.Null)
			{
				MarkEntryConsumed();
				return true;
			}
			SkipEntry();
			return false;
		}

		public override void PrepareNewSerializationSession()
		{
			peekedEntryType = null;
			peekedEntryContent = null;
			peekedEntryName = null;
			seenTypes.Clear();
			reader.Reset();
		}

		protected override EntryType PeekEntry()
		{
			string name;
			return PeekEntry(out name);
		}

		protected override EntryType ReadToNextEntry()
		{
			peekedEntryType = null;
			string name;
			return PeekEntry(out name);
		}

		private void MarkEntryConsumed()
		{
			if (peekedEntryType != EntryType.EndOfArray && peekedEntryType != EntryType.EndOfNode)
			{
				peekedEntryType = null;
			}
		}

		private bool ReadAnyIntReference(out int value)
		{
			int num = -1;
			for (int i = 0; i < peekedEntryContent.Length; i++)
			{
				if (peekedEntryContent[i] == ':')
				{
					num = i;
					break;
				}
			}
			if (num == -1 || num == peekedEntryContent.Length - 1)
			{
				base.Context.Config.DebugContext.LogError("Failed to parse id from: " + peekedEntryContent);
			}
			string text = peekedEntryContent.Substring(num + 1);
			if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
			{
				return true;
			}
			base.Context.Config.DebugContext.LogError("Failed to parse id: " + text);
			value = -1;
			return false;
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
