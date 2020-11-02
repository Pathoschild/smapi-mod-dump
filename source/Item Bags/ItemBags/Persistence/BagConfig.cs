/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using ItemBags.Helpers;
using ItemBags.Menus;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ItemBags.Persistence
{
    [XmlRoot(ElementName = "BagConfig", Namespace = "")]
    public class BagConfig
    {
        [XmlArray("BagTypes")]
        [XmlArrayItem("BagType")]
        public List<BagType> BagTypes { get; set; }

        /// <summary>This property is only public for serialization purposes. Use <see cref="CreatedByVersion"/> instead.</summary>
        [XmlElement("CreatedByVersion")]
        public string CreatedByVersionString { get; set; }
        /// <summary>Warning - in old versions of the mod, this value may be null. This feature was added with v1.0.4</summary>
        [JsonIgnore]
        [XmlIgnore]
        public Version CreatedByVersion {
            get { return string.IsNullOrEmpty(CreatedByVersionString) ? null : Version.Parse(CreatedByVersionString); }
            set { CreatedByVersionString = value == null ? null : value.ToString(); }
        }

        [JsonIgnore]
        public Dictionary<string, BagType> IndexedBagTypes { get; private set; }

        public BagConfig()
        {
            InitializeDefaults();
        }

        internal void AfterLoaded()
        {
            //  Index the BagTypes by their guids
            this.IndexedBagTypes = new Dictionary<string, BagType>();
            foreach (BagType BagType in this.BagTypes)
            {
                if (!IndexedBagTypes.ContainsKey(BagType.Id))
                {
                    IndexedBagTypes.Add(BagType.Id, BagType);
                }
                else
                {
                    string Warning = string.Format("Warning - multiple bag types were found with the same BagType.Id. Did you manually edit your {0} json file or add multiple .json files to the 'Modded Bags' folder with the same ModUniqueId values? Only the first type with Id = {1} will be used when loading your bag instances.",
                        ItemBagsMod.BagConfigDataKey, BagType.Id);
                    ItemBagsMod.ModInstance.Monitor.Log(Warning, LogLevel.Warn);
                }
            }
        }

        internal BagType GetDefaultBoundedBagType()
        {
            return BagTypes.First(x => x.Id != Rucksack.RucksackTypeId && x.Id != OmniBag.OmniBagTypeId && x.Id != BundleBag.BundleBagTypeId);
        }

        private void InitializeDefaults()
        {
            this.BagTypes = new List<BagType>()
            {
                BagTypeFactory.GetGemBagType(),
                BagTypeFactory.GetSmithingBagType(),
                BagTypeFactory.GetMineralBagType(),
                BagTypeFactory.GetMiningBagType(),
                BagTypeFactory.GetResourceBagType(),
                BagTypeFactory.GetConstructionBagType(),
                BagTypeFactory.GetTreeBagType(),
                BagTypeFactory.GetAnimalProductBagType(),
                BagTypeFactory.GetRecycleBagType(),
                BagTypeFactory.GetLootBagType(),
                BagTypeFactory.GetForagingBagType(),
                BagTypeFactory.GetArtifactBagType(),
                BagTypeFactory.GetSeedBagType(),
                BagTypeFactory.GetOceanFishBagType(),
                BagTypeFactory.GetRiverFishBagType(),
                BagTypeFactory.GetLakeFishBagType(),
                BagTypeFactory.GetMiscFishBagType(),
                BagTypeFactory.GetFishBagType(),
                BagTypeFactory.GetFarmerBagType(),
                BagTypeFactory.GetFoodBagType(),
                BagTypeFactory.GetCropBagType()
            };

            //this.CreatedByVersion = ItemBagsMod.CurrentVersion;

            AfterLoaded();
        }

        internal bool EnsureBagTypesExist(params BagType[] Types)
        {
            List<BagType> DistinctTypes = Types.GroupBy(x => x.Id).Select(x => x.First()).ToList();
            List<BagType> MissingTypes = DistinctTypes.Where(x => !this.BagTypes.Any(y => x.Id == y.Id)).ToList();
            if (MissingTypes.Any())
            {
                this.BagTypes = new List<BagType>(BagTypes).Union(MissingTypes).ToList();
                return true;
            }
            else
            {
                return false;
            }
        }

        /*public const string SettingsFilename = "BagConfig.xml";
        public static string SettingsFilePath { get { return Path.Combine(ItemBagsMod.ModInstance.Helper.DirectoryPath, SettingsFilename); } }

        public void Serialize(out bool Successful, out Exception SerializationError)
        {
            XMLSerializer.Serialize(this, SettingsFilePath, out Successful, out SerializationError);
        }

        public static BagConfig Deserialize(out bool Successful, out Exception DeserializationError)
        {
            return XMLSerializer.Deserialize<BagConfig>(SettingsFilePath, out Successful, out DeserializationError);
        }*/

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }
}
