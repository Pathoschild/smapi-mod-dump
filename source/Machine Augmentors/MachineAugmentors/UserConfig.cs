/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using MachineAugmentors.Items;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using StardewValley.Monsters;
using StardewValley.Locations;
using System.Collections.Generic;
using System;
using StardewValley;
using System.IO;

namespace MachineAugmentors
{
    [XmlRoot(ElementName = "Config", Namespace = "")]
    public class UserConfig
    {
        /// <summary>This property is only public for serialization purposes. Use <see cref="CreatedByVersion"/> instead.</summary>
        [XmlElement("CreatedByVersion")]
        public string CreatedByVersionString { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public Version CreatedByVersion {
            get { return string.IsNullOrEmpty(CreatedByVersionString) ? null : Version.Parse(CreatedByVersionString); }
            set { CreatedByVersionString = value == null ? null : value.ToString(); }
        }

        /// <summary>Opacity to use when drawing Augmentor icons on top of placed Objects. 0.0 = Not visible, 1.0 = Fully opaque.</summary>
        [XmlElement("IconOpacity")]
        public double IconOpacity { get; set; }

        [XmlElement("GlobalPriceMultiplier")]
        public double GlobalPriceMultiplier { get; set; }

        /// <summary>The average number of days a machine that is augmented with a single DuplicationAugmentor must be producing something before it creates a duplicate.<para/>
        /// EX: If DaysPerDuplicate = 5.0, and you have a machine that is augmented with 1 DuplicationAugmentor, and that machine is being used for 6 hours each day, 
        /// then it'd take roughly 5.0 * (24/6) = 20.0 days to get 1 duplicate machine.</summary>
        [XmlElement("DaysPerStandardDuplicate")]
        public double DaysPerStandardDuplicate { get; set; }
        /// <summary>The average number of days to produce a duplicate of an inputless machine. See also: <see cref="DaysPerStandardDuplicate"/></summary>
        [XmlElement("DaysPerInputlessDuplicate")]
        public double DaysPerInputlessDuplicate { get; set; }

        [XmlArray("AugmentorConfigs")]
        [XmlArrayItem("AugmentorConfig")]
        public AugmentorConfig[] AugmentorConfigs { get; set; }

        [XmlElement("ShopSettings")]
        public TravellingMerchantSettings ShopSettings { get; set; }

        [XmlElement("MonsterLootSettings")]
        public MonsterLootSettings MonsterLootSettings { get; set; }

        private Dictionary<AugmentorType, AugmentorConfig> IndexedConfigs { get; set; }
        public AugmentorConfig GetConfig(AugmentorType Type) { return IndexedConfigs[Type]; }

        public UserConfig()
        {
            InitializeDefaults();
        }

        internal void AfterLoaded()
        {
            //  Index the AugmentorConfigs by their type
            this.IndexedConfigs = new Dictionary<AugmentorType, AugmentorConfig>();
            foreach (AugmentorConfig Config in AugmentorConfigs)
            {
                IndexedConfigs.Add(Config.AugmentorType, Config);
            }
        }

        private void InitializeDefaults()
        {
            this.IconOpacity = 0.75;
            this.GlobalPriceMultiplier = 1.0;

            this.AugmentorConfigs = new AugmentorConfig[]
            {
                new AugmentorConfig()
                {
                    AugmentorType = AugmentorType.Output,
                    BasePrice = 7000,
                    ShopAppearanceWeight = 10,
                    ShopStockMultiplier = 1.75,
                    MaxAttachmentsPerMachine = 100,
                    UseLinearFormula = false,
                    MaxEffectPerStandardMachine = 1.5, // Recommended: 1.0-2.0,
                    MaxEffectPerInputlessMachine = 0.75, // Recommended: 0.5-1.0
                    StandardDecayRate = 0.04, // Recommended: 0.02-0.05,
                    InputlessDecayRate = 0.04 // Recommended: 0.02-0.05
                },
                new AugmentorConfig()
                {
                    AugmentorType = AugmentorType.Speed,
                    BasePrice = 5000,
                    ShopAppearanceWeight = 12,
                    ShopStockMultiplier = 2.0,
                    MaxAttachmentsPerMachine = 100,
                    UseLinearFormula = false,
                    MaxEffectPerStandardMachine = 0.98, // Recommended: 0.90-0.995
                    MaxEffectPerInputlessMachine = 0.9, // Recommended: 0.7-0.95
                    StandardDecayRate = 0.055, // Recommended: 0.03-0.07
                    InputlessDecayRate = 0.042 // Recommended: 0.02-0.06
                },
                new AugmentorConfig()
                {
                    AugmentorType = AugmentorType.Efficiency,
                    BasePrice = 10000,
                    ShopAppearanceWeight = 8,
                    ShopStockMultiplier = 1.4,
                    MaxAttachmentsPerMachine = 100,
                    UseLinearFormula = false,
                    MaxEffectPerStandardMachine = 0.9, // Recommended: 0.75-0.95
                    MaxEffectPerInputlessMachine = 0.9, // Recommended: 0.75-0.95
                    StandardDecayRate = 0.045, // Recommended: 0.03-0.06
                    InputlessDecayRate = 0.045 // Recommended: 0.03-0.06
                },
                new AugmentorConfig()
                {
                    AugmentorType = AugmentorType.Quality,
                    BasePrice = 10000,
                    ShopAppearanceWeight = 8,
                    ShopStockMultiplier = 1.4,
                    MaxAttachmentsPerMachine = 100,
                    UseLinearFormula = false,
                    MaxEffectPerStandardMachine = 0.98, // Recommended: 0.90-0.995
                    MaxEffectPerInputlessMachine = 0.95, // Recommended: 0.75-0.99
                    StandardDecayRate = 0.045, // Recommended: 0.03-0.06
                    InputlessDecayRate = 0.04 // Recommended: 0.03-0.06
                },
                new AugmentorConfig()
                {
                    AugmentorType = AugmentorType.Production,
                    BasePrice = 8000,
                    ShopAppearanceWeight = 12,
                    ShopStockMultiplier = 2.5,
                    MaxAttachmentsPerMachine = 200,
                    UseLinearFormula = true,
                    MaxEffectPerStandardMachine = 100.0,
                    MaxEffectPerInputlessMachine = 25.0,
                    StandardDecayRate = 0.04, // Recommended: 0.03-0.06
                    InputlessDecayRate = 0.04 // Recommended: 0.03-0.06
                },
                new AugmentorConfig()
                {
                    AugmentorType = AugmentorType.Duplication,
                    BasePrice = 17500,
                    ShopAppearanceWeight = 5,
                    ShopStockMultiplier = 1.2,
                    MaxAttachmentsPerMachine = 10,
                    UseLinearFormula = false,
                    MaxEffectPerStandardMachine = 0.98, // Recommended: 0.90-0.99
                    MaxEffectPerInputlessMachine = 0.98, // Recommended: 0.9-0.99
                    StandardDecayRate = 0.35, // Recommended: 0.2-0.35
                    InputlessDecayRate = 0.35 // Recommended: 0.2-0.35
                },
            };

            this.DaysPerStandardDuplicate = 10.0;
            this.DaysPerInputlessDuplicate = 20.0;

            this.ShopSettings = new TravellingMerchantSettings();
            this.MonsterLootSettings = new MonsterLootSettings();

            AfterLoaded();
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

    [XmlRoot(ElementName = "AugmentorConfig", Namespace = "")]
    public class AugmentorConfig
    {
        /// <summary>The AugmentorType that these settings are for.</summary>
        [JsonProperty("AugmentorType")]
        [XmlElement("AugmentorType")]
        public string AugmentorTypeString { get { return AugmentorType.ToString(); } set { AugmentorType = (AugmentorType)Enum.Parse(typeof(AugmentorType), value); } }
        [JsonIgnore]
        [XmlIgnore]
        public AugmentorType AugmentorType { get; set; }

        /// <summary>The default price of this AugmentorType before any price randomization is applied.</summary>
        [XmlElement("BasePrice")]
        public int BasePrice { get; set; }

        /// <summary>Determines the chance that this AugmentorType will appear in the Travelling Merchant's shop on a given day.<para/>
        /// The % odds are given by ShopAppearanceWeight / Sum(All ShopAppearanceWeights). 
        /// Note that TravellingMerchantConfig defines additional settings like NumAugmentorTypesInShop that can also affect the chance.</summary>
        [XmlElement("ShopAppearanceWeight")]
        public int ShopAppearanceWeight { get; set; }

        /// <summary>If this AugmentorType is chosen to appear in the Travelling Merchant's shop, then this is a multiplier that affects how many of the AugmentorType will be available for purchase.</summary>
        [XmlElement("ShopStockMultiplier")]
        public double ShopStockMultiplier { get; set; }

        /// <summary>The maximum number of this AugmentorType that can be attached to a single machine.</summary>
        [XmlElement("MaxAttachmentsPerMachine")]
        public int MaxAttachmentsPerMachine { get; set; }

        /// <summary>
        /// The C constant value that the exponential decay (increasing form) formula will approach when determining the effects of this AugmentorType.<para/>
        /// Most augmentors use this formula: Effect = C * (1 - e ^ (-X * K))<para/>
        /// Where C is a constant that provides an upper bound to the effect, X is the quantity of the augmentor attached to the machine, and K is the rate of decay<para/>
        /// For example, if C = 2.2, then an OutputAugmentor could, at best, provide a +220% (*3.2) bonus to produced item quantities.
        /// </summary>
        [XmlElement("MaxEffectPerStandardMachine")]
        public double MaxEffectPerStandardMachine { get; set; }
        /// <summary>The C constant value that the exponential decay formula will approach for machines that do not require input items. See also: <see cref="MaxEffectPerStandardMachine"/></summary>
        [XmlElement("MaxEffectPerInputlessMachine")]
        public double MaxEffectPerInputlessMachine { get; set; }

        /// <summary>If true, overrides the effect calculations to simply be NumAttached / MaxAttachmentsPerMachine * MaxEffect, instead of using an exponential decay formula.</summary>
        [XmlElement("UseLinearFormula")]
        public bool UseLinearFormula { get; set; } = false;

        /// <summary>
        /// The rate of decay used in the formula that determines the effects of this AugmentorType.<para/>
        /// Most augmentors use this formula: Effect = C * (1 - e ^ (-X * K))<para/>
        /// Where C is a constant that provides an upper bound to the effect, X is the quantity of the augmentor attached to the machine, and K is the rate of decay<para/>
        /// For example, if DecayRate = 0.075, the effects would approach the multiplier C SLOWER than if DecayRate = 0.10. A higher value for DecayRate means you require less Augmentors to reach the maximum multiplier.
        /// </summary>
        [XmlElement("StandardDecayRate")]
        public double StandardDecayRate { get; set; }
        /// <summary>The rate of decay used for machines that do not require input items. See also: <see cref="StandardDecayRate"/></summary>
        [XmlElement("InputlessDecayRate")]
        public double InputlessDecayRate { get; set; }

        public AugmentorConfig()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            AugmentorType = AugmentorType.Speed;
            BasePrice = 25000;
            ShopAppearanceWeight = 1;
            ShopStockMultiplier = 2.0;
            MaxAttachmentsPerMachine = 100;
            UseLinearFormula = false;
            MaxEffectPerStandardMachine = 2.5;
            MaxEffectPerInputlessMachine = 2.0;
            StandardDecayRate = 0.075;
            InputlessDecayRate = StandardDecayRate / 2.0;
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

    [XmlRoot(ElementName = "TravellingMerchantConfig", Namespace = "")]
    public class TravellingMerchantSettings
    {
        /// <summary>A percentage amount that the actual price of an augmentor can randomly deviate between.<para/>
        /// For example, if an augmentor has a BasePrice of 5000, and PriceDeviation = 0.4, then the actual price of the augmentor is randomly chosen between 100%+/-40% = 60%-140% of the BasePrice, 
        /// so between 3000-7000</summary>
        [XmlElement("PriceDeviation")]
        public double PriceDeviation { get; set; }

        /// <summary>How many random numbers to average when generating the random multiplier that is applied to the augmentor's BasePrice.<para/>
        /// If PriceDeviationRolls = 1, then the chosen price will be a linear distribution within 1.0+/-PriceDeviation. (So the minimum price has the same odds of being chosen as the average price)<para/>
        /// Using more rolls means the price distribution will look more like a bell curve, so the average price multiplier of 100% is more likely than the extremes.</summary>
        [XmlElement("PriceDeviationRolls")]
        public int PriceDeviationRolls { get; set; }

        /// <summary>How many different augmentors appear in the shop on average.</summary>
        [XmlElement("NumAugmentorTypesInShop")]
        public double NumAugmentorTypesInShop { get; set; }

        [XmlElement("BaseQuantityInStock")]
        public double BaseQuantityInStock { get; set; }

        /// <summary>An additional increase to the shop's quantities in stock based on how many years have passed in the current game.<para/>
        /// For example, if set to 0.2, then on Year2, there would be a 20% bonus (x1.2 in stock), Year3 would have 40% bonus etc</summary>
        [XmlElement("YearShopStockMultiplierBonus")]
        public double YearShopStockMultiplierBonus { get; set; }

        public TravellingMerchantSettings()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            this.PriceDeviation = 0.6;
            this.PriceDeviationRolls = 2;
            this.NumAugmentorTypesInShop = 2.5;
            this.BaseQuantityInStock = 1.25;
            this.YearShopStockMultiplierBonus = 0.35;
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

    [XmlRoot(ElementName = "MonsterLootConfig", Namespace = "")]
    public class MonsterLootSettings
    {
        /// <summary>The base chance of receiving an Augmentor when any monster is slain. 0.01 = 1% chance</summary>
        [XmlElement("BaseDropChance")]
        public double BaseDropChance { get; set; }

        #region Location Modifiers
        /// <summary>An additional bonus to the chance of receiving an Augmentor when a monster is slain within the mines. 0.01 = +1% chance<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="MineDepthBonusPerLevel"/> = 0.01, and player is in Mine level 35, you would have a 0.01 * 35 = +35% (1.35 multiplier) chance to receive drops)</summary>
        [XmlElement("MineDepthBonusPerLevel")]
        public double MineDepthBonusPerLevel { get; set; }

        /// <summary>Values of <see cref="MineShaft.mineLevel"/> that are greater than this value will no longer give a bonus to the drop chance of receiving an Augmentor from a slain monster.</summary>
        [XmlElement("MaxMineDepth")]
        public int MaxMineDepth { get; set; }

        /// <summary>An additional bonus to the chance of receiving an Augmentor when a monster is slain within the Quarry, 0.01 = +1% chance<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="QuarryLocationBonus"/> = 0.5, the player is in the Quarry, you would have a 0.5 = +50% (1.5 multiplier) chance to receive drops)</summary>
        [XmlElement("QuarryLocationBonus")]
        public double QuarryLocationBonus { get; set; }

        /// <summary>An additional bonus to the chance of receiving an Augmentor when a monster is slain within the Forest, 0.01 = +1% chance<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="ForestLocationBonus"/> = 0.5, the player is in the Forest, you would have a 0.5 = +50% (1.5 multiplier) chance to receive drops)</summary>
        [XmlElement("ForestLocationBonus")]
        public double ForestLocationBonus { get; set; }

        /// <summary>An additional bonus to the chance of receiving an Augmentor when a monster is slain within a location not recognized by other settings (I.E. not in the mines, quarry, or forest), 0.01 = +1% chance<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="OtherLocationBonus"/> = 0.5, the player is NOT in the mines/quarry/forest, you would have a 0.5 = +50% (1.5 multiplier) chance to receive drops)</summary>
        [XmlElement("OtherLocationBonus")]
        public double OtherLocationBonus { get; set; }
        #endregion Location Modifiers

        #region Experience Modifiers
        /// <summary>The Base amount of <see cref="Monster.ExperienceGained"/> used when calculating additional bonuses or penalties to drop chances.<para/>
        /// If the slain monster awards MORE than <see cref="BaseExperience"/>, you will receive a bonus to the chance of receiving an Augmentor. 
        /// This bonus is affected by <see cref="MaxExperience"/> and <see cref="MaxExperienceMultiplier"/></summary>
        [XmlElement("BaseExperience")]
        public int BaseExperience { get; set; }

        /// <summary>If a slain monster's <see cref="Monster.ExperienceGained"/> is greater than or equal to <see cref="MaxExperience"/>, then the additional bonus to drop rates is set to <see cref="MaxExperienceMultiplier"/></summary>
        [XmlElement("MaxExperience")]
        public int MaxExperience { get; set; }

        /// <summary>The bonus multiplier to apply to the chance of receiving Augmentors from slain monsters, when the monster's <see cref="Monster.ExperienceGained"/> is at least <see cref="MaxExperience"/></summary>
        [XmlElement("MaxExperienceMultiplier")]
        public double MaxExperienceMultiplier { get; set; }

        /// <summary>If a slain monster's <see cref="Monster.ExperienceGained"/> is less than or equal to <see cref="MinExperience"/>, then the additional penalty to drop rates is set to <see cref="MinExperienceMultiplier"/></summary>
        [XmlElement("MinExperience")]
        public int MinExperience { get; set; }

        /// <summary>The penalty multiplier to apply to the chance of receiving Augmentors from slain monsters, when the monster's <see cref="Monster.ExperienceGained"/> is at most <see cref="MinExperience"/></summary>
        [XmlElement("MinExperienceMultiplier")]
        public double MinExperienceMultiplier { get; set; }
        #endregion Experience Modifiers

        /// <summary>An additional bonus to the chance of receiving an Augmentor when a monster is slain. This value is multiplied by the slain monster's <see cref="Monster.MaxHealth"/>/10 (0.01 = +1% chance per 10 HP)<para/>
        /// (Stacks multiplicatively with <see cref="BaseDropChance"/>, so if <see cref="BonusPer10HP"/> = 0.05, and the slain monster had 80 HP, you would have a 0.05*80/10=0.4 = +40% (1.4 multiplier) chance to receive drops)</summary>
        [XmlElement("BonusPer10HP")]
        public double BonusPer10HP { get; set; }

        /// <summary>Values of <see cref="Monster.MaxHealth"/> that are greater than this value will no longer give a bonus to the drop chance of receiving an Augmentor from a slain monster.</summary>
        [XmlElement("MaxHP")]
        public int MaxHP { get; set; }

        public MonsterLootSettings()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            this.BaseDropChance = 0.0013;

            this.MineDepthBonusPerLevel = 0.02;
            this.MaxMineDepth = 270;
            this.QuarryLocationBonus = 0.9;
            this.ForestLocationBonus = 0.75;
            this.OtherLocationBonus = 0.25;

            this.BaseExperience = 9;
            this.MinExperience = 3;
            this.MinExperienceMultiplier = 0.5;
            this.MaxExperience = 17;
            this.MaxExperienceMultiplier = 2.0;

            this.BonusPer10HP = 0.035;
            this.MaxHP = 200;
        }

        public double GetAugmentorDropChance(GameLocation Location, Monster SlainMonster, out double BaseChance, out double LocationMultiplier, out double ExpMultiplier, out double HPMultiplier)
        {
            BaseChance = this.BaseDropChance;

            //  Compute bonuses based on where the monster was killed
            double LocationBonus;
            if (SlainMonster.mineMonster.Value && Location is MineShaft Mine)
            {
                bool IsQuarry = Mine.mapImageSource.Value != null && Mine.mapImageSource.Value != null && Path.GetFileName(Mine.mapImageSource.Value).Equals("mine_quarryshaft", StringComparison.CurrentCultureIgnoreCase);
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
}
