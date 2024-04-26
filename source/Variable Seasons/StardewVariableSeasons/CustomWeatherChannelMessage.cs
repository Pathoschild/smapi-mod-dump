/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using StardewValley;

namespace StardewVariableSeasons
{
    public static class CustomWeatherChannelMessage
    {
        private static string GetWeatherMessage()
        {
            var changeDate = ModEntry.ChangeDate;

            var message = changeDate switch
            {
                23 or 24 =>
                    $"And we're looking at an abnormally short {Game1.currentSeason} this year, make sure you remember to harvest early!",
                25 or 26 => $"It looks like {Game1.currentSeason} will be a bit shorter than average this year.",
                27 or 28 or 1 => $"The {Game1.currentSeason} is going to be about an average length this year.",
                2 or 3 or 4 => $"We're expecting a slightly longer than average {Game1.currentSeason} this year.",
                5 or 6 or 7 =>
                    $"This {Game1.currentSeason} is looking to run abnormally long, according to our projections.",
                _ => ""
            };

            return message;
        }
        
        public static void Postfix(ref string __result)
        {
            if (Game1.dayOfMonth > 15 && Game1.dayOfMonth < 21)
                __result += $" {GetWeatherMessage()}";
        }
    }
}