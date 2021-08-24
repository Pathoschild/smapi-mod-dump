/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using HarmonyLib;
using XSAutomate.Common.Patches;
using StardewModdingAPI;

namespace ExpandedStorage.Framework.Patches
{
    internal class ChestsAnywherePatches : BasePatch<ExpandedStorage>
    {
        private const string ShippingBinContainerType = "Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ShippingBinContainer";

        public ChestsAnywherePatches(IMod mod, Harmony harmony) : base(mod, harmony)
        {
            if (!Mod.Helper.ModRegistry.IsLoaded("Pathoschild.ChestsAnywhere")) return;
            Monitor.LogOnce("Patching Chests Anywhere for Refreshing Shipping Bin");
            harmony.Patch(
                new AssemblyPatch("ChestsAnywhere").Method(ShippingBinContainerType, "GrabItemFromContainerImpl"),
                postfix: new HarmonyMethod(GetType(), nameof(GrabItemFromContainerImplPostfix))
            );
        }

        private static void GrabItemFromContainerImplPostfix()
        {
            Mod.ActiveMenu.Value.RefreshItems();
        }
    }
}