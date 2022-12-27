using System;
using System.Collections.Generic;
using System.Text;

namespace OscJack
{
	public class OscParser
	{
		private Queue<OscMessage> _messageQueue;

		private byte[] _readBuffer;

		private int _readPoint;

		public int MessageCount
		{
			get
			{
				return _messageQueue.Count;
			}
		}

		public OscParser()
		{
			_messageQueue = new Queue<OscMessage>();
		}

		public OscMessage PopMessage()
		{
			return _messageQueue.Dequeue();
		}

		public void FeedData(byte[] data)
		{
			_readBuffer = data;
			_readPoint = 0;
			ReadMessage();
			_readBuffer = null;
		}

		private void ReadMessage()
		{
			string text = ReadString();
			if (text == "#bundle")
			{
				ReadInt64();
				while (true)
				{
					if (_readPoint >= _readBuffer.Length)
					{
						return;
					}
					byte b = _readBuffer[_readPoint];
					if (b == 47 || b == 35)
					{
						break;
					}
					int num = _readPoint + ReadInt32();
					while (_readPoint < num)
					{
						ReadMessage();
					}
				}
				ReadMessage();
				return;
			}
			string text2 = ReadString();
			OscMessage item = new OscMessage(text, new object[text2.Length - 1]);
			for (int i = 0; i < text2.Length - 1; i++)
			{
				switch (text2[i + 1])
				{
				case 'f':
					item.data[i] = ReadFloat32();
					break;
				case 'i':
					item.data[i] = ReadInt32();
					break;
				case 's':
					item.data[i] = ReadString();
					break;
				case 'b':
					item.data[i] = ReadBlob();
					break;
				}
			}
			_messageQueue.Enqueue(item);
		}

		private float ReadFloat32()
		{
			byte[] value = new byte[4]
			{
				_readBuffer[_readPoint + 3],
				_readBuffer[_readPoint + 2],
				_readBuffer[_readPoint + 1],
				_readBuffer[_readPoint]
			};
			_readPoint += 4;
			return BitConverter.ToSingle(value, 0);
		}

		private int ReadInt32()
		{
			int result = (_readBuffer[_readPoint] << 24) + (_readBuffer[_readPoint + 1] << 16) + (_readBuffer[_readPoint + 2] << 8) + _readBuffer[_readPoint + 3];
			_readPoint += 4;
			return result;
		}

		private long ReadInt64()
		{
			long result = ((long)(int)_readBuffer[_readPoint] << 56) + ((long)(int)_readBuffer[_readPoint + 1] << 48) + ((long)(int)_readBuffer[_readPoint + 2] << 40) + ((long)(int)_readBuffer[_readPoint + 3] << 32) + ((long)(int)_readBuffer[_readPoint + 4] << 24) + ((long)(int)_readBuffer[_readPoint + 5] << 16) + ((long)(int)_readBuffer[_readPoint + 6] << 8) + (int)_readBuffer[_readPoint + 7];
			_readPoint += 8;
			return result;
		}

		private string ReadString()
		{
			int i;
			for (i = 0; _readBuffer[_readPoint + i] != 0; i++)
			{
			}
			string @string = Encoding.UTF8.GetString(_readBuffer, _readPoint, i);
			_readPoint += (i + 4) & -4;
			return @string;
		}

		private byte[] ReadBlob()
		{
			int num = ReadInt32();
			byte[] array = new byte[num];
			Array.Copy(_readBuffer, _readPoint, array, 0, num);
			_readPoint += (num + 3) & -4;
			return array;
		}
	}
}
