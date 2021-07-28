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
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace CapstoneProfessions.Patches
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
                original: this.RequireMethod<SObject>("getPriceAfterMultipliers"),
                postfix: this.GetHarmonyMethod(nameof(After_GetPriceAfterMultipliers))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after <see cref="SObject.getPriceAfterMultipliers"/>.</summary>
        private static void After_GetPriceAfterMultipliers(ref float __result)
        {
            float mult = 1;
            foreach (var player in Game1.getAllFarmers())
            {
                if (player.professions.Contains(Mod.ProfessionProfit))
                {
                    mult += 0.05f;
                }
            }
            __result *= mult;
        }
    }
}
