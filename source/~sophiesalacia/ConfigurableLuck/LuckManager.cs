/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using StardewValley;

namespace ConfigurableLuck;

internal class LuckManager
{
    internal static readonly double MIN_LUCK_VALUE = -1.0;
    internal static readonly double MAX_LUCK_VALUE = 0.74;

    internal static void SetLuck(Farmer player, double luckValue)
    {
        luckValue = Math.Clamp(luckValue, MIN_LUCK_VALUE, MAX_LUCK_VALUE);
        player.team.sharedDailyLuck.Value = luckValue;
    }
}
