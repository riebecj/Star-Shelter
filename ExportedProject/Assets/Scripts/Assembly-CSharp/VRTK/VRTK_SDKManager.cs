using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VRTK
{
	public class VRTK_SDKManager : MonoBehaviour
	{
		public sealed class ScriptingDefineSymbolPredicateInfo
		{
			public readonly SDK_ScriptingDefineSymbolPredicateAttribute attribute;

			public readonly MethodInfo methodInfo;

			public ScriptingDefineSymbolPredicateInfo(SDK_ScriptingDefineSymbolPredicateAttribute attribute, MethodInfo methodInfo)
			{
				this.attribute = attribute;
				this.methodInfo = methodInfo;
			}
		}

		[CompilerGenerated]
		private sealed class _003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey0
		{
			internal MethodInfo methodInfo;

			internal ScriptingDefineSymbolPredicateInfo _003C_003Em__0(SDK_ScriptingDefineSymbolPredicateAttribute predicateAttribute)
			{
				return new ScriptingDefineSymbolPredicateInfo(predicateAttribute, methodInfo);
			}
		}

		[CompilerGenerated]
		private sealed class _003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey1<BaseType, FallbackType> where BaseType : SDK_Base where FallbackType : BaseType
		{
			internal Type baseType;

			internal Type fallbackType;

			internal ICollection<string> symbolsOfInstalledSDKs;

			internal bool _003C_003Em__0(Type type)
			{
				return type.IsSubclassOf(baseType) && type != fallbackType && !type.IsAbstract;
			}

			internal bool _003C_003Em__1(VRTK_SDKInfo info)
			{
				string symbol = info.description.symbol;
				return string.IsNullOrEmpty(symbol) || symbolsOfInstalledSDKs.Contains(symbol);
			}
		}

		private static VRTK_SDKManager _instance;

		[Tooltip("If this is true then the instance of the SDK Manager won't be destroyed on every scene load.")]
		public bool persistOnLoad;

		[Tooltip("This determines whether the SDK object references are automatically set to the objects of the selected SDKs. If this is true populating is done whenever the selected SDKs change.")]
		public bool autoPopulateObjectReferences = true;

		[Tooltip("This determines whether the scripting define symbols required by the selected SDKs are automatically added to and removed from the player settings. If this is true managing is done whenever the selected SDKs or the current active SDK Manager change in the Editor.")]
		public bool autoManageScriptDefines = true;

		public List<SDK_ScriptingDefineSymbolPredicateAttribute> activeScriptingDefineSymbolsWithoutSDKClasses;

		[Tooltip("A reference to the GameObject that is the user's boundary or play area, most likely provided by the SDK's Camera Rig.")]
		public GameObject actualBoundaries;

		[Tooltip("A reference to the GameObject that contains the VR camera, most likely provided by the SDK's Camera Rig Headset.")]
		public GameObject actualHeadset;

		[Tooltip("A reference to the GameObject that contains the SDK Left Hand Controller.")]
		public GameObject actualLeftController;

		[Tooltip("A reference to the GameObject that contains the SDK Right Hand Controller.")]
		public GameObject actualRightController;

		[Header("Controller Aliases")]
		[Tooltip("A reference to the GameObject that models for the Left Hand Controller.")]
		public GameObject modelAliasLeftController;

		[Tooltip("A reference to the GameObject that models for the Right Hand Controller.")]
		public GameObject modelAliasRightController;

		[Tooltip("A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.")]
		public GameObject scriptAliasLeftController;

		[Tooltip("A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.")]
		public GameObject scriptAliasRightController;

		private static readonly Dictionary<Type, Type> SDKFallbackTypesByBaseType;

		[SerializeField]
		private VRTK_SDKInfo cachedSystemSDKInfo = VRTK_SDKInfo.Create<SDK_BaseSystem, SDK_FallbackSystem, SDK_FallbackSystem>();

		[SerializeField]
		private VRTK_SDKInfo cachedBoundariesSDKInfo = VRTK_SDKInfo.Create<SDK_BaseBoundaries, SDK_FallbackBoundaries, SDK_FallbackBoundaries>();

		[SerializeField]
		private VRTK_SDKInfo cachedHeadsetSDKInfo = VRTK_SDKInfo.Create<SDK_BaseHeadset, SDK_FallbackHeadset, SDK_FallbackHeadset>();

		[SerializeField]
		private VRTK_SDKInfo cachedControllerSDKInfo = VRTK_SDKInfo.Create<SDK_BaseController, SDK_FallbackController, SDK_FallbackController>();

		private SDK_BaseSystem cachedSystemSDK;

		private SDK_BaseBoundaries cachedBoundariesSDK;

		private SDK_BaseHeadset cachedHeadsetSDK;

		private SDK_BaseController cachedControllerSDK;

		[CompilerGenerated]
		private static Comparison<ScriptingDefineSymbolPredicateInfo> _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static Func<ScriptingDefineSymbolPredicateInfo, bool> _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static Func<ScriptingDefineSymbolPredicateInfo, string> _003C_003Ef__am_0024cache2;

		public static ReadOnlyCollection<ScriptingDefineSymbolPredicateInfo> AvailableScriptingDefineSymbolPredicateInfos { get; private set; }

		public static ReadOnlyCollection<VRTK_SDKInfo> AvailableSystemSDKInfos { get; private set; }

		public static ReadOnlyCollection<VRTK_SDKInfo> AvailableBoundariesSDKInfos { get; private set; }

		public static ReadOnlyCollection<VRTK_SDKInfo> AvailableHeadsetSDKInfos { get; private set; }

		public static ReadOnlyCollection<VRTK_SDKInfo> AvailableControllerSDKInfos { get; private set; }

		public static ReadOnlyCollection<VRTK_SDKInfo> InstalledSystemSDKInfos { get; private set; }

		public static ReadOnlyCollection<VRTK_SDKInfo> InstalledBoundariesSDKInfos { get; private set; }

		public static ReadOnlyCollection<VRTK_SDKInfo> InstalledHeadsetSDKInfos { get; private set; }

		public static ReadOnlyCollection<VRTK_SDKInfo> InstalledControllerSDKInfos { get; private set; }

		public static VRTK_SDKManager instance
		{
			get
			{
				if (_instance == null)
				{
					VRTK_SDKManager vRTK_SDKManager = UnityEngine.Object.FindObjectOfType<VRTK_SDKManager>();
					if ((bool)vRTK_SDKManager)
					{
						vRTK_SDKManager.CreateInstance();
					}
				}
				return _instance;
			}
		}

		public VRTK_SDKInfo systemSDKInfo
		{
			get
			{
				return cachedSystemSDKInfo;
			}
			set
			{
				value = value ?? VRTK_SDKInfo.Create<SDK_BaseSystem, SDK_FallbackSystem, SDK_FallbackSystem>();
				if (!(cachedSystemSDKInfo == value))
				{
					UnityEngine.Object.Destroy(cachedSystemSDK);
					cachedSystemSDK = null;
					cachedSystemSDKInfo = new VRTK_SDKInfo(value);
					PopulateObjectReferences(false);
				}
			}
		}

		public VRTK_SDKInfo boundariesSDKInfo
		{
			get
			{
				return cachedBoundariesSDKInfo;
			}
			set
			{
				value = value ?? VRTK_SDKInfo.Create<SDK_BaseBoundaries, SDK_FallbackBoundaries, SDK_FallbackBoundaries>();
				if (!(cachedBoundariesSDKInfo == value))
				{
					UnityEngine.Object.Destroy(cachedBoundariesSDK);
					cachedBoundariesSDK = null;
					cachedBoundariesSDKInfo = new VRTK_SDKInfo(value);
					PopulateObjectReferences(false);
				}
			}
		}

		public VRTK_SDKInfo headsetSDKInfo
		{
			get
			{
				return cachedHeadsetSDKInfo;
			}
			set
			{
				value = value ?? VRTK_SDKInfo.Create<SDK_BaseHeadset, SDK_FallbackHeadset, SDK_FallbackHeadset>();
				if (!(cachedHeadsetSDKInfo == value))
				{
					UnityEngine.Object.Destroy(cachedHeadsetSDK);
					cachedHeadsetSDK = null;
					cachedHeadsetSDKInfo = new VRTK_SDKInfo(value);
					PopulateObjectReferences(false);
				}
			}
		}

		public VRTK_SDKInfo controllerSDKInfo
		{
			get
			{
				return cachedControllerSDKInfo;
			}
			set
			{
				value = value ?? VRTK_SDKInfo.Create<SDK_BaseController, SDK_FallbackController, SDK_FallbackController>();
				if (!(cachedControllerSDKInfo == value))
				{
					UnityEngine.Object.Destroy(cachedControllerSDK);
					cachedControllerSDK = null;
					cachedControllerSDKInfo = new VRTK_SDKInfo(value);
					PopulateObjectReferences(false);
				}
			}
		}

		static VRTK_SDKManager()
		{
			SDKFallbackTypesByBaseType = new Dictionary<Type, Type>
			{
				{
					typeof(SDK_BaseSystem),
					typeof(SDK_FallbackSystem)
				},
				{
					typeof(SDK_BaseBoundaries),
					typeof(SDK_FallbackBoundaries)
				},
				{
					typeof(SDK_BaseHeadset),
					typeof(SDK_FallbackHeadset)
				},
				{
					typeof(SDK_BaseController),
					typeof(SDK_FallbackController)
				}
			};
			PopulateAvailableScriptingDefineSymbolPredicateInfos();
			PopulateAvailableAndInstalledSDKInfos();
		}

		public SDK_BaseSystem GetSystemSDK()
		{
			if (cachedSystemSDK == null)
			{
				HandleSDKGetter<SDK_BaseSystem>("System", systemSDKInfo, InstalledSystemSDKInfos);
				cachedSystemSDK = (SDK_BaseSystem)ScriptableObject.CreateInstance(systemSDKInfo.type);
			}
			return cachedSystemSDK;
		}

		public SDK_BaseBoundaries GetBoundariesSDK()
		{
			if (cachedBoundariesSDK == null)
			{
				HandleSDKGetter<SDK_BaseBoundaries>("Boundaries", boundariesSDKInfo, InstalledBoundariesSDKInfos);
				cachedBoundariesSDK = (SDK_BaseBoundaries)ScriptableObject.CreateInstance(boundariesSDKInfo.type);
			}
			return cachedBoundariesSDK;
		}

		public SDK_BaseHeadset GetHeadsetSDK()
		{
			if (cachedHeadsetSDK == null)
			{
				HandleSDKGetter<SDK_BaseHeadset>("Headset", headsetSDKInfo, InstalledHeadsetSDKInfos);
				cachedHeadsetSDK = (SDK_BaseHeadset)ScriptableObject.CreateInstance(headsetSDKInfo.type);
			}
			return cachedHeadsetSDK;
		}

		public SDK_BaseController GetControllerSDK()
		{
			if (cachedControllerSDK == null)
			{
				HandleSDKGetter<SDK_BaseController>("Controller", controllerSDKInfo, InstalledControllerSDKInfos);
				cachedControllerSDK = (SDK_BaseController)ScriptableObject.CreateInstance(controllerSDKInfo.type);
			}
			return cachedControllerSDK;
		}

		public void PopulateObjectReferences(bool force)
		{
			if (force || autoPopulateObjectReferences)
			{
				actualBoundaries = null;
				actualHeadset = null;
				actualLeftController = null;
				actualRightController = null;
				modelAliasLeftController = null;
				modelAliasRightController = null;
				SDK_BaseBoundaries boundariesSDK = GetBoundariesSDK();
				SDK_BaseHeadset headsetSDK = GetHeadsetSDK();
				SDK_BaseController controllerSDK = GetControllerSDK();
				Transform playArea = boundariesSDK.GetPlayArea();
				Transform headset = headsetSDK.GetHeadset();
				actualBoundaries = ((!(playArea == null)) ? playArea.gameObject : null);
				actualHeadset = ((!(headset == null)) ? headset.gameObject : null);
				actualLeftController = controllerSDK.GetControllerLeftHand(true);
				actualRightController = controllerSDK.GetControllerRightHand(true);
				modelAliasLeftController = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Left);
				modelAliasRightController = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Right);
			}
		}

		public string[] GetSimplifiedSDKErrorDescriptions()
		{
			List<string> list = new List<string>();
			ReadOnlyCollection<VRTK_SDKInfo>[] array = new ReadOnlyCollection<VRTK_SDKInfo>[4] { InstalledSystemSDKInfos, InstalledBoundariesSDKInfos, InstalledHeadsetSDKInfos, InstalledControllerSDKInfos };
			VRTK_SDKInfo[] array2 = new VRTK_SDKInfo[4] { systemSDKInfo, boundariesSDKInfo, headsetSDKInfo, controllerSDKInfo };
			for (int i = 0; i < array.Length; i++)
			{
				ReadOnlyCollection<VRTK_SDKInfo> readOnlyCollection = array[i];
				VRTK_SDKInfo vRTK_SDKInfo = array2[i];
				Type baseType = vRTK_SDKInfo.type.BaseType;
				if (baseType == null)
				{
					continue;
				}
				string arg = baseType.Name.Remove(0, typeof(SDK_Base).Name.Length);
				if (!readOnlyCollection.Contains(vRTK_SDKInfo))
				{
					list.Add(string.Format("The vendor SDK '{0}' is not installed.", vRTK_SDKInfo.description.prettyName));
				}
				else if (vRTK_SDKInfo.type == typeof(SDK_FallbackSystem))
				{
					if (vRTK_SDKInfo.originalTypeNameWhenFallbackIsUsed != null)
					{
						list.Add(string.Format("The SDK '{0}' doesn't exist anymore. The {1} fallback SDK will be used instead.", vRTK_SDKInfo.originalTypeNameWhenFallbackIsUsed, arg));
					}
					else
					{
						list.Add("A fallback SDK is used. Make sure to set a real SDK.");
					}
				}
			}
			return list.Distinct().ToArray();
		}

		protected virtual void Awake()
		{
			CreateInstance();
			SetupHeadset();
			SetupControllers();
			GetBoundariesSDK().InitBoundaries();
			base.gameObject.AddComponent<VRTK_InstanceMethods>();
		}

		private static void PopulateAvailableScriptingDefineSymbolPredicateInfos()
		{
			List<ScriptingDefineSymbolPredicateInfo> list = new List<ScriptingDefineSymbolPredicateInfo>();
			Type[] types = typeof(VRTK_SDKManager).Assembly.GetTypes();
			foreach (Type type in types)
			{
				for (int j = 0; j < type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Length; j++)
				{
					_003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey0 _003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey = new _003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey0();
					_003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey.methodInfo = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)[j];
					SDK_ScriptingDefineSymbolPredicateAttribute[] array = (SDK_ScriptingDefineSymbolPredicateAttribute[])_003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey.methodInfo.GetCustomAttributes(typeof(SDK_ScriptingDefineSymbolPredicateAttribute), false);
					if (array.Length != 0)
					{
						if (_003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey.methodInfo.ReturnType != typeof(bool) || _003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey.methodInfo.GetParameters().Length != 0)
						{
							throw new InvalidOperationException(string.Format("The method '{0}' on '{1}' has '{2}' specified but its signature is wrong. The method must take no arguments and return bool.", _003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey.methodInfo.Name, type, typeof(SDK_ScriptingDefineSymbolPredicateAttribute)));
						}
						list.AddRange(array.Select(_003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Ec__AnonStorey._003C_003Em__0));
					}
				}
			}
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Em__0;
			}
			list.Sort(_003C_003Ef__am_0024cache0);
			AvailableScriptingDefineSymbolPredicateInfos = list.AsReadOnly();
		}

		private static void PopulateAvailableAndInstalledSDKInfos()
		{
			ReadOnlyCollection<ScriptingDefineSymbolPredicateInfo> availableScriptingDefineSymbolPredicateInfos = AvailableScriptingDefineSymbolPredicateInfos;
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003CPopulateAvailableAndInstalledSDKInfos_003Em__1;
			}
			IEnumerable<ScriptingDefineSymbolPredicateInfo> source = availableScriptingDefineSymbolPredicateInfos.Where(_003C_003Ef__am_0024cache1);
			if (_003C_003Ef__am_0024cache2 == null)
			{
				_003C_003Ef__am_0024cache2 = _003CPopulateAvailableAndInstalledSDKInfos_003Em__2;
			}
			List<string> symbolsOfInstalledSDKs = source.Select(_003C_003Ef__am_0024cache2).ToList();
			List<VRTK_SDKInfo> list = new List<VRTK_SDKInfo>();
			List<VRTK_SDKInfo> list2 = new List<VRTK_SDKInfo>();
			List<VRTK_SDKInfo> list3 = new List<VRTK_SDKInfo>();
			List<VRTK_SDKInfo> list4 = new List<VRTK_SDKInfo>();
			List<VRTK_SDKInfo> list5 = new List<VRTK_SDKInfo>();
			List<VRTK_SDKInfo> list6 = new List<VRTK_SDKInfo>();
			List<VRTK_SDKInfo> list7 = new List<VRTK_SDKInfo>();
			List<VRTK_SDKInfo> list8 = new List<VRTK_SDKInfo>();
			PopulateAvailableAndInstalledSDKInfos<SDK_BaseSystem, SDK_FallbackSystem>(list, list5, symbolsOfInstalledSDKs);
			PopulateAvailableAndInstalledSDKInfos<SDK_BaseBoundaries, SDK_FallbackBoundaries>(list2, list6, symbolsOfInstalledSDKs);
			PopulateAvailableAndInstalledSDKInfos<SDK_BaseHeadset, SDK_FallbackHeadset>(list3, list7, symbolsOfInstalledSDKs);
			PopulateAvailableAndInstalledSDKInfos<SDK_BaseController, SDK_FallbackController>(list4, list8, symbolsOfInstalledSDKs);
			AvailableSystemSDKInfos = list.AsReadOnly();
			AvailableBoundariesSDKInfos = list2.AsReadOnly();
			AvailableHeadsetSDKInfos = list3.AsReadOnly();
			AvailableControllerSDKInfos = list4.AsReadOnly();
			InstalledSystemSDKInfos = list5.AsReadOnly();
			InstalledBoundariesSDKInfos = list6.AsReadOnly();
			InstalledHeadsetSDKInfos = list7.AsReadOnly();
			InstalledControllerSDKInfos = list8.AsReadOnly();
		}

		private static void PopulateAvailableAndInstalledSDKInfos<BaseType, FallbackType>(List<VRTK_SDKInfo> availableSDKInfos, List<VRTK_SDKInfo> installedSDKInfos, ICollection<string> symbolsOfInstalledSDKs) where BaseType : SDK_Base where FallbackType : BaseType
		{
			_003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey1<BaseType, FallbackType> _003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey = new _003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey1<BaseType, FallbackType>();
			_003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey.symbolsOfInstalledSDKs = symbolsOfInstalledSDKs;
			_003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey.baseType = typeof(BaseType);
			_003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey.fallbackType = SDKFallbackTypesByBaseType[_003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey.baseType];
			availableSDKInfos.Add(VRTK_SDKInfo.Create<BaseType, FallbackType, FallbackType>());
			availableSDKInfos.AddRange(_003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey.baseType.Assembly.GetExportedTypes().Where(_003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey._003C_003Em__0).Select(VRTK_SDKInfo.Create<BaseType, FallbackType>));
			availableSDKInfos.Sort(_003CPopulateAvailableAndInstalledSDKInfos_00602_003Em__3<BaseType, FallbackType>);
			installedSDKInfos.AddRange(availableSDKInfos.Where(_003CPopulateAvailableAndInstalledSDKInfos_003Ec__AnonStorey._003C_003Em__1));
		}

		private void SetupHeadset()
		{
			if (!actualHeadset.GetComponent<VRTK_TrackedHeadset>())
			{
				actualHeadset.AddComponent<VRTK_TrackedHeadset>();
			}
		}

		private void SetupControllers()
		{
			if ((bool)actualLeftController && !actualLeftController.GetComponent<VRTK_TrackedController>())
			{
				actualLeftController.AddComponent<VRTK_TrackedController>();
			}
			if ((bool)actualRightController && !actualRightController.GetComponent<VRTK_TrackedController>())
			{
				actualRightController.AddComponent<VRTK_TrackedController>();
			}
			if ((bool)scriptAliasLeftController && !scriptAliasLeftController.GetComponent<VRTK_ControllerTracker>())
			{
				scriptAliasLeftController.AddComponent<VRTK_ControllerTracker>();
			}
			if ((bool)scriptAliasRightController && !scriptAliasRightController.GetComponent<VRTK_ControllerTracker>())
			{
				scriptAliasRightController.AddComponent<VRTK_ControllerTracker>();
			}
		}

		private void CreateInstance()
		{
			if (_instance == null)
			{
				_instance = this;
				VRTK_SDK_Bridge.InvalidateCaches();
				string text = string.Join("\n- ", GetSimplifiedSDKErrorDescriptions());
				if (!string.IsNullOrEmpty(text))
				{
					text = "- " + text;
					VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_MANAGER_ERRORS, text));
				}
				if (persistOnLoad && !VRTK_SharedMethods.IsEditTime())
				{
					UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				}
			}
			else if (_instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private static void HandleSDKGetter<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
		{
			if (!VRTK_SharedMethods.IsEditTime())
			{
				string sDKErrorDescription = GetSDKErrorDescription<BaseType>(prettyName, info, installedInfos);
				if (!string.IsNullOrEmpty(sDKErrorDescription))
				{
					VRTK_Logger.Error(sDKErrorDescription);
				}
			}
		}

		private static string GetSDKErrorDescription<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
		{
			Type type = info.type;
			Type typeFromHandle = typeof(BaseType);
			Type type2 = SDKFallbackTypesByBaseType[typeFromHandle];
			if (type == type2)
			{
				return string.Format("The fallback {0} SDK is being used because there is no other {0} SDK set in the SDK Manager.", prettyName);
			}
			if (!typeFromHandle.IsAssignableFrom(type) || type2.IsAssignableFrom(type))
			{
				string text = string.Format("The fallback {0} SDK is being used despite being set to '{1}'.", prettyName, type.Name);
				if (installedInfos.Select(_003CGetSDKErrorDescription_00601_003Em__4<BaseType>).Contains(type))
				{
					return text + " Its needed scripting define symbols are not added. You can click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and choose to automatically let the manager handle the scripting define symbols.";
				}
				return text + " The needed vendor SDK isn't installed.";
			}
			return null;
		}

		[CompilerGenerated]
		private static int _003CPopulateAvailableScriptingDefineSymbolPredicateInfos_003Em__0(ScriptingDefineSymbolPredicateInfo x, ScriptingDefineSymbolPredicateInfo y)
		{
			return string.Compare(x.attribute.symbol, y.attribute.symbol, StringComparison.Ordinal);
		}

		[CompilerGenerated]
		private static bool _003CPopulateAvailableAndInstalledSDKInfos_003Em__1(ScriptingDefineSymbolPredicateInfo predicateInfo)
		{
			return (bool)predicateInfo.methodInfo.Invoke(null, null);
		}

		[CompilerGenerated]
		private static string _003CPopulateAvailableAndInstalledSDKInfos_003Em__2(ScriptingDefineSymbolPredicateInfo predicateInfo)
		{
			return predicateInfo.attribute.symbol;
		}

		[CompilerGenerated]
		private static int _003CPopulateAvailableAndInstalledSDKInfos_00602_003Em__3<BaseType, FallbackType>(VRTK_SDKInfo x, VRTK_SDKInfo y) where BaseType : SDK_Base where FallbackType : BaseType
		{
			return (!(x.description.prettyName == "Fallback")) ? string.Compare(x.description.prettyName, y.description.prettyName, StringComparison.Ordinal) : (-1);
		}

		[CompilerGenerated]
		private static Type _003CGetSDKErrorDescription_00601_003Em__4<BaseType>(VRTK_SDKInfo installedInfo) where BaseType : SDK_Base
		{
			return installedInfo.type;
		}
	}
}
