using System.Runtime.InteropServices;
using UnityEngine;

public class ONSPProfiler : MonoBehaviour
{
	public const string strONSPS = "AudioPluginOculusSpatializer";

	public bool profilerEnabled;

	private const int DEFAULT_PORT = 2121;

	public int port = 2121;

	private void Start()
	{
		Application.runInBackground = true;
		if (profilerEnabled)
		{
			Debug.Log("Oculus Audio Profiler enabled, IP address = " + Network.player.ipAddress);
		}
	}

	private void Update()
	{
		if (port < 0 || port > 65535)
		{
			port = 2121;
		}
		ONSP_SetProfilerPort(port);
		ONSP_SetProfilerEnabled(profilerEnabled);
	}

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int ONSP_SetProfilerEnabled(bool enabled);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int ONSP_SetProfilerPort(int port);
}
