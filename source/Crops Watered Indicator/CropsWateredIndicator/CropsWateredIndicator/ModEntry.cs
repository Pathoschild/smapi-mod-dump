using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using XRectangle = xTile.Dimensions.Rectangle;

namespace CropsWateredIndicator
{
    public class ModEntry : Mod
    {

        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// Only listen to the RenderedWorld event.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderedWorld += Display_RenderedWorld;
        }

        /// <summary>Raised after the game world is drawn to the sprite patch, before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            this.Config = this.Helper.ReadConfig<ModConfig>();
            var tint = this.Config.Tint;

            var location = Game1.currentLocation;
            var visibleTiles = this.GetVisibleTiles();
            foreach (var tile in visibleTiles)
            {
                HoeDirt dirt = GetDirt(location, tile);
                if (dirt?.crop == null) continue;
                var crop = dirt.crop;
                var watered = dirt.state.Value == HoeDirt.watered;

                if (watered) continue;
                
                var color = new Color(0, 0, 0, tint);
                crop.draw(e.SpriteBatch, tile, color, 0);
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the tiles currently visible to the player.</summary>
        /// Taken from Pathoschild's DataLayers mod https://github.com/Pathoschild/StardewMods/tree/develop/DataLayers
        private IEnumerable<Vector2> GetVisibleTiles()
        {
            Rectangle visibleArea = this.GetVisibleTileArea(Game1.viewport);
            for (var x = visibleArea.Left; x < visibleArea.Right; x++)
            {
                for (var y = visibleArea.Top; y < visibleArea.Bottom; y++)
                    yield return new Vector2(x, y);
            }
        }

        /// <summary>Get the tile area currently visible to the player.</summary>
        /// <param name="viewport">The game viewport.</param>
        /// Taken from Pathoschild's DataLayers mod https://github.com/Pathoschild/StardewMods/tree/develop/DataLayers
        private Rectangle GetVisibleTileArea(XRectangle viewport)
        {
            const int tileSize = Game1.tileSize;
            var left = viewport.X / tileSize;
            var top = viewport.Y / tileSize;
            var width = (int)Math.Ceiling(viewport.Width / (decimal)tileSize);
            var height = (int)Math.Ceiling(viewport.Height / (decimal)tileSize);

            return new Rectangle(left - 1, top - 1, width + 2, height + 2); // extend slightly off-screen to avoid tile pop-in at the edges
        }

        /// <summary>Get the dirt instance for a tile, if any.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="tile">The tile to check.</param>
        /// Taken from Pathoschild's DataLayers mod https://github.com/Pathoschild/StardewMods/tree/develop/DataLayers
        private static HoeDirt GetDirt(GameLocation location, Vector2 tile)
        {
            if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature terrain) && terrain is HoeDirt dirt)
                return dirt;
            if (location.objects.TryGetValue(tile, out StardewValley.Object obj) && obj is IndoorPot pot)
                return pot.hoeDirt.Value;

            return null;
        }
    }
}

