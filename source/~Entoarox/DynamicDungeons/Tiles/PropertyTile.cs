/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using xTile;

namespace Entoarox.DynamicDungeons.Tiles
{
    internal class PropertyTile : Tile
    {
        /*********
        ** Fields
        *********/
        private readonly string Key;
        private readonly string Value;


        /*********
        ** Public methods
        *********/
        public PropertyTile(int x, int y, string layer, string key, string value)
            : base(x, y, layer)
        {
            this.Key = key;
            this.Value = value;
        }

        public override void Apply(int x, int y, Map map)
        {
            map.GetLayer(this.Layer).Tiles[this.X + x, this.Y + y]?.Properties.Add(this.Key, this.Value);
        }
    }
}
