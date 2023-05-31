/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Linq;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Locations.CodeInjections.Initializers
{
    public static class CodeInjectionInitializer
    {
        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, BundleReader bundleReader, LocationChecker locationChecker, StardewItemManager itemManager)
        {
            VanillaCodeInjectionInitializer.Initialize(monitor, modHelper, archipelago, bundleReader, locationChecker, itemManager);
            if (archipelago.SlotData.Mods.IsModded)
            {
                ModCodeInjectionInitializer.Initialize(monitor, modHelper, archipelago, locationChecker);
            }
        }
    }
}
