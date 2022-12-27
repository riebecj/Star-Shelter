using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	public class DetailedInfoBoxAttribute : Attribute
	{
		public string Message { get; private set; }

		public string Details { get; private set; }

		public InfoMessageType InfoMessageType { get; private set; }

		public string VisibleIf { get; private set; }

		public DetailedInfoBoxAttribute(string message, string details, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIf = null)
		{
			Message = message;
			Details = details;
			InfoMessageType = infoMessageType;
			VisibleIf = visibleIf;
		}
	}
}
