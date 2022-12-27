using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class OVRGearVrControllerTest : MonoBehaviour
{
	public class BoolMonitor
	{
		public delegate bool BoolGenerator();

		private string m_name = string.Empty;

		private BoolGenerator m_generator;

		private bool m_prevValue;

		private bool m_currentValue;

		private bool m_currentValueRecentlyChanged;

		private float m_displayTimeout;

		private float m_displayTimer;

		public BoolMonitor(string name, BoolGenerator generator, float displayTimeout = 0.5f)
		{
			m_name = name;
			m_generator = generator;
			m_displayTimeout = displayTimeout;
		}

		public void Update()
		{
			m_prevValue = m_currentValue;
			m_currentValue = m_generator();
			if (m_currentValue != m_prevValue)
			{
				m_currentValueRecentlyChanged = true;
				m_displayTimer = m_displayTimeout;
			}
			if (m_displayTimer > 0f)
			{
				m_displayTimer -= Time.deltaTime;
				if (m_displayTimer <= 0f)
				{
					m_currentValueRecentlyChanged = false;
					m_displayTimer = 0f;
				}
			}
		}

		public void AppendToStringBuilder(ref StringBuilder sb)
		{
			sb.Append(m_name);
			if (m_currentValue && m_currentValueRecentlyChanged)
			{
				sb.Append(": *True*\n");
			}
			else if (m_currentValue)
			{
				sb.Append(":  True \n");
			}
			else if (!m_currentValue && m_currentValueRecentlyChanged)
			{
				sb.Append(": *False*\n");
			}
			else if (!m_currentValue)
			{
				sb.Append(":  False \n");
			}
		}
	}

	public Text uiText;

	private List<BoolMonitor> monitors;

	private StringBuilder data;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache0;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache1;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache2;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache3;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache4;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache5;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache6;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache7;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache8;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache9;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cacheA;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cacheB;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cacheC;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cacheD;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cacheE;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cacheF;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache10;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache11;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache12;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache13;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache14;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache15;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache16;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache17;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache18;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache19;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache1A;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache1B;

	[CompilerGenerated]
	private static BoolMonitor.BoolGenerator _003C_003Ef__am_0024cache1C;

	private void Start()
	{
		if (uiText != null)
		{
			uiText.supportRichText = false;
		}
		data = new StringBuilder(2048);
		List<BoolMonitor> list = new List<BoolMonitor>();
		if (_003C_003Ef__am_0024cache0 == null)
		{
			_003C_003Ef__am_0024cache0 = _003CStart_003Em__0;
		}
		list.Add(new BoolMonitor("WasRecentered", _003C_003Ef__am_0024cache0));
		if (_003C_003Ef__am_0024cache1 == null)
		{
			_003C_003Ef__am_0024cache1 = _003CStart_003Em__1;
		}
		list.Add(new BoolMonitor("One", _003C_003Ef__am_0024cache1));
		if (_003C_003Ef__am_0024cache2 == null)
		{
			_003C_003Ef__am_0024cache2 = _003CStart_003Em__2;
		}
		list.Add(new BoolMonitor("OneDown", _003C_003Ef__am_0024cache2));
		if (_003C_003Ef__am_0024cache3 == null)
		{
			_003C_003Ef__am_0024cache3 = _003CStart_003Em__3;
		}
		list.Add(new BoolMonitor("OneUp", _003C_003Ef__am_0024cache3));
		if (_003C_003Ef__am_0024cache4 == null)
		{
			_003C_003Ef__am_0024cache4 = _003CStart_003Em__4;
		}
		list.Add(new BoolMonitor("Two", _003C_003Ef__am_0024cache4));
		if (_003C_003Ef__am_0024cache5 == null)
		{
			_003C_003Ef__am_0024cache5 = _003CStart_003Em__5;
		}
		list.Add(new BoolMonitor("TwoDown", _003C_003Ef__am_0024cache5));
		if (_003C_003Ef__am_0024cache6 == null)
		{
			_003C_003Ef__am_0024cache6 = _003CStart_003Em__6;
		}
		list.Add(new BoolMonitor("TwoUp", _003C_003Ef__am_0024cache6));
		if (_003C_003Ef__am_0024cache7 == null)
		{
			_003C_003Ef__am_0024cache7 = _003CStart_003Em__7;
		}
		list.Add(new BoolMonitor("PrimaryIndexTrigger", _003C_003Ef__am_0024cache7));
		if (_003C_003Ef__am_0024cache8 == null)
		{
			_003C_003Ef__am_0024cache8 = _003CStart_003Em__8;
		}
		list.Add(new BoolMonitor("PrimaryIndexTriggerDown", _003C_003Ef__am_0024cache8));
		if (_003C_003Ef__am_0024cache9 == null)
		{
			_003C_003Ef__am_0024cache9 = _003CStart_003Em__9;
		}
		list.Add(new BoolMonitor("PrimaryIndexTriggerUp", _003C_003Ef__am_0024cache9));
		if (_003C_003Ef__am_0024cacheA == null)
		{
			_003C_003Ef__am_0024cacheA = _003CStart_003Em__A;
		}
		list.Add(new BoolMonitor("Up", _003C_003Ef__am_0024cacheA));
		if (_003C_003Ef__am_0024cacheB == null)
		{
			_003C_003Ef__am_0024cacheB = _003CStart_003Em__B;
		}
		list.Add(new BoolMonitor("Down", _003C_003Ef__am_0024cacheB));
		if (_003C_003Ef__am_0024cacheC == null)
		{
			_003C_003Ef__am_0024cacheC = _003CStart_003Em__C;
		}
		list.Add(new BoolMonitor("Left", _003C_003Ef__am_0024cacheC));
		if (_003C_003Ef__am_0024cacheD == null)
		{
			_003C_003Ef__am_0024cacheD = _003CStart_003Em__D;
		}
		list.Add(new BoolMonitor("Right", _003C_003Ef__am_0024cacheD));
		if (_003C_003Ef__am_0024cacheE == null)
		{
			_003C_003Ef__am_0024cacheE = _003CStart_003Em__E;
		}
		list.Add(new BoolMonitor("Touchpad (Touch)", _003C_003Ef__am_0024cacheE));
		if (_003C_003Ef__am_0024cacheF == null)
		{
			_003C_003Ef__am_0024cacheF = _003CStart_003Em__F;
		}
		list.Add(new BoolMonitor("TouchpadDown (Touch)", _003C_003Ef__am_0024cacheF));
		if (_003C_003Ef__am_0024cache10 == null)
		{
			_003C_003Ef__am_0024cache10 = _003CStart_003Em__10;
		}
		list.Add(new BoolMonitor("TouchpadUp (Touch)", _003C_003Ef__am_0024cache10));
		if (_003C_003Ef__am_0024cache11 == null)
		{
			_003C_003Ef__am_0024cache11 = _003CStart_003Em__11;
		}
		list.Add(new BoolMonitor("Touchpad (Click)", _003C_003Ef__am_0024cache11));
		if (_003C_003Ef__am_0024cache12 == null)
		{
			_003C_003Ef__am_0024cache12 = _003CStart_003Em__12;
		}
		list.Add(new BoolMonitor("TouchpadDown (Click)", _003C_003Ef__am_0024cache12));
		if (_003C_003Ef__am_0024cache13 == null)
		{
			_003C_003Ef__am_0024cache13 = _003CStart_003Em__13;
		}
		list.Add(new BoolMonitor("TouchpadUp (Click)", _003C_003Ef__am_0024cache13));
		if (_003C_003Ef__am_0024cache14 == null)
		{
			_003C_003Ef__am_0024cache14 = _003CStart_003Em__14;
		}
		list.Add(new BoolMonitor("Start", _003C_003Ef__am_0024cache14));
		if (_003C_003Ef__am_0024cache15 == null)
		{
			_003C_003Ef__am_0024cache15 = _003CStart_003Em__15;
		}
		list.Add(new BoolMonitor("StartDown", _003C_003Ef__am_0024cache15));
		if (_003C_003Ef__am_0024cache16 == null)
		{
			_003C_003Ef__am_0024cache16 = _003CStart_003Em__16;
		}
		list.Add(new BoolMonitor("StartUp", _003C_003Ef__am_0024cache16));
		if (_003C_003Ef__am_0024cache17 == null)
		{
			_003C_003Ef__am_0024cache17 = _003CStart_003Em__17;
		}
		list.Add(new BoolMonitor("Back", _003C_003Ef__am_0024cache17));
		if (_003C_003Ef__am_0024cache18 == null)
		{
			_003C_003Ef__am_0024cache18 = _003CStart_003Em__18;
		}
		list.Add(new BoolMonitor("BackDown", _003C_003Ef__am_0024cache18));
		if (_003C_003Ef__am_0024cache19 == null)
		{
			_003C_003Ef__am_0024cache19 = _003CStart_003Em__19;
		}
		list.Add(new BoolMonitor("BackUp", _003C_003Ef__am_0024cache19));
		if (_003C_003Ef__am_0024cache1A == null)
		{
			_003C_003Ef__am_0024cache1A = _003CStart_003Em__1A;
		}
		list.Add(new BoolMonitor("A", _003C_003Ef__am_0024cache1A));
		if (_003C_003Ef__am_0024cache1B == null)
		{
			_003C_003Ef__am_0024cache1B = _003CStart_003Em__1B;
		}
		list.Add(new BoolMonitor("ADown", _003C_003Ef__am_0024cache1B));
		if (_003C_003Ef__am_0024cache1C == null)
		{
			_003C_003Ef__am_0024cache1C = _003CStart_003Em__1C;
		}
		list.Add(new BoolMonitor("AUp", _003C_003Ef__am_0024cache1C));
		monitors = list;
	}

	private void Update()
	{
		OVRInput.Controller activeController = OVRInput.GetActiveController();
		data.Length = 0;
		byte controllerRecenterCount = OVRInput.GetControllerRecenterCount();
		data.AppendFormat("RecenterCount: {0}\n", controllerRecenterCount);
		byte controllerBatteryPercentRemaining = OVRInput.GetControllerBatteryPercentRemaining();
		data.AppendFormat("Battery: {0}\n", controllerBatteryPercentRemaining);
		float appFramerate = OVRPlugin.GetAppFramerate();
		data.AppendFormat("Framerate: {0:F2}\n", appFramerate);
		string arg = activeController.ToString();
		data.AppendFormat("Active: {0}\n", arg);
		string arg2 = OVRInput.GetConnectedControllers().ToString();
		data.AppendFormat("Connected: {0}\n", arg2);
		Quaternion localControllerRotation = OVRInput.GetLocalControllerRotation(activeController);
		data.AppendFormat("Orientation: ({0:F2}, {1:F2}, {2:F2}, {3:F2})\n", localControllerRotation.x, localControllerRotation.y, localControllerRotation.z, localControllerRotation.w);
		Vector3 localControllerAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(activeController);
		data.AppendFormat("AngVel: ({0:F2}, {1:F2}, {2:F2})\n", localControllerAngularVelocity.x, localControllerAngularVelocity.y, localControllerAngularVelocity.z);
		Vector3 localControllerAngularAcceleration = OVRInput.GetLocalControllerAngularAcceleration(activeController);
		data.AppendFormat("AngAcc: ({0:F2}, {1:F2}, {2:F2})\n", localControllerAngularAcceleration.x, localControllerAngularAcceleration.y, localControllerAngularAcceleration.z);
		Vector3 localControllerPosition = OVRInput.GetLocalControllerPosition(activeController);
		data.AppendFormat("Position: ({0:F2}, {1:F2}, {2:F2})\n", localControllerPosition.x, localControllerPosition.y, localControllerPosition.z);
		Vector3 localControllerVelocity = OVRInput.GetLocalControllerVelocity(activeController);
		data.AppendFormat("Vel: ({0:F2}, {1:F2}, {2:F2})\n", localControllerVelocity.x, localControllerVelocity.y, localControllerVelocity.z);
		Vector3 localControllerAcceleration = OVRInput.GetLocalControllerAcceleration(activeController);
		data.AppendFormat("Acc: ({0:F2}, {1:F2}, {2:F2})\n", localControllerAcceleration.x, localControllerAcceleration.y, localControllerAcceleration.z);
		Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
		data.AppendFormat("PrimaryTouchpad: ({0:F2}, {1:F2})\n", vector.x, vector.y);
		Vector2 vector2 = OVRInput.Get(OVRInput.Axis2D.SecondaryTouchpad);
		data.AppendFormat("SecondaryTouchpad: ({0:F2}, {1:F2})\n", vector2.x, vector2.y);
		for (int i = 0; i < monitors.Count; i++)
		{
			monitors[i].Update();
			monitors[i].AppendToStringBuilder(ref data);
		}
		if (uiText != null)
		{
			uiText.text = data.ToString();
		}
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__0()
	{
		return OVRInput.GetControllerWasRecentered();
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__1()
	{
		return OVRInput.Get(OVRInput.Button.One);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__2()
	{
		return OVRInput.GetDown(OVRInput.Button.One);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__3()
	{
		return OVRInput.GetUp(OVRInput.Button.One);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__4()
	{
		return OVRInput.Get(OVRInput.Button.Two);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__5()
	{
		return OVRInput.GetDown(OVRInput.Button.Two);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__6()
	{
		return OVRInput.GetUp(OVRInput.Button.Two);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__7()
	{
		return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__8()
	{
		return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__9()
	{
		return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__A()
	{
		return OVRInput.Get(OVRInput.Button.Up);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__B()
	{
		return OVRInput.Get(OVRInput.Button.Down);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__C()
	{
		return OVRInput.Get(OVRInput.Button.Left);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__D()
	{
		return OVRInput.Get(OVRInput.Button.Right);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__E()
	{
		return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__F()
	{
		return OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__10()
	{
		return OVRInput.GetUp(OVRInput.Touch.PrimaryTouchpad);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__11()
	{
		return OVRInput.Get(OVRInput.Button.PrimaryTouchpad);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__12()
	{
		return OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__13()
	{
		return OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__14()
	{
		return OVRInput.Get(OVRInput.RawButton.Start);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__15()
	{
		return OVRInput.GetDown(OVRInput.RawButton.Start);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__16()
	{
		return OVRInput.GetUp(OVRInput.RawButton.Start);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__17()
	{
		return OVRInput.Get(OVRInput.RawButton.Back);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__18()
	{
		return OVRInput.GetDown(OVRInput.RawButton.Back);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__19()
	{
		return OVRInput.GetUp(OVRInput.RawButton.Back);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__1A()
	{
		return OVRInput.Get(OVRInput.RawButton.A);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__1B()
	{
		return OVRInput.GetDown(OVRInput.RawButton.A);
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__1C()
	{
		return OVRInput.GetUp(OVRInput.RawButton.A);
	}
}
