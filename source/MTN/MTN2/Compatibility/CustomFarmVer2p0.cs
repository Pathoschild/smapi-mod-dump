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
using System.Reflection;

namespace MTN2.Compatibility
{
    /// <summary>
    /// CustomFarm Class. Contains all the information for a single custom farm map to operate. 
    /// Used primarily to retain the data. Does not perform operations itself.
    /// </summary>
    public class CustomFarmVer2p0 {
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

        //Custom Greenhouse Type link
        public string StartingGreenHouse { get; set; }
        [JsonIgnore]
        public CustomGreenHouse CustomGreenhouse { get; set; }

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

        public static void Convert(CustomFarm farm, CustomFarmVer2p0 oldFarm) {
            farm.ID = oldFarm.ID;
            farm.Name = oldFarm.Name;

            string[] description = oldFarm.Description.Split('_');
            if (description.Length == 0) {
                farm.DescriptionName = "MissingName";
                farm.DescriptionDetails = "MissingDetails";
            } else if (description.Length == 1) {
                farm.DescriptionName = "MissingName";
                farm.DescriptionDetails = description[0];
            } else {
                farm.DescriptionName = description[0];
                farm.DescriptionDetails = description[1];
            }

            farm.Folder = oldFarm.Folder;
            farm.Icon = oldFarm.Icon;
            farm.Version = 2.1f;

            farm.StartingGreenHouse = oldFarm.StartingGreenHouse;

            farm.CabinCapacity = oldFarm.CabinCapacity;
            farm.AllowClose = oldFarm.AllowClose;
            farm.AllowSeperate = oldFarm.AllowSeperate;

            farm.FarmMap = oldFarm.FarmMap;
            farm.AdditionalMaps = oldFarm.AdditionalMaps;

            farm.FarmHouse = oldFarm.FarmHouse;
            farm.GreenHouse = oldFarm.GreenHouse;
            farm.FarmCave = oldFarm.FarmCave;
            farm.ShippingBin = oldFarm.ShippingBin;
            farm.MailBox = oldFarm.MailBox;
            farm.GrandpaShrine = oldFarm.GrandpaShrine;
            farm.RabbitShrine = oldFarm.RabbitShrine;
            farm.PetWaterBowl = oldFarm.PetWaterBowl;

            farm.Neighbors = oldFarm.Neighbors;

            farm.Overrides = oldFarm.Overrides;

            farm.ResourceClumps = oldFarm.ResourceClumps;
            farm.Foraging = oldFarm.Foraging;
            farm.Ores = oldFarm.Ores;
            farm.SpawnMonstersAtNight = oldFarm.SpawnMonstersAtNight;

            farm.FurnitureList = oldFarm.FurnitureList;
            farm.ObjectList = oldFarm.ObjectList;
            farm.FurnitureLayoutFromCanon = oldFarm.FurnitureLayoutFromCanon;
            return;
        }
    }
}
