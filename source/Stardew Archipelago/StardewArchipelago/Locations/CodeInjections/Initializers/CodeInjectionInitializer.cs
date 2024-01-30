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
using StardewArchipelago.GameModifications;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Serialization;
using StardewModdingAPI;
using StardewArchipelago.Stardew;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.GameModifications.Modded;

namespace StardewArchipelago.Locations.CodeInjections.Initializers
{
    public static class CodeInjectionInitializer
    {
        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ArchipelagoStateDto state, LocationChecker locationChecker, StardewItemManager itemManager, WeaponsManager weaponsManager, ShopStockGenerator shopStockGenerator, JunimoShopGenerator junimoShopGenerator, Friends friends)
        {
            var shopReplacer = new ShopReplacer(monitor, modHelper, archipelago, locationChecker);
            VanillaCodeInjectionInitializer.Initialize(monitor, modHelper, archipelago, state, locationChecker, itemManager, weaponsManager, shopReplacer, friends);
            if (archipelago.SlotData.Mods.IsModded)
            {
                ModCodeInjectionInitializer.Initialize(monitor, modHelper, archipelago, locationChecker, shopReplacer, shopStockGenerator, junimoShopGenerator);
            }
        }
    }
}
