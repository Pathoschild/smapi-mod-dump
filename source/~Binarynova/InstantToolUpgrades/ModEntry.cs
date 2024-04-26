/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Binarynova/MyStardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using StardewValley.Inventories;

namespace InstantToolUpgrades
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e) {
            if(e.Name.IsEquivalentTo("Strings/StringsFromCSFiles")) {
                e.Edit(asset => {
                    asset.AsDictionary<string, string>().Data["ShopMenu.cs.11474"] = Helper.Translation.Get("crafting-window");
                    asset.AsDictionary<string, string>().Data["Tool.cs.14317"] = Helper.Translation.Get("post-purchase-dialogue");
                });
            }
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                // Check to see if it's a genericTool...
                if (Game1.player.toolBeingUpgraded.Value is StardewValley.Tools.GenericTool genericTool)
                {
                    // ...and upgrade it.
                    genericTool.actionWhenClaimed();
                }
                else
                {
                    // Otherwise give the player their new tool.
                    Game1.player.addItemToInventory(Game1.player.toolBeingUpgraded.Value);
                }

                // Finally, cancel the queued upgrade.
                Game1.player.toolBeingUpgraded.Value = null;
                Game1.player.daysLeftForToolUpgrade.Value = 0;
            }
        }
	}
}