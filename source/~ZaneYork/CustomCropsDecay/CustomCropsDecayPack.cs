/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZaneYork/SDV_Mods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewModdingAPI;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace CustomCropsDecay
{
    class CustomCropsDecayPack
    {
        public string folderName { get; set; }
        public string fileName { get; set; }
        public string baseFolder { get; set; } = "ContentPack";
        internal IContentPack contentPack { get; set; } = null;
        public string useid { get; set; } = "";
        public string author { get; set; } = "none";
        public string version { get; set; } = "1.0.0";
        public string name { get; set; } = "Custom Crops Decay Pack";
        public List<CustomCropsDecayData> crops { get; set; } = new List<CustomCropsDecayData>();
    }
}
