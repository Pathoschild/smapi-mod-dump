/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;

namespace AnotherHungerMod
{
    public static class Extensions
    {
        public static double GetFullness(this Farmer player)
        {
            if (player != Game1.player)
                return -1;
            return Mod.Data.Fullness;
        }

        public static void UseFullness(this Farmer player, double amt)
        {
            if (player != Game1.player)
                return;

            Mod.Data.Fullness = Math.Max(0, Math.Min(Mod.Data.Fullness - amt, player.GetMaxFullness()));
            Mod.Data.SyncToHost();
        }

        public static int GetMaxFullness(this Farmer player)
        {
            return Mod.Config.MaxFullness;
        }

        public static bool HasFedSpouse(this Farmer player)
        {
            return Mod.Data.FedSpouseMeal;
        }

        public static void SetFedSpouse(this Farmer player, bool fed)
        {
            Mod.Data.FedSpouseMeal = fed;
            Mod.Data.SyncToHost();
        }
    }
}
