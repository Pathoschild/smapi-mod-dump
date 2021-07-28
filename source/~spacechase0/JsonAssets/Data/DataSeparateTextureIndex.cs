/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using SpaceShared;

namespace JsonAssets.Data
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = DiagnosticMessages.IsPublicApi)]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.IsPublicApi)]
    public abstract class DataSeparateTextureIndex : DataNeedsId
    {
        /*********
        ** Accessors
        *********/
        [JsonIgnore]
        internal int TextureIndex { get; set; } = -1;

        // The following is mainly data for the Content Patcher integration.

        [JsonIgnore]
        public string Tilesheet { get; set; }

        [JsonIgnore]
        public int TilesheetX { get; set; }

        [JsonIgnore]
        public int TilesheetY { get; set; }
    }
}
