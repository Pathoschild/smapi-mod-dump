/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal abstract class TileInfo
    {
        /*********
        ** Accessors
        *********/
#pragma warning disable CS0649
        public string Conditions;
        public string MapName;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Optional;

        public int TileX;
        public int TileY;
#pragma warning restore CS0649
    }
}
