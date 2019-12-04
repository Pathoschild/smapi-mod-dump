using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MTN2.Compatibility;
using MTN2.MapData;
using MTN2.Utilities;
using StardewValley;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using SObject = StardewValley.Object;

namespace MTN2.Management {
    internal class FarmManagement {
        //////////////
        /// Fields ///
        //////////////

        private const int NumberOfCanonFarms = 6;
        protected int LoadedIndex = -1;
        protected int SelectedIndex = -1;

        //////////////////
        /// Properties ///
        //////////////////

        public List<CustomFarm> FarmList { get; private set; }

        /// <summary>
        /// Gets the custom farm that the player current has selected
        /// for a new game. Used primarily when the player is creating
        /// said new game.
        /// </summary>
        public CustomFarm SelectedFarm {
            get {
                if (SelectedIndex == -1) return null;
                return FarmList[SelectedIndex];
            }
        }

        /// <summary>Gets the custom farm that is currently loaded/being played.</summary>
        public CustomFarm LoadedFarm {
            get {
                if (LoadedIndex == -1) return null;
                if (FarmList.Count == 0) return null;
                return FarmList[LoadedIndex];
            }
        }

        /// <summary>
        /// Gets the coordinates where the player can interact with the 
        /// starting shipping bin.
        /// </summary>
        public Interaction ShippingBin {
            get {
                return LoadedFarm.ShippingBin.PointOfInteraction;
            }
        }

        /// <summary></summary>
        public Interaction RabbitShrine {
            get {
                return LoadedFarm.RabbitShrine.PointOfInteraction;
            }
        }

        /// <summary></summary>
        public Interaction PetWaterBowl {
            get {
                return LoadedFarm.PetWaterBowl.PointOfInteraction;
            }
        }

        /// <summary></summary>
        public Point FarmCaveOpening {
            get {
                return new Point(LoadedFarm.FarmCave.PointOfInteraction.X, LoadedFarm.FarmCave.PointOfInteraction.Y);
            }
        }

        ////////////////////
        ////////////////////
        /// Constructors ///
        ////////////////////
        ////////////////////
        
        /// <summary>
        /// 
        /// </summary>
        public FarmManagement() {
            Clean();
        }

        ///////////////
        ///////////////
        /// Methods ///
        ///////////////
        ///////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentPack"></param>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public void Populate(IModHelper helper, IContentPack contentPack, IMonitor monitor) {
            CustomFarm FarmData = new CustomFarm();

            if (ProcessContentPack(contentPack, out FarmData)) {
                monitor.Log($"\t + Contains a custom farm.", LogLevel.Trace);
                if (FarmData.Version < 2.1f) {
                    FarmData = BackwardsCompatibility(contentPack, monitor, FarmData.Version);
                }
                LoadIcon(helper, contentPack, FarmData);
                Validate(FarmData);
                FarmData.ContentPack = contentPack;
                FarmList.Add(FarmData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentPack"></param>
        /// <param name="farm"></param>
        /// <returns></returns>
        private bool ProcessContentPack(IContentPack contentPack, out CustomFarm farm) {
            Dictionary<string, object> Extra;
            bool results;

            if (contentPack.Manifest.ExtraFields != null && contentPack.Manifest.ExtraFields.ContainsKey("ContentPackType")) {
                Extra = (Dictionary<string, object>)ObjectToDictionaryHelper.ToDictionary(contentPack.Manifest.ExtraFields["ContentPackType"]);
                if (Extra.ContainsKey("Farm") && bool.Parse(Extra["Farm"].ToString())) {
                    farm = contentPack.ReadJsonFile<CustomFarm>("farmType.json");
                    results = true;
                } else {
                    farm = null;
                    results = false;
                }
            } else {
                farm = contentPack.ReadJsonFile<CustomFarm>("farmType.json");
                results = true;
            }
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="contentPack"></param>
        /// <param name="farm"></param>
        private void LoadIcon(IModHelper helper, IContentPack contentPack, CustomFarm farm) {
            string IconFile;
            IconFile = Path.Combine(contentPack.DirectoryPath, "icon.png");
            if (File.Exists(IconFile)) {
                farm.IconSource = contentPack.LoadAsset<Texture2D>("icon.png");
            } else {
                farm.IconSource = helper.Content.Load<Texture2D>(Path.Combine("Resource", "missingIcon.png"));
            }
        }

        /// <summary>
        /// Converts a content pack with an older farmType.json to the latest.
        /// </summary>
        /// <param name="contentPack">A SMAPI Content Pack</param>
        /// <param name="monitor">SMAPI's IMonitor, to print useful information.</param>
        /// <param name="version">The version value of the Content Pack</param>
        /// <returns>Converted CustomFarm</returns>
        public CustomFarm BackwardsCompatibility(IContentPack contentPack, IMonitor monitor, float version) {
            switch (version) {
                case 2.0f:
                    return PopulateVerison2dot0(contentPack, monitor);
                default:
                    return PopulateVersion1dot0(contentPack, monitor);
            }
        }

        /// <summary>
        /// Converts a content pack with a farmType.json 1.0 (MTN1) to the latest.
        /// </summary>
        /// <param name="contentPack">A SMAPI Content Pack</param>
        /// <param name="monitor">SMAPI's IMonitor, to print useful information.</param>
        /// <returns>Converted CustomFarm</returns>
        private CustomFarm PopulateVersion1dot0(IContentPack contentPack, IMonitor monitor) {
            CustomFarmVer1 oldVersion;
            CustomFarm convertedFarm;

            monitor.Log("\t - Content Pack is for FarmType 1.0 (MTN1). Using Backwards Compatibility.");
            oldVersion = contentPack.ReadJsonFile<CustomFarmVer1>("farmType.json");
            convertedFarm = new CustomFarm();
            CustomFarmVer1.Convert(convertedFarm, oldVersion);
            return convertedFarm;
        }

        /// <summary>
        /// Converts a content pack with a farmType.json 2.0 to the latest.
        /// </summary>
        /// <param name="contentPack">A SMAPI Content Pack</param>
        /// <param name="monitor">SMAPI's IMonitor, to print useful information.</param>
        /// <returns>Converted CustomFarm</returns>
        private CustomFarm PopulateVerison2dot0(IContentPack contentPack, IMonitor monitor) {
            CustomFarmVer2p0 oldVersion;
            CustomFarm convertedFarm;

            monitor.Log("\t - Content Pack is using FarmType 2.0. Using Backwards Compatibility.");
            oldVersion = contentPack.ReadJsonFile<CustomFarmVer2p0>("farmType.json");
            convertedFarm = new CustomFarm();
            CustomFarmVer2p0.Convert(convertedFarm, oldVersion);
            return convertedFarm;
        }

        /// <summary>
        /// Checks each field containing a <see cref="Structure"/> type. Implements the default (canon) values if
        /// the field is omitted.
        /// </summary>
        /// <param name="farm">A MTN Custom Farm</param>
        public void Validate(CustomFarm farm) {
            farm.FarmHouse = farm.FarmHouse ?? new Structure(new Placement(3712f / 64f, 520f / 64f), new Interaction(64, 15));
            farm.GreenHouse = farm.GreenHouse ?? new Structure(new Placement(1600f / 64f, 384f / 64f), new Interaction(28, 17));
            farm.FarmCave = farm.FarmCave ?? new Structure(new Placement(), new Interaction(34, 7));
            farm.ShippingBin = farm.ShippingBin ?? new Structure(new Placement(), new Interaction(71, 13));
            farm.MailBox = farm.MailBox ?? new Structure(new Placement(), new Interaction(68, 16));
            farm.GrandpaShrine = farm.GrandpaShrine ?? new Structure(new Placement(), new Interaction(9, 7));
            farm.RabbitShrine = farm.RabbitShrine ?? new Structure(new Placement(), new Interaction(48, 7));
            farm.PetWaterBowl = farm.PetWaterBowl ?? new Structure(new Placement(), new Interaction(54, 7));
        }

        /// <summary>
        /// Updates the selected farm. Used during the creation of a new game.
        /// </summary>
        /// <param name="farmName"></param>
        /// <returns><c>True</c> if the selected farm was canon. <c>False</c> otherwise.</returns>
        public bool Update(string farmName) {
            if (!farmName.StartsWith("MTN_")) {
                SelectedIndex = -1;
                return true;
            }

            farmName = farmName.Substring(4);
            for (int i = 0; i < FarmList.Count; i++) {
                if (FarmList[i].Name == farmName) {
                    SelectedIndex = i;
                    return false;
                }
            }

            //Throw error. Farm was not found.
            SelectedIndex = -1;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Load() {
            LoadedIndex = SelectedIndex;
            return (SelectedIndex == -1) ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whichFarm"></param>
        /// <returns></returns>
        public bool Load(int whichFarm) {
            if (whichFarm < NumberOfCanonFarms) return true;

            for (int i = 0; i < FarmList.Count; i++) {
                if (FarmList[i].ID == whichFarm) {
                    LoadedIndex = i;
                    break;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clean() {
            FarmList = new List<CustomFarm>();
            Reset();
        }

        public void Reset() {
            LoadedIndex = -1;
            SelectedIndex = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public string GetAssetKey(out Map map) {
            if (!(LoadedFarm.FarmMap.FileType == FileType.xnb)) {
                map = LoadedFarm.ContentPack.LoadAsset<Map>(LoadedFarm.FarmMap.FileName + ".tbin");
            } else {
                map = null;
            }
            return LoadedFarm.ContentPack.GetActualAssetKey(LoadedFarm.FarmMap.FileName + ((!(LoadedFarm.FarmMap.FileType == FileType.xnb)) ? ".tbin" : ".xnb"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Canon"></param>
        /// <returns></returns>
        public int CabinLimit(bool Canon) {
            return (Canon) ? 3 : FarmList[SelectedIndex].CabinCapacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Canon"></param>
        /// <returns></returns>
        public Vector2 GrandpaShrineCoords(bool Canon) {
            if (Canon || LoadedFarm.GrandpaShrine == null) {
                return new Vector2(576f, 448f);
            }
            Interaction POI = LoadedFarm.GrandpaShrine.PointOfInteraction;
            return new Vector2(POI.X * 64f, POI.Y * 64f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="Option"></param>
        /// <param name="Canon"></param>
        /// <returns></returns>
        public Vector2 MailboxNotification(float xOffset, float yOffset, bool Option, bool Canon) {
            Point mailbox_position = Game1.player.getMailboxPosition();
            return new Vector2(mailbox_position.X * 64f + xOffset, (mailbox_position.Y * 64 - 96 - 48) + yOffset);

            //if (Canon || LoadedFarm.MailBox == null) {
            //    return new Vector2((Option) ? 4388f : 4352f, ((Option) ? 928f : 880f) + yOffset);
            //}
            //Interaction POI = LoadedFarm.MailBox.PointOfInteraction;
            //return new Vector2((POI.X * 64f) + xOffset, (POI.Y * 64f) + yOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Option"></param>
        /// <param name="Canon"></param>
        /// <returns></returns>
        public float MailBoxNotifyLayerDepth(bool Option, bool Canon) {
            Point mailbox_position = Game1.player.getMailboxPosition();
            float draw_layer = ((mailbox_position.X + 1) * 64) / 10000f + (mailbox_position.Y * 64) / 10000f;
            return draw_layer + ((Option) ? 1E-05f : 1E-06f);

            //if (Canon) {
            //    return (Option) ? 0.11561f : 0.115601f;
            //} else {
            //    return (((LoadedFarm.MailBox.PointOfInteraction.Y + 2) * 64f) / 10000f) + ((Option) ? 0.00041f : 0.000401f);
            //}
        }

        public SObject MethodHook_GetFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency) {
            if (LoadedFarm.FishingSpawnsFromCanon != -1) return null;
                
            
            return null;
        }
    }
}
