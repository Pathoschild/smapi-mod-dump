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
using StardewValley;
using StardewValley.Tools;

namespace MoreRings.Patches
{
    /// <summary>Applies Harmony patches to <see cref="WateringCan"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class WateringCanPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<WateringCan>(nameof(WateringCan.DoFunction)),
                prefix: this.GetHarmonyMethod(nameof(Before_DoFunction), priority: Priority.First)
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="WateringCan.DoFunction"/>.</summary>
        private static void Before_DoFunction(Hoe __instance, GameLocation location, ref int x, ref int y, int power, Farmer who)
        {
            if (Mod.Instance.HasRingEquipped(Mod.Instance.RingMageHand) > 0)
            {
                x = (int)who.lastClick.X;
                y = (int)who.lastClick.Y;
            }
        }
    }
}
