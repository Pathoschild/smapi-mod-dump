/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using MTN2.MapData;
using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2 {
    public class CustomGreenHouse {
        public string Name { get; set; }
        public string Folder { get; set; }
        public float Version { get; set; }

        [JsonIgnore]
        public IContentPack ContentPack { get; set; }

        public MapFile GreenhouseMap { get; set; }
        public Structure Enterance { get; set; }
    }
}
