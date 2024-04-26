/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using System;

namespace StardewVariableSeasons
{
    public static class CropDeathRandomizer
    {
        public static void Prefix(out bool __state)
        {
            var rnd = new Random();
            var rndNum = rnd.Next(100);
            
            var survivalPercentage = ModEntry.CropSurvivalCounter switch
            {
                0 => 1,
                1 => 25,
                2 => 33,
                3 => 50,
                4 => 75,
                _ => 100
            };

            __state = rndNum >= survivalPercentage;
        }

        public static void Postfix(bool __state, ref bool __result)
        {
            if (__state)
            {
                __result = true;
            }
        }
    }
}