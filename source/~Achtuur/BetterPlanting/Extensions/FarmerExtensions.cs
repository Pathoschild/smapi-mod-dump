/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using StardewValley;

namespace BetterPlanting.Extensions;

internal static class FarmerExtensions
{
    public static bool IsHoldingCategory(this Farmer farmer, int category)
    {
        return farmer.CurrentItem is not null && farmer.CurrentItem.Category == category;
    }
}
