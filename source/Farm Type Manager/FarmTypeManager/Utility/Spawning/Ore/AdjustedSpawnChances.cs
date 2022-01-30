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
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Produces a dictionary containing the final, adjusted spawn chance of each object in the provided dictionaries. (Part of the convoluted object spawning process for ore.)</summary>
            /// <param name="skill">The player skill that affects spawn chances (e.g. Mining for ore spawn chances).</param>
            /// <param name="levelRequired">A dictionary of object names and the skill level required to spawn them.</param>
            /// <param name="startChances">A dictionary of object names and their weighted chances to spawn at their lowest required skill level (e.g. chance of spawning stone if you're level 0).</param>
            /// <param name="maxChances">A dictionary of object names and their weighted chances to spawn at skill level 10.</param>
            /// <returns></returns>
            public static Dictionary<string, int> AdjustedSpawnChances(Utility.Skills skill, Dictionary<string, int> levelRequired, Dictionary<string, int> startChances, Dictionary<string, int> maxChances)
            {
                Dictionary<string, int> adjustedChances = new Dictionary<string, int>();

                int skillLevel = 0; //highest skill level among all existing farmers, not just the host
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    skillLevel = Math.Max(skillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                }

                foreach (KeyValuePair<string, int> objType in levelRequired)
                {
                    int chance = 0; //chance of spawning this object
                    if (objType.Value > skillLevel)
                    {
                        //skill is too low to spawn this object; leave it at 0%
                    }
                    else if (skillLevel >= 10)
                    {
                        chance = maxChances[objType.Key]; //level 10 skill; use the max level chance
                    }
                    else if (objType.Value == skillLevel)
                    {
                        chance = startChances[objType.Key]; //skill is the minimum required; use the starting chance
                    }
                    else //skill is somewhere in between "starting" and "level 10", so do math to set the chance somewhere in between them (i forgot the term for this kind of averaging, sry)
                    {
                        int count = 0;
                        long chanceMath = 0; //used in case the chances are very large numbers for some reason
                        for (int x = objType.Value; x < 10; x++) //loop from [minimum skill level for this object] to [max level - 1], for vague math reasons
                        {
                            if (skillLevel > x)
                            {
                                chanceMath += maxChances[objType.Key]; //add level 10 chance
                            }
                            else
                            {
                                chanceMath += startChances[objType.Key]; //add starting chance
                            }
                            count++;
                        }
                        chanceMath = (long)Math.Round((double)chanceMath / (double)count); //divide to get the average
                        chance = Convert.ToInt32(chanceMath); //convert back to a reasonable number range once the math is done
                    }

                    if (chance > 0) //don't bother adding any objects with 0% or negative spawn chance
                    {
                        adjustedChances.Add(objType.Key, chance); //add the object name & chance to the list of adjusted chances
                    }
                }

                return adjustedChances;
            }
        }
    }
}