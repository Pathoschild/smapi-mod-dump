/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/YariazenMods
**
*************************************************/

using Newtonsoft.Json;
using Semver;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenLib.src.ContentPack.Models
{
    public class ModContent
    {
        [JsonProperty("Format", Required = Required.Always)]
        public SemVersion Format { get; set; }
        [JsonProperty("AssetBundle")]
        public AssetBundle Bundle { get; set; }
        [JsonProperty("Changes", Required = Required.Always)]
        public List<ModChange> Changes { get; set; }
    }
}