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
using DynamicGameAssets.Game;
using HarmonyLib;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley.Objects;

namespace DynamicGameAssets.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Boots"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class BootsPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Boots>("loadDisplayFields"),
                prefix: this.GetHarmonyMethod(nameof(Before_LoadDisplayFields))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Boots.loadDisplayFields"/>.</summary>
        /// <returns>Returns whether to run the original method.</returns>
        private static bool Before_LoadDisplayFields(Boots __instance, ref bool __result)
        {
            if (__instance is CustomBoots boots)
            {
                __result = boots.LoadDisplayFields();
                return false;
            }
            return true;
        }
    }
}
