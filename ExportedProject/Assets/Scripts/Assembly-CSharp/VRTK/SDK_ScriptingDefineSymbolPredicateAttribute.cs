using System;
using UnityEngine;

namespace VRTK
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class SDK_ScriptingDefineSymbolPredicateAttribute : Attribute, ISerializationCallbackReceiver
	{
		public const string RemovableSymbolPrefix = "VRTK_DEFINE_";

		public string symbol;

		[SerializeField]
		private string buildTargetGroupName;

		private SDK_ScriptingDefineSymbolPredicateAttribute()
		{
		}

		public SDK_ScriptingDefineSymbolPredicateAttribute(string symbol, string buildTargetGroupName)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException("symbol");
			}
			if (symbol == string.Empty)
			{
				throw new ArgumentOutOfRangeException("symbol", symbol, "An empty string isn't allowed.");
			}
			this.symbol = symbol;
			if (buildTargetGroupName == null)
			{
				throw new ArgumentNullException("buildTargetGroupName");
			}
			if (buildTargetGroupName == string.Empty)
			{
				throw new ArgumentOutOfRangeException("buildTargetGroupName", buildTargetGroupName, "An empty string isn't allowed.");
			}
			SetBuildTarget(buildTargetGroupName);
		}

		public SDK_ScriptingDefineSymbolPredicateAttribute(SDK_ScriptingDefineSymbolPredicateAttribute attributeToCopy)
		{
			symbol = attributeToCopy.symbol;
			SetBuildTarget(attributeToCopy.buildTargetGroupName);
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			SetBuildTarget(buildTargetGroupName);
		}

		private void SetBuildTarget(string groupName)
		{
			buildTargetGroupName = groupName;
		}
	}
}
