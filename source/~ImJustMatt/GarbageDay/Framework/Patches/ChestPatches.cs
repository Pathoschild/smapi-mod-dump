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
using System.Linq;
using HarmonyLib;
using XSAutomate.Common.Patches;
using StardewModdingAPI;
using StardewValley.Objects;

namespace GarbageDay.Framework.Patches
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ChestPatches : BasePatch<GarbageDay>
    {
        public ChestPatches(IMod mod, Harmony harmony) : base(mod, harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Chest), nameof(Chest.ShowMenu)),
                new HarmonyMethod(GetType(), nameof(ShowMenuPrefix))
            );
        }

        /// <summary>Produce chest interactions on show menu</summary>
        private static bool ShowMenuPrefix(Chest __instance)
        {
            if (!__instance.modData.ContainsKey("furyx639.GarbageDay")) return true;
            var garbageCan = GarbageDay.GarbageCans.Values.SingleOrDefault(gc => ReferenceEquals(gc.Chest, __instance));
            return garbageCan?.OpenCan() ?? true;
        }
    }
}