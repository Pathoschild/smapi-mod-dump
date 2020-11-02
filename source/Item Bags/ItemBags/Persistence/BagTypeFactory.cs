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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags.Persistence
{
    public static class BagTypeFactory
    {
        public static readonly Dictionary<ContainerSize, int> DefaultPrices = new Dictionary<ContainerSize, int>()
        {
            { ContainerSize.Small, 2500 },
            { ContainerSize.Medium, 7500 },
            { ContainerSize.Large, 25000 },
            { ContainerSize.Giant, 75000 },
            { ContainerSize.Massive, 250000 }
        };

        public static readonly Dictionary<ContainerSize, int> DefaultCapacities = new Dictionary<ContainerSize, int>()
        {
            { ContainerSize.Small, 30 },
            { ContainerSize.Medium, 99 },
            { ContainerSize.Large, 300 },
            { ContainerSize.Giant, 999 },
            { ContainerSize.Massive, 9999 }
        };

        public static double GetCapacityMultiplier(ContainerSize Size, int DesiredCapacity)
        {
            return DesiredCapacity * 1.0 / DefaultCapacities[Size];
        }

        public static BagType GetGemBagType()
        {
            BagSizeConfig.BagShop[] DefaultSellers = new BagSizeConfig.BagShop[] { BagSizeConfig.BagShop.Clint, BagSizeConfig.BagShop.Dwarf };

            List<int> AllowedObjectIds = new List<int>()
            {
                68, // Topaz
                66, // Amethyst
                62, // Aquamarine
                70, // Jade
                60, // Emerald
                64, // Ruby
                72,  // Diamond
                74, // Prismatic Shard
                21, // Crystalarium
            };
            HashSet<int> BigCraftableIds = new HashSet<int>() { 21 }; // Crystalarium
            StoreableBagItem[] Items = AllowedObjectIds.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToArray();

            BagMenuOptions MenuOptions = new BagMenuOptions()
            {
                GroupByQuality = false,
                GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
            };

            return new BagType()
            {
                Id = "64fd96d5-b15f-40bb-a60f-181f57f597a0",
                Name = "Gem Bag",
                Description = "A bag for storing valuable gems.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(12 * 16, 2 * 16, 16, 16),
                SizeSettings = GenerateSizeConfigs(DefaultSellers, MenuOptions, Items, 0.4, x =>
                {
                    //  Massive bags are only sold by the dwarf
                    if (x.Size == ContainerSize.Massive)
                    {
                        x.Sellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Dwarf };
                    }

                    x.Items = Items.Take(Items.Length - (int)ContainerSize.Massive + (int)x.Size).ToList();
                    //if (x.Size == ContainerSize.Small)
                    //    x.CapacityMultiplier = 5.0 / DefaultCapacities[x.Size];
                    //else if (x.Size == ContainerSize.Medium)
                    //    x.CapacityMultiplier = 10.0 / DefaultCapacities[x.Size];
                    //else if (x.Size == ContainerSize.Large)
                    //    x.CapacityMultiplier = 25.0 / DefaultCapacities[x.Size];
                    //else if (x.Size == ContainerSize.Giant)
                    //    x.CapacityMultiplier = 100.0 / DefaultCapacities[x.Size];
                    //else if (x.Size == ContainerSize.Massive)
                    //    x.CapacityMultiplier = 999.0 / DefaultCapacities[x.Size];
                })
            };
        }

        public static BagType GetSmithingBagType()
        {
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Clint };
            HashSet<int> BigCraftableIds = new HashSet<int>() { 13 }; // Furnace
            double PriceMultiplier = 1.5;

            return new BagType()
            {
                Id = "e5ccd506-99ac-4238-98ad-4df34f182143",
                Name = "Smithing Bag",
                Description = "A bag for storing ores, bars, and geodes.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(23 * 16, 13 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            382, 378, 334, 535 // Coal, Copper Ore, Copper Bar, Geode
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            382, 378, 334, 380, 335, 80, 338, 535, 536, 13 // Coal, Copper Ore, Copper Bar, Iron Ore, Iron Bar, Quartz, Refined Quartz, Geode, Frozen Geode, Furnace
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToList(),
                        CapacityMultiplier =  60.0 / DefaultCapacities[ContainerSize.Medium],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            382, 378, 334, 380, 335, 384, 336,  // Coal, Copper Ore, Copper Bar, Iron Ore, Iron Bar, Gold Ore, Gold Bar
                            80, 82, 338, 535, 536, 537, 13      // Quartz, Fire Quartz, Refined Quartz, Geode, Frozen Geode, Magma Geode, Furnace
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToList(),
                        CapacityMultiplier = 100.0 / DefaultCapacities[ContainerSize.Large],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 7
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            378, 334, 380, 335, 384, 336, 386, 337,     // Copper Ore, Copper Bar, Iron Ore, Iron Bar, Gold Ore, Gold Bar, Iridium Ore, Iridium Bar
                            382, 80, 82, 338, 535, 536, 537, 749,       // Coal, Quartz, Fire Quartz, Refined Quartz, Geode, Frozen Geode, Magma Geode, Omni Geode
                            13                                          // Furnace
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 8,
                                LineBreakIndices = new int[] { 7, 15 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            378, 334, 380, 335, 384, 336, 386, 337,     // Copper Ore, Copper Bar, Iron Ore, Iron Bar, Gold Ore, Gold Bar, Iridium Ore, Iridium Bar
                            382, 80, 82, 338, 535, 536, 537, 749,       // Coal, Quartz, Fire Quartz, Refined Quartz, Geode, Frozen Geode, Magma Geode, Omni Geode
                            13                                          // Furnace
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 8,
                                LineBreakIndices = new int[] { 7, 15 }
                            }
                        }
                    }
                }
                //SizeSettings = GenerateSizeConfigs(DefaultSellers, MenuOptions, new StoreableBagItem[], 1.5, x =>
                //{

                //}).Skip(2).ToArray()
            };
        }

        public static BagType GetMineralBagType()
        {
            BagSizeConfig.BagShop[] DefaultSellers = new BagSizeConfig.BagShop[] { BagSizeConfig.BagShop.Clint, BagSizeConfig.BagShop.Dwarf };

            List<int> AllowedObjectIds = new List<int>()
            {
                80 , 86 , 84 , 82 ,
                571, 574, 540, 568, 542, 569, 555, 556, 576, 544, 552, 558,
                567, 549, 557, 559, 572, 541, 566, 564, 538, 546, 548, 563,
                573, 570, 545, 551, 554, 561, 575, 560, 550, 577, 562, 539,
                543, 565, 553, 547, 578
            };
            StoreableBagItem[] Items = AllowedObjectIds.Select(x => new StoreableBagItem(x, false, null, false)).ToArray();

            BagMenuOptions MenuOptions = new BagMenuOptions()
            {
                GroupByQuality = false,
                GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                {
                    LineBreakIndices = new int[] { 3 },
                    LineBreakHeights = new int[] { 12 }
                }
            };

            return new BagType()
            {
                Id = "7ccf7f7f-b406-4088-82b1-438164a39e13",
                Name = "Mineral Bag",
                Description = "A bag for storing precious minerals.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(21 * 16, 23 * 16, 16, 16),
                SizeSettings = GenerateSizeConfigs(DefaultSellers, MenuOptions, Items, 0.7, x =>
                {
                    if (x.Size != ContainerSize.Massive)
                    {
                        x.Items = Items.Take(4 + (int)x.Size * 12).ToList();
                    }
                })
            };
        }

        public static BagType GetMiningBagType()
        {
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Dwarf, BagSizeConfig.BagShop.Clint };
            double PriceMultiplier = 2.4;
            HashSet<int> BigCraftableIds = new HashSet<int>();

            return new BagType()
            {
                Id = "4bbd80c6-fc49-4878-9061-7a41a9e25fbb",
                Name = "Mining Bag",
                Description = "A bag for storing ores, geodes, and gems.",
                IconSourceTexture = BagType.SourceTexture.Tools, // BagType.SourceTexture.Cursors,
                IconSourceRect = new Rectangle(81, 96, 16, 16), //new Rectangle(0, 672, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 20.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                390, 382, 378, 535,     // Stone, Coal, Copper Ore, Geode
                                80, 86, 68, 66, 62, 70  // Quartz, Earth Crystal, Topaz, Amethyst, Aquamarine, Jade
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 6,
                                LineBreakIndices = new int[] { 3 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 60.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                390, 382, 378, 380, 535, 536,       // Stone, Coal, Copper Ore, Iron Ore, Geode, Frozen Geode
                                68, 66, 62, 70, 60, 64,             // Topaz, Amethyst, Aquamarine, Jade, Emerald, Ruby
                                80, 86, 84                          // Quartz, Earth Crystal, Frozen Tear
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 300.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                382, 378, 380, 384, 535, 536, 537,      // Coal, Copper Ore, Iron Ore, Gold Ore, Geode, Frozen Geode, Magma Geode
                                68, 66, 62, 70, 60, 64, 72,             // Topaz, Amethyst, Aquamarine, Jade, Emerald, Ruby, Diamond
                                390, 80, 86, 84, 82                     // Stone, Quartz, Earth Crystal, Frozen Tear, Fire Quartz
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 7 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 500.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                378, 380, 384, 386, 535, 536, 537, 749,     // Copper Ore, Iron Ore, Gold Ore, Iridium Ore, Geode, Frozen Geode, Magma Geode, Omni Geode
                                68, 66, 62, 70, 60, 64, 72, 74,             // Topaz, Amethyst, Aquamarine, Jade, Emerald, Ruby, Diamond, Prismatic Shard
                                390, 382, 80, 86, 84, 82                    // Stone, Coal, Quartz, Earth Crystal, Frozen Tear, Fire Quartz
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                378, 380, 384, 386, 535, 536, 537, 749,     // Copper Ore, Iron Ore, Gold Ore, Iridium Ore, Geode, Frozen Geode, Magma Geode, Omni Geode
                                68, 66, 62, 70, 60, 64, 72, 74,             // Topaz, Amethyst, Aquamarine, Jade, Emerald, Ruby, Diamond, Prismatic Shard
                                390, 382, 80, 86, 84, 82                    // Stone, Coal, Quartz, Earth Crystal, Frozen Tear, Fire Quartz
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    }
                }
            };
        }

        public static BagType GetResourceBagType()
        {
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Robin, BagSizeConfig.BagShop.Pierre };

            double PriceMultiplier = 1.4;

            return new BagType()
            {
                Id = "be6830c4-9ceb-451a-a5ed-905db9c7cf3f",
                Name = "Resource Bag",
                Description = "A bag for storing resources.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(13 * 16, 29 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            390, 382, 378, 334,
                            771, 330, 388
                        }.Select(x => new StoreableBagItem(x, false)).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 4
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            378, 334, 380, 335, 338,
                            390, 382, 771, 330, 388
                        }.Select(x => new StoreableBagItem(x, false)).ToList(),
                        //CapacityMultiplier = 80 / (double)DefaultCapacities[ContainerSize.Medium],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 5,
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            378, 334, 380, 335, 384, 336, 338,
                            390, 382, 771, 330, 388, 709
                        }.Select(x => new StoreableBagItem(x, false)).ToList(),
                        //CapacityMultiplier = 200 / (double)DefaultCapacities[ContainerSize.Large],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 7
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            378, 334, 380, 335, 384, 336, 386, 337,
                            390, 382, 771, 330, 388, 709, 338, 787
                        }.Select(x => new StoreableBagItem(x, false)).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 8,
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            378, 334, 380, 335, 384, 336, 386, 337,
                            390, 382, 771, 330, 388, 709, 338, 787
                        }.Select(x => new StoreableBagItem(x, false)).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 8,
                            }
                        }
                    }
                }
            };
        }

        public static BagType GetConstructionBagType()
        {
            BagSizeConfig.BagShop[] DefaultSellers = new BagSizeConfig.BagShop[] { BagSizeConfig.BagShop.Robin };

            List<int> AllowedObjectIds = new List<int>()
            {
                328, 401, 331, 333, 329, 293,   // Wood/Straw/Weathered/Crystal/Stone/Brick Floors
                405, 407, 411, 415, 409,        // Wood/Gravel/Cobblestone/Stepping Stone/Crystal Paths
                325, 322, 323, 324, 298,        // Gate + Wood/Stone/Iron/Hardwood Fences
                390, 388, 709                   // Stone, Wood, Hardwood
            };
            StoreableBagItem[] Items = AllowedObjectIds.Select(x => new StoreableBagItem(x, false, null, false)).ToArray();

            BagMenuOptions MenuOptions = new BagMenuOptions()
            {
                GroupByQuality = false,
                GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                {
                    Columns = 6,
                    LineBreakIndices = new int[] { 5, 10, 15 }
                }
            };

            return new BagType()
            {
                Id = "c5b27c87-7a7d-485b-ab63-95389b41ce65",
                Name = "Construction Bag",
                Description = "A bag for storing fences, floors, path,\nand construction resources.",
                IconSourceTexture = BagType.SourceTexture.Cursors,
                IconSourceRect = new Rectangle(367, 309, 16, 16),
                SizeSettings = GenerateSizeConfigs(DefaultSellers, MenuOptions, Items, 0.7, x => { })
            };
        }

        public static BagType GetTreeBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 105 }; // Tapper
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Robin, BagSizeConfig.BagShop.Pierre };

            double PriceMultiplier = 0.9;

            return new BagType()
            {
                Id = "bbdaf9f5-0389-4232-b466-97ac371d51e5",
                Name = "Tree Bag",
                Description = "A bag for storing tree seeds and products.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(22 * 16, 12 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            388, 92, 309, 310, 311 // Wood, Sap, Acorn, Maple seed, Pine cone
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 5
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            105, 725, 724, 726,     // Tapper, Oak Resin, Maple Syrup, Pine Tar
                            309, 310, 311,          // Acorn, Maple seed, Pine cone
                            388, 709, 92            // Wood, hardwood, sap
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 4,
                                LineBreakIndices = new int[] { 3, 6 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            629, 628, 630, 631, 633, 632,   // Apricot/Cherry/Orange/Peach/Apple/Pomegranate saplings
                            309, 310, 311,                  // Acorn, Maple seed, Pine cone
                            105, 725, 724, 726,             // Tapper, Oak Resin, Maple Syrup, Pine Tar
                            388, 709, 92, 805               // Wood, hardwood, sap, Tree Fertilizer
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 6,
                                LineBreakIndices = new int[] { 5, 8, 12 },
                                LineBreakHeights = new int[] { 0, 12, 0 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            629, 628, 630, 631, 633, 632,   // Apricot/Cherry/Orange/Peach/Apple/Pomegranate saplings
                            309, 310, 311,                  // Acorn, Maple seed, Pine cone
                            105, 725, 724, 726,             // Tapper, Oak Resin, Maple Syrup, Pine Tar
                            388, 709, 92, 805               // Wood, hardwood, sap, Tree Fertilizer
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).Union(
                            new int[] { 634, 638, 635, 636, 613, 637 }.Select(x => new StoreableBagItem(x, true)) // Fruits - Apricot/Cherry/Orange/Peach/Apple/Pomegranate
                        ).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 3,
                                ShowValueColumn = true
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 6,
                                LineBreakIndices = new int[] { 5, 8, 12 },
                                LineBreakHeights = new int[] { 0, 12, 0 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = new int[]
                        {
                            629, 628, 630, 631, 633, 632,   // Apricot/Cherry/Orange/Peach/Apple/Pomegranate saplings
                            309, 310, 311,                  // Acorn, Maple seed, Pine cone
                            105, 725, 724, 726,             // Tapper, Oak Resin, Maple Syrup, Pine Tar
                            388, 709, 92, 805               // Wood, hardwood, sap, Tree Fertilizer
                        }.Select(x => new StoreableBagItem(x, false, null, BigCraftableIds.Contains(x))).Union(
                            new int[] { 634, 638, 635, 636, 613, 637 }.Select(x => new StoreableBagItem(x, true)) // Fruits - Apricot/Cherry/Orange/Peach/Apple/Pomegranate
                        ).ToList(),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 3,
                                ShowValueColumn = true
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 6,
                                LineBreakIndices = new int[] { 5, 8, 12 },
                                LineBreakHeights = new int[] { 0, 12, 0 }
                            }
                        }
                    }
                }
            };
        }

        public static BagType GetAnimalProductBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 24, 16, 17, 19 }; // Mayonnaise Machine, Cheese Press, Loom, Oil Maker
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Marnie };
            double PriceMultiplier = 2.1;

            return new BagType()
            {
                Id = "60b29c0d-1d2e-4433-ada9-1f981ab9c0c1",
                Name = "Animal Products Bag",
                Description = "A bag for storing animal products.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(18 * 16, 7 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] { 176, 180, 442, 440, 184, 436 }, // Egg (white), Egg (brown), Duck Egg, Wool, Milk, Goat Milk
                            new int[] { },
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 3,
                                ShowValueColumn = true
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] 
                            {
                                176, 174, 442, // Egg (white), Large Egg (white), Duck Egg
                                180, 182, 444, // Egg (brown), Large Egg (brown), Duck Feather
                                184, 186, 430, // Milk, Large Milk, Truffle
                                436, 438, 440  // Goat Milk, Large Goat Milk, Wool
                            },
                            new int[] { },
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 3,
                                ShowValueColumn = true
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] 
                            {
                                176, 174, 180, 182, // Egg (white), Large Egg (white), Egg (brown), Large Egg (brown)
                                306, 442, 305, 107, // Mayonnaise, Duck Egg, Void Egg, Dinosaur Egg
                                184, 186, 436, 438, // Milk, Large Milk, Goat Milk, Large Goat Milk
                                424, 426, 430, 440, // Cheese, Goat Cheese, Truffle, Wool
                                444, 446            // Duck Feather, Rabbit's Foot
                            },
                            new int[] { 178, 428, 432, 307, 308, 807 }, // Hay, Cloth, Truffle Oil, Duck Mayonnaise, Void Mayonnaise, Dinosaur Mayonnaise
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 50.0 / DefaultCapacities[ContainerSize.Large],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 4,
                                ShowValueColumn = true
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[]
                            {
                                176, 174, 180, 182, // Egg (white), Large Egg (white), Egg (brown), Large Egg (brown)
                                306, 442, 305, 107, // Mayonnaise, Duck Egg, Void Egg, Dinosaur Egg
                                184, 186, 436, 438, // Milk, Large Milk, Goat Milk, Large Goat Milk
                                424, 426, 430, 440, // Cheese, Goat Cheese, Truffle, Wool
                                444, 446            // Duck Feather, Rabbit's Foot
                            },
                            new int[] 
                            {
                                178, 428, 432, 307, 308, 807,   // Hay, Cloth, Truffle Oil, Duck Mayonnaise, Void Mayonnaise, Dinosaur Mayonnaise
                                24, 16, 17, 19                  // Mayonnaise Machine, Cheese Press, Loom, Oil Maker
                            }, 
                            BigCraftableIds
                        ),
                        //CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 4,
                                ShowValueColumn = true
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 6,
                                LineBreakIndices = new int[] { 5 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[]
                            {
                                176, 174, 180, 182, // Egg (white), Large Egg (white), Egg (brown), Large Egg (brown)
                                306, 442, 305, 107, // Mayonnaise, Duck Egg, Void Egg, Dinosaur Egg
                                184, 186, 436, 438, // Milk, Large Milk, Goat Milk, Large Goat Milk
                                424, 426, 430, 440, // Cheese, Goat Cheese, Truffle, Wool
                                444, 446            // Duck Feather, Rabbit's Foot
                            },
                            new int[]
                            {
                                178, 428, 432, 307, 308, 807,   // Hay, Cloth, Truffle Oil, Duck Mayonnaise, Void Mayonnaise, Dinosaur Mayonnaise
                                24, 16, 17, 19                  // Mayonnaise Machine, Cheese Press, Loom, Oil Maker
                            },
                            BigCraftableIds
                        ),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 4,
                                ShowValueColumn = true
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 6,
                                LineBreakIndices = new int[] { 5 }
                            }
                        }
                    }
                }
            };
        }

        public static BagType GetRecycleBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 20 }; // Recycling Machine
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Krobus, BagSizeConfig.BagShop.Willy };
            double PriceMultiplier = 0.20;

            return new BagType()
            {
                Id = "4582c416-eb5a-4a73-ae94-da1eb0cbe027",
                Name = "Recycling Bag",
                Description = "A bag for storing recyclables and trash.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(3 * 16, 7 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[] { 167, 168, 169, 172 }, // Joja Cola, Trash, Driftwood, Soggy Newspaper
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 30.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[] { 167, 168, 169, 172, 170, 171 }, // Joja Cola, Trash, Driftwood, Soggy Newspaper, Broken Glasses, Broken CD
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 100.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[] { 167, 168, 169, 172, 170, 171, 338, 20 }, // Joja Cola, Trash, Driftwood, Soggy Newspaper, Broken Glasses, Broken CD, Refined Quartz, Recycling Machine
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier * 0.8), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[] { 167, 168, 169, 172, 170, 171, 338, 20 }, // Joja Cola, Trash, Driftwood, Soggy Newspaper, Broken Glasses, Broken CD, Refined Quartz, Recycling Machine
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier * 0.7), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[] { 167, 168, 169, 172, 170, 171, 338, 20 }, // Joja Cola, Trash, Driftwood, Soggy Newspaper, Broken Glasses, Broken CD, Refined Quartz, Recycling Machine
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    }
                }
            };
        }

        public static BagType GetLootBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { };
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Marlon, BagSizeConfig.BagShop.Dwarf };
            double PriceMultiplier = 0.85;

            return new BagType()
            {
                Id = "07b31f0d-1cf3-4e59-b581-915b185e77a4",
                Name = "Loot Bag",
                Description = "A bag for storing monster loot.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(1 * 16, 32 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[] 
                            {
                                766, 92, 684, 286,  // Slime, Sap, Bug Meat, Cherry Bomb
                                717, 273, 157, 96   // Crab, Rice Shoot, White Algae, Dwarf Scroll I
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                766, 92, 684, 767, 286, 287,  // Slime, Sap, Bug Meat, Bat Wing, Cherry Bomb, Bomb
                                768, 717, 273, 157, 96, 97   // Solar Essence, Crab, Rice Shoot, White Algae, Dwarf Scroll I/II
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 100.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                766, 92, 684, 767, 286, 287, 157, 338,  // Slime, Sap, Bug Meat, Bat Wing, Cherry Bomb, Bomb, White Algae, Refined Quartz
                                768, 769, 203, 717, 273, 96, 97, 98     // Solar Essence, Void Essence, Strange Bun, Crab, Rice Shoot, Dwarf Scroll I/II/III
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 500.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                766, 684, 767, 768, 769, 814, 96, 97, 98, 99,   // Slime, Bug Meat, Bat Wing, Solar Essence, Void Essence, Squid Ink, Dwarf Scroll I-IV
                                286, 287, 288, 203, 717, 157, 92, 273, 338      // Cherry Bomb, Bomb, Mega Bomb, Strange Bun, Crab, White Algae, Sap, Rice Shoot, Refined Quartz
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 10 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                766, 684, 767, 768, 769, 814, 96, 97,           // Slime, Bug Meat, Bat Wing, Solar Essence, Void Essence, Squid Ink, Dwarf Scroll I/II
                                286, 287, 288, 92, 273, 338, 98, 99,            // Cherry Bomb, Bomb, Mega Bomb, Sap, Rice Shoot, Refined Quartz, Dwarf Scroll III/IV
                                157, 717, 732, 203, 226, 243, 349, 773          // White Algae, Crab, Crab Cakes, Strange Bun, Spicy Eel, Miner's Treat, Energy Tonic, Life Elixir
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    }
                }
            };
        }

        public static BagType GetForagingBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { };
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Sandy, BagSizeConfig.BagShop.Pierre };
            double PriceMultiplier = 2.2;

            return new BagType()
            {
                Id = "040d414b-3a55-40d2-aa89-8121f4c0b387",
                Name = "Foraging Bag",
                Description = "A bag for storing foraged goods.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(22 * 16, 0 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            new int[] 
                            {
                                16, 396, 406, 414,  // Wild Horseradish, Spice Berry, Wild Plum, Crystal Fruit
                                18, 398, 408, 418,  // Daffodil, Grape, Hazelnut, Crocus
                                20, 402, 410, 283,  // Leek, Sweet Pea, Blackberry, Holly
                                22                  // Dandelion
                            },
                            new int[] { },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 4,
                                ShowValueColumn = true,
                                SlotSize = 64
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            new int[] 
                            {
                                16, 396, 406, 414,  // Wild Horseradish, Spice Berry, Wild Plum, Crystal Fruit
                                18, 398, 408, 418,  // Daffodil, Grape, Hazelnut, Crocus
                                20, 402, 410, 283,  // Leek, Sweet Pea, Blackberry, Holly
                                22, 393, 397, 394,  // Dandelion, Coral, Sea Urchin, Rainbow Shell
                                372, 718, 719, 723  // Clam, Cockle, Mussel, Oyster
                            },
                            new int[] { },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 4,
                                ShowValueColumn = true,
                                SlotSize = 64
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 60.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            new int[] 
                            {
                                16, 396, 406, 414, 372, 393,        // Wild Horseradish, Spice Berry, Wild Plum, Crystal Fruit, Clam, Coral
                                18, 398, 408, 418, 718, 397,        // Daffodil, Grape, Hazelnut, Crocus, Cockle, Sea Urchin
                                20, 402, 410, 283, 719, 394,        // Leek, Sweet Pea, Blackberry, Holly, Mussel, Rainbow Shell
                                22, 259, 412, 416, 723, 392,        // Dandelion, Fiddlehead Fern, Winter Root, Snow Yam, Oyster, Nautilus Shell
                                399, 296                            // Spring Onion, Salmonberry
                            },
                            new int[] { },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 6,
                                ShowValueColumn = true,
                                SlotSize = 64
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            new int[]
                            {
                                16, 396, 406, 414, 372, 404,    // Wild Horseradish, Spice Berry, Wild Plum, Crystal Fruit, Clam, Common Mushroom
                                18, 398, 408, 418, 718, 420,    // Daffodil, Grape, Hazelnut, Crocus, Cockle, Red Mushroom
                                20, 402, 410, 283, 719, 422,    // Leek, Sweet Pea, Blackberry, Holly, Mussel, Purple Mushroom
                                22, 259, 394, 412, 723, 257,    // Dandelion, Fiddlehead Fern, Rainbow Shell, Winter Root, Oyster, Morel
                                399, 90, 393, 416, 392, 281,    // Spring Onion, Cactus Fruit, Coral, Snow Yam, Nautilus Shell, Chanterelle
                                296, 88, 397                    // Salmonberry, Cocnut, Sea Urchin,
                            },
                            new int[] { },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 6,
                                ShowValueColumn = true,
                                SlotSize = 64
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            new int[]
                            {
                                16, 396, 406, 414, 372, 404,    // Wild Horseradish, Spice Berry, Wild Plum, Crystal Fruit, Clam, Common Mushroom
                                18, 398, 408, 418, 718, 420,    // Daffodil, Grape, Hazelnut, Crocus, Cockle, Red Mushroom
                                20, 402, 410, 283, 719, 422,    // Leek, Sweet Pea, Blackberry, Holly, Mussel, Purple Mushroom
                                22, 259, 394, 412, 723, 257,    // Dandelion, Fiddlehead Fern, Rainbow Shell, Winter Root, Oyster, Morel
                                399, 90, 393, 416, 392, 281,    // Spring Onion, Cactus Fruit, Coral, Snow Yam, Nautilus Shell, Chanterelle
                                296, 88, 397                    // Salmonberry, Cocnut, Sea Urchin,
                            },
                            new int[] { },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                GroupsPerRow = 6,
                                ShowValueColumn = true,
                                SlotSize = 64
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    }
                }
            };
        }

        public static BagType GetArtifactBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { };
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Krobus, BagSizeConfig.BagShop.Dwarf };
            double PriceMultiplier = 1.05;

            List<int> ArtifactIds = new List<int>()
            {
                96, 97, 98, 99, 114, 118, 110, 111, 112, 100, 101, 116,
                105, 113, 115, 120, 589, 103, 586, 109, 117, 119, 121, 123,
                579, 580, 581, 582, 583, 584, 585, 588, 587, 104, 122, 125,
                106, 108, 107, 124, 126, 127
            };

            return new BagType()
            {
                Id = "c47bd42a-dcfd-4070-a268-adc91c13d727",
                Name = "Artifact Bag",
                Description = "A bag for storing rare artifacts.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(7 * 16, 4 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            ArtifactIds.Take(ArtifactIds.Count - ((int)ContainerSize.Massive - (int)ContainerSize.Small) * 6).ToArray(),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            ArtifactIds.Take(ArtifactIds.Count - ((int)ContainerSize.Massive - (int)ContainerSize.Medium) * 6).ToArray(),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 60.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            ArtifactIds.Take(ArtifactIds.Count - ((int)ContainerSize.Massive - (int)ContainerSize.Large) * 6).ToArray(),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            ArtifactIds.Take(ArtifactIds.Count - ((int)ContainerSize.Massive - (int)ContainerSize.Giant) * 6).ToArray(),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            ArtifactIds.Take(ArtifactIds.Count - ((int)ContainerSize.Massive - (int)ContainerSize.Massive) * 6).ToArray(),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    }
                }
            };
        }

        public static BagType GetSeedBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 25 }; // Seed Maker
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Sandy, BagSizeConfig.BagShop.Pierre };
            double PriceMultiplier = 1.25;

            List<int> RegularSeeds = new List<int>()
            {
                770, 495, 496, 497, 498,                    // Mixed Seeds, Spring Seeds, Summer Seeds, Fall Seeds, Winter Seeds
                429, 477, 472, 475, 427,                    // Jazz Seeds, Kale Seeds, Parsnip Seeds, Potato Seeds, Tulip bulb
                487, 482, 453, 484, 455, 431, 480, 483,     // Corn Seeds, Pepper Seeds, Poppy Seeds, Radish Seeds, Spangle Seeds, Sunflower Seeds, Tomato Seeds, Wheat Seeds
                299, 494, 491, 488, 425, 492                // Amaranth Seeds, Beet Seeds, Bok Choy Seeds, Eggplant Seeds, Fairy Seeds, Yam Seeds
            };
            List<int> GoodSeeds = new List<int>() { 474, 481, 479, 493, 490 }; // Cauliflower Seeds, Blueberry Seeds, Melon Seeds, Cranberry Seeds, Pumpkin Seeds
            List<int> TrellisSeeds = new List<int>() { 473, 302, 301 }; // Bean Starter, Hops Starter, Grape Starter
            List<int> Year2Seeds = new List<int>() { 476, 273, 489, 485 }; // Garlic Seeds, Rice Shoot, Artichoke Seeds, Red Cabbage Seeds
            List<int> DesertSeeds = new List<int>() { 802, 494, 478, 486 }; // Cactus Seeds, Beet Seeds, Rhubarb Seeds, Starfruit Seeds
            List<int> SpecialSeeds = new List<int>() { 251, 433, 745, 499, 347 }; // Tea Sapling, Coffee Bean, Strawberry Seeds, Ancient Seeds, Rare Seed

            return new BagType()
            {
                Id = "7c79118b-09d3-4173-87f1-2809715e0983",
                Name = "Seed Bag",
                Description = "A bag for storing seeds.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(11 * 16, 14 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 30.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            RegularSeeds,
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 99.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            RegularSeeds.Union(TrellisSeeds).Union(GoodSeeds),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 300.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            RegularSeeds.Union(TrellisSeeds).Union(GoodSeeds).Union(DesertSeeds).Union(Year2Seeds),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            RegularSeeds.Union(TrellisSeeds).Union(GoodSeeds).Union(DesertSeeds).Union(Year2Seeds).Union(SpecialSeeds).Union(BigCraftableIds),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 9999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            new int[] { },
                            RegularSeeds.Union(TrellisSeeds).Union(GoodSeeds).Union(DesertSeeds).Union(Year2Seeds).Union(SpecialSeeds).Union(BigCraftableIds),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { }
                        }
                    }
                }
            };
        }

        public static BagType GetOceanFishBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 154 }; // Worm Bin
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Willy };
            double PriceMultiplier = .75;

            List<int> FishIds = new List<int>()
            {
                129, 147, 131, 150, // Anchovy, Herring, Sardine, Red Snapper
                146, 154, 701, 705, // Red Mullet, Sea Cucumber, Tilapia, Albacore
                151, 708, 148, 130, // Squid, Halibut, Eel, Tuna
                267, 149, 128, 155  // Flounder, Octopus, Pufferfish, Super Cucumber
            };

            List<int> MiscIds = new List<int>()
            {
                152, 153, 157, 685, 774, 154, 166 // Seaweed, Green Algae, White Algae, Bait, Wild Bait, Worm Bin, Treasure Chest
            };

            return new BagType()
            {
                Id = "66519acd-7f45-4091-b31f-b60997b3987e",
                Name = "Ocean Fish Bag",
                Description = "A bag for storing ocean fish.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(8 * 16, 5 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            FishIds.Take(8),
                            MiscIds.Take(4),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            FishIds.Take(12),
                            MiscIds.Take(5),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 5 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 50.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            FishIds.Take(16),
                            MiscIds.Take(6),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            FishIds,
                            MiscIds.Take(7),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 7 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 9999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            FishIds,
                            MiscIds,
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = MiscIds.Count }
                        }
                    }
                }
            };
        }

        public static BagType GetRiverFishBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 154 }; // Worm Bin
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Willy };
            double PriceMultiplier = 0.65;

            List<int> FishIds = new List<int>()
            {
                145, 132, 137, 702, // Sunfish, Bream, Smallmouth Bass, Chub
                706, 141, 138, 139, // Shad, Perch, Rainbow Trout, Salmon
                144, 704, 140, 707, // Pike, Dorado, Walleye, Lingcod
                699, 143,           // Tiger Trout, Catfish
            };

            List<int> MiscIds = new List<int>()
            {
                152, 153, 157, 685, 774, 154, 166 // Seaweed, Green Algae, White Algae, Bait, Wild Bait, Worm Bin, Treasure Chest
            };

            return new BagType()
            {
                Id = "74857d55-8889-4e62-b70e-05d4c7ae523d",
                Name = "River Fish Bag",
                Description = "A bag for storing river fish.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(1 * 16, 6 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            FishIds.Take(6),
                            MiscIds.Take(4),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            FishIds.Take(10),
                            MiscIds.Take(5),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 5 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 50.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            FishIds.Take(14),
                            MiscIds.Take(6),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            FishIds,
                            MiscIds.Take(7),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 7 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 9999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            FishIds,
                            MiscIds,
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = MiscIds.Count }
                        }
                    }
                }
            };
        }

        public static BagType GetLakeFishBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 154 }; // Worm Bin
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Willy };
            double PriceMultiplier = 0.45;

            List<int> FishIds = new List<int>()
            {
                142, 702, 141, 138, // Carp, Chub, Perch, Rainbow Trout
                700, 136, 140, 707, // Bullhead, Largemouth Bass, Walleye, Lingcod
                269, 698            // Midnight Carp, Sturgeon
            };

            List<int> MiscIds = new List<int>()
            {
                152, 153, 157, 685, 774, 154, 166 // Seaweed, Green Algae, White Algae, Bait, Wild Bait, Worm Bin, Treasure Chest
            };

            return new BagType()
            {
                Id = "9d23058a-ec74-4bdc-b118-547eeec6b002",
                Name = "Lake Fish Bag",
                Description = "A bag for storing mountain lake fish.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(16 * 16, 5 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            FishIds.Take(4),
                            MiscIds.Take(4),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            FishIds.Take(8),
                            MiscIds.Take(5),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 5 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 50.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            FishIds.Take(10),
                            MiscIds.Take(6),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            FishIds,
                            MiscIds.Take(7),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 7 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 9999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            FishIds,
                            MiscIds,
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = MiscIds.Count }
                        }
                    }
                }
            };
        }

        public static BagType GetMiscFishBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 154 }; // Worm Bin
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Willy };
            double PriceMultiplier = 0.55;

            List<int> FishIds = new List<int>()
            {
                156, 158, 161, 162, // Ghostfish, Stonefish, Ice Pip, Lava Eel
                734, 798, 799, 800, // Woodskip, Midnight Squid, Spook Fish, Blobfish
                164, 165, 795, 796, // Sandfish, Scorpion Carp, Void Salmon, Slimejack
            };

            List<int> MiscIds = new List<int>()
            {
                152, 153, 157, 685, 774, 154, 166 // Seaweed, Green Algae, White Algae, Bait, Wild Bait, Worm Bin, Treasure Chest
            };

            return new BagType()
            {
                Id = "64207326-abf1-49f8-a02e-c9d675dbc588",
                Name = "Miscellaneous Fish Bag",
                Description = "A bag for storing miscellaneous fish.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(21 * 16, 6 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            FishIds.Take(3),
                            MiscIds.Take(4),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 3 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            FishIds.Take(5),
                            MiscIds.Take(5),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 3 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 5 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 50.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            FishIds.Take(8),
                            MiscIds.Take(6),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            FishIds.Take(10),
                            MiscIds.Take(7),
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 7 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 9999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            FishIds,
                            MiscIds,
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = MiscIds.Count }
                        }
                    }
                }
            };
        }

        public static BagType GetFishBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 154 }; // Worm Bin
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Willy };
            double PriceMultiplier = 2.4;

            List<int> SeasonlessCommonFishIds = new List<int>() {
                142, 132, 702, 700, 136 // Carp, Bream, Chub, Bullhead, Largemouth Bass
            };
            List<int> SpringFishIds = new List<int>() {
                129, 147, 145, 131, 137, 706, 708, 148, 267, 143 // Anchovy, Herring, Sunfish, Sardine, Smallmouth Bass, Shad, Halibut, Eel, Flounder, Catfish
            };
            List<int> SummerFishIds = new List<int>() {
                150, 138, 146, 701, 130, 704, 144, 149, 698, 128, 155 // Red Snapper, Rainbow Trout, Red Mullet, Tilapia, Tuna, Dorado, Pike, Octopus, Sturgeon, Pufferfish, Super Cucumber
            };
            List<int> FallFishIds = new List<int>() {
                154, 705, 139, 140, 269, 699 // Sea Cucumber, Albacore, Salmon, Walleye, Midnight Carp, Tiger Trout
            };
            List<int> WinterFishIds = new List<int>() {
                141, 151, 707 // Perch, Squid, Lingcod
            };
            List<int> CrabPotFishIds = new List<int>() {
                722, 719, 723, 372, 718, 720, 721, 716, 717, 715 // Periwinkle, Mussel, Oyster, Clam, Cockle, Shrimp, Snail, Crayfish, Crab, Lobster
            };
            List<int> MineFishIds = new List<int>() {
                156, 158, 161, 162 // Ghostfish, Stonefish, Ice Pip, Lava Eel
            };
            List<int> DesertFishIds = new List<int>() {
                164, 165 // Sandfish, Scorpion Carp
            };
            List<int> ForestFishIds = new List<int>() {
                734 // Woodskip
            };
            List<int> NightMarketFishIds = new List<int>() {
                798, 799, 800 // Midnight Squid, Spook Fish, Blobfish
            };
            List<int> SpecialFishIds = new List<int>() {
                796, 795 // Slimejack, Void Salmon
            };
            List<int> LegendaryFishIds = new List<int>() {
                160, 775, 682, 159, 163 // Angler, Glacierfish, Mutant Carp, Crimsonfish, Legend
            };

            return new BagType()
            {
                Id = "62e478ee-9d5d-4b88-a34d-c9f490db8c6c",
                Name = "Fish Bag",
                Description = "A bag for storing fish.",
                IconSourceTexture = BagType.SourceTexture.Tools,
                IconSourceRect = new Rectangle(176, 0, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        Items = CreateStoreableItemList(
                            SeasonlessCommonFishIds.Union(SpringFishIds).Union(MineFishIds.Take(1)),
                            new List<int>() { 152, 153, 157, 685 }, // Seaweed, Green Algae, White Algae, Bait
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                ShowValueColumn = false,
                                SlotSize = 64,
                                GroupsPerRow = 4
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 4 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 25.0 / DefaultCapacities[ContainerSize.Medium],
                        Items = CreateStoreableItemList(
                            CrabPotFishIds.Union(SeasonlessCommonFishIds).Union(SpringFishIds).Union(SummerFishIds).Union(MineFishIds.Take(2)),
                            new List<int>() { 152, 153, 157, 685, 710, 154, 219 }, // Seaweed, Green Algae, White Algae, Bait, Crab Pot, Worm Bin, Trout Soup
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                ShowValueColumn = false,
                                SlotSize = 64,
                                GroupsPerRow = 6
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 7 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 50.0 / DefaultCapacities[ContainerSize.Large],
                        Items = CreateStoreableItemList(
                            CrabPotFishIds.Union(SeasonlessCommonFishIds).Union(SpringFishIds).Union(SummerFishIds).Union(FallFishIds).Union(WinterFishIds)
                            .Union(ForestFishIds).Union(DesertFishIds).Union(MineFishIds.Take(3)),
                            new List<int>() { 152, 153, 157, 685, 774, 710, 154, 219, 729, 213 }, // Seaweed, Green Algae, White Algae, Bait, Wild Bait, Crab Pot, Worm Bin, Trout Soup, Escargot, Fish Taco
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                ShowValueColumn = false,
                                SlotSize = 64,
                                GroupsPerRow = 8
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 10 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        Items = CreateStoreableItemList(
                            CrabPotFishIds.Union(SeasonlessCommonFishIds).Union(SpringFishIds).Union(SummerFishIds).Union(FallFishIds).Union(WinterFishIds)
                            .Union(ForestFishIds).Union(DesertFishIds).Union(MineFishIds).Union(NightMarketFishIds).Union(SpecialFishIds),
                            new List<int>() {
                                152, 153, 157, 685, 774, 710, 154, 166, // Seaweed, Green Algae, White Algae, Bait, Wild Bait, Crab Pot, Worm Bin, Treasure Chest
                                219, 729, 213, 242, 728, 730 // Trout Soup, Escargot, Fish Taco, Dish O' the Sea, Fish Stew, Lobster Bisque
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                ShowValueColumn = false,
                                SlotSize = 64,
                                GroupsPerRow = 8
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        //CapacityMultiplier = 9999.0 / DefaultCapacities[ContainerSize.Massive],
                        Items = CreateStoreableItemList(
                            CrabPotFishIds.Union(SeasonlessCommonFishIds).Union(SpringFishIds).Union(SummerFishIds).Union(FallFishIds).Union(WinterFishIds)
                            .Union(ForestFishIds).Union(DesertFishIds).Union(MineFishIds).Union(NightMarketFishIds).Union(SpecialFishIds).Union(LegendaryFishIds),
                            new List<int>() {
                                152, 153, 157, 685, 774, 710, 154, 166, // Seaweed, Green Algae, White Algae, Bait, Wild Bait, Crab Pot, Worm Bin, Treasure Chest
                                219, 729, 213, 242, 728, 730, 265 // Trout Soup, Escargot, Fish Taco, Dish O' the Sea, Fish Stew, Lobster Bisque, Seafoam Pudding
                            },
                            BigCraftableIds
                        ),
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                            {
                                ShowValueColumn = false,
                                SlotSize = 64,
                                GroupsPerRow = 8
                            },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    }
                }
            };
        }

        public static BagType GetFarmerBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() {
                8, 110, 113, 126, 136, 137, 138, 139, 140, 167, // Scarecrow, Rarecrows 1-8, Deluxe Scarecrow
                25 // Seed Maker
            };
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Marnie };
            double PriceMultiplier = 0.35;

            //Fertilizers: Basic Fertilizer 368, Quality Fertilizer 369, Basic Retaining Soil 370, Quality Retaining Soil 371, Speed-Gro 465, Deluxe Speed-Gro 466, Tree Fertilizer 805
            //Scarecrows: Scarecrow 8, Rarecrows 110, 113, 126, 136, 137, 138, 139, 140, Deluxe Scarecrow 167
            //Sprinklers: Sprinkler 599, Quality Sprinkler 621, Iridium Sprinkler 645
            //Totems: Warp Totem: Farm 688, Warp Totem: Mountains 689, Warp Totem: Beach 690, Warp Totem: Desert 261, Rain Totem 681
            //Foods: Hashbrowns 210, Pepper Poppers 215, Tom Kha Soup 218, Complete Breakfast 201, Farmer's Lunch 240
            //Clay 330

            return new BagType()
            {
                Id = "49d045ab-47d8-47fc-aed0-745da4a6d8fa",
                Name = "Farmers Bag",
                Description = "A bag for storing sprinklers, fertilizers, and scarecrows.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(8 * 16, 15 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] { }, 
                            new int[] { 8, 368, 370, 465, 599, 330 }, // Scarecrow, Basic Fertilizer, Basic Retaining Soil, Basic Speed-Gro, Sprinkler, Clay
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 6 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[] 
                            {
                                8, 110, 113, 136, 137, 138, 139, 140, // Scarecrow, Rarecrows 1-8 except 3
                                368, 370, 371, 465, 805, 599, 621, 330 // Basic Fertilizer, Basic Retaining Soil, Quality Retaining Soil, Speed-Gro, Tree Fertilizer, Sprinkler, Quality Sprinkler, Clay
                            },
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 30.0 / DefaultCapacities[ContainerSize.Medium],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 8 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers.ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                8, 110, 113, 126, 136, 137, 138, 139, 140, // Scarecrow, Rarecrows 1-8
                                368, 369, 370, 371, 465, 466, 805, 599, 621, 645, // Basic Fertilizer, Quality Fertilizer, Basic Retaining Soil, Quality Retaining Soil, Speed-Gro, Deluxe Speed-Gro, Tree Fertilizer, Sprinkler, Quality Sprinkler, Iridium Sprinkler
                                25, 688, 681, 330, 210 // Seed Maker, Warp Totem: Farm, Rain Totem, Clay, Hashbrowns
                            },
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 100.0 / DefaultCapacities[ContainerSize.Large],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 10,
                                LineBreakIndices = new int[] { 8, 18 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                8, 110, 113, 126, 136, 137, 138, 139, 140, 167, // Scarecrow, Rarecrows 1-8, Deluxe Scarecrow
                                368, 369, 370, 371, 465, 466, 805, 599, 621, 645, // Basic Fertilizer, Quality Fertilizer, Basic Retaining Soil, Quality Retaining Soil, Speed-Gro, Deluxe Speed-Gro, Tree Fertilizer, Sprinkler, Quality Sprinkler, Iridium Sprinkler
                                688, 689, 690, 261, 681, // Warp Totem: Farm, Warp Totem: Mountains, Warp Totem: Beach, Warp Totem: Desert, Rain Totem
                                210, 215, 218, 201, // Hashbrowns, Pepper Poppers, Tom Kha Soup, Complete Breakfast
                                25, 330 // Seed Maker, Clay
                            },
                            BigCraftableIds
                        ),
                        //CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Giant],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 10 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] { },
                            new int[]
                            {
                                8, 110, 113, 126, 136, 137, 138, 139, 140, 167, // Scarecrow, Rarecrows 1-8, Deluxe Scarecrow
                                368, 369, 370, 371, 465, 466, 805, 599, 621, 645, // Basic Fertilizer, Quality Fertilizer, Basic Retaining Soil, Quality Retaining Soil, Speed-Gro, Deluxe Speed-Gro, Tree Fertilizer, Sprinkler, Quality Sprinkler, Iridium Sprinkler
                                688, 689, 690, 261, 681, // Warp Totem: Farm, Warp Totem: Mountains, Warp Totem: Beach, Warp Totem: Desert, Rain Totem
                                210, 215, 218, 201, 240, // Hashbrowns, Pepper Poppers, Tom Kha Soup, Complete Breakfast, Farmer's Lunch
                                25, 330 // Seed Maker, Clay
                            },
                            BigCraftableIds
                        ),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 10 }
                        }
                    }
                }
            };
        }

        public static BagType GetFoodBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { 12 }; // Keg
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Gus };
            double PriceMultiplier = 0.55;

            List<int> HasQualitiesIds = new List<int>() { 346, 459, 303 }; // Beer, Mead, Pale Ale

            List<int> Ingredients = new List<int>() {
                246, 245, 247, 419, 724, 814 // Wheat Flour, Sugar, Oil, Vinegar, Rice, Maple Syrup, Squid Ink
            };

            List<int> CraftedFoods = new List<int>() {
                403, 773, 772 // Field Snack, Life Elixir, Oil of Garlic
            };

            List<int> Medicines = new List<int>() {
                349, 351 // Energy Tonic, Muscle Remedy
            };

            List<int> Drinks = new List<int>() {
                395, 253, 346, 459, 303, 12 // Coffee, Triple Shot Espresso, Beer, Mead, Pale Ale, Keg
            };

            List<int> Dishes = new List<int>() {
                194, 229, 216, 227, 211, 198, // Fried Egg, Tortilla, Bread, Sashimi, Pancakes, Baked Fish
                207, 219, 244, 456, 196, 199, // Bean Hotpot, Trout Soup, Roots Platter, Algae Soup, Salad, Parsnip Soup
                200, 210, 224, 225, 233, 238, // Vegetable Medley, Hashbrowns, Spaghetti, Fried Eel, Ice Cream, Cranberry Sauce
                195, 729, 727, 223, 202, 209, // Omelet, Escargot, Chowder, Cookie, Fried Calamari, Carp Surprise
                214, 234, 240, 457, 733, 239, // Crispy Bass, Blueberry Tart, Farmer's Lunch, Pale Broth, Shrimp Cocktail, Stuffing
                226, 612, 728, 241, 205, 208, // Spicy Eel, Cranberry Candy, Fish Stew, Survival Burger, Fried Mushroom, Glazed Yams
                215, 220, 231, 243, 730, 605, // Pepper Poppers, Chocolate Cake, Eggplant Parmesan, Miner's Treat, Lobster Bisque, Artichoke Dip
                618, 228, 237, 242, 203, 204, // Bruschetta, Maki Roll, Super Meal, Dish O' The Sea, Strange Bun, Lucky Lunch
                218, 651, 232, 604, 611, 607, // Tom Kha Soup, Poppyseed Muffin, Rice Pudding, Plum Pudding, Blackberry Cobbler, Roasted Hazelnuts
                732, 197, 206, 212, 236, 265, // Crab Cakes, Cheese Cauliflower, Pizza, Salmon Dinner, Pumpkin Soup, Seafoam Pudding
                609, 731, 606, 648, 201, 235, // Radish Salad, Maple Bar, Stir Fry, Coleslaw, Complete Breakfast, Autumn's Bounty
                649, 608, 222, 230, 610, 221, // Fiddlehead Risotto, Pumpkin Pie, Rhubarb Pie, Red Plate, Fruit Salad, Pink Cake
                213 // Fish Taco
            };

            List<int> MiscFoods = new List<int>() {
                78 // Cave Carrot
            };

            return new BagType()
            {
                Id = "f2d4a639-53ab-4124-a80d-c59b1ce67a4b",
                Name = "Food Bag",
                Description = "A bag for storing cooked dishes and ingredients.",
                IconSourceTexture = BagType.SourceTexture.SpringObjects,
                IconSourceRect = new Rectangle(4 * 16, 8 * 16, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] { },
                            Dishes.Take(16).Union(Ingredients.Take(4)),
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 10.0 / DefaultCapacities[ContainerSize.Small],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 12 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            new int[] { },
                            Dishes.Take(32).Union(Drinks.Take(2)).Union(CraftedFoods).Union(Ingredients.Take(6)),
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 30.0 / DefaultCapacities[ContainerSize.Medium],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout() { Columns = 16 }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers.ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = 
                            Dishes.Take(48).Union(Drinks.Take(5)).Union(CraftedFoods).Union(Medicines).Union(Ingredients)
                            .Select(x => new StoreableBagItem(x, HasQualitiesIds.Contains(x), null, BigCraftableIds.Contains(x))).ToList(),
                        CapacityMultiplier = 100.0 / DefaultCapacities[ContainerSize.Large],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 16,
                                LineBreakIndices = new int[] { 15, 31, 47, 61 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items =
                            Dishes.Take(64).Union(Drinks).Union(CraftedFoods).Union(Medicines).Union(MiscFoods).Union(Ingredients)
                            .Select(x => new StoreableBagItem(x, HasQualitiesIds.Contains(x), null, BigCraftableIds.Contains(x))).ToList(),
                        CapacityMultiplier = 300.0 / DefaultCapacities[ContainerSize.Giant],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 16,
                                LineBreakIndices = new int[] { 15, 31, 47, 63, 78 }
                            }
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items =
                            Dishes.Union(Drinks).Union(CraftedFoods).Union(Medicines).Union(MiscFoods).Union(Ingredients)
                            .Select(x => new StoreableBagItem(x, HasQualitiesIds.Contains(x), null, BigCraftableIds.Contains(x))).ToList(),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = false,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                            {
                                Columns = 16,
                                LineBreakIndices = new int[] { 15, 31, 47, 63, Dishes.Count - 1, Dishes.Count - 1 + 15 }
                            }
                        }
                    }
                }
            };
        }

        public static BagType GetCropBagType()
        {
            HashSet<int> BigCraftableIds = new HashSet<int>() { };
            List<BagSizeConfig.BagShop> DefaultSellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Sandy, BagSizeConfig.BagShop.Pierre };
            double PriceMultiplier = 2.00;

            List<int> SpringCrops = new List<int>() {
                433, 591, 271, 24, 188, 597, // Coffee Bean, Tulip, Unmilled Rice, Parsnip, Green Bean, Blue Jazz
                248, 192, 90, 250, 190, 252  // Garlic, Potato, Cactus Fruit, Kale, Cauliflower, Rhubarb
            };

            List<int> SummerCrops = new List<int>() {
                304, 262, 260, 258, 270, 256,   // Hops, Wheat, Hot Pepper, Blueberry, Corn, Tomato
                421, 264, 593, 376, 254         // Sunflower, Radish, Summer Spangle, Poppy, Melon 
            };

            List<int> FallCrops = new List<int>() {
                272, 278, 282, 398, 284, // Eggplant, Bok Choy, Cranberries, Grape, Beet
                300, 274, 280, 595, 276  // Amaranth, Artichoke, Yam, Fairy Rose, Pumpkin
            };

            List<int> SpecialCrops = new List<int>() {
                400, 266, 268, 454, 417 // Strawberry, Red Cabbage, Starfruit, Ancient Fruit, Sweet Gem Berry
            };            

            return new BagType()
            {
                Id = "f05f7a2a-1c68-4f87-9bc9-10a13856b9bc",
                Name = "Crop Bag",
                Description = "A bag for storing farmed fruits, vegetables, and flowers.",
                IconSourceTexture = BagType.SourceTexture.Tools,
                IconSourceRect = new Rectangle(80, 32, 16, 16),
                SizeSettings = new BagSizeConfig[]
                {
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Small,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Small] * PriceMultiplier * 0.6), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            SpringCrops.ToArray(),
                            new int[] { },
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 20.0 / DefaultCapacities[ContainerSize.Small],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout(),
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Medium,
                        Sellers = DefaultSellers,
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Medium] * PriceMultiplier * 0.75), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            SpringCrops.Union(SummerCrops).ToArray(),
                            new int[] { },
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 60.0 / DefaultCapacities[ContainerSize.Medium],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 4 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Large,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Large] * PriceMultiplier * 0.75), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            SpringCrops.Union(SummerCrops).Union(FallCrops).ToArray(),
                            new int[] { },
                            BigCraftableIds
                        ),
                        CapacityMultiplier = 200.0 / DefaultCapacities[ContainerSize.Large],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 5 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Giant,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Giant] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            SpringCrops.Union(SummerCrops).Union(FallCrops).Union(SpecialCrops).ToArray(),
                            new int[] { },
                            BigCraftableIds
                        ),
                        //CapacityMultiplier = 300.0 / DefaultCapacities[ContainerSize.Giant],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 5 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                        }
                    },
                    new BagSizeConfig()
                    {
                        Size = ContainerSize.Massive,
                        Sellers = DefaultSellers.Take(1).ToList(),
                        Price = ItemBag.RoundIntegerToSecondMostSignificantDigit((int)(DefaultPrices[ContainerSize.Massive] * PriceMultiplier), ItemBag.RoundingMode.Floor),
                        Items = CreateStoreableItemList(
                            SpringCrops.Union(SummerCrops).Union(FallCrops).Union(SpecialCrops).ToArray(),
                            new int[] { },
                            BigCraftableIds
                        ),
                        //CapacityMultiplier = 999.0 / DefaultCapacities[ContainerSize.Massive],
                        MenuOptions = new BagMenuOptions()
                        {
                            GroupByQuality = true,
                            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() { GroupsPerRow = 5 },
                            UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                        }
                    }
                }
            };
        }

        //If you create more BagTypes, make sure you add them to BagConfig.InitializeDefaults()

        /// <param name="CraftableIds">Optional. If any of these Ids are in <paramref name="UngroupedIds"/>, 
        /// then the corresponding ungrouped items will have <see cref="StoreableBagItem.IsBigCraftable"/>=true</param>
        private static List<StoreableBagItem> CreateStoreableItemList(IEnumerable<int> GroupedIds, IEnumerable<int> UngroupedIds, IEnumerable<int> CraftableIds = null)
        {
            List<StoreableBagItem> Result = new List<StoreableBagItem>();
            if (GroupedIds != null)
                Result.AddRange(GroupedIds.Select(x => new StoreableBagItem(x, true, null, false)));
            if (UngroupedIds != null)
                Result.AddRange(UngroupedIds.Select(x => new StoreableBagItem(x, false, null, CraftableIds != null && CraftableIds.Contains(x))));
            return Result.ToList();
        }

        /// <summary>Creates an array containing exactly 1 BagSizeConfig object for each ContainerSize.<para/>
        /// The resulting price will be rounded down to 2nd most significant digit's value. 
        /// For example, 874 is rounded down to nearest 10 = 870, while 16446 is rounded down to nearest 1000 = 16000.</summary>
        /// <param name="PriceMultiplier">A multiplier that is applied to <see cref="DefaultPrices"/> to determine the bag's price.</param>
        /// <param name="Action">An additional delegate to invoke on each BagSizeConfig. Useful for fine-tuning extra settings on specific sizes.</param>
        private static BagSizeConfig[] GenerateSizeConfigs(BagSizeConfig.BagShop[] Sellers, BagMenuOptions MenuOptions, IEnumerable<StoreableBagItem> Items, double PriceMultiplier, Action<BagSizeConfig> Action)
        {
            List<BagSizeConfig> Configs = new List<BagSizeConfig>();
            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
            {
                int BasePrice = (int)(DefaultPrices[Size] * PriceMultiplier);
                int RoundedPrice = ItemBag.RoundIntegerToSecondMostSignificantDigit(BasePrice, ItemBag.RoundingMode.Floor);

                BagSizeConfig Config = new BagSizeConfig()
                {
                    Size = Size,
                    Price = RoundedPrice,
                    Sellers = new List<BagSizeConfig.BagShop>(Sellers),
                    MenuOptions = MenuOptions.GetCopy(),
                    Items = Items.ToList()
                };

                Action(Config);
                Configs.Add(Config);
            }
            return Configs.ToArray();
        }
    }
}
