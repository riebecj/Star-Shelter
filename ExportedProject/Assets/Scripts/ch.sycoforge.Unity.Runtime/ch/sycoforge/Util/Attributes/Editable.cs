using System;
using System.Collections;

namespace ch.sycoforge.Util.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class Editable : Attribute
	{
		public enum EditType
		{
			STANDARD = 0,
			READONLY = 1,
			HIDDEN = 2,
			RESOURCE = 3
		}

		public enum EditFormat
		{
			Normal = 0,
			Percent = 1,
			Rounded = 2
		}

		public bool HasRange = false;

		private EditType editType = EditType.STANDARD;

		private string name = null;

		private string classPath = string.Empty;

		private string aliasPath = string.Empty;

		private string alias = string.Empty;

		private IEnumerable multiData = null;

		private float min;

		private float max;

		private float minX = 0f;

		private float maxX = 1f;

		private string group = string.Empty;

		private Type type;

		private bool normalized;

		public string ClassPath
		{
			get
			{
				return classPath;
			}
			set
			{
				classPath = value;
			}
		}

		public string AliasPath
		{
			get
			{
				return aliasPath;
			}
			set
			{
				aliasPath = value;
			}
		}

		public bool IsResource
		{
			get
			{
				return editType == EditType.RESOURCE;
			}
		}

		public string ReferenceName { get; set; }

		public string Tooltip { get; set; }

		public bool Validatable { get; set; }

		public bool DynamicColletion { get; set; }

		public EditFormat Format { get; set; }

		public object Tag { get; set; }

		public bool Dynamic
		{
			get
			{
				return normalized;
			}
			set
			{
				normalized = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return editType == EditType.READONLY;
			}
		}

		public bool HasSpecialFormat
		{
			get
			{
				return Format != EditFormat.Normal;
			}
		}

		public float Min
		{
			get
			{
				return min;
			}
			set
			{
				min = value;
				HasRange = true;
			}
		}

		public float Max
		{
			get
			{
				return max;
			}
			set
			{
				max = value;
				HasRange = true;
			}
		}

		public float MinX
		{
			get
			{
				return minX;
			}
			set
			{
				minX = value;
			}
		}

		public float MaxX
		{
			get
			{
				return maxX;
			}
			set
			{
				maxX = value;
			}
		}

		public string Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public string Alias
		{
			get
			{
				return alias;
			}
			set
			{
				alias = value;
			}
		}

		public EditType Type
		{
			get
			{
				return editType;
			}
		}

		public Type TargetType
		{
			get
			{
				return type;
			}
		}

		public string VisibiltyDependsOn { get; set; }

		public bool VisibiltyDepended
		{
			get
			{
				return !string.IsNullOrEmpty(VisibiltyDependsOn);
			}
		}

		public bool HasAlias
		{
			get
			{
				return alias != null && alias != string.Empty;
			}
		}

		public int Indent { get; set; }

		public Editable()
		{
		}

		public Editable(EditType editType)
		{
			this.editType = editType;
		}

		public Editable(EditType editType, Type type)
		{
			this.editType = editType;
			this.type = type;
		}

		public object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName).GetValue(src, null);
		}
	}
}
