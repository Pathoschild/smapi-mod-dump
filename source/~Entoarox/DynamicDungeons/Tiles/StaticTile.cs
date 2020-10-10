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
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace Entoarox.DynamicDungeons.Tiles
{
    internal class StaticTile : Tile
    {
        /*********
        ** Fields
        *********/
        private readonly int Index;
        private readonly string Sheet;


        /*********
        ** Public methods
        *********/
        public StaticTile(int x, int y, string layer, int index, string sheet)
            : base(x, y, layer)
        {
            this.Index = index >= 0 ? index : throw new ArgumentOutOfRangeException(nameof(index));
            this.Sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
        }

        public override void Apply(int x, int y, Map map)
        {
            Layer layer = map.GetLayer(this.Layer);
            layer.Tiles[this.X + x, this.Y + y] = new xTile.Tiles.StaticTile(layer, map.GetTileSheet(this.Sheet), BlendMode.Additive, this.Index);
        }
    }
}
