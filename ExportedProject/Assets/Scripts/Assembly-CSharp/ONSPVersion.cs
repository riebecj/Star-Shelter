using System.Runtime.InteropServices;
using UnityEngine;

public class ONSPVersion : MonoBehaviour
{
	public const string strONSPS = "AudioPluginOculusSpatializer";

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern void ONSP_GetVersion(ref int Major, ref int Minor, ref int Patch);

	private void Awake()
	{
		int Major = 0;
		int Minor = 0;
		int Patch = 0;
		ONSP_GetVersion(ref Major, ref Minor, ref Patch);
		string message = string.Format("ONSP Version: {0:F0}.{1:F0}.{2:F0}", Major, Minor, Patch);
		Debug.Log(message);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
