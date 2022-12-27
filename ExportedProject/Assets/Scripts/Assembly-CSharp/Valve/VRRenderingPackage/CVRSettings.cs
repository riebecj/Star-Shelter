using System;
using System.Runtime.InteropServices;

namespace Valve.VRRenderingPackage
{
	public class CVRSettings
	{
		private IVRSettings FnTable;

		internal CVRSettings(IntPtr pInterface)
		{
			FnTable = (IVRSettings)Marshal.PtrToStructure(pInterface, typeof(IVRSettings));
		}

		public string GetSettingsErrorNameFromEnum(EVRSettingsError eError)
		{
			IntPtr ptr = FnTable.GetSettingsErrorNameFromEnum(eError);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public bool Sync(bool bForce, ref EVRSettingsError peError)
		{
			return FnTable.Sync(bForce, ref peError);
		}

		public bool GetBool(string pchSection, string pchSettingsKey, bool bDefaultValue, ref EVRSettingsError peError)
		{
			return FnTable.GetBool(pchSection, pchSettingsKey, bDefaultValue, ref peError);
		}

		public void SetBool(string pchSection, string pchSettingsKey, bool bValue, ref EVRSettingsError peError)
		{
			FnTable.SetBool(pchSection, pchSettingsKey, bValue, ref peError);
		}

		public int GetInt32(string pchSection, string pchSettingsKey, int nDefaultValue, ref EVRSettingsError peError)
		{
			return FnTable.GetInt32(pchSection, pchSettingsKey, nDefaultValue, ref peError);
		}

		public void SetInt32(string pchSection, string pchSettingsKey, int nValue, ref EVRSettingsError peError)
		{
			FnTable.SetInt32(pchSection, pchSettingsKey, nValue, ref peError);
		}

		public float GetFloat(string pchSection, string pchSettingsKey, float flDefaultValue, ref EVRSettingsError peError)
		{
			return FnTable.GetFloat(pchSection, pchSettingsKey, flDefaultValue, ref peError);
		}

		public void SetFloat(string pchSection, string pchSettingsKey, float flValue, ref EVRSettingsError peError)
		{
			FnTable.SetFloat(pchSection, pchSettingsKey, flValue, ref peError);
		}

		public void GetString(string pchSection, string pchSettingsKey, string pchValue, uint unValueLen, string pchDefaultValue, ref EVRSettingsError peError)
		{
			FnTable.GetString(pchSection, pchSettingsKey, pchValue, unValueLen, pchDefaultValue, ref peError);
		}

		public void SetString(string pchSection, string pchSettingsKey, string pchValue, ref EVRSettingsError peError)
		{
			FnTable.SetString(pchSection, pchSettingsKey, pchValue, ref peError);
		}

		public void RemoveSection(string pchSection, ref EVRSettingsError peError)
		{
			FnTable.RemoveSection(pchSection, ref peError);
		}

		public void RemoveKeyInSection(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
		{
			FnTable.RemoveKeyInSection(pchSection, pchSettingsKey, ref peError);
		}
	}
}
