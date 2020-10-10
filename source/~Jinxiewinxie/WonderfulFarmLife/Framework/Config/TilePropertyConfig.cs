/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jinxiewinxie/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WonderfulFarmLife.Framework.Constants;

namespace WonderfulFarmLife.Framework.Config
{
    /// <summary>Defines properties to set on a map tile.</summary>
    internal class TilePropertyConfig
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

        /// <summary>The extra fields that don't match a model property.</summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionFields { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get the tile properties for this coordinate.</summary>
        /// <param name="tilesheets">The tilesheet aliases.</param>
        /// <param name="defaultTilesheet">The default tilesheet for tiles that don't specify one.</param>
        public IEnumerable<TileProperty> GetProperties(IDictionary<string, string> tilesheets, string defaultTilesheet)
        {
            foreach (var pair in this.ExtensionFields)
                yield return new TileProperty(this.Layer, this.X, this.Y, pair.Key, pair.Value.Value<string>());
        }
    }
}
