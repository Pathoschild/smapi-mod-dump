/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace GrowableGiantCrops.Framework;

/// <summary>
/// The API for this mod.
/// </summary>
public sealed class Api : IGrowableGiantCropsAPI
{
    public bool IsShovel(Tool tool) => tool is ShovelTool;
}
