using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Sirenix.Utilities
{
	[AttributeUsage(AttributeTargets.Class)]
	public class GlobalConfigAttribute : Attribute
	{
		private string assetPath;

		public string FullPath
		{
			get
			{
				return Application.dataPath + "/" + AssetPath;
			}
		}

		public string AssetPath
		{
			get
			{
				return assetPath.Trim().TrimEnd('/', '\\').TrimStart('/', '\\')
					.Replace('\\', '/') + "/";
			}
		}

		public string ResourcesPath
		{
			get
			{
				string text = "";
				if (IsInResourcesFolder)
				{
					Stack<string> stack = new Stack<string>();
					DirectoryInfo directoryInfo = new DirectoryInfo(FullPath);
					while (!directoryInfo.Name.Equals("resources", StringComparison.OrdinalIgnoreCase))
					{
						stack.Push(directoryInfo.Name);
						directoryInfo = directoryInfo.Parent;
					}
					while (stack.Any())
					{
						text = text + stack.Pop() + "/";
					}
				}
				return text;
			}
		}

		public bool UseAsset { get; set; }

		public bool IsInResourcesFolder
		{
			get
			{
				return AssetPath.Contains("/resources/", StringComparison.OrdinalIgnoreCase);
			}
		}

		public GlobalConfigAttribute()
			: this("Assets/Resources/Global Settings")
		{
		}

		public GlobalConfigAttribute(string assetPath)
		{
			this.assetPath = assetPath;
			UseAsset = true;
		}
	}
}
