/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using Unlockable_Bundles.Lib.MapFeatures;

namespace Unlockable_Bundles.Lib
{
    public class Main
    {
        public static void Initialize()
        {
            ShopTypes.Main.Initialize();
            AdvancedPricing.Main.Initialize();
            MapFeatures.Main.Initialize();
            WalletCurrency.Main.Initialize();

            ConsoleCommands.Initialize();
            AssetRequested.Initialize();
            SaveDataEvents.Initialize();
            ShopObject.Initialize();
            ShopPlacement.Initialize();
            MapPatches.Initialize();
            UtilityMisc.Initialize();
            UnsafeMap.Initialize();
            UBEvent.Initialize();
            _InventoryPage.Initialize();
            _GameLocation.Initialize();
            BundleOverviewMenu.Initialize();
            PlacementRequirement.Initialize();
            Multiplayer.Initialize();
        }
    }
}
