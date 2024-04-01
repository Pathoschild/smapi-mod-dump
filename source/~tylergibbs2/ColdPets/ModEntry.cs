/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace ColdPets
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            Pet pet = Game1.player.getPet();
            if (pet is null || Game1.currentSeason != "winter" || !Context.IsMainPlayer)
                return;

            pet!.CurrentBehavior = Pet.behavior_SitDown;
            pet.warpToFarmHouse(Game1.player);
        }
    }
}
