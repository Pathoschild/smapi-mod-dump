using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using MTN.FarmInfo;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MTN {
    /// <summary>
    /// The full version of a custom farm. This class is meant to just retain information pertaining to
    /// a particular custom map that WAS selected and IS loaded. Non-Loaded entries will remain as CustomFarmEntry
    /// 
    /// Populated by JsonSerializer
    /// </summary>
    public class CustomFarmType : CustomFarmEntry {
        //Farm Map
        public string farmMapFile { get; set; }
        public fileType farmMapType { get; set; } = fileType.xnb;
        public List<additionalTileSheet> additionalTileSheets { get; set; }

        //Additional Resources
        public List<additionalMap<GameLocation>> additionalMaps { get; set; }

        //Structure Relocations
        public StructureInfo farmHouse { get; set; }
        public StructureInfo greenHouse { get; set; }
        public StructureInfo farmCave { get; set; }
        public StructureInfo shippingBin { get; set; }
        public StructureInfo mailBox { get; set; }
        public StructureInfo grandpaShrine { get; set; }
        public StructureInfo rabbitStatue { get; set; }
        public StructureInfo petWaterBowl { get; set; }
        public List<neighboringMap> neighboringMaps { get; set; }

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

        public void checkSpawnIntegrity() {
            for (int i = 0; i < Resource.Count; i++) {
                Resource[i].loadItem();
                if (!Resource[i].isValid) {
                    Memory.instance.Monitor.Log("Resource Spawn invalid, removing.");
                    Resource.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < Forage.Count; i++) {
                Forage[i].loadItem();
                if (!Forage[i].isValid) {
                    Memory.instance.Monitor.Log("Forage Spawn invalid, removing.");
                    Forage.RemoveAt(i);
                    i--;
                }
            }
        }

        //Methods - Update Routines
        public void executeResourceSpawns() {
            for (int i = 0; i < Resource.Count; i++) {
                Resource[i].executeSpawn(10);
            }
        }

        public void executeForageSpawns() {
            for (int i = 0; i < Forage.Count; i++) {
                Forage[i].executeSpawn(10);
            }
        }

        //Methods - Reminder: Invest in Properties.
        public int farmHousePorchX() {
            return farmHouse.pointOfInteraction.x;
        }

        public int farmHousePorchY() {
            return farmHouse.pointOfInteraction.y;
        }

        public Point getFarmHousePorch() {
            return new Point(farmHousePorchX(), farmHousePorchY());
        }

        public Vector2 getFarmHouseRenderPosition(float offsetX = 0, float offsetY = 0) {
            return new Vector2((farmHouse.coordinates.x * 64f) + offsetX, (farmHouse.coordinates.y * 64f) + offsetY);
        }

        public int greenHousePorchX() {
            return greenHouse.pointOfInteraction.x;
        }

        public int greenHousePorchY() {
            return greenHouse.pointOfInteraction.y;
        }

        public Vector2 getGreenHouseRenderPosition(float offsetX = 0, float offsetY = 0) {
            return new Vector2((greenHouse.coordinates.x * 64f) + offsetX, (greenHouse.coordinates.y * 64f) + offsetY);
        }

        public int mailBoxPointY() {
            return mailBox.pointOfInteraction.y;
        }

        public Vector2 getMailBoxNotificationRenderPosition(float offsetX = 0, float offsetY = 0) {
            return new Vector2((mailBox.pointOfInteraction.x * 64f) + offsetX, (mailBox.pointOfInteraction.y * 64f) + offsetY);
        }

        public Vector2 getgrandpaShrineRenderPosition(float offsetX = 0, float offsetY = 0) {
            return new Vector2((grandpaShrine.pointOfInteraction.x * 64f) + offsetX, (mailBox.pointOfInteraction.y * 64f) + offsetY);
        }

        public int farmCavePointX() {
            return farmCave.pointOfInteraction.x;
        }

        public int farmCavePointY() {
            return farmCave.pointOfInteraction.y;
        }
    }
}
