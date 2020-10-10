/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jinxiewinxie/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WonderfulFarmLife.Framework.Constants;

namespace WonderfulFarmLife.Framework.Config
{
    /// <summary>Defines an override to apply to a tile position.</summary>
    internal class TileConfig
    {
        /*********
        ** Accessors
        *********/
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
        /// <summary>Get the tile overrides for this coordinate.</summary>
        /// <param name="tilesheets">The tilesheet aliases.</param>
        /// <param name="defaultTilesheet">The default tilesheet for tiles that don't specify one.</param>
        public IEnumerable<TileOverride> GetOverrides(IDictionary<string, string> tilesheets, string defaultTilesheet)
        {
            foreach (TileLayer layer in Enum.GetValues(typeof(TileLayer)))
            {
                string key = layer.ToString();

                // no override
                if (!this.ExtensionFields.ContainsKey(key))
                    continue;

                // parse
                JToken token = this.ExtensionFields[key];
                int?[] tileIDs = token is JArray
                    ? token.Values<int?>().ToArray()
                    : new[] { token.Value<int?>() };

                // build overrides
                foreach (int? tileID in tileIDs)
                {
                    string tilesheet = tileID != null ? defaultTilesheet : null;
                    yield return new TileOverride(layer, this.X, this.Y, tileID, tilesheet);
                }
            }
        }
    }
}
