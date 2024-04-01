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
using StardewValley.Menus;

namespace AlwaysOrganizeChests
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.World.ChestInventoryChanged += this.OnChestInventoryChanged;
        }
        private void OnChestInventoryChanged(object? sender, ChestInventoryChangedEventArgs e)
        {
            ItemGrabMenu.organizeItemsInList(e.Chest.Items);
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
