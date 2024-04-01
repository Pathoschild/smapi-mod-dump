/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miome/MultitoolMod
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using MultitoolMod.Framework;

namespace MultitoolMod
{
    /// <summary>The mod entry point.</summary>
    public class MultitoolMod : Mod
    {
        private ModConfig Config;
        private Multitool multitool;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.SaveCreated += this.OnSaveCreated;
        }
        private void initalizeMultitool()
        {
            if (this.multitool == null)
            {
                this.multitool = new Multitool(this);
            }
        }
        private void OnSaveCreated(object sender, SaveCreatedEventArgs e)
        {
            initalizeMultitool();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            initalizeMultitool();
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.Config.InfoButton.JustPressed())
            {
                multitool.cursor = e.Cursor;
                int x = (int)e.Cursor.AbsolutePixels.X;
                int y = (int)e.Cursor.AbsolutePixels.Y;
                //int xtile = (int)x / Game1.tileSize;
                //int ytile = (int)y / Game1.tileSize;
                int xtile = (int)e.Cursor.Tile.X;
                int ytile = (int)e.Cursor.Tile.Y;
                GameLocation location = Game1.player.currentLocation;
                Vector2 tileVec = e.Cursor.Tile;
                IDictionary<String, System.Object> properties = multitool.Get_Properties(x, y);
                string formattedProperties = $"At {x}/{y} (tile {xtile}/{ytile}) found the following properties: " + multitool.Format_Properties(properties);
                this.Monitor.Log(formattedProperties,LogLevel.Info);
                Game1.addHUDMessage(new HUDMessage(formattedProperties));
            }
            else if (this.Config.ToolButton.JustPressed())
            {
                multitool.cursor = e.Cursor;
                int powerupLevel = 1;
                int x = (int)e.Cursor.AbsolutePixels.X;
                int y = (int)e.Cursor.AbsolutePixels.Y;
                //int xtile = (int)x / Game1.tileSize;
                //int ytile = (int)y / Game1.tileSize;
                int xtile = (int)e.Cursor.Tile.X;
                int ytile = (int)e.Cursor.Tile.Y;
                GameLocation location = Game1.player.currentLocation;
                Vector2 tileVec = e.Cursor.Tile;
                multitool.DoFunction(Game1.currentLocation, x, y, powerupLevel, Game1.player);
            }
            // else if ( e.Button == this.Config.CleanButton ){
            //    multitool.cleanInventory(Game1.player);
            //}
        }
    }
}
