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
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Compatibility {
    /// <summary>
    /// This class is a modified version of the first farmType. It is designed to allow
    /// for backwards compatibility.
    /// </summary>
    public class CustomFarmVer1 {
        //Required Information
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Folder { get; set; }
        public string Icon { get; set; }
        public float version { get; set; }

        //Multiplayer
        public int cabinCapacity = 3;
        public bool allowClose = true;
        public bool allowSeperate = true;

        //Farm Map
        public string farmMapFile { get; set; }
        public FileType farmMapType { get; set; } = FileType.xnb;
        public List<additionalTileSheet> additionalTileSheets { get; set; }

        //Additional Resources
        public List<additionalMap> additionalMaps { get; set; }

        //Structure Relocations
        public StructureInfo farmHouse { get; set; }
        public StructureInfo greenHouse { get; set; }
        public StructureInfo farmCave { get; set; }
        public StructureInfo shippingBin { get; set; }
        public StructureInfo mailBox { get; set; }
        public StructureInfo grandpaShrine { get; set; }
        public StructureInfo rabbitStatue { get; set; }
        public StructureInfo petWaterBowl { get; set; }
        public List<Neighbor> neighboringMaps { get; set; }

        //Maps to be overwritten
        public List<overrideMap> overrideMaps { get; set; }

        //Behavior
        public List<resourceSpawns> Resource { get; set; } = new List<resourceSpawns>();
        public List<forageSpawns> Forage { get; set; } = new List<forageSpawns>();
        //public List<oreSpawns> Mining { get; set; }
        //public List<fishingSpawns> Fishing { get; set; }
        public bool spawnMonstersAtNight { get; set; } = false;
        //public List<monsterSpawns> Monsters { get; set; }

        //Furniture
        public List<Furniture> furnitureList { get; set; }
        public List<StardewValley.Object> objectList { get; set; }
        public int furnitureLayoutFromCanon { get; set; } = -1;

        public static void Convert(CustomFarm farm, CustomFarmVer1 oldFarm) {
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
            farm.Version = oldFarm.version;

            farm.CabinCapacity = oldFarm.cabinCapacity;
            farm.AllowClose = oldFarm.allowClose;
            farm.AllowSeperate = oldFarm.allowSeperate;

            farm.FarmMap = new MapFile(oldFarm.farmMapFile, "Farm", "Farm", oldFarm.farmMapType);

            farm.AdditionalMaps = new List<MapFile>();
            additionalMap.Convert(farm, oldFarm);

            farm.FarmHouse = new Structure();
            StructureInfo.Convert(farm.FarmHouse, oldFarm.farmHouse);
            farm.GreenHouse = new Structure();
            StructureInfo.Convert(farm.GreenHouse, oldFarm.greenHouse);
            farm.FarmCave = new Structure();
            StructureInfo.Convert(farm.FarmCave, oldFarm.farmCave);
            farm.ShippingBin = new Structure();
            StructureInfo.Convert(farm.ShippingBin, oldFarm.shippingBin);
            farm.MailBox = new Structure();
            StructureInfo.Convert(farm.MailBox, oldFarm.mailBox);
            farm.GrandpaShrine = new Structure();
            StructureInfo.Convert(farm.GrandpaShrine, oldFarm.grandpaShrine);
            farm.RabbitShrine = new Structure();
            StructureInfo.Convert(farm.RabbitShrine, oldFarm.rabbitStatue);
            farm.PetWaterBowl = new Structure();
            StructureInfo.Convert(farm.PetWaterBowl, oldFarm.petWaterBowl);

            farm.Neighbors = new List<Neighbor>();
            farm.Neighbors = oldFarm.neighboringMaps;

            farm.Overrides = new List<MapFile>();
            overrideMap.Convert(farm, oldFarm);

            resourceSpawns.Convert(farm, oldFarm);
            //forageSpawns.Convert(farm, oldFarm);

            farm.SpawnMonstersAtNight = oldFarm.spawnMonstersAtNight;

            farm.FurnitureList = oldFarm.furnitureList;
            farm.FurnitureLayoutFromCanon = oldFarm.furnitureLayoutFromCanon;
            farm.ObjectList = oldFarm.objectList;
        }
    }
}
