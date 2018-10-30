using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using PyTK.Extensions;
using PyTK.Types;
using PyTK.CustomElementHandler;
using PyTK.CustomTV;
using PyTK;
using StardewValley.Network;
using System.Collections.Generic;
using System.Linq;


using Microsoft.Xna.Framework.Graphics;

namespace ServerCode
{
    //todo
    // add warning in smapi if you have wrong mods installed
    // set up wrong mods kick
    // use "Mymod.Myaddress" to make sure peopel stay up to date



    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        private int numPlayers = 0; //stores number of players
        private int gameClockTicks; //stores in game clock change
        private int halfsecondTicks = 0;
        private int cheatClockTicks = 0;
        private List<string> blockedModNames = new List<string>();
        bool cheater = false;
        bool messageSent = false;
        bool preMessageDeleteMessageSent = false;
        private string currentPass = "SCAL"; //current code password 4 characters only
        private string introMessage = "ServerCode7.2 Activated";
        // private bool shipTicker = false;
        // private int shipTicks = 0;


        public override void Entry(IModHelper helper)
        {
            // MultiplayerEvents.BeforeMainSync += MultiplayerEvents_BeforeMainSync; 
            //  SaveEvents.BeforeSave += this.Shipping_Menu; // Shipping Menu handler
            TimeEvents.TimeOfDayChanged += this.TimeEvents_TimeOfDayChanged; // Time of day change handler
            //GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;
            GameEvents.HalfSecondTick += this.GameEvents_HalfSecondTick;
            GameEvents.FourthUpdateTick += this.GameEvents_FourthUpdateTick;
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            SaveEvents.BeforeSave += this.Shipping_Menu; // Shipping Menu handler
            GraphicsEvents.OnPreRenderGuiEvent += PreRenderGuiEvent; // for fixing social menu
            

            bool consoleCommandsIsLoaded = this.Helper.ModRegistry.IsLoaded("SMAPI.ConsoleCommands");
            if (consoleCommandsIsLoaded == true)
            { blockedModNames.Add("Console Commands"); cheater = true; }
            bool cJBCheatsMenuIsLoaded = this.Helper.ModRegistry.IsLoaded("CJBok.CheatsMenu");
            if (cJBCheatsMenuIsLoaded == true)
            { blockedModNames.Add("CJB CHEATS"); cheater = true; }
            bool bcmpincMovementSpeedIsLoaded = this.Helper.ModRegistry.IsLoaded("bcmpinc.MovementSpeed");
            if (bcmpincMovementSpeedIsLoaded == true)
            { blockedModNames.Add ("Movement Speed"); cheater = true; }
            bool bcmpincHarvestWithScytheIsLoaded = this.Helper.ModRegistry.IsLoaded("bcmpinc.HarvestWithScythe");
            if (bcmpincHarvestWithScytheIsLoaded == true)
            { blockedModNames.Add("HarvestWithScythe"); cheater = true; }
            bool dewModsStardewValleyModsPhoneVillagersIsLoaded = this.Helper.ModRegistry.IsLoaded("DewMods.StardewValleyMods.PhoneVillagers");
            if (dewModsStardewValleyModsPhoneVillagersIsLoaded == true)
            { blockedModNames.Add("PhoneVillager"); cheater = true; }
            bool vylusPelicanPostalServiceIsLoaded = this.Helper.ModRegistry.IsLoaded("Vylus.PelicanPostalService");
            if (vylusPelicanPostalServiceIsLoaded == true)
            { blockedModNames.Add("PelicanPostalService"); cheater = true; }
            bool crazywigtoolpowerselectIsLoaded = this.Helper.ModRegistry.IsLoaded("crazywig.toolpowerselect");
            if (crazywigtoolpowerselectIsLoaded == true)
            { blockedModNames.Add("ToolPowerSelect"); cheater = true; }
            bool defenTheNationTimeMultiplierIsLoaded = this.Helper.ModRegistry.IsLoaded("DefenTheNation.TimeMultiplier");
            if (defenTheNationTimeMultiplierIsLoaded == true)
            { blockedModNames.Add("TimeMultiplier"); cheater = true; }
            bool scythHarvestIsLoaded = this.Helper.ModRegistry.IsLoaded("965169fd-e1ed-47d0-9f12-b104535fb4bc");
            if (scythHarvestIsLoaded == true)
            { blockedModNames.Add("ScythHarvest"); cheater = true; }
            bool omegasisAutoSpeedIsLoaded = this.Helper.ModRegistry.IsLoaded("Omegasis.AutoSpeed");
            if (omegasisAutoSpeedIsLoaded == true)
            { blockedModNames.Add("AutoSpeed"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool cantorsdustTimeSpeedIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.TimeSpeed");
            if (cantorsdustTimeSpeedIsLoaded == true)
            { blockedModNames.Add("TimeSpeed"); cheater = true; }
            bool cJBokItemSpawnerIsLoaded = this.Helper.ModRegistry.IsLoaded("CJBok.ItemSpawner");
            if (cJBokItemSpawnerIsLoaded == true)
            { blockedModNames.Add("CJB ItemSpawner"); cheater = true; }
            bool pathoschildChestsAnywhereIsLoaded = this.Helper.ModRegistry.IsLoaded("Pathoschild.ChestsAnywhere");
            if (pathoschildChestsAnywhereIsLoaded == true)
            { blockedModNames.Add("ChestsAnywhere"); cheater = true; }
            bool dewModsStardewValleyModsSkipFishingMinigameIsLoaded = this.Helper.ModRegistry.IsLoaded("DewMods.StardewValleyMods.SkipFishingMinigame");
            if (dewModsStardewValleyModsSkipFishingMinigameIsLoaded == true)
            { blockedModNames.Add("SkipFishingMinigame"); cheater = true; }
            bool shalankwaWarpToFriendsIsLoaded = this.Helper.ModRegistry.IsLoaded("Shalankwa.WarpToFriends");
            if (shalankwaWarpToFriendsIsLoaded == true)
            { blockedModNames.Add("WarpToFriends"); cheater = true; }
            bool punyoPassableObjectsIsLoaded = this.Helper.ModRegistry.IsLoaded("punyo.PassableObjects");
            if (punyoPassableObjectsIsLoaded == true)
            { blockedModNames.Add("PassableObjects"); cheater = true; }
            bool defenTheNationBackpackResizerdIsLoaded = this.Helper.ModRegistry.IsLoaded("DefenTheNation.BackpackResizer");
            if (defenTheNationBackpackResizerdIsLoaded == true)
            { blockedModNames.Add("BackpackResizer"); cheater = true; }
            bool dWhiteMindAFIsLoaded = this.Helper.ModRegistry.IsLoaded("WhiteMind.AF");
            if (dWhiteMindAFIsLoaded == true)
            { blockedModNames.Add("AutoFish"); cheater = true; }
            bool mucchanPrairieKingMadeEasyIsLoaded = this.Helper.ModRegistry.IsLoaded("Mucchan.PrairieKingMadeEasy");
            if (mucchanPrairieKingMadeEasyIsLoaded == true)
            { blockedModNames.Add("PrairieKingMadeEasy"); cheater = true; }
            bool jwdredAnimalSitterIsLoaded = this.Helper.ModRegistry.IsLoaded("jwdred.AnimalSitter");
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool cantorsdustAllProfessionsIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.AllProfessions");
            if (cantorsdustAllProfessionsIsLoaded == true)
            { blockedModNames.Add("AllProfessions"); cheater = true; }
            bool MoreArtifactSpotsIsLoaded = this.Helper.ModRegistry.IsLoaded("451");
            if (MoreArtifactSpotsIsLoaded == true)
            { blockedModNames.Add("More Artifact Spots"); cheater = true; }
            bool allCropsAllSeasons = this.Helper.ModRegistry.IsLoaded("cantorsdust.AllCropsAllSeasons");
            if (allCropsAllSeasons == true)
            { blockedModNames.Add("AllCropsAllSeasons"); cheater = true; }
            bool drynwynnFishingAutomatonIsLoaded = this.Helper.ModRegistry.IsLoaded("Drynwynn.FishingAutomaton");
            if (drynwynnFishingAutomatonIsLoaded == true)
            { blockedModNames.Add("FishingAutomaton"); cheater = true; }
            bool cantorsdustRecatchLegendaryFishIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.RecatchLegendaryFish");
            if (cantorsdustRecatchLegendaryFishIsLoaded == true)
            { blockedModNames.Add("RecatchLegendaryFish"); cheater = true; }
            bool kathrynHazukaFasterRunIsLoaded = this.Helper.ModRegistry.IsLoaded("KathrynHazuka.FasterRun");
            if (kathrynHazukaFasterRunIsLoaded == true)
            { blockedModNames.Add("FasterRun"); cheater = true; }
            bool bitwiseJonModsInstantBuildingsIsLoaded = this.Helper.ModRegistry.IsLoaded("BitwiseJonMods.InstantBuildings");
            if (bitwiseJonModsInstantBuildingsIsLoaded == true)
            { blockedModNames.Add("InstantBuildings"); cheater = true; }
            bool jotserAutoGrabberModIsLoaded = this.Helper.ModRegistry.IsLoaded("Jotser.AutoGrabberMod");
            if (jotserAutoGrabberModIsLoaded == true)
            { blockedModNames.Add("AutoGrabber"); cheater = true; }
            bool dcantorsdustInstantGrowTreesIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.InstantGrowTrees");
            if (dcantorsdustInstantGrowTreesIsLoaded == true)
            { blockedModNames.Add("InstantGrowTrees"); cheater = true; }
            bool jwdredPointAndPlantIsLoaded = this.Helper.ModRegistry.IsLoaded("jwdred.PointAndPlant");
            if (jwdredPointAndPlantIsLoaded == true)
            { blockedModNames.Add("PointAndPlant"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool magicIsLoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.Magic");
            if (magicIsLoaded == true)
            { blockedModNames.Add("Magic"); cheater = true; }
            bool fastTravelIsLoaded = this.Helper.ModRegistry.IsLoaded("DeathGameDev.FastTravel");
            if (fastTravelIsLoaded == true)
            { blockedModNames.Add("FastTravel"); cheater = true; }
            bool scytheHarvesting2IsLoaded = this.Helper.ModRegistry.IsLoaded("mmanlapat.ScytheHarvesting");
            if (scytheHarvesting2IsLoaded == true)
            { blockedModNames.Add("ScytheHarvesting2"); cheater = true; }
            bool SkullCavernElevatorIsLoaded = this.Helper.ModRegistry.IsLoaded("SkullCavernElevator");
            if (SkullCavernElevatorIsLoaded == true)
            { blockedModNames.Add("SkullCavernElevator"); cheater = true; }
            bool horseWhistleIsLoaded = this.Helper.ModRegistry.IsLoaded("icepuente.HorseWhistle");
            if (horseWhistleIsLoaded == true)
            { blockedModNames.Add("HorseWhistle"); cheater = true; }
            bool fasterHorseIsLoaded = this.Helper.ModRegistry.IsLoaded("kibbe.faster_horse");
            if (fasterHorseIsLoaded == true)
            { blockedModNames.Add("FasterHorse"); cheater = true; }
            bool cropTransplantIsLoaded = this.Helper.ModRegistry.IsLoaded("DIGUS.CropTransplantMod");
            if (cropTransplantIsLoaded == true)
            { blockedModNames.Add("CropTransplant"); cheater = true; }
            bool automateIsLoaded = this.Helper.ModRegistry.IsLoaded("Pathoschild.Automate");
            if (automateIsLoaded == true)
            { blockedModNames.Add("Automate"); cheater = true; }
            bool kisekaeIsLoaded = this.Helper.ModRegistry.IsLoaded("Kabigon.kisekae");
            if (kisekaeIsLoaded == true)
            { blockedModNames.Add("kisekae"); cheater = true; }
            bool bjsTimeSkipperIsLoaded = this.Helper.ModRegistry.IsLoaded("BunnyJumps.BJSTimeSkipper");
            if (bjsTimeSkipperIsLoaded == true)
            { blockedModNames.Add("BJSTimeSkipper"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool timeFreezeIsLoaded = this.Helper.ModRegistry.IsLoaded("Omegasis.TimeFreeze");
            if (timeFreezeIsLoaded == true)
            { blockedModNames.Add("TimeFreeze"); cheater = true; }
            bool ultimateGoldIsLoaded = this.Helper.ModRegistry.IsLoaded("Shadowfoxss.UltimateGoldStardew");
            if (ultimateGoldIsLoaded == true)
            { blockedModNames.Add("UltimateGold"); cheater = true; }
            bool moreMapWarpsIsLoaded = this.Helper.ModRegistry.IsLoaded("rc.maps");
            if (moreMapWarpsIsLoaded == true)
            { blockedModNames.Add("MmoreMapWarps"); cheater = true; }
            bool debugIsLoaded = this.Helper.ModRegistry.IsLoaded("Pathoschild.DebugMode");
            if (debugIsLoaded == true)
            { blockedModNames.Add("DebugMode"); cheater = true; }
            bool idleTimerIsLoaded = this.Helper.ModRegistry.IsLoaded("LordAndreios.IdleTimer");
            if (idleTimerIsLoaded == true)
            { blockedModNames.Add("IdleTimer"); cheater = true; }
            bool extremeFishingOverhaulIsLoaded = this.Helper.ModRegistry.IsLoaded("DevinLematty.ExtremeFishingOverhaul");
            if (extremeFishingOverhaulIsLoaded == true)
            { blockedModNames.Add("ExtremeFishingOverhaul"); cheater = true; }
            bool tehsFishingOverhaulIsLoaded = this.Helper.ModRegistry.IsLoaded("TehPers.FishingOverhaul");
            if (tehsFishingOverhaulIsLoaded == true)
            { blockedModNames.Add("TehsFishingOverhaul"); cheater = true; }
            bool selfServiceShopsIsLoaded = this.Helper.ModRegistry.IsLoaded("GuiNoya.SelfServiceShop");
            if (selfServiceShopsIsLoaded == true)
            { blockedModNames.Add("SelfServiceShops"); cheater = true; }
            bool dailyQuestAnywhereIsLoaded = this.Helper.ModRegistry.IsLoaded("Omegasis.DailyQuestAnywhere");
            if (dailyQuestAnywhereIsLoaded == true)
            { blockedModNames.Add("DailyQuestAnywhere"); cheater = true; }
            bool bjsNoClipIsLoaded = this.Helper.ModRegistry.IsLoaded("BunnyJumps.BJSNoClip");
            if (bjsNoClipIsLoaded == true)
            { blockedModNames.Add("BJSNoClip"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool longevityIsLoaded = this.Helper.ModRegistry.IsLoaded("UnlockedRecipes.pseudohub.de");
            if (longevityIsLoaded == true)
            { blockedModNames.Add("Longevity"); cheater = true; }
            bool pHDEUnlockedCookingRecipesIsLoaded = this.Helper.ModRegistry.IsLoaded("RTGOAT.Longevity");
            if (pHDEUnlockedCookingRecipesIsLoaded == true)
            { blockedModNames.Add("UnlockedCookingRecipes"); cheater = true; }
            bool questDelayIsLoaded = this.Helper.ModRegistry.IsLoaded("BadNetCode.QuestDelay");
            if (questDelayIsLoaded == true)
            { blockedModNames.Add("QuestDelay"); cheater = true; }
            bool moreRainIsLoaded = this.Helper.ModRegistry.IsLoaded("Omegasis.MoreRain");
            if (moreRainIsLoaded == true)
            { blockedModNames.Add("MoreRain"); cheater = true; }
            bool fishingAdjustIsLoaded = this.Helper.ModRegistry.IsLoaded("shuaiz.FishingAdjustMod");
            if (fishingAdjustIsLoaded == true)
            { blockedModNames.Add("FishingAdjust"); cheater = true; }
            bool oneClickShedReloaderIsLoaded = this.Helper.ModRegistry.IsLoaded("BitwiseJonMods.OneClickShedReloader");
            if (fishingAdjustIsLoaded == true)
            { blockedModNames.Add("FishingAdjust"); cheater = true; }
            bool parsnipsIsLoaded = this.Helper.ModRegistry.IsLoaded("SolomonsWorkshop.ParsnipsAbsolutelyEverywhere");
            if (parsnipsIsLoaded == true)
            { blockedModNames.Add("ParsnipsAbsolutelyEverywhere"); cheater = true; }
            bool godModeIsLoaded = this.Helper.ModRegistry.IsLoaded("treyh0.GodMode");
            if (godModeIsLoaded == true)
            { blockedModNames.Add("GodMode"); cheater = true; }
            bool moveThroughObjectIsLoaded = this.Helper.ModRegistry.IsLoaded("ylsama.MoveThoughObject");
            if (moveThroughObjectIsLoaded == true)
            { blockedModNames.Add("MoveThoughObject"); cheater = true; }
            bool EZLegendaryFishIsLoaded = this.Helper.ModRegistry.IsLoaded("misscoriel.EZLegendaryFish");
            if (EZLegendaryFishIsLoaded == true)
            { blockedModNames.Add("EZLegendaryFish"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool infinitejunimocartlivesIsLoaded = this.Helper.ModRegistry.IsLoaded("renny.infinitejunimocartlives");
            if (infinitejunimocartlivesIsLoaded == true)
            { blockedModNames.Add("infinitejunimocartlives"); cheater = true; }
            bool quickStartIsLoaded = this.Helper.ModRegistry.IsLoaded("WuestMan.QuickStart");
            if (quickStartIsLoaded == true)
            { blockedModNames.Add("QuickStart"); cheater = true; }
            bool renameIsLoaded = this.Helper.ModRegistry.IsLoaded("Remmie.Rename");
            if (renameIsLoaded == true)
            { blockedModNames.Add("Rename"); cheater = true; }
            bool everlastingBaitsIsLoaded = this.Helper.ModRegistry.IsLoaded("DIGUS.EverlastingBaitsAndUnbreakableTacklesMod");
            if (everlastingBaitsIsLoaded == true)
            { blockedModNames.Add("EverlastingBaitsAndUnbreakableTackles"); cheater = true; }
            bool realisticFishingIsLoaded = this.Helper.ModRegistry.IsLoaded("KevinConnors.RealisticFishing");
            if (realisticFishingIsLoaded == true)
            { blockedModNames.Add("RealisticFishing"); cheater = true; }
            bool adjustablePriceHikesIsLoaded = this.Helper.ModRegistry.IsLoaded("Pokeytax.AdjustablePriceHikes");
            if (adjustablePriceHikesIsLoaded == true)
            { blockedModNames.Add("AdjustablePriceHikes"); cheater = true; }
            bool moreSiloStorageIsLoaded = this.Helper.ModRegistry.IsLoaded("OrneryWalrus.MoreSiloStorage");
            if (moreSiloStorageIsLoaded == true)
            { blockedModNames.Add("MoreSiloStorage"); cheater = true; }
            bool multiToolModIsLoaded = this.Helper.ModRegistry.IsLoaded("miome.MultiToolMod");
            if (multiToolModIsLoaded == true)
            { blockedModNames.Add("MultiTool"); cheater = true; }
            bool foxyfficiencyIsLoaded = this.Helper.ModRegistry.IsLoaded("Fokson.Foxyfficiency");
            if (foxyfficiencyIsLoaded == true)
            { blockedModNames.Add("Foxyfficiency"); cheater = true; }
            bool noAddedFlyingMineMonstersIsLoaded = this.Helper.ModRegistry.IsLoaded("Drynwynn.NoAddedFlyingMineMonsters");
            if (noAddedFlyingMineMonstersIsLoaded == true)
            { blockedModNames.Add("No More Random Mine Flyers"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool longerLastingLuresIsLoaded = this.Helper.ModRegistry.IsLoaded("caraxian.LongerLastingLures");
            if (longerLastingLuresIsLoaded == true)
            { blockedModNames.Add("LongerLastingLures"); cheater = true; }
            bool bJSStopGrassIsLoaded = this.Helper.ModRegistry.IsLoaded("BunnyJumps.BJSStopGrass");
            if (bJSStopGrassIsLoaded == true)
            { blockedModNames.Add("BJSStopGrass"); cheater = true; }
            bool bigSiloIsLoaded = this.Helper.ModRegistry.IsLoaded("lperkins2.BigSilo");
            if (bigSiloIsLoaded == true)
            { blockedModNames.Add("BigSilos"); cheater = true; }
            bool plantableMushroomTreesIsLoaded = this.Helper.ModRegistry.IsLoaded("f4iTh.PMT");
            if (plantableMushroomTreesIsLoaded == true)
            { blockedModNames.Add("PlantableMushroomTrees"); cheater = true; }
            bool infiniteInventoryIsLoaded = this.Helper.ModRegistry.IsLoaded("DevinLematty.InfiniteInventory");
            if (infiniteInventoryIsLoaded == true)
            { blockedModNames.Add("InfiniteInventory"); cheater = true; }
            bool betterJunimosIsLoaded = this.Helper.ModRegistry.IsLoaded("hawkfalcon.BetterJunimos");
            if (betterJunimosIsLoaded == true)
            { blockedModNames.Add("BetterJunimos"); cheater = true; }
            bool tillableGroundIsLoaded = this.Helper.ModRegistry.IsLoaded("hawkfalcon.TillableGround");
            if (tillableGroundIsLoaded == true)
            { blockedModNames.Add("TillableGround"); cheater = true; }
            bool customQuestExpirationIsLoaded = this.Helper.ModRegistry.IsLoaded("hawkfalcon.CustomQuestExpiration");
            if (tillableGroundIsLoaded == true)
            { blockedModNames.Add("TillableGround"); cheater = true; }
            bool betterTransmutationIsLoaded = this.Helper.ModRegistry.IsLoaded("f4iTh.BetterTransmutation");
            if (betterTransmutationIsLoaded == true)
            { blockedModNames.Add("BetterTransmutation"); cheater = true; }
            bool moreMineLaddersIsLoaded = this.Helper.ModRegistry.IsLoaded("JadeTheavas.MoreMineLadders");
            if (moreMineLaddersIsLoaded == true)
            { blockedModNames.Add("MoreMineLadders"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool priceDropsIsLoaded = this.Helper.ModRegistry.IsLoaded("skuldomg.priceDrops");
            if (priceDropsIsLoaded == true)
            { blockedModNames.Add("PriceDrops"); cheater = true; }
            bool safelightningIsLoaded = this.Helper.ModRegistry.IsLoaded("cat.safelightning");
            if (safelightningIsLoaded == true)
            { blockedModNames.Add("Safelightnings"); cheater = true; }
            bool customizabledeathpenaltyIsLoaded = this.Helper.ModRegistry.IsLoaded("cat.customizabledeathpenalty");
            if (customizabledeathpenaltyIsLoaded == true)
            { blockedModNames.Add("CustomizableDeathPenalty"); cheater = true; }
            bool customwarplocationsIsLoaded = this.Helper.ModRegistry.IsLoaded("cat.customwarplocations");
            if (customwarplocationsIsLoaded == true)
            { blockedModNames.Add("CustomWarpLocations"); cheater = true; }
            bool nocrowsIsLoaded = this.Helper.ModRegistry.IsLoaded("cat.nocrows");
            if (nocrowsIsLoaded == true)
            { blockedModNames.Add("No Crows"); cheater = true; }
            bool autoWaterIsLoaded = this.Helper.ModRegistry.IsLoaded("Whisk.AutoWater");
            if (autoWaterIsLoaded == true)
            { blockedModNames.Add("AutoWater"); cheater = true; }
            bool equivalentExchangeIsLoaded = this.Helper.ModRegistry.IsLoaded("MercuriusXeno.EquivalentExchange");
            if (equivalentExchangeIsLoaded == true)
            { blockedModNames.Add("EquivalentExchange"); cheater = true; }
            bool seedCatalogueIsLoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.SeedCatalogue");
            if (seedCatalogueIsLoaded == true)
            { blockedModNames.Add("SeedCatalogue"); cheater = true; }
            bool wintergrassIsLoaded = this.Helper.ModRegistry.IsLoaded("cat.wintergrass");
            if (wintergrassIsLoaded == true)
            { blockedModNames.Add("WinterGrass"); cheater = true; }
            bool moodGuardIsLoaded = this.Helper.ModRegistry.IsLoaded("YonKuma.MoodGuard");
            if (moodGuardIsLoaded == true)
            { blockedModNames.Add("MoodGuard"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool ExtendedReachIsLoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.ExtendedReach");
            if (ExtendedReachIsLoaded == true)
            { blockedModNames.Add("ExtendedReach"); cheater = true; }
            bool seasonalitemsIsLoaded = this.Helper.ModRegistry.IsLoaded("midoriarmstrong.seasonalitems");
            if (seasonalitemsIsLoaded == true)
            { blockedModNames.Add("SeasonalItems"); cheater = true; }
            bool betterhayIsLoaded = this.Helper.ModRegistry.IsLoaded("cat.betterhay");
            if (betterhayIsLoaded == true)
            { blockedModNames.Add("BetterHay"); cheater = true; }
            bool CustomizableCartReduxIsLoaded = this.Helper.ModRegistry.IsLoaded("KoihimeNakamura.CCR");
            if (CustomizableCartReduxIsLoaded == true)
            { blockedModNames.Add("CustomizableCartRedux"); cheater = true; }
            bool MoveFasterIsLoaded = this.Helper.ModRegistry.IsLoaded("shuaiz.MoveFasterMod");
            if (MoveFasterIsLoaded == true)
            { blockedModNames.Add("MoveFaster"); cheater = true; }
            bool TreeTransplantIsLoaded = this.Helper.ModRegistry.IsLoaded("TreeTransplant");
            if (TreeTransplantIsLoaded == true)
            { blockedModNames.Add("TreeTransplant"); cheater = true; }
            bool RentedToolsIsLoaded = this.Helper.ModRegistry.IsLoaded("JarvieK.RentedTools");
            if (RentedToolsIsLoaded == true)
            { blockedModNames.Add("RentedTools"); cheater = true; }
            bool SelfServiceIsLoaded = this.Helper.ModRegistry.IsLoaded("JarvieK.SelfService");
            if (SelfServiceIsLoaded == true)
            { blockedModNames.Add("SelfService"); cheater = true; }
            bool BregsFishIsLoaded = this.Helper.ModRegistry.IsLoaded("Bregodon.BregsFish");
            if (BregsFishIsLoaded == true)
            { blockedModNames.Add("BregsFish"); cheater = true; }
            bool ExpandedFridgeIsLoaded = this.Helper.ModRegistry.IsLoaded("Uwazouri.ExpandedFridge");
            if (ExpandedFridgeIsLoaded == true)
            { blockedModNames.Add("ExpandedFridge"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool StartingMoneyIsLoaded = this.Helper.ModRegistry.IsLoaded("mmanlapat.StartingMoney");
            if (StartingMoneyIsLoaded == true)
            { blockedModNames.Add("StartingMoney"); cheater = true; }
            bool wHatsUpIsLoaded = this.Helper.ModRegistry.IsLoaded("wHatsUp");
            if (wHatsUpIsLoaded == true)
            { blockedModNames.Add("wHatsUp"); cheater = true; }
            bool ChefsClosetIsLoaded = this.Helper.ModRegistry.IsLoaded("Duder.ChefsCloset");
            if (ChefsClosetIsLoaded == true)
            { blockedModNames.Add("ChefsCloset"); cheater = true; }
            bool ImprovedQualityOfLifeIsLoaded = this.Helper.ModRegistry.IsLoaded("Demiacle.ImprovedQualityOfLife");
            if (ImprovedQualityOfLifeIsLoaded == true)
            { blockedModNames.Add("ImprovedQualityOfLife"); cheater = true; }
            bool AutoAnimalDoorsIsLoaded = this.Helper.ModRegistry.IsLoaded("AaronTaggart.AutoAnimalDoors");
            if (AutoAnimalDoorsIsLoaded == true)
            { blockedModNames.Add("AutoAnimalDoors"); cheater = true; }
            bool PartoftheCommunityIsLoaded = this.Helper.ModRegistry.IsLoaded("SB_PotC");
            if (PartoftheCommunityIsLoaded == true)
            { blockedModNames.Add("PartoftheCommunity"); cheater = true; }
            bool CasksAnywhereIsLoaded = this.Helper.ModRegistry.IsLoaded("CasksAnywhere");
            if (CasksAnywhereIsLoaded == true)
            { blockedModNames.Add("CasksAnywhere"); cheater = true; }
            bool RocsReseedIsLoaded = this.Helper.ModRegistry.IsLoaded("Roc.Reseed");
            if (RocsReseedIsLoaded == true)
            { blockedModNames.Add("RocsReseed"); cheater = true; }
            bool BasicSprinklerImprovedIsLoaded = this.Helper.ModRegistry.IsLoaded("lrsk_sdvm_bsi.0117171308");
            if (BasicSprinklerImprovedIsLoaded == true)
            { blockedModNames.Add("BasicSprinklerImproved"); cheater = true; }
            bool MiningWithExplosivesIsLoaded = this.Helper.ModRegistry.IsLoaded("MiningWithExplosives");
            if (MiningWithExplosivesIsLoaded == true)
            { blockedModNames.Add("MiningWithExplosives"); cheater = true; }
            bool AutoEatIsLoaded = this.Helper.ModRegistry.IsLoaded("Permamiss.AutoEat");
            if (AutoEatIsLoaded == true)
            { blockedModNames.Add("AutoEat"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool ReplanterIsLoaded = this.Helper.ModRegistry.IsLoaded("jwdred.Replanter");
            if (ReplanterIsLoaded == true)
            { blockedModNames.Add("Replanter"); cheater = true; }
            bool SprintAndDashReduxIsLoaded = this.Helper.ModRegistry.IsLoaded("littleraskol.SprintAndDashRedux");
            if (SprintAndDashReduxIsLoaded == true)
            { blockedModNames.Add("SprintAndDashRedux"); cheater = true; }
            bool LuckSkillIsLoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.LuckSkill");
            if (LuckSkillIsLoaded == true)
            { blockedModNames.Add("LuckSkill"); cheater = true; }
            bool BuildHealthIsLoaded = this.Helper.ModRegistry.IsLoaded("Omegasis.BuildHealth");
            if (BuildHealthIsLoaded == true)
            { blockedModNames.Add("BuildHealth"); cheater = true; }
            bool BuildEnduranceIsLoaded = this.Helper.ModRegistry.IsLoaded("Omegasis.BuildEndurance");
            if (BuildEnduranceIsLoaded == true)
            { blockedModNames.Add("BuildEndurance"); cheater = true; }
            bool AlmightyFarmingToolIsLoaded = this.Helper.ModRegistry.IsLoaded("439");
            if (AlmightyFarmingToolIsLoaded == true)
            { blockedModNames.Add("Almighty Farming Tool"); cheater = true; }
            bool DynamicMachinesIsLoaded = this.Helper.ModRegistry.IsLoaded("DynamicMachines");
            if (DynamicMachinesIsLoaded == true)
            { blockedModNames.Add("DynamicMachines"); cheater = true; }
            bool ConfigurableMachinesIsLoaded = this.Helper.ModRegistry.IsLoaded("21da6619-dc03-4660-9794-8e5b498f5b97");
            if (ConfigurableMachinesIsLoaded == true)
            { blockedModNames.Add("ConfigurableMachines"); cheater = true; }
            bool ATappersDreamIsLoaded = this.Helper.ModRegistry.IsLoaded("ddde5195-8f85-4061-90cc-0d4fd5459358");
            if (ATappersDreamIsLoaded == true)
            { blockedModNames.Add("ATappersDream"); cheater = true; }
            bool NoSoilDecayReduxIsLoaded = this.Helper.ModRegistry.IsLoaded("Platonymous.NoSoilDecayRedux");
            if (NoSoilDecayReduxIsLoaded == true)
            { blockedModNames.Add("NoSoilDecayRedux"); cheater = true; }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool SprintAndDashIsLoaded = this.Helper.ModRegistry.IsLoaded("SPDSprintAndDash");
            if (SprintAndDashIsLoaded == true)
            { blockedModNames.Add("SprintAndDash"); cheater = true; }
            bool BetterSprinklersIsLoaded = this.Helper.ModRegistry.IsLoaded("Speeder.BetterSprinklers");
            if (BetterSprinklersIsLoaded == true)
            { blockedModNames.Add("BetterSprinklers"); cheater = true; }
            bool LineSprinklersIsLoaded = this.Helper.ModRegistry.IsLoaded("hootless.LineSprinklers");
            if (LineSprinklersIsLoaded == true)
            { blockedModNames.Add("LineSprinklers"); cheater = true; }
            bool JASprinklersIsLoaded = this.Helper.ModRegistry.IsLoaded("hootless.JASprinklers");
            if (JASprinklersIsLoaded == true)
            { blockedModNames.Add("LineSprinklers"); cheater = true; }



        }





        private void InputEvents_ButtonPressed(object sender, EventArgs e)
        {
            if (cheater == true)
            {
                Game1.activeClickableMenu.exitThisMenu(true);
                this.Monitor.Log("Blocked Mods Detected:", LogLevel.Warn);
                foreach (string o in blockedModNames)
                {
                    this.Monitor.Log(o, LogLevel.Warn);
                }
                
            }
            else
            {
                InputEvents.ButtonPressed -= this.InputEvents_ButtonPressed;
            }
        }
        private void GameEvents_HalfSecondTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is TitleMenu)
            {
                messageSent = false;
            }


            if (!Context.IsWorldReady)
                return;

            halfsecondTicks += 1;
            if (halfsecondTicks >= 30 && messageSent == false && cheater == false)
            {
                var playerID = Game1.player.UniqueMultiplayerID;

                Game1.chatBox.addInfoMessage($"{introMessage}");
                Game1.displayHUD = true;
                Game1.addHUDMessage(new HUDMessage($"{introMessage}", ""));

                //old way of sending ids
                /*Game1.chatBox.activate();
                Game1.chatBox.setText("/color");
                Game1.chatBox.chatBox.RecieveCommandInput('\r');
                Game1.chatBox.activate();
                Game1.chatBox.setText($"{currentPass}{playerID}"); 
                Game1.chatBox.chatBox.RecieveCommandInput('\r');*/

                string myStringMessage = $"{currentPass}{playerID}";
                Game1.client.sendMessage((byte)18, myStringMessage);




                //delete last line so we dont see ID
                List<ChatMessage> messages = this.Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
                int currentMessage = messages.Count - 1;
                messages.RemoveAt(currentMessage);

                messageSent = true;
            }
        }
        private void GameEvents_FourthUpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (preMessageDeleteMessageSent == false)
            {
                var playerID = Game1.player.UniqueMultiplayerID;
                Game1.chatBox.activate();
                Game1.chatBox.setText("/color");
                Game1.chatBox.chatBox.RecieveCommandInput('\r');
                Game1.chatBox.activate();
                Game1.chatBox.setText("ServerCode Sent");
                Game1.chatBox.chatBox.RecieveCommandInput('\r');
                string myStringMessage = $"{currentPass}{playerID}";
                Game1.client.sendMessage((byte)18, myStringMessage);
                preMessageDeleteMessageSent = true;
            }

            List<ChatMessage> messages = this.Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
            string[] messageDumpString = messages.SelectMany(p => p.message).Select(p => p.message).ToArray();
            string lastFragment = messageDumpString.LastOrDefault()?.Split(':').Last().Trim();

            if (!String.IsNullOrWhiteSpace(lastFragment) && lastFragment.Length >= 4 && lastFragment.Substring(0, 4) == $"{currentPass}") 
            {
                int currentMessage = messages.Count - 1;
                messages.RemoveAt(currentMessage);
            }

        }

        //fix social tab
        private void PreRenderGuiEvent(object sender, EventArgs e)
        {

            if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.currentTab == GameMenu.socialTab)
            {
                List<IClickableMenu> tabs = this.Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();
                SocialPage socialPage = (SocialPage)tabs[gameMenu.currentTab];
                IReflectedField<int> numFarmers = this.Helper.Reflection.GetField<int>(socialPage, "numFarmers");
                numFarmers.SetValue(1);

            }
        }


        // shipping menu"OK" click through code
        private void Shipping_Menu(object sender, EventArgs e)
        {

            this.Monitor.Log("This is the Shipping Menu");
            this.Helper.Reflection.GetMethod(Game1.activeClickableMenu, "okClicked").Invoke();

        }

        /*private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {

 
            if (!Context.IsWorldReady)
                return;

            secondTicks += 1;

                if (sendID == true && secondTicks <= 3)
                {

                    
                    var playerID = Game1.player.UniqueMultiplayerID;
                    
                    //send Messages
                    PyNet.sendMessage("anticheat.3.7", playerID);
                    this.Monitor.Log("ID Sent:" + playerID.ToString());
                }
                else if (sendID == false)
                {
                    Game1.chatBox.addInfoMessage("Cheat Mods Detected!");
                    Game1.displayHUD = true;
                    Game1.addHUDMessage(new HUDMessage("Cheat Mods Detected!", ""));
                }*/






















        /* doesn't work for now, no timer in the ship menu, maybe figure out later////////////////////////////////////////////////////////////////////////////////////////////////////
                // timer for shipping ok click delay
                private void MultiplayerEvents_BeforeMainSync(object sender, EventArgs e)
                {
                    if (!Context.IsMultiplayer) return;

                    if (shipTicker == true)
                    {
                        shipTicks += 1;

                    }

                    if (shipTicker == false)
                    {
                        shipTicks = 0;
                    }

                }



                // shipping menu"OK" click through code
                private void Shipping_Menu(object sender, EventArgs e)
                {
                    shipTicker = true;

                    if (shipTicks == 1800)
                    {
                        this.Monitor.Log("This is the Shipping Menu");
                        this.Helper.Reflection.GetMethod(Game1.activeClickableMenu, "okClicked").Invoke();
                        shipTicker = false;
                    }

                }


        *//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        // Holiday code
        private void TimeEvents_TimeOfDayChanged(object sender, EventArgs e)
        {
            gameClockTicks += 1;
            

            if (cheater == true)
            {
                cheatClockTicks += 1;
                Game1.chatBox.addInfoMessage("Blocked Mods Detected!");
                Game1.displayHUD = true;
                Game1.addHUDMessage(new HUDMessage("Blocked Mods Detected!", ""));
                if (cheatClockTicks == 6)
                {
                    Game1.exitToTitle = true;
                    cheatClockTicks = 0;
                }
            }




            //Game1.player.hasRustyKey = true;  //wallet addition


            this.numPlayers = Game1.otherFarmers.Count;

            


            if (gameClockTicks >= 1)
            {
                var currentTime = Game1.timeOfDay;
                var currentDate = SDate.Now();
                var eggFestival = new SDate(13, "spring");
                var dayAfterEggFestival = new SDate(14, "spring");
                var flowerDance = new SDate(24, "spring");
                var luau = new SDate(11, "summer");
                var danceOfJellies = new SDate(28, "summer");
                var stardewValleyFair = new SDate(16, "fall");
                var spiritsEve = new SDate(27, "fall");
                var festivalOfIce = new SDate(8, "winter");
                var feastOfWinterStar = new SDate(25, "winter");



                if (currentDate == eggFestival && numPlayers >= 1)   //set back to 1 after testing~!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    EggFestival();
                }


                //flower dance
                else if (currentDate == flowerDance && numPlayers >= 1)
                {
                     FlowerDance();
                }

                else if (currentDate == luau && numPlayers >= 1)
                {
                    Luau();
                }

                else if (currentDate == danceOfJellies && numPlayers >= 1)
                {
                    DanceOfTheMoonlightJellies();
                }

                else if (currentDate == stardewValleyFair && numPlayers >= 1)
                {
                    StardewValleyFair();
                }

                else if (currentDate == spiritsEve && numPlayers >= 1)
                {
                    SpiritsEve();
                }

                else if (currentDate == festivalOfIce && numPlayers >= 1)
                {
                    FestivalOfIce();
                }

                else if (currentDate == feastOfWinterStar && numPlayers >= 1)
                {
                    FeastOfWinterStar();
                }



                gameClockTicks = 0;   // never reaches rest of code bc gameClockTicks is reset to 0, these methods below are called higher up.




                void EggFestival()
                {
                    if (currentTime >= 900 && currentTime <= 1400)
                    {



                        //teleports to egg festival
                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Town", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                // flower dance turned off causes game crashes
                void FlowerDance()
                {
                    if (currentTime >= 900 && currentTime <= 1400)
                    {

                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Forest", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void Luau()
                {

                    if (currentTime >= 900 && currentTime <= 1400)
                    {

                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Beach", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void DanceOfTheMoonlightJellies()
                {



                    if (currentTime >= 2200 && currentTime <= 2400)
                    {


                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Beach", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void StardewValleyFair()
                {
                    if (currentTime >= 900 && currentTime <= 1500)
                    {



                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Town", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void SpiritsEve()
                {


                    if (currentTime >= 2200 && currentTime <= 2350)
                    {



                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Town", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void FestivalOfIce()
                {
                    if (currentTime >= 900 && currentTime <= 1400)
                    {


                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Forest", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void FeastOfWinterStar()
                {
                    if (currentTime >= 900 && currentTime <= 1400)
                    {


                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Town", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

            }


        }
        




    }
}