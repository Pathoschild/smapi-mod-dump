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
using System.Collections.Generic;
using System.Linq;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace Entoarox.DynamicDungeons.Tiles
{
    internal class AnimatedTile : Tile
    {
        /*********
        ** Fields
        *********/
        /// <summary>The tilesheet names and animation indexes.</summary>
        private readonly Tuple<string, int>[] Frames;
        private readonly int Interval;


        /*********
        ** Public methods
        *********/
        public AnimatedTile(int x, int y, string layer, int[] frames, string sheet, int interval)
            : this(x, y, layer, frames.Select(a => Tuple.Create(sheet, a)).ToArray(), interval) { }

        public AnimatedTile(int x, int y, string layer, Tuple<string, int>[] frames, int interval)
            : base(x, y, layer)
        {
            this.Frames = frames;
            this.Interval = interval;
        }

        public override void Apply(int x, int y, Map map)
        {
            Layer layer = map.GetLayer(this.Layer);
            layer.Tiles[this.X + x, this.Y + y] = new xTile.Tiles.AnimatedTile(layer, this.GetFrameTiles(layer, map).ToArray(), this.Interval);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the static animation frames.</summary>
        /// <param name="layer">The layer on which to add it.</param>
        /// <param name="map">The map for which to add it.</param>
        private IEnumerable<xTile.Tiles.StaticTile> GetFrameTiles(Layer layer, Map map)
        {
            foreach (Tuple<string, int> entry in this.Frames)
                yield return new xTile.Tiles.StaticTile(layer, map.GetTileSheet(entry.Item1), BlendMode.Additive, entry.Item2);
        }
    }
}
