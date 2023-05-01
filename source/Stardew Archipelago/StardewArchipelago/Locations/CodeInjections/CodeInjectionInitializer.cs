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
using StardewArchipelago.Locations.Festival;
using StardewArchipelago.Locations.GingerIsland.Boat;
using StardewModdingAPI;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Locations.CodeInjections
{
    public static class CodeInjectionInitializer
    {
        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, BundleReader bundleReader, LocationChecker locationChecker, StardewItemManager itemManager)
        {
            var shopReplacer = new ShopReplacer(monitor, modHelper, archipelago, locationChecker);
            BackpackInjections.Initialize(monitor, archipelago, locationChecker);
            ToolInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            ScytheInjections.Initialize(monitor, locationChecker);
            FishingRodInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CommunityCenterInjections.Initialize(monitor, bundleReader, locationChecker);
            MineshaftInjections.Initialize(monitor, archipelago, locationChecker);
            SkillInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            QuestInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            CarpenterInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            WizardInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            IsolatedEventInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            AdventurerGuildInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            ArcadeMachineInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            TravelingMerchantInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FishingInjections.Initialize(monitor, modHelper, archipelago, locationChecker, itemManager);
            MuseumInjections.Initialize(monitor, modHelper, archipelago, locationChecker, itemManager);
            FriendshipInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            SpecialOrderInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            InitializeFestivalPatches(monitor, modHelper, archipelago, locationChecker, shopReplacer);
        }

        private static void InitializeFestivalPatches(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago,
            LocationChecker locationChecker, ShopReplacer shopReplacer)
        {
            EggFestivalInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FlowerDanceInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            LuauInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            MoonlightJelliesInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            FairInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            SpiritEveInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            IceFestivalInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            MermaidHouseInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            BeachNightMarketInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
            WinterStarInjections.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer);
        }
    }
}
