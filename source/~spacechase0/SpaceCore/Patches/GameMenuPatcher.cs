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
using StardewValley.Menus;

namespace SpaceCore.Patches
{
    /// <summary>Applies Harmony patches to <see cref="GameMenu"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class GameMenuPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<GameMenu>(nameof(GameMenu.getTabNumberFromName)),
                postfix: this.GetHarmonyMethod(nameof(After_GetTabNumberFromName))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after <see cref="GameMenu.getTabNumberFromName"/>.</summary>
        public static void After_GetTabNumberFromName(GameMenu __instance, string name, ref int __result)
        {
            foreach (var tab in Menus.ExtraGameMenuTabs)
            {
                if (name == tab.Value)
                    __result = tab.Key;
            }
        }
    }
}
