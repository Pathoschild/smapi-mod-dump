/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jinxiewinxie/StardewValleyMods
**
*************************************************/

using WonderfulFarmLife.Framework.Constants;

namespace WonderfulFarmLife.Framework.Config
{
    /// <summary>Defines a property to set on a map tile.</summary>
    internal class TileProperty
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The map layer.</summary>
        public TileLayer Layer { get; set; }

        /// <summary>The X tile coordinate.</summary>
        public int X { get; set; }

        /// <summary>The Y tile coordinate.</summary>
        public int Y { get; set; }

        /// <summary>The property key.</summary>
        public string Key { get; set; }

        /// <summary>The property value.</summary>
        public string Value { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="layer">The map layer.</param>
        /// <param name="x">The X tile coordinate.</param>
        /// <param name="y">The Y tile coordinate.</param>
        /// <param name="key">The property key.</param>
        /// <param name="value">The property value.</param>
        public TileProperty(TileLayer layer, int x, int y, string key, string value)
        {
            this.Layer = layer;
            this.X = x;
            this.Y = y;
            this.Key = key;
            this.Value = value;
        }
    }
}
