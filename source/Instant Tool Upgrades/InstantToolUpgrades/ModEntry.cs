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

namespace InstantToolUpgrades
{
    public class ModEntry : Mod, IAssetEditor
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if(asset.AssetName.Contains("StringsFromCSFiles"))
            {
                return true;
            }
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {            
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
            data["ShopMenu.cs.11474"] = Helper.Translation.Get("crafting-window");
            data["Tool.cs.14317"] = Helper.Translation.Get("post-purchase-dialogue");
        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            // Any time the player submits a tool to be upgraded...
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