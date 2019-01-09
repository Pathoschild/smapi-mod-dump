using Microsoft.Xna.Framework.Graphics;
using MTN2.MapData;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Objects;
using Newtonsoft.Json;

namespace MTN2
{
    /// <summary>
    /// CustomFarm Class. Contains all the information for a single custom farm map to operate. 
    /// Used primarily to retain the data. Does not perform operations itself.
    /// </summary>
    public class CustomFarm {
        //Fundalmentals
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Folder { get; set; }
        public string Icon { get; set; }
        public float Version { get; set; }

        [JsonIgnore]
        public Texture2D IconSource { get; set; }
        [JsonIgnore]
        public IContentPack ContentPack { get; set; }

        //Multiplayer
        public int CabinCapacity { get; set; } = 3;
        public bool AllowClose { get; set; } = true;
        public bool AllowSeperate { get; set; } = true;

        //Farm Map and Additional Maps
        public MapFile FarmMap { get; set; }
        public List<MapFile> AdditionalMaps { get; set; }

        //Structure Relocations
        public Structure FarmHouse { get; set; }
        public Structure GreenHouse { get; set; }
        public Structure FarmCave { get; set; }
        public Structure ShippingBin { get; set; }
        public Structure MailBox { get; set; }
        public Structure GrandpaShrine { get; set; }
        public Structure RabbitShrine { get; set; }
        public Structure PetWaterBowl { get; set; }

        //Neighboring Maps
        public List<Neighbor> Neighbors { get; set; }

        //Map Overrides
        public List<MapFile> Overrides { get; set; }

        //Behavioral
        public LargeDebris ResourceClumps { get; set; }
        public Forage Foraging { get; set; }
        public Ore Ores { get; set; }
        public bool SpawnMonstersAtNight = false;

        //Furniture
        public List<Furniture> FurnitureList { get; set; }
        public List<StardewValley.Object> ObjectList { get; set; }
        public int FurnitureLayoutFromCanon { get; set; } = -1;
    }
}
