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
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.GameModifications;
using StardewArchipelago.GameModifications.CodeInjections.Modded;
using StardewArchipelago.GameModifications.Modded;
using StardewArchipelago.Locations.CodeInjections.Modded;
using StardewArchipelago.Locations.CodeInjections.Modded.SVE;
using StardewModdingAPI;

namespace StardewArchipelago.Locations.CodeInjections.Initializers
{
    public static class ModCodeInjectionInitializer
    {
        static ArchipelagoClient _archipelago;
        private const string BEAR_KNOWLEDGE = "Bear's Knowledge";
        private const int OATMEAL_PRICE = 12500;
        private const int COOKIE_PRICE = 8750;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, SeedShopStockModifier seedShopStockModifier)
        {
            _archipelago = archipelago;
            InitializeModdedContent(monitor, modHelper, archipelago, locationChecker, seedShopStockModifier);
        }

        private static void InitializeModdedContent(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, SeedShopStockModifier seedShopStockModifier)
        {
            if (_archipelago.SlotData.Mods.HasMod(ModNames.DEEP_WOODS))
            {
                DeepWoodsModInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            }
            if (_archipelago.SlotData.Mods.HasMod(ModNames.MAGIC))
            {
                MagicModInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            }
            if (_archipelago.SlotData.Mods.HasMod(ModNames.ARCHAEOLOGY))
            {
                ArchaeologyConfigCodeInjections.Initialize(monitor, modHelper, archipelago);
            }
            if (_archipelago.SlotData.Mods.HasMod(ModNames.SKULL_CAVERN_ELEVATOR))
            {
                SkullCavernInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            }
            if (archipelago.SlotData.Mods.HasMod(ModNames.SVE))
            {
                SVECutsceneInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            }

            if (archipelago.SlotData.Mods.HasMod(ModNames.DISTANT_LANDS))
            {
                ModdedEventInjections.Initialize(monitor, modHelper, archipelago, locationChecker);
            }
            if (archipelago.SlotData.Mods.HasMod(ModNames.BOARDING_HOUSE))
            {
                BoardingHouseInjections.Initialize(monitor, locationChecker, archipelago);
            }
        }
    }
}
