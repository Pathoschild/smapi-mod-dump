/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterRarecrows.Patches
{
    /// <summary>Contains patches for patching game code in the StardewValley.Farm class.</summary>
    internal class FarmPatch
    {
        /// <summary>The code that get's ran before the Farm.addCrows game method gets ran</summary>
        /// <param name="__instance">The instance of the farm, used for checking which rarecrows have been placed</param>
        /// <returns>If there are enough rarecrows placed, return false. (Game method doesn't get ran) If there aren't enough rarecrows placed, return true (Game method gets ran)</returns>
        internal static bool AddCrowsPrefix(ref Farm __instance)
        {
            if (ModEntry.PreviousDate != Game1.dayOfMonth)
            {
                ModEntry.PreviousDate = Game1.dayOfMonth;
                ModEntry.CurrentRarecrows = new List<int>();
            }

            // Check how many rarecrows have been placed
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in __instance.objects.Pairs)
            {
                if (pair.Value.bigCraftable && pair.Value.Name.Contains("Rarecrow"))
                {
                    if (!ModEntry.CurrentRarecrows.Contains(pair.Value.ParentSheetIndex))
                    {
                        ModEntry.CurrentRarecrows.Add(pair.Value.ParentSheetIndex);
                    }
                }
            }

            if (ModEntry.CurrentRarecrows.Count() >= ModEntry.Config.NumberOfRequiredRareCrows)
            {
                ModEntry.ModMonitor.Log($"All {ModEntry.CurrentRarecrows.Count()} out of {ModEntry.Config.NumberOfRequiredRareCrows} rarecrows found on the farm", LogLevel.Trace);
                return false;
            }
            else
            {
                ModEntry.ModMonitor.Log($"Only {ModEntry.CurrentRarecrows.Count()} out of {ModEntry.Config.NumberOfRequiredRareCrows} rarecrows found on the farm", LogLevel.Trace);

                if (ModEntry.Config.EnableProgressiveMode)
                {
                    ModEntry.ModMonitor.Log($"Progressive mod enabled", LogLevel.Trace);

                    // Calculate the a random chance to determine if the crows should be able to spawn
                    int chanceUpperBound = Math.Min(100, ModEntry.Config.ProgressivePercentPerRarecrow * ModEntry.CurrentRarecrows.Count());
                    int chance = Math.Max(0, chanceUpperBound);

                    double randomChance = Game1.random.NextDouble();

                    if (chance / 100d < randomChance)
                    {
                        return false;
                    }
                }
                else
                {
                    ModEntry.ModMonitor.Log($"Progressive mod disabled", LogLevel.Trace);
                }

                return true;
            }
        }
    }
}
