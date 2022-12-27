using System;
using System.IO;

namespace Sirenix.Serialization
{
	public interface IDataReader : IDisposable
	{
		TwoWaySerializationBinder Binder { get; set; }

		Stream Stream { get; set; }

		bool IsInArrayNode { get; }

		string CurrentNodeName { get; }

		int CurrentNodeId { get; }

		int CurrentNodeDepth { get; }

		DeserializationContext Context { get; set; }

		bool EnterNode(out Type type);

		bool ExitNode();

		bool EnterArray(out long length);

		bool ExitArray();

		bool ReadPrimitiveArray<T>(out T[] array) where T : struct;

		EntryType PeekEntry(out string name);

		bool ReadInternalReference(out int id);

		bool ReadExternalReference(out int index);

		bool ReadExternalReference(out Guid guid);

		bool ReadExternalReference(out string id);

		bool ReadChar(out char value);

		bool ReadString(out string value);

		bool ReadGuid(out Guid value);

		bool ReadSByte(out sbyte value);

		bool ReadInt16(out short value);

		bool ReadInt32(out int value);

		bool ReadInt64(out long value);

		bool ReadByte(out byte value);

		bool ReadUInt16(out ushort value);

		bool ReadUInt32(out uint value);

		bool ReadUInt64(out ulong value);

		bool ReadDecimal(out decimal value);

		bool ReadSingle(out float value);

		bool ReadDouble(out double value);

		bool ReadBoolean(out bool value);

		bool ReadNull();

		void SkipEntry();

		void PrepareNewSerializationSession();
	}
}
