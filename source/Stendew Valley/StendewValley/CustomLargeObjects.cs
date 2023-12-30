/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HauntedPineapple/Stendew-Valley
**
*************************************************/

/// Project: Stendew Valley
/// File: CustomLargeObjects.cs
/// Description: Holds class representing a multi tile object that can be spawned
/// Author: Team Stendew Valley

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.Tiles;

namespace StendewValley
{
    /// <summary>
    /// Custom class for multi tile objects that can be added and removed
    /// </summary>
    public class CustomLargeObject
    {
        // Fields
        public string id;
        private bool enabled;

        // Stores tile elements of the object
        private List<CustomObjectTile> elements = new List<CustomObjectTile>();

        // Stores the map the object is located in
        private GameLocation location;

        // Constructor
        public CustomLargeObject(string id, bool enabled, GameLocation loc)
        {
            this.id = id;
            this.enabled = enabled;
            this.location = loc;
        }

        // Properties
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value != enabled)
                {
                    if (value) { AddObject(); }
                    else { RemoveObject(); }
                }
                enabled = value;
            }
        }

        // Methods

        /// <summary>
        /// Sets the sprite of a specific tile
        /// </summary>
        /// <param name="x">X location in map</param>
        /// <param name="y">y location in map</param>
        /// <param name="layer">Layer to spawn into (buildings, front, etc)</param>
        /// <param name="tileIndex">Index of tile in the tile sheet</param>
        /// <param name="tileSheetIndex">Index of the tilesheet in the map file</param>
        public void SetSprite(int x, int y, string layer, int tileIndex, int tileSheetIndex)
        {
            // Adds tile to the list of elements
            elements.Add(new CustomObjectTile(x, y, layer, tileIndex, tileSheetIndex));
        }

        /// <summary>
        /// Helper method to spawn the object
        /// </summary>
        public void Spawn()
        {
            if (enabled)
            {
                AddObject();
            }
        }

        /// <summary>
        /// Helper method to remove the object
        /// </summary>
        private void RemoveObject()
        {
            foreach (CustomObjectTile tile in elements)
            {
                tile.Remove(location);
            }
        }

        /// <summary>
        /// Helper method to add the custom object to the map
        /// </summary>
        private void AddObject()
        {
            foreach (CustomObjectTile tile in elements)
            {
                tile.Add(location);
            }
        }
        
    }

    /// <summary>
    /// Custom class for object tiles that is used by the CustomLargeObjects class
    /// </summary>
    internal class CustomObjectTile
    {
        // Fields
        public int x_index;
        public int y_index;
        public string layer;
        public int tileSheetIndex;
        public int tileIndex;

        // Constructor
        public CustomObjectTile(int x, int y, string layer, int tileIndex, int tileSheetIndex)
        {
            this.x_index = x;
            this.y_index = y;
            this.layer = layer;
            this.tileIndex = tileIndex;
            this.tileSheetIndex = tileSheetIndex;
        }

        // Methods

        /// <summary>
        /// Removes the tile from the given map
        /// </summary>
        /// <param name="loc">Map to remove tile from</param>
        public void Remove(GameLocation loc)
        {
            // Removes the tile at the given coordinates and layer
            loc.removeTile(x_index, y_index, layer);
        }

        /// <summary>
        /// Adds a tile to the given map
        /// </summary>
        /// <param name="loc"></param>
        public void Add(GameLocation loc)
        {
            // Adds a tile to the given coordinates and layer with the given sprite
            loc.setMapTileIndex(x_index, y_index, tileIndex, layer, tileSheetIndex);
        }
    }
}
