/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace MoreGrass.Config;

/// <summary>The content pack configuration.</summary>
public class ContentPackConfig
{
    /*********
    ** Properties
    *********/
    /// <summary>Whether default grass sprites should be drawn too.</summary>
    public bool EnableDefaultGrass { get; set; } = true;

    /// <summary>The locations that each specified grass is allowed to be in.</summary>
    public Dictionary<string, List<string>> WhiteListedGrass { get; set; } = [];

    /// <summary>The locations that each specified grass isn't allowed to be in.</summary>
    public Dictionary<string, List<string>> BlackListedGrass { get; set; } = [];

    /// <summary>The locations that this pack is allowed to retexture grass in.</summary>
    public List<string> WhiteListedLocations { get; set; } = [];

    /// <summary>The locations that this pack isn't allowed to retexture grass is.</summary>
    public List<string> BlackListedLocations { get; set; } = [];
}
