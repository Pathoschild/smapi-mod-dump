/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;

namespace MusicMaster.SMAPIConsole;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class StatsAttribute : Attribute {
	internal readonly string Name;

	internal StatsAttribute(string name) => Name = name;
}
