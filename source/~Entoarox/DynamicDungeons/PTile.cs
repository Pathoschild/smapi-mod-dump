/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using xTile.Layers;

namespace Entoarox.DynamicDungeons
{
    internal struct PTile : ITile
    {
        /*********
        ** Accessors
        *********/
        public int X { get; set; }
        public int Y { get; set; }
        public Layer Layer { get; set; }
        public string Key;
        public string Value;


        /*********
        ** Public methods
        *********/
        public PTile(int x, int y, Layer layer, string key, string value)
        {
            this.X = x;
            this.Y = y;
            this.Layer = layer;
            this.Key = key;
            this.Value = value;
        }

        public xTile.Tiles.Tile Get()
        {
            throw new NotImplementedException();
        }
    }
}
