/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace DynamicFestivalRewards;

internal class ModConfig
{
    public bool Randomize { get; set; } = true;
    public int MinValue { get; set; } = 200;
    public int MaxValue { get; set; } = 1000;
}