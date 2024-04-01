/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTokens
{
    public class DeathAndExhaustionTokens
    {
        
        internal void UpdateDeathAndExhaustionTokens(IMonitor monitor, PerScreen<PlayerData> data, ModConfig config, Update update)
        {
            if (Game1.killScreen == true && update.updatedeath == true)
            {
                if (Game1.player.isMarriedOrRoommates() == true)
                {
                    data.Value.DeathCountMarried++;
                    // Display trace information in SMAPI log
                    if (config.ResetDeathCountMarriedWhenDivorced == true)
                    {
                        monitor.Log($"{Game1.player.Name} has died {data.Value.DeathCountMarried} time(s) since last marriage.");
                    }
                    else
                    {
                        monitor.Log($"{Game1.player.Name} has died {data.Value.DeathCountMarried} time(s) whilst married.");
                    }
                }

                update.updatedeath = false;
            }

            else if(Game1.killScreen == false && update.updatedeath == false)
            {
                update.updatedeath = true;
            }

            // Has player passed out?
            else if (update.updatepassout == true && (Game1.timeOfDay == 2600 || Game1.player.stamina <= -15))
            {
                // Yes, update tracker

                // Increment tracker
                data.Value.PassOutCount++;
                // Already updated, ensures tracker won't repeatedly increment
                update.updatepassout = false;

                // Display trace information in SMAPI log
                if (data.Value.PassOutCount > 20)
                {
                    monitor.Log($"{Game1.player.Name} has passed out {data.Value.PassOutCount} time(s). Maybe you should go to bed earlier.");
                }
                else
                {
                    monitor.Log($"{Game1.player.Name} has passed out {data.Value.PassOutCount} time(s).");
                }

            }

            else if (Game1.timeOfDay == 2610 && update.updatepassout == false)
            {
                // Decrement tracker, player can stay up later
                data.Value.PassOutCount--;
                // Already updated, ensures tracker won't repeatedly decrement
                update.updatepassout = true;
                // Display trace information in SMAPI log
                monitor.Log($"Nevermind, {Game1.player.Name} has actually passed out {data.Value.PassOutCount} time(s). Aren't you getting tired?");
            }
        }
    }
}
