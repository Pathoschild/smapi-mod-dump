/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Tilesheet : MapFileLink
    {
        /*********
        ** Accessors
        *********/
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Seasonal;

        public string SheetId;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Tilesheet({this.MapName}:{this.SheetId},{this.FileName ?? "null"}){{Seasonal={this.Seasonal}}}";
        }
    }
}
