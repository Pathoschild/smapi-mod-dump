/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.GameModifications.CodeInjections.Modded;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications.Modded
{
    public class ModRandomizedLogicPatcher
    {
        private readonly Harmony _harmony;
        private readonly ArchipelagoClient _archipelago;
        private readonly StardewItemManager _stardewItemManager;
        private JunimoShopStockModifier _junimoShopStockModifier;
        private IModHelper _modHelper;

        public ModRandomizedLogicPatcher(IMonitor monitor, IModHelper modHelper, Harmony harmony, ArchipelagoClient archipelago, SeedShopStockModifier seedShopStockModifier, StardewItemManager stardewItemManager)
        {
            _harmony = harmony;
            _archipelago = archipelago;
            _stardewItemManager = stardewItemManager;
            _modHelper = modHelper;
            if (_archipelago.SlotData.Mods.HasMod(ModNames.SVE))
            {
                _junimoShopStockModifier = new JunimoShopStockModifier(monitor, modHelper, archipelago, _stardewItemManager);
            }
        }

        public void PatchAllGameLogic()
        {
            if (_archipelago.SlotData.Mods.HasMod(ModNames.SVE))
            {
                _modHelper.Events.Content.AssetRequested += _junimoShopStockModifier.OnShopStockRequested;
            }
        }

        public void CleanEvents()
        {
            if (_archipelago.SlotData.Mods.HasMod(ModNames.SVE))
            {
                _modHelper.Events.Content.AssetRequested -= _junimoShopStockModifier.OnShopStockRequested;
            }
        }
    }
}
