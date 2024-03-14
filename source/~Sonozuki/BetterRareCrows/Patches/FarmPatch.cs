/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace BetterRarecrows.Patches
{
    /// <summary>Contains code patches for the <see cref="Farm"/> class.</summary>
    internal class FarmPatch
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>The prefix for the <see cref="Farm.addCrows"/> method.</summary>
        /// <param name="__instance">The current <see cref="Farm"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This is used to prevent crows spawning when the necessary conditions are met.</remarks>
        public static bool AddCrowsPrefix(Farm __instance)
        {
            // check how many distinct rarecrows have been placed
            var currentRarecrowIds = new List<int>();
            foreach (var pair in __instance.objects.Pairs)
                if (pair.Value.bigCraftable && pair.Value.Name.Contains("Rarecrow"))
                    if (!currentRarecrowIds.Contains(pair.Value.ParentSheetIndex))
                        currentRarecrowIds.Add(pair.Value.ParentSheetIndex);

            if (currentRarecrowIds.Count >= ModEntry.Instance.Config.NumberOfRequiredRareCrows)
            {
                ModEntry.Instance.Monitor.Log($"{currentRarecrowIds.Count} rarecrows out of {ModEntry.Instance.Config.NumberOfRequiredRareCrows} required rarecrows found on the farm, preventing crow spawn.", LogLevel.Trace);
                return false;
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"Only {currentRarecrowIds.Count} out of {ModEntry.Instance.Config.NumberOfRequiredRareCrows} required rarecrows found on the farm.", LogLevel.Trace);

                // check if the crow spawn should be prevented with progressive mode
                if (ModEntry.Instance.Config.EnableProgressiveMode)
                {
                    ModEntry.Instance.Monitor.Log("Progressive mod enabled.", LogLevel.Trace);

                    // calculate whether progressive mode should stop to crow from spawning
                    var chance = Math.Max(0, Math.Min(100, ModEntry.Instance.Config.ProgressivePercentPerRarecrow * currentRarecrowIds.Count));
                    if (chance / 100f >= Game1.random.NextDouble())
                    {
                        ModEntry.Instance.Monitor.Log("Progressive mode successful, preventing crow spawn.", LogLevel.Trace);
                        return false;
                    }
                }

                ModEntry.Instance.Monitor.Log("Crow spawning.", LogLevel.Trace);
                return true;
            }
        }
    }
}
