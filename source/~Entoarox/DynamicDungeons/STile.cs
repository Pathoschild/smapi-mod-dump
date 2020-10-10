/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using xTile.Layers;
using xTile.Tiles;

namespace Entoarox.DynamicDungeons
{
    internal struct STile : ITile
    {
        /*********
        ** Accessors
        *********/
        public int X { get; set; }
        public int Y { get; set; }
        public Layer Layer { get; set; }
        public TileSheet Sheet;
        public int Index;


        /*********
        ** Public methods
        *********/
        public STile(int x, int y, Layer layer, TileSheet sheet, int index)
        {
            this.X = x;
            this.Y = y;
            this.Layer = layer;
            this.Sheet = sheet;
            this.Index = index;
        }

        public xTile.Tiles.Tile Get()
        {
            return new StaticTile(this.Layer, this.Sheet, BlendMode.Additive, this.Index);
        }
    }
}
