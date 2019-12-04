using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MTN2.Compatibility;
using MTN2.Management;
using MTN2.MapData;
using StardewModdingAPI;
using StardewValley.Locations;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace MTN2.Management
{
    /// <summary>
    /// CustomManager class. Performs all the nessecary operations needed to allow a custom maps (custom farms, greenhouse, etc)
    /// to properly function. It is also built to distinguish between canon, or custom. Contains a List<T>
    /// of custom classes and manipulates / gathers the data accordingly.
    /// </summary>
    internal class CustomManager : ICustomManager {
        public IContentPack LoadedPack { get; private set; }
        private FarmManagement FarmManager { get; set; }
        private GHouseManagement GreenhouseManager { get; set; }
        private FHouseManagement HouseManager { get; set; }
        
        public bool NoDebris { get; set; } = false;
        public bool Canon { get; private set; } = true;
        public int ScienceHouseIndex { get; private set; }

        public int CabinLimit { get { return FarmManager.CabinLimit(Canon); } }
        public List<CustomFarm> FarmList { get { return FarmManager.FarmList; } }
        public List<CustomGreenHouse> GreenHouseList { get { return GreenhouseManager.GreenHouseList; } }
        public CustomFarm SelectedFarm { get { return FarmManager.SelectedFarm; } }
        public CustomFarm LoadedFarm { get { return FarmManager.LoadedFarm; } }
        public Interaction ShippingBin { get { return FarmManager.ShippingBin; } }
        public Interaction RabbitShrine { get { return FarmManager.RabbitShrine; } }
        public Interaction PetWaterBowl { get { return FarmManager.PetWaterBowl; } }
        public Point FarmHousePorch { get { return HouseManager.FrontPorch; } }
        public Point GreenHouseDoor { get { return GreenhouseManager.GreenHouseDoor; } }
        public Point FarmCaveOpening { get { return FarmManager.FarmCaveOpening; } }
        public int FurnitureLayout { get { return HouseManager.FurnitureLayout; } }
        public int GreenHouseEntryX { get { return GreenhouseManager.GreenHouseEntryX(Canon); } }
        public int GreenHouseEntryY { get { return GreenhouseManager.GreenHouseEntryY(Canon); } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CustomManager() {
            FarmManager = new FarmManagement();
            GreenhouseManager = new GHouseManagement(FarmManager);
            HouseManager = new FHouseManagement(FarmManager);
        }

        /// <summary>
        /// Populates the List<CustomFarm> FarmList variable with all the Content Packs
        /// registered to MTN.
        /// </summary>
        /// <param name="helper">SMAPI's IModHelper, to load in the Content Packs</param>
        /// <param name="monitor">SMAPI's IMonitor, to print useful information</param>
        public void Populate(IModHelper helper, IMonitor monitor) {

            foreach (IContentPack contentPack in helper.ContentPacks.GetOwned()) {
                monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}.");
                FarmManager.Populate(helper, contentPack, monitor);
                GreenhouseManager.Populate(contentPack, monitor);
            }

            return;
        }

        /// <summary>
        /// Updates the selected farm. Used during the creation of a new game.
        /// </summary>
        /// <param name="farmName"></param>
        public void UpdateSelectedFarm(string farmName) {
            Canon = FarmManager.Update(farmName);
            return;
        }

        /// <summary>
        /// Set the Selected Farm as the Loaded Farm. Used when creating a new game,
        /// as a confirmation.
        /// </summary>
        public void LoadCustomFarm() {
            Canon = FarmManager.Load();
            //if (!Canon) GreenhouseManager.LinkToFarm(FarmManager.LoadedFarm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whichFarm"></param>
        public void LoadCustomFarm(int whichFarm) {
            Canon = FarmManager.Load(whichFarm);
            //if (!Canon) GreenhouseManager.LinkToFarm(FarmManager.LoadedFarm);
        }
        
        /// <summary>
        /// Gets the Asset Key of the Base Farm Map. Only does work on the base
        /// farm map of any custom map.
        /// </summary>
        /// <param name="map">The base farm map, loaded.</param>
        /// <returns>The Actual Asset Key</returns>
        public string GetAssetKey(out Map map, string type) {
            switch(type) {
                case "Farm":
                    return FarmManager.GetAssetKey(out map);
                case "Greenhouse":
                    return GreenhouseManager.GetAssetKey(out map);
                default:
                    map = null;
                    return null;
            }
        }

        /// <summary>
        /// Gets the Asset Key of additional maps. Only does work on additional maps,
        /// and not the base farm map of any custom map.
        /// </summary>
        /// <param name="fileName">The filename of the map</param>
        /// <param name="fileType">The file type (xnb or tbin)</param>
        /// <returns>The Actual Asset Key</returns>
        public string GetAssetKey(string fileName, FileType fileType) {
            return LoadedFarm.ContentPack.GetActualAssetKey(fileName + ((fileType == FileType.raw) ? ".tbin" : ".xnb"));
        }

        /// <summary>
        /// Loads a map, based on the given filename. Only does work on additional maps.
        /// Should not be used for base farm map.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The custom map, loaded into memory.</returns>
        public Map LoadMap(string fileName) {
            return LoadedFarm.ContentPack.LoadAsset<Map>(fileName);
        }

        /// <summary>
        /// Gets the X and Y coordinates of the top left point of the Farmhouse in Vector2
        /// Form. Allows offsetting the coordinates. 
        /// </summary>
        /// <param name="OffsetX">The Offset value for the X Coordinate</param>
        /// <param name="OffsetY">The Offset value for the Y Coordinate</param>
        /// <returns>The coordinates in Vector2 form.</returns>
        public Vector2 FarmHouseCoords(float OffsetX = 0, float OffsetY = 0) {
            return HouseManager.FarmHouseCoords(OffsetX, OffsetY, Canon);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="Option"></param>
        /// <returns></returns>
        public Vector2 MailboxNotification(float xOffset, float yOffset, bool Option) {
            return FarmManager.MailboxNotification(xOffset, yOffset, Option, Canon);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Option"></param>
        /// <returns></returns>
        public float MailBoxNotifyLayerDepth(bool Option) {
            return FarmManager.MailBoxNotifyLayerDepth(Option, Canon);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SetScienceIndex(int index) {
            ScienceHouseIndex = index;
        }

        public void Reset() {
            Canon = true;
            FarmManager.Reset();
        }

        public float FarmHouseLayerDepth() {
            return HouseManager.FarmHouseLayerDepth(Canon);
        }

        public Vector2 GreenHouseCoords() {
            return GreenhouseManager.GreenHouseCoords(Canon);
        }

        public float GreenHouseLayerDepth() {
            return GreenhouseManager.GreenHouseLayerDepth(Canon);
        }

        public Vector2 GrandpaShrineCoords() {
            return FarmManager.GrandpaShrineCoords(Canon);
        }
    }
}
