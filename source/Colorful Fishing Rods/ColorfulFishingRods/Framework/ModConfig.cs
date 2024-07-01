/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6480k/colorful-fishing-rods
**
*************************************************/

namespace ColorfulFishingRods.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// The config class for this mod.
/// </summary>
public sealed class ModConfig
{
    /// <summary>
    /// A map of colors and overrides.
    /// </summary>
    public Dictionary<string, Color> Map = new();
}
