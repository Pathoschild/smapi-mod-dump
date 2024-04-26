/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace AlwaysOrganizeChests
{
    public class ModEntry : Mod
    {
        int organizedOnTick = 0;

        public override void Entry(IModHelper helper)
        {
            helper.Events.World.ChestInventoryChanged += this.OnChestInventoryChanged;
        }
        private void OnChestInventoryChanged(object? sender, ChestInventoryChangedEventArgs e)
        {
            // For items with a durability (like bobbers), organizing the inventory can cause the "ChestInventoryChanged" event to be fired again.
            // Thus, to avoid an infinite loop, we ensure that we did not already organize the inventory on this frame or the last frame.
            if (organizedOnTick != Game1.ticks && organizedOnTick != Game1.ticks - 1)
            {
                ItemGrabMenu.organizeItemsInList(e.Chest.Items);
                organizedOnTick = Game1.ticks;
            }
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
