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
using Microsoft.Xna.Framework;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace DynamicGameAssets.Patches
{
    /// <summary>Applies Harmony patches to <see cref="FruitTree"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class FruitTreePatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<FruitTree>(nameof(FruitTree.shake)),
                prefix: this.GetHarmonyMethod(nameof(Before_Shake))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="FruitTree.shake"/>.</summary>
        /// <returns>Returns whether to run the original method.</returns>
        private static bool Before_Shake(FruitTree __instance, Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location)
        {
            if (__instance is CustomFruitTree tree)
            {
                tree.Shake(tileLocation, doEvenIfStillShaking, location);
                return false;
            }

            return true;
        }
    }
}
