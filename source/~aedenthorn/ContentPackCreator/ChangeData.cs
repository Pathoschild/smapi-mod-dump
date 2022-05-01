/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace ContentPackCreator
{
    public class ChangeData
    {
        public string Action = "";
        public string LogName = "";
        public string Update;
        public string Target = "";
        public string FromFile;

        public Dictionary<string, string> When;
        
        public string PatchMode;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Rectangle FromArea;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Rectangle ToArea;

        public Dictionary<string, string> MapProperties;
        public List<string> AddWarps;
        public List<TextOperation> TextOperations;
    }

    public class TextOperation
    {
        public string Operation;
        public string Value;
        public string Delimiter;
        public List<string> Target;
    }
}