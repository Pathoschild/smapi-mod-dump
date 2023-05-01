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
    public class NPC_Schedules_Changes
    {
        public void GameLoop_DayStarted(StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.BNWChapter1"))
            {
                var robin = Game1.getCharacterFromName("Robin");
                robin.shouldPlayRobinHammerAnimation.Value = false;
                robin.ignoreScheduleToday = false;
                robin.resetCurrentDialogue();
                robin.reloadDefaultLocation();
                Game1.warpCharacter(robin, robin.DefaultMap, robin.DefaultPosition / 64f);
                var jas = Game1.getCharacterFromName("Jas");
                Game1.warpCharacter(jas, jas.DefaultMap, jas.DefaultPosition / 64f);
                var marnie = Game1.getCharacterFromName("Marnie");
                Game1.warpCharacter(marnie, marnie.DefaultMap, marnie.DefaultPosition / 64f);
                var shane = Game1.getCharacterFromName("Shane");
                Game1.warpCharacter(shane, shane.DefaultMap, shane.DefaultPosition / 64f);
            }
        }
    }
}
