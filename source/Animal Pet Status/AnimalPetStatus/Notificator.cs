/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hakej/Animal-Pet-Status
**
*************************************************/

using StardewValley;

namespace AnimalPetStatus
{
    public class Notificator
    {
        public static void NotifyWithJingle()
        {
            Game1.addHUDMessage(new HUDMessage("All pets have been pet! :)", HUDMessage.newQuest_type));
            PlayJingle();
        }

        private static void PlayJingle()
        {
            DelayedAction.playSoundAfterDelay("drumkit4", 0);
            DelayedAction.playSoundAfterDelay("drumkit1", 200);
            DelayedAction.playSoundAfterDelay("drumkit2", 400);
            DelayedAction.playSoundAfterDelay("Duck", 575);
        }
    }
}
