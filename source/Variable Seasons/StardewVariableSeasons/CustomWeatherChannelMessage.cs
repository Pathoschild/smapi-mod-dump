/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;

namespace StardewVariableSeasons
{
    public static class CustomWeatherChannelMessage
    {
        private static string GetWeatherMessage()
        {
            var message = ModEntry.ChangeDate switch
            {
                22 or 23 =>
                    $"And we're looking at an abnormally short {Game1.currentSeason} this year, make sure you remember to harvest early!",
                24 or 25 => $"It looks like {Game1.currentSeason} will be a bit shorter than average this year.",
                26 or 27 or 0 => $"The {Game1.currentSeason} is going to be about an average length this year.",
                1 or 2 or 3 => $"We're expecting a slightly longer than average {Game1.currentSeason} this year.",
                4 or 5 or 6 =>
                    $"This {Game1.currentSeason} is looking to run abnormally long, according to our projections.",
                _ => "You... played with the dev panel didn't you? Well, be careful out there, time traveller!"
            };

            return message;
        }

        public static void Postfix(ref string __result)
        {
            if (Game1.dayOfMonth > 15 && Game1.dayOfMonth < 23)
                __result += $" {GetWeatherMessage()}";
        }
    }
}