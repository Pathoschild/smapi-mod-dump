/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

namespace Farmtronics.M1.Filesystem {
	public class FileInfo {
		public string date;         // file timestamp, in SQL format
		public long size;           // size in bytes
		public bool isDirectory;    // true if it's a directory
		public string comment;      // file comment

		public override string ToString() {
			return $"FileInfo(date={date}, size={size}, isDirectory={isDirectory}, comment={comment})";
		}
	}
}