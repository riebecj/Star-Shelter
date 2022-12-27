using System;
using System.IO;

namespace Sirenix.Utilities
{
	public static class DirectoryInfoExtensions
	{
		public static bool HasSubDirectory(this DirectoryInfo parentDir, DirectoryInfo subDir)
		{
			string text = parentDir.FullName.TrimEnd('\\', '/');
			while (subDir != null)
			{
				if (subDir.FullName.TrimEnd('\\', '/') == text)
				{
					return true;
				}
				subDir = subDir.Parent;
			}
			return false;
		}

		public static DirectoryInfo FindParentDirectoryWithName(this DirectoryInfo dir, string folderName)
		{
			if (dir.Parent == null)
			{
				return null;
			}
			if (string.Equals(dir.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
			{
				return dir;
			}
			return dir.Parent.FindParentDirectoryWithName(folderName);
		}
	}
}
