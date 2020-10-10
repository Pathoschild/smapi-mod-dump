/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miketweaver/BashNinja_SDV_Mods
**
*************************************************/

﻿using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace MapPatcher
{
    public class MapPatcher : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            //Start example at the beginnig of each day
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs eventArgs)
        {
            //Example file
            Map greenhouse = this.Helper.Content.Load<Map>(@"assets\greenhousemap.xnb", ContentSource.ModFolder);
            //Example
            this.PatchMap(Game1.getLocationFromName("Greenhouse"), greenhouse, "untitled tile sheet", 64, 64);
        }

        /// <summary>Patch a map into a GameLocation.</summary>
        /// <param name="farmLocation">The Location to patch.</param>
        /// <param name="map">The Map to patch in.</param>
        /// <param name="givenTileSheet">The tilesheet to use for drawing.</param>
        /// <param name="xLocation">The X location to start patching at. Left most location.</param>
        /// <param name="yLocation">The X location to start patching at. Top most location.</param>
        public void PatchMap(GameLocation farmLocation, Map map, string givenTileSheet, int xLocation, int yLocation)
        {
            TileSheet tilesheet = farmLocation.map.GetTileSheet(givenTileSheet);

            string[] layers = { "Back", "Buildings", "Front" };
            foreach (string lay in layers)
            {
                Layer maplayer = map.GetLayer(lay);
                Layer layer = farmLocation.map.GetLayer(lay);

                bool sizeChanged = true;
                int farRight = map.GetLayer(lay).TileSize.Width + xLocation;
                int farBottom = map.GetLayer(lay).TileSize.Height + yLocation;

                if (layer.TileSize.Width >= farRight)
                {
                    sizeChanged = false;
                    farRight = layer.TileSize.Width;
                }

                if (layer.TileSize.Height >= farBottom)
                {
                    sizeChanged = false;
                    farBottom = layer.TileSize.Height;
                }

                if (sizeChanged)
                    layer.LayerSize = new xTile.Dimensions.Size(farRight, farBottom);

                for (int x = 0; x < maplayer.LayerSize.Width; x++)
                {
                    for (int y = 0; y < maplayer.LayerSize.Height; y++)
                    {
                        Tile specificTile = maplayer.Tiles[x, y];
                        if (specificTile != null)
                            layer.Tiles[xLocation + x, yLocation + y] = new StaticTile(layer, tilesheet, BlendMode.Alpha, specificTile.TileIndex);
                    }
                }
            }

        }


    }
}
