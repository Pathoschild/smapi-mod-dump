/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/domsim1/stardew-valley-deluxe-hats-mod
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace DeluxeHats
{
    public enum HatNames 
    {
    
    }
    public class ModEntry : Mod
    {
        
        public override void Entry(IModHelper helper)
        {
            HatService.Monitor = Monitor;
            HatService.Helper = helper;
            HatService.Harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            HatService.HarmonyId = ModManifest.UniqueID;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.DayStarted += HatService.DayStarted;
            helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
            helper.Events.GameLoop.UpdateTicked += HatService.UpdateTicked;
            helper.Events.GameLoop.TimeChanged += HatService.TimeChanged;
            helper.Events.Player.InventoryChanged += HatService.InventoryChanged;
            helper.Events.Input.ButtonPressed += HatService.ButtonPressed;
            helper.Events.GameLoop.DayEnding += HatService.DayEnding;

            HatService.Harmony.Patch(
                original: AccessTools.Method(typeof(Hat), "loadDisplayFields"),
                prefix: new HarmonyMethod(typeof(HatService), nameof(HatService.LoadDisplayFields_Prefix)));
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Game1.player.hat.fieldChangeEvent += HatService.HatChanged;
        }

        private void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            HatService.CleanUp();
        } 
    }
}
