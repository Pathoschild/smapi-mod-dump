/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AnotherPillow/WarpParsnip
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace WarpParsnip
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private GameLocation getRandomLocation()
        {
            int locationIndex = Game1.random.Next(Game1.locations.Count);
            return Game1.locations[locationIndex];
        }

        private int[] getRandomLocationCoords(GameLocation location)
        {
            var map = location.Map;
            int width = map.Layers[0].LayerWidth;
            int height = map.Layers[0].LayerHeight;

            int x = Game1.random.Next(width);
            int y = Game1.random.Next(height);

            return new int[] { x, y };
        }

        private void doWarp()
        {
            GameLocation location = getRandomLocation();
            GameLocation playerLocation = Game1.player.currentLocation;
            int[] coords = getRandomLocationCoords(location);
            Monitor.Log($"Warping to {location.Name}", LogLevel.Info);
            //Game1.warpFarmer(location.Name, coords[0], coords[1], this.getRandomFacingDirection());
            playerLocation.performTouchAction($"MagicWarp {location.Name} {coords[0]} {coords[1]}", Game1.player.position);
        }

        private bool checkIfItemParsnip(Item item)
        {
            string itemName = item.Name;
            Monitor.Log($"Removed {itemName}", LogLevel.Info);

            if (itemName == "Parsnip")
            {
                Monitor.Log($"Parsnip removed", LogLevel.Info);
                // doWarp();

                return true;
            }

            return false;
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            //wait for anim to end (game1.player.iseating become false)
            if (!Game1.player.isEating)
                return;
            
            


            //if (Game1.activeClickableMenu != null && !Game1.player.isEating)
            //    return;
            
            

            var removed = e.Removed;
            var changed = e.QuantityChanged;

            foreach (Item item in removed)
            {
                if (checkIfItemParsnip(item))
                {
                    Game1.player.forceCanMove();
                    doWarp();
                }
            
            }

            foreach (ItemStackSizeChange change in changed)
            {
                if (change.OldSize > change.NewSize && checkIfItemParsnip(change.Item))
                {
                    Game1.player.forceCanMove();
                    doWarp();
                }
            }

        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e) 
        {
            if (e.Name.IsEquivalentTo("Maps/springobjects"))
            {
                //replace 0, 0 to 64,64 with assets/asdf
                e.Edit((asset) =>
                {
                    var editor = asset.AsImage();

                    Rectangle rect = new Rectangle(0, 0, 16, 16);
                    var texture = Helper.ModContent.Load<Texture2D>("assets/WarpParsnip.png");

                    editor.PatchImage(texture, rect, new Rectangle(0, 16, 16, 16), PatchMode.Replace);
                });
            }
        }
    }
}
