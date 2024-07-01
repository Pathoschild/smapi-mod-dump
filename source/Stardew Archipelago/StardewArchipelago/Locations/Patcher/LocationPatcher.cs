/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Bundles;
using StardewArchipelago.GameModifications;
using StardewArchipelago.GameModifications.Modded;
using StardewArchipelago.Locations.CodeInjections.Initializers;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Serialization;
using StardewArchipelago.Stardew;
using StardewModdingAPI;

namespace StardewArchipelago.Locations.Patcher
{
    public class LocationPatcher
    {
        private List<ILocationPatcher> _patchers;

        public LocationPatcher(IMonitor monitor, IModHelper modHelper, ModConfig config, Harmony harmony, ArchipelagoClient archipelago, ArchipelagoStateDto state,
            LocationChecker locationChecker, StardewItemManager itemManager, WeaponsManager weaponsManager, BundlesManager bundlesManager,
            SeedShopStockModifier seedShopStockModifier, Friends friends)
        {
            CodeInjectionInitializer.Initialize(monitor, modHelper, config, archipelago, state, locationChecker, itemManager, weaponsManager, bundlesManager, seedShopStockModifier, friends);
            _patchers = new List<ILocationPatcher>();
            _patchers.Add(new VanillaLocationPatcher(monitor, modHelper, harmony, archipelago, locationChecker, itemManager));
            if (archipelago.SlotData.Mods.IsModded)
            {
                _patchers.Add(new ModLocationPatcher(harmony, monitor, modHelper, archipelago, itemManager));
            }
        }

        public void ReplaceAllLocationsRewardsWithChecks()
        {
            foreach (var patcher in _patchers)
            {
                patcher.ReplaceAllLocationsRewardsWithChecks();
            }
        }

        public void CleanEvents()
        {
            foreach (var patcher in _patchers)
            {
                patcher.CleanEvents();
            }
        }
    }
}
