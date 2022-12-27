using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	public sealed class InfoBoxAttribute : Attribute
	{
		public string Message { get; private set; }

		public InfoMessageType InfoMessageType { get; private set; }

		public string VisibleIf { get; private set; }

		public InfoBoxAttribute(string message, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIfMemberName = null)
		{
			Message = message;
			InfoMessageType = infoMessageType;
			VisibleIf = visibleIfMemberName;
		}

		public InfoBoxAttribute(string message, string visibleIfMemberName)
		{
			Message = message;
			InfoMessageType = InfoMessageType.Info;
			VisibleIf = visibleIfMemberName;
		}
	}
}
