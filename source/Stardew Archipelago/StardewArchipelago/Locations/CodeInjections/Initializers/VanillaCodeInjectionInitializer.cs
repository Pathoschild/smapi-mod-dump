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
using StardewArchipelago.Goals;
using StardewArchipelago.Locations.Festival;
using StardewModdingAPI;
using StardewArchipelago.Stardew;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.CodeInjections.Vanilla.CC;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Quests;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Serialization;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;

namespace StardewArchipelago.Locations.CodeInjections.Initializers
{
    public static class VanillaCodeInjectionInitializer
    {
        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ArchipelagoStateDto state, LocationChecker locationChecker, StardewItemManager itemManager, WeaponsManager weaponsManager, ShopReplacer shopReplacer, Friends friends)
        {
            BackpackInjections.Initialize(monitor, archipelago, locationChecker);
            ToolInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            ScytheInjections.Initialize(monitor, locationChecker);
            FishingRodInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            var bundleReader = new BundleReader();
            GoalCodeInjection.Initialize(monitor, modHelper, archipelago, locationChecker, bundleReader);
            CommunityCenterInjections.Initialize(monitor, archipelago, locationChecker, bundleReader);
            JunimoNoteMenuInjections.Initialize(monitor, modHelper, archipelago, state, locationChecker, bundleReader);
            MineshaftInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            SkillInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            QuestInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            DarkTalismanInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CarpenterInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            WizardInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            IsolatedEventInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            AdventurerGuildInjections.Initialize(monitor, modHelper, archipelago, locationChecker, weaponsManager);
            ArcadeMachineInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            TravelingMerchantInjections.Initialize(monitor, modHelper, archipelago, locationChecker, state);
            FishingInjections.Initialize(monitor, modHelper, archipelago, locationChecker, itemManager);
            MuseumInjections.Initialize(monitor, modHelper, archipelago, locationChecker, itemManager);
            FriendshipInjections.Initialize(monitor, modHelper, archipelago, locationChecker, friends, itemManager);
            SpecialOrderInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            SpouseInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            PregnancyInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CropsanityInjections.Initialize(monitor, archipelago, locationChecker, itemManager);
            InitializeFestivalPatches(monitor, modHelper, archipelago, state, locationChecker, shopReplacer);
            MonsterSlayerInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CookingInjections.Initialize(monitor, archipelago, locationChecker, itemManager);
            QueenOfSauceInjections.Initialize(monitor, modHelper, archipelago, state, locationChecker, itemManager);
            RecipePurchaseInjections.Initialize(monitor, modHelper, archipelago, locationChecker, itemManager);
            RecipeLevelUpInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            RecipeFriendshipInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CraftingInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            KrobusShopInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FarmCaveInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
        }

        private static void InitializeFestivalPatches(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ArchipelagoStateDto state, LocationChecker locationChecker, ShopReplacer shopReplacer)
        {
            EggFestivalInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FlowerDanceInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            LuauInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            MoonlightJelliesInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            FairInjections.Initialize(monitor, modHelper, archipelago, state, locationChecker, shopReplacer);
            SpiritEveInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            IceFestivalInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            MermaidHouseInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            BeachNightMarketInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            WinterStarInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
        }
    }
}
