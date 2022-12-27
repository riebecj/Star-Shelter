namespace OscJack
{
	public struct OscMessage
	{
		public string address;

		public object[] data;

		public OscMessage(string address, object[] data)
		{
			this.address = address;
			this.data = data;
		}

		public override string ToString()
		{
			string text = address + ":";
			for (int i = 0; i < data.Length - 1; i++)
			{
				text = string.Concat(text, data[i], ",");
			}
			return text + data[data.Length];
		}
	}
}
