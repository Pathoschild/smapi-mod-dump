/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using SObject = StardewValley.Object;

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal enum Quality
    {
        Any = -1,
        None = SObject.lowQuality,
        Silver = SObject.medQuality,
        Gold = SObject.highQuality,
        Iridium = SObject.bestQuality
    }

    internal enum TreasureAppearanceSettings
    {
        Vanilla,
        Never,
        Always
    }
}
