/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;

namespace ItemResearchSpawner.Models.Enums
{
    public enum ItemQuality
    {
        Normal = StardewValley.Object.lowQuality,
        Silver = StardewValley.Object.medQuality,
        Gold = StardewValley.Object.highQuality,
        Iridium = StardewValley.Object.bestQuality
    }

    internal static class ItemQualityExtensions
    {
        public static ItemQuality GetNext(this ItemQuality current)
        {
            return current switch
            {
                ItemQuality.Normal => ItemQuality.Silver,
                ItemQuality.Silver => ItemQuality.Gold,
                ItemQuality.Gold => ItemQuality.Iridium,
                ItemQuality.Iridium => ItemQuality.Normal,
                _ => throw new NotSupportedException($"Unknown item quality: '{current}'")
            };
        }
        
        public static ItemQuality GetPrevious(this ItemQuality current)
        {
            return current switch
            {
                ItemQuality.Normal => ItemQuality.Iridium,
                ItemQuality.Silver => ItemQuality.Normal,
                ItemQuality.Gold => ItemQuality.Silver,
                ItemQuality.Iridium => ItemQuality.Gold,
                _ => throw new NotSupportedException($"Unknown item quality: '{current}'")
            };
        }
    }
}