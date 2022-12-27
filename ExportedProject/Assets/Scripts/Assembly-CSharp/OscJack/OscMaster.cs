namespace OscJack
{
	public static class OscMaster
	{
		private static int[] listenPortList = new int[1] { 9000 };

		private static OscDirectory _directory = new OscDirectory(listenPortList);

		public static OscDirectory MasterDirectory
		{
			get
			{
				return _directory;
			}
		}

		public static bool HasData(string address)
		{
			return _directory.HasData(address);
		}

		public static object[] GetData(string address)
		{
			return _directory.GetData(address);
		}
	}
}
