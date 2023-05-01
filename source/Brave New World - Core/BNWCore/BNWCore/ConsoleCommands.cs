/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace BNWCore
{
    public class ConsoleCommands
    {
        public static void SendEmails(string command, string[] args)
        {
            Game1.player.mailbox.Add("earth_farming_blessing");
            Game1.player.mailbox.Add("nature_foraging_blessing");
            Game1.player.mailbox.Add("water_fishing_blessing");
            Game1.player.mailbox.Add("fire_mining_blessing");
            Game1.player.mailbox.Add("wind_combat_blessing");
        }
    }
}
