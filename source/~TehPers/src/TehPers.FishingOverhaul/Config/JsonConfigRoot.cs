/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// A configuration.
    /// </summary>
    public abstract record JsonConfigRoot
    {
        private protected const string jsonSchemaRootUrl =
            "https://raw.githubusercontent.com/TehPers/StardewValleyMods/master/docs/TehPers.FishingOverhaul/schemas/";

        /// <summary>
        /// Optional '$schema' URL. This is ignored and exists entirely for convenience.
        /// </summary>
        [JsonProperty("$schema", DefaultValueHandling = DefaultValueHandling.Ignore)]
        protected abstract string? Schema { get; }
    }
}
