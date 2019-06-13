using HardcoreBundles.Perks;
using StardewValley;
using System.Collections.Generic;
using C = HardcoreBundles.Constants;

namespace HardcoreBundles
{
    partial class Bundles
    {
        // This is the master list of bundles...
        // Only add new bundles to end of list, regardless of room.
        // Never remove bundles outright. No promises what happens with ids.
        public static IList<BundleModel> MakeList()
        {
            return new List<BundleModel>
        {
            new BundleModel(CraftRoom, "BasicForage",
                new BundleItem(C.WildHorseradish, 50, Gold),
                new BundleItem(C.Daffodil, 50, Gold),
                new BundleItem(C.Leek, 50, Gold),
                new BundleItem(C.Dandelion, 50, Gold),
                new BundleItem(C.SpiceBerry, 50, Gold),
                new BundleItem(C.Grape, 50, Gold),
                new BundleItem(C.SweetPea, 50, Gold),
                new BundleItem(C.WildPlum, 50, Gold),
                new BundleItem(C.Hazelnut, 50, Gold),
                new BundleItem(C.Blackberry, 50, Gold),
                new BundleItem(C.WinterRoot, 50, Gold),
                new BundleItem(C.CrystalFruit, 50, Gold)
            ){
                PerkDesc = "Both lvl 5 foraging perks",
                Perk = new ProfessionPerk((f) => f.ForagingLevel, 5, Farmer.tiller, Farmer.rancher )
            },

            new BundleModel(CraftRoom, "RareForage",
                new BundleItem(C.SpringOnion, 50, Gold),
                new BundleItem(C.Salmonberry, 50, Gold),
                new BundleItem(C.FiddleheadFern, 50, Gold),
                new BundleItem(C.SnowYam, 50, Gold),
                new BundleItem(C.Crocus, 50, Gold),
                new BundleItem(C.Holly, 50, Gold),
                new BundleItem(C.Coconut, 50, Gold),
                new BundleItem(C.CactusFruit, 50, Gold),
                new BundleItem(C.NautilusShell, 50, Gold),
                new BundleItem(C.Coral, 50, Gold),
                new BundleItem(C.RainbowShell, 50, Gold),
                new BundleItem(C.SeaUrchin, 50, Gold)
            ){
                PerkDesc = "All 4 lvl 10 foraging perks"
            },

            new BundleModel(CraftRoom,"Construction",
                new BundleItem(C.Wood, 999),
                new BundleItem(C.Wood, 999),
                new BundleItem(C.Wood, 999),
                new BundleItem(C.Hardwood, 500),
                new BundleItem(C.Stone, 999),
                new BundleItem(C.Stone, 999),
                new BundleItem(C.Fiber, 999),
                new BundleItem(C.Sap, 999),
                new BundleItem(C.Clay, 300),
                new BundleItem(C.CopperBar, 50)
            ){
                PerkDesc = "Half off material costs at robin's"
            },

            new BundleModel(CraftRoom,"DankForage",
                new BundleItem(C.Morel, 50, Gold),
                new BundleItem(C.CommonMushroom, 50, Gold),
                new BundleItem(C.RedMushroom, 50, Gold),
                new BundleItem(C.Chanterelle, 50, Gold),
                new BundleItem(C.PurpleMushroom, 50, Gold),
                new BundleItem(C.CaveCarrot, 150)
            ){
                PerkDesc = "Plantable mushroom trees"
            },

            new BundleModel(CraftRoom, "Overachievers",
                // These are unobtainable items given for certain achievements in the game
                new BundleItem(C.LuckyPurpleShorts, 1, Iridium), // todo: buy in casino
                new BundleItem(C.Bouquet, 1, Iridium), // todo: all friendships maxed
                new BundleItem(C.LuckyLunch, 1, Iridium), // todo: cook every recipe
                new BundleItem(C.ChickenStatue, 1, Iridium), // todo: full museum collection
                new BundleItem(C.Hardwood, 1, Iridium), // todo: fully upgraded house
                new BundleItem(C.PrehistoricTibia, 1, Iridium), //todo: pet loves you
                new BundleItem(C.Torch, 1, Iridium), // todo: 4 candles
                new BundleItem(C.JojaCola, 1, Iridium), // todo: 50M lifetime earnings
                new BundleItem(C.CactusSeeds, 1, Iridium), // todo: Beat prarie king
                new BundleItem(C.MinersTreat, 1, Iridium), // todo: all combat challenges
                new BundleItem(C.GoldenRelic, 1, Iridium) //todo: 50 help wanted quests
            ){
                PerkDesc = "All Skills can level up 5 more levels"
            },

            new BundleModel(CraftRoom,"Friendship",
                new BundleItem(C.SalmonDinner, 10),
                new BundleItem(C.Pomegranate, 10, Iridium),
                new BundleItem(C.Pickles, 10),
                new BundleItem(C.CactusFruit, 10, Gold),
                new BundleItem(C.Obsidian, 10),
                new BundleItem(C.PepperPoppers, 10),
                new BundleItem(C.Amethyst, 10),
                new BundleItem(C.Cloth, 10),
                new BundleItem(C.PinkCake, 10),
                new BundleItem(C.Wine, 10),
                new BundleItem(C.BatteryPack, 10),
                new BundleItem(C.Melon, 10, Gold)
            ){
                PerkDesc = "Gift influence doubled"
            },

            new BundleModel(Pantry, "SpringCrops",
                new BundleItem(C.Cauliflower, 150, Gold),
                new BundleItem(C.CoffeeBean, 150, Gold),
                new BundleItem(C.Garlic, 150, Gold),
                new BundleItem(C.GreenBean, 150, Gold),
                new BundleItem(C.Kale, 150, Gold),
                new BundleItem(C.Parsnip, 200, Gold),
                new BundleItem(C.Potato, 150, Gold),
                new BundleItem(C.Rhubarb, 150, Gold),
                new BundleItem(C.Strawberry, 150, Gold),
                new BundleItem(C.AncientFruit, 150, Gold),
                new BundleItem(C.FarmersLunch, 15)
            ){
                PerkDesc = "Both lvl 5 farming perks"
            },

            new BundleModel(Pantry, "SummerCrops",
                new BundleItem(C.Blueberry, 200, Gold),
                new BundleItem(C.Corn, 150, Gold),
                new BundleItem(C.Hops, 200, Gold),
                new BundleItem(C.HotPepper, 150, Gold),
                new BundleItem(C.Melon, 150, Gold),
                new BundleItem(C.Radish, 150, Gold),
                new BundleItem(C.RedCabbage, 150, Gold),
                new BundleItem(C.Starfruit, 150, Gold),
                new BundleItem(C.Tomato, 150, Gold),
                new BundleItem(C.Wheat, 150, Gold)
            ){
                PerkDesc = "Giant Crop chance increased"
            },

            new BundleModel(Pantry, "FallCrops",
                new BundleItem(C.Amaranth, 150, Gold),
                new BundleItem(C.Artichoke, 150, Gold),
                new BundleItem(C.Beet, 150, Gold),
                new BundleItem(C.BokChoy, 150, Gold),
                new BundleItem(C.Cranberries, 150, Gold),
                new BundleItem(C.Eggplant, 150, Gold),
                new BundleItem(C.Grape, 150, Gold),
                new BundleItem(C.Pumpkin, 150, Gold),
                new BundleItem(C.Yam, 150, Gold),
                new BundleItem(C.SweetGemBerry, 150, Gold)
            ){
                PerkDesc = "No More Crows"
            },

            new BundleModel(Bulletin, "DarkMagic",
                new BundleItem(C.VoidEssence, 50),
                new BundleItem(C.VoidEgg, 100),
                new BundleItem(C.VoidMayonnaise, 100),
                new BundleItem(C.VoidSalmon, 100),
                new BundleItem(C.AncientDoll, 1),
                new BundleItem(C.AncientSword, 1),
                new BundleItem(C.AncientDrum, 1),
                new BundleItem(C.StrangeDollA, 1)
            ){
                PerkDesc = "No unlucky days"
            },

            new BundleModel(Pantry, "Orchard",
                new BundleItem(C.Apricot, 60, Iridium),
                new BundleItem(C.Apple, 60, Iridium),
                new BundleItem(C.Orange, 60, Iridium),
                new BundleItem(C.Peach, 60, Iridium),
                new BundleItem(C.Pomegranate, 60, Iridium),
                new BundleItem(C.Cherry, 60, Iridium),
                new BundleItem(C.PineTar, 50),
                new BundleItem(C.OakResin, 50),
                new BundleItem(C.MapleSyrup, 50)
            ){
                PerkDesc = "All crops give extra harvest"
            },

            new BundleModel(Pantry, "Animal",
                new BundleItem(C.LargeWhiteEgg, 50, Iridium),
                new BundleItem(C.LargeBrownEgg, 50, Iridium),
                new BundleItem(C.LargeMilk, 50, Iridium),
                new BundleItem(C.LargeGoatMilk, 50, Iridium),
                new BundleItem(C.Truffle, 50, Gold),
                new BundleItem(C.Wool, 50, Iridium),
                new BundleItem(C.DuckEgg, 50, Iridium),
                new BundleItem(C.VoidEgg, 50, Iridium),
                new BundleItem(C.DuckFeather, 30, Gold),
                new BundleItem(C.RabbitsFoot, 15, Gold)
            ){
                PerkDesc = "Animals auto-petted every morning"
            },

            new BundleModel(Pantry, "Artisan",
                new BundleItem(C.Cheese, 50, Iridium),
                new BundleItem(C.GoatCheese, 50, Iridium),
                new BundleItem(C.Mayonnaise, 50, Gold),
                new BundleItem(C.DuckMayonnaise, 50, Gold),
                new BundleItem(C.VoidMayonnaise, 50, Gold),
                new BundleItem(C.Cloth, 50),
                new BundleItem(C.Honey, 100),
                new BundleItem(C.Pickles, 100),
                new BundleItem(C.Jelly, 100)
            ){
                PerkDesc = "All 4 lvl 10 farming perks"
            },

            new BundleModel(Bulletin, "Chefs",
                new BundleItem(C.CompleteBreakfast, 50),
                new BundleItem(C.Pancakes, 50),
                new BundleItem(C.PepperPoppers, 50),
                new BundleItem(C.SpicyEel, 50),
                new BundleItem(C.SuperMeal, 50),
                new BundleItem(C.CrabCakes, 50),
                new BundleItem(C.LobsterBisque, 50),
                new BundleItem(C.FruitSalad, 50),
                new BundleItem(C.EggplantParmesan, 50),
                new BundleItem(C.PumpkinSoup, 50),
                new BundleItem(C.GlazedYams, 50),
                new BundleItem(C.Omelet, 50)
            ){
                PerkDesc = "All recipes yield double"
            },

            new BundleModel(Bulletin, "Bakers",
                new BundleItem(C.BlueberryTart, 50),
                new BundleItem(C.ChocolateCake, 50),
                new BundleItem(C.PinkCake, 50),
                new BundleItem(C.PumpkinPie, 50),
                new BundleItem(C.RhubarbPie, 50),
                new BundleItem(C.StrangeBun, 50),
                new BundleItem(C.Bread, 50),
                new BundleItem(C.Cookie, 50),
                new BundleItem(C.BlackberryCobbler, 50),
                new BundleItem(C.PoppyseedMuffin, 50),
                new BundleItem(C.MapleBar, 50),
                new BundleItem(C.Pizza, 50)
            ){
                PerkDesc = "All recipes yield gold quality dishes"
            },

            new BundleModel(Bulletin, "Florists",
                new BundleItem(C.BlueJazz, 50, Gold),
                new BundleItem(C.Tulip, 50, Gold),
                new BundleItem(C.Poppy, 50, Gold),
                new BundleItem(C.SummerSpangle, 50, Gold),
                new BundleItem(C.Sunflower, 100, Gold),
                new BundleItem(C.FairyRose, 100, Gold),
                new BundleItem(C.Bouquet, 20),
                new BundleItem(C.Tulip, 50, Gold),
                new BundleItem(C.Poppy, 50, Gold),
                new BundleItem(C.SummerSpangle, 50, Gold),
                new BundleItem(C.BlueJazz, 50, Gold),
                new BundleItem(C.Honey, 100)
            ){
                PerkDesc = "All village shops open earier"
            },

            new BundleModel(Bulletin, "Colorful",
                new BundleItem(C.PrismaticShard, 2),
                new BundleItem(C.RainbowShell, 30, Gold),
                new BundleItem(C.PurpleMushroom, 30, Gold),
                new BundleItem(C.SnowYam, 30, Gold),
                new BundleItem(C.SeaUrchin, 30, Gold),
                new BundleItem(C.MinersTreat, 30),
                new BundleItem(C.IceCream, 30),
                new BundleItem(C.RedPlate, 30),
                new BundleItem(C.Cranberries, 200, Silver),
                new BundleItem(C.Orange, 100, Silver)
            ){
                PerkDesc = "Much better luck in casino"
            },

            new BundleModel(Bulletin, "PotentPotables",
                new BundleItem(C.Wine, 100, Iridium),
                new BundleItem(C.PaleAle, 100, Iridium),
                new BundleItem(C.Beer, 100, Iridium),
                new BundleItem(C.Mead, 100, Iridium),
                new BundleItem(C.Juice, 100),
                new BundleItem(C.Coffee, 100),
                new BundleItem(C.TruffleOil, 50),
                new BundleItem(C.Oil, 100),
                new BundleItem(C.OilofGarlic, 100),
                new BundleItem(C.LifeElixir, 100),
                new BundleItem(C.EnergyTonic, 75),
                new BundleItem(C.MuscleRemedy, 75)
            ){
                PerkDesc = "Prarie King Cheats Enabled"
            },

            new BundleModel(Bulletin, "Caffeinated",
                new BundleItem(C.Coffee, 999),
                new BundleItem(C.Coffee, 999),
                new BundleItem(C.Coffee, 999),
                new BundleItem(C.Coffee, 999),
                new BundleItem(C.Coffee, 999)
            ){
                PerkDesc = "Coffee is now an AMAZING superfood"
            },


            new BundleModel(Boiler, "Metallic",
                new BundleItem(C.CopperBar, 200),
                new BundleItem(C.IronBar, 200),
                new BundleItem(C.GoldBar, 200),
                new BundleItem(C.IridiumBar, 200),
                new BundleItem(C.RefinedQuartz, 200),
                new BundleItem(C.Geode, 50),
                new BundleItem(C.FrozenGeode, 50),
                new BundleItem(C.MagmaGeode, 50),
                new BundleItem(C.OmniGeode, 50)
            ){
                PerkDesc = "Both Level 5 Mining Perks"
            },

            new BundleModel(Boiler, "Monster",
                new BundleItem(C.BugMeat, 200),
                new BundleItem(C.Slime, 200),
                new BundleItem(C.BatWing, 200),
                new BundleItem(C.SolarEssence, 100),
                new BundleItem(C.VoidEssence, 100),
                new BundleItem(C.RedSlimeEgg, 1),
                new BundleItem(C.BlueSlimeEgg, 1),
                new BundleItem(C.GreenSlimeEgg, 1),
                new BundleItem(C.PurpleSlimeEgg, 1)
            ){
                PerkDesc = "Monster loot increased"
            },

            new BundleModel(Boiler, "Gemstone",
                new BundleItem(C.Diamond, 30),
                new BundleItem(C.Emerald, 30),
                new BundleItem(C.Amethyst, 30),
                new BundleItem(C.Topaz, 30),
                new BundleItem(C.Ruby, 30),
                new BundleItem(C.Aquamarine, 30),
                new BundleItem(C.Jade, 30),
                new BundleItem(C.Tigerseye, 30),
                new BundleItem(C.FireQuartz, 30),
                new BundleItem(C.FrozenTear, 30),
                new BundleItem(C.EarthCrystal, 30),
                new BundleItem(C.Marble, 30)
            ){
                PerkDesc = "All 4 Level 10 mining perks"
            },

            new BundleModel(Boiler, "Crafting",
                new BundleItem(C.Sprinkler, 20),
                new BundleItem(C.QualitySprinkler, 20),
                new BundleItem(C.IridiumSprinkler, 20),
                new BundleItem(C.CherryBomb, 100),
                new BundleItem(C.Bomb, 100),
                new BundleItem(C.MegaBomb, 100),
                new BundleItem(C.IridiumBand, 5),
                new BundleItem(C.BatteryPack, 20)
            ){
                PerkDesc = "Crystalariums run much faster"
            },

            new BundleModel(Boiler, "Trashtag",
                new BundleItem(C.Trash, 50),
                new BundleItem(C.Driftwood, 50),
                new BundleItem(C.SoggyNewspaper, 50),
                new BundleItem(C.BrokenCD, 50),
                new BundleItem(C.BrokenGlasses, 50),
                new BundleItem(C.JojaCola, 50),
                new BundleItem(C.RottenPlant, 50)
            ){
                PerkDesc = "Recycling machine is really good now"
            },

            new BundleModel(Boiler, "Archaeologists",
                new BundleItem(C.Arrowhead, 1),
                new BundleItem(C.ElvishJewelry, 1),
                new BundleItem(C.OrnamentalFan, 1),
                new BundleItem(C.RareDisc, 1),
                new BundleItem(C.RustyCog, 1),
                new BundleItem(C.ChickenStatue, 1),
                new BundleItem(C.AncientSeed, 1),
                new BundleItem(C.PrehistoricTool, 1),
                new BundleItem(C.DwarfGadget, 1),
                new BundleItem(C.GoldenMask, 1),
                new BundleItem(C.NautilusFossil, 1),
                new BundleItem(C.GoldenRelic, 1)
            ){
                PerkDesc = "Dig spots more prevalent"
            },


            new BundleModel(FishTank, "EasyFishing",
                new BundleItem(C.Carp, 15, Gold),
                new BundleItem(C.Herring, 15, Gold),
                new BundleItem(C.SmallmouthBass, 15, Gold),
                new BundleItem(C.Anchovy, 15, Gold),
                new BundleItem(C.Sardine, 15, Gold),
                new BundleItem(C.Sunfish, 15, Gold),
                new BundleItem(C.Chub, 15, Gold),
                new BundleItem(C.Perch, 15, Gold),
                new BundleItem(C.Bream, 15, Gold),
                new BundleItem(C.RedSnapper, 15, Gold),
                new BundleItem(C.SeaCucumber, 15, Gold),
                new BundleItem(C.RainbowTrout, 15, Gold)
            ){
                PerkDesc = "Both Level 5 Fishing Perks"
            },

            new BundleModel(FishTank, "IntermediateFishing",
                new BundleItem(C.Walleye, 15, Gold),
                new BundleItem(C.Shad, 15, Gold),
                new BundleItem(C.Bullhead, 15, Gold),
                new BundleItem(C.LargemouthBass, 15, Gold),
                new BundleItem(C.Salmon, 15, Gold),
                new BundleItem(C.Ghostfish, 15, Gold),
                new BundleItem(C.Tilapia, 15, Gold),
                new BundleItem(C.Woodskip, 15, Silver),
                new BundleItem(C.Halibut, 15, Gold),
                new BundleItem(C.Slimejack, 15, Gold),
                new BundleItem(C.RedMullet, 15, Gold),
                new BundleItem(C.Pike, 15, Gold)
            ){
                PerkDesc = "Never lose a fish again"
            },

            new BundleModel(FishTank, "HarderFishing",
                new BundleItem(C.TigerTrout, 15, Gold),
                new BundleItem(C.Albacore, 15, Gold),
                new BundleItem(C.Sandfish, 15, Silver),
                new BundleItem(C.Tuna, 15, Gold),
                new BundleItem(C.Eel, 15, Gold),
                new BundleItem(C.Catfish, 15, Gold),
                new BundleItem(C.Squid, 15, Gold),
                new BundleItem(C.Sturgeon, 15, Gold),
                new BundleItem(C.Dorado, 15, Gold),
                new BundleItem(C.Pufferfish, 15, Gold),
                new BundleItem(C.VoidSalmon, 15, Gold),
                new BundleItem(C.Stonefish, 15, Gold)
            ){
                PerkDesc = "Faster catch"
            },

            new BundleModel(FishTank, "Crabby",
                new BundleItem(C.Lobster, 25),
                new BundleItem(C.Clam, 25),
                new BundleItem(C.Crayfish, 25),
                new BundleItem(C.Crab, 25),
                new BundleItem(C.Cockle, 25),
                new BundleItem(C.Mussel, 25),
                new BundleItem(C.Shrimp, 25),
                new BundleItem(C.Snail, 25),
                new BundleItem(C.Periwinkle, 25),
                new BundleItem(C.Oyster, 25),
                new BundleItem(C.Seaweed, 25),
                new BundleItem(C.GreenAlgae, 25)
            ){
                PerkDesc = "???"
            },

            new BundleModel(FishTank, "TrickyFishing",
                new BundleItem(C.SuperCucumber, 10, Gold),
                new BundleItem(C.IcePip, 10, Gold),
                new BundleItem(C.Lingcod, 10, Gold),
                new BundleItem(C.ScorpionCarp, 10, Gold),
                new BundleItem(C.LavaEel, 10, Gold),
                new BundleItem(C.Octopus, 10, Gold),
                new BundleItem(C.Sashimi, 100),
                new BundleItem(C.MidnightSquid, 10, Silver),
                new BundleItem(C.SpookFish, 10, Silver),
                new BundleItem(C.Blobfish, 10, Silver)
            ){
                PerkDesc = "All level 10 fishing perks"
            },

            new BundleModel(FishTank, "Legendary",
                new BundleItem(C.Crimsonfish, 1),
                new BundleItem(C.Angler, 1),
                new BundleItem(C.Legend, 1),
                new BundleItem(C.Glacierfish, 1),
                new BundleItem(C.MutantCarp, 1)
            ){
                PerkDesc = "???"
            },


            new BundleModel(Vault, "Millionaire",
                new BundleItem(-1, 1000000, 1000000)
            ){
                PerkDesc = "All level 5 Combat Perks"
            },

            new BundleModel(Vault, "2Millionaire",
                new BundleItem(-1, 2000000, 2000000)
            ){
                PerkDesc = "Bus to desert leaves earlier each morning"
            },

            new BundleModel(Vault, "5Millionaire",
                new BundleItem(-1, 5000000, 5000000)
            ){
                PerkDesc = "All level 10 combat perks"
            },

            new BundleModel(Vault, "10Millionaire",
                new BundleItem(-1, 10000000, 10000000)
            ){
                PerkDesc = "Health regenerates"
            },

            new BundleModel(CraftRoom, "Party",
                new BundleItem(C.WhiteEgg, 4, Iridium), // todo: add easter egg
                new BundleItem(C.Tulip, 4, Iridium), // todo: something custom from flower dance
                new BundleItem(C.AlgaeSoup, 4, Iridium), // todo: something custom from luau
                new BundleItem(C.Jelly, 4, Iridium), // 
                new BundleItem(C.SurvivalBurger, 4, Iridium), // win the grange
                new BundleItem(C.IceCream, 4, Iridium), // buy with tokens
                new BundleItem(C.GoldenPumpkin, 4),
                new BundleItem(C.IcePip, 4, Iridium),
                new BundleItem(C.Pearl, 4),
                new BundleItem(C.Cookie, 4, Iridium)
            ){
                PerkDesc = "???"
            },



        };
        }
    }
}
