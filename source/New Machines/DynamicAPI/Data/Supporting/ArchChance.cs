/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

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