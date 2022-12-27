using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[DontApplyToListElements]
	public sealed class RequiredAttribute : Attribute
	{
		public string ErrorMessage { get; private set; }

		public InfoMessageType MessageType { get; private set; }

		public RequiredAttribute()
		{
			MessageType = InfoMessageType.Error;
		}

		public RequiredAttribute(string errorMessage, InfoMessageType messageType)
		{
			ErrorMessage = errorMessage;
			MessageType = messageType;
		}

		public RequiredAttribute(string errorMessage)
		{
			ErrorMessage = errorMessage;
			MessageType = InfoMessageType.Error;
		}

		public RequiredAttribute(InfoMessageType messageType)
		{
			MessageType = messageType;
		}
	}
}
