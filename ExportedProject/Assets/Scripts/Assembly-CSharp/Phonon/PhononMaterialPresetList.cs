namespace Phonon
{
	public static class PhononMaterialPresetList
	{
		private static PhononMaterialValue[] values;

		private static bool IsInitialized()
		{
			return values != null;
		}

		public static void Initialize()
		{
			int num = 12;
			values = new PhononMaterialValue[num];
			values[0] = new PhononMaterialValue(0.1f, 0.2f, 0.3f);
			values[1] = new PhononMaterialValue(0.03f, 0.04f, 0.07f);
			values[2] = new PhononMaterialValue(0.05f, 0.07f, 0.08f);
			values[3] = new PhononMaterialValue(0.01f, 0.02f, 0.02f);
			values[4] = new PhononMaterialValue(0.6f, 0.7f, 0.8f);
			values[5] = new PhononMaterialValue(0.24f, 0.69f, 0.73f);
			values[6] = new PhononMaterialValue(0.06f, 0.03f, 0.02f);
			values[7] = new PhononMaterialValue(0.12f, 0.06f, 0.04f);
			values[8] = new PhononMaterialValue(0.11f, 0.07f, 0.06f);
			values[9] = new PhononMaterialValue(0.2f, 0.07f, 0.06f);
			values[10] = new PhononMaterialValue(0.13f, 0.2f, 0.24f);
			values[11] = new PhononMaterialValue();
		}

		public static PhononMaterialValue PresetValue(int index)
		{
			if (!IsInitialized())
			{
				Initialize();
			}
			return values[index];
		}
	}
}
