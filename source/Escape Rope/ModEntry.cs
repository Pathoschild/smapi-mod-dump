/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bifibi/SDVEscapeRope
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace EscapeRope
{
    /// <summary>The mod entry point.</summary>
    /// multiplayer
    /// I18N
    public class ModEntry : Mod
    {
        private int escapeRopeId;
        private string animationAssetKey;
        private IModHelper helper;
        private JsonAssetsAPI jsonAssetsAPI;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.helper = helper;

            helper.Events.Display.MenuChanged += this.onMenuChanged;
            helper.Events.GameLoop.GameLaunched += this.onGameLaunched;
            helper.Events.Input.ButtonPressed += this.onButtonPressed;
        }

        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.menuUp || Game1.player.hasMenuOpen || e.IsSuppressed() || !e.Button.IsActionButton() || Game1.eventUp || !Context.IsPlayerFree) return;

            Item currentItem = Game1.player.CurrentItem;
            if (currentItem == null || (currentItem != null && currentItem.ParentSheetIndex != this.escapeRopeId)) // works
            {
                return;
            }
            // subscribe to update tick to verify that still no menu is up
            this.helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.menuUp || Game1.player.hasMenuOpen || Game1.eventUp || !Context.IsPlayerFree)
            {
                // the button click opened a menu
                this.helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
                return;
            }
            this.handleRopeInteraction();
            this.helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
        }

        private void handleRopeInteraction()
        {
            if (!(Game1.currentLocation is MineShaft))
            {     
                return;
            }

            Game1.player.reduceActiveItemByOne();
            Game1.player.jitterStrength = 1f;
            Game1.currentLocation.playSound("warrior");
            Game1.player.faceDirection(2);
            Game1.player.CanMove = false;
            Game1.player.temporarilyInvincible = true;
            Game1.player.temporaryInvincibilityTimer = -2000;
            Game1.changeMusicTrack("none");

            StardewValley.Network.NetPosition position = Game1.player.position;
            Vector2 positionAboveFarmer = new Vector2(position.X, position.Y - 150);
            var ropeAnimation = new TemporaryAnimatedSprite(
                textureName: this.animationAssetKey,
                sourceRect: new Rectangle(0, 0, 64, 64),
                animationInterval: 100,
                animationLength: 7,
                numberOfLoops: 1,
                position: positionAboveFarmer,
                flicker: false,
                flipped: false)
            {
                drawAboveAlwaysFront = true
            };
            Game1.currentLocation.temporarySprites.Add(ropeAnimation);

            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
                            {
                                new FarmerSprite.AnimationFrame(
                                    57,
                                    1000,
                                    secondaryArm: false,
                                    flip: false),
                                new FarmerSprite.AnimationFrame(
                                    (short)Game1.player.FarmerSprite.CurrentFrame,
                                    0,
                                    secondaryArm: false,
                                    flip: false,
                                    this.DoWarp,
                                    behaviorAtEndOfFrame: true)
                            });
        }

        private void DoWarp(Farmer who)
        {
            if (who.currentLocation is MineShaft)
            {
                if (Game1.CurrentMineLevel == 77377) // the QUARRY
                {
                    Game1.warpFarmer("Mine", 67, 10, flip: false);
                    return;
                }
                if (Game1.CurrentMineLevel > 120)
                {
                    Game1.warpFarmer("SkullCave", 3, 4, flip: false);
                }
                else
                {
                    Game1.warpFarmer("Mine", 17, 4, flip: false);
                }

            }
        }

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.jsonAssetsAPI = this.helper.ModRegistry.GetApi<JsonAssetsAPI>("spacechase0.JsonAssets");
            string path = Path.Combine(this.Helper.DirectoryPath, "assets");
            this.jsonAssetsAPI.LoadAssets(path);
            this.jsonAssetsAPI.IdsAssigned += this.onIdsAssigned;
            this.animationAssetKey = this.Helper.Content.GetActualAssetKey("assets/rope-animated.png", ContentSource.ModFolder);
            this.Monitor.Log($"loaded asset {this.animationAssetKey}");
        }

        private void onIdsAssigned(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, int> keyValuePair in this.jsonAssetsAPI.GetAllObjectIds())
            {
                this.Monitor.Log($"key: {keyValuePair.Key}, value: {keyValuePair.Value}");
            }

            this.escapeRopeId = this.jsonAssetsAPI.GetObjectId("Escape Rope");
            this.Monitor.Log($" id in assigned: {this.escapeRopeId}");
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!(e.NewMenu is ShopMenu menu) || menu.portraitPerson?.Name != "Dwarf")
                return;

            this.Monitor.Log($"Adding rope to Dwarf's shop.");

            var forSale = menu.forSale;
            var itemPriceAndStock = menu.itemPriceAndStock;

            var escapeRope = new StardewValley.Object(this.escapeRopeId, 1);
            forSale.Add(escapeRope);
            itemPriceAndStock.Add(escapeRope, new int[] { 2500, 2147483647 });
        }

    }
}

