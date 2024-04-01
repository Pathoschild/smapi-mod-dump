/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Extensions;

internal static class DirectoryExt {
	internal static bool CompressDirectory(string path, bool force = false) {
		if (Runtime.IsWindows) {
			return DirectoryExtWindows.CompressDirectory(path, force);
		}

		return false;
	}
}
