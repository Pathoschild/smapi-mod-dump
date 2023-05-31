/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

/*
This module provides utilities for manipulating MiniScript file paths
(which always use a '/' as the path separator, even on Windows).
*/
namespace Farmtronics.Utils {
	public static class PathUtils {
		/// <summary>
		/// Get just the last part of the given path (i.e. the file name).
		/// </summary>
		public static string GetFileName(string path) {
			int pos = path.LastIndexOf("/");
			if (pos < 0) return path;
			return path.Substring(pos+1);
		}
	
		/// <summary>
		/// Get everything *except* the file name in the given path.
		/// </summary>
		public static string StripFileName(string path) {
			int pos = path.LastIndexOf("/");
			if (pos < 0) return path;
			return path.Substring(0, pos);		
		}
	
		/// <summary>
		/// Combine a base path and a partial path.
		/// </summary>
		public static string PathCombine(string basePath, string partialPath) {
			if (!basePath.EndsWith("/")) basePath += "/";
			return basePath + partialPath;
		}


	}
}