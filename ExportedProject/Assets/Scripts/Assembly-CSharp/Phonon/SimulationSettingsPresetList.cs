namespace Phonon
{
	public static class SimulationSettingsPresetList
	{
		private static SimulationSettingsValue[] values;

		private static bool IsInitialized()
		{
			return values != null;
		}

		private static void Initialize()
		{
			int num = 4;
			values = new SimulationSettingsValue[num];
			values[0] = new SimulationSettingsValue(4096, 1024, 2, 16384, 4096, 32, 1f, 1, 32);
			values[1] = new SimulationSettingsValue(8192, 1024, 4, 32768, 4096, 64, 1f, 1, 32);
			values[2] = new SimulationSettingsValue(16384, 1024, 8, 65536, 4096, 128, 1f, 1, 32);
			values[3] = new SimulationSettingsValue();
		}

		public static SimulationSettingsValue PresetValue(int index)
		{
			if (!IsInitialized())
			{
				Initialize();
			}
			return values[index];
		}
	}
}
