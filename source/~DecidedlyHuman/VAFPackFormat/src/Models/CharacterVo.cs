/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace VAFPackFormat.Models
{
    public class CharacterVo
    {
        [JsonProperty("Format")] public string FormatVersion;
        public string Directory;
        public int Priority;
        public string Character { get; set; }
        public List<Voiceover> Voiceover;
    }
}
