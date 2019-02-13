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
