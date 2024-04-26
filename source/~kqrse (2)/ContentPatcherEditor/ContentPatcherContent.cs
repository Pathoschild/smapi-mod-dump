/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ContentPatcherEditor
{
    public class ContentPatcherContent
    {
        public string Format;
        public List<JObject> Changes;
        public Dictionary<string, ConfigVar> ConfigSchema;
        public List<Dictionary<string, List<KeyValuePair<string, JToken?>>>> lists;
    }

    public class ConfigVar
    {
        public string AllowValues;
        public bool AllowBlank;
        public bool AllowMultiple;
        public string Default;
    }
}