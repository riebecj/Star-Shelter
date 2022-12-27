namespace Phonon
{
	public struct AudioFormat
	{
		public ChannelLayoutType channelLayoutType;

		public ChannelLayout channelLayout;

		public int numSpeakers;

		public Vector3[] speakerDirections;

		public int ambisonicsOrder;

		public AmbisonicsOrdering ambisonicsOrdering;

		public AmbisonicsNormalization ambisonicsNormalization;

		public ChannelOrder channelOrder;
	}
}
