/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using BetterMixedSeeds.Models;
using Harmony;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BetterMixedSeeds.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Crop"/> class.</summary>
    internal class CropPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The transpiler for the <see cref="Crop(int, int, int)"/> constructor.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>Used for removing the id shifting if green beans were picked (which resulted in never finding green beans).</remarks>
        internal static IEnumerable<CodeInstruction> ConstructorTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var greenBeanPatchApplied = false;
            var gingerIslandPatchApplied = false;

            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                // check if this instruction is the one responsible for the condition to id shift green beans
                if (!greenBeanPatchApplied && ModEntry.Instance.Config.EnableTrellisCrops && instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == 473) // ensure trillis crops are allowed (green beans is a trellis crop)
                {
                    greenBeanPatchApplied = true;

                    // change the id shift condition to 1 (which won't ever be true) so grean beans can be dropped
                    instruction.operand = 1;
                }

                // check if this instruction is the one responsible for checking if the curent location is ginger island
                if (!gingerIslandPatchApplied && instruction.opcode == OpCodes.Isinst && instruction.operand == typeof(IslandLocation))
                {
                    gingerIslandPatchApplied = true;

                    // this will change the code: Game1.currentLocation is IslandLocation
                    // to be                    : Game1.currentLocation is int
                    // this will always be false meaning the game won't change the id of seeds planted in ginger island
                    instruction.operand = typeof(int);
                }

                yield return instruction;
            }
        }

        /// <summary>The prefix for the <see cref="Crop.getRandomLowGradeCropForThisSeason(string)"/> method.</summary>
        /// <param name="season">The current game season.</param>
        /// <param name="__result">The return object from the original method (the seed id that will be planted from the mixed seeds).</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (this depends on if there were valid seeds to plant).</returns>
        /// <remarks>Used for calculating the result from the seed list.</remarks>
        internal static bool GetRandomLowGradeCropForThisSeasonPrefix(string season, ref int __result)
        {
            List<Seed> possibleSeeds;

            // get possible seeds, determined from season and whether they are in the greenhouse
            if (Game1.currentLocation.IsGreenhouse || Game1.currentLocation is IslandLocation)
                possibleSeeds = ModEntry.Instance.Seeds
                    .ToList();
            else
                possibleSeeds = ModEntry.Instance.Seeds
                    .Where(seed => seed.Season.ToLower() == season.ToLower())
                    .ToList();

            // ensure there are possible seeds, if not then let the original method run
            if (!possibleSeeds.Any())
                return true;

            // pick a random seed
            var totalDropChance = possibleSeeds.Select(seed => seed.DropChance).Sum();
            var randomChance = (float)(Game1.random.NextDouble() * totalDropChance);
            foreach (var possibleSeed in possibleSeeds)
            {
                randomChance -= possibleSeed.DropChance;
                if (randomChance <= 0)
                {
                    __result = possibleSeed.Id;
                    return false;
                }
            }

            // this shouldn't ever get ran, but if for whatever reason this it does, just return the first seed
            __result = possibleSeeds[0].Id;
            return false;
        }
    }
}
