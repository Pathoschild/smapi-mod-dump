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
using Harmony;
using Spacechase.Shared.Harmony;
using SpaceCore.Events;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Events;

namespace SpaceCore.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Utility"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class UtilityPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Utility>(nameof(Utility.pickFarmEvent)),
                postfix: this.GetHarmonyMethod(nameof(After_PickFarmEvent))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after <see cref="Utility.pickFarmEvent"/>.</summary>
        private static void After_PickFarmEvent(ref FarmEvent __result)
        {
            __result = SpaceEvents.InvokeChooseNightlyFarmEvent(__result);
        }
    }
}
