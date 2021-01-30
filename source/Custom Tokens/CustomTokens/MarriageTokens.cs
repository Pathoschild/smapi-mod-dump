/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTokens
{
    public class MarriageTokens
    {

        internal void UpdateMarriageTokens(IMonitor monitor, PerScreen<PlayerData> data, PlayerDataToWrite datatowrite, ModConfig config)
        {
            // Get days married
            int DaysMarried = Game1.player.GetDaysMarried();
            float Years = DaysMarried / 112;
            // Get years married
            double YearsMarried = Math.Floor(Years);
            // Get Anniversary date
            var anniversary = SDate.Now().AddDays(-(DaysMarried - 1));          

            // Set tokens for the start of the day
            data.Value.CurrentYearsMarried = Game1.player.isMarried() == true ? YearsMarried : 0;

            data.Value.AnniversarySeason = Game1.player.isMarried() == true ? anniversary.Season : "No season";

            data.Value.AnniversaryDay = Game1.player.isMarried() == true ? anniversary.Day : 0;

            // Test if player is married
            if (Game1.player.isMarried() is false)
            {
                // No, relevant trackers will use their default values

                monitor.Log($"{Game1.player.Name} is not married");

                if (config.ResetDeathCountMarriedWhenDivorced == true && datatowrite.DeathCountMarried != 0)
                {
                    // Reset tracker if player is no longer married
                    datatowrite.DeathCountMarried = 0;
                }
            }

            // Yes, tokens exist
            else
            {
                monitor.Log($"{Game1.player.Name} has been married for {YearsMarried} year(s)");

                monitor.Log($"Anniversary is the {anniversary.Day} of {anniversary.Season}");
            }

            // Fix death tracker
            if (datatowrite.DeathCountMarriedOld < datatowrite.DeathCountMarried)
            {
                monitor.Log("Fixing tracker to discard unsaved data");
                datatowrite.DeathCountMarried = datatowrite.DeathCountMarriedOld;
            }
        }
    }
}
