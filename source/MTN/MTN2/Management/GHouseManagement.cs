using Microsoft.Xna.Framework;
using MTN2.MapData;
using MTN2.Utilities;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace MTN2.Management {
    internal class GHouseManagement {
        protected int LoadedIndex = -1;

        private readonly FarmManagement farmManagement;
        public List<CustomGreenHouse> GreenHouseList { get; private set; }

        /// <summary></summary>
        private CustomGreenHouse SpecificGreenHouse {
            get {
                return farmManagement.LoadedFarm.CustomGreenhouse;
            }
        }

        /// <summary></summary>
        public Point GreenHouseDoor {
            get {
                return new Point(farmManagement.LoadedFarm.GreenHouse.PointOfInteraction.X, farmManagement.LoadedFarm.GreenHouse.PointOfInteraction.Y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="farmManagement"></param>
        public GHouseManagement(FarmManagement farmManagement) {
            this.farmManagement = farmManagement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentPack"></param>
        /// <param name="monitor"></param>
        public void Populate(IContentPack contentPack, IMonitor monitor) {
            CustomGreenHouse GreenHouseData = new CustomGreenHouse();

            if (ProcessContentPack(contentPack, out GreenHouseData)) {
                monitor.Log($"\t + Contains a custom greenhouse.", LogLevel.Trace);
                //Version control?
                //Validate?
                GreenHouseData.ContentPack = contentPack;
                GreenHouseList.Add(GreenHouseData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customFarm"></param>
        public void LinkToFarm(CustomFarm customFarm) {
            if (customFarm.StartingGreenHouse == null) return;
            for (int i = 0; i < GreenHouseList.Count; i++) {
                if (GreenHouseList[i].Name == customFarm.StartingGreenHouse) {
                    customFarm.CustomGreenhouse = GreenHouseList[i];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentPack"></param>
        /// <param name="greenHouse"></param>
        /// <returns></returns>
        private bool ProcessContentPack(IContentPack contentPack, out CustomGreenHouse greenHouse) {
            Dictionary<string, object> Extra;
            if (contentPack.Manifest.ExtraFields != null && contentPack.Manifest.ExtraFields.ContainsKey("ContentPackType")) {
                Extra = (Dictionary<string, object>)ObjectToDictionaryHelper.ToDictionary(contentPack.Manifest.ExtraFields["ContentPackType"]);
                if (Extra.ContainsKey("Greenhouse") && bool.Parse(Extra["Greenhouse"].ToString())) {
                    greenHouse = contentPack.ReadJsonFile<CustomGreenHouse>("greenHouseType.json");
                    return true;
                }
            }
            greenHouse = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 GreenHouseCoords(bool Canon) {
            if (Canon || farmManagement.LoadedFarm.GreenHouse == null) {
                return (Game1.whichFarm == 5) ? new Vector2(2304f, 1600f) : new Vector2(1600f, 384f);
            }
            Placement? Coordinates = farmManagement.LoadedFarm.GreenHouse.Coordinates;
            return new Vector2(Coordinates.Value.X * 64f, Coordinates.Value.Y * 64f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Canon"></param>
        /// <returns></returns>
        public float GreenHouseLayerDepth(bool Canon) {
            if (Canon) {
                return 0.0704f;
            } else {
                return ((farmManagement.LoadedFarm.GreenHouse.PointOfInteraction.Y - 7 + 2) * 64f) / 10000f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public string GetAssetKey(out Map map) {
            //if (LoadedFarm.CustomGreenhouse.GreenhouseMap.FileType == FileType.raw || LoadedFarm.CustomGreenhouse.GreenhouseMap.FileType == FileType.tbin) {
            if (!(SpecificGreenHouse.GreenhouseMap.FileType == FileType.xnb)) { 
                map = SpecificGreenHouse.ContentPack.LoadAsset<Map>(SpecificGreenHouse.GreenhouseMap.FileName + ".tbin");
            } else {
                map = null;
            }
            return SpecificGreenHouse.ContentPack.GetActualAssetKey(SpecificGreenHouse.GreenhouseMap.FileName + ((SpecificGreenHouse.GreenhouseMap.FileType != FileType.xnb) ? ".tbin" : ".xnb"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Canon"></param>
        /// <returns></returns>
        public int GreenHouseEntryX(bool Canon) {
            if (Canon || SpecificGreenHouse == null) return 10;
            return SpecificGreenHouse.Enterance.PointOfInteraction.X;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Canon"></param>
        /// <returns></returns>
        public int GreenHouseEntryY(bool Canon) {
            if (Canon || SpecificGreenHouse == null) return 23;
            return SpecificGreenHouse.Enterance.PointOfInteraction.Y;
        }
    }
}
