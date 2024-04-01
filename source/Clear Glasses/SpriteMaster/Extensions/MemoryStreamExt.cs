/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.IO;

namespace SpriteMaster.Extensions;

internal static class MemoryStreamExt {
	internal static byte[] GetArray(this MemoryStream stream) {
		var buffer = stream.GetBuffer();
		return buffer.Length == stream.Length ? buffer : stream.ToArray();
	}
}
