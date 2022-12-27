using System;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public sealed class ValidateInputAttribute : Attribute
	{
		public string DefaultMessage { get; private set; }

		public string MemberName { get; private set; }

		public InfoMessageType MessageType { get; private set; }

		public bool IncludeChildren { get; set; }

		public ValidateInputAttribute(string memberName, string defaultMessage = null, InfoMessageType messageType = InfoMessageType.Error)
		{
			MemberName = memberName;
			DefaultMessage = defaultMessage;
			MessageType = messageType;
			IncludeChildren = true;
		}

		[Obsolete("Rejecting invalid input is no longer supported. Use the other constructor instead.", false)]
		public ValidateInputAttribute(string memberName, string message, InfoMessageType messageType, bool rejectedInvalidInput)
		{
			MemberName = memberName;
			DefaultMessage = message;
			MessageType = messageType;
			IncludeChildren = true;
		}
	}
}
