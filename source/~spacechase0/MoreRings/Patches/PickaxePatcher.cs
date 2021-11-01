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
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace MoreRings.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Pickaxe"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class PickaxePatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Pickaxe>(nameof(Pickaxe.DoFunction)),
                prefix: this.GetHarmonyMethod(nameof(Before_DoFunction), priority: Priority.First)
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Pickaxe.DoFunction"/>.</summary>
        private static void Before_DoFunction(ref int x, ref int y, Farmer who)
        {
            if (Mod.Instance.HasRingEquipped(Mod.Instance.RingMageHand))
            {
                x = (int)who.lastClick.X;
                y = (int)who.lastClick.Y;
            }
        }
    }
}
