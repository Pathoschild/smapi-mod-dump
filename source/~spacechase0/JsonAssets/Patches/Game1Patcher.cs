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

namespace JsonAssets.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Game1"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class Game1Patcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Game1>(nameof(Game1.loadForNewGame)),
                prefix: this.GetHarmonyMethod(nameof(Before_LoadForNewGame))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Game1.loadForNewGame"/>.</summary>
        private static void Before_LoadForNewGame()
        {
            Mod.instance.OnBlankSave();
        }
    }
}
