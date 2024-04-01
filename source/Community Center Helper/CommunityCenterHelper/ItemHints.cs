/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/vgperson/CommunityCenterHelper
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using System;

namespace CommunityCenterHelper
{
    /// <summary>Provides hint text for how to obtain items.</summary>
    public class ItemHints
    {
        public static ITranslationHelper str;
        public static IModRegistry modRegistry;
        public static ModConfig Config;
        
        /****************************
         ** Main Hint Text Methods **
         ****************************/
        
        /// <summary>Returns hint text for an item, given an item ID and quality.</summary>
        /// <param name="id">The item ID.</param>
        /// <param name="quality">Minimum required quality of item.</param>
        /// <param name="category">The item category, for items that accept anything in the category.</param>
        public static string getHintText(string id, int quality, int? category = 0)
        {
            try
            {
                string recipeUnlockTip = "";
                
                switch (id)
                {
                    /********** Crafts Room **********/
                    
                    // Spring Foraging Bundle
                    
                    case ItemID.IT_WildHorseradish:
                        return strSeasonalForage("spring");
                    
                    case ItemID.IT_Daffodil:
                        return strSeasonalForage("spring") + "\n"
                             + strBuyFrom("shopFlowerDance");
                    
                    case ItemID.IT_Leek:
                        return strSeasonalForage("spring");
                    
                    case ItemID.IT_Dandelion:
                        return strSeasonalForage("spring") + "\n"
                             + strBuyFrom("shopFlowerDance");
                    
                    // Summer Foraging Bundle
                    
                    case ItemID.IT_Grape:
                        return strSeasonalForage("summer") + "\n"
                             + strSeasonalCrop("fall", quality);
                    
                    case ItemID.IT_SpiceBerry:
                        return strSeasonalForage("summer")
                             + possibleSourceFruitBatCave();
                    
                    case ItemID.IT_SweetPea:
                        return strSeasonalForage("summer");
                    
                    // Fall Foraging Bundle
                    
                    case ItemID.IT_CommonMushroom:
                        return strSeasonalForage("fall") + "\n"
                             + strSeasonalLocationalForage(seasonList: new string[] { "spring", "fall" }, locationKey: "locationWoods");
                    
                    case ItemID.IT_WildPlum:
                        return strSeasonalForage("fall")
                             + possibleSourceFruitBatCave();
                    
                    case ItemID.IT_Hazelnut:
                        return strSeasonalForage("fall");
                    
                    case ItemID.IT_Blackberry:
                        return strSeasonalForage("fall")
                             + possibleSourceFruitBatCave();
                    
                    // Winter Foraging Bundle
                    
                    case ItemID.IT_WinterRoot:
                        return strSeasonalTilling("winter") + "\n"
                             + strDroppedByMonster("Frost Jelly");
                    
                    case ItemID.IT_CrystalFruit:
                        return strSeasonalForage("winter") + "\n"
                             + strDroppedByMonster("Dust Spirit");
                    
                    case ItemID.IT_SnowYam:
                        return strSeasonalTilling("winter");
                    
                    case ItemID.IT_Crocus:
                        return strSeasonalForage("winter");
                    
                    // Construction Bundle
                    
                    case ItemID.IT_Wood:
                        return strChopWood() + "\n"
                             + strBuyFrom("shopCarpenter");
                    
                    case ItemID.IT_Stone:
                        return strHitRocks() + "\n"
                             + strBuyFrom("shopCarpenter");
                    
                    case ItemID.IT_Hardwood:
                        return strChopHardwood();
                    
                    // Exotic Foraging Bundle
                    
                    case ItemID.IT_Coconut:
                        return strLocationalForage(isDesertKnown()? "locationDesert" : "locationUnknown") + "\n"
                             + strBuyFromOasisWeekday("monday");
                    
                    case ItemID.IT_CactusFruit:
                        return strLocationalForage(isDesertKnown()? "locationDesert" : "locationUnknown") + "\n"
                             + strBuyFromOasisWeekday("tuesday");
                    
                    case ItemID.IT_CaveCarrot:
                        return str.Get("smashCrates") + "\n"
                             + strLocationalTilling("locationMines");
                    
                    case ItemID.IT_RedMushroom:
                        return strLocationalForage("locationMines") + "\n"
                             + strSeasonalLocationalForage(seasonList: new string[] { "summer", "fall" },
                                                           locationKey: "locationWoods") + "\n"
                             + strTapTree("treeMushroom")
                             + possibleSourceMushroomCave()
                             + possibleSourceSpecialFarmSeasonal("forest", "fall");
                    
                    case ItemID.IT_PurpleMushroom:
                        return strLocationalForage("locationMinesArea3")
                             + possibleSourceMushroomCave()
                             + possibleSourceSpecialFarmSeasonal("forest", "fall");
                    
                    case ItemID.IT_MapleSyrup:
                        return strTapTree("treeMaple");
                    
                    case ItemID.IT_OakResin:
                        return strTapTree("treeOak");
                    
                    case ItemID.IT_PineTar:
                        return strTapTree("treePine");
                    
                    case ItemID.IT_Morel:
                        return strSeasonalLocationalForage("spring", "locationWoods")
                             + possibleSourceMushroomCave()
                             + possibleSourceSpecialFarmSeasonal("forest", "spring");
                    
                    /********** Pantry **********/
                    
                    // Spring Crops Bundle
                    
                    case ItemID.IT_Parsnip:
                        return strSeasonalCrop("spring", quality);
                    
                    case ItemID.IT_GreenBean:
                        return strSeasonalCrop("spring", quality);
                    
                    case ItemID.IT_Cauliflower:
                        return strSeasonalCrop("spring", quality);
                    
                    case ItemID.IT_Potato:
                        return strSeasonalCrop("spring", quality);
                    
                    // Summer Crops Bundle
                    
                    case ItemID.IT_Tomato:
                        return strSeasonalCrop("summer", quality);
                    
                    case ItemID.IT_Blueberry:
                        return strSeasonalCrop("summer", quality);
                    
                    case ItemID.IT_HotPepper:
                        return strSeasonalCrop("summer", quality);
                    
                    case ItemID.IT_Melon:
                        return strSeasonalCrop("summer", quality);
                    
                    // Fall Crops Bundle
                    
                    case ItemID.IT_Corn:
                        return strSeasonalCrop(seasonList: new string[] { "summer", "fall" }, quality: quality);
                    
                    case ItemID.IT_Eggplant:
                        return strSeasonalCrop("fall", quality);
                    
                    case ItemID.IT_Pumpkin: // Also in Quality Crops Bundle
                        return strSeasonalCrop("fall", quality);
                    
                    case ItemID.IT_Yam:
                        return strSeasonalCrop("fall", quality)
                             + (quality == 0? "\n" + strDroppedByMonster("Duggy") : ""); // Base quality only
                    
                    // Animal Bundle
                    
                    case ItemID.IT_LargeMilk:
                        return strAnimalProduct("animalCow", true);
                    
                    case ItemID.IT_LargeEggBrown:
                        return strAnimalProduct("animalChicken", true);
                    
                    case ItemID.IT_LargeEgg:
                        return strAnimalProduct("animalChicken", true);
                    
                    case ItemID.IT_LargeGoatMilk:
                        return strAnimalProduct("animalGoat", true);
                    
                    case ItemID.IT_Wool:
                        return strAnimalProduct("animalSheep") + "\n"
                             + strAnimalProduct("animalRabbit");
                    
                    case ItemID.IT_DuckEgg:
                        return strAnimalProduct("animalDuck");
                    
                    // Artisan Bundle
                    
                    case ItemID.IT_TruffleOil:
                        return strPutItemInMachine(ItemID.IT_Truffle, ItemID.BC_OilMaker);
                    
                    case ItemID.IT_Cloth:
                        return strPutItemInMachine(ItemID.IT_Wool, ItemID.BC_Loom) + "\n"
                             + strPutItemInMachine(ItemID.IT_SoggyNewspaper, ItemID.BC_RecyclingMachine);
                    
                    case ItemID.IT_GoatCheese:
                        return strMachineOrCaskForQuality(ItemID.IT_GoatMilk, ItemID.BC_CheesePress, ItemID.IT_GoatCheese, quality);
                    
                    case ItemID.IT_Cheese:
                        return strMachineOrCaskForQuality(ItemID.IT_Milk, ItemID.BC_CheesePress, ItemID.IT_Cheese, quality);
                    
                    case ItemID.IT_Honey:
                        return strNoItemMachine(ItemID.BC_BeeHouse) + "\n"
                             + strBuyFromOasisWeekday("friday");
                    
                    case ItemID.IT_Jelly:
                        return strPutItemInMachine(StardewValley.Object.FruitsCategory.ToString(), ItemID.BC_PreservesJar);
                    
                    case ItemID.IT_Apple:
                        return strFruitTreeDuringSeason("treeApple", "fall")
                             + possibleSourceFruitBatCave();
                    
                    case ItemID.IT_Apricot:
                        return strFruitTreeDuringSeason("treeApricot", "spring")
                             + possibleSourceFruitBatCave();
                    
                    case ItemID.IT_Orange:
                        return strFruitTreeDuringSeason("treeOrange", "summer")
                             + possibleSourceFruitBatCave();
                    
                    case ItemID.IT_Peach:
                        return strFruitTreeDuringSeason("treePeach", "summer")
                             + possibleSourceFruitBatCave();
                    
                    case ItemID.IT_Pomegranate:
                        return strFruitTreeDuringSeason("treePomegranate", "fall")
                             + possibleSourceFruitBatCave();
                    
                    case ItemID.IT_Cherry:
                        return strFruitTreeDuringSeason("treeCherry", "spring")
                             + possibleSourceFruitBatCave();
                    
                    /********** Fish Tank **********/
                    
                    // River Fish Bundle
                    
                    case ItemID.IT_Sunfish:
                        bool fishableFarm = haveSpecialFarmType("riverlands") || haveSpecialFarmType("wilderness");
                        return strFishBase(waterList: new string[] { "waterRivers", fishableFarm? "waterSpecialFarm" : "" },
                                           start: "6am", end: "7pm",
                                           seasonList: new string[] { "spring", "summer" }, weatherKey: "weatherSun");
                    
                    case ItemID.IT_Catfish:
                        return strFishBase("waterRivers", "6am", "12am", seasonList: new string[] { "spring", "fall", "winter" },
                                           weatherKey: "weatherRain") + "\n"
                             + strFishBase("waterWoods", "6am", "12am", weatherKey: "weatherRain");
                    
                    case ItemID.IT_Shad:
                        return strFishBase("waterRivers", "9am", "2am",
                                           seasonList: new string[] { "spring", "summer", "fall" }, weatherKey: "weatherRain");
                    
                    case ItemID.IT_TigerTrout:
                        return strFishBase("waterRivers", "6am", "7pm",
                                           seasonList: new string[] { "fall", "winter" });
                    
                    // Lake Fish Bundle
                    
                    case ItemID.IT_LargemouthBass:
                        return strFishBase("waterMountain", "6am", "7pm");
                    
                    case ItemID.IT_Carp:
                        return strFishBase("waterMountain", seasonList: new string[] { "spring", "summer", "fall" }) + "\n"
                             + strFishBase(waterList: new string[] { "waterWoods", isSewerKnown()? "waterSewer" : "" });
                    
                    case ItemID.IT_Bullhead:
                        return strFishBase("waterMountain");
                    
                    case ItemID.IT_Sturgeon:
                        return strFishBase("waterMountain", "6am", "7pm", seasonList: new string[] { "summer", "winter" });
                    
                    // Ocean Fish Bundle
                    
                    case ItemID.IT_Sardine:
                        return strFishBase("waterOcean", "6am", "7pm", seasonList: new string[] { "spring", "fall", "winter" });
                    
                    case ItemID.IT_Tuna:
                        return strFishBase("waterOcean", "6am", "7pm", seasonList: new string[] { "summer", "winter" })
                             + (isIslandKnown()? "\n" + strFishBase(waterList: new string[] { "waterIslandOcean", "waterIslandCove" },
                                                                   start: "6am", end: "7pm") : "");
                    
                    case ItemID.IT_RedSnapper:
                        return strFishBase("waterOcean", "6am", "7pm", seasonList: new string[] { "summer", "fall", "winter" },
                                           weatherKey: "weatherRain");
                    
                    case ItemID.IT_Tilapia:
                        return strFishBase("waterOcean", "6am", "2pm", seasonList: new string[] { "summer", "fall" })
                             + (isIslandKnown()? "\n" + strFishBase("waterIslandRiver", "6am", "2pm") : "");
                    
                    // Night Fishing Bundle
                    
                    case ItemID.IT_Walleye:
                        return strFishBase(waterList: new string[] { "waterRivers", "waterMountain", "waterForestPond" },
                                           start: "12pm", end: "2am", seasonList: new string[] { "fall", "winter" },
                                           weatherKey: "weatherRain");
                    
                    case ItemID.IT_Bream:
                        return strFishBase("waterRivers");
                    
                    case ItemID.IT_Eel:
                        return strFishBase("waterOcean", "4pm", "2am", seasonList: new string[] { "spring", "fall" },
                                           weatherKey: "weatherRain");
                    
                    // Crab Pot Bundle
                    
                    case ItemID.IT_Lobster:
                        return strCrabPot("waterTypeOcean");
                    
                    case ItemID.IT_Crayfish:
                        return strCrabPot("waterTypeFresh");
                    
                    case ItemID.IT_Crab:
                        return strCrabPot("waterTypeOcean") + "\n"
                             + strDroppedByMonster("Rock Crab") + "\n"
                             + strDroppedByMonster("Lava Crab");
                    
                    case ItemID.IT_Cockle:
                        return strCrabPot("waterTypeOcean") + "\n"
                             + strLocationalForage("locationBeach");
                    
                    case ItemID.IT_Mussel:
                        return strCrabPot("waterTypeOcean") + "\n"
                             + strLocationalForage("locationBeach");
                    
                    case ItemID.IT_Shrimp:
                        return strCrabPot("waterTypeOcean");
                    
                    case ItemID.IT_Snail:
                        return strCrabPot("waterTypeFresh");
                    
                    case ItemID.IT_Periwinkle:
                        return strCrabPot("waterTypeFresh");
                    
                    case ItemID.IT_Oyster:
                        return strCrabPot("waterTypeOcean") + "\n"
                             + strLocationalForage("locationBeach");
                    
                    case ItemID.IT_Clam:
                        return strCrabPot("waterTypeOcean") + "\n"
                             + strLocationalForage("locationBeach");
                    
                    // Specialty Fish Bundle
                    
                    case ItemID.IT_Pufferfish:
                        return strFishBase("waterOcean", "12pm", "4pm", "summer", "weatherSun")
                             + (isIslandKnown()? "\n" + strFishBase(waterList: new string[] { "waterIslandOcean", "waterIslandCove" },
                                                                   start: "12pm", end: "4pm", weatherKey: "weatherSun") : "");
                    
                    case ItemID.IT_Ghostfish:
                        return strFishBase("waterMines") + "\n"
                             + strDroppedByMonster("Ghost");
                    
                    case ItemID.IT_Sandfish:
                        return strFishBase(isDesertKnown()? "waterDesert" : "waterUnknown", "6am", "8pm");
                    
                    case ItemID.IT_Woodskip:
                        return strFishBase(waterList: new string[] { "waterWoods",
                                                                     haveSpecialFarmType("forest")? "waterSpecialFarm" : "" });
                    
                    /********** Boiler Room **********/
                    
                    // Blacksmith's Bundle
                    
                    case ItemID.IT_CopperBar:
                        return strPutItemInMachine(ItemID.IT_CopperOre, ItemID.BC_Furnace);
                    
                    case ItemID.IT_IronBar:
                        return strPutItemInMachine(ItemID.IT_IronOre, ItemID.BC_Furnace) + "\n"
                             + strCraftRecipe("Transmute (Fe)");
                    
                    case ItemID.IT_GoldBar:
                        return strPutItemInMachine(ItemID.IT_GoldOre, ItemID.BC_Furnace) + "\n"
                             + strCraftRecipe("Transmute (Au)");
                    
                    // Geologist's Bundle
                    
                    case ItemID.IT_Quartz:
                        return strLocationalForage("locationMines");
                    
                    case ItemID.IT_EarthCrystal:
                        return strLocationalForage("locationMinesArea1") + "\n"
                             + strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode) + "\n"
                             + strDroppedByMonster("Duggy") + "\n"
                             + strPanning();
                    
                    case ItemID.IT_FrozenTear:
                        return strLocationalForage("locationMinesArea2") + "\n"
                             + strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode) + "\n"
                             + strDroppedByMonster("Dust Spirit") + "\n"
                             + strPanning();
                    
                    case ItemID.IT_FireQuartz:
                        return strLocationalForage("locationMinesArea3") + "\n"
                             + strOpenGeode(ItemID.IT_MagmaGeode, ItemID.IT_OmniGeode) + "\n"
                             + strPanning();
                    
                    // Adventurer's Bundle
                    
                    case ItemID.IT_Slime:
                        return strDroppedByMonster(monsterKey: "monsterGeneralSlime");
                    
                    case ItemID.IT_BatWing:
                        return strDroppedByMonster(monsterKey: "monsterGeneralBat");
                    
                    case ItemID.IT_SolarEssence:
                        return strDroppedByMonster("Ghost") + "\n"
                             + strDroppedByMonster("Squid Kid") + "\n"
                             + strDroppedByMonster("Metal Head") + "\n"
                             + strBuyFromKrobus()
                             + (isSkullCavernKnown()? "\n" + strDroppedByMonster("Mummy") : "") + "\n"
                             + strFishPond(ItemID.IT_Sunfish, 10);
                    
                    case ItemID.IT_VoidEssence:
                        return strDroppedByMonster("Shadow Brute") + "\n"
                             + strBuyFromKrobus()
                             + (isSkullCavernKnown()? "\n" + strDroppedByMonster("Serpent") : "") + "\n"
                             + strFishPond(ItemID.IT_VoidSalmon, 9);
                    
                    /********** Bulletin Board **********/
                    
                    // Chef's Bundle
                    
                    case ItemID.IT_FiddleheadFern:
                        return strSeasonalLocationalForage("summer", "locationWoods");
                    
                    case ItemID.IT_Truffle:
                        return strAnimalProduct("animalPig", mustBeOutside: true);
                        
                    case ItemID.IT_Poppy:
                        return strSeasonalCrop("summer", quality);
                    
                    case ItemID.IT_MakiRoll:
                        return strCookRecipe("Maki Roll");
                    
                    case ItemID.IT_FriedEgg:
                        return strCookRecipe("Fried Egg");
                    
                    // Dye Bundle
                    
                    case ItemID.IT_SeaUrchin:
                        if (fixedBridgeToEastBeach())
                            return strLocationalForage("locationBeachEast");
                        else
                            return strLocationalForage(locationLiteral: str.Get("locationBeachEastInaccessible",
                                                                                new { wood = getItemName(ItemID.IT_Wood) }));
                    
                    case ItemID.IT_Sunflower:
                        return strSeasonalCrop(seasonList: new string[] { "summer", "fall" }, quality: quality);
                    
                    case ItemID.IT_DuckFeather:
                        return strAnimalProduct("animalDuck", true);
                    
                    case ItemID.IT_Aquamarine:
                        return str.Get("gemAquamarine") + "\n"
                             + str.Get("smashCrates") + "\n"
                             + strFishingChest(2) + "\n"
                             + strPanning();
                    
                    case ItemID.IT_RedCabbage:
                        return strSeasonalCrop("summer", quality, startingYear: 2);
                    
                    // Field Research Bundle
                    
                    case ItemID.IT_NautilusShell:
                        return strSeasonalLocationalForage("winter", "locationBeach");
                    
                    case ItemID.IT_Chub:
                        return strFishBase(waterList: new string[] { "waterMountain", "waterForestRiver" });
                    
                    case ItemID.IT_FrozenGeode:
                        return strHitRocks("locationMinesArea2") + "\n"
                             + strSeasonalLocationalForage("winter", "locationFarm") + "\n"
                             + strFishingChest() + "\n"
                             + strFishPond(ItemID.IT_IcePip, 9);
                    
                    // Fodder Bundle
                    
                    case ItemID.IT_Wheat:
                        return strSeasonalCrop(seasonList: new string[] { "summer", "fall" }, quality: quality);
                    
                    case ItemID.IT_Hay:
                        return strBuyFrom(shopLiteral: multiKey("shopMarnie", isDesertKnown()? "shopDesertTrader" : "")) + "\n"
                             + str.Get("harvestHay", new { wheat = getItemName(ItemID.IT_Wheat) })
                             + (modRegistry.IsLoaded("ppja.artisanvalleyPFM")? // Artisan Valley machine rules
                                "\n" + strPutItemInMachine(ItemID.IT_Fiber, machineName: "Drying Rack") : "");
                    
                    // Enchanter's Bundle
                    
                    case ItemID.IT_Wine: // Also in Missing Bundle at silver quality
                        return strMachineOrCaskForQuality(StardewValley.Object.FruitsCategory.ToString(), ItemID.BC_Keg, ItemID.IT_Wine, quality);
                    
                    case ItemID.IT_RabbitsFoot:
                        return strAnimalProduct("animalRabbit", true) + "\n"
                             + strDroppedByMonster("Serpent");
                    
                    /********** The Missing Bundle **********/
                    
                    case ItemID.IT_DinosaurMayonnaise:
                        return strPutItemInMachine(ItemID.IT_DinosaurEgg, ItemID.BC_MayonnaiseMachine);
                    
                    case ItemID.IT_PrismaticShard:
                        return str.Get("gemPrismaticShard") + "\n"
                             + strFishingChest(6) + "\n"
                             + strFishPond(ItemID.IT_RainbowTrout, 9);
                    
                    case ItemID.IT_AncientFruit:
                        if (!Game1.player.knowsRecipe("Ancient Seeds"))
                            recipeUnlockTip = parenthesize(getCraftingRecipeSources("Ancient Seeds"));
                        return strGrowSeeds(ItemID.IT_AncientSeeds, recipeUnlockTip, quality);
                    
                    case ItemID.IT_VoidSalmon:
                        return strFishBase(isWitchSwampKnown()? "waterSwamp" : "waterUnknown");
                    
                    case ItemID.IT_Caviar:
                        return strPutItemInMachine(ItemID.IT_Roe, itemLiteral: getFishRoeName(ItemID.IT_Sturgeon),
                                                   machineID: ItemID.BC_PreservesJar);
                    
                    /********** Crafts Room (Remix) **********/
                    
                    // Forest Bundle
                    case ItemID.IT_Moss:
                        return str.Get("treeMoss");
                    
                    /********** Pantry (Remix) **********/
                    
                    // Spring Crops Bundle
                    case ItemID.IT_Carrot:
                        return strGrowSeeds(ItemID.IT_CarrotSeeds, parenthesize(strSeasonalForage("spring")), quality);
                    
                    // Summer Crops Bundle
                    case ItemID.IT_SummerSquash:
                        return strGrowSeeds(ItemID.IT_SummerSquashSeeds, parenthesize(strSeasonalForage("summer")), quality);
                    
                    // Fall Crops Bundle
                    case ItemID.IT_Broccoli:
                        return strGrowSeeds(ItemID.IT_BroccoliSeeds, parenthesize(strSeasonalForage("fall")), quality);
                    
                    // Fish Farmer's Bundle
                    
                    case ItemID.IT_Roe:
                        return str.Get("fishPondAny");
                    
                    /********** Boiler Room (Remix) **********/
                    
                    // Adventurer's Bundle
                    
                    case ItemID.IT_BoneFragment:
                        return strDroppedByMonster("Skeleton") + "\n"
                             + strDroppedByMonster("Lava Lurk");
                    
                    /********** Bulletin Board (Remix) **********/
                    
                    // Children's Bundle
                    
                    case ItemID.IT_IceCream:
                        return strCookRecipe("Ice Cream");
                        
                    case ItemID.IT_Cookie:
                        return strCookRecipe("Cookies");
                    
                    // Home Cook's Bundle
                    
                    case ItemID.IT_WheatFlour:
                        return strBuyFrom(shopLiteral: getSeedShopsString());
                    
                    // Helper's Bundle
                    case ItemID.IT_PrizeTicket:
                        return str.Get("prizeTicket");
                    
                    case ItemID.IT_MysteryBox:
                        if (areMysteryBoxesUnlocked())
                            return str.Get("artifactDigging") + "\n"
                                 + str.Get("fishingChest") + "\n"
                                 + str.Get("hitRocks");
                        else
                        {
                            if (Config.ShowSpoilers)
                                return str.Get("mysteryBoxesLocked");
                            else
                                return str.Get("unknownSource");
                        }
                    
                    // Winter Star Bundle
                    case ItemID.IT_Powdermelon:
                        return strGrowSeeds(ItemID.IT_PowdermelonSeeds, parenthesize(strSeasonalForage("winter")), quality);
                    
                    /********** [EasierBundles] Crafts Room **********/
                    
                    // [EasierBundles] Construction Bundle
                    
                    case ItemID.IT_Clay:
                        return str.Get("generalTilling") + "\n"
                             + str.Get("openAnyGeode");
                    
                    case ItemID.IT_Fiber:
                        return str.Get("clearWeeds");
                    
                    /********** [EasierBundles] Pantry **********/
                    
                    // [EasierBundles] Animal Bundle
                    
                    case ItemID.IT_VoidEgg:
                        return strBuyFromKrobus() + "\n"
                             + str.Get("voidEggEvent") + (getCoopLevel() < 1? parenthesize(str.Get("animalReqCoopLv2")) : "") + "\n"
                             + strAnimalProduct("animalVoidChicken") + "\n"
                             + strFishPond(ItemID.IT_VoidSalmon, 9);
                    
                    case ItemID.IT_BugMeat:
                        return strDroppedByMonster(monsterKey: "monsterGeneralBug");
                    
                    // [EasierBundles] Artisan Bundle
                    
                    case ItemID.IT_Pickles:
                        return strPutItemInMachine(StardewValley.Object.VegetableCategory.ToString(), ItemID.BC_PreservesJar);
                    
                    /********** [EasierBundles] Fish Tank **********/
                    
                    // [EasierBundles] River Fish Bundle
                    
                    case ItemID.IT_Opal:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Esperite:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Calcite:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Orpiment:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    // [EasierBundles] Lake Fish Bundle
                    
                    case ItemID.IT_Sandstone:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Granite:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Limestone:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Mudstone:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    // [EasierBundles] Ocean Fish Bundle
                    
                    case ItemID.IT_RainbowShell:
                        return strSeasonalLocationalForage("summer", "locationBeach");
                    
                    case ItemID.IT_Celestine:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Slate:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    // [EasierBundles] Night Fishing Bundle
                    
                    case ItemID.IT_Torch:
                        return strCraftRecipe("Torch") + "\n"
                             + strPutItemInMachine(ItemID.IT_SoggyNewspaper, ItemID.BC_RecyclingMachine);
                    
                    case ItemID.IT_BatteryPack:
                        return strNoItemMachine(ItemID.BC_LightningRod);
                    
                    case ItemID.IT_JackOLantern:
                        return strBuyFrom("shopSpiritsEve");
                    
                    // [EasierBundles] Crab Pot Bundle
                    
                    case ItemID.IT_Coral:
                        if (fixedBridgeToEastBeach())
                            return strLocationalForage("locationBeachEast");
                        else
                            return strLocationalForage(locationLiteral: str.Get("locationBeachEastInaccessible",
                                                                                new { wood = getItemName(ItemID.IT_Wood) }));
                    
                    // [EasierBundles] Specialty Bundle
                    
                    case ItemID.IT_Emerald:
                        return str.Get("gemEmerald") + "\n"
                             + strFishingChest(2) + "\n"
                             + strPanning();
                    
                    case ItemID.IT_Ruby:
                        return str.Get("gemRuby") + "\n"
                             + strFishingChest(2) + "\n"
                             + strPanning();
                    
                    case ItemID.IT_Topaz:
                        return str.Get("gemTopaz") + "\n"
                             + strFishingChest(2) + "\n"
                             + strPanning();
                    
                    case ItemID.IT_Jade:
                        return str.Get("gemJade") + "\n"
                             + strFishingChest(2);
                    
                    /********** [EasierBundles] Bulletin Board **********/
                    
                    // [EasierBundles] Chef's Bundle
                    
                    case ItemID.IT_SalmonDinner:
                        return strCookRecipe("Salmon Dinner");
                    
                    case ItemID.IT_Beer:
                        return (quality == 0? (strBuyFrom("shopSaloon") + "\n") : "") // Include Saloon for base quality only
                             + strMachineOrCaskForQuality(ItemID.IT_Wheat, ItemID.BC_Keg, ItemID.IT_Beer, quality);
                    
                    case ItemID.IT_Juice:
                        return strPutItemInMachine(StardewValley.Object.VegetableCategory.ToString(), ItemID.BC_Keg);
                    
                    // [EasierBundles] Field Research Bundle
                    
                    case ItemID.IT_Chanterelle:
                        return strSeasonalLocationalForage("fall", "locationWoods")
                             + possibleSourceMushroomCave();
                    
                    case ItemID.IT_PineCone:
                        return str.Get("seedFromTree", new { tree = str.Get("treePine") });
                    
                    // [EasierBundles] Fodder Bundle
                    
                    case ItemID.IT_Amaranth:
                        return strSeasonalCrop("fall", quality);
                    
                    // [EasierBundles] Enchanter's Bundle
                    
                    case ItemID.IT_Holly:
                        return strSeasonalForage("winter");
                    
                    case ItemID.IT_Mead:
                        return strMachineOrCaskForQuality(ItemID.IT_Honey, ItemID.BC_Keg, ItemID.IT_Mead, quality);
                    
                    /********** [Vegan Bundles] Pantry **********/
                    
                    // [Vegan Bundles] Cleaning Bundle
                    
                    case ItemID.IT_Trash:
                        return str.Get("trash");
                    
                    case ItemID.IT_SoggyNewspaper:
                        return str.Get("trash");
                    
                    case ItemID.IT_BrokenCD:
                        return str.Get("trash");
                    
                    case ItemID.IT_BrokenGlasses:
                        return str.Get("trash");
                    
                    // [Vegan Bundles] Artisan Bundle
                   
                    case ItemID.IT_Oil:
                        return strPutItemInMachine(itemLiteral: multiItem(ItemID.IT_Corn, ItemID.IT_SunflowerSeeds, ItemID.IT_Sunflower),
                                                   machineID: ItemID.BC_OilMaker);
                    
                    case ItemID.IT_BokChoy:
                        return strSeasonalCrop("fall", quality);
                    
                    case ItemID.IT_Garlic:
                        return strSeasonalCrop("spring", quality);
                    
                    case ItemID.IT_Hops:
                        return strSeasonalCrop("summer", quality);
                    
                    /********** [Vegan Bundles] Fish Tank **********/
                    
                    // [Vegan Bundles] Snacks Bundle
                    
                    case ItemID.IT_FriedMushroom:
                        return strCookRecipe("Fried Mushroom");
                    
                    case ItemID.IT_Hashbrowns:
                        return strCookRecipe("Hashbrowns");
                    
                    case ItemID.IT_Tortilla:
                        return strCookRecipe("Tortilla");
                    
                    case ItemID.IT_RoastedHazelnuts:
                        return strCookRecipe("Roasted Hazelnuts");
                    
                    // [Vegan Bundles] Beverages Bundle
                    
                    case ItemID.IT_Coffee:
                        return strPutItemInMachine(ItemID.IT_CoffeeBean, ItemID.BC_Keg, itemQuantity: 5);
                    
                    case ItemID.IT_GreenTea:
                        return strPutItemInMachine(ItemID.IT_TeaLeaves, ItemID.BC_Keg);
                    
                    // [Vegan Bundles] Flowers Bundle
                   
                    case ItemID.IT_Tulip:
                        return strSeasonalCrop("spring", quality);
                    
                    case ItemID.IT_SummerSpangle:
                        return strSeasonalCrop("summer", quality);
                    
                    case ItemID.IT_FairyRose:
                        return strSeasonalCrop("fall", quality);
                    
                    case ItemID.IT_BlueJazz:
                        return strSeasonalCrop("spring", quality);
                    
                    // [Vegan Bundles] Underrated Bundle
                    
                    case ItemID.IT_SpringOnion:
                        return strSeasonalLocationalForage("spring", "locationForest");
                    
                    // [Vegan Bundles] Dessert Bundle
                    
                    case ItemID.IT_MapleBar:
                        return strCookRecipe("Maple Bar");
                    
                    case ItemID.IT_PoppyseedMuffin:
                        return strCookRecipe("Poppyseed Muffin");
                    
                    case ItemID.IT_BlackberryCobbler:
                        return strCookRecipe("Blackberry Cobbler");
                    
                    case ItemID.IT_PlumPudding:
                        return strCookRecipe("Plum Pudding");
                    
                    case ItemID.IT_CranberrySauce:
                        return strCookRecipe("Cran. Sauce");
                    
                    // [Vegan Bundles] Sparkly Bundle
                    
                    case ItemID.IT_Alamite:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Aerinite:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Geminite:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_OceanStone:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    /********** [Vegan Bundles] Boiler Room **********/
                    
                    // [Vegan Bundles] Explorer's Bundle
                    
                    case ItemID.IT_AncientDoll:
                        return str.Get("artifactDigging") + "\n"
                             + strFishingChest(2);
                    
                    case ItemID.IT_RustyCog:
                        return str.Get("artifactDigging") + "\n"
                             + strFishingChest(2);
                    
                    case ItemID.IT_ChickenStatue:
                        return str.Get("artifactDigging") + "\n"
                             + strFishingChest(2);
                    
                    case ItemID.IT_GlassShards:
                        return str.Get("artifactDigging") + "\n"
                             + strFishingChest(2);
                    
                    /********** [Vegan Bundles] Bulletin Board **********/
                    
                    // [Vegan Bundles] Chef's Bundle
                    
                    case ItemID.IT_Rice:
                        return strBuyFrom(shopLiteral: getSeedShopsString()) + "\n"
                             + str.Get("putItemInMill", new { rawItem = getItemName(ItemID.IT_UnmilledRice) });
                    
                    case ItemID.IT_Salad:
                        return strCookRecipe("Salad");
                    
                    // [Vegan Bundles] Dye Bundle
                    
                    case ItemID.IT_TeaLeaves:
                        if (!Game1.player.knowsRecipe("Tea Sapling"))
                            recipeUnlockTip = parenthesize(getCraftingRecipeSources("Tea Sapling"));
                        return strGrowSeeds(ItemID.IT_TeaSapling, recipeUnlockTip, quality);
                    
                    // [Vegan Bundles] Scythe Bundle
                   
                    case ItemID.IT_Kale:
                        return strSeasonalCrop("spring", quality);
                    
                    // [Vegan Bundles] Enchanter's Bundle
                    
                    case ItemID.IT_Diamond:
                        return str.Get("gemDiamond") + "\n"
                             + strPanning();
                    
                    /********** [Vegan Bundles] The Missing Bundle **********/
                    
                    case ItemID.IT_PaleAle:
                        return strMachineOrCaskForQuality(ItemID.IT_Hops, ItemID.BC_Keg, ItemID.IT_PaleAle, quality);
                    
                    case ItemID.IT_IridiumOre:
                        return str.Get("mineIridiumOre") + "\n"
                             + strOpenGeode(ItemID.IT_MagmaGeode, ItemID.IT_OmniGeode) + "\n"
                             + strFishingChest() + "\n"
                             + strDroppedByMonster(monsterKey: "monsterPurpleSlime") + "\n"
                             + strDroppedByMonster("Iridium Crab") + "\n"
                             + strDroppedByMonster("Iridium Bat") + "\n"
                             + strFishPond(ItemID.IT_SuperCucumber, 9) + "\n"
                             + (Config.ShowSpoilers || Game1.year >= 3? strNoItemMachine(ItemID.BC_StatueOfPerfection)
                                                                      : str.Get("unknownSource")) + "\n"
                             + strPanning();
                    
                    case ItemID.IT_SuperMeal:
                        return strCookRecipe("Super Meal");
                    
                    /********** [Difficulty Options] Crafts Room **********/
                    
                    // [Difficulty Options] Spring Foraging Bundle
                    
                    case ItemID.IT_Salmonberry:
                        return str.Get("salmonberryForage")
                             + possibleSourceFruitBatCave();
                    
                    /********** [Difficulty Options] Pantry **********/
                    
                    // [Difficulty Options] Spring Crops Bundle
                    
                    case ItemID.IT_CoffeeBean:
                        return strSeasonalCrop(seasonList: new string[] { "spring", "summer" }, quality: quality,
                                               shopKey: "shopTravelingCartRandom");
                    
                    case ItemID.IT_Rhubarb:
                        return strSeasonalCrop("spring", quality, isDesertKnown()? "shopOasis" : "shopUnknown");
                    
                    case ItemID.IT_Strawberry:
                        return strSeasonalCrop("spring", quality, "shopEggFestival");
                    
                    // [Difficulty Options] Summer Crops Bundle
                    
                    case ItemID.IT_Radish:
                        return strSeasonalCrop("summer", quality);
                    
                    // [Difficulty Options] Fall Crops Bundle
                    
                    case ItemID.IT_Artichoke:
                        return strSeasonalCrop("fall", quality, startingYear: 2);
                    
                    case ItemID.IT_Beet:
                        return strSeasonalCrop("fall", quality, isDesertKnown()? "shopOasis" : "shopUnknown");
                    
                    case ItemID.IT_Cranberries:
                        return strSeasonalCrop("fall", quality);
                    
                    // [Difficulty Options] Quality Crops Bundle
                    
                    case ItemID.IT_Starfruit:
                        return strSeasonalCrop("summer", quality, isDesertKnown()? "shopOasis" : "shopUnknown");
                    
                    /********** [Difficulty Options] Fish Tank **********/
                    
                    // [Difficulty Options] Specialty Fish Bundle
                    
                    case ItemID.IT_Angler:
                        return strFishBase("waterAnglerSpot", seasonKey: "fall", fishingLevel: 3, extraLinebreak: true);
                    
                    case ItemID.IT_Crimsonfish:
                        return strFishBase("waterCrimsonfishSpot", seasonKey: "summer", fishingLevel: 5, extraLinebreak: true);
                    
                    case ItemID.IT_Glacierfish:
                        return strFishBase("waterGlacierfishSpot", seasonKey: "winter", fishingLevel: 6, extraLinebreak: true);
                    
                    case ItemID.IT_Legend:
                        return strFishBase("waterLegendSpot", seasonKey: "spring", fishingLevel: 10, extraLinebreak: true);
                    
                    case ItemID.IT_MutantCarp:
                        return strFishBase(isSewerKnown()? "waterSewer" : "waterUnknown");
                    
                    /********** [Difficulty Options] Boiler Room **********/
                    
                    // [Difficulty Options] Blacksmith's Bundle
                    
                    case ItemID.IT_IridiumBar:
                        return strPutItemInMachine(ItemID.IT_IridiumOre, ItemID.BC_Furnace);
                    
                    /********** [Difficulty Options] Bulletin Board **********/
                    
                    // [Difficulty Options] Chef's Bundle
                    
                    case ItemID.IT_FishTaco:
                        return strCookRecipe("Fish Taco");
                    
                    // [Difficulty Options] Field Research Bundle
                    
                    case ItemID.IT_Geode:
                        return strHitRocks("locationMinesArea1") + "\n"
                             + strDroppedByMonster("Duggy") + "\n"
                             + strFishingChest();
                    
                    /********** [Difficulty Options] The Missing Bundle **********/
                    
                    case ItemID.IT_SpicyEel:
                        return strCookRecipe("Spicy Eel") + "\n"
                             + strDroppedByMonster("Serpent") + "\n"
                             + strFishPond(ItemID.IT_LavaEel, 9);
                    
                    case ItemID.IT_Blobfish:
                        return str.Get("fishSubmarine");
                    
                    case ItemID.IT_Pearl:
                        bool gotPearl = Game1.player.mailReceived.Contains("gotPearl");
                        bool showPearlHint = Config.ShowSpoilers || Game1.player.secretNotesSeen.Contains(15);
                        return (!gotPearl? (str.Get("mermaidPearl", new { hint = showPearlHint? str.Get("mermaidHint") : "" }) + "\n")
                                         : "")
                             + str.Get("fishSubmarine") + "\n"
                             + strFishPond(ItemID.IT_Blobfish, 9);
                    
                    case ItemID.IT_DinosaurEgg:
                        return strLocationalArtifact(locationList: new string[] { "locationMountains", "locationQuarry" }) + "\n"
                             + strFishingChest(2) + "\n"
                             + strDroppedByMonster("Pepper Rex") + "\n"
                             + strLocationalForage(isDesertKnown()? "locationSkullPrehistoric" : "locationUnknown") + "\n"
                             + str.Get(isTheatherKnown()? "winCraneGame" : "unknownSource");
                    
                    case ItemID.IT_MegaBomb:
                        return strCraftRecipe("Mega Bomb") + "\n"
                             + strDroppedByMonster("Squid Kid") + "\n"
                             + strDroppedByMonster("Iridium Bat") + "\n"
                             + strBuyFromDwarf() + "\n"
                             + str.Get(isTheatherKnown()? "winCraneGame" : "unknownSource");
                    
                    case ItemID.IT_SweetGemBerry:
                        return strGrowSeeds(ItemID.IT_RareSeed, parenthesize(str.Get("shopTravelingCartSeason",
                                                                             new { season = multiSeason("spring", "summer") })),
                                            quality);
                    
                    /********** [AlternateBundles] Pantry **********/
                    
                    // [AlternateBundles] Spring Crops Bundle
                    
                    case ItemID.IT_UnmilledRice:
                        return strSeasonalCrop("spring", quality, startingYear: 2);
                    
                    // [AlternateBundles] Artisan Bundle
                    
                    case ItemID.IT_AgedRoe:
                        return strPutItemInMachine(ItemID.IT_Roe, ItemID.BC_PreservesJar);
                    
                    /********** [AlternateBundles] Fish Tank **********/
                    
                    // [AlternateBundles] River Fish Bundle
                    
                    case ItemID.IT_SmallmouthBass:
                        return strFishBase(waterList: new string[] { "waterTown", "waterForestPond" },
                                           seasonList: new string[] { "spring", "fall" });
                    
                    case ItemID.IT_Salmon:
                        return strFishBase("waterRivers", "6am", "7pm", "fall");
                    
                    case ItemID.IT_Perch:
                        return strFishBase(waterList: new string[] { "waterRivers", "waterMountain", "waterForestPond" },
                                           seasonKey: "winter");
                    
                    case ItemID.IT_Pike:
                        return strFishBase(waterList: new string[] { "waterRivers", "waterForestPond" },
                                           seasonList: new string[] { "summer", "winter" });
                    
                    // [AlternateBundles] Lake Fish Bundle
                    
                    case ItemID.IT_RainbowTrout:
                        return strFishBase(waterList: new string[] { "waterRivers", "waterMountain" },
                                           start: "6am", end: "7pm", seasonKey: "summer");
                    
                    case ItemID.IT_Lingcod:
                        return strFishBase(waterList: new string[] { "waterRivers", "waterMountain" }, seasonKey: "winter");
                    
                    // [AlternateBundles] Ocean Fish Bundle
                    
                    case ItemID.IT_Anchovy:
                        return strFishBase("waterOcean", seasonList: new string[] { "spring", "fall" });
                    
                    case ItemID.IT_Herring:
                        return strFishBase("waterOcean", seasonList: new string[] { "spring", "winter" });
                    
                    case ItemID.IT_Flounder:
                        return strFishBase("waterOcean", "6am", "8pm", seasonList: new string[] { "spring", "summer" })
                             + (isIslandKnown()? "\n" + strFishBase(waterList: new string[] { "waterIslandOcean", "waterIslandCove" },
                                                                    start: "6am", end: "8pm") : "");
                    
                    case ItemID.IT_Halibut:
                        return strFishBase("waterOcean", "6am", "11am", start2: "7pm", end2: "2am",
                                           seasonList: new string[] { "spring", "summer", "winter" });
                    
                    // [AlternateBundles] Night Fishing Bundle
                    
                    case ItemID.IT_MidnightCarp:
                        return strFishBase(waterList: new string[] { "waterMountain", "waterForestPond" },
                                           start: "10pm", end: "2am", seasonList: new string[] { "fall", "winter" })
                             + (isIslandKnown()? "\n" + strFishBase("waterIslandRiver", "10pm", "2am") : "");
                    
                    case ItemID.IT_SuperCucumber:
                        return strFishBase("waterOcean", "6pm", "2am", seasonList: new string[] { "summer", "fall" }) + "\n"
                             + (isIslandKnown()? strFishBase(waterList: new string[] { "waterIslandOcean", "waterIslandCove" },
                                                             start: "6pm", end: "2am") + "\n" : "")
                             + str.Get("fishSubmarine");
                    
                    case ItemID.IT_Squid:
                        return strFishBase("waterOcean", "6pm", "2am", "winter");
                    
                    // [AlternateBundles] Specialty Fish Bundle
                    
                    case ItemID.IT_IcePip:
                        return strFishBase("waterMines60", fishingLevel: 5);
                    
                    case ItemID.IT_ScorpionCarp:
                        return strFishBase("waterDesert", "6am", "8pm");
                    
                    case ItemID.IT_Dorado:
                        return strFishBase("waterForestRiver", "6am", "7pm", "summer");
                    
                    /********** [AlternateBundles] Boiler Room **********/
                    
                    // [AlternateBundles] Blacksmith's Bundle
                    
                    case ItemID.IT_Coal:
                        return strHitRocks() + "\n"
                             + strBuyFrom("shopBlacksmith") + "\n"
                             + strDroppedByMonster("Dust Spirit") + "\n"
                             + strPutItemInMachine(ItemID.IT_Wood, ItemID.BC_CharcoalKiln, itemQuantity: 10) + "\n"
                             + strFishingChest() + "\n"
                             + strPanning();
                    
                    case ItemID.IT_RefinedQuartz:
                        return strPutItemInMachine(ItemID.IT_BrokenCD, ItemID.BC_RecyclingMachine) + "\n"
                             + strPutItemInMachine(ItemID.IT_BrokenGlasses, ItemID.BC_RecyclingMachine) + "\n"
                             + strPutItemInMachine(ItemID.IT_Quartz, ItemID.BC_Furnace) + "\n"
                             + strPutItemInMachine(ItemID.IT_FireQuartz, ItemID.BC_Furnace);
                    
                    /********** [AlternateBundles] Bulletin Board **********/
                    
                    // [AlternateBundles] Chef's Bundle
                    
                    case ItemID.IT_Omelet:
                        return strCookRecipe("Omelet");
                    
                    case ItemID.IT_Bread:
                        return strCookRecipe("Bread") + "\n"
                             + strBuyFrom("shopSaloon");
                    
                    // [AlternateBundles] Dye Bundle
                    
                    case ItemID.IT_Amethyst:
                        return str.Get("gemAmethyst") + "\n"
                             + strFishingChest(2) + "\n"
                             + strPanning();
                    
                    // [AlternateBundles] Field Research Bundle
                    
                    case ItemID.IT_Sap:
                        return strChopTrees() + "\n"
                             + strDroppedByMonster(monsterKey: "monsterGeneralSlime");
                    
                    // [AlternateBundles] Enchanter's Bundle
                    
                    case ItemID.IT_OilOfGarlic:
                        return strCraftRecipe("Oil Of Garlic") + "\n"
                             + strBuyFromDwarf();
                    
                    case ItemID.IT_SquidInk:
                        return strDroppedByMonster("Squid Kid") + "\n"
                             + strFishPond(ItemID.IT_Squid, 1) + "\n"
                             + strFishPond(ItemID.IT_MidnightSquid, 1);
                    
                    /********** [The Impossible Bundle] Pantry **********/
                    
                    // [The Impossible Bundle] Trash Bundle
                    
                    case ItemID.IT_Driftwood:
                        return str.Get("trash");
                    
                    // [The Impossible Bundle] Dwarf Bundle
                    
                    case ItemID.IT_DwarfScroll1:
                        return strLocationalForage("locationMines") + "\n"
                             + strDroppedByMonster("Bat") + "\n"
                             + strDroppedByMonster("Bug") + "\n"
                             + strDroppedByMonster("Cave Fly") + "\n"
                             + strDroppedByMonster("Duggy") + "\n"
                             + strDroppedByMonster("Green Slime") + "\n"
                             + strDroppedByMonster("Grub") + "\n"
                             + strDroppedByMonster("Rock Crab") + "\n"
                             + strDroppedByMonster("Stone Golem");
                    
                    case ItemID.IT_DwarfScroll2:
                        return strLocationalForage("locationMinesArea1") + "\n"
                             + strDroppedByMonster(monsterKey: "monsterBlueSlime") + "\n"
                             + strDroppedByMonster("Dust Spirit") + "\n"
                             + strDroppedByMonster("Frost Bat") + "\n"
                             + strDroppedByMonster("Ghost");
                    
                    case ItemID.IT_DwarfScroll3:
                        return strDroppedByMonster(monsterKey: "monsterBRPCISlime") + "\n"
                             + strDroppedByMonster("Lava Bat") + "\n"
                             + strDroppedByMonster("Lava Crab") + "\n"
                             + strDroppedByMonster("Squid Kid") + "\n"
                             + strDroppedByMonster("Shadow Brute") + "\n"
                             + strDroppedByMonster("Shadow Shaman") + "\n"
                             + strDroppedByMonster("Metal Head");
                    
                    case ItemID.IT_DwarfScroll4:
                        return strLocationalForage("locationMinesArea3") + "\n"
                             + strDroppedByMonster(monsterKey: "monsterGeneralAnyInMines");
                    
                    // [The Impossible Bundle] Tree Bundle
                    
                    case ItemID.IT_Acorn:
                        return str.Get("seedFromTree", new { tree = str.Get("treeOak") });
                    
                    case ItemID.IT_MapleSeed:
                        return str.Get("seedFromTree", new { tree = str.Get("treeMaple") });
                    
                    // [The Impossible Bundle] Chicken Bundle
                    
                    case ItemID.IT_Egg:
                        return strAnimalProduct("animalChicken");
                    
                    case ItemID.IT_BrownEgg:
                        return strAnimalProduct("animalChicken");
                    
                    case ItemID.IT_Mayonnaise:
                        return strPutItemInMachine(ItemID.IT_Egg, ItemID.BC_MayonnaiseMachine);
                    
                    case ItemID.IT_VoidMayonnaise:
                        return strPutItemInMachine(ItemID.IT_VoidEgg, ItemID.BC_MayonnaiseMachine);
                    
                    // [The Impossible Bundle] Geode Bundle
                    
                    case ItemID.IT_MagmaGeode:
                        return strHitRocks("locationMinesArea3") + "\n"
                             + strFishingChest() + "\n"
                             + strFishPond(ItemID.IT_LavaEel, 9);
                    
                    case ItemID.IT_OmniGeode:
                        return strHitRocks("locationMines21Plus") + "\n"
                            + strFishPond(ItemID.IT_Octopus, 9) + "\n"
                            + strDroppedByMonster("Carbon Ghost") + "\n"
                            + strBuyFromOasisWeekday("wednesday") + "\n"
                            + strBuyFromKrobusWeekday("tuesday") + "\n"
                            + strPanning() + "\n"
                            + str.Get(isTheatherKnown()? "winCraneGame" : "unknownSource");
                    
                    /********** [The Impossible Bundle] The Edited Bundle **********/
                    
                    case ItemID.IT_BasicFertilizer:
                        return strCraftRecipe("Basic Fertilizer") + "\n"
                             + strBuyFrom("shopPierre");
                    
                    /********** [Bundle Overhaul] Crafts Room **********/
                    
                    // [Bundle Overhaul] D.I.Y. Bundle
                    
                    case ItemID.IT_MixedSeeds:
                        return str.Get("clearWeeds") + "\n"
                             + str.Get("generalTilling")
                             + (Game1.player.FishingLevel < 2? "\n" + strFishingChest() : "") + "\n"
                             + strBuyFromKrobusWeekday("thursday");
                    
                    // [Bundle Overhaul] Reduce, Reuse, Recycle Bundle
                    
                    case ItemID.IT_JojaCola:
                        return str.Get("trash") + "\n"
                             + (isJojaOpen()? strBuyFrom("shopJoja") + "\n" : "")
                             + strBuyFrom("shopSaloon");
                    
                    /********** [Bundle Overhaul] Pantry **********/
                    
                    // [Bundle Overhaul] Artisan Products Bundle
                    
                    case ItemID.IT_DuckMayonnaise:
                        return strPutItemInMachine(ItemID.IT_DuckEgg, ItemID.BC_MayonnaiseMachine);
                    
                    /********** [Bundle Overhaul] Fish Tank **********/
                    
                    // [Bundle Overhaul] Beginner Fish Bundle
                    
                    case ItemID.IT_SeaCucumber:
                        return strFishBase("waterOcean", "6am", "7pm", seasonList: new string[] { "fall", "winter" }) + "\n"
                             + str.Get("fishSubmarine");
                    
                    // [Bundle Overhaul] Advanced Fish Bundle
                    
                    case ItemID.IT_RedMullet:
                        return strFishBase("waterOcean", "6am", "7pm", seasonList: new string[] { "summer", "winter" });
                    
                    // [Bundle Overhaul] Expert Fish Bundle
                    
                    case ItemID.IT_Albacore:
                        return strFishBase("waterOcean", "6am", "11am", start2: "6pm", end2: "2am",
                                           seasonList: new string[] { "fall", "winter" });
                    
                    case ItemID.IT_Octopus:
                        return strFishBase("waterOcean", "6am", "1pm", "summer") + "\n"
                             + (isIslandKnown()? strFishBase("waterIslandOceanWest", "6am", "1pm") + "\n" : "")
                             + str.Get("fishSubmarine");
                    
                    // [Bundle Overhaul] Specialty Fish Bundle
                    
                    case ItemID.IT_Stonefish:
                        return strFishBase("waterMines20", fishingLevel: 3);
                    
                    case ItemID.IT_LavaEel:
                        return strFishBase("waterMines100", fishingLevel: 7);
                    
                    /********** [Bundle Overhaul] Boiler Room **********/
                    
                    // [Bundle Overhaul] Geology Bundle
                    
                    case ItemID.IT_PetrifiedSlime:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Jamborite:
                        return strOpenGeode(ItemID.IT_Geode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Fluorapatite:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_FairyStone:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Kyanite:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_FireOpal:
                        return strOpenGeode(ItemID.IT_MagmaGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Neptunite:
                        return strOpenGeode(ItemID.IT_MagmaGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_Helvite:
                        return strOpenGeode(ItemID.IT_MagmaGeode, ItemID.IT_OmniGeode);
                    
                    case ItemID.IT_StarShards:
                        return strOpenGeode(ItemID.IT_MagmaGeode, ItemID.IT_OmniGeode);
                    
                    /********** [Bundle Overhaul] Bulletin Board **********/
                    
                    // [Bundle Overhaul] Cooking Class Bundle
                    
                    case ItemID.IT_CompleteBreakfast:
                        return strCookRecipe("Complete Breakfast");
                    
                    case ItemID.IT_CrabCakes:
                        return strCookRecipe("Crab Cakes") + "\n"
                             + strDroppedByMonster("Iridium Crab");
                    
                    case ItemID.IT_SurvivalBurger:
                        return strCookRecipe("Survival Burger");
                    
                    case ItemID.IT_PinkCake:
                        return strCookRecipe("Pink Cake");
                    
                    case ItemID.IT_CheeseCauliflower:
                        return strCookRecipe("Cheese Cauli.");
                    
                    case ItemID.IT_RootsPlatter:
                        return strCookRecipe("Roots Platter");
                    
                    case ItemID.IT_Sashimi:
                        return strCookRecipe("Sashimi");
                    
                    case ItemID.IT_PepperPoppers:
                        return strCookRecipe("Pepper Poppers");
                    
                    // [Bundle Overhaul] BYO Night Bundle
                    
                    case ItemID.IT_CranberryCandy:
                        return strCookRecipe("Cranberry Candy");
                    
                    case ItemID.IT_LifeElixir:
                        return strCraftRecipe("Life Elixir") + "\n"
                             + strDroppedByMonster("Iridium Bat");
                    
                    case ItemID.IT_GoatMilk:
                        return strAnimalProduct("animalGoat");
                    
                    case ItemID.IT_Milk:
                        return strAnimalProduct("animalCow");
                    
                    /********** [Very Hard Version] Crafts Room **********/
                    
                    // [Very Hard Version] Summer Foraging Bundle
                    
                    case ItemID.IT_FieldSnack:
                        return strCraftRecipe("Field Snack");
                    
                    /********** [Very Hard Version] Bulletin Board **********/
                    
                    // [Very Hard Version (Basic)] Cooking Bundle
                    
                    case ItemID.IT_VegetableMedley:
                        return strCookRecipe("Vegetable Stew");
                    
                    case ItemID.IT_EggplantParmesan:
                        return strCookRecipe("Eggplant Parm.");
                    
                    case ItemID.IT_RicePudding:
                        return strCookRecipe("Rice Pudding");
                    
                    case ItemID.IT_AutumnsBounty:
                        return strCookRecipe("Autumn's Bounty");
                    
                    case ItemID.IT_PumpkinSoup:
                        return strCookRecipe("Pumpkin Soup");
                    
                    case ItemID.IT_Stuffing:
                        return strCookRecipe("Stuffing");
                    
                    case ItemID.IT_FishStew:
                        return strCookRecipe("Fish Stew");
                    
                    case ItemID.IT_TomKhaSoup:
                        return strCookRecipe("Tom Kha Soup");
                    
                    // [Very Hard Version (Version C)] Cooking Bundle
                    
                    case ItemID.IT_BakedFish:
                        return strCookRecipe("Baked Fish");
                    
                    case ItemID.IT_GlazedYams:
                        return strCookRecipe("Glazed Yams");
                    
                    case ItemID.IT_Pancakes:
                        return strCookRecipe("Pancakes");
                    
                    case ItemID.IT_TroutSoup:
                        return strCookRecipe("Trout Soup");
                    
                    case ItemID.IT_StirFry:
                        return strCookRecipe("Stir Fry");
                    
                    case ItemID.IT_RadishSalad:
                        return strCookRecipe("Radish Salad");
                    
                    /********** [Minerva's Harder CC] Fish Tank **********/
                    
                    // [Minerva's Harder CC] Winter Fish Bundle
                    
                    case ItemID.IT_MidnightSquid:
                        return str.Get("fishSubmarine");
                    
                    case ItemID.IT_SpookFish:
                        return str.Get("fishSubmarine");
                    
                    /********** [Minerva's Harder CC] Boiler Room **********/
                    
                    // [Minerva's Harder CC] Blacksmith's Bundle
                    
                    case ItemID.IT_CopperOre:
                        return str.Get("mineCopperOre") + "\n"
                             + strBuyFrom("shopBlacksmith") + "\n"
                             + str.Get("openAnyGeode") + "\n"
                             + strFishingChest() + "\n"
                             + str.Get("artifactDigging") + "\n"
                             + strLocationalForage("locationMines") + "\n"
                             + strPanning();
                    
                    case ItemID.IT_IronOre:
                        return str.Get("mineIronOre") + "\n"
                             + strBuyFrom("shopBlacksmith") + "\n"
                             + str.Get("openAnyGeode") + "\n"
                             + strFishingChest() + "\n"
                             + strLocationalForage("locationMines") + "\n"
                             + strPanning();
                    
                    case ItemID.IT_GoldOre:
                        return str.Get("mineGoldOre") + "\n"
                             + strBuyFrom("shopBlacksmith") + "\n"
                             + strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_MagmaGeode, ItemID.IT_OmniGeode) + "\n"
                             + strFishingChest() + "\n"
                             + strDroppedByMonster("Ghost") + "\n"
                             + strPanning();
                    
                    /********** [Minerva's Harder CC] Bulletin Board **********/
                    
                    // [Minerva's Harder CC] Saloon Menu Bundle
                    
                    case ItemID.IT_PumpkinPie:
                        return strCookRecipe("Pumpkin Pie");
                    
                    case ItemID.IT_Chowder:
                        return strCookRecipe("Chowder");
                    
                    case ItemID.IT_CrispyBass:
                        return strCookRecipe("Crispy Bass");
                    
                    case ItemID.IT_FruitSalad:
                        return strCookRecipe("Fruit Salad");
                    
                    case ItemID.IT_ArtichokeDip:
                        return strCookRecipe("Artichoke Dip");
                    
                    case ItemID.IT_ParsnipSoup:
                        return strCookRecipe("Parsnip Soup");
                    
                    case ItemID.IT_Coleslaw:
                        return strCookRecipe("Coleslaw");
                    
                    // [Minerva's Harder CC] Enchanter's Bundle
                    
                    case ItemID.IT_GhostCrystal:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    /********** [Minerva's Harder CC (PPJA)] Crafts Room **********/
                    
                    // [Minerva's Harder CC (PPJA)] Spring Foraging Bundle
                    
                    case ItemID.IT_RiceShoot:
                        return strBuyFrom(shopLiteral: str.Get("shopPierreSeason",
                                          new { season = str.Get("spring"), startingYear = getStartingYearString(2) })) + "\n"
                             + strFishingChest() + "\n"
                             + strDroppedByMonster("Grub");
                    
                    // [Minerva's Harder CC (PPJA)] Crafting Bundle
                    
                    case ItemID.IT_WiltedBouquet:
                        return strPutItemInMachine(ItemID.IT_Bouquet, ItemID.BC_Furnace);
                    
                    // [Minerva's Harder CC (PPJA)] Specialty Foraging Bundle
                    
                    case ItemID.MFM_Starfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strLocationalForage("locationBeach");
                        else
                            return "";
                    
                    case ItemID.MFM_CrownOfThornsStarfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strLocationalForage("locationBeach");
                        else
                            return "";
                    
                    case ItemID.MFM_HolyGrenadeStarfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strLocationalForage("locationBeach");
                        else
                            return "";
                    
                    /********** [Minerva's Harder CC (PPJA)] Fish Tank **********/
                    
                    // [Minerva's Harder CC (PPJA)] Spring Fish Bundle
                    
                    case ItemID.MFM_Lionhead:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterForestPond", "8am", "4pm", "spring", "weatherSun");
                        else
                            return "";
                    
                    case ItemID.MFM_Pacu:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterRivers", "waterMountain" },
                                               seasonList: new string[] { "spring", "summer", "fall" });
                        else
                            return "";
                    
                    case ItemID.MFM_GreenTerror:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterTown", "6am", "6pm",
                                               seasonList: new string[] { "spring", "summer" }, weatherKey: "weatherSun");
                        else
                            return "";
                    
                    case ItemID.MFM_Ladyfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "6am", "7pm",
                                               seasonList: new string[] { "spring", "summer" }, weatherKey: "weatherSun");
                        else
                            return "";
                    
                    case ItemID.MFM_Barracuda:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "10am", "7pm", "spring", "weatherSun");
                        else
                            return "";
                    
                    case ItemID.MFM_ZebraTilapia:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterMountain", "8am", "4pm",
                                               seasonList: new string[] { "spring", "summer" }, weatherKey: "weatherSun");
                        else
                            return "";
                    
                    case ItemID.MFM_ClownLoach:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterMountain" },
                                               seasonList: new string[] { "spring", "summer" });
                        else
                            return "";
                    
                    case ItemID.MFM_Elephantfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterMountain" },
                                               start: "8am", end: "8pm", seasonList: new string[] { "spring", "summer" },
                                               weatherKey: "weatherSun");
                        else
                            return "";
                    
                    case ItemID.MFM_YamabukiKoi:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterWoods", "6am", "12am", "spring", "weatherRain");
                        else
                            return "";
                    
                    // [Minerva's Harder CC (PPJA)] Summer Fish Bundle
                    
                    case ItemID.MFM_Pangasius:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterMountain" },
                                               seasonList: new string[] { "spring", "summer", "fall" });
                        else
                            return "";
                    
                    case ItemID.MFM_KohakuKoi:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterWoods", seasonKey: "spring");
                        else
                            return "";
                    
                    case ItemID.MFM_Comet:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterForestPond", "8am", "4pm", "summer");
                        else
                            return "";
                    
                    case ItemID.MFM_Tucunare:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterForestRiver", "6am", "4pm", "summer", "weatherRain");
                        else
                            return "";
                    
                    case ItemID.MFM_FreshwaterPufferfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterRivers", seasonKey: "summer", weatherKey: "weatherRain");
                        else
                            return "";
                    
                    case ItemID.MFM_Anochoviella:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "6am", "4pm",
                                               seasonList: new string[] { "spring", "summer" }, weatherKey: "weatherRain");
                        else
                            return "";
                    
                    case ItemID.MFM_RibbonEel:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "8pm", "2am", "summer", "weatherSun");
                        else
                            return "";
                    
                    case ItemID.MFM_SmallMantaRay:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "8am", "2pm", "summer");
                        else
                            return "";
                    
                    // [Minerva's Harder CC (PPJA)] Fall Fish Bundle
                    
                    case ItemID.MFM_SmallSwordfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "12pm", "6pm", "fall");
                        else
                            return "";
                    
                    case ItemID.MFM_BlueRingedOctopus:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "6am", "12pm", "fall");
                        else
                            return "";
                    
                    case ItemID.MFM_GhostEel:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "8pm", "2am", "fall");
                        else
                            return "";
                    
                    case ItemID.MFM_ClownKnifefish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterMountain", "12pm", "12am", "fall");
                        else
                            return "";
                    
                    case ItemID.MFM_RedtailShark:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterMountain" },
                                               start: "10am", end: "8pm", seasonKey: "fall");
                        else
                            return "";
                    
                    case ItemID.MFM_GhostKoi:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterWoods", "6am", "12am", "fall", "weatherRain");
                        else
                            return "";
                    
                    case ItemID.MFM_Telescope:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterForestPond", "6am", "6pm", "fall", "weatherRain");
                        else
                            return "";
                    
                    case ItemID.MFM_Trahira:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterTown", "waterMountain" },
                                               seasonList: new string[] { "spring", "summer", "fall" }) + "\n"
                                 + strFishBase("waterForestRiver", seasonList: new string[] { "spring", "summer" }) + "\n"
                                 + strFishBase(isSewerKnown()? "waterSewer" : "waterUnknown");
                        else
                            return "";
                    
                    case ItemID.MFM_CommonPleco:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterTown" },
                                               start: "6pm", end: "2am", seasonList: new string[] { "summer", "fall" },
                                               weatherKey: "weatherRain") + "\n"
                                 + strFishBase("waterMountain", "6pm", "2am", "fall", "weatherRain");
                        else
                            return "";
                    
                    // [Minerva's Harder CC (PPJA)] Winter Fish Bundle
                    
                    case ItemID.MFM_SnowballPleco:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterTown", "waterMountain" },
                                               start: "6pm", end: "2am", seasonKey: "winter");
                        else
                            return "";
                    
                    case ItemID.MFM_ArcticChar:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterForestRiver", "8am", "6pm", "winter");
                        else
                            return "";
                    
                    case ItemID.MFM_Ide:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterMountain" },
                                               start: "6am", end: "6pm", seasonKey: "winter");
                        else
                            return "";
                    
                    case ItemID.MFM_ShiroUtsuriKoi:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterWoods", seasonKey: "winter");
                        else
                            return "";
                    
                    case ItemID.MFM_Sauger:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterMountain" },
                                               start: "8am", end: "6pm", seasonKey: "winter");
                        else
                            return "";
                    
                    case ItemID.MFM_Tench:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase(waterList: new string[] { "waterForestPond", "waterMountain" },
                                               start: "6am", end: "4pm", seasonList: new string[] { "fall", "winter" });
                        else
                            return "";
                    
                    case ItemID.MFM_Haddock:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "10am", "8pm", seasonList: new string[] { "fall", "winter" });
                        else
                            return "";
                    
                    case ItemID.MFM_Hagfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "8am", "6pm", "winter");
                        else
                            return "";
                    
                    case ItemID.MFM_KingCrab:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterOcean", "8am", "10pm", "winter");
                        else
                            return "";
                    
                    // [Minerva's Harder CC (PPJA)] Crab Pot Bundle
                    
                    case ItemID.MFM_FreshwaterCrab:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeFresh");
                        else
                            return "";
                    
                    case ItemID.MFM_FreshwaterShrimp:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeFresh");
                        else
                            return "";
                    
                    case ItemID.MFM_Jellyfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeOcean");
                        else
                            return "";
                    
                    case ItemID.MFM_SwimmerCrab:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeOcean");
                        else
                            return "";
                    
                    case ItemID.MFM_Prawn:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeOcean");
                        else
                            return "";
                    
                    case ItemID.MFM_Nautilus:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeOcean");
                        else
                            return "";
                    
                    case ItemID.MFM_BriefSquid:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeOcean");
                        else
                            return "";
                    
                    case ItemID.MFM_SandDollar:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeOcean");
                        else
                            return "";
                    
                    case ItemID.MFM_BlueDragonSlug:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strCrabPot("waterTypeOcean");
                        else
                            return "";
                    
                    // [Minerva's Harder CC (PPJA)] Specialty Fish Bundle
                    
                    case ItemID.MFM_TigerFish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterDesert", "8am", "2am");
                        else
                            return "";
                    
                    case ItemID.MFM_ElectricCatfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterDesert");
                        else
                            return "";
                    
                    case ItemID.MFM_Blinky:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterSewer");
                        else
                            return "";
                    
                    case ItemID.MFM_Glassfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterMines");
                        else
                            return "";
                    
                    case ItemID.MFM_Coelacanth:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterMines", "6am", "2am");
                        else
                            return "";
                    
                    case ItemID.MFM_Barreleye:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterMines", "8pm", "2am");
                        else
                            return "";
                    
                    case ItemID.MFM_Lungfish:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterDesert", "8am", "2am");
                        else
                            return "";
                    
                    /********** [Minerva's Harder CC (PPJA)] Boiler Room **********/
                    
                    // [Minerva's Harder CC (PPJA)] Geologist's Bundle
                    
                    case ItemID.IT_Soapstone:
                        return strOpenGeode(ItemID.IT_FrozenGeode, ItemID.IT_OmniGeode);
                    
                    /********** [Minerva's Harder CC (PPJA)] The Missing Bundle **********/
                    
                    case ItemID.MFM_TrappedSoul:
                        if (modRegistry.IsLoaded("MoreFish"))
                            return strFishBase("waterSwamp");
                        else
                            return "";
                    
                    /********** [Challenging Bundles Vanilla] Crafts Room **********/
                    
                    // [Challenging Vanilla] Construction Bundle
                    
                    case ItemID.IT_WoodFence:
                        return strCraftRecipe("Wood Fence");
                    
                    /********** [Challenging Bundles Vanilla] Bulletin Board **********/
                    
                    // [Challenging Vanilla] Chef's Bundle
                    
                    case ItemID.IT_TripleShotEspresso:
                        return strCookRecipe("Triple Shot Espresso");
                    
                    case ItemID.IT_Escargot:
                        return strCookRecipe("Escargot");
                   
                    case ItemID.IT_ShrimpCocktail:
                        return strCookRecipe("Shrimp Cocktail");
                    
                    // [Challenging Vanilla] Fodder Bundle
                    
                    case ItemID.IT_Seaweed:
                        return strFishBase("waterOcean") + "\n"
                             + (fixedBridgeToEastBeach()? strLocationalForage("locationBeachEast")
                                                        : strLocationalForage(locationLiteral: str.Get("locationBeachEastInaccessible",
                                                                              new { wood = getItemName(ItemID.IT_Wood) }))) + "\n"
                             + str.Get("trashCan");
                    
                    case ItemID.IT_GreenAlgae:
                        return strFishBase("waterRivers") + "\n"
                             + str.Get("trashCan");
                    
                    case ItemID.IT_WildBait:
                        return strCraftRecipe("Wild Bait");
                    
                    // [Challenging Vanilla] Field Research Bundle
                    
                    case ItemID.IT_RedPlate:
                        return strCookRecipe("Red Plate");
                    
                    /********** [Challenging Bundles Vanilla] The Missing Bundle **********/
                    
                    case ItemID.IT_MagicRockCandy:
                        return strBuyFrom("shopDesertTrader") + "\n"
                             + strDroppedByMonster("Haunted Skull");
                    
                    case ItemID.IT_Slimejack:
                        return strFishBase("waterBugLand");
                    
                    /********** [Challenging Bundles PPJA] Boiler Room **********/
                    
                    // [Challenging PPJA] Adventurer's Bundle
                    
                    case ItemID.IT_WhiteAlgae:
                        return strFishBase(waterList: new string[] { isSewerKnown()? "waterSewer" : "",
                                                                     isWitchSwampKnown()? "waterSwamp" : "",
                                                                     "waterMines" });
                    
                    case ItemID.IT_Bomb:
                        return strCraftRecipe("Bomb") + "\n"
                             + strBuyFromDwarf();
                    
                    case ItemID.IT_AncientSword:
                        return strLocationalArtifact(locationList: new string[] { "locationForest", "locationMountains" }) + "\n"
                             + strFishingChest(2);
                    
                    /********** [Challenging Bundles PPJA] Bulletin Board **********/
                    
                    // [Challenging PPJA] Dye Bundle
                    
                    case ItemID.IT_LuckyPurpleShorts:
                        return str.Get("luckyShorts")
                            + (modRegistry.IsLoaded("ppja.artisanvalleyPFM")? // Artisan Valley machine rules
                               "\n" + strPutItemInMachine(ItemID.IT_Amethyst, ItemID.BC_Loom) : "");
                    
                    // [Challenging PPJA] Field Research Bundle
                    
                    case ItemID.IT_AmphibianFossil:
                        return strLocationalArtifact(locationList: new string[] { "locationForest", "locationMountains" }) + "\n"
                             + strFishingChest(2);
                    
                    // [Challenging PPJA] Enchanter's Bundle
                    
                    case ItemID.IT_SeafoamPudding:
                        return strCookRecipe("Seafoam Pudding");
                    
                    case ItemID.IT_RedSlimeEgg:
                        return strPutItemInMachine(ItemID.IT_Slime, ItemID.BC_SlimeEggPress, itemQuantity: 100) + "\n"
                             + strDroppedByMonster(monsterKey: "monsterRedSlime");
                    
                    case ItemID.IT_WarpTotemFarm:
                        return strCraftRecipe("Warp Totem: Farm") + "\n"
                             + strFishPond(ItemID.IT_Blobfish, 9) + "\n"
                             + str.Get(isTheatherKnown()? "winCraneGame" : "unknownSource");
                    
                    case ItemID.IT_GoldenPumpkin:
                        return str.Get("goldenPumpkin");
                    
                    case ItemID.IT_DwarfGadget:
                        return strLocationalTilling("locationMinesArea2") + "\n"
                             + strOpenGeode(ItemID.IT_MagmaGeode, ItemID.IT_OmniGeode);
                }
                
                // If item ID was not matched and category is specified, look for category match.
                
                switch (category)
                {
                    /********** Bulletin Board (Remix) **********/
                    
                    // Home Cook's Bundle
                    
                    case StardewValley.Object.EggCategory:
                        return strAnimalProduct("animalChicken") + "\n"
                             + strAnimalProduct("animalDuck") + "\n"
                             + strAnimalProduct("animalOstrich");
                    
                    case StardewValley.Object.MilkCategory:
                        return strAnimalProduct("animalCow") + "\n"
                             + strAnimalProduct("animalGoat");
                }
                
                // If neither the item ID nor category was covered above (usually because it's a mod-added item), search by name.
                
                switch (getItemName(id, true))
                {
                    /********** [Minerva's Harder CC (PPJA)] Crafts Room **********/
                    
                    // [Minerva's Harder CC (PPJA)] Summer Foraging Bundle
                    
                    case "Asparagus":
                        return strSeasonalCrop("summer", quality, startingYear: 4) +
                               (modRegistry.IsLoaded("minervamaga.FTM.PPJAForage")? // Forage pack included with bundle mod
                                "\n" + strSeasonalForage("summer") : "");
                    
                    case "Raspberry":
                        return strSeasonalCrop("summer", quality, startingYear: 2) +
                               (modRegistry.IsLoaded("minervamaga.FTM.PPJAForage")? // Forage pack included with bundle mod
                                "\n" + strSeasonalForage("summer") : "");
                    
                    case "Summer Rose":
                        return strSeasonalCrop("summer", quality) +
                               (modRegistry.IsLoaded("minervamaga.FTM.PPJAForage")? // Forage pack included with bundle mod
                                "\n" + strSeasonalForage("summer") : "");
                    
                    // [Minerva's Harder CC (PPJA)] Fall Foraging Bundle
                    
                    case "Thyme":
                        return strSeasonalCrop("fall", quality) +
                               (modRegistry.IsLoaded("minervamaga.FTM.PPJAForage")? // Forage pack included with bundle mod
                                "\n" + strSeasonalForage("fall") : "");
                    
                    case "Barley":
                        return strSeasonalCrop("fall", quality, startingYear: 2) +
                               (modRegistry.IsLoaded("minervamaga.FTM.PPJAForage")? // Forage pack included with bundle mod
                                "\n" + strSeasonalForage("fall") : "");
                    
                    case "Fennel":
                        return strSeasonalCrop("fall", quality, startingYear: 2) +
                               (modRegistry.IsLoaded("minervamaga.FTM.PPJAForage")? // Forage pack included with bundle mod
                                "\n" + strSeasonalForage("fall") : "");
                    
                    // [Minerva's Harder CC (PPJA)] Crafting Bundle
                    
                    case "Beeswax":
                        return strPutItemInMachine(ItemID.IT_Honey, machineName: "Extruder");
                    
                    case "Thread":
                        return strPutItemInMachine(itemName: "Cotton Boll", machineID: ItemID.BC_Loom);
                    
                    case "Twine":
                        return strPutItemInMachine(ItemID.IT_Hay, ItemID.BC_Loom);
                    
                    /********** [Minerva's Harder CC (PPJA)] Boiler Room **********/
                    
                    // [Minerva's Harder CC (PPJA)] Adventurer's Bundle
                    
                    case "Prismatic Popsicle":
                        return strCookRecipe("Prismatic Popsicle");
                    
                    case "Berry Fusion Tea":
                        return strCookRecipe("Berry Fusion Tea");
                    
                    case "Black Tea":
                        return strCookRecipe("Black Tea");
                    
                    case "Cranberry Pomegranate Tea":
                        return strCookRecipe("Cranberry Pomegranate Tea");
                    
                    case "Southern Sweet Tea":
                        return strCookRecipe("Southern Sweet Tea");
                    
                    case "Surimi":
                        return strCookRecipe("Surimi");
                    
                    case "PB&J Sandwich":
                        return strCookRecipe("PB&J Sandwich");
                    
                    /********** [Minerva's Harder CC (PPJA)] The Missing Bundle **********/
                    
                    case "Cactus Flower":
                        return strSeasonalCrop(seasonList: new string[] { "spring", "summer", "fall" }, quality: quality,
                                               shopKey: "shopOasis", startingYear: 3);
                    
                    case "Grilled Zucchini":
                        return strCookRecipe("Grilled Zucchini");
                    
                    case "Habanero Extract":
                        return strPutItemInMachine(itemName: "Habanero", machineName: "Pepper Blender");
                    
                    case "Chocolate Mouse Bread":
                        return strCookRecipe("Chocolate Mouse Bread");
                    
                    /********** [Challenging PPJA] Crafts Room **********/
                    
                    // [Challenging PPJA] Spring Foraging Bundle
                    
                    case "Basil":
                        return strSeasonalCrop("spring", quality)
                             + (modRegistry.IsLoaded("alja.FTMCCCB")? // Challenging PPJA bundle forage pack
                                "\n" + strSeasonalForage("spring") : "");
                    
                    // [Challenging PPJA] Summer Foraging Bundle
                    
                    case "Blue Mist":
                        return strSeasonalCrop("summer", quality)
                             + (modRegistry.IsLoaded("alja.FTMCCCB")? // Challenging PPJA bundle forage pack
                                "\n" + strSeasonalForage("summer") : "");
                    
                    // [Challenging PPJA] Fall Foraging Bundle
                    
                    case "Shiitake Mushroom":
                        return strSeasonalCrop(seasonList: new string[] { "spring", "fall" }, quality: quality, startingYear: 4)
                             + (modRegistry.IsLoaded("alja.FTMCCCB")? // Challenging PPJA bundle forage pack
                                "\n" + strSeasonalForage("fall") : "");
                    
                    /********** [Challenging PPJA] Pantry **********/
                    
                    // [Challenging PPJA] Spring Crops Bundle
                    
                    case "Honeysuckle":
                        return strSeasonalCrop(seasonList: new string[] { "spring", "summer" }, quality: quality, shopKey: "shopMarnie");
                    
                    case "Spinach":
                        return strSeasonalCrop(seasonList: new string[] { "spring", "fall" }, quality: quality);
                    
                    case "Passion Fruit":
                        return strSeasonalCrop("spring", quality);
                    
                    // [Challenging PPJA] Summer Crops Bundle
                    
                    case "Chives":
                        return strSeasonalCrop("summer", quality);
                    
                    case "Clary Sage":
                        return strSeasonalCrop("summer", quality);
                    
                    case "Kiwi":
                        return strSeasonalCrop("summer", quality);
                    
                    case "Oregano":
                        return strSeasonalCrop("summer", quality);
                    
                    case "Sugar Cane":
                        return strSeasonalCrop("summer", quality, shopKey: isDesertKnown()? "shopOasis" : "shopUnknown");
                    
                    // [Challenging PPJA] Fall Crops Bundle
                    
                    case "Sweet Potato":
                        return strSeasonalCrop("fall", quality);
                    
                    case "Bell Pepper":
                        return strSeasonalCrop("fall", quality);
                    
                    case "Watermelon":
                        return strSeasonalCrop("fall", quality);
                    
                    case "Sweet Jasmine":
                        return strSeasonalCrop("fall", quality);
                    
                    case "Ginger":
                        return strSeasonalCrop("fall", quality, shopKey: isDesertKnown()? "shopOasis" : "shopUnknown");
                    
                    // [Challenging PPJA] Quality Crops Bundle
                    
                    case "Mint":
                        return strSeasonalCrop("winter", quality);
                    
                    case "Rose":
                        return strSeasonalCrop(seasonList: new string[] { "spring", "summer", "fall" }, quality: quality);
                    
                    case "Wasabi Root":
                        return strSeasonalCrop("summer", quality);
                    
                    case "Cotton Boll":
                        return strSeasonalCrop(seasonList: new string[] { "summer", "fall" }, quality: quality);
                    
                    // [Challenging PPJA] Animal Products Bundle
                    
                    case "Butter":
                        return strPutItemInMachine(ItemID.IT_Milk, machineName: "Butter Churn");
                    
                    // [Challenging PPJA] Artisan Products Bundle
                    
                    case "Fruit Juice":
                        return strPutItemInMachine(StardewValley.Object.FruitsCategory.ToString(), machineName: "Juicer");
                    
                    case "Apple Cider":
                        return strPutItemInMachine(itemName: "Wine Yeast", machineID: ItemID.BC_Keg);
                    
                    case "Essential Oil":
                        return strPutItemInMachine(itemName: "Dried Flower", machineName: "Drying Rack");
                    
                    case "Lime":
                        return strFruitTreeDuringSeason("treeLime", "spring");
                    
                    case "Papaya":
                        return strFruitTreeDuringSeason("treePapaya", "summer", isDesertKnown()? "shopOasis" : "shopUnknown");
                    
                    case "Walnut":
                        return strFruitTreeDuringSeason("treeWalnut", "fall");
                    
                    case "Persimmon":
                        return strFruitTreeDuringSeason("treePersimmon", "winter");
                    
                    /********** [Challenging PPJA] Bulletin Board **********/
                    
                    // [Challenging PPJA] Chef's Bundle
                    
                    case "Poached Pear":
                        return strCookRecipe("Poached Pear");
                    
                    case "Avocado Eel Roll":
                        return strCookRecipe("Avocado Eel Roll");
                    
                    case "Rich Tiramisu":
                        return strCookRecipe("Rich Tiramisu");
                    
                    case "Mushroom and Pepper Crepe":
                        return strCookRecipe("Mushroom and Pepper Crepe");
                    
                    case "Halloumi Burger":
                        return strCookRecipe("Halloumi Burger");
                    
                    case "Sparkling Wine":
                        return strPutItemInMachine(ItemID.IT_Wine, ItemID.BC_Keg);
                    
                    case "Gin":
                        return strPutItemInMachine(itemName: "Juniper Berry", machineName: "Still");
                    
                    case "Brandy":
                        return strPutItemInMachine(ItemID.IT_Wine, machineName: "Still");
                    
                    case "Affogato":
                        return strPutItemInMachine(itemName: "Vanilla Ice Cream", machineName: "Espresso Machine");
                    
                    // [Challenging PPJA] Dye Bundle
                    
                    case "Gold Thread":
                        return strPutItemInMachine(ItemID.IT_GoldBar, ItemID.BC_Loom);
                    
                    case "Yarn":
                        return strPutItemInMachine(ItemID.IT_Fiber, ItemID.BC_Loom);
                    
                    case "Berry Fusion Kombucha":
                        return strPutItemInMachine(itemName: "Berry Fusion Tea", machineName: "Glass Jar");
                    
                    case "Dried Flower":
                        return strPutItemInMachine(StardewValley.Object.flowersCategory.ToString(), machineName: "Drying Rack");
                    
                    case "Herbal Lavender":
                        return strSeasonalCrop("summer", quality);
                    
                    // [Challenging PPJA] Field Research Bundle
                    
                    case "Handmade Soap":
                        return strPutItemInMachine(StardewValley.Object.flowersCategory.ToString(), machineName: "Soap Press") + "\n"
                             + strPutItemInMachine(itemLiteral: str.Get("itemCategoryNut"), machineName: "Soap Press");
                    
                    case "Aloe":
                        return strSeasonalCrop("summer", quality, "shopClinic", startingYear: 2);
                    
                    case "Ground Vegetable":
                        return strPutItemInMachine(StardewValley.Object.VegetableCategory.ToString(), machineName: "Grinder");
                    
                    case "Breakfast Tea":
                        return strCookRecipe("Breakfast Tea");
                    
                    case "Dried Fruit":
                        return strPutItemInMachine(StardewValley.Object.FruitsCategory.ToString(), machineName: "Dehydrator");
                    
                    // [Challenging PPJA] Fodder Bundle
                    
                    case "Soybean":
                        return strSeasonalCrop("fall", quality, startingYear: 2);
                    
                    case "Cabbage":
                        return strSeasonalCrop(seasonList: new string[] { "spring", "fall" }, quality: quality);
                    
                    case "Carrot":
                        return strSeasonalCrop("fall", quality);
                    
                    case "Apple Rind":
                        return strPutItemInMachine(ItemID.IT_Apple, machineName: "Grinder");
                    
                    case "Granny Smith Apple":
                        return strFruitTreeDuringSeason("treeGranny", "fall");
                    
                    case "Eucalyptus Leaves":
                        return strFruitTreeDuringSeason("treeEucalyptus", "spring", "shopClinic", startingYear: 2);
                    
                    // [Challenging PPJA] Enchanter's Bundle
                    
                    case "Candle":
                        return strPutItemInMachine(StardewValley.Object.flowersCategory.ToString(), machineName: "Wax Barrel");
                    
                    case "Sun Tea":
                        return strCookRecipe("Sun Tea");
                    
                    case "Dried Mushroom":
                        return strPutItemInMachine(itemLiteral: str.Get("itemCategoryMushroom"), machineName: "Drying Rack");
                    
                    case "Moonshine":
                        return strPutItemInMachine(ItemID.IT_Corn, machineName: "Still");
                    
                    /********** [Challenging PPJA] The Missing Bundle **********/
                    
                    case "Prismatic Ice Cream":
                        return strPutItemInMachine(ItemID.IT_PrismaticShard, machineName: "Ice Cream Machine");
                    
                    case "Jalapeno Extract":
                        return strPutItemInMachine(itemName: "Jalapeno", machineName: "Pepper Blender");
                    
                    case "Popcorn":
                        return strCookRecipe("Popcorn");
                    
                    case "Wasabi Peas":
                        return strCookRecipe("Wasabi Peas");
                    
                    case "Seaweed Chips":
                        return strCookRecipe("Seaweed Chips");
                    
                    case "Hot Irish Coffee":
                        return strPutItemInMachine(itemName: "Whiskey", machineName: "Espresso Machine");
                    
                    case "Dark Ale":
                        return strPutItemInMachine(itemName: "Durum", machineID: ItemID.BC_Keg);
                    
                    case "Strawberry Lemonade":
                        return strCookRecipe("Strawberry Lemonade");
                    
                    case "Ice Cream Brownie":
                        return strCookRecipe("Ice Cream Brownie");
                    
                    /********** [Challenging Bundles Vanilla_SVE] Fish Tank **********/
                    
                    // [Challenging Vanilla_SVE] River Fish Bundle
                    
                    case "Butterfish":
                        return strFishBase("waterShearwaterBridge",
                                           seasonList: new string[] { "spring", "summer", "fall" }, weatherKey: "weatherSun");
                    
                    // [Challenging Vanilla_SVE] Lake Fish Bundle
                    
                    case "Minnow":
                        return strFishBase(waterList: new string[] { "waterBlueMoonVineyard", "waterShearwaterBridge", "waterAdventurerSummit" },
                                           start: "6am", end: "6pm");
                    
                    // [Challenging Vanilla_SVE] Ocean Fish Bundle
                    
                    case "Clownfish":
                        return strFishBase(waterList: new string[] { "waterBeach", "waterBlueMoonVineyard" },
                                           start: "10am", end: "5pm", weatherKey: "weatherSun")
                             + (isIslandKnown()? "\n" + strFishBase("waterIsland", "10am", "5pm", weatherKey: "weatherSun") : "");
                    
                    case "Starfish":
                        return strFishBase(waterList: new string[] { "waterBeach", "waterBlueMoonVineyard" },
                                           seasonList: new string[] { "spring", "summer", "fall" }, start: "6am", end: "10pm")
                             + (isIslandKnown()? "\n" + strFishBase("waterIsland", "6am", "10pm") : "");
                    
                    // [Challenging Vanilla_SVE] Specialty Fish Bundle
                    
                    case "Puppyfish":
                        return strFishBase("waterShearwaterBridge", seasonList: new string[] { "spring", "summer", "fall" }) + "\n"
                             + strFishBase("waterForestWest", seasonKey: "summer");
                    
                    /********** [Challenging Bundles Vanilla_SVE] Bulletin Board **********/
                    
                    // [Challenging Vanilla_SVE] Chef's Bundle
                    
                    case "Big Bark Burger":
                        return strCookRecipe("Big Bark Burger");
                        // (cooking recipe from saloon if you have 5 hearts with gus)
                    
                    case "Glazed Butterfish":
                        return strCookRecipe("Glazed Butterfish") + "\n"
                             + strFishPond(fishItemName: "Butterfish", numRequired: 10);
                    
                    // [Challenging Vanilla_SVE] Field Research Bundle
                    
                    case "King Salmon":
                        return strFishBase("waterForestWest", "6am", "10pm", seasonList: new string[] { "spring", "summer" });
                    
                    // [Challenging Vanilla_SVE] Enchanter's Bundle
                    
                    case "Frog":
                        return strFishBase("waterMountain", "6pm", "2am",
                                           seasonList: new string[] { "spring", "summer" }, weatherKey: "weatherRain");
                    
                    /********** [Challenging Bundles Vanilla_SVE] The Missing Bundle **********/
                    
                    case "Void Pebble":
                        return strFishPond(fishItemName: "Void Eel", numRequired: 8) + "\n"
                             + strDroppedByMonster("Mummy") + "\n"
                             + strDroppedByMonster("Serpent") + "\n"
                             + strDroppedByMonster("Carbon Ghost");
                    
                    case "Void Salmon Sushi":
                        return strCookRecipe("Void Salmon Sushi");
                    
                    case "Void Delight":
                        return strCookRecipe("Void Delight");
                }
                
                return ModEntry.debugShowUnknownIDs? "??? (ID: " + id + ")" : "";
            }
            catch (Exception e)
            {
                ModEntry.Log("Error in getHintText: " + e.Message + Environment.NewLine + e.StackTrace);
                return "";
            }
        }
        
        /// <summary>Returns short hint text for some items, for use in another item's suggestion.</summary>
        /// <param name="id">The item ID.</param>
        public static string getShortHint(string id)
        {
            try
            {
                switch (id)
                {
                    case ItemID.IT_Hops:
                        return str.Get("shortHintSeasonalCrop", new { season = str.Get("summer") });
                    
                    case ItemID.IT_Wheat:
                    case ItemID.IT_Corn:
                    case ItemID.IT_Sunflower:
                        return str.Get("shortHintSeasonalCrop", new { season = multiSeason("summer", "fall") });
                    
                    case ItemID.IT_SunflowerSeeds:
                        return str.Get("shortHintSeasonalSeed", new { season = multiSeason("summer", "fall") });
                    
                    case ItemID.IT_SoggyNewspaper:
                    case ItemID.IT_BrokenCD:
                    case ItemID.IT_BrokenGlasses:
                        return str.Get("shortHintTrash");
                    
                    case ItemID.IT_Roe:
                        return str.Get("shortHintFishPond");
                    
                    case ItemID.IT_Fiber:
                        return str.Get("shortHintFiber");
                    
                    case ItemID.IT_Hay:
                        return str.Get("shortHintHay", new { wheat = getItemName(ItemID.IT_Wheat) });
                    
                    case ItemID.IT_Wine:
                        return str.Get("shortHintMachine", new { item = getItemName(StardewValley.Object.FruitsCategory.ToString()),
                                                                 machine = getBigCraftableName(ItemID.BC_Keg) });
                    
                    case ItemID.IT_Egg:
                        return str.Get("animalChicken");
                    
                    case ItemID.IT_DuckEgg:
                        return str.Get("animalDuck");
                    
                    case ItemID.IT_VoidEgg:
                        return str.Get("animalVoidChicken");
                    
                    case ItemID.IT_Milk:
                        return str.Get("animalCow");
                    
                    case ItemID.IT_GoatMilk:
                        return str.Get("animalGoat");
                    
                    case ItemID.IT_Truffle:
                        return str.Get("animalPig");
                    
                    case ItemID.IT_Wool:
                        return multiKey("animalSheep", "animalRabbit");
                    
                    case ItemID.IT_Bouquet:
                        return str.Get("shopPierre");
                    
                    case ItemID.IT_CoffeeBean:
                        return str.Get("shopTravelingCartRandom");
                    
                    case ItemID.IT_TeaLeaves:
                        return str.Get("recipeSourceHeartEvent", new { person = getPersonName("Caroline"), hearts = 2 });
                    
                    case ItemID.IT_Honey:
                        return getBigCraftableName(ItemID.BC_BeeHouse);
                    
                    case ItemID.IT_Quartz:
                    case ItemID.IT_FireQuartz:
                    case ItemID.IT_Amethyst:
                        return str.Get("locationMines");
                }
                
                // If ID was not covered in the above switch statement (usually because it's a mod-added item), search by name.
                
                string itemByName = "";
                string machineByName = "";
                
                switch (getItemName(id, true))
                {
                    case "Cotton Boll":
                        return str.Get("shortHintSeasonalCrop", new { season = multiSeason(new string[] { "summer", "fall" }) });
                    
                    case "Jalapeno":
                    case "Durum":
                        return str.Get("shortHintSeasonalCrop", new { season = str.Get("fall") });
                    
                    case "Habanero":
                        return str.Get("shortHintSeasonalCropStartingYear", new { season = str.Get("fall"), year = 3 });
                    
                    case "Juniper Berry":
                        return str.Get("shortHintSeasonalCrop", new { season = str.Get("winter") });
                    
                    case "Wine Yeast":
                        return str.Get("shopPierre");
                    
                    case "Berry Fusion Tea":
                        return str.Get("shopSaloon");
                    
                    case "Dried Flower":
                        if (findBigCraftableIDByName("Drying Rack", out machineByName))
                            return getBigCraftableName(machineByName);
                        else
                            return "";
                    
                    case "Vanilla Ice Cream":
                        if (findBigCraftableIDByName("Ice Cream Machine", out machineByName))
                            return getBigCraftableName(machineByName);
                        else
                            return "";
                    
                    case "Whiskey":
                        if (findItemIDByName("Wheat Malt", out itemByName)
                         && findBigCraftableIDByName("Still", out machineByName))
                            return str.Get("shortHintMachine", new { item = getItemName(itemByName),
                                                                     machine = getBigCraftableName(machineByName) });
                        else
                            return "";
                }
                
                return "";
            }
            catch (Exception e)
            {
                ModEntry.Log("Error in getShortHint: " + e.Message + Environment.NewLine + e.StackTrace);
                return "";
            }
        }
       
        /*******************************
         ** Suggestion String Methods **
         *******************************/
        
        /// <summary>Suggestion for a crop grown during particular season(s).</summary>
        /// <param name="seasonKey">String key for season name (spring/summer/fall/winter).</param>
        /// <param name="quality">The quality required. Fertilizer is recommended for quality crops.</param>
        /// <param name="shopKey">String key for shop that sells the seeds (shopX). Defaults to Pierre's/JojaMart.</param>
        /// <param name="startingYear">What year the crop becomes available.</param>
        /// <param name="seasonList">Override for array of multiple season keys.</param>
        private static string strSeasonalCrop(string seasonKey = "", int quality = 0, string shopKey = "",
                                              int startingYear = 0, string[] seasonList = null)
        {
            string yearStr = getStartingYearString(startingYear);
            string qualityCrop = quality > 0? str.Get("qualityCrop") : "";
            
            return str.Get("seasonalCrop", new { seedShop = shopKey != ""? str.Get(shopKey) : getSeedShopsString(),
                                                 season = seasonList != null? multiSeason(seasonList) : str.Get(seasonKey),
                                                 startingYear = yearStr, qualityCrop = qualityCrop });
        }
        
        /// <summary>Suggestion for a crop grown from particular seeds, no specification of season.</summary>
        /// <param name="itemID">Item ID of the seeds.</param>
        /// <param name="seedTip">Tip for how to get seeds.</param>
        /// <param name="quality">The quality required.</param>
        private static string strGrowSeeds(string itemID, string seedTip, int quality)
        {
            string qualityCrop = quality > 0? str.Get("qualityCrop") : "";
            
            return str.Get("growSeeds", new { seeds = getItemName(itemID) + seedTip, qualityCrop = qualityCrop });
        }
        
        /// <summary>Suggestion for an item forageable during particular season(s).</summary>
        /// <param name="seasonKey">String key for season name (spring/summer/fall/winter).</param>
        /// <param name="seasonList">Override for array of multiple season keys.</param>
        private static string strSeasonalForage(string seasonKey = "", string[] seasonList = null)
        {
            return str.Get("seasonalForage", new { season = seasonList != null? multiSeason(seasonList) : str.Get(seasonKey) });
        }
        
        /// <summary>Suggestion of an item forageable in particular location.</summary>
        /// <param name="locationKey">String key for location name (locationX).</param>
        /// <param name="locationLiteral">Override for preformatted text.</param>
        private static string strLocationalForage(string locationKey = "", string locationLiteral = "")
        {
            return str.Get("locationalForage", new { location = locationLiteral != ""? locationLiteral : str.Get(locationKey) });
        }
        
        /// <summary>Suggestion of an item forageable in particular location during particular season(s).</summary>
        /// <param name="seasonKey">String key for season name (spring/summer/fall/winter).</param>
        /// <param name="locationKey">String key for location name (locationX).</param>
        /// <param name="seasonList">Override for array of multiple season keys.</param>
        /// <param name="locationLiteral">Override for preformatted text.</param>
        private static string strSeasonalLocationalForage(string seasonKey = "", string locationKey = "",
                                                          string[] seasonList = null, string locationLiteral = "")
        {
            return str.Get("seasonalLocationalForage", new { season = seasonList != null? multiSeason(seasonList) : str.Get(seasonKey),
                                                             location = locationLiteral != ""? locationLiteral :str.Get(locationKey) });
        }
        
        /// <summary>Suggestion for a diggable item during particular season(s).</summary>
        /// <param name="seasonKey">String key for season name (spring/summer/fall/winter).</param>
        /// <param name="seasonList">Override for array of multiple season keys.</param>
        private static string strSeasonalTilling(string seasonKey = "", string[] seasonList = null)
        {
            return str.Get("seasonalTilling", new { season = seasonList != null? multiSeason(seasonList) : str.Get(seasonKey) });
        }
        
        /// <summary>Suggestion for a diggable item in particular location.</summary>
        /// <param name="locationKey">String key for location name (locationX).</param>
        /// <param name="locationLiteral">Override for preformatted text.</param>
        private static string strLocationalTilling(string locationKey = "", string locationLiteral = "")
        {
            return str.Get("locationalTilling", new { location = locationLiteral != ""? locationLiteral : str.Get(locationKey) });
        }
        
        /// <summary>Suggestion for a diggable artifact in particular location.</summary>
        /// <param name="locationKey">String key for location name (locationX).</param>
        /// <param name="locationList">Override for array of multiple location keys.</param>
        private static string strLocationalArtifact(string locationKey = "", string[] locationList = null)
        {
            return str.Get("locationalArtifact", new { location = locationList != null? multiKey(locationList) : str.Get(locationKey) });
        }
        
        /// <summary>Suggestion to hit rocks with pickaxe.</summary>
        /// <param name="locationKey">String key for specific location.</param>
        private static string strHitRocks(string locationKey = "")
        {
            if (locationKey != "")
                return str.Get("hitRocksInLocation", new { location = str.Get(locationKey) });
            else // Load "Pickaxe" tool name from strings
                return str.Get("hitRocks", new { pickaxe = TokenParser.ParseText(DataLoader.Tools(Game1.content)["Pickaxe"].DisplayName) });
        }
        
        /// <summary>Suggestion for where and when to catch fish.</summary>
        /// <param name="waterKey">String key for water location (waterX).</param>
        /// <param name="start">Start of catchable time range (i.e. "6am", "12pm").</param>
        /// <param name="end">End of catchable time range (i.e. "6am", "12pm").</param>
        /// <param name="seasonKey">String key for season name (spring/summer/fall/winter).</param>
        /// <param name="weatherKey">String key for specific weather (weatherSun, weatherRain).)</param>
        /// <param name="waterList">Override for array of multiple water keys.</param>
        /// <param name="seasonList">Override for array of multiple season keys.</param>
        /// <param name="start2">Start of second catchable time range (i.e. "6am", "12pm").</param>
        /// <param name="end2">End of second catchable time range (i.e. "6am", "12pm").</param>
        /// <param name="fishingLevel",>The minimum Fishing level required.</param>
        /// <param name="extraLinebreak">Whether to include an extra linebreak in the middle for long lines.</param>
        private static string strFishBase(string waterKey = "", string start = "", string end = "",
                                          string seasonKey = "allSeasons", string weatherKey = "",
                                          string[] waterList = null, string[] seasonList = null,
                                          string start2 = "", string end2 = "", int fishingLevel = 0, bool extraLinebreak = false)
        {
            string timeStr;
            if (start != "" && end != "")
            {
                timeStr = getTimeRange(start, end);
                if (start2 != "" && end2 != "")
                    timeStr += str.Get("multipleThingSeparator") + getTimeRange(start2, end2);
            }
            else
                timeStr = str.Get("anyTime");
            
            string water = waterList != null? multiKey(waterList) : str.Get(waterKey);
            string season = seasonList != null? multiSeason(seasonList) : str.Get(seasonKey);
            string weather = weatherKey != ""? str.Get(weatherKey) : "";
            
            return str.Get("fishBase", new { water = water,
                                             extraLinebreak = extraLinebreak? "\n" : "",
                                             time = timeStr,
                                             weather = weather,
                                             season = season })
                 + (fishingLevel > 0? levelRequirementString("fishing", fishingLevel) : "");
        }
        
        /// <summary>Suggestion for items randomly obtainable from fishing chests.</summary>
        /// <param name="level">The minimum Fishing level at which the item will start appearing.</param>
        private static string strFishingChest(int level = 0)
        {
            string levelStr = "";
            if (level != 0 && Game1.player.FishingLevel < level)
                levelStr = levelRequirementString("fishing", level);
            
            return str.Get("fishingChest") + levelStr;
        }
        
        /// <summary>Suggestion for items catchable in a Crab Pot.</summary>
        /// <param name="waterTypeKey">String key for type of water (waterTypeX).</param>
        private static string strCrabPot(string waterTypeKey)
        {
            return str.Get("crabPot", new { crabPot = getItemName(ItemID.IT_CrabPot), waterType = str.Get(waterTypeKey) });
        }
        
        /// <summary>Suggestion for an item produced by an animal. Includes building requirement for said animal.</summary>
        /// <param name="animalKey">String key for animal name (animalX).</param>
        /// <param name="mustBeHappy">Whether the animal has to be sufficiently happy to produce the item.</param>
        /// <param name="mustBeOutside">Whether the animal has to be outside to produce the item.</param>
        private static string strAnimalProduct(string animalKey, bool mustBeHappy = false, bool mustBeOutside = false)
        {
            int coopReq = -1, barnReq = -1;
            switch (animalKey)
            {
                case "animalChicken": coopReq = 0; break;
                case "animalVoidChicken": coopReq = 1; break;
                case "animalDuck": coopReq = 1; break;
                case "animalRabbit": coopReq = 2; break;
                case "animalCow": barnReq = 0; break;
                case "animalOstrich": barnReq = 0; break;
                case "animalGoat": barnReq = 1; break;
                case "animalSheep": barnReq = 2; break;
                case "animalPig": barnReq = 2; break;
            }
            
            int coopLevel = getCoopLevel();
            int barnLevel = getBarnLevel();
            
            string animalUnlockTip = "";
            if (coopLevel < coopReq)
                animalUnlockTip = parenthesize(str.Get("animalReqCoopLv" + (coopReq + 1)));
            else if (barnLevel < barnReq)
                animalUnlockTip = parenthesize(str.Get("animalReqBarnLv" + (barnReq + 1)));
            
            string details = "";
            if (animalUnlockTip == "") // Details not really necessary if animal is not even obtainable yet
                details = mustBeHappy? str.Get("inGoodMood") : mustBeOutside? str.Get("animalOutside") : "";
            
            return str.Get("animalProduct", new { animal = str.Get(animalKey) + animalUnlockTip, details = details });
        }
        
        /// <summary>Suggestion for an item produced by putting an item in a machine.
        /// Includes requirement for said machine's recipe.</summary>
        /// <param name="itemID">Item ID of the item to put in.</param>
        /// <param name="machineID">Big Craftable ID of the machine.</param>
        /// <param name="itemLiteral">Override for preformatted text.</param>
        /// <param name="itemQuantity">Required quantity of input item.</param>
        /// <param name="itemName">String referencr for mod-added items.</param>
        /// <param name="machineName">String reference for mod-added machines.</param>
        private static string strPutItemInMachine(string itemID = "", string machineID = "",
                                                  string itemLiteral = "", int itemQuantity = 1,
                                                  string itemName = "", string machineName = "")
        {
            if (itemName != "")
            {
                if (!findItemIDByName(itemName, out itemID))
                    return "";
            }
            
            if (machineName != "")
            {
                if (!findBigCraftableIDByName(machineName, out machineID))
                    return "";
            }
            else
                machineName = getBigCraftableName(machineID, true);
            
            bool canMakeMachine = Game1.MasterPlayer.craftingRecipes.ContainsKey(machineName);
            
            string itemTip = getShortHint(itemID);
            if (itemTip != "")
                itemTip = parenthesize(itemTip);
            
            string machineUnlockTip = "";
            if (!canMakeMachine)
            {
                machineUnlockTip = getCraftingRecipeSources(machineName);
                if (machineUnlockTip != "")
                    machineUnlockTip = parenthesize(machineUnlockTip);
            }
            
            string itemStr = itemLiteral != ""? itemLiteral : getItemName(itemID);
            
            if (itemQuantity > 1)
                itemStr = str.Get("machineItemQuantity", new { item = itemStr, num = itemQuantity });
            
            return str.Get("putItemInMachine", new { rawItem = itemStr + itemTip,
                                                     machine = getBigCraftableName(machineID) + machineUnlockTip });
        }
        
        /// <summary>Suggestion for putting raw item in Keg for base quality, or result item in Cask for higher quality.</summary>
        /// <param name="rawItemID">Item ID of the raw material to put in the base machine.</param>
        /// <param name="machineID">Big Craftable ID of the base machine.</param>
        /// <param name="resultItemID">Item ID of the result you get from the base machine.</param>
        /// <param name="quality">The quality required for the bundle.</param>
        private static string strMachineOrCaskForQuality(string rawItemID, string machineID, string resultItemID, int quality)
        {
            if (quality == 0) // Base quality, so put raw item in "base" machine
                return strPutItemInMachine(rawItemID, machineID);
            else // Higher quality requires putting raw item in base machine, then result in Cask
            {
                string machineName = getBigCraftableName(machineID, true);
                bool canMakeMachine = Game1.MasterPlayer.craftingRecipes.ContainsKey(machineName);
                
                string machineUnlockTip = "";
                if (!canMakeMachine)
                {
                    machineUnlockTip = getCraftingRecipeSources(machineName);
                    if (machineUnlockTip != "")
                        machineUnlockTip = parenthesize(machineUnlockTip);
                }
                
                string caskName = getBigCraftableName(ItemID.BC_Cask, true);
                bool canMakeCask = Game1.MasterPlayer.craftingRecipes.ContainsKey(caskName);
                
                string caskUnlockTip = "";
                if (!canMakeCask)
                {
                    caskUnlockTip = getCraftingRecipeSources(caskName);
                    if (caskUnlockTip != "")
                        caskUnlockTip = parenthesize(caskUnlockTip);
                }
                
                return str.Get("putItemInMachineThenCask", new { rawItem = getItemName(rawItemID),
                                                                 machine = getBigCraftableName(machineID) + machineUnlockTip,
                                                                 resultItem = getItemName(resultItemID),
                                                                 cask = getBigCraftableName(ItemID.BC_Cask) + caskUnlockTip});
            }
        }
        
        /// <summary>Suggestion for an item produced by a machine on its own.
        /// Includes requirement for said machine's recipe.</summary>
        /// <param name="machineID">Big Craftable ID of the machine.</param>
        private static string strNoItemMachine(string machineID)
        {
            string machineUnlockTip = "";
            
            // Statue of Perfection isn't crafted, only obtained, so it's a special case.
            if (machineID == ItemID.BC_StatueOfPerfection
             && !Utility.doesItemExistAnywhere("(BC)" + ItemID.BC_StatueOfPerfection))
            {
                if (Game1.getFarm().grandpaScore.Value < 4) // Haven't had evaluation, or didn't get four candles
                    machineUnlockTip = parenthesize(str.Get("statueOfPerfectionTip"));
                else // Got four candles, so it's waiting to be collected at shrine
                    machineUnlockTip = parenthesize(str.Get("statueOfPerfectionAvailable"));
            }
            else
            {
                string machineName = getBigCraftableName(machineID, true);
                bool canMakeMachine = Game1.MasterPlayer.craftingRecipes.ContainsKey(machineName);
                
                if (!canMakeMachine)
                {
                    machineUnlockTip = getCraftingRecipeSources(machineName);
                    if (machineUnlockTip != "")
                        machineUnlockTip = parenthesize(machineUnlockTip);
                }
            }
            
            return str.Get("noItemMachine", new { machine = getBigCraftableName(machineID) + machineUnlockTip });
        }
        
        /// <summary>Suggestion for getting item from a fish pond with a minimum population of a certain fish.</summary>
        /// <param name="fishItemID">Item ID of the fish.</param>
        /// <param name="numRequired">Minimum number of fish required in the pond for item to spawn.</param>
        /// <param name="fishItemName">String reference for mod-added fish.</param>
        private static string strFishPond(string fishItemID = "", int numRequired = 1, string fishItemName = "")
        {
            if (fishItemName != "")
            {
                if (!findItemIDByName(fishItemName, out fishItemID))
                    return "";
            }
            
            return str.Get("fishPond", new { fish = getItemName(fishItemID), num = numRequired });
        }
        
        /// <summary>Suggestion for an item dropped by a monster. Includes location of said monster.</summary>
        /// <param name="monsterName">Internal string name of the monster.</param>
        /// <param name="monsterKey">String key for general monster category or specific monster color (monsterX).</param>
        private static string strDroppedByMonster(string monsterName = "", string monsterKey = "")
        {
            string monsterStr = monsterKey != ""? str.Get(monsterKey) : getMonsterName(monsterName);
            
            // Set location and requirements based on monster name or type.
            string locationKey = "";
            int startFloor = -1, endFloor = -1;
            int mineLevelReq = -1;
            bool skullReq = false, volcanoReq = false;
            
            if (monsterKey != "")
            {
                switch(monsterKey)
                {
                    case "monsterGeneralSlime":
                    case "monsterBRPCISlime":
                        locationKey = "monsterAreaMinesAndSkull"; break;
                    case "monsterGeneralBat":
                    case "monsterGeneralAnyInMines":
                        locationKey = "monsterAreaMinesAll"; break;
                    case "monsterPurpleSlime":
                        locationKey = "monsterAreaSkull"; break;
                }
            }
            else
            {
                switch (monsterName)
                {
                    case "Green Slime":
                        locationKey = "monsterAreaMinesAndSkull";
                        break;
                    case "Haunted Skull":
                        locationKey = "monsterAreaMinesAll";
                        break;
                    case "Bug":
                    case "Cave Fly":
                    case "Rock Crab":
                    case "Blue Squid":
                        startFloor = 1;
                        endFloor = 29;
                        locationKey = "monsterAreaMinesRange";
                        break;
                    case "Duggy":
                        mineLevelReq = 5;
                        startFloor = 6;
                        endFloor = 29;
                        locationKey = "monsterAreaMinesRange";
                        break;
                    case "Grub":
                        mineLevelReq = 10;
                        startFloor = 11;
                        endFloor = 29;
                        locationKey = "monsterAreaMinesRange";
                        break;
                    case "Bat":
                    case "Stone Golem":
                        mineLevelReq = 30;
                        startFloor = 31;
                        endFloor = 39;
                        locationKey = "monsterAreaMinesRange";
                        break;
                    case "Dust Spirit":
                    case "Frost Bat":
                    case "Frost Jelly":
                        mineLevelReq = 40;
                        startFloor = 41;
                        endFloor = 79;
                        locationKey = "monsterAreaMinesRange";
                        break;
                    case "Ghost":
                    case "Putrid Ghost":
                        mineLevelReq = 50;
                        startFloor = 51;
                        endFloor = 79;
                        locationKey = "monsterAreaMinesRange";
                        break;
                    case "Skeleton":
                        mineLevelReq = 70;
                        startFloor = 71;
                        endFloor = 79;
                        locationKey = "monsterAreaMinesRange";
                        break;
                    case "Lava Bat":
                    case "Lava Crab":
                    case "Metal Head":
                    case "Sludge": // Red
                    case "Shadow Brute":
                    case "Shadow Shaman":
                    case "Shadow Sniper":
                    case "Squid Kid":
                        mineLevelReq = 80;
                        startFloor = 81;
                        endFloor = 119;
                        locationKey = "monsterAreaMinesRange";
                        break;
                    case "Mummy":
                    case "Serpent":
                    case "Royal Serpent":
                        skullReq = true;
                        locationKey = "monsterAreaSkull";
                        break;
                    case "Iridium Crab":
                        skullReq = true;
                        startFloor = 26;
                        locationKey = "monsterAreaSkullFloorPlus";
                        break;
                    case "Iridium Bat":
                        skullReq = true;
                        startFloor = 50;
                        locationKey = "monsterAreaSkullFloorPlus";
                        break;
                    case "Pepper Rex":
                        skullReq = true;
                        locationKey = "monsterAreaSkullPrehistoric";
                        break;
                    case "Carbon Ghost":
                        skullReq = true;
                        locationKey = "monsterAreaSkullMummy";
                        break;
                    case "Lava Lurk":
                    case "Hot Head":
                    case "Spider":
                    case "Magma Duggy":
                    case "Magma Sprite":
                    case "Magma Sparker":
                    case "Dwarvish Sentry":
                    case "False Magma Cap":
                        volcanoReq = true;
                        locationKey = "monsterAreaVolcano";
                        break;
                }
            }
            
            // Convert "Mines and Skull Cavern" to just "Mines" if you shouldn't know about the latter.
            if (locationKey.Equals("monsterAreaMinesAndSkull") && !isSkullCavernKnown())
                locationKey = "monsterAreaMinesAll";
            
            if (!Config.ShowSpoilers)
            {
                if ((skullReq && !isSkullCavernKnown()) // Skull Cavern only, but not yet unlocked
                 || (volcanoReq && !isVolcanoKnown()) // Volcano, but not yet unlocked
                 || (mineLevelReq > MineShaft.lowestLevelReached)) // Mines, but haven't gotten deep enough
                    monsterStr = str.Get("monsterUnknown"); // Conceal monster name
                
                if ((skullReq && !isSkullCavernKnown()) // Skull Cavern only, but not yet unlocked
                 || (volcanoReq && !isVolcanoKnown())) // Volcano, but not yet unlocked
                    locationKey = "monsterAreaUnknown"; // Conceal area name
            }
            
            if (locationKey != "") // Always append location, even if its name is hidden
                monsterStr += parenthesize(str.Get(locationKey, new { start = startFloor != -1? startFloor.ToString() : "",
                                                                      end = endFloor != -1? endFloor.ToString() : "" }));
            
            return str.Get("droppedByMonster", new { monster = monsterStr });
        }
        
        /// <summary>Suggestion for an item that grows on a fruit tree during a particular season.</summary>
        /// <param name="treeKey">String key for tree name (treeX).</param>
        /// <param name="seasonKey">String key for season name (spring/summer/fall/winter).</param>
        /// <param name="shopKey">String key for the shop that sells the saplings (shopX). Defaults to Pierre's.</param>
        /// <param name="startingYear">What year the seedlings become available.</param>
        private static string strFruitTreeDuringSeason(string treeKey, string seasonKey,
                                                       string shopKey = "shopPierre", int startingYear = 0)
        {
            string yearStr = getStartingYearString(startingYear);
            string seedlingStr = parenthesize(str.Get("treeDescSeedling", new { shop = str.Get(shopKey), startingYear = yearStr }));
            
            return str.Get("treeDuringSeason", new { tree = str.Get(treeKey) + seedlingStr, season = str.Get(seasonKey) });
        }
        
        /// <summary>Suggestion for an item obtainable by tapping a particular tree. Includes seed used to grow said tree.</summary>
        /// <param name="treeKey">String key for tree name (treeX).</param>
        private static string strTapTree(string treeKey)
        {
            string treeSeed = "";
            switch (treeKey)
            {
                case "treeMaple": treeSeed = getItemName(ItemID.IT_MapleSeed); break;
                case "treeOak": treeSeed = getItemName(ItemID.IT_Acorn); break;
                case "treePine": treeSeed = getItemName(ItemID.IT_PineCone); break;
            }
            
            string tapperUnlockTip = "";
            if (Game1.player.ForagingLevel < 3)
                tapperUnlockTip = levelRequirementString("foraging", 3);
            
            string tapperStr = getBigCraftableName(ItemID.BC_Tapper) + tapperUnlockTip;
            string treeStr = treeSeed != ""? str.Get(treeKey) + parenthesize(str.Get("treeDescNonFruit", new { treeSeed = treeSeed }))
                                           : str.Get(treeKey);
            return str.Get("tapTree", new { tapper = tapperStr, tree = treeStr });
        }
        
        /// <summary>Suggestion for craftable item. Includes either how to get recipe, or materials.</summary>
        /// <param name="recipeName">Internal string name of the recipe.</param>
        private static string strCraftRecipe(string recipeName)
        {
            string[] recipeData = DataLoader.CraftingRecipes(Game1.content)[recipeName].Split('/');
            string ingredientDefinition = recipeData[0];
            
            string recipeDesc = "";
            
            if (Game1.player.knowsRecipe(recipeName)) // Have recipe, so list ingredients
                recipeDesc = getRecipeIngredientsString(ingredientDefinition);
            else // Describe where to get recipe
            {
                string recipeSources = getCraftingRecipeSources(recipeName);
                if (recipeSources != "")
                    recipeDesc = parenthesize(str.Get("recipeFrom", new { sources = recipeSources }));
            }
            
            // For recipes where the recipe name is not the created item, include the name of the recipe.
            bool includeRecipeName = false;
            switch (recipeName)
            {
                case "Transmute (Fe)":
                case "Transmute (Au)":
                    includeRecipeName = true;
                    break;
            }
            
            return (includeRecipeName? str.Get("craftRecipeByName", new { recipe = getCraftingRecipeName(recipeName) })
                                     : str.Get("craftRecipe"))
                 + recipeDesc;
        }
        
        /// <summary>Suggestion for cookable item. Includes either how to get recipe, or ingredients.</summary>
        /// <param name="recipeName">Internal string name of the recipe.</param>
        private static string strCookRecipe(string recipeName)
        {
            string[] recipeData = DataLoader.CookingRecipes(Game1.content)[recipeName].Split('/');
            string ingredientDefinition = recipeData[0];
            
            string recipeDesc = "";
            
            if (ModEntry.debugUnlockCooking && Game1.player.HouseUpgradeLevel <= 0)
                Game1.player.HouseUpgradeLevel = 1;
            
            if (Game1.player.HouseUpgradeLevel <= 0) // Don't have house upgrade to cook anything yet
                recipeDesc = parenthesize(str.Get("recipeTipHouseUpgrade"));
            else if (Game1.player.knowsRecipe(recipeName)) // Have recipe, so list ingredients
                recipeDesc = getRecipeIngredientsString(ingredientDefinition);
            else // Describe where to get recipe
            {
                string recipeSources = getCookingRecipeSources(recipeName);
                if (recipeSources != "")
                    recipeDesc = parenthesize(str.Get("recipeFrom", new { sources = recipeSources }));
            }
            
            return str.Get("cookRecipe") + recipeDesc;
        }
        
        /// <summary>Suggestion for items randomly obtainable from geodes.</summary>
        /// <param name="geodeIDs">List of item IDs for geode types.</param>
        private static string strOpenGeode(params string[] geodeIDs)
        {
            return str.Get("openGeode", new { geode = multiItem(geodeIDs) });
        }
        
        /// <summary>Suggestion for panning, described relative to spoiler policy and glittering boulder being removed.</summary>
        private static string strPanning()
        {
            return Config.ShowSpoilers || Game1.MasterPlayer.mailReceived.Contains("ccFishTank")? str.Get("panning")
                                                                                                : str.Get("unknownSource");
        }
        
        /// <summary>Suggestion to buy from a shop.</summary>
        /// <param name="shopKey">String key for shop (shopX).</param>
        /// <param name="shopLiteral">Override for preformatted text (usually used with multiKey).</param>
        private static string strBuyFrom(string shopKey = "", string shopLiteral = "")
        {
            return str.Get("buyFrom", new { shop = shopLiteral != ""? shopLiteral : str.Get(shopKey) });
        }
        
        /// <summary>Suggestion to buy from Dwarf, or an unknown shop if not yet known.</summary>
        private static string strBuyFromDwarf()
        {
            return str.Get("buyFrom", new { shop = str.Get(isDwarfKnown()? "shopDwarf" : "shopUnknown") });
        }
        
        /// <summary>Suggestion to buy from Krobus, or an unknown shop if not yet known.</summary>
        private static string strBuyFromKrobus()
        {
            return str.Get("buyFrom", new { shop = str.Get(isSewerKnown()? "shopKrobus" : "shopUnknown")
                                                 + (!Game1.player.hasRustyKey? parenthesize(str.Get("sewerRequirement")) : "") });
        }
        
        /// <summary>Suggestion to buy from Krobus on a certain day, or an unknown shop if not yet known.</summary>
        private static string strBuyFromKrobusWeekday(string dayKey)
        {
            return str.Get("buyFrom", new { shop = (isSewerKnown()? str.Get("shopKrobusWeekday", new { day = str.Get(dayKey) })
                                                                  : str.Get("shopUnknown"))
                                                 + (!Game1.player.hasRustyKey? parenthesize(str.Get("sewerRequirement")) : "") });
        }
        
        /// <summary>Suggestion to buy from Oasis on a certain day, or an unknown shop if not yet known.</summary>
        /// <param name="dayKey">Key for the day of the week (monday, tuesday, wednesday, thursday, friday, saturday, sunday)</param>
        private static string strBuyFromOasisWeekday(string dayKey)
        {
            return str.Get("buyFrom", new { shop = isDesertKnown()? str.Get("shopOasisWeekday", new { day = str.Get(dayKey) })
                                                                  : str.Get("shopUnknown") });
        }
        
        /// <summary>Suggestion to chop wood with axe.</summary>
        private static string strChopWood()
        {
            // Loads "Axe" tool name from strings
            return str.Get("chopWood", new { axe = TokenParser.ParseText(DataLoader.Tools(Game1.content)["Axe"].DisplayName) });
        }
        
        /// <summary>Suggestion to chop stumps and logs with upgraded axe for hardwood.</summary>
        private static string strChopHardwood()
        {
            string axe = TokenParser.ParseText(DataLoader.Tools(Game1.content)["Axe"].DisplayName); // Axe tool name
            string copperTool = Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14299"); // "Copper {0}" string
            string steelTool = Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14300"); // "Steel {0}" string
            return str.Get("chopHardwood", new { copperAxe = string.Format(copperTool, axe),
                                                 steelAxe = string.Format(steelTool, axe) });
        }
        
        /// <summary>Suggestion to chop trees with axe.</summary>
        private static string strChopTrees()
        {
            // Loads "Axe" tool name from strings
            return str.Get("chopTrees", new { axe = TokenParser.ParseText(DataLoader.Tools(Game1.content)["Axe"].DisplayName) });
        }
        
        /// <summary>Suggestion of fruit bat cave depending on progress, choice, and spoiler policy.
        /// Returns blank string if not applicable, so non-blank results are prefixed with a newline.</summary>
        private static string possibleSourceFruitBatCave()
        {
            switch (Game1.MasterPlayer.caveChoice.Value)
            {
                case 0: // Not yet chosen, so don't spoil, or specify which type if spoilers are okay
                    if (Config.ShowSpoilers)
                        return "\n" + str.Get("caveFruitBats");
                    else
                        return "\n" + str.Get("unknownSource");
                case 1: // Chose fruit bat cave, so simply say it can appear in your cave
                    return "\n" + str.Get("caveYours");
                case 2: // Did not choose fruit bat cave, so don't include it
                    return "";
            }
            return "";
        }
        
        /// <summary>Suggestion of mushroom cave depending on progress, choice, and spoiler policy.
        /// Returns blank string if not applicable, so non-blank results are prefixed with a newline.</summary>
        private static string possibleSourceMushroomCave()
        {
            switch (Game1.MasterPlayer.caveChoice.Value)
            {
                case 0: // Not yet chosen, so don't spoil, or specify which type if spoilers are okay
                    if (Config.ShowSpoilers)
                        return "\n" + str.Get("caveMushrooms");
                    else
                        return "\n" + str.Get("unknownSource");
                case 1: // Did not choose mushroom cave, so don't include it
                    return "";
                case 2: // Chose mushroom cave, so simply say it can appear in your cave
                    return "\n" + str.Get("caveYours");
            }
            return "";
        }
        
        /// <summary>Suggestion of foraging on farm in a particular season, if you chose a certain farm type.
        /// Returns blank string if not applicable, so non-blank results are prefixed with a newline.</summary>
        /// <param name="farmType">String for farm type (standard, riverland, forest, hilltop, wilderness, fourCorners).</param>
        /// <param name="seasonKey">String key for season name (spring/summer/fall/winter).</param>
        private static string possibleSourceSpecialFarmSeasonal(string farmType, string seasonKey)
        {
            if (haveSpecialFarmType(farmType))
                return "\n" + strSeasonalLocationalForage(seasonKey, "locationFarm");
            else
                return "";
        }
        
        /***********************
         ** Name Data Getters **
         ***********************/
        
        /// <summary>Returns item name for the current language (or internal name), or category for negative category IDs.</summary>
        /// <param name="id">The item ID or negative category ID.</param>
        /// <param name="internalName">Whether to return the internal name instead of the display name.</param>
        private static string getItemName(string id, bool internalName = false)
        {
            int intID;
            if (int.TryParse(id, out intID))
            {
                if (intID < 0) // Category
                {
                    switch (intID)
                    {
                        case StardewValley.Object.GemCategory: return str.Get("itemCategoryGem");
                        case StardewValley.Object.FishCategory: return str.Get("itemCategoryFish");
                        case StardewValley.Object.EggCategory: return str.Get("itemCategoryEgg");
                        case StardewValley.Object.MilkCategory: return str.Get("itemCategoryMilk");
                        case StardewValley.Object.mineralsCategory: return str.Get("itemCategoryMineral");
                        case StardewValley.Object.junkCategory: return str.Get("itemCategoryJunk");
                        case StardewValley.Object.SeedsCategory: return str.Get("itemCategorySeed");
                        case StardewValley.Object.VegetableCategory: return str.Get("itemCategoryVegetable");
                        case StardewValley.Object.FruitsCategory: return str.Get("itemCategoryFruit");
                        case StardewValley.Object.flowersCategory: return str.Get("itemCategoryFlower");
                        case -777: return str.Get("itemCategoryWildSeed"); // Used in Tea Sapling crafting recipe
                    }
                    return "[Category " + id + "]";
                }
            }
            
            if (Game1.objectData.ContainsKey(id) && Game1.objectData[id] != null)
            {
                if (internalName)
                    return Game1.objectData[id].Name;
                else
                    return TokenParser.ParseText(Game1.objectData[id].DisplayName);
            }
            
            return internalName? "" : "[Item " + id + "]";
        }
        
        /// <summary>Returns "big craftable" item name for the current language (or internal name).</summary>
        /// <param name="id">The item's Big Craftable ID.</param>
        /// <param name="internalName">Whether to return the internal name instead of the display name.</param>
        private static string getBigCraftableName(string id, bool internalName = false)
        {
            if (Game1.bigCraftableData.ContainsKey(id) && Game1.bigCraftableData[id] != null)
            {
                if (internalName)
                    return Game1.bigCraftableData[id].Name;
                else
                    return TokenParser.ParseText(Game1.bigCraftableData[id].DisplayName);
            }
            
            return internalName? "" : "[BigCraftable " + id + "]";
        }
        
        /// <summary>Returns name of person for the current language.</summary>
        /// <param name="name">The internal name of the person.</param>
        public static string getPersonName(string name)
        {
            return TokenParser.ParseText(DataLoader.Characters(Game1.content)[name].DisplayName);
        }
        
        /// <summary>Returns name of monster for the current language.</summary>
        /// <param name="name">The internal name of the monster.</param>
        private static string getMonsterName(string name)
        {
            return DataLoader.Monsters(Game1.content)[name].Split('/')[14];
        }
        
        /// <summary>Returns name of crafting recipe for the current language.</summary>
        /// <param name="name">The internal name of the recipe.</param>
        private static string getCraftingRecipeName(string name)
        {
            string[] recipeData = DataLoader.CraftingRecipes(Game1.content)[name].Split('/');
            if (recipeData.Length < 3) // Not long enough to contain item ID
                return "";
            
            bool bigCraftable = false;
            if (recipeData.Length > 3) // Long enough to contain "is big craftable" bool
                bool.TryParse(recipeData[3], out bigCraftable);
            
            if (!bigCraftable)
                return TokenParser.ParseText(Game1.objectData[recipeData[2]].DisplayName);
            else
                return TokenParser.ParseText(Game1.bigCraftableData[recipeData[2]].DisplayName);
        }
        
        /// <summary>Returns display name of roe for a specific fish.</summary>
        /// <param name="id">The fish's item ID.</param>
        private static string getFishRoeName(string id)
        {
            string fish = getItemName(id); // Fish name
            string fishRoe = Game1.content.LoadString("Strings\\StringsFromCSFiles:Roe_DisplayName"); // "{0} Roe" string
            return string.Format(fishRoe, fish);
        }
        
        /// <summary>Looks for item by name and returns success.</summary>
        /// <param name="name">The internal name of the item.</param>
        /// <param name="itemID">Result item ID. -1 if not found.</param>
        private static bool findItemIDByName(string name, out string itemID)
        {
            foreach (string id in Game1.objectData.Keys)
            {
                if (Game1.objectData[id] != null)
                {
                    if (Game1.objectData[id].Name.Equals(name))
                    {
                        itemID = id;
                        return true;
                    }
                }
            }
            
            itemID = "";
            return false;
        }
        
        /// <summary>Looks for item by name and returns success.</summary>
        /// <param name="name">The internal name of the item.</param>
        /// <param name="bigCraftableID">Result Big Craftable ID. -1 if not found.</param>
        private static bool findBigCraftableIDByName(string name, out string bigCraftableID)
        {
            foreach (string id in Game1.bigCraftableData.Keys)
            {
                if (Game1.bigCraftableData[id] != null)
                {
                    if (Game1.bigCraftableData[id].Name.Equals(name))
                    {
                        bigCraftableID = id;
                        return true;
                    }
                }
            }
            
            bigCraftableID = "";
            return false;
        }
        
        /**********************************
         ** Miscellaneous String Helpers **
         **********************************/
        
        /// <summary>Returns a list of the ingredients for a recipe.</summary>
        /// <param name="ingredients">The string definition of ingredient IDs and quantities from the data.</param>
        private static string getRecipeIngredientsString(string ingredients)
        {
            string list = "";
            string[] data = ingredients.Split(' ');
            
            for (int i = 0; i < data.Length; i += 2)
            {
                string itemName = getItemName(data[i]);
                int quantity = int.Parse(data[i + 1]);
                if (list != "")
                    list += str.Get("recipeIngredientSeparator");
                list += str.Get(quantity == 1? "recipeIngredientSingle" : "recipeIngredientMultiple",
                                new { ingredient = itemName, num = quantity });
            }
            
            return parenthesize(list);
        }
        
        /// <summary>Returns a list of sources for the given crafting recipe.</summary>
        /// <param name="recipeName">The internal name of the recipe.</param>
        private static string getCraftingRecipeSources(string recipeName)
        {
            return getRecipeSources(recipeName, false);
        }
        
        /// <summary>Returns a list of sources for the given cooking recipe.</summary>
        /// <param name="recipeName">The internal name of the recipe.</param>
        private static string getCookingRecipeSources(string recipeName)
        {
            return getRecipeSources(recipeName, true);
        }
        
        /// <summary>Returns a list of sources for the given recipe.</summary>
        /// <param name="recipeName">The internal name of the recipe.</param>
        /// <param name="isCooking">Whether the recipe is a cooking recipe rather than a crafting recipe.</param>
        private static string getRecipeSources(string recipeName, bool isCooking)
        {
            string recipeSources = "";
            
            // First, look for base requirement in recipe data definitions.
            string[] recipeData;
            string requirementDefinition = "";
            
            if (!isCooking)
            {
                recipeData = DataLoader.CraftingRecipes(Game1.content)[recipeName].Split('/');
                if (recipeData.Length > 4)
                    requirementDefinition = recipeData[4];
            }
            else
            {
                recipeData = DataLoader.CookingRecipes(Game1.content)[recipeName].Split('/');
                if (recipeData.Length > 3)
                    requirementDefinition = recipeData[3];
            }
            
            string[] data = requirementDefinition.Split(' ');
            string dataZero = data[0].ToLower();
            int value;
            
            switch (dataZero)
            {
                case "s": // Skill Level (s SkillName Lv#)
                case "farming": // Alternates (SkillName Lv#)
                case "mining":
                case "fishing":
                case "foraging":
                case "combat":
                    string skillName = dataZero.Equals("s")? data[1].ToLower() : dataZero;
                    if (int.TryParse(dataZero.Equals("s")? data[2] : data[1], out value))
                        if (value >= 1 && !skillName.Equals("luck")) // Ignore if inconsequential or Luck skill
                            recipeSources = str.Get(skillName + "LvRequirement", new { num = value });
                    break;
                case "l": // Total Skill Level (l Lv#), combined total divided by two
                    if (int.TryParse(data[1], out value))
                        if (value >= 1 && value <= 25) // Ignore if inconsequential/unreachable (max is 25: 5 skills * 10 levels / 2)
                            recipeSources = str.Get("totalSkillLvRequirement", new { num = value * 2 });
                            // Combined total divided by two, so double it here to make it more understandable
                    break;
                case "f": // Friendship (f PersonName Heart#)
                    if (int.TryParse(data[2], out value))
                        recipeSources = str.Get("recipeSourceFriendship", new { person = getPersonName(data[1]), hearts = value });
                    break;
            }
            
            // Manual mail unlocks (often with multiple factors) for mod-added recipes.
            switch (recipeName)
            {
                case "Butter Churn":
                    if (modRegistry.IsLoaded("ppja.artisanvalleyforMFM")) // Artisan Valley mail
                        recipeSources = conditionalMailSource("Marnie", 4, "farming", 5);
                    break;
                
                case "Still":
                    if (modRegistry.IsLoaded("ppja.artisanvalleyforMFM")) // Artisan Valley mail
                        recipeSources = conditionalMailSource(skill: "farming", level: 10, year: 3, seasonKey: "fall");
                    break;
                
                case "Poached Pear":
                    if (modRegistry.IsLoaded("ppja.MoreRecipesforMFM")) // More Recipes mail
                        recipeSources = conditionalMailSource("Gus", 5, year: 2);
                    break;
                
                case "PB&J Sandwich":
                    if (modRegistry.IsLoaded("ppja.MoreRecipesforMFM")) // More Recipes mail
                        recipeSources = conditionalMailSource(skill: "farming", level: 4);
                    break;
                
                case "Seaweed Chips":
                    if (modRegistry.IsLoaded("ppja.EvenMoreRecipesforMFM")) // Even More Recipes mail
                        recipeSources = str.Get("recipeSourceFriendship", new { person = getPersonName("Willy"), hearts = 5 });
                    break;
                
                case "Chocolate Mouse Bread":
                    if (modRegistry.IsLoaded("ppja.EvenMoreRecipesforMFM")) // Even More Recipes mail
                        recipeSources = conditionalMailSource("Penny", 8, year: 3);
                    break;
            }
            
            // For cooking recipes, determine if obtainable from Queen of Sauce and add that as a source if so.
            if (isCooking)
            {
                for (int i = 1; i <= 32; i++)
                {
                    if (DataLoader.Tv_CookingChannel(Game1.content)[i.ToString()].Split('/')[0].Equals(recipeName))
                    {
                        string seasonKey = new string[] { "spring", "summer", "fall", "winter" }[((i - 1) / 4) % 4];
                        int day = (((i - 1) % 4) + 1) * 7;
                        int year = ((i - 1) / 16) + 1;
                        
                        int airDate = i * 7;
                        bool alreadyAired = Game1.stats.DaysPlayed > airDate;
                        if (alreadyAired) // If first airing already passed, give next scheduled airing 2n years later
                        {
                            do
                            {
                                airDate += 224;
                                year += 2;
                            } while (Game1.stats.DaysPlayed > airDate);
                        }
                        
                        string date;
                        if (Game1.stats.DaysPlayed == airDate) // Scheduled airing today
                            date = str.Get("today");
                        else // Scheduled airing at a future date
                        {
                            if (year != Game1.year) // Not this year, so year must be specified
                                date = str.Get("specificDate", new { season = str.Get(seasonKey), day, year });
                            else // Current year, so save space by not specifying
                                date = str.Get("generalDate", new { season = str.Get(seasonKey), day });
                        }
                        
                        recipeSources += (recipeSources != ""? str.Get("recipeSourceSeparator") : "")
                                       + str.Get("recipeSourceTV" + (alreadyAired? "AlreadyAired" : ""), new { date });
                        break;
                    }
                }
            }
            
            // Add other hardcoded sources.
            string separator = recipeSources != ""? str.Get("recipeSourceSeparator") : "";
            switch (recipeName)
            {
                // Crafting Recipes
                case "Wood Floor":
                case "Straw Floor":
                case "Stone Floor":
                case "Brick Floor":
                case "Stepping Stone Path":
                case "Crystal Path":
                case "Wooden Brazier":
                case "Stone Brazier":
                case "Gold Brazier":
                case "Carved Brazier":
                case "Stump Brazier":
                case "Barrel Brazier":
                case "Skull Brazier":
                case "Marble Brazier":
                case "Wood Lamp-post":
                case "Iron Lamp-post":
                    recipeSources += separator + str.Get("shopCarpenter");
                    break;
                
                case "Grass Starter":
                    recipeSources += separator + str.Get("shopPierre");
                    break;
                
                case "Weathered Floor":
                    if (isDwarfKnown())
                        recipeSources += separator + str.Get("shopDwarf");
                    break;
                
                case "Crystal Floor":
                case "Wicked Statue":
                    if (isSewerKnown())
                        recipeSources += separator + str.Get("shopKrobus");
                    break;
                
                case "Warp Totem: Desert":
                    if (isDesertKnown())
                        recipeSources += separator + str.Get("shopDesertTrader");
                    break;
                
                case "Tub o' Flowers":
                    recipeSources += separator + str.Get("shopFlowerDance");
                    break;
                
                case "Jack-O-Lantern":
                    recipeSources += separator + str.Get("shopSpiritsEve");
                    break;
                
                case "Furnace":
                    recipeSources += separator + str.Get("recipeSourceFirstCopper",
                                                         new { copperOre = getItemName(ItemID.IT_CopperOre) });
                    break;
                
                case "Garden Pot":
                    recipeSources += separator + str.Get("recipeSourceAfterGreenhouse");
                    break;
                
                case "Cask":
                    recipeSources += separator + str.Get("recipeSourceFinalFarmhouse");
                    break;
                
                case "Ancient Seeds":
                    recipeSources += separator + str.Get("recipeSourceAncientSeed",
                                                         new { seedArtifact = getItemName(ItemID.IT_AncientSeedArtifact) });
                    break;
                
                case "Deluxe Scarecrow":
                    recipeSources += separator + str.Get("recipeSourceRarecrows");
                    break;
                
                case "Tea Sapling":
                    recipeSources += separator + str.Get("recipeSourceHeartEvent",
                                                         new { person = getPersonName("Caroline"), hearts = 2 });
                    break;
                
                case "Wild Bait":
                    recipeSources += separator + str.Get("recipeSourceHeartEvent",
                                                         new { person = getPersonName("Linus"), hearts = 4 });
                    break;
                
                case "Mini-Jukebox":
                    recipeSources += separator + str.Get("recipeSourceHeartEvent",
                                                         new { person = getPersonName("Gus"), hearts = 5 });
                    break;
                
                case "Flute Block":
                case "Drum Block":
                    recipeSources += separator + str.Get("recipeSourceHeartEvent",
                                                         new { person = getPersonName("Robin"), hearts = 6 });
                    break;
                
                // Cooking Recipes
                case "Hashbrowns":
                case "Omelet":
                case "Pancakes":
                case "Bread":
                case "Tortilla":
                case "Pizza":
                case "Maki Roll":
                case "Triple Shot Espresso":
                    recipeSources += separator + str.Get("shopSaloon");
                    break;
                
                case "Cookies":
                    recipeSources += separator + str.Get("recipeSourceHeartEvent",
                                                         new { person = getPersonName("Evelyn"), hearts = 4 });
                    break;
                
                // PPJA Cooking Recipes
                case "Mushroom and Pepper Crepe":
                case "Popcorn":
                case "Strawberry Lemonade":
                    if (modRegistry.IsLoaded("ppja.evenmorerecipes")) // Even More Recipes assets
                        recipeSources += separator + str.Get("shopSaloon");
                    break;
                
                case "Breakfast Tea":
                    if (modRegistry.IsLoaded("paradigmnomad.morefood")) // More Recipes (Fruits and Veggies) assets
                        recipeSources += separator + str.Get("shopSaloon");
                    break;
                
                case "Halloumi Burger":
                    if (modRegistry.IsLoaded("ppja.evenmorerecipes")) // Even More Recipes assets
                        recipeSources += separator + str.Get("shopSaloon") + parenthesize(str.Get("randomlyAvailable"));
                    break;
                
                case "Berry Fusion Tea":
                    if (modRegistry.IsLoaded("paradigmnomad.morefood")) // More Recipes (Fruits and Veggies) assets
                        recipeSources += separator + str.Get("shopSaloon") + parenthesize(str.Get("spring"));
                    break;
                
                case "Southern Sweet Tea":
                    if (modRegistry.IsLoaded("paradigmnomad.morefood")) // More Recipes (Fruits and Veggies) assets
                        recipeSources += separator + str.Get("shopSaloon") + parenthesize(str.Get("summer"));
                    break;
                
                case "Cranberry Pomegranate Tea":
                    if (modRegistry.IsLoaded("paradigmnomad.morefood")) // More Recipes (Fruits and Veggies) assets
                        recipeSources += separator + str.Get("shopSaloon") + parenthesize(str.Get("fall"));
                    break;
                
                case "Avocado Eel Roll":
                    if (modRegistry.IsLoaded("paradigmnomad.morefood")) // More Recipes (Fruits and Veggies) assets
                        recipeSources += separator + str.Get("shopSaloon") + getStartingYearString(2);
                    break;
                
                case "Rich Tiramisu":
                    if (modRegistry.IsLoaded("ppja.evenmorerecipes")) // Even More Recipes assets
                        recipeSources += separator + str.Get("shopSaloon") + getStartingYearString(2);
                    break;
                
                case "Ice Cream Brownie":
                    if (modRegistry.IsLoaded("ppja.evenmorerecipes")) // Even More Recipes assets
                        recipeSources += separator + str.Get("shopSaloon") + parenthesize(str.Get("randomlyAvailable"))
                                       + getStartingYearString(2);
                    break;
                
                case "Wasabi Peas":
                case "Grilled Zucchini":
                    if (modRegistry.IsLoaded("ppja.evenmorerecipes")) // Even More Recipes assets
                        recipeSources += separator + str.Get("shopSaloon") + getStartingYearString(3);
                    break;
                
                case "Sun Tea":
                    if (modRegistry.IsLoaded("paradigmnomad.morefood")) // More Recipes (Fruits and Veggies) assets
                        if (isDesertKnown())
                            recipeSources += separator + str.Get("shopOasis");
                    break;
                
                case "Prismatic Popsicle":
                    if (modRegistry.IsLoaded("ppja.evenmorerecipes")) // Even More Recipes assets
                    {
                        if (isDesertKnown())
                        {
                            bool shippedBerry = Game1.player.basicShipped.ContainsKey(ItemID.IT_SweetGemBerry)
                                             && Game1.player.basicShipped[ItemID.IT_SweetGemBerry] >= 1;
                            
                            recipeSources += separator + str.Get("shopOasis")
                                           + (!shippedBerry? parenthesize(str.Get("mustHaveShipped",
                                                                                  new { item = getItemName(ItemID.IT_SweetGemBerry) }))
                                                           : "");
                        }
                    }
                    break;
                
                // SVE Cooking Recipes
                case "Big Bark Burger":
                case "Glazed Butterfish":
                    recipeSources += separator + str.Get("shopSaloon")
                                   + parenthesize(str.Get("recipeSourceFriendship",
                                                  new { person = getPersonName("Gus"), hearts = 5 }));
                    break;
                
                case "Void Salmon Sushi":
                case "Void Delight":
                    if (isSewerKnown())
                        recipeSources += separator + str.Get("shopKrobus")
                                       + parenthesize(str.Get("recipeSourceFriendship",
                                                              new { person = getPersonName("Krobus"), hearts = 10 }));
                    break;
            }
            
            if (recipeSources.Equals("")) // No sources were found (usually due to leaving out spoilers)
                return str.Get("unknownSource");
            
            return recipeSources;
        }
        
        /// <summary>Returns a string describing conditions for recieving a certain piece of mail.</summary>
        /// <param name="person">Person who you need sufficient friendship with.</param>
        /// <param name="hearts">Hearts required with person.</param>
        /// <param name="skill">Skill that needs at a certain level.</param>
        /// <param name="level">Level required in skill.</param>
        /// <param name="year">Year of the earliest time you'll be able to receive the mail. Omitted if not specified.</param>
        /// <param name="seasonKey">Season of the earliest time you'll be able to receive the mail. Omitted if not specified.</param>
        /// <param name="day">Day of the earliest time you'll be able to receive the mail. Omitted if not specified.</param>
        private static string conditionalMailSource(string person = "", int hearts = 0,
                                                    string skill = "", int level = 0,
                                                    int year = 0, string seasonKey = "", int day = 0)
        {
            string friendReq = "", skillReq = "", time = "";
            
            int daysPlayedReq = ((year != 0? year - 1 : 0) * 112) + (day != 0? day : 1);
            switch (seasonKey)
            {
                // "" (default) and "spring" need not add any days
                case "summer": daysPlayedReq += 28; break;
                case "fall": daysPlayedReq += 56; break;
                case "winter": daysPlayedReq += 84; break;
            }
            
            if (Game1.stats.DaysPlayed < daysPlayedReq) // No need to mention time if player has reached required day
            {
                if (seasonKey != "" && day != 0 && year != 0)
                    time = str.Get("specificDate", new { season = str.Get(seasonKey), day = day, year = year });
                else if (seasonKey != "" && day != 0)
                    time = str.Get("generalDate", new { season = str.Get(seasonKey), day = day });
                else if (seasonKey != "" && year != 0)
                    time = str.Get("yearSeason", new { season = str.Get(seasonKey), year = year });
                else if (year != 0)
                    time = str.Get("year", new { year = year });
            }
            
            if (person != "")
                friendReq = str.Get("mailConditionFriendship", new { person = getPersonName(person), hearts = hearts });
            
            if (skill != "")
                skillReq = str.Get(skill + "LvRequirement", new { num = level });
            
            // Return string combining the requirements depending on which are specified.
            if (time != "" && friendReq != "" && skillReq != "")
                return str.Get("conditionalMailAll", new { time, friendReq, skillReq });
            else if (time != "" && friendReq != "")
                return str.Get("conditionalMailTimeFriend", new { time, friendReq });
            else if (time != "" && skillReq != "")
                return str.Get("conditionalMailTimeSkill", new { time, skillReq });
            else if (friendReq != "" && skillReq != "")
                return str.Get("conditionalMailFriendSkill", new { friendReq, skillReq });
            else if (time != "")
                return time;
            else if (friendReq != "")
                return friendReq;
            else if (skillReq != "")
                return skillReq;
            else
                return "";
        }
        
        /// <summary>Returns startingYear hint with the given year, or a blank string if that year has already been reached.</summary>
        /// <param name="year">The starting year.</param>
        private static string getStartingYearString(int year)
        {
            return Game1.year < year? str.Get("startingYear", new { num = year }) : "";
        }
        
        /// <summary>Returns shops where you can buy standard seeds (removing JojaMart after it closes).</summary>
        private static string getSeedShopsString()
        {
            return multiKey("shopPierre", isJojaOpen()? "shopJoja" : "");
        }
        
        /// <summary>Returns time range given start and end hours.</summary>
        /// <param name="start">Start of time range (i.e. "6am", "12pm").</param>
        /// <param name="end">End of time range (i.e. "6am", "12pm").</param>
        private static string getTimeRange(string start, string end)
        {
            if (!using24HourTime())
            {
                string startTime = start.Contains("am")? str.Get("timeAM", new { hour = start.Replace("am", "") })
                                                       : str.Get("timePM", new { hour = start.Replace("pm", "") });
                string endTime = end.Contains("am")? str.Get("timeAM", new { hour = end.Replace("am", "") })
                                                   : str.Get("timePM", new { hour = end.Replace("pm", "") });
                return str.Get("timeRange", new { start = startTime, end = endTime });
            }
            else
            {
                int startHour = start.Contains("am")? int.Parse(start.Replace("am", ""))
                                                    : int.Parse(start.Replace("pm", "")) + 12;
                int endHour = end.Contains("am")? int.Parse(end.Replace("am", ""))
                                                : int.Parse(end.Replace("pm", "")) + 12;
                
                // Adjust for 12 AM/PM
                if (startHour == 12) // 12 AM
                    startHour = 24;
                else if (startHour == 24) // 12 PM
                    startHour = 12;
                
                if (endHour == 12) // 12 AM
                    endHour = 24;
                else if (endHour == 24) // 12 PM
                    endHour = 12;
                
                // When range goes past midnight (wrapping around to be less than start hour), end hour should go past 24
                if (endHour < startHour)
                    endHour += 24;
                
                string startTime = str.Get("time24Hour", new { hour = startHour });
                string endTime = str.Get("time24Hour", new { hour = endHour });
                return str.Get("timeRange", new { start = startTime, end = endTime });
            }
        }
        
        /// <summary>Returns a skill level requirement string in parentheses.</summary>
        /// <param name="skillName">The nane of the skill.</param>
        /// <param name="level">The level required.</param>
        private static string levelRequirementString(string skillName, int level)
        {
            return parenthesize(str.Get(skillName + "LvRequirement", new { num = level }));
        }
        
        /// <summary>Returns a string with parentheses (and preceding space) put around it.</summary>
        /// <param name="text">The contained text.</param>
        private static string parenthesize(string text)
        {
            return str.Get("parenthesized", new { text = text });
        }
        
        /// <summary>Returns combined string for multiple seasons.</summary>
        /// <param name="seasons">List of season keys.</param>
        private static string multiSeason(params string[] seasons)
        {
            if (seasons.Length >= 4)
                return str.Get("allSeasons");
            else
                return multiKey(seasons);
        }
        
        /// <summary>Returns combined string for multiple string keys.</summary>
        /// <param name="keys">List of keys.</param>
        private static string multiKey(params string[] keys)
        {
            string[] strings = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
                strings[i] = keys[i] != ""? str.Get(keys[i]) : "";
            return multiString(strings);
        }
        
        /// <summary>Returns combined string for multiple item names.</summary>
        /// <param name="items">List of item IDs.</param>
        private static string multiItem(params string[] items)
        {
            string[] strings = new string[items.Length];
            for (int i = 0; i < items.Length; i++)
                strings[i] = getItemName(items[i]);
            return multiString(strings);
        }
        
        /// <summary>Returns combined string for multiple strings.</summary>
        /// <param name="strings">List of strings.</param>
        private static string multiString(params string[] strings)
        {
            if (strings.Length == 1)
                return strings[0];
            else
            {
                string result = strings[0];
                for (int i = 1; i < strings.Length; i++)
                {
                    if (strings[i] != "")
                        result += (result != ""? str.Get("multipleThingSeparator") : "") + strings[i];
                }
                return result;
            }
        }
        
        /*************************
         ** Player Data Getters **
         *************************/
        
        /// <summary>Returns whether player chose a particular type of farm.</summary>
        /// <param name="farmType">String for farm type (standard, riverland, forest, hilltop, wilderness, fourCorners).</param>
        private static bool haveSpecialFarmType(string farmType)
        {
            string yourFarmType = "";
            switch (Game1.whichFarm)
            {
                case 0: yourFarmType = "standard"; break;
                case 1: yourFarmType = "riverland"; break;
                case 2: yourFarmType = "forest"; break;
                case 3: yourFarmType = "hilltop"; break;
                case 4: yourFarmType = "wilderness"; break;
                case 5: yourFarmType = "fourCorners"; break;
            }
            return yourFarmType.Equals(farmType);
        }
        
        /// <summary>Returns a number representing the highest-level coop on the farm.</summary>
        /// <returns>-1: No coop built; 0: Coop; 1: Big Coop; 2: Deluxe Coop</returns>
        private static int getCoopLevel()
        {
            return Game1.getFarm().isBuildingConstructed("Deluxe Coop")? 2
                 : Game1.getFarm().isBuildingConstructed("Big Coop")? 1
                 : Game1.getFarm().isBuildingConstructed("Coop")? 0
                                                                : -1;
        }
        
        /// <summary>Returns a number representing the highest-level barn on the farm.</summary>
        /// <returns>-1: No barn built; 0: Barn; 1: Big Barn; 2: Deluxe Barn</returns>
        private static int getBarnLevel()
        {
            return Game1.getFarm().isBuildingConstructed("Deluxe Barn")? 2
                 : Game1.getFarm().isBuildingConstructed("Big Barn")? 1
                 : Game1.getFarm().isBuildingConstructed("Barn")? 0
                                                                : -1;
        }
        
        /// <summary>Returns whether the bridge on the beach has been fixed, allowing access to the east side.</summary>
        private static bool fixedBridgeToEastBeach()
        {
            Beach beach = Game1.getLocationFromName("Beach") as Beach;
            if (beach != null)
                return beach.bridgeFixed.Value;
            return false;
        }
        
        /// <summary>Returns whether JojaMart is still open.</summary>
        private static bool isJojaOpen()
        {
            return Game1.isLocationAccessible("JojaMart");
        }
        
        /// <summary>Returns whether it's okay to mention Krobus/sewer, either due to spoiler policy or having Rusty Key.</summary>
        private static bool isSewerKnown()
        {
            return Config.ShowSpoilers || Game1.player.hasRustyKey;
        }
        
        /// <summary>Returns whether it's okay to mention dwarf, either due to spoiler policy or knowing dwarven language.</summary>
        private static bool isDwarfKnown()
        {
            return Config.ShowSpoilers || Game1.player.canUnderstandDwarves;
        }
        
        /// <summary>Returns whether it's okay to mention the desert, either due to spoiler policy or having unlocked it.</summary>
        private static bool isDesertKnown()
        {
            return Config.ShowSpoilers || Game1.MasterPlayer.mailReceived.Contains("ccVault");
        }
        
        /// <summary>Returns whether it's okay to mention the Skull Cavern, either due to spoiler policy or having unlocked it.</summary>
        private static bool isSkullCavernKnown()
        {
            return Config.ShowSpoilers || Game1.MasterPlayer.hasUnlockedSkullDoor;
        }
        
        /// <summary>Returns whether it's okay to mention the Witch's Swamp, either due to spoiler policy or having unlocked it.</summary>
        private static bool isWitchSwampKnown()
        {
            return Config.ShowSpoilers || Game1.MasterPlayer.hasOrWillReceiveMail("witchStatueGone");
        }
        
        /// <summary>Returns whether it's okay to mention Ginger Island, either due to spoiler policy or having unlocked it.</summary>
        private static bool isIslandKnown()
        {
            return Config.ShowSpoilers || Game1.MasterPlayer.hasOrWillReceiveMail("seenBoatJourney");
        }
        
        /// <summary>Returns whether it's okay to mention the volcano, either due to spoiler policy or having unlocked it.</summary>
        private static bool isVolcanoKnown()
        {
            return Config.ShowSpoilers || Game1.MasterPlayer.hasOrWillReceiveMail("islandNorthCaveOpened");
        }
        
        /// <summary>Returns whether it's okay to mention the Movie Theather, either due to spoiler policy or having unlocked it.</summary>
        private static bool isTheatherKnown()
        {
            return Config.ShowSpoilers || Game1.MasterPlayer.mailReceived.Contains("ccMovieTheater")
                                       || Game1.MasterPlayer.mailReceived.Contains("ccMovieTheaterJoja");
        }
        
        /// <summary>Returns whether the Mystery Boxes have been dropped.</summary>
        private static bool areMysteryBoxesUnlocked()
        {
            return Game1.MasterPlayer.mailReceived.Contains("sawQiPlane");
        }
        
        /// <summary>Returns whether current language should use 24-hour time.</summary>
        private static bool using24HourTime()
        {
            return Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ja;
        }
    }
}
