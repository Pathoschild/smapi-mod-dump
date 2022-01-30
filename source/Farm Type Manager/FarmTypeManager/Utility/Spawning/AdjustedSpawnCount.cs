/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Calculates the final number of objects to spawn today in the current spawning process, based on config settings and player levels in a relevant skill.</summary>
            /// <param name="min">Minimum number of objects to spawn today (before skill multiplier).</param>
            /// <param name="max">Maximum number of objects to spawn today (before skill multiplier).</param>
            /// <param name="percent">Additive multiplier for each of the player's levels in the relevant skill (e.g. 10 would represent +10% objects per level).</param>
            /// <param name="skill">Enumerator for the skill on which the "percent" additive multiplier is based.</param>
            /// <returns>The final number of objects to spawn today in the current spawning process.</returns>
            public static int AdjustedSpawnCount(int min, int max, int percent, Utility.Skills skill)
            {
                int spawnCount = RNG.Next(min, max + 1); //random number from min to max (higher number is exclusive, so +1 to adjust for it)

                //calculate skill multiplier bonus
                double skillMultiplier = percent;
                skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. default config is "10" (10% per level) so it converts to "0.1"
                int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                }
                skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. with default config: "1.0" at level 0, "1.7" at level 7, etc

                //calculate final forage amount
                skillMultiplier *= spawnCount; //multiply the initial random spawn count by the skill multiplier
                spawnCount = (int)skillMultiplier; //store the integer portion of the current multiplied value (e.g. this is "1" if the multiplier is "1.7")
                double remainder = skillMultiplier - (int)skillMultiplier; //store the decimal portion of the multiplied value (e.g. this is "0.7" if the multiplier is "1.7")

                if (RNG.NextDouble() < remainder) //use remainder as a % chance to spawn one extra object (e.g. if the final count would be "1.7", there's a 70% chance of spawning 2 objects)
                {
                    spawnCount++;
                }

                return spawnCount;
            }
        }
    }
}