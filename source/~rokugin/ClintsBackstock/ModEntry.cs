/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ClintsBackstock {
    internal class ModEntry : Mod {

        public override void Entry(IModHelper helper) {
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
            if (e.Name.IsEquivalentTo("Strings/StringsFromCSFiles")) {
                e.Edit(asset => {
                    asset.AsDictionary<string, string>().Data["ShopMenu.cs.11474"] = Helper.Translation.Get("crafting-window");
                    asset.AsDictionary<string, string>().Data["Tool.cs.14317"] = Helper.Translation.Get("post-purchase-dialogue");
                });
            }
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e) {
            if (Game1.player.daysLeftForToolUpgrade.Value > 0) {
                Game1.player.daysLeftForToolUpgrade.Value = 0;
            }
        }

    }
}
