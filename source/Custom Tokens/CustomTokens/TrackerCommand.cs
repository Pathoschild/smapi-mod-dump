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
using System.Collections;
using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTokens
{
    public class TrackerCommand
    {

        internal void DisplayInfo(IMonitor monitor, PerScreen<PlayerData> data, PlayerDataToWrite datatowrite)
        {

            string Quests(ArrayList collection)
            {
                StringBuilder questsasstring = new StringBuilder("None");

                // Remove default string if array isn't empty
                if (collection.Count > 0)
                {
                    questsasstring.Remove(0, 4);
                }

                // Add each quest id to string
                foreach (var quest in collection)
                {
                    questsasstring.Append($", {quest}");

                    // Remove whitespace and comma if id is the first in the array
                    if (collection.IndexOf(quest) == 0)
                    {
                        questsasstring.Remove(0, 2);
                    }
                }

                return questsasstring.ToString();
            }

            try
            {
                // Display information in SMAPI console
                monitor.Log($"\n\nMineLevel: {data.Value.CurrentMineLevel}" +
                    $"\nVolcanoFloor: {data.Value.CurrentVolcanoFloor}" +
                    $"\nDeepestMineLevel: {data.Value.DeepestMineLevel}" +
                    $"\nYearsMarried: {data.Value.CurrentYearsMarried}" +
                    $"\nAnniversaryDay: {data.Value.AnniversaryDay}" +
                    $"\nAnniversarySeason: {data.Value.AnniversarySeason}" +
                    $"\nQuestIDsCompleted: {Quests(data.Value.QuestsCompleted)}" +
                    $"\nSOIDsCompleted: {Quests(data.Value.SpecialOrdersCompleted)}" +
                    $"\nSOCompleted: {data.Value.SpecialOrdersCompleted.Count}" +
                    $"\nQuestsCompleted: {Game1.stats.questsCompleted}" +
                    $"\nDeathCount: {Game1.stats.timesUnconscious}" +
                    $"\nDeathCountMarried: {datatowrite.DeathCountMarried}" +
                    $"\nDeathCountPK: {(Game1.player.isMarried() ? Game1.stats.timesUnconscious + 1 : 0)}" +
                    $"\nDeathCountMarriedPK: {(Game1.player.isMarried() ? datatowrite.DeathCountMarried + 1 : 0)}" +
                    $"\nPassOutCount: {datatowrite.PassOutCount}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                // Throw an exception if command failed to execute
                throw new Exception("Command failed somehow", ex);
            }

        }
    }
}
