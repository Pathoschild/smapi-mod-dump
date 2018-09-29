using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class ArchChance
    {
        #region	Properties

        [JsonProperty(Required = Required.Always)]
        public string Location { get; set; }

        [JsonProperty(Required = Required.Always)]
        public decimal Chance { get; set; }

        #endregion

        #region Serialization

        public override string ToString()
        {
            return $"{Location} {Chance:.#####}";
        }

        #endregion
    }
}