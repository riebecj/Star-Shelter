using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sirenix.Serialization
{
	public class JsonDataWriter : BaseDataWriter
	{
		private bool justStarted;

		private bool forceNoSeparatorNextLine;

		private StringBuilder escapeStringBuilder;

		private StreamWriter writer;

		private Dictionary<Type, Delegate> primitiveTypeWriters;

		private Dictionary<Type, int> seenTypes = new Dictionary<Type, int>(16);

		public bool FormatAsReadable { get; set; }

		public bool EnableTypeOptimization { get; set; }

		public override Stream Stream
		{
			get
			{
				return base.Stream;
			}
			set
			{
				base.Stream = value;
				writer = new StreamWriter(base.Stream);
			}
		}

		public JsonDataWriter(Stream stream, SerializationContext context, bool formatAsReadable = true)
			: base(stream, context)
		{
			FormatAsReadable = formatAsReadable;
			justStarted = true;
			EnableTypeOptimization = true;
			primitiveTypeWriters = new Dictionary<Type, Delegate>
			{
				{
					typeof(char),
					new Action<string, char>(WriteChar)
				},
				{
					typeof(sbyte),
					new Action<string, sbyte>(WriteSByte)
				},
				{
					typeof(short),
					new Action<string, short>(WriteInt16)
				},
				{
					typeof(int),
					new Action<string, int>(WriteInt32)
				},
				{
					typeof(long),
					new Action<string, long>(WriteInt64)
				},
				{
					typeof(byte),
					new Action<string, byte>(WriteByte)
				},
				{
					typeof(ushort),
					new Action<string, ushort>(WriteUInt16)
				},
				{
					typeof(uint),
					new Action<string, uint>(WriteUInt32)
				},
				{
					typeof(ulong),
					new Action<string, ulong>(WriteUInt64)
				},
				{
					typeof(decimal),
					new Action<string, decimal>(WriteDecimal)
				},
				{
					typeof(bool),
					new Action<string, bool>(WriteBoolean)
				},
				{
					typeof(float),
					new Action<string, float>(WriteSingle)
				},
				{
					typeof(double),
					new Action<string, double>(WriteDouble)
				},
				{
					typeof(Guid),
					new Action<string, Guid>(WriteGuid)
				}
			};
		}

		public void MarkJustStarted()
		{
			justStarted = true;
		}

		public override void FlushToStream()
		{
			writer.Flush();
		}

		public override void BeginReferenceNode(string name, Type type, int id)
		{
			WriteEntry(name, "{");
			PushNode(name, id, type);
			forceNoSeparatorNextLine = true;
			WriteInt32("$id", id);
			if (type != null)
			{
				WriteTypeEntry(type);
			}
		}

		public override void BeginStructNode(string name, Type type)
		{
			WriteEntry(name, "{");
			PushNode(name, -1, type);
			forceNoSeparatorNextLine = true;
			if (type != null)
			{
				WriteTypeEntry(type);
			}
		}

		public override void EndNode(string name)
		{
			PopNode(name);
			StartNewLine(true);
			writer.Write("}");
		}

		public override void BeginArrayNode(long length)
		{
			WriteInt64("$rlength", length);
			WriteEntry("$rcontent", "[");
			forceNoSeparatorNextLine = true;
			PushArray();
		}

		public override void EndArrayNode()
		{
			PopArray();
			StartNewLine(true);
			writer.Write("]");
		}

		public override void WritePrimitiveArray<T>(T[] array)
		{
			if (!FormatterUtilities.IsPrimitiveArrayType(typeof(T)))
			{
				throw new ArgumentException("Type " + typeof(T).Name + " is not a valid primitive array type.");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Action<string, T> action = (Action<string, T>)primitiveTypeWriters[typeof(T)];
			WriteInt64("$plength", array.Length);
			WriteEntry("$pcontent", "[");
			forceNoSeparatorNextLine = true;
			PushArray();
			for (int i = 0; i < array.Length; i++)
			{
				action(null, array[i]);
			}
			PopArray();
			StartNewLine(true);
			writer.Write("]");
		}

		public override void WriteBoolean(string name, bool value)
		{
			WriteEntry(name, value ? "true" : "false");
		}

		public override void WriteByte(string name, byte value)
		{
			WriteUInt64(name, value);
		}

		public override void WriteChar(string name, char value)
		{
			WriteString(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void WriteDecimal(string name, decimal value)
		{
			WriteEntry(name, value.ToString("G", CultureInfo.InvariantCulture));
		}

		public override void WriteDouble(string name, double value)
		{
			WriteEntry(name, value.ToString("R", CultureInfo.InvariantCulture));
		}

		public override void WriteInt32(string name, int value)
		{
			WriteInt64(name, value);
		}

		public override void WriteInt64(string name, long value)
		{
			WriteEntry(name, value.ToString("D", CultureInfo.InvariantCulture));
		}

		public override void WriteNull(string name)
		{
			WriteEntry(name, "null");
		}

		public override void WriteInternalReference(string name, int id)
		{
			WriteEntry(name, "$iref:" + id.ToString("D", CultureInfo.InvariantCulture));
		}

		public override void WriteSByte(string name, sbyte value)
		{
			WriteInt64(name, value);
		}

		public override void WriteInt16(string name, short value)
		{
			WriteInt64(name, value);
		}

		public override void WriteSingle(string name, float value)
		{
			WriteEntry(name, value.ToString("R", CultureInfo.InvariantCulture));
		}

		public override void WriteString(string name, string value)
		{
			WriteEntry(name, EscapeString(value), '"');
		}

		public override void WriteGuid(string name, Guid value)
		{
			WriteEntry(name, value.ToString("D", CultureInfo.InvariantCulture));
		}

		public override void WriteUInt32(string name, uint value)
		{
			WriteUInt64(name, value);
		}

		public override void WriteUInt64(string name, ulong value)
		{
			WriteEntry(name, value.ToString("D", CultureInfo.InvariantCulture));
		}

		public override void WriteExternalReference(string name, int index)
		{
			WriteEntry(name, "$eref:" + index.ToString("D", CultureInfo.InvariantCulture));
		}

		public override void WriteExternalReference(string name, Guid guid)
		{
			WriteEntry(name, "$guidref:" + guid.ToString("D", CultureInfo.InvariantCulture));
		}

		public override void WriteExternalReference(string name, string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			WriteEntry(name, "$strref:" + id);
		}

		public override void WriteUInt16(string name, ushort value)
		{
			WriteUInt64(name, value);
		}

		public override void Dispose()
		{
			writer.Dispose();
		}

		public override void PrepareNewSerializationSession()
		{
			seenTypes.Clear();
			justStarted = true;
		}

		private string EscapeString(string str)
		{
			bool flag = false;
			foreach (char c in str)
			{
				int num = Convert.ToInt32(c);
				if (num < 0 || num > 127)
				{
					flag = true;
					break;
				}
				switch (c)
				{
				case '\0':
				case '\a':
				case '\b':
				case '\t':
				case '\n':
				case '\f':
				case '\r':
				case '"':
				case '\\':
					flag = true;
					break;
				}
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				return str;
			}
			if (escapeStringBuilder == null)
			{
				escapeStringBuilder = new StringBuilder(str.Length * 2);
			}
			else
			{
				escapeStringBuilder.Length = 0;
			}
			StringBuilder stringBuilder = escapeStringBuilder;
			foreach (char c2 in str)
			{
				int num2 = Convert.ToInt32(c2);
				if (num2 < 0 || num2 > 127)
				{
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "\\u{0:x4} ", num2).Trim());
					continue;
				}
				switch (c2)
				{
				case '"':
					stringBuilder.Append("\\\"");
					break;
				case '\\':
					stringBuilder.Append("\\\\");
					break;
				case '\a':
					stringBuilder.Append("\\a");
					break;
				case '\b':
					stringBuilder.Append("\\b");
					break;
				case '\f':
					stringBuilder.Append("\\f");
					break;
				case '\n':
					stringBuilder.Append("\\n");
					break;
				case '\r':
					stringBuilder.Append("\\r");
					break;
				case '\t':
					stringBuilder.Append("\\t");
					break;
				case '\0':
					stringBuilder.Append("\\0");
					break;
				default:
					stringBuilder.Append(c2);
					break;
				}
			}
			return stringBuilder.ToString();
		}

		private void WriteEntry(string name, string contents, char? surroundContentsWith = null)
		{
			StartNewLine();
			if (name != null)
			{
				writer.Write('"');
				writer.Write(name);
				writer.Write("\":");
				if (FormatAsReadable)
				{
					writer.Write(' ');
				}
			}
			if (surroundContentsWith.HasValue)
			{
				writer.Write(surroundContentsWith.Value);
			}
			writer.Write(contents);
			if (surroundContentsWith.HasValue)
			{
				writer.Write(surroundContentsWith.Value);
			}
		}

		private void WriteTypeEntry(Type type)
		{
			if (EnableTypeOptimization)
			{
				int value;
				if (seenTypes.TryGetValue(type, out value))
				{
					WriteInt32("$type", value);
					return;
				}
				value = seenTypes.Count;
				seenTypes.Add(type, value);
				WriteString("$type", value + "|" + base.Binder.BindToName(type, base.Context.Config.DebugContext));
			}
			else
			{
				WriteString("$type", base.Binder.BindToName(type, base.Context.Config.DebugContext));
			}
		}

		private void StartNewLine(bool noSeparator = false)
		{
			if (justStarted)
			{
				justStarted = false;
				return;
			}
			if (!noSeparator && !forceNoSeparatorNextLine)
			{
				writer.Write(',');
			}
			forceNoSeparatorNextLine = false;
			if (FormatAsReadable)
			{
				writer.Write(Environment.NewLine);
				int num = base.NodeDepth * 4;
				for (int i = 0; i < num; i++)
				{
					writer.Write(' ');
				}
			}
		}
	}
}
