/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Archipelago;
using StardewArchipelago.Bundles;
using StardewArchipelago.GameModifications.CodeInjections.Television;
using StardewArchipelago.Goals;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Bundles;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Quests;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Walnutsanity;
using StardewArchipelago.Locations.Festival;
using StardewArchipelago.Serialization;
using StardewArchipelago.Stardew;
using StardewModdingAPI;

namespace StardewArchipelago.Locations.CodeInjections.Initializers
{
    public static class VanillaCodeInjectionInitializer
    {
        public static void Initialize(IMonitor monitor, IModHelper modHelper, ModConfig config, ArchipelagoClient archipelago, ArchipelagoStateDto state, LocationChecker locationChecker, StardewItemManager itemManager, WeaponsManager weaponsManager, BundlesManager bundlesManager, Friends friends)
        {
            BackpackInjections.Initialize(monitor, archipelago, locationChecker);
            ScytheInjections.Initialize(monitor, locationChecker);
            FishingRodInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CopperPanInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            var bundleReader = new BundleReader();
            var killList = new MonsterKillList(archipelago);
            GoalCodeInjection.Initialize(monitor, modHelper, archipelago, locationChecker, bundleReader, killList);
            InitializeBundleInjections(monitor, modHelper, archipelago, state, locationChecker, bundlesManager, bundleReader);
            MineshaftInjections.Initialize(monitor, modHelper, config, archipelago, locationChecker);
            InitializeSkills(monitor, modHelper, archipelago, locationChecker);
            QuestInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            DarkTalismanInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CarpenterInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            IsolatedEventInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            WizardBookInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            PhoneInjections.Initialize(monitor, modHelper, archipelago, weaponsManager);
            ArcadeMachineInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            TravelingMerchantInjections.Initialize(monitor, modHelper, archipelago, locationChecker, state);
            FishingInjections.Initialize(monitor, modHelper, archipelago, locationChecker, itemManager);
            MuseumInjections.Initialize(monitor, modHelper, archipelago, locationChecker, itemManager);
            FriendshipInjections.Initialize(monitor, modHelper, archipelago, locationChecker, friends, itemManager);
            SpecialOrderInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            SpouseInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            PregnancyInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CropsanityInjections.Initialize(monitor, archipelago, locationChecker, itemManager);
            InitializeFestivalPatches(monitor, modHelper, archipelago, state, locationChecker);
            MonsterSlayerInjections.Initialize(monitor, modHelper, archipelago, locationChecker, killList);
            CookingInjections.Initialize(monitor, archipelago, locationChecker, itemManager);
            var qosManager = new QueenOfSauceManager(state);
            QueenOfSauceInjections.Initialize(monitor, modHelper, archipelago, locationChecker, itemManager, qosManager);
            RecipeLevelUpInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            RecipeFriendshipInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CraftingInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FarmCaveInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FarmEventInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            BookInjections.Initialize(monitor, modHelper, archipelago, locationChecker, qosManager);
            InitializeWalnutsanityInjections(monitor, modHelper, archipelago, locationChecker);
        }

        private static void InitializeBundleInjections(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ArchipelagoStateDto state, LocationChecker locationChecker, BundlesManager bundlesManager, BundleReader bundleReader)
        {
            CommunityCenterInjections.Initialize(monitor, archipelago, locationChecker, bundleReader);
            JunimoNoteMenuInjections.Initialize(monitor, modHelper, archipelago, state, locationChecker, bundleReader);
            BundleInjections.Initialize(monitor, modHelper, archipelago, state, locationChecker, bundlesManager);
            RaccoonInjections.Initialize(monitor, modHelper, archipelago, state, locationChecker, bundlesManager);
        }

        private static void InitializeSkills(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            SkillInjections.Initialize(monitor, modHelper, archipelago, locationChecker);

            if (archipelago.SlotData.SkillProgression != SkillsProgression.ProgressiveWithMasteries)
            {
                return;
            }

            MasteriesInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
        }

        private static void InitializeFestivalPatches(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ArchipelagoStateDto state, LocationChecker locationChecker)
        {
            EggFestivalInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            DesertFestivalInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FlowerDanceInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            LuauInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            TroutDerbyInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            MoonlightJelliesInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FairInjections.Initialize(monitor, modHelper, archipelago, state, locationChecker);
            SpiritEveInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            IceFestivalInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            SquidFestInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            MermaidHouseInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            BeachNightMarketInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            WinterStarInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
        }

        private static void InitializeWalnutsanityInjections(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            WalnutPuzzleInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            WalnutBushInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            WalnutDigSpotsInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            WalnutRepeatablesInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
        }
    }
}
