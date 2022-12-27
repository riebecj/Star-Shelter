namespace MidiJack
{
	public static class MidiMaster
	{
		public static float GetKey(MidiChannel channel, int noteNumber)
		{
			return MidiDriver.Instance.GetKey(channel, noteNumber);
		}

		public static float GetKey(int noteNumber)
		{
			return MidiDriver.Instance.GetKey(MidiChannel.All, noteNumber);
		}

		public static bool GetKeyDown(MidiChannel channel, int noteNumber)
		{
			return MidiDriver.Instance.GetKeyDown(channel, noteNumber);
		}

		public static bool GetKeyDown(int noteNumber)
		{
			return MidiDriver.Instance.GetKeyDown(MidiChannel.All, noteNumber);
		}

		public static bool GetKeyUp(MidiChannel channel, int noteNumber)
		{
			return MidiDriver.Instance.GetKeyUp(channel, noteNumber);
		}

		public static bool GetKeyUp(int noteNumber)
		{
			return MidiDriver.Instance.GetKeyUp(MidiChannel.All, noteNumber);
		}

		public static int[] GetKnobNumbers(MidiChannel channel)
		{
			return MidiDriver.Instance.GetKnobNumbers(channel);
		}

		public static int[] GetKnobNumbers()
		{
			return MidiDriver.Instance.GetKnobNumbers(MidiChannel.All);
		}

		public static float GetKnob(MidiChannel channel, int knobNumber, float defaultValue = 0f)
		{
			return MidiDriver.Instance.GetKnob(channel, knobNumber, defaultValue);
		}

		public static float GetKnob(int knobNumber, float defaultValue = 0f)
		{
			return MidiDriver.Instance.GetKnob(MidiChannel.All, knobNumber, defaultValue);
		}
	}
}
