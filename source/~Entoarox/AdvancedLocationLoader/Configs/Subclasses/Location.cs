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
    internal class Location : MapFileLink
    {
        /*********
        ** Accessors
        *********/
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Farmable;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Outdoor;

        [DefaultValue("Default")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string Type;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Location({this.MapName},{this.FileName}){{Outdoor={this.Outdoor},Farmable={this.Farmable},Type={this.Type}}}";
        }
    }
}
