namespace MidiJack
{
	public struct MidiMessage
	{
		public uint source;

		public byte status;

		public byte data1;

		public byte data2;

		public MidiMessage(ulong data)
		{
			source = (uint)(data & 0xFFFFFFFFu);
			status = (byte)((data >> 32) & 0xFF);
			data1 = (byte)((data >> 40) & 0xFF);
			data2 = (byte)((data >> 48) & 0xFF);
		}

		public override string ToString()
		{
			return string.Format("s({0:X2}) d({1:X2},{2:X2}) from {3:X8}", status, data1, data2, source);
		}
	}
}
