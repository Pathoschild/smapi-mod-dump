/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace xBRZ;

sealed class Options {
	internal bool Preview { get; init; } = false;
	internal List<string> Paths { get; init; } = new();
}
