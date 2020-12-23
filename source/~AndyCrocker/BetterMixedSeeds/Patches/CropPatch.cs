/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BetterMixedSeeds.Patches
{
    /// <summary>Contains patches for patching game code in the StardewValley.Crop class.</summary>
    internal class CropPatch
    {
        /// <summary>Change the condition for the seed index from 473 to 1. (this was preventing Green Beans from being planted as it would decrement the number)</summary>
        internal static IEnumerable<CodeInstruction> ConstructorTranspile(IEnumerable<CodeInstruction> instructions)
        {
            bool changed = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!changed && instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == 473)
                {
                    // Changes the condition of the if to check for id of 1 instead of 473 (an Id that will never be returned by the method patch)
                    changed = true;
                    instruction.operand = 1;
                }

                yield return instruction;
            }
        }

        /// <summary>This is code that will replace some game code, this is ran whenever the player is about to place some mixed seeds. Used for calculating the result from the seed list.</summary>
        /// <param name="season">The current game season.</param>
        /// <param name="__result">The seed id that will be planted from the mixed seeds.</param>
        /// <returns>If no seeds are available, return true (This means the actual game code will be ran). If seeds are available, return false (This means the actual game code doesn't get ran)</returns>
        internal static bool RandomCropPrefix(string season, ref int __result)
        {
            List<int> possibleSeeds = new List<int>();

            if (Game1.currentLocation.IsGreenhouse)
            {
                possibleSeeds = ModEntry.Seeds
                    .Select(seed => seed.Id)
                    .ToList();
            }
            else
            {
                possibleSeeds = ModEntry.Seeds
                    .Where(seed => seed.Seasons.Contains(season))
                    .Select(seed => seed.Id)
                    .ToList();
            }

            if (possibleSeeds.Any())
            {
                __result = possibleSeeds[new Random().Next(possibleSeeds.Count())];
                var test2 = __result;
                var test = ModEntry.Seeds.Where(seed => seed.Id == test2);
            }
            else
            {
                ModEntry.ModMonitor.Log("No possible seeds in seed list", LogLevel.Error);
                return true;
            }

            return false;
        }
    }
}
