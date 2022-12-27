using System;

namespace Sirenix.OdinInspector
{
	public sealed class DictionaryDrawerSettings : Attribute
	{
		private string keyLabel = "Keys";

		private string valueLabel = "Values";

		public DictionaryDisplayOptions DisplayMode { get; set; }

		public bool IsReadOnly { get; set; }

		public string KeyLabel
		{
			get
			{
				return keyLabel;
			}
			set
			{
				keyLabel = value;
			}
		}

		public string ValueLabel
		{
			get
			{
				return valueLabel;
			}
			set
			{
				valueLabel = value;
			}
		}
	}
}
