using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.NewMachinesMod.Data;
using PreferencesList = System.Collections.Generic.List<Igorious.StardewValley.DynamicAPI.Data.Supporting.DynamicID<Igorious.StardewValley.DynamicAPI.Constants.ItemID, Igorious.StardewValley.DynamicAPI.Constants.CategoryID>>;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig : DynamicConfiguration
    {
        #region Constants

        private const string AutoColor = "@";

        private const int MillID = 164;
        private const int TankID = 166;
        private const int VinegarJugID = 168;
        private const int DryerID = 169;
        private const int MixerID = 172;
        private const int SeparatorID = 179;
        private const int FermenterID = 180;
        private const int ChurnID = 182;

        private const int MoreCrops_Rice = 839;
        private const int MoreCrops_Cotton = 836;
        private const int MoreCrops_Olive = 812;

        private const int MeadID = 900;
        private const int VodkaID = 901;
        private const int ColorWineID = 902;
        private const int CactusSeedID = 904;
        private const int ColorJellyID = 905;
        private const int ColorPicklesID = 907;
        private const int ColorJuiceID = 909;
        public const int ExperementalLiquidID = 911;
        private const int PreservedMushroomID = 913;
        private const int BlackRaisinsID = 915;
        private const int DriedFruitID = 917;
        private const int BlackRaisinsMuffinID = 919;
        private const int BigWarpTotemFarmID = 921;
        private const int BigWarpTotemMountainsID = 924;
        private const int BigWarpTotemBeachID = 927;
        private const int SakeID = 930;
        private const int ButterID = 931;
        private const int SourCreamID = 932;

        #endregion

        #region Initialize

        public override void CreateDefaultConfiguration()
        {
            SimpleMachines = new List<MachineInformation>
            {
                GetMill(),
                GetVinegarJug(),
                GetDryingRack(),
            };

            Tank = GetTank();
            Mixer = GetMixer();
            Fermenter = GetFermenter();
            Churn = GetChurn();
            Separator = GetSeparator();

            ItemOverrides = new List<ItemInformation>
            {
                new ItemInformation
                {
                    ID = ItemID.WheatFlour,
                    Name = "Flour",
                    Description = "A common cooking ingredient made from crushed seeds.",
                }
            };

            Items = new List<ItemInformation>
            {
                GetMead(),
                GetVodka(),
                GetColorJuice(),
                GetColorWine(),
                GetCactusSeed(),
                GetColorJelly(),
                GetColorPickles(),
                GetExperementalLiquid(),
                GetPreservedMushrooms(),
                GetBlackRaisins(),
                GetDriedFruits(),
                GetBlackRisinsMaffin(),
                GetSake(),
                GetSourCream(),
                GetButter(),
            };
            Totems = GetWarpTotems();

            MachineOverrides = new List<OverridedMachineInformation>
            {
                GetKegOverride(),
                GetPreserveJarOverride(),
                GetCharcoalKilnOverride(),
                GetLoomOverride(),
                GetOilMakerOverride(),
                GetRecyclingMachineOverride(),
            };

            LocalizationStrings = new Dictionary<LocalizationString, string>
            {
                {LocalizationString.TankRequiresWater, "Fill with water first"},
            };

            Crops.Add(GetCactusCrop());
            GiftPreferences.AddRange(GetGiftPreferences());
            Bundles = GetBundleInformation();

            CookingRecipes.Add(new CookingRecipeInformation(GetBlackRisinsMaffin(),
                new IngredientInfo(BlackRaisinsID, 1),
                new IngredientInfo(ItemID.Sugar, 1),
                new IngredientInfo(ItemID.WheatFlour, 1)));
        }

        #endregion

        #region	Auxiliary Methods

        private static BundlesInformation GetBundleInformation()
        {
            return new BundlesInformation(
                new List<OverridedBundleInformation>
                {
                    new OverridedBundleInformation("Bulletin Board/33",
                        new BundleItemInformation(ColorWineID, quality: 2)),
                    new OverridedBundleInformation("Pantry/5",
                        new BundleItemInformation(ColorJellyID, quality: 2)),
                },
                new List<OverridedBundleInformation>
                {
                    new OverridedBundleInformation("Bulletin Board/33",
                        new BundleItemInformation(ItemID.Wine)),
                    new OverridedBundleInformation("Pantry/5",
                        new BundleItemInformation(ItemID.Jelly)),
                });
        }

        private static CropInformation GetCactusCrop()
        {
            return new CropInformation(CactusSeedID, ItemID.CactusFruit, 2, 3, 4, 3, 3)
            {
                HarvestMethod = 1,
                MaxHarvest = 3,
                MinHarvest = 3,
                MaxHarvestIncreaseForLevel = 3,
                TextureIndex = 90,
                ResourceIndex = 0,
                IsRaisedSeeds = true,
            };
        }

        private List<WarpTotemInformation> GetWarpTotems()
        {
            return new List<WarpTotemInformation>
            {
                new WarpTotemInformation(BigWarpTotemFarmID, "Big Warp Totem: Farm", "Warp directly to your house.")
                {
                    Category = CategoryID.Crafting,
                    Price = 300,
                    ResourceLength = 3,
                    ResourceHeight = 2,
                    ResourceIndex = 21,
                    WarpLocation = LocationName.Farm,
                    Materials = new Dictionary<DynamicID<ItemID>, int>
                    {
                        { ItemID.Hardwood, 20 },
                        { ItemID.Honey, 10 },
                        { ItemID.Fiber, 150 },
                    },
                    Skill = Skill.Foraging,
                    SkillLevel = 10,
                },

                new WarpTotemInformation(BigWarpTotemMountainsID, "Big Warp Totem: Mountains", "Warp directly to the mountains.")
                {
                    Category = CategoryID.Crafting,
                    Price = 300,
                    ResourceLength = 3,
                    ResourceHeight = 2,
                    ResourceIndex = 24,
                    WarpLocation = LocationName.Mountain,
                    Materials = new Dictionary<DynamicID<ItemID>, int>
                    {
                        { ItemID.Hardwood, 20 },
                        { ItemID.IronBar, 10 },
                        { ItemID.Stone, 175 },
                    },
                    Skill = Skill.Foraging,
                    SkillLevel = 10,
                },

                new WarpTotemInformation(BigWarpTotemBeachID, "Big Warp Totem: Beach", "Warp directly to the beach.")
                {
                    Category = CategoryID.Crafting,
                    Price = 300,
                    ResourceLength = 3,
                    ResourceHeight = 2,
                    ResourceIndex = 27,
                    WarpLocation = LocationName.Beach,
                    Materials = new Dictionary<DynamicID<ItemID>, int>
                    {
                        { ItemID.Hardwood, 20 },
                        { ItemID.Coral, 20 },
                        { ItemID.Fiber, 75 },
                    },
                    Skill = Skill.Foraging,
                    SkillLevel = 10,
                },
            };
        }

        private static OverridedMachineInformation GetPreserveJarOverride()
        {
            return new OverridedMachineInformation(CraftableID.PreservesJar,
                new MachineOutputInformation(new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                {
                    {CategoryID.Vegetable, new OutputItem(ColorPicklesID, "Pickled {1}") { Color = AutoColor}},
                    {ItemID.WildHorseradish, new OutputItem(ColorPicklesID, "Pickled {1}") { Color = "897121"}},
                    {ItemID.SnowYam, new OutputItem(ColorPicklesID, "Pickled {1}") { Color = "009EFF"}},
                    {ItemID.CaveCarrot, new OutputItem(ColorPicklesID, "Pickled {1}") { Color = "68380C"}},
                    {ItemID.Leek, new OutputItem(ColorPicklesID, "Pickled {1}") { Color = "C4B0BA"}},
                    {ItemID.SpringOnion, new OutputItem(ColorPicklesID, "Pickled {1}") { Color = "E2AAB5"}},

                    {CategoryID.Fruits, new OutputItem(ColorJellyID, "{1} {0}") { Color = AutoColor }},
                    {ItemID.WildPlum, new OutputItem(ColorJellyID, "{1} {0}") { Color = AutoColor }},

                    {ItemID.RedMushroom, new OutputItem(PreservedMushroomID, "Preserved {1}") { Color = AutoColor }},
                    {ItemID.PurpleMushroom, new OutputItem(PreservedMushroomID, "Preserved {1}") { Color = AutoColor }},
                    {ItemID.CommonMushroom, new OutputItem(PreservedMushroomID, "Preserved {1}") { Color = "924903" }},
                    {ItemID.Morel, new OutputItem(PreservedMushroomID, "Preserved {1}") { Color = "C68243" }},
                    {ItemID.Chanterelle, new OutputItem(PreservedMushroomID, "Preserved {1}") { Color = AutoColor }},
                    {ItemID.Truffle, new OutputItem(PreservedMushroomID, "Preserved {1}") { Color = "541A15" }},
                })
                {
                    Quality = "q",
                    Price = "40 + 8 * p / 5",
                    MinutesUntilReady = 4000,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                });
        }

        private static OverridedMachineInformation GetKegOverride()
        {
            return new OverridedMachineInformation(CraftableID.Keg,
                new MachineOutputInformation(new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                {
                    {ItemID.Hops, new OutputItem(ItemID.PaleAle) {MinutesUntilReady = 2360}},
                    {ItemID.Wheat, new OutputItem(ItemID.Beer) {MinutesUntilReady = 2250}},
                    {ItemID.Honey, new OutputItem(MeadID) {MinutesUntilReady = 4000, Quality = "p / 300"}},
                    {ItemID.Potato, new OutputItem(VodkaID) {MinutesUntilReady = 3000}},
                    {MoreCrops_Rice, new OutputItem(SakeID) {InputCount = 5, MinutesUntilReady = 3000}},
                    {CategoryID.Vegetable, new OutputItem(ColorJuiceID) {MinutesUntilReady = 6000, Color = AutoColor, Price = "9 * p / 5", Name = "{1} {0}"}},
                    {CategoryID.Fruits, new OutputItem(ColorWineID) {MinutesUntilReady = 10000, Color = AutoColor, Price = "12 * p / 5", Name = "{1} {0}"}},
                    {ItemID.WildPlum, new OutputItem(ColorWineID) {MinutesUntilReady = 10000, Color = AutoColor, Price = "12 * p / 5", Name = "{1} {0}"}},
                })
                {
                    Quality = "q",
                    MinutesUntilReady = 1440,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                });
        }

        private static OverridedMachineInformation GetCharcoalKilnOverride()
        {
            return new OverridedMachineInformation(CraftableID.CharcoalKiln,
                new MachineOutputInformation(new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                {
                    { ItemID.Hardwood, new OutputItem(ItemID.Coal) { MinutesUntilReady = 30 } },
                    { ItemID.HardwoodFence, new OutputItem(ItemID.Coal) { MinutesUntilReady = 30 } },
                    { ItemID.Gate, new OutputItem(ItemID.Coal) { MinutesUntilReady = 30 } },
                    { ItemID.WoodFence, new OutputItem(ItemID.Coal) { InputCount = 5, MinutesUntilReady = 30 } },
                })
                {
                    Sounds = new List<Sound> { Sound.OpenBox, Sound.Fireball },
                    Animation = Animation.Steam,
                })
            {
                Draw = new MachineDraw { Working = +1 },
            };
        }

        private static OverridedMachineInformation GetLoomOverride()
        {
            return new OverridedMachineInformation(CraftableID.Loom,
                new MachineOutputInformation(new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                {
                    { MoreCrops_Cotton, new OutputItem(ItemID.Cloth) { InputCount = 4, MinutesUntilReady = 240 } },
                }))
            {
                Draw = new MachineDraw { Ready = +1 },
            };
        }

        private static OverridedMachineInformation GetOilMakerOverride()
        {
            return new OverridedMachineInformation(CraftableID.OilMaker,
                new MachineOutputInformation(new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                {
                    {MoreCrops_Olive, new OutputItem(ItemID.Oil) {MinutesUntilReady = 1000}},
                })
                {
                    Sounds = new List<Sound> { Sound.Bubbles, Sound.SipTea }
                });
        }

        private static OverridedMachineInformation GetRecyclingMachineOverride()
        {
            return new OverridedMachineInformation(CraftableID.RecyclingMachine,
                new MachineOutputInformation(new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                {
                    [ItemID.JojaCola] = new OutputItem
                    {
                        Switch = new List<OutputItem>
                        {
                            new OutputItem(ItemID.IronOre) {Chance = 0.3m},
                            new OutputItem(ItemID.Slime),
                        }
                    }
                })
                {
                    MinutesUntilReady = 60,
                });
        }

        private static ItemInformation GetColorPickles()
        {
            return new ItemInformation(ColorPicklesID, "Pickles", "A jar of your home-made pickles.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 500,
                ResourceIndex = 7,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetPreservedMushrooms()
        {
            return new ItemInformation(PreservedMushroomID, "Preserved Mushroom", "A jar with processed mushrooms.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 500,
                ResourceIndex = 13,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetBlackRaisins()
        {
            return new ItemInformation(BlackRaisinsID, "Black Raisins", "Dried berries of black grape.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 140,
                ResourceIndex = 15,
                MealCategory = MealCategory.Food,
                Edibility = 40,
            };
        }

        private static ItemInformation GetDriedFruits()
        {
            return new ItemInformation(DriedFruitID, "Dried Fruits", "Dried fruits.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 140,
                ResourceIndex = 17,
                ResourceLength = 2,
                MealCategory = MealCategory.Food,
                Edibility = 40,
                IsColored = true,
            };
        }

        private static ItemInformation GetBlackRisinsMaffin()
        {
            return new ItemInformation(BlackRaisinsMuffinID, "Black Raisins Muffin", "A small tasty cake.")
            {
                Category = CategoryID.Cooking,
                Price = 250,
                ResourceIndex = 19,
                MealCategory = MealCategory.Food,
                Edibility = 60,
            };
        }

        private static ItemInformation GetColorJelly()
        {
            return new ItemInformation(ColorJellyID, "Jelly", "Gooey.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 500,
                ResourceIndex = 5,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetCactusSeed()
        {
            return new ItemInformation(CactusSeedID, "Cactus Seed", "Requres special environment. Takes 15 days to mature.")
            {
                Category = CategoryID.Seeds,
                Type = ObjectType.Seeds,
                Price = 30,
                ResourceIndex = 4,
            };
        }

        private static ItemInformation GetColorWine()
        {
            return new ItemInformation(ColorWineID, "Wine", "Drink in moderation.")
            {
                Category = CategoryID.ArtisanGoods,
                ResourceIndex = 2,
                ResourceLength = 2,
                Edibility = 15,
                IsColored = true,
            };
        }

        private static ItemInformation GetColorJuice()
        {
            return new ItemInformation(ColorJuiceID, "Juice", "A sweet, nutritious beverage.")
            {
                Category = CategoryID.ArtisanGoods,
                ResourceIndex = 9,
                ResourceLength = 2,
                Edibility = 18,
                IsColored = true,
            };
        }

        private static ItemInformation GetExperementalLiquid()
        {
            return new ItemInformation(ExperementalLiquidID, "Experemental Liquid", "A strange liquid.")
            {
                Category = CategoryID.ArtisanGoods,
                Type = ObjectType.Basic,
                ResourceIndex = 11,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetVodka()
        {
            return new ItemInformation(VodkaID, "Vodka", "A light alcohol drink.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 400,
                Edibility = 15,
                MealCategory = MealCategory.Drink,
                SkillUps = new SkillUpInformation { Speed = -1, Defence = +1 },
                Duration = 60,
                ResourceIndex = 1,
            };
        }

        private static ItemInformation GetSake()
        {
            return new ItemInformation(SakeID, "Sake", "A Japanese alcohol drink.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 400,
                Edibility = 17,
                MealCategory = MealCategory.Drink,
                SkillUps = new SkillUpInformation { MaxEnergy = +50, Speed = -1 },
                Duration = 60,
                ResourceIndex = 30,
            };
        }

        private static ItemInformation GetButter()
        {
            return new ItemInformation(ButterID, "Butter", "A milk product.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 180,
                Edibility = 17,
                MealCategory = MealCategory.Food,
                ResourceIndex = 31,
            };
        }

        private static ItemInformation GetSourCream()
        {
            return new ItemInformation(SourCreamID, "Sour Cream", "A milk product.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 180,
                Edibility = 17,
                MealCategory = MealCategory.Food,
                ResourceIndex = 32,
            };
        }

        private static ItemInformation GetMead()
        {
            return new ItemInformation(MeadID, "Mead", "Drink from honey.")
            {
                Category = CategoryID.ArtisanGoods,
                Price = 500,
                Edibility = 20,
                MealCategory = MealCategory.Drink,
                SkillUps = new SkillUpInformation { MaxEnergy = +50 },
                ResourceIndex = 0,
            };
        }

        private static MachineInformation GetDryingRack()
        {
            return new MachineInformation
            {
                ID = DryerID,
                ResourceIndex = 5,
                ResourceLength = 3,
                Name = "Drying Rack",
                Description = "It has been created to dry everything.",
                Skill = Skill.Farming,
                SkillLevel = 6,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    {ItemID.Wood, 30},
                    {ItemID.Hardwood, 2},
                },
                Output = new MachineOutputInformation
                {
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ItemID.Fiber, new OutputItem(2, ItemID.Hay, draw: new MachineDraw { WorkingColor = "A3BE00", Working = +1, ReadyColor = "FCD000", Ready = +1 }) { Count = "2" } },
                        { MoreCrops_Rice, new OutputItem(4, ItemID.Rice, draw: new MachineDraw { WorkingColor = "FFFFFF", Working = +2, ReadyColor = "FFFFFF", Ready = +2 }) },
                        { ItemID.Grape, new OutputItem(BlackRaisinsID, draw: new MachineDraw { WorkingColor = "C46DC9", Working = +2, ReadyColor = "C46DC9", Ready = +2 }) },
                        { CategoryID.Fruits, new OutputItem(DriedFruitID, draw: new MachineDraw { WorkingColor = "@", Working = +2, ReadyColor = "@", Ready = +2 }) { Color = "@", Name = "Dried {1}", Price = "25 + p * 11 / 10" } },
                    },
                    MinutesUntilReady = 720,
                },
            };
        }

        private static MachineInformation GetMixer()
        {
            return new MachineInformation
            {
                ID = MixerID,
                ResourceIndex = 8,
                ResourceLength = 3,
                Name = "Mixer",
                Description = "Science machine that allows to mix various items.",
                Skill = Skill.Farming,
                SkillLevel = 10,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.CopperBar, 1 },
                    { ItemID.IronBar, 1 },
                    { ItemID.GoldOre, 1 },
                    { ItemID.IridiumOre, 1 },
                    { ItemID.RefinedQuartz, 1 },
                },
                Output = new MachineOutputInformation
                {
                    ID = ExperementalLiquidID,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ColorWineID, null },
                        { ColorJuiceID, null },
                    },
                    MinutesUntilReady = 2880,
                },
            };
        }

        private static MachineInformation GetVinegarJug()
        {
            return new MachineInformation
            {
                ID = VinegarJugID,
                ResourceIndex = 4,
                Name = "Vinegar Jug",
                Description = "Ceramic thing, that used to vinegar creation.",
                Skill = Skill.Farming,
                SkillLevel = 9,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.Clay, 30 },
                    { ItemID.Slime, 10 },
                    { ItemID.CopperBar, 1 },
                },
                Output = new MachineOutputInformation
                {
                    ID = ItemID.Vinegar,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ItemID.Apple, new OutputItem {Name = "Apple Cider {0}"} },
                        { ItemID.Wine, new OutputItem {Name = "Wine {0}", Price = "500", Quality = "q + 1"} },
                        { ColorWineID, new OutputItem {Name = "Wine {0}", Price = "500", Quality = "q + 1"} },
                    },
                    Quality = "q",
                    Count = "(r1 <= 0.00025 * p)? 2 : 1",
                    MinutesUntilReady = 600,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                },
            };
        }

        private static MachineInformation GetTank()
        {
            return new MachineInformation
            {
                ID = TankID,
                ResourceIndex = 2,
                ResourceLength = 2,
                Name = "Tank",
                Description = "Fill with water and use for sugar extraction from beet.",
                Skill = Skill.Farming,
                SkillLevel = 8,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.CopperBar, 1 },
                    { ItemID.IronBar, 1 },
                    { ItemID.Coal, 5 },
                },
                Output = new MachineOutputInformation
                {
                    ID = ItemID.Sugar,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ItemID.Beet, new OutputItem() },
                        { 870, new OutputItem { Name = "Cane Sugar" } },
                    },
                    Name = "{1} {0}",
                    Price = "p + 25",
                    Quality = "q",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                    MinutesUntilReady = 360,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                },
            };
        }

        private static MachineInformation GetSeparator()
        {
            return new MachineInformation
            {
                ID = SeparatorID,
                ResourceIndex = 15,
                Name = "Separator",
                Description = "Performs separation of liquids. Section: Requires additional modules to work.",
                Skill = Skill.Farming,
                SkillLevel = 8,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.IronBar, 2 },
                },
                Output = new MachineOutputInformation
                {
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>(),
                    Price = "3 * p / 2",
                    MinutesUntilReady = 60,
                    Animation = Animation.Steam,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                },
                AllowedModules = new List<DynamicID<CraftableID>> { ChurnID, FermenterID }
            };
        }

        private static MachineInformation GetChurn()
        {
            return new MachineInformation
            {
                ID = ChurnID,
                ResourceIndex = 18,
                ResourceLength = 4,
                Name = "Churn",
                Description = "Used for creating butter. Module: Place to the right of Separator.",
                Skill = Skill.Farming,
                SkillLevel = 8,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.IronBar, 2 },
                },
                Output = new MachineOutputInformation
                {
                    ID = ButterID,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ItemID.Milk, new OutputItem { Quality = "0" } },
                        { ItemID.GoatMilk, new OutputItem { Quality = "0" } },
                        { ItemID.LargeMilk, new OutputItem { Quality = "2" } },
                        { ItemID.LargeGoatMilk, new OutputItem { Quality = "2" } },
                    },
                    Price = "3 * p / 2",
                    MinutesUntilReady = 60,
                },
                AllowedSections = new List<DynamicID<CraftableID>> { SeparatorID },
            };
        }

        private static MachineInformation GetFermenter()
        {
            return new MachineInformation
            {
                ID = FermenterID,
                ResourceIndex = 16,
                ResourceLength = 2,
                Name = "Fermenter",
                Description = "Used for creating sour cream. Module: Place to the right of Separator.",
                Skill = Skill.Farming,
                SkillLevel = 8,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.IronBar, 2 },
                },
                Output = new MachineOutputInformation
                {
                    ID = SourCreamID,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ItemID.Milk, new OutputItem { Quality = "0" } },
                        { ItemID.GoatMilk, new OutputItem { Quality = "0" } },
                        { ItemID.LargeMilk, new OutputItem { Quality = "2" } },
                        { ItemID.LargeGoatMilk, new OutputItem { Quality = "2" } },
                    },
                    Price = "3 * p / 2",
                    MinutesUntilReady = 60,
                },
                AllowedSections = new List<DynamicID<CraftableID>> { SeparatorID },
            };
        }

        private static MachineInformation GetMill()
        {
            return new MachineInformation
            {
                ID = MillID,
                ResourceIndex = 0,
                ResourceLength = 2,
                Name = "Mill",
                Description = "Small machine, that crushes seeds to flour.",
                Skill = Skill.Farming,
                SkillLevel = 7,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    {ItemID.Wood, 50},
                    {ItemID.Hardwood, 5},
                    {ItemID.IronBar, 1},
                },
                Output = new MachineOutputInformation
                {
                    ID = ItemID.WheatFlour,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        {ItemID.Wheat, new OutputItem()},
                        {ItemID.Amaranth, new OutputItem()},
                        {ItemID.Corn, new OutputItem()},
                    },
                    Name = "{1} {0}",
                    Price = "p + 25",
                    Quality = "q",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                    MinutesUntilReady = 360,
                },
                Draw = new MachineDraw
                {
                    Ready = +1,
                }
            };
        }

        private static List<GiftPreferences> GetGiftPreferences()
        {
            return new List<GiftPreferences>
            {
                new GiftPreferences(CharacterName.Penny) { Neutral = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Leah) { Loved = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Elliott) { Liked = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Gus) { Liked = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Harvey) { Loved = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Jas) { Liked = new PreferencesList {ColorJellyID} },
                new GiftPreferences(CharacterName.Vincent)
                {
                    Liked = new PreferencesList {ColorJellyID},
                    Loved = new PreferencesList {BlackRaisinsID, BlackRaisinsMuffinID},
                },
                new GiftPreferences(CharacterName.Maru) { Hated = new PreferencesList {ColorPicklesID} },
                new GiftPreferences(CharacterName.Sam) { Hated = new PreferencesList {ColorPicklesID} },
                new GiftPreferences(CharacterName.Shane) { Hated = new PreferencesList {ColorPicklesID} },
                new GiftPreferences(CharacterName.Harvey) { Loved = new PreferencesList {ColorPicklesID} },
            };
        }

        #endregion
    }
}