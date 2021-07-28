/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Harmony;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace CarryChest.Patches
{
    /// <summary>Applies Harmony patches to <see cref="SObject"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class ObjectPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<SObject>(nameof(SObject.getDescription)),
                postfix: this.GetHarmonyMethod(nameof(After_GetDescription))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="SObject.getDescription"/>.</summary>
        private static void After_GetDescription(SObject __instance, ref string __result)
        {
            if (__instance.ParentSheetIndex == 130)
            {
                var chest = __instance as Chest;
                __result += "\n" + $"Contains {chest?.items?.Count ?? 0} items.";
            }
        }
    }
}
