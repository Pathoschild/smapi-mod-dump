/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Spacechase.Shared.Patching;
using SpaceCore.Events;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;

namespace SpaceCore.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Farmer"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class FarmerPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Farmer>(nameof(Farmer.doneEating)),
                postfix: this.GetHarmonyMethod(nameof(After_DoneEating))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after <see cref="Farmer.doneEating"/>.</summary>
        public static void After_DoneEating(Farmer __instance)
        {
            if (__instance.itemToEat == null)
                return;
            SpaceEvents.InvokeOnItemEaten(__instance);
        }
    }
}
