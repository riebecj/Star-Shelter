using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MidiJack
{
	public class MidiDriver
	{
		private class ChannelState
		{
			public float[] _noteArray;

			public Dictionary<int, float> _knobMap;

			public ChannelState()
			{
				_noteArray = new float[128];
				_knobMap = new Dictionary<int, float>();
			}
		}

		private ChannelState[] _channelArray;

		private int _lastFrame;

		private static MidiDriver _instance;

		public static MidiDriver Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MidiDriver();
				}
				return _instance;
			}
		}

		private MidiDriver()
		{
			_channelArray = new ChannelState[17];
			for (int i = 0; i < 17; i++)
			{
				_channelArray[i] = new ChannelState();
			}
		}

		public float GetKey(MidiChannel channel, int noteNumber)
		{
			UpdateIfNeeded();
			float num = _channelArray[(int)channel]._noteArray[noteNumber];
			if (num > 1f)
			{
				return num - 1f;
			}
			if (num > 0f)
			{
				return num;
			}
			return 0f;
		}

		public bool GetKeyDown(MidiChannel channel, int noteNumber)
		{
			UpdateIfNeeded();
			return _channelArray[(int)channel]._noteArray[noteNumber] > 1f;
		}

		public bool GetKeyUp(MidiChannel channel, int noteNumber)
		{
			UpdateIfNeeded();
			return _channelArray[(int)channel]._noteArray[noteNumber] < 0f;
		}

		public int[] GetKnobNumbers(MidiChannel channel)
		{
			UpdateIfNeeded();
			ChannelState channelState = _channelArray[(int)channel];
			int[] array = new int[channelState._knobMap.Count];
			channelState._knobMap.Keys.CopyTo(array, 0);
			return array;
		}

		public float GetKnob(MidiChannel channel, int knobNumber, float defaultValue)
		{
			UpdateIfNeeded();
			ChannelState channelState = _channelArray[(int)channel];
			if (channelState._knobMap.ContainsKey(knobNumber))
			{
				return channelState._knobMap[knobNumber];
			}
			return defaultValue;
		}

		private void UpdateIfNeeded()
		{
			if (Application.isPlaying)
			{
				int frameCount = Time.frameCount;
				if (frameCount != _lastFrame)
				{
					Update();
					_lastFrame = frameCount;
				}
			}
		}

		private void Update()
		{
			ChannelState[] channelArray = _channelArray;
			foreach (ChannelState channelState in channelArray)
			{
				for (int j = 0; j < 128; j++)
				{
					float num = channelState._noteArray[j];
					if (num > 1f)
					{
						channelState._noteArray[j] = num - 1f;
					}
					else if (num < 0f)
					{
						channelState._noteArray[j] = 0f;
					}
				}
			}
			while (true)
			{
				ulong num2 = DequeueIncomingData();
				if (num2 == 0)
				{
					break;
				}
				MidiMessage midiMessage = new MidiMessage(num2);
				int num3 = midiMessage.status >> 4;
				int num4 = midiMessage.status & 0xF;
				if (num3 == 9)
				{
					float num5 = 1f / 127f * (float)(int)midiMessage.data2 + 1f;
					_channelArray[num4]._noteArray[midiMessage.data1] = num5;
					_channelArray[16]._noteArray[midiMessage.data1] = num5;
				}
				if (num3 == 8 || (num3 == 9 && midiMessage.data2 == 0))
				{
					_channelArray[num4]._noteArray[midiMessage.data1] = -1f;
					_channelArray[16]._noteArray[midiMessage.data1] = -1f;
				}
				if (num3 == 11)
				{
					float value = 1f / 127f * (float)(int)midiMessage.data2;
					_channelArray[num4]._knobMap[midiMessage.data1] = value;
					_channelArray[16]._knobMap[midiMessage.data1] = value;
				}
			}
		}

		[DllImport("MidiJackPlugin", EntryPoint = "MidiJackDequeueIncomingData")]
		public static extern ulong DequeueIncomingData();
	}
}
