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
using ItemBags.Menus;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static ItemBags.Bags.ItemBag;
using static ItemBags.Persistence.BagSizeConfig;

namespace ItemBags.Persistence
{
    [XmlRoot(ElementName = "Config", Namespace = "")]
    public class UserConfig
    {
        /// <summary>This property is only public for serialization purposes. Use <see cref="CreatedByVersion"/> instead.</summary>
        [XmlElement("CreatedByVersion")]
        public string CreatedByVersionString { get; set; }
        /// <summary>Warning - in old versions of the mod, this value may be null. This feature was added with v1.0.4</summary>
        [JsonIgnore]
        [XmlIgnore]
        public Version CreatedByVersion
        {
            get { return string.IsNullOrEmpty(CreatedByVersionString) ? null : Version.Parse(CreatedByVersionString); }
            set { CreatedByVersionString = value == null ? null : value.ToString(); }
        }

        [XmlElement("GlobalPriceModifier")]
        public double GlobalPriceModifier { get; set; }
        [XmlElement("GlobalCapacityModifier")]
        public double GlobalCapacityModifier { get; set; }

        [XmlArray("StandardBagSettings")]
        [XmlArrayItem("StandardBagSizeConfig")]
        public StandardBagSizeConfig[] StandardBagSettings { get; set; }
        [XmlArray("BundleBagSettings")]
        [XmlArrayItem("BundleBagSizeConfig")]
        public BundleBagSizeConfig[] BundleBagSettings { get; set; }

        [XmlArray("RucksackSettings")]
        [XmlArrayItem("RucksackSizeConfig")]
        public RucksackSizeConfig[] RucksackSettings { get; set; }
        [XmlArray("OmniBagSettings")]
        [XmlArrayItem("OmniBagSizeConfig")]
        public OmniBagSizeConfig[] OmniBagSettings { get; set; }

        [XmlElement("HideSmallBagsFromShops")]
        public bool HideSmallBagsFromShops { get; set; }
        [XmlElement("HideMediumBagsFromShops")]
        public bool HideMediumBagsFromShops { get; set; }
        [XmlElement("HideLargeBagsFromShops")]
        public bool HideLargeBagsFromShops { get; set; }
        [XmlElement("HideGiantBagsFromShops")]
        public bool HideGiantBagsFromShops { get; set; }
        [XmlElement("HideMassiveBagsFromShops")]
        public bool HideMassiveBagsFromShops { get; set; }

        [XmlElement("HideObsoleteBagsFromShops")]
        public bool HideObsoleteBagsFromShops { get; set; }

        [XmlElement("MonsterLootSettings")]
        public MonsterLootSettings MonsterLootSettings { get; set; }

        [XmlElement("AllowAutofillInsideChest")]
        public bool AllowAutofillInsideChest { get; set; }

        [XmlElement("GamepadSettings")]
        public GamepadControls GamepadSettings { get; set; }

        [XmlElement("ShowAutofillMessage")]
        public bool ShowAutofillMessage { get; set; }

        public UserConfig()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            this.GlobalPriceModifier = 1.0;
            this.GlobalCapacityModifier = 1.0;

            this.StandardBagSettings = new StandardBagSizeConfig[]
            {
                new StandardBagSizeConfig(ContainerSize.Small, 1.0, 1.0),
                new StandardBagSizeConfig(ContainerSize.Medium, 1.0, 1.0),
                new StandardBagSizeConfig(ContainerSize.Large, 1.0, 1.0),
                new StandardBagSizeConfig(ContainerSize.Giant, 1.0, 1.0),
                new StandardBagSizeConfig(ContainerSize.Massive, 1.0, 1.0)
            };

            this.BundleBagSettings = new BundleBagSizeConfig[]
            {
                new BundleBagSizeConfig(ContainerSize.Large, 1.0, 2500),
                new BundleBagSizeConfig(ContainerSize.Massive, 1.0, 10000)
            };

            this.RucksackSettings = new RucksackSizeConfig[]
            {
                new RucksackSizeConfig(ContainerSize.Small, 1.0, 1.0, 10000, 30, 6, 12, BagInventoryMenu.DefaultInventoryIconSize),
                new RucksackSizeConfig(ContainerSize.Medium, 1.0, 1.0, 30000, 99, 12, 12, BagInventoryMenu.DefaultInventoryIconSize),
                new RucksackSizeConfig(ContainerSize.Large, 1.0, 1.0, 90000, 300, 24, 12, BagInventoryMenu.DefaultInventoryIconSize),
                new RucksackSizeConfig(ContainerSize.Giant, 1.0, 1.0, 200000, 999, 36, 12, BagInventoryMenu.DefaultInventoryIconSize),
                new RucksackSizeConfig(ContainerSize.Massive, 1.0, 1.0, 500000, 9999, 72, 12, BagInventoryMenu.DefaultInventoryIconSize)
            };

            this.OmniBagSettings = new OmniBagSizeConfig[]
            {
                new OmniBagSizeConfig(ContainerSize.Small, 1.0, 5000, 8, BagInventoryMenu.DefaultInventoryIconSize),
                new OmniBagSizeConfig(ContainerSize.Medium, 1.0, 20000, 8, BagInventoryMenu.DefaultInventoryIconSize),
                new OmniBagSizeConfig(ContainerSize.Large, 1.0, 50000, 8, BagInventoryMenu.DefaultInventoryIconSize),
                new OmniBagSizeConfig(ContainerSize.Giant, 1.0, 250000, 8, BagInventoryMenu.DefaultInventoryIconSize),
                new OmniBagSizeConfig(ContainerSize.Massive, 1.0, 1000000, 8, BagInventoryMenu.DefaultInventoryIconSize)
            };

            this.HideSmallBagsFromShops = false;
            this.HideMediumBagsFromShops = false;
            this.HideLargeBagsFromShops = false;
            this.HideGiantBagsFromShops = false;
            this.HideMassiveBagsFromShops = false;

            this.HideObsoleteBagsFromShops = true;

            this.MonsterLootSettings = new MonsterLootSettings();

            this.AllowAutofillInsideChest = true;

            this.GamepadSettings = new GamepadControls();

            this.ShowAutofillMessage = true;
        }

        public bool AllowDowngradeBundleItemQuality(ContainerSize Size)
        {
            BundleBagSizeConfig SizeCfg = BundleBagSettings.First(x => x.Size == Size);
            return SizeCfg.AllowDowngradeItemQuality;
        }

        public bool IsSizeVisibleInShops(ContainerSize Size) => Size switch
        {
            ContainerSize.Small => !HideSmallBagsFromShops,
            ContainerSize.Medium => !HideMediumBagsFromShops,
            ContainerSize.Large => !HideLargeBagsFromShops,
            ContainerSize.Giant => !HideGiantBagsFromShops,
            ContainerSize.Massive => !HideMassiveBagsFromShops,
            _ => true
        };

        public int GetStandardBagPrice(ContainerSize Size, BagType Type)
        {
            if (Type == null)
                return 0;

            StandardBagSizeConfig SizeCfg = StandardBagSettings.First(x => x.Size == Size);
            int BasePrice = Type.SizeSettings.First(x => x.Size == Size).Price;
            double Multiplier = GlobalPriceModifier * SizeCfg.PriceModifier;
            if (Multiplier == 1.0)
                return BasePrice;
            else
                return RoundIntegerToSecondMostSignificantDigit((int)(BasePrice * Multiplier), RoundingMode.Floor);
        }

        public int GetStandardBagCapacity(ContainerSize Size, BagType Type)
        {
            if (Type == null)
                return 0;

            StandardBagSizeConfig SizeCfg = StandardBagSettings.First(x => x.Size == Size);
            return SizeCfg.GetCapacity(Type, GlobalCapacityModifier);
        }

        public int GetBundleBagPrice(ContainerSize Size)
        {
            BundleBagSizeConfig SizeCfg = BundleBagSettings.First(x => x.Size == Size);
            int BasePrice = SizeCfg.BasePrice;
            double Multiplier = GlobalPriceModifier * SizeCfg.PriceModifier;
            if (Multiplier == 1.0)
                return BasePrice;
            else
                return RoundIntegerToSecondMostSignificantDigit((int)(BasePrice * Multiplier), RoundingMode.Round);
        }

        public int GetRucksackPrice(ContainerSize Size)
        {
            RucksackSizeConfig SizeCfg = RucksackSettings.First(x => x.Size == Size);
            int BasePrice = SizeCfg.BasePrice;
            double Multiplier = GlobalPriceModifier * SizeCfg.PriceModifier;
            if (Multiplier == 1.0)
                return BasePrice;
            else
                return RoundIntegerToSecondMostSignificantDigit((int)(BasePrice * Multiplier), RoundingMode.Round);
        }

        public int GetRucksackCapacity(ContainerSize Size)
        {
            RucksackSizeConfig SizeCfg = RucksackSettings.First(x => x.Size == Size);
            int BaseCapacity = SizeCfg.BaseCapacity;
            double Multiplier = GlobalCapacityModifier * SizeCfg.CapacityModifier;
            if (Multiplier == 1.0)
                return BaseCapacity;
            else
                return Math.Max(1, RoundIntegerToSecondMostSignificantDigit((int)(BaseCapacity * Multiplier), RoundingMode.Round));
        }

        public int GetRucksackSlotCount(ContainerSize Size)
        {
            return RucksackSettings.First(x => x.Size == Size).Slots;
        }

        public void GetRucksackMenuOptions(ContainerSize Size, out int NumColumns, out int SlotSize)
        {
            RucksackSizeConfig SizeCfg = RucksackSettings.First(x => x.Size == Size);
            NumColumns = SizeCfg.MenuColumns;
            SlotSize = SizeCfg.MenuSlotSize;
        }

        public int GetOmniBagPrice(ContainerSize Size)
        {
            OmniBagSizeConfig SizeCfg = OmniBagSettings.First(x => x.Size == Size);
            int BasePrice = SizeCfg.BasePrice;
            double Multiplier = GlobalPriceModifier * SizeCfg.PriceModifier;
            if (Multiplier == 1.0)
                return BasePrice;
            else
                return RoundIntegerToSecondMostSignificantDigit((int)(BasePrice * Multiplier), RoundingMode.Round);
        }

        public void GetOmniBagMenuOptions(ContainerSize Size, out int NumColumns, out int SlotSize)
        {
            OmniBagSizeConfig SizeCfg = OmniBagSettings.First(x => x.Size == Size);
            NumColumns = SizeCfg.MenuColumns;
            SlotSize = SizeCfg.MenuSlotSize;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }

    [XmlRoot(ElementName = "StandardBagSizeConfig", Namespace = "")]
    public class StandardBagSizeConfig
    {
        private static readonly Dictionary<ContainerSize, int> BaseCapacities = new Dictionary<ContainerSize, int>()
        {
            { ContainerSize.Small, 30 },
            { ContainerSize.Medium, 99 },
            { ContainerSize.Large, 300 },
            { ContainerSize.Giant, 999 },
            { ContainerSize.Massive, 9999 },
        };

        [XmlElement("Size")]
        public ContainerSize Size { get; set; }
        [XmlElement("PriceModifier")]
        public double PriceModifier { get; set; }
        [XmlElement("CapacityModifier")]
        public double CapacityModifier { get; set; }

        public StandardBagSizeConfig()
        {
            InitializeDefaults();
        }

        public StandardBagSizeConfig(ContainerSize Size, double PriceModifier, double CapacityModifier)
        {
            InitializeDefaults();
            this.Size = Size;
            this.PriceModifier = PriceModifier;
            this.CapacityModifier = CapacityModifier;
        }

        private void InitializeDefaults()
        {
            this.Size = ContainerSize.Small;
            this.PriceModifier = 1.0;
            this.CapacityModifier = 1.0;
        }

        public int GetCapacity(BagType Type, double GlobalCapacityModifier)
        {
            int BaseCapacity = BaseCapacities[Size];
            double Multiplier = GlobalCapacityModifier * Type.SizeSettings.First(x => x.Size == Size).CapacityMultiplier;
            if (Multiplier == 1.0)
                return BaseCapacity;
            else
                return Math.Max(1, RoundIntegerToSecondMostSignificantDigit((int)(BaseCapacity * Multiplier), RoundingMode.Round));
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }

    [XmlRoot(ElementName = "BundleBagSizeConfig", Namespace = "")]
    public class BundleBagSizeConfig
    {
        [XmlElement("Size")]
        public ContainerSize Size { get; set; }

        /// <summary>If true, then placing items inside the BundleBag will allow  downgrading the placed item's <see cref="StardewValley.Object.Quality"/> to the highest quality still needed of that item for an incomplete bundle.<para/>
        /// For example, suppose you picked up a Gold-quality Parsnip. Gold parsnips are needed for the Quality crops bundle and Regular-quality are needed for Spring crops bundle.<para/>
        /// If Quality crops is already complete, then the picked-up Parsnip will be downgraded to regular quality, to fulfill the Spring crops bundle instead.</summary>
        [XmlElement("AllowDowngradeItemQuality")]
        public bool AllowDowngradeItemQuality { get; set; } = true;

        [XmlElement("PriceModifier")]
        public double PriceModifier { get; set; }
        [XmlElement("BasePrice")]
        public int BasePrice { get; set; }

        [XmlArray("Shops")]
        [XmlArrayItem("Shop")]
        public BagShop[] Sellers { get; set; }

        public BundleBagSizeConfig()
        {
            InitializeDefaults();
        }

        public BundleBagSizeConfig(ContainerSize Size, double PriceModifier, int BasePrice)
        {
            InitializeDefaults();
            this.Size = Size;
            this.PriceModifier = PriceModifier;
            this.BasePrice = BasePrice;
        }

        private void InitializeDefaults()
        {
            this.AllowDowngradeItemQuality = true;
            this.Size = BundleBag.ValidSizes.First();
            this.Sellers = new BagShop[] { BagShop.TravellingCart };
            this.PriceModifier = 1.0;
            this.BasePrice = 0;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }

    [XmlRoot(ElementName = "RucksackSizeConfig", Namespace = "")]
    public class RucksackSizeConfig
    {
        [XmlElement("Size")]
        public ContainerSize Size { get; set; }

        [XmlElement("PriceModifier")]
        public double PriceModifier { get; set; }
        [XmlElement("CapacityModifier")]
        public double CapacityModifier { get; set; }
        [XmlElement("BasePrice")]
        public int BasePrice { get; set; }
        [XmlElement("BaseCapacity")]
        public int BaseCapacity { get; set; }
        [XmlElement("Slots")]
        public int Slots { get; set; }
        [XmlElement("MenuColumns")]
        public int MenuColumns { get; set; }
        [XmlElement("MenuSlotSize")]
        public int MenuSlotSize { get; set; }

        [XmlArray("Shops")]
        [XmlArrayItem("Shop")]
        public BagShop[] Sellers { get; set; }

        public RucksackSizeConfig()
        {
            InitializeDefaults();
        }

        public RucksackSizeConfig(ContainerSize Size, double PriceModifier, double CapacityModifier, int BasePrice, int BaseCapacity, int Slots, int MenuColumns, int MenuSlotSize)
        {
            InitializeDefaults();
            this.Size = Size;
            this.PriceModifier = PriceModifier;
            this.CapacityModifier = CapacityModifier;
            this.BasePrice = BasePrice;
            this.BaseCapacity = BaseCapacity;
            this.Slots = Slots;
            this.MenuColumns = MenuColumns;
            this.MenuSlotSize = MenuSlotSize;
        }

        private void InitializeDefaults()
        {
            this.Size = ContainerSize.Small;
            this.PriceModifier = 1.0;
            this.CapacityModifier = 1.0;
            this.BasePrice = 0;
            this.BaseCapacity = 1;
            this.Slots = 1;
            this.MenuColumns = 12;
            this.MenuSlotSize = BagInventoryMenu.DefaultInventoryIconSize;
            this.Sellers = new BagShop[] { BagShop.Pierre };
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }

    [XmlRoot(ElementName = "OmniBagSizeConfig", Namespace = "")]
    public class OmniBagSizeConfig
    {
        [XmlElement("Size")]
        public ContainerSize Size { get; set; }

        [XmlElement("PriceModifier")]
        public double PriceModifier { get; set; }
        [XmlElement("BasePrice")]
        public int BasePrice { get; set; }
        [XmlElement("MenuColumns")]
        public int MenuColumns { get; set; }
        [XmlElement("MenuSlotSize")]
        public int MenuSlotSize { get; set; }

        [XmlArray("Shops")]
        [XmlArrayItem("Shop")]
        public BagShop[] Sellers { get; set; }

        public OmniBagSizeConfig()
        {
            InitializeDefaults();
        }

        public OmniBagSizeConfig(ContainerSize Size, double PriceModifier, int BasePrice, int MenuColumns, int MenuSlotSize)
        {
            InitializeDefaults();
            this.Size = Size;
            this.PriceModifier = PriceModifier;
            this.BasePrice = BasePrice;
            this.MenuColumns = MenuColumns;
            this.MenuSlotSize = MenuSlotSize;
        }

        private void InitializeDefaults()
        {
            this.Size = ContainerSize.Small;
            this.PriceModifier = 1.0;
            this.BasePrice = 0;
            this.MenuColumns = 12;
            this.MenuSlotSize = BagInventoryMenu.DefaultInventoryIconSize;
            this.Sellers = new BagShop[] { BagShop.Pierre };
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }

    /// <summary>Settings which affect how likely the player is to receive an ItemBag as a monster drop when killing a monster.</summary>
    [DataContract(Name = "MonsterLootSettings", Namespace = "")]
    public class MonsterLootSettings
    {
        [JsonProperty("LogDropChancesToConsole")]
        public bool LogDropChancesToConsole { get; set; } = false;

        [JsonProperty("CanReceiveBagsAsDrops")]
        public bool CanReceiveBagsAsDrops { get; set; } = true;

        /// <summary>If you've earned an ItemBag drop, and that drop is randomly chosen to be a standard bag (A <see cref="BoundedBag"/>), then this is the chance that it will 
        /// try to force a new type that you don't already own.</summary>
        [JsonProperty("ForceNewBagTypeChance")]
        public double ForceNewBagTypeChance { get; set; } = 0.4;

        //  The settings in this region determine the odds of getting any ItemBag when killing a monster. The actual bag chosen if these rolls succeed is then based on the corresponding BagTypeDropSettings
        #region Initial Chance
        /// <summary>The base chance of receiving an ItemBag when any monster is slain. 0.01 = 1% chance</summary>
        [JsonProperty("BaseDropChance")]
        public double BaseDropChance { get; set; }

        #region Location Modifiers
        /// <summary>An additional bonus to the chance of receiving an ItemBag when a monster is slain within the mines. 0.01 = +1% chance<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="MineDepthBonusPerLevel"/> = 0.01, and player is in Mine level 35, you would have a 0.01 * 35 = +35% (1.35 multiplier) chance to receive drops)</summary>
        [JsonProperty("MineDepthBonusPerLevel")]
        public double MineDepthBonusPerLevel { get; set; }

        /// <summary>Values of <see cref="MineShaft.mineLevel"/> that are greater than this value will no longer give a bonus to the drop chance of receiving an ItemBag from a slain monster.</summary>
        [JsonProperty("MaxMineDepth")]
        public int MaxMineDepth { get; set; }

        /// <summary>An additional bonus to the chance of receiving an ItemBag when a monster is slain within the Quarry, 0.01 = +1% chance<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="QuarryLocationBonus"/> = 0.5, the player is in the Quarry, you would have a 0.5 = +50% (1.5 multiplier) chance to receive drops)</summary>
        [JsonProperty("QuarryLocationBonus")]
        public double QuarryLocationBonus { get; set; }

        /// <summary>An additional bonus to the chance of receiving an ItemBag when a monster is slain within the Forest, 0.01 = +1% chance<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="ForestLocationBonus"/> = 0.5, the player is in the Forest, you would have a 0.5 = +50% (1.5 multiplier) chance to receive drops)</summary>
        [JsonProperty("ForestLocationBonus")]
        public double ForestLocationBonus { get; set; }

        /// <summary>An additional bonus to the chance of receiving an ItemBag when a monster is slain within a location not recognized by other settings (I.E. not in the mines, quarry, or forest), 0.01 = +1% chance<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="OtherLocationBonus"/> = 0.5, the player is NOT in the mines/quarry/forest, you would have a 0.5 = +50% (1.5 multiplier) chance to receive drops)</summary>
        [JsonProperty("OtherLocationBonus")]
        public double OtherLocationBonus { get; set; }
        #endregion Location Modifiers

        #region Experience Modifiers
        /// <summary>The Base amount of <see cref="Monster.ExperienceGained"/> used when calculating additional bonuses or penalties to drop chances.<para/>
        /// If the slain monster awards MORE than <see cref="BaseExperience"/>, you will receive a bonus to the chance of receiving an ItemBag. 
        /// This bonus is affected by <see cref="MaxExperience"/> and <see cref="MaxExperienceMultiplier"/><para/>
        /// If the slain monster awards LESS than <see cref="BaseExperience"/>, you will receive a penalty to the chance of receiving an ItemBag.
        /// This penalty is affected by <see cref="MinExperience"/> and <see cref="MinExperienceMultiplier"/></summary>
        [JsonProperty("BaseExperience")]
        public int BaseExperience { get; set; }

        /// <summary>If a slain monster's <see cref="Monster.ExperienceGained"/> is greater than or equal to <see cref="MaxExperience"/>, then the additional bonus to drop rates is set to <see cref="MaxExperienceMultiplier"/></summary>
        [JsonProperty("MaxExperience")]
        public int MaxExperience { get; set; }

        /// <summary>The bonus multiplier to apply to the chance of receiving ItemBags from slain monsters, when the monster's <see cref="Monster.ExperienceGained"/> is at least <see cref="MaxExperience"/></summary>
        [JsonProperty("MaxExperienceMultiplier")]
        public double MaxExperienceMultiplier { get; set; }

        /// <summary>If a slain monster's <see cref="Monster.ExperienceGained"/> is less than or equal to <see cref="MinExperience"/>, then the additional penalty to drop rates is set to <see cref="MinExperienceMultiplier"/></summary>
        [JsonProperty("MinExperience")]
        public int MinExperience { get; set; }

        /// <summary>The penalty multiplier to apply to the chance of receiving ItemBags from slain monsters, when the monster's <see cref="Monster.ExperienceGained"/> is at most <see cref="MinExperience"/></summary>
        [JsonProperty("MinExperienceMultiplier")]
        public double MinExperienceMultiplier { get; set; }
        #endregion Experience Modifiers

        /// <summary>An additional bonus to the chance of receiving an ItemBag when a monster is slain. This value is multiplied by the slain monster's <see cref="Monster.MaxHealth"/>/10 (0.01 = +1% chance per 10 HP)<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="BonusPer10HP"/> = 0.05, and the slain monster had 80 HP, you would have a 0.05*80/10=0.4 = +40% (1.4 multiplier) chance to receive drops)</summary>
        [JsonProperty("BonusPer10HP")]
        public double BonusPer10HP { get; set; }

        /// <summary>Values of <see cref="Monster.MaxHealth"/> that are greater than this value will no longer give a bonus to the drop chance of receiving an ItemBag from a slain monster.</summary>
        [JsonProperty("MaxHP")]
        public int MaxHP { get; set; }
        #endregion Initial Chance

        [JsonProperty("RucksackDropSettings")]
        public BagTypeDropSettings RucksackDropSettings { get; set; }
        [JsonProperty("OmniBagDropSettings")]
        public BagTypeDropSettings OmniBagDropSettings { get; set; }
        [JsonProperty("BundleBagDropSettings")]
        public BagTypeDropSettings BundleBagDropSettings { get; set; }
        [JsonProperty("StandardBagDropSettings")]
        public BagTypeDropSettings StandardBagDropSettings { get; set; }

        public MonsterLootSettings()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
#if DEBUG
            this.LogDropChancesToConsole = true;
#else
            this.LogDropChancesToConsole = false;
#endif

            this.CanReceiveBagsAsDrops = true;

            this.ForceNewBagTypeChance = 0.35;
            this.BaseDropChance = 0.0066;   // ~1/150 before additional bonuses/penalties are applied
                                            // Killing a strong monster deep within the skull cavern will typically give ~2-2.5x modifier
                                            // While killing a weak monster in the low levels of the mines will typically give a ~0.75-1x modifier

            this.MineDepthBonusPerLevel = 0.005;
            this.MaxMineDepth = 220; // Floor #100 in the Skull Cavern (since the skull cavern starts at Depth=120)
            this.QuarryLocationBonus = 0.4;
            this.ForestLocationBonus = 0.3;
            this.OtherLocationBonus = 0.2;

            this.BaseExperience = 8;
            this.MinExperience = 4;
            this.MinExperienceMultiplier = 0.75;
            this.MaxExperience = 15;
            this.MaxExperienceMultiplier = 1.25;

            this.BonusPer10HP = 0.012;
            this.MaxHP = 250;

            this.RucksackDropSettings = new BagTypeDropSettings()
            {
                TypeWeight = 10,
                SizeWeights = new Dictionary<ContainerSize, int>()
                {
                    { ContainerSize.Small, 42 },
                    { ContainerSize.Medium, 35 },
                    { ContainerSize.Large, 15 },
                    { ContainerSize.Giant, 6 },
                    { ContainerSize.Massive, 2 }
                }
            };

            this.OmniBagDropSettings = new BagTypeDropSettings()
            {
                TypeWeight = 4,
                SizeWeights = new Dictionary<ContainerSize, int>()
                {
                    { ContainerSize.Small, 40 },
                    { ContainerSize.Medium, 30 },
                    { ContainerSize.Large, 10 },
                    { ContainerSize.Giant, 4 },
                    { ContainerSize.Massive, 2 }
                }
            };

            this.BundleBagDropSettings = new BagTypeDropSettings()
            {
                TypeWeight = 26,
                SizeWeights = new Dictionary<ContainerSize, int>()
                {
                    { ContainerSize.Large, 3 },
                    { ContainerSize.Massive, 1 }
                }
            };

            this.StandardBagDropSettings = new BagTypeDropSettings()
            {
                TypeWeight = 60,
                SizeWeights = new Dictionary<ContainerSize, int>()
                {
                    { ContainerSize.Small, 30 },
                    { ContainerSize.Medium, 52 },
                    { ContainerSize.Large, 12 },
                    { ContainerSize.Giant, 4 },
                    { ContainerSize.Massive, 2 }
                }
            };
        }

        public double GetItemBagDropChance(GameLocation Location, Monster SlainMonster, out double BaseChance, out double LocationMultiplier, out double ExpMultiplier, out double HPMultiplier)
        {
            BaseChance = this.BaseDropChance;

            //  Compute bonuses based on where the monster was killed
            double LocationBonus;
            if (SlainMonster.mineMonster.Value && Location is MineShaft Mine)
            {
                bool IsQuarry = Mine.mapImageSource.Value != null && Path.GetFileName(Mine.mapImageSource.Value).Equals("mine_quarryshaft", StringComparison.CurrentCultureIgnoreCase);
                if (IsQuarry)
                    LocationBonus = QuarryLocationBonus;
                else
                {
                    int Depth = Math.Min(MaxMineDepth, Mine.mineLevel);
                    LocationBonus = Depth * MineDepthBonusPerLevel;
                }
            }
            else if (Location is Woods)
            {
                LocationBonus = ForestLocationBonus;
            }
            else
            {
                LocationBonus = OtherLocationBonus;
            }
            LocationMultiplier = 1.0 + LocationBonus;

            //  Compute bonuses based on how strong the monster was (determined by how much experience it gave)
            ExpMultiplier = 1.0;
            double ExponentialDecayRate = 0.032;
            if (SlainMonster.ExperienceGained < BaseExperience)
            {
                if (MinExperience >= BaseExperience)
                    ExpMultiplier = 1.0;
                else
                {
                    int TruncatedExp = Math.Max(MinExperience, SlainMonster.ExperienceGained);
                    double LinearlyInterpolatedExp = (BaseExperience - TruncatedExp * 1.0) / (BaseExperience - MinExperience * 1.0) * 100; // Fit the monster exp to the range 0 to 100
                    ExpMultiplier = (1.0 - MinExperienceMultiplier) * Math.Pow(Math.E, -1 * ExponentialDecayRate * LinearlyInterpolatedExp) + MinExperienceMultiplier; // Standard "exponential decay (increasing form)" model (y = Ce^(-kt)), but offset by MinExperienceMultiplier
                }
            }
            else if (SlainMonster.ExperienceGained > BaseExperience)
            {
                if (MaxExperience <= BaseExperience)
                    ExpMultiplier = 1.0;
                else
                {
                    int TruncatedExp = Math.Min(MaxExperience, SlainMonster.ExperienceGained);
                    double LinearlyInterpolatedExp = 100 - (TruncatedExp - BaseExperience * 1.0) / (MaxExperience - BaseExperience * 1.0) * 100; // Fit the monster exp to the range 0 to 100
                    ExpMultiplier = (MaxExperienceMultiplier - 1.0) * Math.Pow(Math.E, -1 * ExponentialDecayRate * LinearlyInterpolatedExp) + 1.0; // Standard "exponential decay (increasing form)" model (y = Ce^(-kt)), but offset by 1.0
                }
            }

            //  Compute bonuses based on how long it took to kill the monster (determined by the monster's MaxHealth)
            double HealthBonus = BonusPer10HP * (Math.Min(MaxHP, SlainMonster.MaxHealth) / 10.0);
            HPMultiplier = 1.0 + HealthBonus;

            return Math.Max(0.0, Math.Min(1.0, BaseChance * LocationMultiplier * ExpMultiplier * HPMultiplier));
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }

    [DataContract(Name = "BagTypeDropSettings", Namespace = "")]
    public class BagTypeDropSettings
    {
        [JsonProperty("TypeWeight")]
        public int TypeWeight { get; set; } = 1;
        [JsonProperty("SizeWeights")]
        public Dictionary<ContainerSize, int> SizeWeights { get; set; } = new Dictionary<ContainerSize, int>();

        public BagTypeDropSettings()
        {
            InitializeDefaults();
        }

        public BagTypeDropSettings(int TypeWeight, Dictionary<ContainerSize, int> SizeWeights)
        {
            InitializeDefaults();
            this.TypeWeight = TypeWeight;
            this.SizeWeights = SizeWeights;
        }

        private void InitializeDefaults()
        {
            this.TypeWeight = 1;
            this.SizeWeights = new Dictionary<ContainerSize, int>();
        }

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
