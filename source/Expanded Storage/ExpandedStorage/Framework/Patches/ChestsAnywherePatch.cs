/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System;
using System.Linq;
using Harmony;
using ImJustMatt.Common.PatternPatches;
using ImJustMatt.ExpandedStorage.Framework.UI;
using StardewModdingAPI;
using StardewValley;

namespace ImJustMatt.ExpandedStorage.Framework.Patches
{
    internal class ChestsAnywherePatch : Patch<ModConfig>
    {
        private readonly bool _isChestsAnywhereLoaded;
        private readonly Type _type;

        internal ChestsAnywherePatch(IMonitor monitor, ModConfig config, bool isChestsAnywhereLoaded)
            : base(monitor, config)
        {
            _isChestsAnywhereLoaded = isChestsAnywhereLoaded;

            if (!isChestsAnywhereLoaded)
                return;

            var chestsAnywhereAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName.StartsWith("ChestsAnywhere,"));
            _type = chestsAnywhereAssembly.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ShippingBinContainer");
        }

        protected internal override void Apply(HarmonyInstance harmony)
        {
            if (_isChestsAnywhereLoaded)
            {
                Monitor.Log("Patching Chests Anywhere for Refreshing Shipping Bin");
                var methodInfo = AccessTools.GetDeclaredMethods(_type)
                    .Find(m => m.Name.Equals("GrabItemFromContainerImpl", StringComparison.OrdinalIgnoreCase));
                harmony.Patch(methodInfo, postfix: new HarmonyMethod(GetType(), nameof(GrabItemFromContainerImplPostfix)));
            }
        }

        public static void GrabItemFromContainerImplPostfix(object __instance, Item item, Farmer player)
        {
            MenuViewModel.RefreshItems();
        }
    }
}