using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WonderfulFarmLife.Framework.Config
{
    /// <summary>Defines properties to set on a map tile.</summary>
    internal class TileIndexPropertyConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The tile ID in the tilesheet.</summary>
        public int ID { get; set; }

        /// <summary>The extra fields that don't match a model property.</summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionFields { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get the tilesheet properties.</summary>
        /// <param name="tilesheets">The tilesheet aliases.</param>
        /// <param name="defaultTilesheet">The default tilesheet for tiles that don't specify one.</param>
        public IEnumerable<TileIndexProperty> GetProperties(IDictionary<string, string> tilesheets, string defaultTilesheet)
        {
            foreach (var pair in this.ExtensionFields)
                yield return new TileIndexProperty(defaultTilesheet, this.ID, pair.Key, pair.Value.Value<string>());
        }
    }
}
