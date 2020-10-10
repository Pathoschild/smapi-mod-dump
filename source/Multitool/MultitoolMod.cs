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
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;
using StardewValley;
using MultitoolMod.Framework;
using SObject = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

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
            helper.Events.Input.ButtonPressed += this.onButtonPressed;
            this.multitool = new Multitool(this);
        }

        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (e.Button == this.Config.InfoButton)
            {
                int x = (int)e.Cursor.AbsolutePixels.X;
                int y = (int)e.Cursor.AbsolutePixels.Y;
                int xtile = (int)x / Game1.tileSize;
                int ytile = (int)y / Game1.tileSize;
                GameLocation location = Game1.player.currentLocation;
                Vector2 tileVec = new Vector2(xtile, ytile);
                IDictionary<String, System.Object> properties = multitool.Get_Properties(x, y);
                string formattedProperties = $"At {x}/{y} (tile {xtile}/{ytile}) found the following properties: " + multitool.Format_Properties(properties);
                this.Monitor.Log(formattedProperties);
                Game1.addHUDMessage(new HUDMessage(formattedProperties));
            }
            else if (e.Button == this.Config.ToolButton)
            {
                int powerupLevel = 1;
                int x = (int)e.Cursor.AbsolutePixels.X;
                int y = (int)e.Cursor.AbsolutePixels.Y;
                int xtile = (int)x / Game1.tileSize;
                int ytile = (int)y / Game1.tileSize;
                GameLocation location = Game1.player.currentLocation;
                Vector2 tileVec = new Vector2(xtile, ytile);
                multitool.DoFunction(Game1.currentLocation, x, y, powerupLevel, Game1.player);
            }
            // else if ( e.Button == this.Config.CleanButton ){
            //    multitool.cleanInventory(Game1.player);
            //}
        }
    }
}
