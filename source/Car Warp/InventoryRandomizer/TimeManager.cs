/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace InventoryRandomizer;

internal class TimeManager
{
    private static int SecondsUntilRandomization = Globals.Config.SecondsUntilInventoryRandomization;

    internal static void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
    {
        // only count down if save is loaded and player has control
        if (!Context.IsWorldReady || !Game1.player.CanMove)
            return;

        SecondsUntilRandomization--;

        switch (SecondsUntilRandomization)
        {
            // send chat message on 60 second intervals and at 30 seconds
            case > 0 when SecondsUntilRandomization % 60 == 0:
            case 30:
                if (!Globals.Config.ChatMessageAlerts)
                    return;

                ChatManager.DisplayTimedChatMessage($"Randomizing inventory in {SecondsUntilRandomization} seconds...", 120);
                break;

            // send chat messages on final 5 seconds
            case > 0 and < 6:
                // clear my chat messages as they show up, to avoid clogging the chat
                if (!Globals.Config.ChatMessageAlerts)
                    return;

                ChatManager.ClearPreviousMessages();
                ChatManager.DisplayTimedChatMessage(
                    $"Randomizing inventory in {SecondsUntilRandomization} {(SecondsUntilRandomization == 1 ? "second" : "seconds")}...",
                    120
                );
                
                break;

            // randomize and reset timer at 0
            case <= 0:
                // if sound config is turned on, play sound
                if (Globals.Config.PlaySoundOnRandomization)
                {
                    Game1.playSound("cowboy_powerup");
                }

                if (Globals.Config.ChatMessageAlerts)
                {
                    ChatManager.ClearPreviousMessages();
                    ChatManager.DisplayTimedChatMessage("Inventory randomized!", 150);
                }

                InventoryRandomizer.RandomizeInventory();
                ResetTimer();
                break;

            default:
                return;
        }
    }

    internal static void ResetTimer()
    {
        SecondsUntilRandomization = Globals.Config.SecondsUntilInventoryRandomization;
    }
}
