using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sirenix.Serialization
{
	public sealed class MultiDimensionalArrayFormatter<TArray, TElement> : BaseFormatter<TArray> where TArray : class
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass6_0
		{
			public IDataReader reader;

			public string name;
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass6_1
		{
			public int elements;

			public long length;

			public _003C_003Ec__DisplayClass6_0 CS_0024_003C_003E8__locals1;

			internal TElement _003CDeserializeImplementation_003Eb__0()
			{
				if (CS_0024_003C_003E8__locals1.reader.PeekEntry(out CS_0024_003C_003E8__locals1.name) == EntryType.EndOfArray)
				{
					CS_0024_003C_003E8__locals1.reader.Context.Config.DebugContext.LogError("Reached end of array after " + elements + " elements, when " + length + " elements were expected.");
					throw new InvalidOperationException();
				}
				TElement result = MultiDimensionalArrayFormatter<TArray, TElement>.ValueReaderWriter.ReadValue(CS_0024_003C_003E8__locals1.reader);
				if (!CS_0024_003C_003E8__locals1.reader.IsInArrayNode)
				{
					CS_0024_003C_003E8__locals1.reader.Context.Config.DebugContext.LogError("Reading array went wrong at position " + CS_0024_003C_003E8__locals1.reader.Stream.Position + ".");
					throw new InvalidOperationException();
				}
				elements++;
				return result;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass7_0
		{
			public IDataWriter writer;

			internal void _003CSerializeImplementation_003Eb__0(TElement v)
			{
				MultiDimensionalArrayFormatter<TArray, TElement>.ValueReaderWriter.WriteValue(v, writer);
			}
		}

		private const string RANKS_NAME = "ranks";

		private const char RANKS_SEPARATOR = '|';

		private static readonly int ArrayRank;

		private static readonly Serializer<TElement> ValueReaderWriter;

		static MultiDimensionalArrayFormatter()
		{
			ValueReaderWriter = Serializer.Get<TElement>();
			if (!typeof(TArray).IsArray)
			{
				throw new ArgumentException("Type " + typeof(TArray).Name + " is not an array.");
			}
			if (typeof(TArray).GetElementType() != typeof(TElement))
			{
				throw new ArgumentException("Array of type " + typeof(TArray).Name + " does not have the required element type of " + typeof(TElement).Name + ".");
			}
			ArrayRank = typeof(TArray).GetArrayRank();
			if (ArrayRank <= 1)
			{
				throw new ArgumentException("Array of type " + typeof(TArray).Name + " only has one rank.");
			}
		}

		protected override TArray GetUninitializedObject()
		{
			return null;
		}

		protected override void DeserializeImplementation(ref TArray value, IDataReader reader)
		{
			_003C_003Ec__DisplayClass6_0 _003C_003Ec__DisplayClass6_ = new _003C_003Ec__DisplayClass6_0();
			_003C_003Ec__DisplayClass6_.reader = reader;
			EntryType entryType = _003C_003Ec__DisplayClass6_.reader.PeekEntry(out _003C_003Ec__DisplayClass6_.name);
			if (entryType == EntryType.StartOfArray)
			{
				_003C_003Ec__DisplayClass6_1 _003C_003Ec__DisplayClass6_2 = new _003C_003Ec__DisplayClass6_1();
				_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass6_;
				_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.EnterArray(out _003C_003Ec__DisplayClass6_2.length);
				entryType = _003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.PeekEntry(out _003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.name);
				if (entryType != EntryType.String || _003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.name != "ranks")
				{
					value = null;
					_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.SkipEntry();
					return;
				}
				string value2;
				_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.ReadString(out value2);
				string[] array = value2.Split('|');
				if (array.Length != ArrayRank)
				{
					value = null;
					_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.SkipEntry();
					return;
				}
				int[] array2 = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					int result;
					if (int.TryParse(array[i], out result))
					{
						array2[i] = result;
						continue;
					}
					value = null;
					_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.SkipEntry();
					return;
				}
				long num = array2[0];
				for (int j = 1; j < array2.Length; j++)
				{
					num *= array2[j];
				}
				if (num != _003C_003Ec__DisplayClass6_2.length)
				{
					value = null;
					_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.SkipEntry();
					return;
				}
				value = (TArray)(object)Array.CreateInstance(typeof(TElement), array2);
				RegisterReferenceID(value, _003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader);
				_003C_003Ec__DisplayClass6_2.elements = 0;
				try
				{
					IterateArrayWrite((Array)(object)value, _003C_003Ec__DisplayClass6_2._003CDeserializeImplementation_003Eb__0);
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception exception)
				{
					_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.Context.Config.DebugContext.LogException(exception);
				}
				_003C_003Ec__DisplayClass6_2.CS_0024_003C_003E8__locals1.reader.ExitArray();
			}
			else
			{
				value = null;
				_003C_003Ec__DisplayClass6_.reader.SkipEntry();
			}
		}

		protected override void SerializeImplementation(ref TArray value, IDataWriter writer)
		{
			_003C_003Ec__DisplayClass7_0 _003C_003Ec__DisplayClass7_ = new _003C_003Ec__DisplayClass7_0();
			_003C_003Ec__DisplayClass7_.writer = writer;
			Array array = value as Array;
			try
			{
				_003C_003Ec__DisplayClass7_.writer.BeginArrayNode(array.LongLength);
				int[] array2 = new int[ArrayRank];
				for (int i = 0; i < ArrayRank; i++)
				{
					array2[i] = array.GetLength(i);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < ArrayRank; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append('|');
					}
					stringBuilder.Append(array2[j].ToString(CultureInfo.InvariantCulture));
				}
				string value2 = stringBuilder.ToString();
				_003C_003Ec__DisplayClass7_.writer.WriteString("ranks", value2);
				IterateArrayRead((Array)(object)value, _003C_003Ec__DisplayClass7_._003CSerializeImplementation_003Eb__0);
			}
			finally
			{
				_003C_003Ec__DisplayClass7_.writer.EndArrayNode();
			}
		}

		private void IterateArrayWrite(Array a, Func<TElement> write)
		{
			int[] indices = new int[ArrayRank];
			IterateArrayWrite(a, 0, indices, write);
		}

		private void IterateArrayWrite(Array a, int rank, int[] indices, Func<TElement> write)
		{
			for (int i = 0; i < a.GetLength(rank); i++)
			{
				indices[rank] = i;
				if (rank + 1 < a.Rank)
				{
					IterateArrayWrite(a, rank + 1, indices, write);
				}
				else
				{
					a.SetValue(write(), indices);
				}
			}
		}

		private void IterateArrayRead(Array a, Action<TElement> read)
		{
			int[] indices = new int[ArrayRank];
			IterateArrayRead(a, 0, indices, read);
		}

		private void IterateArrayRead(Array a, int rank, int[] indices, Action<TElement> read)
		{
			for (int i = 0; i < a.GetLength(rank); i++)
			{
				indices[rank] = i;
				if (rank + 1 < a.Rank)
				{
					IterateArrayRead(a, rank + 1, indices, read);
				}
				else
				{
					read((TElement)a.GetValue(indices));
				}
			}
		}
	}
}
