/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/Personal-Anvil
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Shops;



namespace PersonalAnvil
{
    public class ModEntry : Mod
    {
        private string _anvilQualifiedId;
        private string _anvilId;
        private int _leftClickXPos;
        private int _leftClickYPos;

        public override void Entry(IModHelper helper)
        {
            _anvilId = ModManifest.UniqueID + "_Anvil";
            _anvilQualifiedId = ItemRegistry.type_bigCraftable + _anvilId;

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (e.Button == SButton.MouseLeft)
            {
                _leftClickXPos = (int)e.Cursor.ScreenPixels.X;
                _leftClickYPos = (int)e.Cursor.ScreenPixels.Y;
            }

            if (!e.Button.IsActionButton()) return;
            var tile = Helper.Input.GetCursorPosition().Tile;
            Game1.currentLocation.Objects.TryGetValue(tile, out var obj);
            if (obj == null || !obj.bigCraftable.Value) return;
            if (obj.QualifiedItemId == _anvilQualifiedId)
                Game1.activeClickableMenu = new WorkbenchGeodeMenu(Helper);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            string modId = this.ModManifest.UniqueID;
            
            //add item data
            if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, BigCraftableData>().Data;
                    data[this._anvilId] = new BigCraftableData
                    {
                        Name = this._anvilId,
                        DisplayName = "Personal Anvil",
                        Description = "Use this to break geodes, troves, golden coconuts and mystery boxes.",
                        Texture = $"LooseSprites/{modId}"
                    };
                });
            }

            // add to shop
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ShopData>().Data;
                    if (data.TryGetValue(Game1.shop_blacksmith, out ShopData shop))
                    {
                        shop.Items.Add(new ShopItemData
                        {
                            Id = this._anvilId,
                            ItemId = this._anvilId,
                            Price = 10000,
                            Condition = "PLAYER_FRIENDSHIP_POINTS Current Clint 1250, PLAYER_STAT Current geodesCracked 50"
                        }
                        );
                    }
                });
            }

            // add texture
            else if (e.NameWithoutLocale.IsEquivalentTo($"LooseSprites/{modId}"))
                e.LoadFromModFile<Texture2D>("assets/anvil.png", AssetLoadPriority.Exclusive);



        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Re-send a left click to the geode menu if one is already not being broken, the player has the room and money for it, and the click was on the geode spot.
            if (!e.IsMultipleOf(4) || !Helper.Input.IsDown(SButton.MouseLeft) ||
                !(Game1.activeClickableMenu is WorkbenchGeodeMenu menu))
                return;
            var clintNotBusy = menu.heldItem != null && Utility.IsGeode(menu.heldItem) && menu.geodeAnimationTimer <= 0;

            var playerHasRoom = Game1.player.freeSpotsInInventory() > 1 || Game1.player.freeSpotsInInventory() == 1 &&
                menu.heldItem != null && menu.heldItem.Stack == 1;

            if (clintNotBusy && playerHasRoom && menu.geodeSpot.containsPoint(_leftClickXPos, _leftClickYPos))
            {
                menu.receiveLeftClick(_leftClickXPos, _leftClickYPos, false);
            }
        }
    }
}