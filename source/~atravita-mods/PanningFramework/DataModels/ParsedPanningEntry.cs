/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;

namespace PanningFramework.DataModels;

/// <summary>
/// A parsed panning entry.
/// </summary>
internal class ParsedPanningEntry
{
    internal ParsedPanningEntry(ItemTypeEnum itemType, int index, double chance)
    {
        this.itemType = itemType;
        this.index = index;
        this.chance = chance;
    }

    internal ItemTypeEnum itemType { get; set; }

    internal int index { get; set; } = -1;

    internal int minStack { get; set; } = 1;

    internal int maxStack { get; set; } = 5;

    internal double chance { get; set; } = 0.05;

    internal double dailyLuckAdjustment { get; set; } = 0;

    internal double luckStatAdjustment { get; set; } = 0;


}