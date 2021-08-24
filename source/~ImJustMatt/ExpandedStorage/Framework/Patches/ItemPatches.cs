/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using ExpandedStorage.Framework.Controllers;
using HarmonyLib;
using XSAutomate.Common.Patches;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ExpandedStorage.Framework.Patches
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ItemPatches : BasePatch<ExpandedStorage>
    {
        public ItemPatches(IMod mod, Harmony harmony) : base(mod, harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Item), nameof(Item.canStackWith), new[] {typeof(ISalable)}),
                postfix: new HarmonyMethod(GetType(), nameof(CanStackWithPostfix))
            );
        }

        private static void CanStackWithPostfix(Item __instance, ref bool __result, ISalable other)
        {
            if (__instance is Chest
                || other is Chest
                || Mod.AssetController.TryGetStorage(__instance, out var storage)
                && (storage.Config.Option("CanCarry", true) == StorageConfigController.Choice.Enable
                    || storage.Config.Option("AccessCarried", true) != StorageConfigController.Choice.Enable))
            {
                __result = false;
            }
        }
    }
}