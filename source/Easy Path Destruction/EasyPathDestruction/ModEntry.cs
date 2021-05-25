/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/Stardew-EasyPathDestruction
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace EasyPathDestruction
{
    public class ModEntry : Mod
    {
        private ModConfig config;

        bool userNeedsToBeNextToTile;
        bool destructionModeActive;
        GameLocation location;
        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            userNeedsToBeNextToTile = config.RequireUserToBeNextToTile;
            destructionModeActive = false;

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Player.Warped += OnPlayerWarped;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            location = Game1.currentLocation;
        }

        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            location = e.NewLocation;
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (e.Pressed.Contains(SButton.J))
            {
                destructionModeActive = !destructionModeActive;
                if (destructionModeActive)
                {
                    Game1.chatBox.addMessage("Destruction Mode Enabled", Color.White);
                }
                else
                {
                    Game1.chatBox.addMessage("Destruction Mode Disabled", Color.White);
                }
            } 
            if (destructionModeActive && e.Held.Contains(SButton.MouseLeft))
            {
                Vector2 tile = e.Cursor.GrabTile;
                Pickaxe pickaxe = new Pickaxe();
                float stamina = Game1.player.stamina;
                if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature) && feature is Flooring)
                {
                    if (userNeedsToBeNextToTile)
                    {
                        if (PlayerIsNextToTile(tile))
                        {
                            pickaxe.DoFunction(location, (int)(tile.X * Game1.tileSize), (int)(tile.Y * Game1.tileSize), 1, Game1.player);
                            Game1.player.stamina = stamina;
                        }
                    }
                    else
                    {
                        pickaxe.DoFunction(location, (int)tile.X, (int)tile.Y, 1, Game1.player);
                        Game1.player.stamina = stamina;
                    }
                }
            }
        }

        private bool PlayerIsNextToTile(Vector2 tile)
        {
            Vector2 playerTile = Game1.player.getTileLocation();
            float xDistance = Math.Abs(tile.X - playerTile.X);
            float yDistance = Math.Abs(tile.Y - playerTile.Y);
            return xDistance <= 1 && yDistance <= 1;
        }
    }

    class ModConfig
    {
        public bool RequireUserToBeNextToTile { get; set; } = true;
    }
}