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
using StardewValley.Tools;

namespace DynamicGameAssets.Patches
{
    /// <summary>Applies Harmony patches to <see cref="MeleeWeapon"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class MeleeWeaponPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.RecalculateAppliedForges)),
                prefix: this.GetHarmonyMethod(nameof(Before_RecalculateAppliedForges))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="MeleeWeapon.RecalculateAppliedForges"/>.</summary>
        /// <returns>Returns whether to run the original method.</returns>
        private static bool Before_RecalculateAppliedForges(MeleeWeapon __instance, bool force)
        {
            if (__instance is CustomMeleeWeapon weapon)
            {
                weapon.RecalculateAppliedForges(force);
                return false;
            }

            return true;
        }
    }
}
