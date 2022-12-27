using System;
using UnityEngine;

namespace VRTK
{
	[Serializable]
	public class VRTK_ControllerModelElementPaths
	{
		[Tooltip("The overall shape of the controller.")]
		public string bodyModelPath = string.Empty;

		[Tooltip("The model that represents the trigger button.")]
		public string triggerModelPath = string.Empty;

		[Tooltip("The model that represents the left grip button.")]
		public string leftGripModelPath = string.Empty;

		[Tooltip("The model that represents the right grip button.")]
		public string rightGripModelPath = string.Empty;

		[Tooltip("The model that represents the touchpad.")]
		public string touchpadModelPath = string.Empty;

		[Tooltip("The model that represents button one.")]
		public string buttonOneModelPath = string.Empty;

		[Tooltip("The model that represents button two.")]
		public string buttonTwoModelPath = string.Empty;

		[Tooltip("The model that represents the system menu button.")]
		public string systemMenuModelPath = string.Empty;

		[Tooltip("The model that represents the start menu button.")]
		public string startMenuModelPath = string.Empty;
	}
}
